using System;
using tSHess.Engine;
using Xunit;

namespace tSHess.Tests
{
    public class ProgramTests
    {
        [Theory]
        [InlineData("1", 0)]
        [InlineData("human vs human", 0)]
        [InlineData("2", 1)]
        [InlineData("HC", 1)]
        [InlineData("3", 2)]
        [InlineData("computer-computer", 2)]
        public void TryParseGameMode_RecognizesSupportedInputs(string input, int expected)
        {
            bool parsed = Program.TryParseGameMode(input, out Program.GameMode actual);

            Assert.True(parsed);
            Assert.Equal((Program.GameMode)expected, actual);
        }

        [Fact]
        public void TryParseGameMode_InvalidInput_ReturnsFalse()
        {
            bool parsed = Program.TryParseGameMode("bogus", out Program.GameMode actual);

            Assert.False(parsed);
            Assert.Equal(Program.GameMode.HumanVsComputer, actual);
        }

        [Fact]
        public void ResolvePlayers_HumanVsHuman_AssignsBothHumans()
        {
            Program.ResolvePlayers(Program.GameMode.HumanVsHuman, Color.White, out Program.PlayerKind whitePlayer, out Program.PlayerKind blackPlayer);

            Assert.Equal(Program.PlayerKind.Human, whitePlayer);
            Assert.Equal(Program.PlayerKind.Human, blackPlayer);
        }

        [Fact]
        public void ResolvePlayers_HumanVsComputer_UsesChosenHumanColor()
        {
            Program.ResolvePlayers(Program.GameMode.HumanVsComputer, Color.Black, out Program.PlayerKind whitePlayer, out Program.PlayerKind blackPlayer);

            Assert.Equal(Program.PlayerKind.Computer, whitePlayer);
            Assert.Equal(Program.PlayerKind.Human, blackPlayer);
        }

        [Fact]
        public void ResolvePlayers_ComputerVsComputer_AssignsBothComputers()
        {
            Program.ResolvePlayers(Program.GameMode.ComputerVsComputer, Color.White, out Program.PlayerKind whitePlayer, out Program.PlayerKind blackPlayer);

            Assert.Equal(Program.PlayerKind.Computer, whitePlayer);
            Assert.Equal(Program.PlayerKind.Computer, blackPlayer);
        }

        [Theory]
        [InlineData("mode", 2)]
        [InlineData("engine", 3)]
        [InlineData("new", 4)]
        [InlineData("continue", 1)]
        [InlineData("quit", 5)]
        public void TryParseTurnAction_RecognizesControlCommands(string input, int expected)
        {
            bool parsed = Program.TryParseTurnAction(input, out Program.TurnAction actual);

            Assert.True(parsed);
            Assert.Equal((Program.TurnAction)expected, actual);
        }

        [Fact]
        public void TryParseTurnAction_MoveText_ReturnsFalse()
        {
            bool parsed = Program.TryParseTurnAction("e4", out Program.TurnAction actual);

            Assert.False(parsed);
            Assert.Equal(Program.TurnAction.PlayMove, actual);
        }

        [Theory]
        [InlineData("switch mode")]
        [InlineData("new game")]
        [InlineData("resume")]
        [InlineData("exit")]
        public void TryParseTurnAction_Aliases_AreRejected(string input)
        {
            bool parsed = Program.TryParseTurnAction(input, out Program.TurnAction actual);

            Assert.False(parsed);
            Assert.Equal(Program.TurnAction.PlayMove, actual);
        }

        [Fact]
        public void FormatStatusLine_IncludesModeAndRoles()
        {
            string statusLine = Program.FormatStatusLine(Program.GameMode.HumanVsComputer, Program.PlayerKind.Human, Program.PlayerKind.Computer, Program.EngineKind.MTDv2);

            Assert.Equal("Mode: Human vs Computer | White: Human | Black: Computer | Engine: MTDv2", statusLine);
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("continue", 0)]
        [InlineData("mode", 1)]
        [InlineData("engine", 2)]
        [InlineData("new", 3)]
        [InlineData("quit", 4)]
        [InlineData("auto on", 5)]
        [InlineData("auto off", 6)]
        public void TryParseAutoplayAction_RecognizesCommands(string input, int expected)
        {
            bool parsed = Program.TryParseAutoplayAction(input, out Program.AutoplayAction actual);

            Assert.True(parsed);
            Assert.Equal((Program.AutoplayAction)expected, actual);
        }

        [Fact]
        public void TryParseAutoplayAction_UnknownCommand_ReturnsFalse()
        {
            bool parsed = Program.TryParseAutoplayAction("speed up", out Program.AutoplayAction actual);

            Assert.False(parsed);
            Assert.Equal(Program.AutoplayAction.Continue, actual);
        }

        [Theory]
        [InlineData("eval")]
        [InlineData("evaluation")]
        [InlineData(" EVAL ")]
        public void IsEvaluationCommand_RecognizesSupportedInputs(string input)
        {
            Assert.True(Program.IsEvaluationCommand(input));
        }

        [Theory]
        [InlineData("")]
        [InlineData("evaluate")]
        [InlineData("hint")]
        [InlineData("moves")]
        public void IsEvaluationCommand_RejectsUnsupportedInputs(string input)
        {
            Assert.False(Program.IsEvaluationCommand(input));
        }

        [Theory]
        [InlineData("eval", Color.White, Color.White)]
        [InlineData("evaluation", Color.Black, Color.Black)]
        [InlineData("eval white", Color.Black, Color.White)]
        [InlineData("eval black", Color.White, Color.Black)]
        [InlineData("eval w", Color.Black, Color.White)]
        [InlineData("evaluation b", Color.White, Color.Black)]
        public void TryParseEvaluationCommand_ParsesPerspective(string input, Color sideToMove, Color expectedPerspective)
        {
            bool parsed = Program.TryParseEvaluationCommand(input, sideToMove, out Color perspective);

            Assert.True(parsed);
            Assert.Equal(expectedPerspective, perspective);
        }

        [Theory]
        [InlineData("")]
        [InlineData("eval now")]
        [InlineData("eval white extra")]
        [InlineData("evaluation side")]
        [InlineData("hint")]
        public void TryParseEvaluationCommand_InvalidInput_ReturnsFalse(string input)
        {
            bool parsed = Program.TryParseEvaluationCommand(input, Color.White, out Color perspective);

            Assert.False(parsed);
            Assert.Equal(Color.White, perspective);
        }

        [Fact]
        public void FormatGameResult_Stalemate_IsDrawMessage()
        {
            string result = Program.FormatGameResult(SituationCode.Stalemate, Color.White, 0);

            Assert.Equal("Game result: Draw by stalemate.", result);
        }

        [Fact]
        public void FormatGameResult_Checkmate_AnnouncesWinner()
        {
            string result = Program.FormatGameResult(SituationCode.Checkmate, Color.White, 0);

            Assert.Equal("Game result: Black wins by checkmate.", result);
        }

        [Fact]
        public void FormatGameResult_NoLegalMovesWithoutMate_IsDraw()
        {
            string result = Program.FormatGameResult(SituationCode.Normal, Color.White, 0);

            Assert.Equal("Game result: Draw.", result);
        }

        [Fact]
        public void CapturedPiecesSummary_AtStart_ShowsNone()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();

            Assert.Equal("Captured by White: none | Captured by Black: none", snapshot.CapturedPiecesSummary);
        }

        [Fact]
        public void CapturedPiecesSummary_AfterPawnCapture_ShowsCounts()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("d5"));
            snapshot.PerformMove(snapshot.SanToMove("exd5"));

            Assert.Equal("Captured by White: Px1 | Captured by Black: none", snapshot.CapturedPiecesSummary);
        }

        [Fact]
        public void GetCapturedPiecesSummary_UnicodeMode_UsesPieceSymbols()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("d5"));
            snapshot.PerformMove(snapshot.SanToMove("exd5"));

            string summary = snapshot.GetCapturedPiecesSummary(true);
            Assert.Equal("Captured by White: ♙x1 | Captured by Black: none", summary);
        }

        [Fact]
        public void GetCapturedPiecesSummary_UsesStandardOrder_QRBNP()
        {
            string ordered = SnapShot.BuildCapturedPiecesStringForTests(
                pawns: 5,
                rooks: 2,
                knights: 3,
                bishops: 4,
                queens: 1,
                capturedPieceColor: Color.Black,
                useUnicode: false);

            Assert.Equal("Qx1 Rx2 Bx4 Nx3 Px5", ordered);

            int q = ordered.IndexOf("Qx1", StringComparison.Ordinal);
            int r = ordered.IndexOf("Rx2", StringComparison.Ordinal);
            int b = ordered.IndexOf("Bx4", StringComparison.Ordinal);
            int n = ordered.IndexOf("Nx3", StringComparison.Ordinal);
            int p = ordered.IndexOf("Px5", StringComparison.Ordinal);
            Assert.True(q < r && r < b && b < n && n < p, "Expected ordering Q-R-B-N-P.");
        }

        [Fact]
        public void ToGameOutputString_HidesSnapshotHeaderAndMoveMeta()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));

            string output = snapshot.ToGameOutputString(useUnicode: false, includeLegalMoves: false, includeHistory: false);
            Assert.DoesNotContain("tSHess Snapshot", output);
            Assert.DoesNotContain("To move:", output);
            Assert.DoesNotContain("Last move:", output);
        }

        [Fact]
        public void GetHistorySanString_FormatsCompleteHistoryInSan()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("e5"));
            snapshot.PerformMove(snapshot.SanToMove("Nf3"));

            string history = snapshot.GetHistorySanString(includeMoveNumbers: true);
            Assert.Equal("1. e4 e5 2. Nf3", history);
        }

        [Fact]
        public void BuildCapturedPiecesStringForTests_UnicodeWhitePawn_IsPieceSymbolNotDot()
        {
            string token = SnapShot.BuildCapturedPiecesStringForTests(
                pawns: 1,
                rooks: 0,
                knights: 0,
                bishops: 0,
                queens: 0,
                capturedPieceColor: Color.White,
                useUnicode: true);

            Assert.Equal("♟x1", token);
            Assert.DoesNotContain("·", token);
        }

        [Theory]
        [InlineData(Color.White, 1, 0, 0, 0, 0, "♟x1")]
        [InlineData(Color.White, 0, 1, 0, 0, 0, "♜x1")]
        [InlineData(Color.White, 0, 0, 1, 0, 0, "♞x1")]
        [InlineData(Color.White, 0, 0, 0, 1, 0, "♝x1")]
        [InlineData(Color.White, 0, 0, 0, 0, 1, "♛x1")]
        [InlineData(Color.Black, 1, 0, 0, 0, 0, "♙x1")]
        [InlineData(Color.Black, 0, 1, 0, 0, 0, "♖x1")]
        [InlineData(Color.Black, 0, 0, 1, 0, 0, "♘x1")]
        [InlineData(Color.Black, 0, 0, 0, 1, 0, "♗x1")]
        [InlineData(Color.Black, 0, 0, 0, 0, 1, "♕x1")]
        public void BuildCapturedPiecesStringForTests_Unicode_AllPiecesBothColors(
            Color capturedPieceColor,
            int pawns,
            int rooks,
            int knights,
            int bishops,
            int queens,
            string expectedToken)
        {
            string token = SnapShot.BuildCapturedPiecesStringForTests(
                pawns: pawns,
                rooks: rooks,
                knights: knights,
                bishops: bishops,
                queens: queens,
                capturedPieceColor: capturedPieceColor,
                useUnicode: true);

            Assert.Equal(expectedToken, token);
            Assert.DoesNotContain("·", token);
        }
    }
}