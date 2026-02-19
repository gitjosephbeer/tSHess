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

            string openingBook = "openings.txt";

            // CLI helper: validate SAN openings file and exit
            if (args != null && args.Length > 0 && args[0].ToLower() == "validate-san-openings")
            {
                OpeningBook ob = new OpeningBook();
                string fileName = "openings_san.txt";
                if (args.Length > 1 && !string.IsNullOrEmpty(args[1])) fileName = args[1];
                var errors = ob.ValidateSANOpenings(fileName);
                if (errors == null || errors.Count == 0)
                {
                    Console.WriteLine("Validation: OK - no errors found in " + fileName);
                    File.WriteAllText("openings_san-validation-report.txt","Validation: OK - no errors found in " + fileName + Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("Validation found " + errors.Count.ToString() + " error(s):");
                    using (StreamWriter sw = new StreamWriter("openings_san-validation-report.txt",false))
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
                OpeningBook ob = new OpeningBook();
                string fileName = openingBook;
                if (args.Length > 1 && !string.IsNullOrEmpty(args[1])) fileName = args[1];
                var errors = ob.ValidateOpenings(fileName);
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
                    string[] test4 = { "Nf3", "Nf6", "Nc3", "Nc6", "d4", "d5", "Nxd5", "Nxd5", "Qxd5" };
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
                        "Be3", "e5", "Nf3", "Be7", "f4", "O-O", "Qd2", "Nbd7", "O-O-O", "b5"
                    };
                    RunSANTest(sw, snapshot, test6);
                    
                    // Test 7: Queenside castling
                    sw.WriteLine();
                    sw.WriteLine("TEST 7: Queenside Castling (O-O-O)");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test7 = { "d4", "d5", "Nf3", "Nf6", "c4", "e6", "Bg5", "Be7", "e3", "O-O", "Nc3", "Nbd7", "Rc1", "c6" };
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
                    
                    // Test 10: Pawn promotion attempted (long endgame scenario)
                    sw.WriteLine();
                    sw.WriteLine("TEST 10: Advanced Pawn with Promotion Opportunity");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test10 = {
                        "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "a8=Q"  // a8=Q is pawn promotion to Queen
                    };
                    RunSANTest(sw, snapshot, test10);
                    
                    // Test 11: Complex endgame with multiple piece types
                    sw.WriteLine();
                    sw.WriteLine("TEST 11: Endgame with Mixed Piece Captures");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test11 = {
                        "e4", "c5", "Nf3", "d6", "d4", "cxd4", "Nxd4", "Nf6", 
                        "Nc3", "a6", "Bg5", "e6", "f4", "Be7", "Qf3", "Nbd7"
                    };
                    RunSANTest(sw, snapshot, test11);
                    
                    // Test 12: Bishop moves with disambiguation
                    sw.WriteLine();
                    sw.WriteLine("TEST 12: Bishop Moves and Complex Piece Interactions");
                    snapshot = SnapShot.StartUpSnapShot();
                    string[] test12 = {
                        "d4", "d5", "Bf4", "Nf6", "Nf3", "c6", "c3", "Bf5", 
                        "Nbd2", "Nbd7", "e3", "e6", "Bd3", "Bxd3"
                    };
                    RunSANTest(sw, snapshot, test12);
                    
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

            // CLI helper: convert openings to SAN notation
            if (args != null && args.Length > 0 && args[0].ToLower() == "convert-openings-to-san")
            {
                string inFile = openingBook;
                string outFile = "openings_san.txt";
                if (args.Length > 1 && !string.IsNullOrEmpty(args[1])) inFile = args[1];
                if (args.Length > 2 && !string.IsNullOrEmpty(args[2])) outFile = args[2];

                try
                {
                    using (StreamReader sr = new StreamReader(inFile))
                    using (StreamWriter sw = new StreamWriter(outFile, false))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (line.StartsWith("#"))
                            {
                                int j = 1;
                                while (j < line.Length && Char.IsDigit(line[j])) j++;
                                if (j > 1 && (j == line.Length || Char.IsWhiteSpace(line[j])))
                                {
                                    string moveLabel = line.Substring(0, j);
                                    string movesAndComment = line.Substring(j).TrimStart();
                                    
                                    // Find comment
                                    int idxHash = movesAndComment.IndexOf('#');
                                    int idxSemi = movesAndComment.IndexOf(';');
                                    int idxSlash = movesAndComment.IndexOf("//", StringComparison.Ordinal);
                                    int commentIdx = -1;
                                    if (idxHash >= 0) commentIdx = idxHash;
                                    if (idxSemi >= 0 && (commentIdx == -1 || idxSemi < commentIdx)) commentIdx = idxSemi;
                                    if (idxSlash >= 0 && (commentIdx == -1 || idxSlash < commentIdx)) commentIdx = idxSlash;
                                    
                                    string movesPart = movesAndComment;
                                    string commentPart = "";
                                    if (commentIdx >= 0)
                                    {
                                        movesPart = movesAndComment.Substring(0, commentIdx).Trim();
                                        commentPart = " " + movesAndComment.Substring(commentIdx).TrimStart();
                                    }
                                    
                                    // Parse moves
                                    string[] tokens = movesPart.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                                    SnapShot snapshot = SnapShot.StartUpSnapShot();
                                    List<string> sanMoves = new List<string>();
                                    
                                    for (int i = 0; i < (int)(tokens.Length / 2); i++)
                                    {
                                        try
                                        {
                                            int from = Int32.Parse(tokens[i*2]);
                                            int to = Int32.Parse(tokens[i*2+1]);
                                            
                                            // Get legal move from LegalMoves property
                                            MoveList legalMoves = snapshot.LegalMoves;
                                            Move legalMove = legalMoves[from, to];
                                            if (legalMove != null)
                                            {
                                                string san = snapshot.MoveToSAN(legalMove);
                                                sanMoves.Add(san);
                                                snapshot.PerformMove(legalMove);
                                            }
                                            else
                                            {
                                                sanMoves.Add("ERROR");
                                            }
                                        }
                                        catch
                                        {
                                            sanMoves.Add("ERROR");
                                        }
                                    }
                                    
                                    // Write to output
                                    sw.Write(moveLabel);
                                    for (int i = 0; i < sanMoves.Count; i++)
                                        sw.Write(" " + sanMoves[i]);
                                    sw.WriteLine(commentPart);
                                    Console.WriteLine(moveLabel + " " + string.Join(" ", sanMoves) + commentPart);
                                }
                            }
                        }
                    }
                    Console.WriteLine("Conversion complete: " + outFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
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
                Console.WriteLine("Your move: ");
                while (true)
                {
                    string move_string = Console.ReadLine();
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
                    else if (s.LegalMoves.ToString().Contains(move_string + ";") || s.LegalMoves.ToString().Contains(";" + move_string))
                    {
                        m = Helper.String2Move(move_string);
                        break;
                    }
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
