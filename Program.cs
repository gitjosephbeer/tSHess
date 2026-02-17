using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
