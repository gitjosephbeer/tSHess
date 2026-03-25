using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
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

        internal sealed class BenchmarkOptions
        {
            public EngineKind Engine = EngineKind.PVS;
            public int RunsPerPosition = 3;
            public bool UseOpeningBook = false;
            public int MaxIterationDepth = 8;
            public int MaxSearchSize = 20000;
            public bool Compare = false;
        }

        internal sealed class BenchmarkCase
        {
            public string Name;
            public string Fen;

            public BenchmarkCase(string name, string fen)
            {
                Name = name;
                Fen = fen;
            }
        }

        internal static bool TryParseEngineKind(string input, out EngineKind engine)
        {
            string normalized = input == null ? "" : input.Trim().ToLowerInvariant();
            switch (normalized)
            {
                case "1":
                case "mtd":
                    engine = EngineKind.MTD;
                    return true;
                case "2":
                case "pvs":
                    engine = EngineKind.PVS;
                    return true;
                default:
                    engine = EngineKind.PVS;
                    return false;
            }
        }

        internal static bool TryParseBenchmarkOptions(string[] args, out BenchmarkOptions options, out string error)
        {
            options = new BenchmarkOptions();
            error = "";

            if (args == null || args.Length == 0)
                return true;

            for (int i = 1; i < args.Length; i++)
            {
                string token = args[i] == null ? "" : args[i].Trim().ToLowerInvariant();
                if (token.Length == 0)
                    continue;

                if (token == "--engine")
                {
                    if (i + 1 >= args.Length)
                    {
                        error = "Missing value for --engine (expected: mtd or pvs).";
                        return false;
                    }

                    if (!TryParseEngineKind(args[i + 1], out EngineKind parsedEngine))
                    {
                        error = "Invalid engine '" + args[i + 1] + "' (expected: mtd or pvs).";
                        return false;
                    }

                    options.Engine = parsedEngine;
                    i++;
                    continue;
                }

                if (token == "--runs")
                {
                    if (i + 1 >= args.Length)
                    {
                        error = "Missing value for --runs (expected positive integer).";
                        return false;
                    }

                    if (!int.TryParse(args[i + 1], out int runs) || runs <= 0)
                    {
                        error = "Invalid --runs value '" + args[i + 1] + "' (expected positive integer).";
                        return false;
                    }

                    options.RunsPerPosition = runs;
                    i++;
                    continue;
                }

                if (token == "--depth")
                {
                    if (i + 1 >= args.Length)
                    {
                        error = "Missing value for --depth (expected positive integer).";
                        return false;
                    }

                    if (!int.TryParse(args[i + 1], out int depth) || depth <= 0)
                    {
                        error = "Invalid --depth value '" + args[i + 1] + "' (expected positive integer).";
                        return false;
                    }

                    options.MaxIterationDepth = depth;
                    i++;
                    continue;
                }

                if (token == "--nodes")
                {
                    if (i + 1 >= args.Length)
                    {
                        error = "Missing value for --nodes (expected positive integer).";
                        return false;
                    }

                    if (!int.TryParse(args[i + 1], out int nodes) || nodes <= 0)
                    {
                        error = "Invalid --nodes value '" + args[i + 1] + "' (expected positive integer).";
                        return false;
                    }

                    options.MaxSearchSize = nodes;
                    i++;
                    continue;
                }

                if (token == "--book" || token == "--use-book")
                {
                    options.UseOpeningBook = true;
                    continue;
                }

                if (token == "--no-book")
                {
                    options.UseOpeningBook = false;
                    continue;
                }

                if (token == "--compare")
                {
                    options.Compare = true;
                    continue;
                }

                error = "Unknown benchmark option '" + args[i] + "'.";
                return false;
            }

            return true;
        }

        static void ShowBenchmarkUsage()
        {
            Console.WriteLine("Benchmark mode usage:");
            Console.WriteLine("  tSHess benchmark [--engine mtd|pvs] [--runs N] [--depth N] [--nodes N] [--book|--no-book] [--compare]");
            Console.WriteLine("Defaults: --engine pvs --runs 3 --depth 8 --nodes 20000 --no-book --compare=false");
            Console.WriteLine("Examples:");
            Console.WriteLine("  tSHess benchmark");
            Console.WriteLine("  tSHess benchmark --engine pvs --runs 5 --depth 10 --nodes 40000");
            Console.WriteLine("  tSHess benchmark --compare                (compare MTD vs PVS on same suite)");
            Console.WriteLine("  tSHess benchmark --engine mtd --runs 3 --book");
        }

        static void RunBenchmarkSuite(BenchmarkOptions options, string openingBookSan)
        {
            if (options.Compare)
            {
                RunBenchmarkComparison(options, openingBookSan);
                return;
            }

            StringBuilder report = new StringBuilder();
            Action<string> writeLine = line =>
            {
                Console.WriteLine(line);
                report.AppendLine(line);
            };

            BenchmarkCase[] suite = new BenchmarkCase[]
            {
                new BenchmarkCase("Start Position", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1"),
                new BenchmarkCase("Open Sicilian", "r1bqk2r/pp2bppp/2n2n2/2pp4/3P4/2PBPN2/PP3PPP/RNBQ1RK1 w kq - 0 8"),
                new BenchmarkCase("King Attack", "r2q1rk1/ppp2ppp/2n2n2/3bp3/3P4/2N1PN2/PPQ2PPP/R1B1KB1R w KQ - 2 9"),
                new BenchmarkCase("Closed Center", "r1bq1rk1/pp1n1pbp/2pp1np1/4p3/2PPP3/2N1BN2/PP3PPP/R2QKB1R w KQ - 0 9"),
                new BenchmarkCase("Endgame Rook", "8/5pk1/3p2p1/2pPp3/2P1P3/5KP1/8/3R4 w - - 0 1"),
                new BenchmarkCase("Minor Piece Endgame", "8/2k5/2p2p2/3p4/3P4/2P2P2/2K5/3B4 w - - 0 1"),
                new BenchmarkCase("Tactical Middlegame", "r2q1rk1/pp1bbppp/2n1pn2/2pp4/3P4/2N1PN2/PPQ1BPPP/R1B2RK1 w - - 0 10"),
                new BenchmarkCase("Queenside Pressure", "2rq1rk1/pp1n1ppp/2pbpn2/3p4/3P4/2N1PN2/PPQ1BPPP/2RR2K1 w - - 3 13")
            };

            SnapShot.ConfigureSearchLimits(options.MaxIterationDepth, options.MaxSearchSize);

            writeLine("Running benchmark suite...");
            writeLine("Engine: " + (options.Engine == EngineKind.PVS ? "PVS" : "MTD"));
            writeLine("Runs per position: " + options.RunsPerPosition);
            writeLine("Search depth cap: " + options.MaxIterationDepth);
            writeLine("Search node cap: " + options.MaxSearchSize);
            writeLine("Opening book: " + (options.UseOpeningBook ? "enabled" : "disabled"));
            writeLine("");

            long suiteMillis = 0;
            int completedSearches = 0;

            foreach (BenchmarkCase testCase in suite)
            {
                long sumMillis = 0;
                int sumEval = 0;
                string sampleMove = "-";

                for (int run = 0; run < options.RunsPerPosition; run++)
                {
                    SnapShot s = SnapShot.FromFen(testCase.Fen);
                    Stopwatch sw = Stopwatch.StartNew();
                    Move bestMove = GetEngineMove(s, openingBookSan, options.Engine, options.UseOpeningBook, announceThinking: false);
                    sw.Stop();

                    long elapsed = Math.Max(1, sw.ElapsedMilliseconds);
                    sumMillis += elapsed;
                    completedSearches++;

                    if (bestMove != null)
                    {
                        sumEval += bestMove.Evaluation;
                        sampleMove = DescribeMove(s, bestMove);
                    }
                }

                suiteMillis += sumMillis;
                long avgMillis = Math.Max(1, sumMillis / options.RunsPerPosition);
                int avgEval = options.RunsPerPosition > 0 ? sumEval / options.RunsPerPosition : 0;

                writeLine(testCase.Name + " | avg=" + avgMillis + " ms | eval=" + avgEval + " | move=" + sampleMove);
            }

            writeLine("");
            writeLine("Summary:");
            writeLine("  Positions: " + suite.Length);
            writeLine("  Total searches: " + completedSearches);
            writeLine("  Total time: " + suiteMillis + " ms");
            if (suiteMillis > 0)
            {
                double searchesPerSecond = (completedSearches * 1000.0) / suiteMillis;
                writeLine("  Throughput: " + searchesPerSecond.ToString("F2") + " searches/s");
            }

            string reportFile = "benchmark-report.txt";
            File.WriteAllText(reportFile, report.ToString());
            writeLine("");
            writeLine("Report saved to " + Path.GetFullPath(reportFile));
        }

        static void RunBenchmarkComparison(BenchmarkOptions options, string openingBookSan)
        {
            StringBuilder report = new StringBuilder();
            Action<string> writeLine = line =>
            {
                Console.WriteLine(line);
                report.AppendLine(line);
            };

            BenchmarkCase[] suite = new BenchmarkCase[]
            {
                new BenchmarkCase("Start Position", "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w - - 0 1"),
                new BenchmarkCase("Open Sicilian", "r1bqk2r/pp2bppp/2n2n2/2pp4/3P4/2PBPN2/PP3PPP/RNBQ1RK1 w kq - 0 8"),
                new BenchmarkCase("King Attack", "r2q1rk1/ppp2ppp/2n2n2/3bp3/3P4/2N1PN2/PPQ2PPP/R1B1KB1R w KQ - 2 9"),
                new BenchmarkCase("Closed Center", "r1bq1rk1/pp1n1pbp/2pp1np1/4p3/2PPP3/2N1BN2/PP3PPP/R2QKB1R w KQ - 0 9"),
                new BenchmarkCase("Endgame Rook", "8/5pk1/3p2p1/2pPp3/2P1P3/5KP1/8/3R4 w - - 0 1"),
                new BenchmarkCase("Minor Piece Endgame", "8/2k5/2p2p2/3p4/3P4/2P2P2/2K5/3B4 w - - 0 1"),
                new BenchmarkCase("Tactical Middlegame", "r2q1rk1/pp1bbppp/2n1pn2/2pp4/3P4/2N1PN2/PPQ1BPPP/R1B2RK1 w - - 0 10"),
                new BenchmarkCase("Queenside Pressure", "2rq1rk1/pp1n1ppp/2pbpn2/3p4/3P4/2N1PN2/PPQ1BPPP/2RR2K1 w - - 3 13")
            };

            SnapShot.ConfigureSearchLimits(options.MaxIterationDepth, options.MaxSearchSize);

            writeLine("Comparing MTD vs PVS on benchmark suite...");
            writeLine("Runs per position: " + options.RunsPerPosition);
            writeLine("Search depth cap: " + options.MaxIterationDepth);
            writeLine("Search node cap: " + options.MaxSearchSize);
            writeLine("Opening book: " + (options.UseOpeningBook ? "enabled" : "disabled"));
            writeLine("");

            long mtdSuiteMillis = 0;
            long pvsSuiteMillis = 0;
            int mtdCompletedSearches = 0;
            int pvsCompletedSearches = 0;

            writeLine(string.Format("{0,-30} | {1,-15} | {2,-15}", "Position", "MTD (ms)", "PVS (ms)"));
            writeLine(new string('-', 64));

            foreach (BenchmarkCase testCase in suite)
            {
                long mtdMillis = 0;
                long pvsMillis = 0;

                for (int run = 0; run < options.RunsPerPosition; run++)
                {
                    SnapShot s1 = SnapShot.FromFen(testCase.Fen);
                    Stopwatch sw1 = Stopwatch.StartNew();
                    Move mtdMove = GetEngineMove(s1, openingBookSan, EngineKind.MTD, options.UseOpeningBook, announceThinking: false);
                    sw1.Stop();
                    mtdMillis += Math.Max(1, sw1.ElapsedMilliseconds);
                    mtdCompletedSearches++;

                    SnapShot s2 = SnapShot.FromFen(testCase.Fen);
                    Stopwatch sw2 = Stopwatch.StartNew();
                    Move pvsMove = GetEngineMove(s2, openingBookSan, EngineKind.PVS, options.UseOpeningBook, announceThinking: false);
                    sw2.Stop();
                    pvsMillis += Math.Max(1, sw2.ElapsedMilliseconds);
                    pvsCompletedSearches++;
                }

                mtdSuiteMillis += mtdMillis;
                pvsSuiteMillis += pvsMillis;

                long mtdAvg = mtdMillis / options.RunsPerPosition;
                long pvsAvg = pvsMillis / options.RunsPerPosition;

                writeLine(string.Format("{0,-30} | {1,-15} | {2,-15}", testCase.Name, mtdAvg, pvsAvg));
            }

            writeLine("");
            writeLine("Summary:");
            writeLine("  Positions: " + suite.Length);
            writeLine("");
            writeLine("  MTD:");
            writeLine("    Total searches: " + mtdCompletedSearches);
            writeLine("    Total time: " + mtdSuiteMillis + " ms");
            if (mtdSuiteMillis > 0)
            {
                double mtdSearchesPerSecond = (mtdCompletedSearches * 1000.0) / mtdSuiteMillis;
                writeLine("    Throughput: " + mtdSearchesPerSecond.ToString("F2") + " searches/s");
            }

            writeLine("");
            writeLine("  PVS:");
            writeLine("    Total searches: " + pvsCompletedSearches);
            writeLine("    Total time: " + pvsSuiteMillis + " ms");
            if (pvsSuiteMillis > 0)
            {
                double pvsSearchesPerSecond = (pvsCompletedSearches * 1000.0) / pvsSuiteMillis;
                writeLine("    Throughput: " + pvsSearchesPerSecond.ToString("F2") + " searches/s");
            }

            writeLine("");
            if (mtdSuiteMillis > 0 && pvsSuiteMillis > 0)
            {
                double advantage = ((mtdSuiteMillis - pvsSuiteMillis) * 100.0) / mtdSuiteMillis;
                if (advantage > 0)
                    writeLine("  PVS is " + advantage.ToString("F1") + "% faster than MTD");
                else
                    writeLine("  MTD is " + Math.Abs(advantage).ToString("F1") + "% faster than PVS");
            }

            string reportFile = "benchmark-report.txt";
            File.WriteAllText(reportFile, report.ToString());
            writeLine("");
            writeLine("Report saved to " + Path.GetFullPath(reportFile));
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

        static Move GetEngineMove(SnapShot s, string openingBookSan, EngineKind engine, bool useOpeningBook = true, bool announceThinking = true)
        {
            if (announceThinking)
            {
                Console.WriteLine();
                Console.WriteLine("Thinking...");
                Console.WriteLine();
            }

            if (engine == EngineKind.PVS)
            {
                if (useOpeningBook)
                    return s.GetBestMovePVS(openingBookSan, OpeningBookFormat.SanNotation);
                return s.GetBestMovePVS();
            }

            if (useOpeningBook)
                return s.GetBestMoveMTD(openingBookSan, OpeningBookFormat.SanNotation);
            return s.GetBestMoveMTD();
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

            // CLI helper: benchmark suite and exit
            if (args != null && args.Length > 0 && args[0].ToLower() == "benchmark")
            {
                if (args.Any(a => (a ?? "").Trim().ToLowerInvariant() == "--help" || (a ?? "").Trim().ToLowerInvariant() == "-h"))
                {
                    ShowBenchmarkUsage();
                    return;
                }

                if (!TryParseBenchmarkOptions(args, out BenchmarkOptions options, out string parseError))
                {
                    Console.WriteLine("Benchmark option error: " + parseError);
                    Console.WriteLine();
                    ShowBenchmarkUsage();
                    return;
                }

                RunBenchmarkSuite(options, openingBookSan);
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
