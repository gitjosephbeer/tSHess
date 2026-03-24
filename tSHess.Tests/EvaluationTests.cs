using tSHess.Engine;
using Xunit;

namespace tSHess.Tests
{
    public class EvaluationTests
    {
        [Fact]
        public void EvaluateMaterial_StartupPosition_IsZeroForBothSides()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();

            Assert.Equal(0, snapshot.EvaluateMaterial(Color.White));
            Assert.Equal(0, snapshot.EvaluateMaterial(Color.Black));
        }

        [Fact]
        public void EvaluateMaterial_AfterSymmetricMoves_RemainsZero()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("e5"));

            Assert.Equal(0, snapshot.EvaluateMaterial(Color.White));
            Assert.Equal(0, snapshot.EvaluateMaterial(Color.Black));
        }

        [Fact]
        public void EvaluateMaterial_AfterWinningPawn_IsPositiveForWinningSide()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("d5"));
            snapshot.PerformMove(snapshot.SanToMove("exd5"));

            Assert.True(snapshot.EvaluateMaterial(Color.White) > 0);
            Assert.True(snapshot.EvaluateMaterial(Color.Black) < 0);
        }

        [Fact]
        public void EvaluateComplete_StartupPosition_IsConsistentAcrossPerspective()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            int white = snapshot.EvaluateComplete(Color.White);
            int black = snapshot.EvaluateComplete(Color.Black);

            Assert.Equal(white, black);
        }

        [Fact]
        public void EvaluateComplete_AfterWinningPawn_FavorsWinningSide()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("d5"));
            snapshot.PerformMove(snapshot.SanToMove("exd5"));

            Assert.True(snapshot.EvaluateComplete(Color.White) > snapshot.EvaluateComplete(Color.Black));
        }

        [Fact]
        public void FromFen_StartupPosition_HasBalancedMaterial()
        {
            SnapShot snapshot = SnapShot.FromFen("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1");

            Assert.Equal(0, snapshot.EvaluateMaterial(Color.White));
            Assert.Equal(0, snapshot.EvaluateMaterial(Color.Black));
        }

        [Fact]
        public void GetBestMoveMTD_StalematePosition_ReturnsDrawEvaluation()
        {
            SnapShot snapshot = SnapShot.FromFen("7k/5Q2/7K/8/8/8/8/8 b - - 0 1");
            Move bestMove = snapshot.GetBestMoveMTD("openings.txt", OpeningBookFormat.CoordinateNotation);

            Assert.NotNull(bestMove);
            Assert.Equal(0, bestMove.Evaluation);
        }

        [Fact]
        public void GetBestMoveMTD_CheckmatedSideToMove_ReturnsLargeNegativeEvaluation()
        {
            SnapShot snapshot = SnapShot.FromFen("7k/6Q1/6K1/8/8/8/8/8 b - - 0 1");
            Move bestMove = snapshot.GetBestMoveMTD("openings.txt", OpeningBookFormat.CoordinateNotation);

            Assert.NotNull(bestMove);
            Assert.True(bestMove.Evaluation <= -29000);
        }

        [Fact]
        public void EvaluateComplete_EndgameKingCentralization_IsPreferred()
        {
            SnapShot centralKing = SnapShot.FromFen("7k/7p/8/8/4K3/8/P7/8 w - - 0 1");
            SnapShot cornerKing = SnapShot.FromFen("7k/7p/8/8/8/8/P7/K7 w - - 0 1");

            Assert.True(centralKing.EvaluateComplete(Color.White) > cornerKing.EvaluateComplete(Color.White));
        }

        [Fact]
        public void EvaluateComplete_EndgameAdvancedPassedPawn_IsPreferred()
        {
            SnapShot advancedPawn = SnapShot.FromFen("7k/8/4P3/8/4K3/8/8/8 w - - 0 1");
            SnapShot backwardPawn = SnapShot.FromFen("7k/8/8/8/4K3/4P3/8/8 w - - 0 1");

            Assert.True(advancedPawn.EvaluateComplete(Color.White) > backwardPawn.EvaluateComplete(Color.White));
        }

        [Fact]
        public void EvaluateComplete_QueenlessPosition_ReducesCastlingPressure()
        {
            SnapShot withQueens = SnapShot.FromFen("r2qk2r/8/8/8/8/8/8/R2QK2R w - - 0 1");
            SnapShot queenless = SnapShot.FromFen("r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1");

            Assert.True(queenless.EvaluateComplete(Color.White) > withQueens.EvaluateComplete(Color.White));
        }

        [Fact]
        public void IsDrawByInsufficientMaterial_KingVersusKing_IsTrue()
        {
            SnapShot snapshot = SnapShot.FromFen("7k/8/8/8/8/8/8/7K w - - 0 1");

            Assert.True(snapshot.IsDrawByInsufficientMaterial());
            Assert.Equal(0, snapshot.EvaluateComplete(Color.White));
        }

        [Fact]
        public void IsDrawByInsufficientMaterial_KingAndBishopVersusKing_IsTrue()
        {
            SnapShot snapshot = SnapShot.FromFen("7k/8/8/8/8/8/6B1/7K w - - 0 1");

            Assert.True(snapshot.IsDrawByInsufficientMaterial());
        }

        [Fact]
        public void IsDrawByInsufficientMaterial_KingBishopKnightVersusKing_IsFalse()
        {
            SnapShot snapshot = SnapShot.FromFen("7k/8/8/8/8/8/6BN/7K w - - 0 1");

            Assert.False(snapshot.IsDrawByInsufficientMaterial());
        }

        [Fact]
        public void IsDrawByRepetition_ThreefoldSequence_IsTrue()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("Nf3"));
            snapshot.PerformMove(snapshot.SanToMove("Nf6"));
            snapshot.PerformMove(snapshot.SanToMove("Ng1"));
            snapshot.PerformMove(snapshot.SanToMove("Ng8"));
            snapshot.PerformMove(snapshot.SanToMove("Nf3"));
            snapshot.PerformMove(snapshot.SanToMove("Nf6"));
            snapshot.PerformMove(snapshot.SanToMove("Ng1"));
            snapshot.PerformMove(snapshot.SanToMove("Ng8"));

            Assert.True(snapshot.IsDrawByRepetition());
        }

        [Fact]
        public void IsDrawByFiftyMoveRule_LessThanHundredMoves_IsFalse()
        {
            // Fewer than 100 half-moves in history should return false
            SnapShot snapshot = SnapShot.FromFen("7k/r6r/8/8/8/8/R6R/7K w - - 0 1");
            
            // Make only a few non-pawn, non-capture moves
            snapshot.PerformMove(snapshot.SanToMove("Ra1"));
            snapshot.PerformMove(snapshot.SanToMove("Ra6"));
            
            Assert.False(snapshot.IsDrawByFiftyMoveRule());
        }

        [Fact]
        public void IsDrawByFiftyMoveRule_HundredConsecutiveNonCaptureNonPawnMoves_IsTrue()
        {
            // 100 consecutive non-capturing, non-pawn moves should trigger fifty-move rule
            SnapShot snapshot = SnapShot.FromFen("7k/r6r/8/8/8/8/R6R/7K w - - 0 1");
            
            // Make 100 non-pawn, non-capture moves by alternating rook moves along the a-file
            // Pattern: Ra1, Ra6, Ra2, Ra7 (repeats the 4-move pattern 25 times = 100 half-moves)
            var movePattern = new string[] { "Ra1", "Ra6", "Ra2", "Ra7" };
            
            for (int i = 0; i < 25; i++)  // 25 * 4 moves = 100 half-moves
            {
                foreach (var move in movePattern)
                {
                    snapshot.PerformMove(snapshot.SanToMove(move));
                }
            }
            
            Assert.True(snapshot.IsDrawByFiftyMoveRule());
        }

        [Fact]
        public void IsDrawByFiftyMoveRule_PawnMoveBreaksCounter_IsFalse()
        {
            // A pawn move should reset the fifty-move counter
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            
            // Make a pawn move (e4), which resets the counter
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            
            // After moving a pawn, the counter is reset and we cannot have a fifty-move draw without 100 more moves
            Assert.False(snapshot.IsDrawByFiftyMoveRule());
        }

        [Fact]
        public void EvaluateSituation_KnightOnOutpost_ScoresHigherThanKnightOnHomeRank()
        {
            // White knight on e5 (rank 5, no black pawn on d6 or f6 to attack it) is an outpost.
            // Same material, knight on e2 (rank 2) is not an outpost.
            // Use a middlegame position (rooks present) so endgamePhase is low and the outpost bonus is not scaled away.
            SnapShot outpost    = SnapShot.FromFen("r3k2r/1p4p1/8/4N3/8/8/1P4P1/R3K2R w KQkq - 0 1");
            SnapShot noOutpost  = SnapShot.FromFen("r3k2r/1p4p1/8/8/8/4N3/1P4P1/R3K2R w KQkq - 0 1");

            Assert.True(outpost.EvaluateComplete(Color.White) > noOutpost.EvaluateComplete(Color.White));
        }

        [Fact]
        public void EvaluateSituation_BishopOnOpenDiagonals_ScoresHigherThanBishopBlocked()
        {
            // Bishop on c4 (light square) has 11 open diagonal squares (22 pts mobility, 0 own-pawn-color penalty).
            // Bishop on a1 (dark square) has its only diagonal immediately blocked by own pawn on b2 (dark);
            // it scores 0 mobility and incurs an 8-pt own-pawn-color penalty.
            // Pawn on b2 (both) prevents draw-by-insufficient-material without affecting the c4 diagonals.
            SnapShot centralBishop = SnapShot.FromFen("4k3/1p6/8/8/2B5/8/1P6/4K3 w - - 0 1");
            SnapShot cornerBishop  = SnapShot.FromFen("4k3/1p6/8/8/8/8/1P6/B3K3 w - - 0 1");

              int centralScore = centralBishop.EvaluateComplete(Color.White);
              int cornerScore  = cornerBishop.EvaluateComplete(Color.White);
              Assert.True(centralScore > cornerScore, $"central={centralScore} corner={cornerScore}");
        }

        [Fact]
        public void EvaluateSituation_MinorOnPawnWeakSquare_ScoresHigherThanAttackedSquare()
        {
            // Same material in both positions. White bishop sits on d5 in both.
            // Left position: c7 pawn does not attack d5.
            // Right position: c6 pawn attacks d5, removing weak-square occupation bonus.
            SnapShot weakSquare = SnapShot.FromFen("4k3/p1p5/8/3B4/8/8/P1P5/4K3 w - - 0 1");
            SnapShot attacked   = SnapShot.FromFen("4k3/p7/2p5/3B4/8/8/P1P5/4K3 w - - 0 1");

            Assert.True(weakSquare.EvaluateComplete(Color.White) > attacked.EvaluateComplete(Color.White));
        }

        [Fact]
        public void EvaluateSituation_AdvancedPawnsCreateMoreSpaceThanBackPawns()
        {
            // Same material and similar pawn structure. White c/f pawns are advanced in one position.
            // Rooks are included so middlegame-weighted space scoring remains active (not tapered to zero).
            SnapShot advancedSpace = SnapShot.FromFen("r3k2r/p6p/2p2p2/8/2P2P2/8/P6P/R3K2R w KQkq - 0 1");
            SnapShot crampedSpace  = SnapShot.FromFen("r3k2r/p6p/2p2p2/8/8/8/P1P2P1P/R3K2R w KQkq - 0 1");

            int advancedScore = advancedSpace.EvaluateComplete(Color.White);
            int crampedScore = crampedSpace.EvaluateComplete(Color.White);
            Assert.True(advancedScore > crampedScore, $"advanced={advancedScore} cramped={crampedScore}");
        }

        [Fact]
        public void EvaluateSituation_KnightBlockadingPawn_ScoresHigherThanNonBlockadingKnight()
        {
            // Same material and same pawn structure.
            // In the first position, white knight on d5 blockades black pawn on d6.
            // In the second, the knight is on f5 and does not blockade that pawn.
            SnapShot blockadingKnight = SnapShot.FromFen("4k3/p7/3p4/3N4/8/8/P7/4K3 w - - 0 1");
            SnapShot freeKnight       = SnapShot.FromFen("4k3/p7/3p4/5N2/8/8/P7/4K3 w - - 0 1");

            Assert.True(blockadingKnight.EvaluateComplete(Color.White) > freeKnight.EvaluateComplete(Color.White));
        }

        [Fact]
        public void EvaluateSituation_KeySquareCoordination_ScoresHigherWithMultiAttack()
        {
            // In both positions, white bishop on c3 attacks e5.
            // Coordinated case adds a white knight on f3, also attacking e5.
            // Uncoordinated case places the knight on h3, not attacking e5.
            SnapShot coordinated   = SnapShot.FromFen("4k3/p7/8/8/8/2B2N2/P7/4K3 w - - 0 1");
            SnapShot uncoordinated = SnapShot.FromFen("4k3/p7/8/8/8/2B4N/P7/4K3 w - - 0 1");

            Assert.True(coordinated.EvaluateComplete(Color.White) > uncoordinated.EvaluateComplete(Color.White));
        }

        [Fact]
        public void EvaluateSituation_BishopPairWithOpponentColorComplexImbalance_ScoresHigher()
        {
            // White has bishop pair in both positions.
            // In imbalanced case, black pawns are concentrated on one color complex (c6,e6,g6).
            // In balanced case, black pawns are distributed across both colors (c6,e5,g6).
            SnapShot imbalanced = SnapShot.FromFen("4k3/p7/2p1p1p1/8/8/2B5/P3B3/4K3 w - - 0 1");
            SnapShot balanced   = SnapShot.FromFen("4k3/p7/2p3p1/4p3/8/2B5/P3B3/4K3 w - - 0 1");

            int imbalancedScore = imbalanced.EvaluateComplete(Color.White);
            int balancedScore = balanced.EvaluateComplete(Color.White);
            Assert.True(imbalancedScore > balancedScore, $"imbalanced={imbalancedScore} balanced={balancedScore}");
        }

        [Fact]
        public void EvaluateSituation_HeavyPieceInEnemyCamp_ScoresHigherThanBackRankPlacement()
        {
            // Same material and pawn structure.
            // White queen on e6 (enemy camp) should score higher than queen on e2.
            // Keep black queen fixed on c7 in both positions to preserve material balance.
            SnapShot penetrating = SnapShot.FromFen("7k/2q4p/4Q3/8/8/8/P6P/7K w - - 0 1");
            SnapShot passive     = SnapShot.FromFen("7k/2q4p/8/8/8/8/P3Q2P/7K w - - 0 1");

            int penetratingScore = penetrating.EvaluateComplete(Color.White);
            int passiveScore = passive.EvaluateComplete(Color.White);
            Assert.True(penetratingScore > passiveScore, $"penetrating={penetratingScore} passive={passiveScore}");
        }

        [Fact]
        public void EvaluateSituation_WeakSquareBonus_IsSymmetricAcrossColors()
        {
            SnapShot whiteMotif = SnapShot.FromFen("4k3/p7/8/3B4/8/8/P7/4K3 w - - 0 1");
            SnapShot blackMotif = SnapShot.FromFen("4k3/p7/8/8/3b4/8/P7/4K3 b - - 0 1");

            Assert.Equal(whiteMotif.EvaluateComplete(Color.White),blackMotif.EvaluateComplete(Color.Black));
        }

        [Fact]
        public void EvaluateSituation_SpaceControl_IsSymmetricAcrossColors()
        {
            SnapShot symmetricSpace = SnapShot.FromFen("r3k2r/p6p/2p2p2/8/2P2P2/8/P6P/R3K2R w KQkq - 0 1");

            Assert.Equal(symmetricSpace.EvaluateComplete(Color.White),symmetricSpace.EvaluateComplete(Color.Black));
        }

        [Fact]
        public void EvaluateSituation_BlockadeBonus_IsSymmetricAcrossColors()
        {
            SnapShot symmetricBlockade = SnapShot.FromFen("4k3/p7/3p4/3N4/3n4/3P4/P7/4K3 w - - 0 1");

            Assert.Equal(symmetricBlockade.EvaluateComplete(Color.White),symmetricBlockade.EvaluateComplete(Color.Black));
        }

        [Fact]
        public void EvaluateSituation_KeySquareCoordination_IsSymmetricAcrossColors()
        {
            SnapShot symmetricCoordination = SnapShot.FromFen("4k3/p7/2b2n2/8/8/2B2N2/P7/4K3 w - - 0 1");

            Assert.Equal(symmetricCoordination.EvaluateComplete(Color.White),symmetricCoordination.EvaluateComplete(Color.Black));
        }

        [Fact]
        public void EvaluateSituation_BishopPairPressure_IsSymmetricAcrossColors()
        {
            SnapShot symmetricBishopPairs = SnapShot.FromFen("4k3/p1b3b1/2p1p1p1/8/8/2P1P1P1/P1B3B1/4K3 w - - 0 1");

            Assert.Equal(symmetricBishopPairs.EvaluateComplete(Color.White),symmetricBishopPairs.EvaluateComplete(Color.Black));
        }

        [Fact]
        public void EvaluateSituation_HeavyPiecePenetration_IsSymmetricAcrossColors()
        {
            SnapShot symmetricPenetration = SnapShot.FromFen("4k3/p6p/3Q4/8/8/3q4/P6P/4K3 w - - 0 1");

            Assert.Equal(symmetricPenetration.EvaluateComplete(Color.White),symmetricPenetration.EvaluateComplete(Color.Black));
        }

        [Fact]
        public void GetEvaluationBreakdown_TotalMatchesEvaluateComplete()
        {
            SnapShot snapshot = SnapShot.FromFen("r3k2r/p6p/2p2p2/8/2P2P2/8/P6P/R3K2R w KQkq - 0 1");

            SnapShot.EvaluationBreakdown breakdown = snapshot.GetEvaluationBreakdown(Color.White);
            Assert.False(breakdown.IsDrawnPosition);
            Assert.Equal(snapshot.EvaluateComplete(Color.White),breakdown.TotalScore);
            Assert.Equal(breakdown.PositionalScore, breakdown.TrackedHeuristicsScore + breakdown.ResidualPositionalScore);
        }

        [Fact]
        public void GetEvaluationBreakdown_DrawPosition_ReturnsZeroAndDrawFlag()
        {
            SnapShot snapshot = SnapShot.FromFen("7k/8/8/8/8/8/8/7K w - - 0 1");

            SnapShot.EvaluationBreakdown breakdown = snapshot.GetEvaluationBreakdown(Color.White);
            Assert.True(breakdown.IsDrawnPosition);
            Assert.Equal(0, breakdown.TotalScore);
            Assert.Equal(0, breakdown.MaterialScore);
            Assert.Equal(0, breakdown.PositionalScore);
        }

        [Fact]
        public void GetEvaluationBreakdownString_ContainsKeySections()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();

            string text = snapshot.GetEvaluationBreakdownString(Color.White);
            Assert.Contains("Evaluation breakdown:", text);
            Assert.Contains("Material:", text);
            Assert.Contains("Positional:", text);
            Assert.Contains("Total:", text);
        }
    }
}