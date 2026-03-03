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

            SnapShot s = SnapShot.StartUpSnapShot();

            s.PromotePawnEventHandler += new PromotePawnEventHandler(Program.OnPromotePawn);
            s.PieceHitEventHandler += new PieceHitEventHandler(Program.OnPieceHit);
            s.CheckEventHandler += new CheckEventHandler(Program.OnCheck);
            s.GameOverEventHandler += new GameOverEventHandler(Program.OnGameOver);

            string openingBook = "openings-san.txt";

            // CLI helper: validate SAN openings file and exit
            if (args != null && args.Length > 0 && args[0].ToLower() == "validate-san-openings")
            {
                //OpeningBook ob = new OpeningBook();
                string fileName = "openings-san.txt";
                if (args.Length > 1 && !string.IsNullOrEmpty(args[1])) fileName = args[1];
                var errors = OpeningBook.ValidateSanOpenings(fileName);
                if (errors == null || errors.Count == 0)
                {
                    Console.WriteLine("Validation: OK - no errors found in " + fileName);
                    File.WriteAllText("openings-san-validation-report.txt","Validation: OK - no errors found in " + fileName + Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Validation found " + errors.Count.ToString() + " error(s):");
                    using (StreamWriter sw = new StreamWriter("openings-san-validation-report.txt",false))
                    {
                        sw.WriteLine("Validation found " + errors.Count.ToString() + " error(s):");
                        foreach (var e in errors)
                        {
                            Console.WriteLine(e);
                            sw.WriteLine(e);
                        }
                    }
                }
                return;
            }

            // CLI helper: validate openings file and exit
            if (args != null && args.Length > 0 && args[0].ToLower() == "validate-openings")
            {
                //OpeningBook ob = new OpeningBook();
                string fileName = openingBook;
                if (args.Length > 1 && !string.IsNullOrEmpty(args[1])) fileName = args[1];
                var errors = OpeningBook.ValidateOpenings(fileName);
                if (errors == null || errors.Count == 0)
                {
                    Console.WriteLine("Validation: OK - no errors found in " + fileName);
                    File.WriteAllText("openings-validation-report.txt","Validation: OK - no errors found in " + fileName + Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Validation found " + errors.Count.ToString() + " error(s):");
                    using (StreamWriter sw = new StreamWriter("openings-validation-report.txt",false))
                    {
                        sw.WriteLine("Validation found " + errors.Count.ToString() + " error(s):");
                        foreach (var e in errors)
                        {
                            Console.WriteLine(e);
                            sw.WriteLine(e);
                        }
                    }
                }
                return;
            }

            // CLI helper: test SAN parsing logic
            if (args != null && args.Length > 0 && args[0].ToLower() == "test-san")
            {
                using (StreamWriter sw = new StreamWriter("san-test-report.txt", false))
                {
                    sw.WriteLine("========================================");
                    sw.WriteLine("COMPREHENSIVE SAN PARSING TEST SUITE");
                    sw.WriteLine("========================================");
                    sw.WriteLine();
                    
                    // Test 1: Standard openings
                    sw.WriteLine("TEST 1: Sicilian Najdorf (12 moves - standard opening)");
                    SnapShot snapshot = SnapShot.StartUpSnapShot();
                    string[] test1 = { "e4", "c5", "Nf3", "d6", "d4", "cxd4", "Nxd4", "Nf6", "Nc3", "a6", "g3", "e5" };
                    RunSANTest(sw, snapshot, test1);
                    
                    // Test 2: Ruy Lopez with castling
                    sw.WriteLine();
                    sw.WriteLine("TEST 2: Ruy Lopez with Castling (14 moves)");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test2 = { "e4", "e5", "Nf3", "Nc6", "Bb5", "a6", "Ba4", "Nf6", "O-O", "Be7", "Re1", "b5", "Bb3", "d6" };
                    RunSANTest(sw, snapshot, test2);
                    
                    // Test 3: Queen's Gambit
                    sw.WriteLine();
                    sw.WriteLine("TEST 3: Queen's Gambit (10 moves - standard)");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test3 = { "d4", "d5", "c4", "e6", "Nf3", "Nf6", "Bg5", "Be7", "e3", "O-O" };
                    RunSANTest(sw, snapshot, test3);
                    
                    // Test 4: Knight disambiguation (multiple knights can move to same square)
                    sw.WriteLine();
                    sw.WriteLine("TEST 4: Knight Disambiguation (complex piece moves)");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test4 = { "Nf3", "Nf6", "Nc3", "Nc6", "d4", "d5", "Nxd5", "Nxd5", "e4" };
                    RunSANTest(sw, snapshot, test4);
                    
                    // Test 5: Complex captures and piece interactions
                    sw.WriteLine();
                    sw.WriteLine("TEST 5: Multiple Captures and Piece Interactions");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test5 = { "e4", "c5", "Nf3", "Nc6", "d4", "cxd4", "Nxd4", "e5", "Nxc6", "bxc6", "Nc3", "Nf6" };
                    RunSANTest(sw, snapshot, test5);
                    
                    // Test 6: Pawn advancement toward promotion (long game)
                    sw.WriteLine();
                    sw.WriteLine("TEST 6: Pawn Advancement Toward Promotion (20 moves - deep game)");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test6 = {
                        "e4", "c5", "Nf3", "d6", "d4", "cxd4", "Nxd4", "Nf6", "Nc3", "a6",
                        "Be3", "e5", "Nf3", "Be7", "g3", "O-O", "Qd2", "Nbd7", "O-O-O", "b5"
                    };
                    RunSANTest(sw, snapshot, test6);
                    
                    // Test 7: Queenside castling
                    sw.WriteLine();
                    sw.WriteLine("TEST 7: Queenside Castling (O-O-O)");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test7 = { "d4", "d5", "Nc3", "Nc6", "Bf4", "Bf5", "Qd2", "Qd7", "O-O-O" };
                    RunSANTest(sw, snapshot, test7);
                    
                    // Test 8: File disambiguation (rooks on same file)
                    sw.WriteLine();
                    sw.WriteLine("TEST 8: Rook File Disambiguation and Complex Movement");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test8 = { "a4", "a5", "Ra3", "e6", "Rh3", "Ke7", "Rg3", "Kd6", "Rf3", "Kc5" };
                    RunSANTest(sw, snapshot, test8);
                    
                    // Test 9: En Passant capture (if possible in the game)
                    sw.WriteLine();
                    sw.WriteLine("TEST 9: En Passant Capture Setup");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test9 = {
                        "e4", "a6", "e5", "d5", "exd6"  // exd6 is the en passant capture
                    };
                    RunSANTest(sw, snapshot, test9);
                    
                    // Test 10: Pawn promotion to Queen
                    sw.WriteLine();
                    sw.WriteLine("TEST 10: Pawn Promotion to Queen");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test10 = {
                        "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "axb8=Q"  // axb8=Q is capture-promotion to Queen
                    };
                    RunSANTest(sw, snapshot, test10);

                    // Test 11: Pawn promotion to Rook
                    sw.WriteLine();
                    sw.WriteLine("TEST 11: Pawn Promotion to Rook");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test11 = {
                        "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "axb8=R"
                    };
                    RunSANTest(sw, snapshot, test11);

                    // Test 12: Pawn promotion to Bishop
                    sw.WriteLine();
                    sw.WriteLine("TEST 12: Pawn Promotion to Bishop");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test12 = {
                        "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "axb8=B"
                    };
                    RunSANTest(sw, snapshot, test12);

                    // Test 13: Pawn promotion to Knight
                    sw.WriteLine();
                    sw.WriteLine("TEST 13: Pawn Promotion to Knight");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test13 = {
                        "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "axb8=N"
                    };
                    RunSANTest(sw, snapshot, test13);
                    
                    // Test 14: Complex endgame with multiple piece types
                    sw.WriteLine();
                    sw.WriteLine("TEST 14: Endgame with Mixed Piece Captures");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test14 = {
                        "e4", "c5", "Nf3", "d6", "d4", "cxd4", "Nxd4", "Nf6", 
                        "Nc3", "a6", "Bg5", "e6", "f4", "Be7", "Qf3", "Nbd7"
                    };
                    RunSANTest(sw, snapshot, test14);
                    
                    // Test 15: Bishop moves with disambiguation
                    sw.WriteLine();
                    sw.WriteLine("TEST 15: Bishop Moves and Complex Piece Interactions");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test15 = {
                        "d4", "d5", "Bf4", "Nf6", "Nf3", "c6", "c3", "Bf5", 
                        "Nbd2", "Nbd7", "e3", "e6", "Bd3", "Bxd3"
                    };
                    RunSANTest(sw, snapshot, test15);

                    // Test 16: Check suffix parsing (+)
                    sw.WriteLine();
                    sw.WriteLine("TEST 16: Check Suffix Parsing (+)");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test16 = {
                        "e4", "e5", "Qh5", "Nc6", "Bc4", "Nf6", "Qxf7+"
                    };
                    RunSANTest(sw, snapshot, test16);

                    // Test 17: Checkmate suffix parsing (#)
                    sw.WriteLine();
                    sw.WriteLine("TEST 17: Checkmate Suffix Parsing (#)");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test17 = {
                        "f3", "e5", "g4", "Qh4#"
                    };
                    RunSANTest(sw, snapshot, test17);

                    // Test 18: Castling with zeros notation (0-0)
                    sw.WriteLine();
                    sw.WriteLine("TEST 18: Castling with Zero Notation (0-0)");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test18 = {
                        "e4", "e5", "Nf3", "Nc6", "Bc4", "Bc5", "0-0"
                    };
                    RunSANTest(sw, snapshot, test18);
                    
                    sw.WriteLine();
                    sw.WriteLine("========================================");
                    sw.WriteLine("TEST SUITE COMPLETED");
                    sw.WriteLine("========================================");
                }
                Console.WriteLine("Comprehensive SAN test completed - see san-test-report.txt");
                return;
            }

            static void RunSANTest(StreamWriter sw, SnapShot snapshot, string[] moves)
            {
                int successCount = 0;
                foreach (string san in moves)
                {
                    try
                    {
                        Move move = snapshot.SANToMove(san);
                        sw.WriteLine($"  ✓ {san}");
                        snapshot.PerformMove(move);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        sw.WriteLine($"  ✗ {san} - ERROR: {ex.Message}");
                        // Show board state for debugging
                        sw.WriteLine($"    Board state:");
                        sw.WriteLine($"    {snapshot.ToString().Replace(Environment.NewLine, Environment.NewLine + "    ")}");
                        sw.WriteLine($"    Legal moves: {snapshot.LegalMoves.ToString()}");
                        break;
                    }
                }
                sw.WriteLine($"  Result: {successCount}/{moves.Length} moves parsed correctly");
            }

            // CLI helper: test SAN <-> Move roundtrip for legal moves in many positions
            if (args != null && args.Length > 0 && args[0].ToLower() == "test-san-roundtrip")
            {
                using (StreamWriter sw = new StreamWriter("san-roundtrip-report.txt", false))
                {
                    sw.WriteLine("========================================");
                    sw.WriteLine("SAN ROUNDTRIP CONSISTENCY TEST SUITE");
                    sw.WriteLine("========================================");
                    sw.WriteLine();

                    int totalPositions = 0;
                    int totalMovesChecked = 0;
                    int totalErrors = 0;

                    void ValidateRoundTripAtPosition(string label, SnapShot snap)
                    {
                        MoveList legal = snap.LegalMoves;
                        int positionErrors = 0;

                        for (int i = 0; i < legal.Count; i++)
                        {
                            Move legalMove = legal[i];
                            try
                            {
                                string san = snap.MoveToSAN(legalMove);
                                Move reparsed = snap.SANToMove(san);

                                if (reparsed == null || reparsed.FieldNumberFrom != legalMove.FieldNumberFrom || reparsed.FieldNumberTo != legalMove.FieldNumberTo)
                                {
                                    positionErrors++;
                                    sw.WriteLine($"  ✗ {label}: roundtrip mismatch for {legalMove} -> '{san}' -> {reparsed}");
                                }
                            }
                            catch (Exception ex)
                            {
                                positionErrors++;
                                sw.WriteLine($"  ✗ {label}: exception on {legalMove}: {ex.Message}");
                            }
                        }

                        totalPositions++;
                        totalMovesChecked += legal.Count;
                        totalErrors += positionErrors;
                        sw.WriteLine($"  {label}: {legal.Count} legal moves checked, errors: {positionErrors}");
                    }

                    void RunRoundTripScenario(string title, string[] moves)
                    {
                        sw.WriteLine(title);
                        SnapShot snapshot = SnapShot.StartUpSnapShot();

                        ValidateRoundTripAtPosition("start position", snapshot);

                        for (int i = 0; i < moves.Length; i++)
                        {
                            string san = moves[i];
                            try
                            {
                                Move move = snapshot.SANToMove(san);
                                snapshot.PerformMove(move);
                                ValidateRoundTripAtPosition("after " + san, snapshot);
                            }
                            catch (Exception ex)
                            {
                                totalErrors++;
                                sw.WriteLine($"  ✗ scenario move parse failed at '{san}': {ex.Message}");
                                break;
                            }
                        }

                        sw.WriteLine();
                    }

                    RunRoundTripScenario("SCENARIO 1: Sicilian", new string[] { "e4", "c5", "Nf3", "d6", "d4", "cxd4", "Nxd4", "Nf6", "Nc3", "a6" });
                    RunRoundTripScenario("SCENARIO 2: Ruy Lopez", new string[] { "e4", "e5", "Nf3", "Nc6", "Bb5", "a6", "Ba4", "Nf6", "O-O", "Be7" });
                    RunRoundTripScenario("SCENARIO 3: En Passant Line", new string[] { "e4", "a6", "e5", "d5", "exd6" });
                    RunRoundTripScenario("SCENARIO 4: Promotion Line", new string[] { "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "axb8=Q" });
                    RunRoundTripScenario("SCENARIO 5: Check/Checkmate Suffix Inputs", new string[] { "f3", "e5", "g4", "Qh4#" });

                    sw.WriteLine("========================================");
                    sw.WriteLine($"TOTAL POSITIONS CHECKED: {totalPositions}");
                    sw.WriteLine($"TOTAL LEGAL MOVES CHECKED: {totalMovesChecked}");
                    sw.WriteLine($"TOTAL ERRORS: {totalErrors}");
                    sw.WriteLine("========================================");
                }

                Console.WriteLine("SAN roundtrip test completed - see san-roundtrip-report.txt");
                return;
            }

            Console.WriteLine("Welcom to tSHess!\n*****************\n\n");
            Console.WriteLine(s);
            
            Move m = null;
            int c1 = 0;
            int c2 = 0;
            // while (true)
            // {
            //     if (s.LegalMoves.Count == 0)
            //     {
            //         break;
            //     }
            //     Console.WriteLine();
            //     m = s.GetBestMoveMTD(openingBook);
            //     m = s.GetBestMoveAlphaBeta(openingBook);
            //     s.PerformMove(m);
            //     Console.WriteLine();
            //     Console.WriteLine(s.ToString());
            //     c1++;
            //     if ((c1 % 2) == 0)
            //         c2++;
            // }
            while (true)
            {
                if (s.LegalMoves.Count == 0)
                {
                    break;
                }
                m = null;
                MoveList legalMoves = s.LegalMoves;
                Console.WriteLine("Legal moves (coord): " + legalMoves.ToString());
                Console.WriteLine("Legal moves (SAN):   " + legalMoves.ToSanString(s));
                Console.WriteLine("Your move (SAN or coordinates, e.g. e4 or A2-A4): ");
                while (true)
                {
                    string move_string = Console.ReadLine();
                    if (move_string == null)
                    {
                        Console.WriteLine("Illegal move, try again: ");
                        continue;
                    }
                    move_string = move_string.Trim();

                    if (move_string.ToLower() == "exit" || move_string.ToLower() == "quit")
                    {
                        Console.WriteLine("bye-bye...");
                        return;
                    }
                    if (move_string.ToLower() == "help")
                    {
                        m = s.GetBestMoveMTD(openingBook);
                        break;
                    }

                    bool parsed = false;

                    // 1) Coordinate notation (e.g. A2-A4)
                    if (move_string.Length == 5 && move_string[2] == '-')
                    {
                        try
                        {
                            Move coordMove = Helper.String2Move(move_string.ToUpper());
                            Move legalCoordMove = legalMoves[coordMove.FieldNumberFrom, coordMove.FieldNumberTo];
                            if (legalCoordMove != null)
                            {
                                m = legalCoordMove;
                                parsed = true;
                            }
                        }
                        catch
                        {
                        }
                    }

                    // 2) SAN notation (e.g. e4, Nf3, O-O, exd6, axb8=Q)
                    if (!parsed)
                    {
                        try
                        {
                            m = s.SANToMove(move_string);
                            parsed = true;
                        }
                        catch
                        {
                        }
                    }

                    if (parsed)
                        break;

                    Console.WriteLine("Illegal move, try again: ");
                }
                s.PerformMove(m);
                Console.WriteLine();
                Console.WriteLine(s.ToString());
                Console.WriteLine("\nthinking...\n");
                m = s.GetBestMoveMTD(openingBook);
                s.PerformMove(m);
                Console.WriteLine();
                Console.WriteLine(s.ToString());
                c1++;
                if ((c1 % 2) == 0)
                    c2++;
            }

            Console.WriteLine("Done!");
            Console.ReadLine();
        }
    }
}
