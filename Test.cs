// project created on 16.03.2004 at 21:34
using System;
using System.Text;
using Joe.Chess;

namespace Joe.Chess
{

class Test
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
	
	public static void Main(string[] args)
	{
		SnapShot s = SnapShot.StartUpSnapShot();

		s.PromotePawnEventHandler += new PromotePawnEventHandler(Test.OnPromotePawn);
		s.PieceHitEventHandler += new PieceHitEventHandler(Test.OnPieceHit);
		s.CheckEventHandler += new CheckEventHandler(Test.OnCheck);
		s.GameOverEventHandler += new GameOverEventHandler(Test.OnGameOver);

		string openingBook = "openings.txt";
		
		Console.WriteLine();
		Console.WriteLine(s);
/*
		s.PerformMove(
			new Move(
				Helper.Coordinates2FieldNumber(HorizontalCoordinateCode.HG,VerticalCoordinateCode.V1),
				Helper.Coordinates2FieldNumber(HorizontalCoordinateCode.HF,VerticalCoordinateCode.V3)
				)
			);
		Console.WriteLine();
		Console.WriteLine(s);
*/
		Move m = null;
		int c1 = 0;
		int c2 = 0;
		while (true)
		{
			if (s.LegalMoves.Count == 0)
			{
				break;
			}
//			Console.ReadLine();
			Console.WriteLine();
//			if ((c2 % 2) == 0)
				m = s.GetBestMoveMTD(openingBook);
//			else
//				m = s.GetBestMoveAlphaBeta(openingBook);
			s.PerformMove(m);
			Console.WriteLine();
			Console.WriteLine(s.ToString());
			c1++;
			if ((c1 % 2) == 0)
				c2++;
		}

		Console.WriteLine();
		Console.ReadLine();
	}
}


}
