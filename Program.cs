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
