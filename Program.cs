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

            SnapShot s = SnapShot.StartUpSnapShot();

            s.PromotePawnEventHandler += new PromotePawnEventHandler(Program.OnPromotePawn);
            s.PieceHitEventHandler += new PieceHitEventHandler(Program.OnPieceHit);
            s.CheckEventHandler += new CheckEventHandler(Program.OnCheck);
            s.GameOverEventHandler += new GameOverEventHandler(Program.OnGameOver);

            string openingBook = "openings.txt";
            string openingBookSan = "openings-san.txt";

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
            Console.WriteLine(s);
            
            Move m = null;
            int c1 = 0;
            int c2 = 0;
            while (true)
            {
                if (s.LegalMoves.Count == 0)
                {
                    break;
                }
                m = null;
                MoveList legalMoves = s.LegalMoves;
                Console.WriteLine();
                Console.WriteLine("Your move (e.g. e4 or A2-A4): ");
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
                        m = s.GetBestMoveMTD(openingBookSan, OpeningBookFormat.SanNotation);
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
                Console.WriteLine();
                s.PerformMove(m);
                Console.WriteLine(s.ToString());
                Console.WriteLine();
                Console.WriteLine("Thinking...");
                Console.WriteLine();
                m = s.GetBestMoveMTD(openingBookSan, OpeningBookFormat.SanNotation);
                s.PerformMove(m);
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
