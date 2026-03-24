using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using tSHess.Engine;
using tSHess.UI;

namespace tSHess
{
    class Program
    {
        internal enum GameMode
        {
            HumanVsHuman,
            HumanVsComputer,
            ComputerVsComputer
        }

        internal enum PlayerKind
        {
            Human,
            Computer
        }

        internal enum EngineKind
        {
            MTD,
            PVS
        }

        internal enum TurnAction
        {
            PlayMove,
            Continue,
            SwitchMode,
            SwitchEngine,
            NewGame,
            Quit
        }

        internal enum AutoplayAction
        {
            Continue,
            SwitchMode,
            SwitchEngine,
            NewGame,
            Quit,
            EnableFast,
            DisableFast
        }

        internal static bool IsEvaluationCommand(string input)
        {
            string normalized = input == null ? "" : input.Trim().ToLowerInvariant();
            return normalized == "eval" || normalized == "evaluation";
        }

        internal static bool TryParseEvaluationCommand(string input, Color sideToMove, out Color perspective)
        {
            perspective = sideToMove;
            string normalized = input == null ? "" : input.Trim().ToLowerInvariant();
            if (normalized.Length == 0)
                return false;

            string[] parts = normalized.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
                return false;

            if (parts[0] != "eval" && parts[0] != "evaluation")
                return false;

            if (parts.Length == 1)
            {
                perspective = sideToMove;
                return true;
            }

            if (parts.Length == 2)
            {
                if (parts[1] == "white" || parts[1] == "w")
                {
                    perspective = Color.White;
                    return true;
                }

                if (parts[1] == "black" || parts[1] == "b")
                {
                    perspective = Color.Black;
                    return true;
                }
            }

            return false;
        }

        public static void OnPromotePawn(object obj, PromotePawnEventArgs args)
        {
            Console.WriteLine("-> Pawn promoted");
        }

        public static void OnPieceHit(object obj, PieceHitEventArgs args)
        {
            Console.WriteLine("-> Piece hit");
        }

        public static void OnCheck(object obj, CheckEventArgs args)
        {
            Console.WriteLine("-> Check");
        }

        public static void OnGameOver(object obj, GameOverEventArgs args)
        {
            Console.WriteLine("-> Game over");
        }

        internal static bool TryParseGameMode(string input, out GameMode mode)
        {
            string normalized = input == null ? "" : input.Trim().ToLowerInvariant();
            switch (normalized)
            {
                case "1":
                case "hh":
                case "human-human":
                case "human vs human":
                case "humanvshuman":
                    mode = GameMode.HumanVsHuman;
                    return true;
                case "2":
                case "hc":
                case "human-computer":
                case "human vs computer":
                case "humanvscomputer":
                    mode = GameMode.HumanVsComputer;
                    return true;
                case "3":
                case "cc":
                case "computer-computer":
                case "computer vs computer":
                case "computervscomputer":
                    mode = GameMode.ComputerVsComputer;
                    return true;
                default:
                    mode = GameMode.HumanVsComputer;
                    return false;
            }
        }

        internal static void ResolvePlayers(GameMode mode, Color humanColor, out PlayerKind whitePlayer, out PlayerKind blackPlayer)
        {
            whitePlayer = PlayerKind.Human;
            blackPlayer = PlayerKind.Human;

            if (mode == GameMode.HumanVsComputer)
            {
                if (humanColor == Color.White)
                    blackPlayer = PlayerKind.Computer;
                else
                    whitePlayer = PlayerKind.Computer;
            }
            else if (mode == GameMode.ComputerVsComputer)
            {
                whitePlayer = PlayerKind.Computer;
                blackPlayer = PlayerKind.Computer;
            }
        }

        internal static bool TryParseTurnAction(string input, out TurnAction action)
        {
            string normalized = input == null ? "" : input.Trim().ToLowerInvariant();
            switch (normalized)
            {
                case "continue":
                    action = TurnAction.Continue;
                    return true;
                case "mode":
                    action = TurnAction.SwitchMode;
                    return true;
                case "engine":
                    action = TurnAction.SwitchEngine;
                    return true;
                case "new":
                    action = TurnAction.NewGame;
                    return true;
                case "quit":
                    action = TurnAction.Quit;
                    return true;
                default:
                    action = TurnAction.PlayMove;
                    return false;
            }
        }

        internal static bool TryParseAutoplayAction(string input, out AutoplayAction action)
        {
            string normalized = input == null ? "" : input.Trim().ToLowerInvariant();
            switch (normalized)
            {
                case "":
                case "continue":
                    action = AutoplayAction.Continue;
                    return true;
                case "mode":
                    action = AutoplayAction.SwitchMode;
                    return true;
                case "engine":
                    action = AutoplayAction.SwitchEngine;
                    return true;
                case "new":
                    action = AutoplayAction.NewGame;
                    return true;
                case "quit":
                    action = AutoplayAction.Quit;
                    return true;
                case "auto on":
                    action = AutoplayAction.EnableFast;
                    return true;
                case "auto off":
                    action = AutoplayAction.DisableFast;
                    return true;
                default:
                    action = AutoplayAction.Continue;
                    return false;
            }
        }

        static SnapShot CreateNewSnapShot()
        {
            SnapShot s = SnapShot.StartUpSnapShot();

            s.PromotePawnEventHandler += new PromotePawnEventHandler(Program.OnPromotePawn);
            s.PieceHitEventHandler += new PieceHitEventHandler(Program.OnPieceHit);
            s.CheckEventHandler += new CheckEventHandler(Program.OnCheck);
            s.GameOverEventHandler += new GameOverEventHandler(Program.OnGameOver);

            return s;
        }

        static void ConfigureMode(ref GameMode mode, ref Color humanColor, out PlayerKind whitePlayer, out PlayerKind blackPlayer)
        {
            mode = PromptGameMode();
            if (mode == GameMode.HumanVsComputer)
                humanColor = PromptHumanColor();

            ResolvePlayers(mode, humanColor, out whitePlayer, out blackPlayer);
        }

        static GameMode PromptGameMode()
        {
            while (true)
            {
                Console.WriteLine("Choose mode:");
                Console.WriteLine("  1 - Human vs Human");
                Console.WriteLine("  2 - Human vs Computer");
                Console.WriteLine("  3 - Computer vs Computer");
                Console.Write("Selection: ");

                if (TryParseGameMode(Console.ReadLine(), out GameMode mode))
                {
                    Console.WriteLine();
                    return mode;
                }

                Console.WriteLine("Unknown mode. Please choose 1, 2 or 3.");
                Console.WriteLine();
            }
        }

        static Color PromptHumanColor()
        {
            while (true)
            {
                Console.Write("Play as white or black? [w/b]: ");
                string input = Console.ReadLine();
                string normalized = input == null ? "" : input.Trim().ToLowerInvariant();
                if (normalized == "w" || normalized == "white")
                {
                    Console.WriteLine();
                    return Color.White;
                }
                if (normalized == "b" || normalized == "black")
                {
                    Console.WriteLine();
                    return Color.Black;
                }

                Console.WriteLine("Please enter 'w' or 'b'.");
            }
        }

        static EngineKind PromptEngineKind()
        {
            while (true)
            {
                Console.WriteLine("Choose engine:");
                Console.WriteLine("  1 - MTD");
                Console.WriteLine("  2 - PVS");
                Console.Write("Selection: ");

                string input = Console.ReadLine();
                string normalized = input == null ? "" : input.Trim().ToLowerInvariant();
                if (normalized == "1" || normalized == "mtd")
                {
                    Console.WriteLine();
                    return EngineKind.MTD;
                }
                if (normalized == "2" || normalized == "pvs")
                {
                    Console.WriteLine();
                    return EngineKind.PVS;
                }

                Console.WriteLine("Unknown engine. Please choose 1 or 2.");
                Console.WriteLine();
            }
        }

        static void ShowHumanOptions(string randomSan)
        {
            Console.WriteLine("Options:");
            Console.WriteLine("- Enter a move, e.g. e4, Nf3, O-O, exd6, axb8=Q.");
            Console.WriteLine("- Type moves to show legal moves.");
            Console.WriteLine("- Type hint to ask the engine for a suggested move.");
            Console.WriteLine("- Type eval to print evaluation breakdown for the side to move.");
            Console.WriteLine("- Type eval white or eval black to choose perspective.");
            Console.WriteLine("- Type mode to switch the current play mode.");
            Console.WriteLine("- Type engine to switch the engine (MTD/PVS).");
            Console.WriteLine("- Type new to start a new game.");
            Console.WriteLine("- Type help to show this list of options.");
            Console.WriteLine("- Type quit to leave the game.");
            Console.WriteLine("Example from this position: " + randomSan);
        }

        static void ShowAutoplayOptions(bool fastAutoplay)
        {
            Console.WriteLine("Autoplay options:");
            Console.WriteLine("- Press Enter to continue computer vs computer.");
            Console.WriteLine("- Type moves to show legal moves.");
            Console.WriteLine("- Type eval to print evaluation breakdown for the side to move.");
            Console.WriteLine("- Type eval white or eval black to choose perspective.");
            Console.WriteLine("- Type mode to switch the current play mode.");
            Console.WriteLine("- Type engine to switch the engine (MTD/PVS).");
            Console.WriteLine("- Type new to start a new game.");
            Console.WriteLine("- Type auto on to enable fast autoplay checkpoints.");
            Console.WriteLine("- Type auto off to disable fast autoplay checkpoints.");
            Console.WriteLine("- Type quit to leave the game.");
            Console.WriteLine("Current autoplay speed: " + (fastAutoplay ? "fast" : "normal"));
        }

        static void ShowLegalMoves(SnapShot s)
        {
            Console.WriteLine("Legal moves:");
            Console.WriteLine(s.LegalMoves.ToSanString(s.Clone()));
        }

        internal static string FormatStatusLine(GameMode mode, PlayerKind whitePlayer, PlayerKind blackPlayer)
        {
            return "Mode: " + DescribeGameMode(mode) + " | White: " + DescribePlayerKind(whitePlayer) + " | Black: " + DescribePlayerKind(blackPlayer);
        }

        static string DescribeGameMode(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.HumanVsHuman:
                    return "Human vs Human";
                case GameMode.HumanVsComputer:
                    return "Human vs Computer";
                case GameMode.ComputerVsComputer:
                    return "Computer vs Computer";
                default:
                    return mode.ToString();
            }
        }

        static string DescribePlayerKind(PlayerKind playerKind)
        {
            switch (playerKind)
            {
                case PlayerKind.Human:
                    return "Human";
                case PlayerKind.Computer:
                    return "Computer";
                default:
                    return playerKind.ToString();
            }
        }

        static string DescribeEngineKind(EngineKind engine)
        {
            switch (engine)
            {
                case EngineKind.MTD:
                    return "MTD";
                default:
                    return engine.ToString();
            }
        }

        internal static string FormatStatusLine(GameMode mode, PlayerKind whitePlayer, PlayerKind blackPlayer, EngineKind engine)
        {
            return "Mode: " + DescribeGameMode(mode) + " | White: " + DescribePlayerKind(whitePlayer) + " | Black: " + DescribePlayerKind(blackPlayer) + " | Engine: " + DescribeEngineKind(engine);
        }

        static void ShowGameState(SnapShot s, bool useUnicodeBoard, GameMode mode, PlayerKind whitePlayer, PlayerKind blackPlayer, EngineKind engine)
        {
            Console.WriteLine(FormatStatusLine(mode, whitePlayer, blackPlayer, engine));
            Console.WriteLine(s.GetCapturedPiecesSummary(useUnicodeBoard));
            string historySan = s.GetHistorySanString(includeMoveNumbers: true);
            Console.WriteLine("Moves: " + (string.IsNullOrEmpty(historySan) ? "-" : historySan));
            Console.WriteLine(s.ToGameOutputString(useUnicode: useUnicodeBoard, includeLegalMoves: false, includeHistory: false));
        }

        static string DescribeMove(SnapShot s, Move move)
        {
            if (move == null)
                return "<no move>";

            try
            {
                return s.MoveToSan(move);
            }
            catch
            {
                return move.ToString();
            }
        }

        internal static string FormatGameResult(SnapShot s)
        {
            if (s == null)
                return "Game result: Unknown.";

            return FormatGameResult(s.CurrentSituationCode, s.WhoToMove, s.LegalMoves.Count);
        }

        internal static string FormatGameResult(SituationCode currentSituationCode, Color whoToMove, int legalMovesCount)
        {
            if (currentSituationCode == SituationCode.Stalemate)
                return "Game result: Draw by stalemate.";

            if (currentSituationCode == SituationCode.Checkmate)
            {
                string winner = whoToMove == Color.White ? "Black" : "White";
                return "Game result: " + winner + " wins by checkmate.";
            }

            if (legalMovesCount == 0)
                return "Game result: Draw.";

            return "Game result: Ongoing.";
        }

        static Move GetEngineMove(SnapShot s, string openingBookSan, EngineKind engine)
        {
            Console.WriteLine();
            Console.WriteLine("Thinking...");
            Console.WriteLine();
            if (engine == EngineKind.PVS)
                return s.GetBestMovePVS(openingBookSan, OpeningBookFormat.SanNotation);

            return s.GetBestMoveMTD(openingBookSan, OpeningBookFormat.SanNotation);
        }

        static TurnAction PromptHumanTurn(SnapShot s, string openingBookSan, EngineKind engine, out Move move)
        {
            move = null;

            while (true)
            {
                MoveList legalMoves = s.LegalMoves;
                string randomSan = s.MoveToSan(legalMoves[Helper.RandomNumber(legalMoves.Count)]);

                Console.WriteLine();
                Console.Write((s.WhoToMove == Color.White ? "White" : "Black") + " to move");
                Console.WriteLine(" (e.g. " + randomSan + "):");

                string moveString = Console.ReadLine();
                if (moveString == null)
                {
                    Console.WriteLine("Illegal move, try again.");
                    continue;
                }

                moveString = moveString.Trim();
                string command = moveString.ToLowerInvariant();

                if (TryParseTurnAction(command, out TurnAction action) && action != TurnAction.Continue)
                {
                    if (action == TurnAction.Quit)
                        Console.WriteLine("bye-bye...");
                    return action;
                }

                if (command == "help")
                {
                    ShowHumanOptions(randomSan);
                    continue;
                }

                if (command == "moves")
                {
                    ShowLegalMoves(s);
                    continue;
                }

                if (command == "hint")
                {
                    Move hintMove = GetEngineMove(s, openingBookSan, engine);
                    Console.WriteLine("Suggestion: " + DescribeMove(s, hintMove));
                    continue;
                }

                if (TryParseEvaluationCommand(command,s.WhoToMove,out Color perspective))
                {
                    Console.WriteLine(s.GetEvaluationBreakdownString(perspective));
                    continue;
                }

                bool parsed = false;
                if (!parsed)
                {
                    try
                    {
                        move = s.SanToMove(moveString);
                        parsed = true;
                    }
                    catch
                    {
                    }
                }

                if (parsed)
                    return TurnAction.PlayMove;

                Console.WriteLine("Illegal move, try again.");
            }
        }

        static AutoplayAction PromptAutoplayTurnAction(SnapShot s, bool fastAutoplay)
        {
            while (true)
            {
                Console.Write("Autoplay command (Enter=continue, mode/new/auto on/auto off/eval [white|black]/quit/help): ");
                string input = Console.ReadLine();
                string normalized = input == null ? "" : input.Trim().ToLowerInvariant();

                if (normalized == "help")
                {
                    ShowAutoplayOptions(fastAutoplay);
                    continue;
                }

                if (normalized == "moves")
                {
                    ShowLegalMoves(s);
                    continue;
                }

                if (TryParseEvaluationCommand(normalized,s.WhoToMove,out Color perspective))
                {
                    Console.WriteLine(s.GetEvaluationBreakdownString(perspective));
                    continue;
                }

                if (TryParseAutoplayAction(normalized, out AutoplayAction action))
                    return action;

                Console.WriteLine("Unknown command. Type help for options.");
            }
        }

        static void ValidateOpeningsFile(string fileName, string reportFile, OpeningBookFormat format)
        {
            var errors = OpeningBook.Validate(fileName, format);
            if (errors == null || errors.Count == 0)
            {
                Console.WriteLine("Validation: OK - no errors found in " + fileName);
                File.WriteAllText(reportFile, "Validation: OK - no errors found in " + fileName + Environment.NewLine);
            }
            else
            {
                Console.WriteLine("Validation found " + errors.Count + " error(s):");
                using (StreamWriter sw = new StreamWriter(reportFile, false))
                {
                    sw.WriteLine("Validation found " + errors.Count + " error(s):");
                    foreach (var e in errors)
                    {
                        Console.WriteLine(e);
                        sw.WriteLine(e);
                    }
                }
            }
        }

        ///// <summary>
        ///// The main entry point for the application.
        ///// </summary>
        //[STAThread]
        //static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1());
        //}

        [STAThread]
        static void Main(string[] args)
        {
            // Application.SetHighDpiMode(System.Windows.Forms.HighDpiMode.SystemAware);
            // Application.EnableVisualStyles();
            // Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new Board());

            SnapShot s = CreateNewSnapShot();

            string openingBook = "openings.txt";
            string openingBookSan = "openings-san.txt";
            bool useUnicodeBoard = true;

            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string a = args[i] == null ? "" : args[i].Trim().ToLower();
                    if (a == "--ascii")
                        useUnicodeBoard = false;
                    else if (a == "--unicode")
                        useUnicodeBoard = true;
                }
            }

            // CLI helper: validate SAN openings file and exit
            if (args != null && args.Length > 0 && args[0].ToLower() == "validate-openings-san")
            {
                ValidateOpeningsFile(openingBookSan, $"validation-report-{openingBookSan}", OpeningBookFormat.SanNotation);
                return;
            }

            // CLI helper: validate openings file and exit
            if (args != null && args.Length > 0 && args[0].ToLower() == "validate-openings")
            {
                ValidateOpeningsFile(openingBook, $"validation-report-{openingBook}", OpeningBookFormat.CoordinateNotation);
                return;
            }

            Console.WriteLine("Welcom to tSHess!\n*****************\n\n");
            GameMode mode = GameMode.HumanVsComputer;
            EngineKind engine = EngineKind.MTD;
            Color humanColor = Color.White;
            bool fastAutoplay = false;
            const int fastAutoplayCheckpointMoves = 10;
            int movesUntilAutoplayCheckpoint = fastAutoplayCheckpointMoves;
            ConfigureMode(ref mode, ref humanColor, out PlayerKind whitePlayer, out PlayerKind blackPlayer);
            ShowGameState(s, useUnicodeBoard, mode, whitePlayer, blackPlayer, engine);

            while (true)
            {
                if (s.LegalMoves.Count == 0)
                {
                    Console.WriteLine(FormatGameResult(s));
                    break;
                }
                Move m = null;
                PlayerKind currentPlayer = s.WhoToMove == Color.White ? whitePlayer : blackPlayer;

                if (currentPlayer == PlayerKind.Human)
                {
                    TurnAction action = PromptHumanTurn(s, openingBookSan, engine, out m);
                    if (action == TurnAction.Quit)
                        return;
                    if (action == TurnAction.SwitchMode)
                    {
                        ConfigureMode(ref mode, ref humanColor, out whitePlayer, out blackPlayer);
                        ShowGameState(s, useUnicodeBoard, mode, whitePlayer, blackPlayer, engine);
                        continue;
                    }
                    if (action == TurnAction.SwitchEngine)
                    {
                        engine = PromptEngineKind();
                        ShowGameState(s, useUnicodeBoard, mode, whitePlayer, blackPlayer, engine);
                        continue;
                    }
                    if (action == TurnAction.NewGame)
                    {
                        s = CreateNewSnapShot();
                        ShowGameState(s, useUnicodeBoard, mode, whitePlayer, blackPlayer, engine);
                        continue;
                    }
                }
                else
                {
                    Color movingSide = s.WhoToMove;
                    m = GetEngineMove(s, openingBookSan, engine);
                    Console.WriteLine((movingSide == Color.White ? "White" : "Black") + " plays " + DescribeMove(s, m) + ".");
                }

                Console.WriteLine();
                s.PerformMove(m);
                ShowGameState(s, useUnicodeBoard, mode, whitePlayer, blackPlayer, engine);

                if (mode == GameMode.ComputerVsComputer && s.LegalMoves.Count > 0)
                {
                    bool promptNow = !fastAutoplay;
                    if (fastAutoplay)
                    {
                        movesUntilAutoplayCheckpoint--;
                        if (movesUntilAutoplayCheckpoint <= 0)
                        {
                            promptNow = true;
                            Console.WriteLine("Fast autoplay checkpoint reached.");
                        }
                    }

                    if (!promptNow)
                        continue;

                    movesUntilAutoplayCheckpoint = fastAutoplayCheckpointMoves;
                    AutoplayAction action = PromptAutoplayTurnAction(s, fastAutoplay);
                    if (action == AutoplayAction.Quit)
                    {
                        Console.WriteLine("bye-bye...");
                        return;
                    }
                    if (action == AutoplayAction.SwitchMode)
                    {
                        ConfigureMode(ref mode, ref humanColor, out whitePlayer, out blackPlayer);
                        movesUntilAutoplayCheckpoint = fastAutoplayCheckpointMoves;
                        ShowGameState(s, useUnicodeBoard, mode, whitePlayer, blackPlayer, engine);
                        continue;
                    }
                    if (action == AutoplayAction.SwitchEngine)
                    {
                        engine = PromptEngineKind();
                        ShowGameState(s, useUnicodeBoard, mode, whitePlayer, blackPlayer, engine);
                        continue;
                    }
                    if (action == AutoplayAction.NewGame)
                    {
                        s = CreateNewSnapShot();
                        movesUntilAutoplayCheckpoint = fastAutoplayCheckpointMoves;
                        ShowGameState(s, useUnicodeBoard, mode, whitePlayer, blackPlayer, engine);
                        continue;
                    }
                    if (action == AutoplayAction.EnableFast)
                    {
                        fastAutoplay = true;
                        movesUntilAutoplayCheckpoint = fastAutoplayCheckpointMoves;
                        Console.WriteLine("Fast autoplay enabled (checkpoint every " + fastAutoplayCheckpointMoves + " moves).");
                        continue;
                    }
                    if (action == AutoplayAction.DisableFast)
                    {
                        fastAutoplay = false;
                        movesUntilAutoplayCheckpoint = fastAutoplayCheckpointMoves;
                        Console.WriteLine("Fast autoplay disabled.");
                        continue;
                    }
                }
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
