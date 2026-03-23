// project created on 15.03.2004 at 20:38

/*
   internal (field numbers) and user view of the board:
               _______________________________________________
              |7    |15   |23   |31   |39   |47   |55   |63   |
            8 |     |     |     |     |     |     |     |     | <-- Black
              |_____|_____|_____|_____|_____|_____|_____|_____|
              |6    |14   |22   |30   |38   |46   |54   |62   |
            7 |     |     |     |     |     |     |     |     |
              |_____|_____|_____|_____|_____|_____|_____|_____|
              |5    |13   |21   |29   |37   |45   |53   |61   |
            6 |     |     |     |     |     |     |     |     |
              |_____|_____|_____|_____|_____|_____|_____|_____|
              |4    |12   |20   |28   |36   |44   |52   |60   |
            5 |     |     |     |     |     |     |     |     |
              |_____|_____|_____|_____|_____|_____|_____|_____|
              |3    |11   |19   |27   |35   |43   |51   |59   |
            4 |     |     |     |     |     |     |     |     |
              |_____|_____|_____|_____|_____|_____|_____|_____|
              |2    |10   |18   |26   |34   |42   |50   |58   |
            3 |     |     |     |     |     |     |     |     |
              |_____|_____|_____|_____|_____|_____|_____|_____|
              |1    |9    |17   |25   |33   |41   |49   |57   |
            2 |     |     |     |     |     |     |     |     |
              |_____|_____|_____|_____|_____|_____|_____|_____|
              |0    |8    |16   |24   |32   |40   |48   |56   |
            1 |     |     |     |     |     |     |     |     | <-- White
              |_____|_____|_____|_____|_____|_____|_____|_____|
                 A     B     C     D     E     F     G     H   
                 
*/

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace tSHess.Engine
{
	/// <summary>
	/// Represents the color/side of a chess player or piece.
	/// Values are encoded to align with internal bit masks in board representation.
	/// </summary>
	public enum Color : int
	{
		White	= 0,
		Black	= 8
	}

	/// <summary>
	/// Describes how an evaluation value from search should be interpreted.
	/// </summary>
	public enum EvaluationType : int
	{
		Null		= -1,
		Accurate	= 0,
		Upperbound	= 1,
		Lowerbound	= 2
	}

	/// <summary>
	/// Indicates the side associated with a move entry.
	/// </summary>
	public enum MoveType : int
	{
		Null		= -1,
		White		= Color.White,
		Black		= Color.Black
	}

	/// <summary>
	/// Represents whether a side is controlled by a human or the engine.
	/// </summary>
	public enum PlayerMode : int
	{
		Human		= 0,
		Computer	= 1
	}

	/// <summary>
	/// Supported opening book notation formats.
	/// </summary>
	public enum OpeningBookFormat : int
	{
		CoordinateNotation	= 0,
		SanNotation			= 1
	}

	/// <summary>
	/// Chess piece types used by the engine.
	/// </summary>
	public enum PieceType : int
	{
		None	= -1,
		Pawn	= 0,
		Rook	= 1,
		Knight	= 2,
		Bishop	= 3,
		Queen	= 4,
		King	= 5
	}

	/// <summary>
	/// Bit masks used for internal board-square byte encoding.
	/// </summary>
	public enum Mask : int
	{
		PieceType	= 7,
		Color		= 8,
		MovedFlag	= 16,
		NullFlag	= 32
	}

	/// <summary>
	/// Allowed promotion target piece types for pawn promotion.
	/// </summary>
	public enum PromotionPieceType : int
	{
		Queen	= PieceType.Queen,
		Bishop	= PieceType.Bishop,
		Knight	= PieceType.Knight,
		Rook	= PieceType.Rook
	}

	/// <summary>
	/// Tactical situation state of a side after a move.
	/// </summary>
	public enum SituationCode : int
	{
		Unknown		= -1,
		Normal		= 0,
		Check		= 1,
		Checkmate	= 2,
		Stalemate	= 3,
	}

	/// <summary>
	/// Classifies a move by its special semantics (castling, en passant, promotion, etc.).
	/// </summary>
	public enum MoveCode : int
	{
		Unknown					= -1,
		Resign					= 0,
		NormalMove				= 1,
		EnPassant				= 2,
		HittingPiece			= 3,
		SmallCastling			= 4,
		BigCastling				= 5,
		PromotePawn				= 6,
		PromotePawnHittingPiece	= 7
	}

	/// <summary>
	/// File coordinate (A..H) for board indexing helpers.
	/// </summary>
	public enum HorizontalCoordinateCode : int
	{
		HA	= 0,
		HB	= 1,
		HC	= 2,
		HD	= 3,
		HE	= 4,
		HF	= 5,
		HG	= 6,
		HH	= 7
	}

	/// <summary>
	/// Rank coordinate (1..8) for board indexing helpers.
	/// </summary>
	public enum VerticalCoordinateCode : int
	{
		V1	= 0,
		V2	= 1,
		V3	= 2,
		V4	= 3,
		V5	= 4,
		V6	= 5,
		V7	= 6,
		V8	= 7
	}

	/// <summary>
	/// Direction categories used in move generation.
	/// </summary>
	public enum MoveDirectionCode : int
	{
		Diagonal1	= 0,
		Diagonal2	= 1,
		Horizontal	= 2,
		Vertical	= 3,
		Knight1		= 4,
		Knight2		= 5,
		Knight3		= 6,
		Knight4		= 7
	}

	/// <summary>
	/// Static utility and lookup helper for board coordinates, piece names, values, and direction steps.
	/// </summary>
	public class Helper
	{
		public static int[] moveDirectionCode2Step = new int[] { 9, 7, 8, 1, 6, 10, 15, 17 };
		public static int[] pieceType2Value = new int[] { 100, 500, 300, 350, 900, 2000 };
		public static string[] pieceType2LongName = new string[] { "Pawn", "Rook", "Knight", "Bishop", "Queen", "King"};
		public static string[] pieceType2ShortName = new string[] { " ", "R", "N", "B", "Q", "K"};
		public static Color[] fieldNumber2Color = new Color[] {
																   Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White,
																   Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black,
																   Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White,
																   Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black,
																   Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White,
																   Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black,
																   Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White,
																   Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black, Color.White, Color.Black };
		public static HorizontalCoordinateCode[] fieldNumber2HorizontalCoordinateCode = new HorizontalCoordinateCode[] {
																															HorizontalCoordinateCode.HA, HorizontalCoordinateCode.HA, HorizontalCoordinateCode.HA, HorizontalCoordinateCode.HA, HorizontalCoordinateCode.HA, HorizontalCoordinateCode.HA, HorizontalCoordinateCode.HA, HorizontalCoordinateCode.HA,
																															HorizontalCoordinateCode.HB, HorizontalCoordinateCode.HB, HorizontalCoordinateCode.HB, HorizontalCoordinateCode.HB, HorizontalCoordinateCode.HB, HorizontalCoordinateCode.HB, HorizontalCoordinateCode.HB, HorizontalCoordinateCode.HB,
																															HorizontalCoordinateCode.HC, HorizontalCoordinateCode.HC, HorizontalCoordinateCode.HC, HorizontalCoordinateCode.HC, HorizontalCoordinateCode.HC, HorizontalCoordinateCode.HC, HorizontalCoordinateCode.HC, HorizontalCoordinateCode.HC,
																															HorizontalCoordinateCode.HD, HorizontalCoordinateCode.HD, HorizontalCoordinateCode.HD, HorizontalCoordinateCode.HD, HorizontalCoordinateCode.HD, HorizontalCoordinateCode.HD, HorizontalCoordinateCode.HD, HorizontalCoordinateCode.HD,
																															HorizontalCoordinateCode.HE, HorizontalCoordinateCode.HE, HorizontalCoordinateCode.HE, HorizontalCoordinateCode.HE, HorizontalCoordinateCode.HE, HorizontalCoordinateCode.HE, HorizontalCoordinateCode.HE, HorizontalCoordinateCode.HE,
																															HorizontalCoordinateCode.HF, HorizontalCoordinateCode.HF, HorizontalCoordinateCode.HF, HorizontalCoordinateCode.HF, HorizontalCoordinateCode.HF, HorizontalCoordinateCode.HF, HorizontalCoordinateCode.HF, HorizontalCoordinateCode.HF,
																															HorizontalCoordinateCode.HG, HorizontalCoordinateCode.HG, HorizontalCoordinateCode.HG, HorizontalCoordinateCode.HG, HorizontalCoordinateCode.HG, HorizontalCoordinateCode.HG, HorizontalCoordinateCode.HG, HorizontalCoordinateCode.HG,
																															HorizontalCoordinateCode.HH, HorizontalCoordinateCode.HH, HorizontalCoordinateCode.HH, HorizontalCoordinateCode.HH, HorizontalCoordinateCode.HH, HorizontalCoordinateCode.HH, HorizontalCoordinateCode.HH, HorizontalCoordinateCode.HH };
		public static VerticalCoordinateCode[] fieldNumber2VerticalCoordinateCode = new VerticalCoordinateCode[] {
																													  VerticalCoordinateCode.V1, VerticalCoordinateCode.V2, VerticalCoordinateCode.V3, VerticalCoordinateCode.V4, VerticalCoordinateCode.V5, VerticalCoordinateCode.V6, VerticalCoordinateCode.V7, VerticalCoordinateCode.V8,
																													  VerticalCoordinateCode.V1, VerticalCoordinateCode.V2, VerticalCoordinateCode.V3, VerticalCoordinateCode.V4, VerticalCoordinateCode.V5, VerticalCoordinateCode.V6, VerticalCoordinateCode.V7, VerticalCoordinateCode.V8,
																													  VerticalCoordinateCode.V1, VerticalCoordinateCode.V2, VerticalCoordinateCode.V3, VerticalCoordinateCode.V4, VerticalCoordinateCode.V5, VerticalCoordinateCode.V6, VerticalCoordinateCode.V7, VerticalCoordinateCode.V8,
																													  VerticalCoordinateCode.V1, VerticalCoordinateCode.V2, VerticalCoordinateCode.V3, VerticalCoordinateCode.V4, VerticalCoordinateCode.V5, VerticalCoordinateCode.V6, VerticalCoordinateCode.V7, VerticalCoordinateCode.V8,
																													  VerticalCoordinateCode.V1, VerticalCoordinateCode.V2, VerticalCoordinateCode.V3, VerticalCoordinateCode.V4, VerticalCoordinateCode.V5, VerticalCoordinateCode.V6, VerticalCoordinateCode.V7, VerticalCoordinateCode.V8,
																													  VerticalCoordinateCode.V1, VerticalCoordinateCode.V2, VerticalCoordinateCode.V3, VerticalCoordinateCode.V4, VerticalCoordinateCode.V5, VerticalCoordinateCode.V6, VerticalCoordinateCode.V7, VerticalCoordinateCode.V8,
																													  VerticalCoordinateCode.V1, VerticalCoordinateCode.V2, VerticalCoordinateCode.V3, VerticalCoordinateCode.V4, VerticalCoordinateCode.V5, VerticalCoordinateCode.V6, VerticalCoordinateCode.V7, VerticalCoordinateCode.V8,
																													  VerticalCoordinateCode.V1, VerticalCoordinateCode.V2, VerticalCoordinateCode.V3, VerticalCoordinateCode.V4, VerticalCoordinateCode.V5, VerticalCoordinateCode.V6, VerticalCoordinateCode.V7, VerticalCoordinateCode.V8 };

		/// <summary>
		/// Gets the configured material value for a piece type.
		/// </summary>
		/// <param name="pieceType">The piece type to evaluate.</param>
		/// <returns>The material value used by the engine.</returns>
		public static int PieceType2Value(PieceType pieceType)
		{
			return pieceType2Value[(int)pieceType];
		}

		/// <summary>
		/// Gets the opposing side color.
		/// </summary>
		/// <param name="color">The input side color.</param>
		/// <returns>The opposing side color.</returns>
		public static Color OpponentColor(Color color)
		{
			if (color == Color.White)
				return Color.Black;
			else
				return Color.White;
		}

		/// <summary>
		/// Gets the display name of a piece type (for example, <c>Pawn</c>).
		/// </summary>
		/// <param name="pieceType">Piece type to convert.</param>
		/// <returns>Long piece name string.</returns>
		public static string PieceType2LongName(PieceType pieceType)
		{
			return pieceType2LongName[(int)pieceType];
		}

		/// <summary>
		/// Gets the SAN-style short name of a piece type (for example, <c>N</c> for knight).
		/// </summary>
		/// <param name="pieceType">Piece type to convert.</param>
		/// <returns>Short piece name string.</returns>
		public static string PieceType2ShortName(PieceType pieceType)
		{
			return pieceType2ShortName[(int)pieceType];
		}

		/// <summary>
		/// Converts a move direction code into the corresponding board-step delta.
		/// </summary>
		/// <param name="c">Direction code.</param>
		/// <returns>Square index delta used in internal 0..63 board indexing.</returns>
		public static int MoveDirectionCode2Step(MoveDirectionCode c)
		{
			return moveDirectionCode2Step[(int)c];
		}

		/// <summary>
		/// Gets the board square color (light/dark) for a field index.
		/// </summary>
		/// <param name="f">Board field number (0..63).</param>
		/// <returns>Square color as <see cref="Color.White"/> or <see cref="Color.Black"/>.</returns>
		public static Color FieldNumber2Color(int f)
		{
			return fieldNumber2Color[f];
		}

		/// <summary>
		/// Gets the file component (A..H) for a board field index.
		/// </summary>
		/// <param name="f">Board field number (0..63).</param>
		/// <returns>Horizontal/file coordinate code.</returns>
		public static HorizontalCoordinateCode FieldNumber2HorizontalCoordinateCode(int f)
		{
			return fieldNumber2HorizontalCoordinateCode[f];
		}

		/// <summary>
		/// Gets the rank component (1..8) for a board field index.
		/// </summary>
		/// <param name="f">Board field number (0..63).</param>
		/// <returns>Vertical/rank coordinate code.</returns>
		public static VerticalCoordinateCode FieldNumber2VerticalCoordinateCode(int f)
		{
			return fieldNumber2VerticalCoordinateCode[f];
		}

		/// <summary>
		/// Converts a board field index to coordinate text (for example, <c>A1</c>).
		/// </summary>
		/// <param name="f">Board field number (0..63).</param>
		/// <returns>Coordinate string in algebraic format.</returns>
		public static string FieldNumber2String(int f)
		{
			return ((char)(fieldNumber2HorizontalCoordinateCode[f] + 'A')).ToString()+((char)(fieldNumber2VerticalCoordinateCode[f] + '1')).ToString();
		}

		/// <summary>
		/// Parses coordinate notation in the form <c>A2-A4</c> into a move.
		/// </summary>
		/// <param name="move_string">Move text in coordinate notation.</param>
		/// <returns>A move instance with parsed source and destination fields.</returns>
		public static Move String2Move(string move_string)
		{
			var from_hc = HChar2HorizontalCoordinateCode(move_string[0]);
			var from_vc = VChar2VerticalCoordinateCode(move_string[1]);
			var to_hc = HChar2HorizontalCoordinateCode(move_string[3]);
			var to_vc = VChar2VerticalCoordinateCode(move_string[4]);
			var from_fn = Coordinates2FieldNumber(from_hc,from_vc);
			var to_fn = Coordinates2FieldNumber(to_hc,to_vc);
			return new Move(from_fn,to_fn);
		}

		/// <summary>
		/// Converts a file character from <c>A</c> to <c>H</c> into a horizontal coordinate code.
		/// </summary>
		/// <param name="c">A file character between <c>A</c> and <c>H</c>.</param>
		/// <returns>The corresponding horizontal coordinate code.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="c"/> is outside <c>A</c>..<c>H</c>.</exception>
		public static HorizontalCoordinateCode HChar2HorizontalCoordinateCode(char c)
		{
			if (c < 'A' || c > 'H')
				throw new ArgumentException("Out of range","c");
			return (HorizontalCoordinateCode)(c - 'A');
		}

		/// <summary>
		/// Converts a rank character from <c>1</c> to <c>8</c> into a vertical coordinate code.
		/// </summary>
		/// <param name="c">A rank character between <c>1</c> and <c>8</c>.</param>
		/// <returns>The corresponding vertical coordinate code.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="c"/> is outside <c>1</c>..<c>8</c>.</exception>
		public static VerticalCoordinateCode VChar2VerticalCoordinateCode(char c)
		{
			if (c < '1' || c > '8')
				throw new ArgumentException("Out of range","c");
			return (VerticalCoordinateCode)(c - '1');
		}

		/// <summary>
		/// Converts horizontal/vertical coordinate codes into algebraic square text (for example, <c>E4</c>).
		/// </summary>
		/// <param name="hc">Horizontal/file coordinate.</param>
		/// <param name="vc">Vertical/rank coordinate.</param>
		/// <returns>Square text in algebraic format.</returns>
		public static string Coordinates2String(HorizontalCoordinateCode hc, VerticalCoordinateCode vc)
		{
			return ((char)(hc + 'A')).ToString()+((char)(vc + '1')).ToString();
		}

		/// <summary>
		/// Converts board coordinates to the internal field index (<c>0..63</c>).
		/// </summary>
		/// <param name="hc">Horizontal coordinate code (file).</param>
		/// <param name="vc">Vertical coordinate code (rank).</param>
		/// <returns>The internal field number for the coordinate pair.</returns>
		public static int Coordinates2FieldNumber(HorizontalCoordinateCode hc, VerticalCoordinateCode vc)
		{
			return ((int)hc)*8 + (int)vc;
		}
		
		private static Random random = new Random((int)DateTime.Now.Ticks);
		/// <summary>
		/// Returns a non-negative random number that is less than the specified maximum.
		/// </summary>
		/// <param name="maxNumber">Exclusive upper bound for the generated number.</param>
		/// <returns>A random integer in the range <c>[0, maxNumber)</c>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maxNumber"/> is less than zero.</exception>
		public static int RandomNumber(int maxNumber)
		{
			return random.Next(maxNumber);
		}
		/// <summary>
		/// Returns a random number within a specified range.
		/// </summary>
		/// <param name="minNumber">Inclusive lower bound of the random range.</param>
		/// <param name="maxNumber">Exclusive upper bound of the random range.</param>
		/// <returns>A random integer in the range <c>[minNumber, maxNumber)</c>.</returns>
		public static int RandomNumber(int minNumber, int maxNumber)
		{
			return random.Next(minNumber,maxNumber);
		}
		/// <summary>
		/// Returns a non-negative random integer.
		/// </summary>
		/// <returns>A random non-negative integer.</returns>
		public static int RandomNumber()
		{
			return random.Next();
		}
	} // Helper

	//public class GameOfChess
	//{



	public class OpeningBookEntry
	{
		// A signature for the board position stored in the entry
		public long Lock = 0;

		// Moves
		public MoveType MoveType = MoveType.Null;

		public bool IsNullEntry
		{
			get
			{
				return MoveType == MoveType.Null;
			}
		}

		public MoveList MoveList = null;

		// Construction
		public OpeningBookEntry()
		{
		}
	}

	public class OpeningBook
	{
		// The hash table itself
		private const int TABLE_SIZE = 1024;
		private OpeningBookEntry[] openingBookEntryTable = null;

		/// <summary>
		/// Initializes an empty opening book hash table.
		/// </summary>
		public OpeningBook()
		{
			openingBookEntryTable = new OpeningBookEntry[TABLE_SIZE];
			for (int i = 0; i < TABLE_SIZE; i++)
				openingBookEntryTable[i] = new OpeningBookEntry();
		}

		/// <summary>
		/// Queries the opening book for a move in the given position.
		/// </summary>
		/// <param name="snapShot">The position to query.</param>
		/// <returns>A cloned opening move if available; otherwise <c>null</c>.</returns>
		public Move Query(SnapShot snapShot)
		{
			// First, look for a match in the table
			int key = Math.Abs(snapShot.GetHashCode() % TABLE_SIZE);
			int _lock = snapShot.GetHashLockCode();

			// If the hash lock doesn't match the one for our position, get out
			OpeningBookEntry entry = openingBookEntryTable[key];
			if (entry.MoveType == MoveType.Null || entry.MoveType != (MoveType)snapShot.WhoToMove || entry.Lock != _lock || entry.MoveList == null || entry.MoveList.Count == 0)
				return null;

			if (entry.MoveList.Count == 1)
				return entry.MoveList[0].Clone();
			else
				return entry.MoveList[Helper.RandomNumber(entry.MoveList.Count)].Clone();
		}

		/// <summary>
		/// Loads opening lines from a file using the specified notation format.
		/// </summary>
		/// <param name="fileName">Path to the openings file.</param>
		/// <param name="format">Notation format used in the file.</param>
		/// <returns><c>true</c> if loading completed; otherwise <c>false</c>.</returns>
		public bool Load(string fileName, OpeningBookFormat format)
		{
			if (format == OpeningBookFormat.SanNotation)
				return LoadSan(fileName);
			return LoadCoordinate(fileName);
		}

		private bool LoadCoordinate(string fileName)
		{
			try 
			{
				using (StreamReader sr = new StreamReader(fileName)) 
				{
					string line;
					while ((line = sr.ReadLine()) != null) 
					{
						SnapShot s = SnapShot.StartUpSnapShot();
						// strip leading numeric label like "#14 " (keep rest of line),
						// then strip inline comments ("#", ";" or "//") and skip empty lines
						line = line.Trim();
						if (line.StartsWith("#"))
						{
							int j = 1;
							while (j < line.Length && Char.IsDigit(line[j])) j++;
							if (j > 1 && (j == line.Length || Char.IsWhiteSpace(line[j])))
							{
								line = line.Substring(j).TrimStart();
							}
						}
						int idxHash = line.IndexOf('#');
						int idxSemi = line.IndexOf(';');
						int idxSlash = line.IndexOf("//", StringComparison.Ordinal);
						int commentIdx = -1;
						if (idxHash >= 0) commentIdx = idxHash;
						if (idxSemi >= 0 && (commentIdx == -1 || idxSemi < commentIdx)) commentIdx = idxSemi;
						if (idxSlash >= 0 && (commentIdx == -1 || idxSlash < commentIdx)) commentIdx = idxSlash;
						if (commentIdx >= 0)
							line = line.Substring(0, commentIdx).Trim();
						if (line.Length == 0)
							continue;
						// split on whitespace and remove empty tokens
						string[] tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < (int)(tokens.Length / 2); i++)
						{
							int from = 0;
							int to = 0;
							try
							{
								string sFrom = tokens[i*2];
								sFrom = sFrom == null ? "" : sFrom.Trim();
								string sTo = tokens[i*2+1];
								sTo = sTo == null ? "" : sTo.Trim();

								from = Int32.Parse(sFrom);
								to = Int32.Parse(sTo);

								Move m = new Move(from,to);

								s.Clone().PerformMove(m);

								StoreMove(s,m);

								s.PerformMove(m);
							}
							catch
							{
							}
						}
					}
				}
			}
			catch
			{
				return false;
			}

			return true;
		}

		// Loading the table from a SAN openings file
		private bool LoadSan(string fileName)
		{
			try
			{
				using (StreamReader sr = new StreamReader(fileName))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						SnapShot s = SnapShot.StartUpSnapShot();
						// strip leading numeric label like "#14 " (keep rest of line),
						// then strip inline comments ("#", ";" or "//") and skip empty lines
						line = line.Trim();
						if (line.StartsWith("#"))
						{
							int j = 1;
							while (j < line.Length && Char.IsDigit(line[j])) j++;
							if (j > 1 && (j == line.Length || Char.IsWhiteSpace(line[j])))
							{
								line = line.Substring(j).TrimStart();
							}
						}
						int idxHash = line.IndexOf('#');
						int idxSemi = line.IndexOf(';');
						int idxSlash = line.IndexOf("//", StringComparison.Ordinal);
						int commentIdx = -1;
						if (idxHash >= 0) commentIdx = idxHash;
						if (idxSemi >= 0 && (commentIdx == -1 || idxSemi < commentIdx)) commentIdx = idxSemi;
						if (idxSlash >= 0 && (commentIdx == -1 || idxSlash < commentIdx)) commentIdx = idxSlash;
						if (commentIdx >= 0)
							line = line.Substring(0, commentIdx).Trim();
						if (line.Length == 0)
							continue;

						string[] tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < tokens.Length; i++)
						{
							try
							{
								string sanMove = tokens[i];
								sanMove = sanMove == null ? "" : sanMove.Trim();
								if (sanMove.Length == 0)
									continue;

                                Move m = s.SanToMove(sanMove);

								StoreMove(s,m);

								s.PerformMove(m);
							}
							catch
							{
							}
						}
					}
				}
			}
			catch
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Validates an openings file and returns parse/legality errors.
		/// </summary>
		/// <param name="fileName">Path to the openings file.</param>
		/// <param name="format">Notation format used in the file.</param>
		/// <returns>A list of validation errors, or an empty list if valid.</returns>
		public static List<string> Validate(string fileName, OpeningBookFormat format)
		{
			if (format == OpeningBookFormat.SanNotation)
				return ValidateOpeningsSan(fileName);
			return ValidateOpenings(fileName);
		}

		// Validate SAN (Standard Algebraic Notation) openings file. Returns a list of error messages (empty if valid).
		private static List<string> ValidateOpeningsSan(string fileName)
		{
			List<string> errors = new List<string>();
			try
			{
				using (StreamReader sr = new StreamReader(fileName))
				{
					string line;
					int lineNo = 0;
					while ((line = sr.ReadLine()) != null)
					{
						lineNo++;
						string original = line;
						// same trimming/comment stripping as Load
						line = line.Trim();
						if (line.StartsWith("#"))
						{
							int j = 1;
							while (j < line.Length && Char.IsDigit(line[j])) j++;
							if (j > 1 && (j == line.Length || Char.IsWhiteSpace(line[j])))
							{
								line = line.Substring(j).TrimStart();
							}
						}
						int idxHash = line.IndexOf('#');
						int idxSemi = line.IndexOf(';');
						int idxSlash = line.IndexOf("//", StringComparison.Ordinal);
						int commentIdx = -1;
						if (idxHash >= 0) commentIdx = idxHash;
						if (idxSemi >= 0 && (commentIdx == -1 || idxSemi < commentIdx)) commentIdx = idxSemi;
						if (idxSlash >= 0 && (commentIdx == -1 || idxSlash < commentIdx)) commentIdx = idxSlash;
						if (commentIdx >= 0)
							line = line.Substring(0, commentIdx).Trim();
						if (line.Length == 0)
							continue;
						string[] tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
						SnapShot s = SnapShot.StartUpSnapShot();
						for (int i = 0; i < tokens.Length; i++)
						{
							string sanMove = tokens[i];
							try
							{
								Move m = s.SanToMove(sanMove);
								s.PerformMove(m);
							}
							catch (Exception ex)
							{
								string msg = "Line " + lineNo.ToString() + ": invalid SAN move '" + sanMove + "' at position " + (i + 1).ToString() + " - " + ex.Message + Environment.NewLine;
								msg += "Board before failure:" + Environment.NewLine + s.ToString() + Environment.NewLine;
								msg += "Legal moves before failure:" + Environment.NewLine + s.LegalMoves.ToString() + Environment.NewLine;
								msg += "Full line: " + original.Trim() + Environment.NewLine;
								errors.Add(msg);
								break;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				errors.Add("Could not open/parse file: " + ex.Message);
			}
			return errors;
		}

		// Validate an openings file. Returns a list of error messages (empty if valid).
		private static List<string> ValidateOpenings(string fileName)
		{
			List<string> errors = new List<string>();
			try
			{
				using (StreamReader sr = new StreamReader(fileName))
				{
					string line;
					int lineNo = 0;
					while ((line = sr.ReadLine()) != null)
					{
						lineNo++;
						string original = line;
						// same trimming/comment stripping as Load
						line = line.Trim();
						if (line.StartsWith("#"))
						{
							int j = 1;
							while (j < line.Length && Char.IsDigit(line[j])) j++;
							if (j > 1 && (j == line.Length || Char.IsWhiteSpace(line[j])))
							{
								line = line.Substring(j).TrimStart();
							}
						}
						int idxHash = line.IndexOf('#');
						int idxSemi = line.IndexOf(';');
						int idxSlash = line.IndexOf("//", StringComparison.Ordinal);
						int commentIdx = -1;
						if (idxHash >= 0) commentIdx = idxHash;
						if (idxSemi >= 0 && (commentIdx == -1 || idxSemi < commentIdx)) commentIdx = idxSemi;
						if (idxSlash >= 0 && (commentIdx == -1 || idxSlash < commentIdx)) commentIdx = idxSlash;
						if (commentIdx >= 0)
							line = line.Substring(0, commentIdx).Trim();
						if (line.Length == 0)
							continue;
						string[] tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
						SnapShot s = SnapShot.StartUpSnapShot();
						// tokens should be pairs of ints
						for (int i = 0; i < (int)(tokens.Length / 2); i++)
						{
							int from = 0;
							int to = 0;
							try
							{
								string sFrom = tokens[i*2];
								sFrom = sFrom == null ? "" : sFrom.Trim();
								string sTo = tokens[i*2+1];
								sTo = sTo == null ? "" : sTo.Trim();
								from = Int32.Parse(sFrom);
								to = Int32.Parse(sTo);
								Move m = new Move(from,to);
								// capture board before attempting move for better diagnostics
								string boardBefore = s.ToString();
								string legalBefore = s.LegalMoves != null ? s.LegalMoves.ToString() : "";
								// Validate by attempting the move on a clone; any invalid move will throw
								try
								{
									s.Clone().PerformMove(m);
								}
								catch (Exception ex)
								{
									string msg = "Line " + lineNo.ToString() + ": invalid move '" + sFrom + " " + sTo + "' - " + ex.Message + " -- " + original.Trim() + Environment.NewLine;
									msg += "Board before failure:" + Environment.NewLine + boardBefore + Environment.NewLine;
									msg += "Legal moves before failure:" + Environment.NewLine + legalBefore + Environment.NewLine;
									errors.Add(msg);
									// stop validating this line further
									break;
								}
								// move is valid for this position, now perform it on real snapshot
								s.PerformMove(m);
							}
							catch (FormatException fex)
							{
								errors.Add("Line " + lineNo.ToString() + ": invalid token format - " + fex.Message + " -- " + original.Trim());
								break;
							}
							catch (Exception ex)
							{
								errors.Add("Line " + lineNo.ToString() + ": unexpected error parsing moves - " + ex.Message + " -- " + original.Trim());
								break;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				errors.Add("Could not open/parse file: " + ex.Message);
			}
			return errors;
		}

		// private StoreMove( jcBoard, jcMov )
		private bool StoreMove(SnapShot snapShot, Move move)
		{
			// Where should we store this data?
			int key = Math.Abs(snapShot.GetHashCode() % TABLE_SIZE);
			int _lock = snapShot.GetHashLockCode();

			OpeningBookEntry entry = openingBookEntryTable[key];
			if (entry.MoveType != MoveType.Null && entry.Lock != _lock)
				return false;

			// And store the new move
			if (entry.MoveList == null)
				entry.MoveList = new MoveList();
			entry.Lock = _lock;
			entry.MoveType = (MoveType)snapShot.WhoToMove;
			entry.MoveList.Add(move.Clone());

			return true;
		}
	}


	public class TranspositionEntry
	{
		// Data fields, beginning with the actual value of the board and whether this
		// value represents an accurate evaluation or only a boundary
		public EvaluationType EvaluationType = EvaluationType.Null;

		public bool IsNullEntry
		{
			get
			{
				return EvaluationType == EvaluationType.Null;
			}
		}

		public int Evaluation = 0;

		// This value was obtained through a search to what depth?  0 means that
		// it was obtained during quiescence search (which is always effectively
		// of infinite depth but only within the quiescence domain; full-width
		// search of depth 1 is still more valuable than whatever Qsearch result)
		public int SearchDepth = 0;

		// Board position signature, used to detect collisions
		public long Lock = 0;

		// What this entry stored so long ago that it may no longer be useful?
		// Without this, the table will slowly become clogged with old, deep search
		// results for positions with no chance of happening again, and new positions
		// (specifically the 0-depth quiescence search positions) will never be
		// stored!
		public long TimeStamp = 0;

		// Optional best-move hint for move ordering on lookup.
		public int BestMoveFrom = -1;
		public int BestMoveTo = -1;

		// construction
		public TranspositionEntry()
		{
		}
	}


	public class TranspositionTable
	{
		// The size of a transposition table, in entries
		private const int TABLE_SIZE = 262144;//131072;

		// Data
		private TranspositionEntry[] table = null;

		/// <summary>
		/// Initializes an empty transposition table.
		/// </summary>
		public TranspositionTable()
		{
			table = new TranspositionEntry[TABLE_SIZE];
			for (int i = 0; i < TABLE_SIZE; i++)
				table[i] = new TranspositionEntry();
		}

		/// <summary>
		/// Looks up a stored evaluation for the given snapshot.
		/// </summary>
		/// <param name="snapShot">Position to query.</param>
		/// <param name="move">Output target to receive stored evaluation metadata.</param>
		/// <returns><c>true</c> when a matching table entry is found; otherwise <c>false</c>.</returns>
		public bool LookupBoard(SnapShot snapShot, Move move)
		{
			// Find the board's hash position in Table
			int key = Math.Abs(snapShot.GetHashCode() % TABLE_SIZE);
			TranspositionEntry entry = table[key];

			// If the entry is an empty placeholder, we don't have a match
			if (entry.IsNullEntry)
				return false;

			// Check for a hashing collision!
			if (entry.Lock != snapShot.GetHashLockCode())
				return false;

			// Now, we know that we have a match!  Copy it into the output parameter
			// and return
			move.Evaluation = entry.Evaluation;
			move.EvaluationType = entry.EvaluationType;
			move.SearchDepth = entry.SearchDepth;
			move.FieldNumberFrom = entry.BestMoveFrom;
			move.FieldNumberTo = entry.BestMoveTo;
			return true;
		}

		/// <summary>
		/// Stores an evaluation for the given snapshot in the transposition table.
		/// </summary>
		/// <param name="snapShot">Position signature to store.</param>
		/// <param name="evaluation">Evaluation score.</param>
		/// <param name="evaluationType">Bound accuracy type of the score.</param>
		/// <param name="searchDepth">Depth at which the score was obtained.</param>
		/// <param name="timeStamp">Search timestamp used for replacement decisions.</param>
		/// <returns><c>true</c> when the call completes (including when the existing entry is kept).</returns>
		public bool StoreSnapShot(SnapShot snapShot, int evaluation, EvaluationType evaluationType, int searchDepth, int timeStamp)
		{
			return StoreSnapShot(snapShot,evaluation,evaluationType,searchDepth,timeStamp,null);
		}

		/// <summary>
		/// Stores an evaluation and optional best move for the given snapshot in the transposition table.
		/// </summary>
		/// <param name="snapShot">Position signature to store.</param>
		/// <param name="evaluation">Evaluation score.</param>
		/// <param name="evaluationType">Bound accuracy type of the score.</param>
		/// <param name="searchDepth">Depth at which the score was obtained.</param>
		/// <param name="timeStamp">Search timestamp used for replacement decisions.</param>
		/// <param name="bestMove">Best move associated with the evaluation, if available.</param>
		/// <returns><c>true</c> when the call completes (including when the existing entry is kept).</returns>
		public bool StoreSnapShot(SnapShot snapShot, int evaluation, EvaluationType evaluationType, int searchDepth, int timeStamp, Move bestMove)
		{
			int key = Math.Abs(snapShot.GetHashCode() % TABLE_SIZE);
			TranspositionEntry entry = table[key];

			// Would we erase a more useful (i.e., higher) position if we stored this
			// one?  If so, don't bother!
			if (!entry.IsNullEntry && entry.SearchDepth > searchDepth && entry.TimeStamp >= timeStamp)
				return true;

			// And now, do the actual work
			entry.Lock = snapShot.GetHashLockCode();
			entry.Evaluation = evaluation;
			entry.SearchDepth = searchDepth;
			entry.EvaluationType = evaluationType;
			entry.TimeStamp = timeStamp;
			if (bestMove != null)
			{
				entry.BestMoveFrom = bestMove.FieldNumberFrom;
				entry.BestMoveTo = bestMove.FieldNumberTo;
			}
			else
			{
				entry.BestMoveFrom = -1;
				entry.BestMoveTo = -1;
			}
			return true;
		}
	} // TranspositionTable


	/// <summary>
	/// Global move-history heuristic table used for move ordering during search.
	/// 
	/// Tracks how often a move from a specific square to another square causes
	/// successful alpha-beta cutoffs. Moves with higher scores are prioritized in future
	/// searches to improve pruning effectiveness.
	/// 
	/// Storage layout:
	/// - history[colorIndex, fromSquare, toSquare]
	/// - colorIndex: 0 = White, 1 = Black
	/// - squares: 0..63 board indices
	/// 
	/// Implemented as singleton so all search instances share learning across moves.
	/// </summary>
	public class MoveHistoryTable
	{
		private int[,,] history = new int[2,64,64];

		// This is a singleton class; the same history can be shared by two AI's
		private static MoveHistoryTable singleInstance;

		static MoveHistoryTable()
		{
			singleInstance = new MoveHistoryTable();
		}

		/// <summary>
		/// Comparer that sorts moves by move-history heuristic score for a specific side.
		/// Higher history scores are ordered first.
		/// </summary>
		public class MoveComparer : IComparer
		{
			private int colorIndex = 0;
			/// <summary>
			/// Compares two moves by history score.
			/// </summary>
			/// <param name="x">First move.</param>
			/// <param name="y">Second move.</param>
			/// <returns>Negative when <paramref name="x"/> should come first; positive otherwise.</returns>
			public int Compare(object x, object y)
			{
				Move mx = x as Move;
				Move my = y as Move;
				if (GetInstance().history[colorIndex,mx.FieldNumberFrom,mx.FieldNumberTo] > GetInstance().history[colorIndex,my.FieldNumberFrom,my.FieldNumberTo])
					return -1;
				else
					return 1;
			}
			/// <summary>
			/// Initializes a comparer for the specified side.
			/// </summary>
			/// <param name="color">Side whose history scores should be used.</param>
			public MoveComparer(Color color)
			{
				if (color == Color.White)
					colorIndex = 0;
				else
					colorIndex = 1;
			}
		}

		/// <summary>
		/// Returns the singleton instance of the move history table.
		/// </summary>
		/// <returns>The shared move history table instance.</returns>
		public static MoveHistoryTable GetInstance()
		{
			return singleInstance;
		}

		/// <summary>
		/// Records a move occurrence for move-ordering heuristics.
		/// </summary>
		/// <param name="color">Side for which the move is being recorded.</param>
		/// <param name="move">Move to record; null values are ignored.</param>
		public void AddMove(Color color, Move move)
		{
			if (move == null)
				return;

			if (color == Color.White)
				history[0,move.FieldNumberFrom,move.FieldNumberTo]++;
			else
				history[1,move.FieldNumberFrom,move.FieldNumberTo]++;
		}

		/// <summary>
		/// Gets the accumulated move-history score for a move and side.
		/// </summary>
		/// <param name="color">Side to query.</param>
		/// <param name="move">Move to query.</param>
		/// <returns>History score; returns 0 when move is null.</returns>
		public int GetScore(Color color, Move move)
		{
			if (move == null)
				return 0;
			if (color == Color.White)
				return history[0,move.FieldNumberFrom,move.FieldNumberTo];
			return history[1,move.FieldNumberFrom,move.FieldNumberTo];
		}

		/// <summary>
		/// Resets all recorded move history counters.
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < 2; i++)
				for (int j = 0; j < 64; j++)
					for (int k = 0; k < 64; k++)
						history[i,j,k] = 0;
		}

		private MoveHistoryTable()
		{
		}
	} // MoveHistoryTable


	/// <summary>
	/// Represents a single chess move and associated game-state information.
	/// 
	/// A Move encapsulates:
	/// - Source and destination squares (FieldNumberFrom/To, 0-63)
	/// - Piece being moved and piece captured (if any)
	/// - Position evaluation and search depth (from transposition table or search)
	/// - Game-state snapshots (current position and prior position via SnapShot)
	/// - Move type information (regular move, castling, promotion, etc.)
	/// - Board situation before/after the move (for move legality checks)
	/// 
	/// Usage Contexts:
	/// 1. Move Generation: Moves in a legal move list with FieldNumber fields set
	/// 2. Search History: Moves in history with SnapShot pointing to position before move
	/// 3. Transposition Table Entry: Moves with Evaluation, EvaluationType, SearchDepth set
	/// 4. Principal Variation: Moves returned from search with Evaluation field indicating value
	/// 
	/// Comparison: Two moves are equal if they have the same from/to fields (piece/capture don't matter for move identity).
	/// ToString: Returns coordinate notation like "A2-A4".
	/// Clone: Creates a deep copy including any associated SnapShot.
	/// </summary>
	/// <summary>
	/// Represents a single chess move, including source/target squares, move metadata,
	/// tactical context, and search evaluation results.
	/// 
	/// A Move object serves two roles:
	/// 1. Game move representation (from/to, piece type, captures, SAN context)
	/// 2. Search result container (evaluation, bound type, search depth)
	/// 
	/// Core fields:
	/// - FieldNumberFrom / FieldNumberTo: Source and destination square indices (0..63)
	/// - PieceType / PieceHit: Moving and captured piece types
	/// - MoveCode: Special move classification (castling, promotion, resign, etc.)
	/// - OwnSituationCode / OpponentSituationCode: Tactical state (check/checkmate/stalemate)
	/// - Evaluation / EvaluationType / SearchDepth: Search metadata for transposition table
	/// - SnapShot: Optional reference to pre-move position for history/reconstruction
	/// - PromotionPieceType: Promotion target for pawn promotions (if applicable)
	/// </summary>
	public class Move
	{
		public int FieldNumberFrom = -1;
		public int FieldNumberTo = -1;

		public PieceType PieceType = PieceType.None;
		public MoveCode MoveCode = MoveCode.Unknown;
		public PieceType PieceHit = PieceType.None;
		public SituationCode OwnSituationCode = SituationCode.Unknown;
		public SituationCode OpponentSituationCode = SituationCode.Unknown;

		public SnapShot SnapShot = null;

		public int Evaluation;
		public EvaluationType EvaluationType;
		public int SearchDepth;
		
		// For pawn promotion moves, stores which piece type the pawn should promote to
		// If null, the event handler will be called to let user choose
		// If set, this specific piece type will be used (for SAN-based moves)
		public PromotionPieceType? PromotionPieceType = null;

		/// <summary>
		/// Compares two moves by source and destination field numbers.
		/// </summary>
		/// <param name="o">Object to compare with.</param>
		/// <returns><c>true</c> when both moves target the same from/to fields; otherwise <c>false</c>.</returns>
		public override bool Equals(object o)
		{
			Move m = o as Move;
			if (m != null && m.FieldNumberFrom == this.FieldNumberFrom && m.FieldNumberTo == this.FieldNumberTo)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns a hash code based on the coordinate move representation.
		/// </summary>
		/// <returns>A hash code for this move.</returns>
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		/// <summary>
		/// Returns coordinate notation in the form <c>A2-A4</c>.
		/// </summary>
		/// <returns>Coordinate move text.</returns>
		public override string ToString()
		{
			return Helper.FieldNumber2String(this.FieldNumberFrom)+"-"+Helper.FieldNumber2String(this.FieldNumberTo);
		}

		/// <summary>
		/// Returns move text and optionally prefixes snapshot information.
		/// </summary>
		/// <param name="includeSnapShot">Whether to include snapshot text when available.</param>
		/// <returns>Formatted move string.</returns>
		public string ToString(bool includeSnapShot)
		{
			string retVal = "";
			if (includeSnapShot && this.SnapShot != null)
					retVal = this.SnapShot.ToString()+": -> ";
			retVal = retVal+this.ToString();
			return retVal;
		}
		
		/// <summary>
		/// Creates the inverse move by swapping source and destination fields.
		/// </summary>
		/// <returns>The inverse move.</returns>
		public Move Inverse()
		{
			return new Move(this.FieldNumberTo,this.FieldNumberFrom);
		}

		/// <summary>
		/// Creates a deep clone of this move and its optional snapshot.
		/// </summary>
		/// <returns>A cloned move instance.</returns>
		public Move Clone()
		{
			Move m = new Move();
			m.FieldNumberFrom = this.FieldNumberFrom;
			m.FieldNumberTo = this.FieldNumberTo;
			m.PieceType = this.PieceType;
			m.MoveCode = this.MoveCode;
			m.PieceHit = this.PieceHit;
			m.OwnSituationCode = this.OwnSituationCode;
			m.OpponentSituationCode = this.OpponentSituationCode;
			m.SnapShot = this.SnapShot == null ? null : this.SnapShot.Clone();
			m.Evaluation = this.Evaluation;
			m.EvaluationType = this.EvaluationType;
			m.SearchDepth = this.SearchDepth;
			m.PromotionPieceType = this.PromotionPieceType;
			return m;
		}

		/// <summary>
		/// Initializes an empty move with unset fields.
		/// </summary>
		public Move()
		{
		}

		/// <summary>
		/// Initializes a move using internal field numbers.
		/// </summary>
		/// <param name="fieldNumberFrom">Source field index (0..63).</param>
		/// <param name="fieldNumberTo">Destination field index (0..63).</param>
		/// <exception cref="ArgumentException">Thrown when either field number is outside 0..63.</exception>
		public Move(int fieldNumberFrom, int fieldNumberTo)
		{
			if (fieldNumberFrom < 0 || fieldNumberFrom > 63)
				throw new ArgumentException("Invalid","fieldNumberFrom");
			if (fieldNumberTo < 0 || fieldNumberTo > 63)
				throw new ArgumentException("Invalid","fieldNumberTo");
			this.FieldNumberFrom = fieldNumberFrom;
			this.FieldNumberTo = fieldNumberTo;
		}

	} // Move
	
	/// <summary>
	/// Collection of <see cref="Move"/> objects with optional duplicate suppression.
	/// 
	/// Provides a chess-specific move list abstraction on top of ArrayList,
	/// including utility methods for cloning, sorting, SAN conversion, and filtering.
	/// 
	/// Key behavior:
	/// - Duplicate suppression can be enabled/disabled via SuppressDuplicates
	/// - Supports ICollection interface for interoperability
	/// - Preserves insertion order unless explicitly sorted
	/// </summary>
	public class MoveList : ICollection
	{
		private ArrayList moves = new ArrayList();

		private bool suppressDuplicates = true;
		/// <summary>
		/// Gets or sets whether duplicate moves are ignored when adding to the list.
		/// </summary>
		public bool SuppressDuplicates
		{
			get
			{
				return suppressDuplicates;
			}
			set
			{
				suppressDuplicates = value;
			}
		}

		/// <summary>
		/// Copies the list contents to an array beginning at the specified index.
		/// </summary>
		/// <param name="array">The destination array.</param>
		/// <param name="index">Start index in <paramref name="array"/>.</param>
		public virtual void CopyTo(Array array, int index)
		{
			moves.CopyTo(array,index);
		}

		/// <summary>
		/// Gets the number of moves currently stored.
		/// </summary>
		public virtual int Count
		{
			get
			{
				return moves.Count;
			}
		}

		/// <summary>
		/// Sorts moves by history-heuristic score for the given side.
		/// </summary>
		/// <param name="color">The side whose history table should be used.</param>
		public virtual void Sort(Color color)
		{
			SortedList sl = new SortedList(new MoveHistoryTable.MoveComparer(color));
			for (int i = 0; i < Count; i++)
				sl.Add(this[i],null);
			this.Clear();
			for (int i = 0; i < sl.Count; i++)
				this.Add((Move)sl.GetKey(i));
		}

		/// <summary>
		/// Returns an enumerator for iterating through the move list.
		/// </summary>
		/// <returns>An enumerator over the contained moves.</returns>
		public virtual IEnumerator GetEnumerator()
		{
			return moves.GetEnumerator();
		}

		/// <summary>
		/// Gets whether access to the underlying collection is synchronized (thread-safe).
		/// </summary>
		public virtual bool IsSynchronized
		{
			get
			{
				return moves.IsSynchronized;
			}
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the collection.
		/// </summary>
		public virtual object SyncRoot
		{
			get
			{
				return moves.SyncRoot;
			}
		}

		/// <summary>
		/// Determines whether an equivalent move is present in the list.
		/// </summary>
		/// <param name="move">The move to search for.</param>
		/// <returns><c>true</c> when a matching move exists; otherwise <c>false</c>.</returns>
		public virtual bool Contains(Move move)
		{
			if (move == null)
				return false;
			for (int i = 0; i < moves.Count; i++)
			{
				if (((Move)moves[i]).Equals(move))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Adds a move to the list.
		/// </summary>
		/// <param name="move">The move to add.</param>
		/// <remarks>Null values and suppressed duplicates are ignored.</remarks>
		public virtual void Add(Move move)
		{
			if (move == null || (suppressDuplicates && this.Contains(move)))
				return;
			moves.Add(move);
		}

		/// <summary>
		/// Appends all moves from another list.
		/// </summary>
		/// <param name="moves">Source list to merge from.</param>
		public virtual void Merge(MoveList moves)
		{
			if (moves == null)
				return;
			for (int i = 0; i < moves.Count; i++)
				this.Add(moves[i]);
		}

		/// <summary>
		/// Removes all matching occurrences of a move.
		/// </summary>
		/// <param name="move">The move to remove.</param>
		public virtual void Remove(Move move)
		{
			if (move == null)
				return;
			for (int i = 0; i < moves.Count; i++)
			{
				if (((Move)moves[i]).Equals(move))
					moves.RemoveAt(i);
			}
		}

		/// <summary>
		/// Removes a move at a given index.
		/// </summary>
		/// <param name="index">Index to remove; out-of-range values are ignored.</param>
		public virtual void RemoveAt(int index)
		{
			if (moves.Count == 0 || index < 0 || index+1 > moves.Count)
				return;
			moves.RemoveAt(index);
		}

		/// <summary>
		/// Removes all moves from the list.
		/// </summary>
		public virtual void Clear()
		{
			moves.Clear();
		}

		/// <summary>
		/// Gets a move by zero-based index.
		/// </summary>
		/// <param name="index">Requested index.</param>
		/// <returns>The move at <paramref name="index"/>, or <c>null</c> when out of range.</returns>
		public virtual Move this[int index]
		{
			get
			{
				if (moves.Count == 0 || index < 0 || index+1 > moves.Count)
					return null;
				return (Move)moves[index];
			}
		}

		/// <summary>
		/// Gets the first move matching the given source and destination fields.
		/// </summary>
		/// <param name="fieldNumberFrom">Source field index (0..63).</param>
		/// <param name="fieldNumberTo">Destination field index (0..63).</param>
		/// <returns>A matching move, or <c>null</c> when none exists or input is out of range.</returns>
		public virtual Move this[int fieldNumberFrom, int fieldNumberTo]
		{
			get
			{
				if (moves.Count == 0 || fieldNumberFrom < 0 || fieldNumberFrom > 63 || fieldNumberTo < 0 || fieldNumberTo > 63)
					return null;
				for (int i = 0; i < this.Count; i++)
				{
					Move m = this[i];
					if (m.FieldNumberFrom == fieldNumberFrom && m.FieldNumberTo == fieldNumberTo)
						return m;
				}
				return null;
			}
		}

		/// <summary>
		/// Creates a deep clone of this list and its move entries.
		/// </summary>
		/// <returns>A cloned move list.</returns>
		public MoveList Clone()
		{
			MoveList ml = new MoveList();
			ml.suppressDuplicates = this.suppressDuplicates;
			for (int i = 0; i < this.Count; i++)
				ml.Add(this[i].Clone());
			return ml;
		}

		/// <summary>
		/// Returns coordinate-notation text for all moves, separated by semicolons.
		/// </summary>
		/// <returns>Formatted coordinate move list string.</returns>
		public override string ToString()
		{
			string retVal = "";
			for (int i = 0; i < moves.Count; i++)
			{
				retVal += ((Move)moves[i]).ToString() + (i == moves.Count-1 ? "" : ";");
				if ((i + 1) < moves.Count && ((i + 1) % 10) == 0)
					retVal += Environment.NewLine;
			}
			return retVal;
		}

		/// <summary>
		/// Formats the move list as SAN values for a specific position context.
		/// </summary>
		/// <param name="s">Snapshot used to convert each move to SAN.</param>
		/// <returns>Semicolon-separated SAN text. Individual conversion failures are rendered as <c>ERROR</c>.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="s"/> is null.</exception>
		public string ToSanString(SnapShot s)
		{
			if (s == null)
				throw new ArgumentException("Invalid","s");

			string retVal = "";
			for (int i = 0; i < moves.Count; i++)
			{
				Move move = (Move)moves[i];
				string san = "";
				try
				{
					san = s.MoveToSan(move);
				}
				catch
				{
					san = "ERROR";
				}

				retVal += san + (i == moves.Count-1 ? "" : ";");
				if ((i + 1) < moves.Count && ((i + 1) % 10) == 0)
					retVal += Environment.NewLine;
			}
			return retVal;
		}
	} // MoveList


	/// <summary>
	/// Delegate for notifications when a piece is captured.
	/// </summary>
	/// <param name="sender">Event source.</param>
	/// <param name="e">Capture event data.</param>
	public delegate void PieceHitEventHandler(object sender, PieceHitEventArgs e);
	
	/// <summary>
	/// Event data for a captured piece notification.
	/// </summary>
	public class PieceHitEventArgs : EventArgs
	{
		private PieceType pieceHit = PieceType.None;
		/// <summary>
		/// Gets the type of piece that was captured.
		/// </summary>
		public PieceType PieceHit
		{
			get
			{
				return pieceHit;
			}
		}
		/// <summary>
		/// Initializes capture event data.
		/// </summary>
		/// <param name="pieceHit">Captured piece type.</param>
		public PieceHitEventArgs(PieceType pieceHit)
		{
			if (pieceHit == PieceType.None)
				throw new ArgumentException("Invalid","pieceHit");
			this.pieceHit = pieceHit;
		}
	}

	/// <summary>
	/// Delegate for pawn-promotion selection callbacks.
	/// </summary>
	/// <param name="sender">Event source.</param>
	/// <param name="e">Promotion selection event data.</param>
	public delegate void PromotePawnEventHandler(object sender, PromotePawnEventArgs e);
	
	/// <summary>
	/// Event data for pawn promotion requests.
	/// </summary>
	public class PromotePawnEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the selected promotion piece. Defaults to queen.
		/// </summary>
		public PromotionPieceType PromoteTo = PromotionPieceType.Queen;
	}

	/// <summary>
	/// Delegate for check-state notifications.
	/// </summary>
	/// <param name="sender">Event source.</param>
	/// <param name="e">Check event data.</param>
	public delegate void CheckEventHandler(object sender, CheckEventArgs e);
	
	/// <summary>
	/// Event data for check notifications.
	/// </summary>
	public class CheckEventArgs : EventArgs
	{
	}

	/// <summary>
	/// Delegate for game-over notifications.
	/// </summary>
	/// <param name="sender">Event source.</param>
	/// <param name="e">Game-over event data.</param>
	public delegate void GameOverEventHandler(object sender, GameOverEventArgs e);
	
	/// <summary>
	/// Event data for game-over notifications.
	/// </summary>
	public class GameOverEventArgs : EventArgs
	{
	}


	/// <summary>
	/// Represents a single chess position (board state) snapshot, including piece placement,
	/// side to move, castling rights, legal moves, history, and evaluation caches.
	/// 
	/// SnapShot is the core data structure for board representation and manipulation.
	/// It maintains both the current position and a linked history of moves that led to this position,
	/// enabling move rollback, repetition detection, and game-state analysis.
	/// 
	/// Key Features:
	/// - Piece placement via situation[] byte array (piece type and color encoded)
	/// - King position caching for efficient move legality checks
	/// - Legal move generation and caching
	/// - Move history with bidirectional linking (via Move.SnapShot)
	/// - Hashing via Zobrist keys (GetHashCode, GetHashLockCode)
	/// - Move normalization keys for repetition detection (GetRepetitionHashCode, GetRepetitionLockCode)
	/// - Complete game-phase evaluation (material + positional + endgame heuristics)
	/// - Draw detection (insufficient material, threefold repetition, fifty-move rule)
	/// - FEN position import/export for test setup (FromFen)
	/// 
	/// All positions are immutable after creation; modifications use PerformMove and Rollback
	/// to maintain history and enable efficient undo.
	/// </summary>
	public class SnapShot
	{
		private static BestMovePicker MTDPicker = new MTD();
		private static BestMovePicker MTDPickerV2 = new MTDv2();
//		private static BestMovePicker AlphaBetaPicker = new AlphaBeta();
		private static Hashtable openingBooks = new Hashtable();

		/// <summary>
	/// Abstract base class for chess move selection algorithms (engines).
	/// 
	/// Defines the common interface and shared infrastructure for search engines.
	/// Subclasses implement concrete search strategies (e.g., MTDv2, simple alpha-beta, minimax).
	/// 
	/// Shared Components:
	/// - TranspositionTable (tTable): Caches evaluated positions to avoid redundant computation
	/// - MoveHistoryTable (hTable): Tracks move success patterns for move ordering heuristics
	/// - Alpha-beta search constants and boundaries
	/// - Search statistics (nodes, cutoffs, transposition table hits)
	/// - Abstract GetBestMove method for subclass implementation
	/// 
	/// Transposition Table: Stores board positions with their evaluated scores, search depth,
	/// and bound types (exact, lower bound, upper bound) to enable move ordering and significant
	/// search pruning via cutoff detection.
	/// 
	/// Move History: Records how often each move at each position leads to cutoffs,
	/// allowing engines to prioritize promising moves in future searches.
	/// </summary>
	public abstract class BestMovePicker
		{
			// A transposition table for this object
			protected TranspositionTable tTable = new TranspositionTable();

			// A handle to the system's history table
			protected MoveHistoryTable hTable = MoveHistoryTable.GetInstance();

			// How will we assess position strengths?
			//		protected jcBoardEvaluator Evaluator;
			protected Color fromWhosePerspective;
			

			// Search node types: MAXNODEs are nodes where the computer player is the
			// one to move; MINNODEs are positions where the opponent is to move.
			protected const bool MAXNODE = true;
			protected const bool MINNODE = false;

			protected int maxQuiescenceDepth = 1000;

			// Alphabeta search boundaries
			protected const int ALPHABETA_MAXVAL = 30000;
			protected const int ALPHABETA_MINVAL = -30000;
			protected const int ALPHABETA_ILLEGAL = -31000;

			// An approximate upper bound on the total value of all positional
			// terms in the evaluation function
			protected const int EVAL_THRESHOLD = 200;

			// A score below which we give up: if Alphabeta ever returns a value lower
			// than this threshold, then all is lost and we might as well resign.  Here,
			// the value is equivalent to "mated by the opponent in 3 moves or less".
			protected const int ALPHABETA_GIVEUP = -29995;

			// Statistics
			protected int numRegularNodes = 0;
			protected int numQuiescenceNodes = 0;
			protected int numRegularTTHits = 0;
			protected int numQuiescenceTTHits = 0;
			protected int numRegularCutoffs = 0;
			protected int numQuiescenceCutoffs = 0;

			// A move counter, so that the agent knows when it can delete old stuff from
			// its transposition table
			protected int moveCounter = 0;

			/// <summary>
			/// Initializes a new move picker base instance with shared search infrastructure.
			/// </summary>
			public BestMovePicker()
			{
				//			Evaluator = new jcBoardEvaluator();
			}

			// int AlphaBeta
			// The basic alpha-beta algorithm, used in one disguise or another by
			// every search agent class
			protected int AlphaBeta(bool nodeType, SnapShot snapShot, int depth, int alpha, int beta)
			{
				Move move = new Move();

				// Count the number of nodes visited in the full-width search
				numRegularNodes++;

				// First things first: let's see if there is already something useful
				// in the transposition table, which might save us from having to search
				// anything at all
				if (tTable.LookupBoard(snapShot,move) && (move.SearchDepth >= depth))
				{
					if (nodeType == MAXNODE)
					{
						if (move.EvaluationType == EvaluationType.Accurate || move.EvaluationType == EvaluationType.Lowerbound)
						{
							if (move.Evaluation >= beta)
							{
								numRegularTTHits++;
								return move.Evaluation;
							}
						}
					}
					else
					{
						if (move.EvaluationType == EvaluationType.Accurate || move.EvaluationType == EvaluationType.Upperbound)
						{
							if (move.Evaluation <= alpha)
							{
								numRegularTTHits++;
								return move.Evaluation;
							}
						}
					}
				}

				// If we have reached the maximum depth of the search, stop recursion
				// and begin quiescence search
				if (depth == 0)
				{
					return QuiescenceSearch(nodeType,snapShot,maxQuiescenceDepth,alpha,beta);
				}

				snapShot.CalculateLegalMoves();

				// Sort the moves according to History heuristic values
				snapShot.legalMoves.Sort(snapShot.whoToMove);

				// OK, now, get ready to search
				int bestSoFar;

				if (nodeType == MAXNODE) // Case #1: We are searching a Max Node
				{
					bestSoFar = ALPHABETA_MINVAL;
					int currentAlpha = alpha;

					// Loop on the successors
					for (int i = 0; i < snapShot.legalMoves.Count; i++)
					{
						move = snapShot.legalMoves[i];

						// Compute a board position resulting from the current successor
						snapShot.PerformMove(move,true);
moveCounter++;
						// And search it in turn
						int moveScore = AlphaBeta(!nodeType,snapShot,depth-1,currentAlpha,beta);
moveCounter--;
						snapShot.Rollback(1);

						currentAlpha = Math.Max(currentAlpha,moveScore);

						// Is the current successor better than the previous best?
						if (moveScore > bestSoFar)
						{
							bestSoFar = moveScore;
							// Can we cutoff now?
							if (bestSoFar >= beta)
							{
								// Store this best move in the TransTable
								tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Upperbound,depth,moveCounter);

								// Add this move's efficiency in the HistoryTable
								hTable.AddMove(snapShot.whoToMove,move);
								numRegularCutoffs++;
								return bestSoFar;
							}
						}
					}

					// Test for checkmate or stalemate
					// Both cases occur if and only if there is no legal move for MAX, i.e.,
					// if "bestSoFar" is ALPHABETA_MINVAL.  There are two cases: we
					// have checkmate (in which case the score is accurate) or stalemate (in
					// which case the position should be re-scored as a draw with value 0.
					if (snapShot.legalMoves.Count == 0)
					{
						// If MIN can capture MAX's king, we have checkmate and must return MINVAL.  We
						// add the depth simply to "favor" delaying tactics: a mate in 5 will
						// score higher than a mate in 3, because the likelihood that the
						// opponent will miss it is higher; might as well make life difficult!
						if (snapShot.history != null && snapShot.history.Count > 0 && snapShot.history[snapShot.history.Count-1].OpponentSituationCode == SituationCode.Check)
							return bestSoFar + depth;
						else
							return 0;
					}
				}
				else // Case #2: Min Node
				{
					bestSoFar = ALPHABETA_MAXVAL;
					int currentBeta = beta;

					for (int i = 0; i < snapShot.legalMoves.Count; i++)
					{
						move = snapShot.legalMoves[i];

						// Compute a board position resulting from the current successor
						snapShot.PerformMove(move,true);
moveCounter++;
						// And search it in turn
						int moveScore = AlphaBeta(!nodeType,snapShot,depth-1,alpha,currentBeta);
moveCounter--;
						snapShot.Rollback(1);

						currentBeta = Math.Min(currentBeta,moveScore);

						if (moveScore < bestSoFar)
						{
							bestSoFar = moveScore;
							// Cutoff?
							if (bestSoFar <= alpha)
							{
								tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Upperbound,depth,moveCounter);
								hTable.AddMove(snapShot.whoToMove,move);
								numRegularCutoffs++;
								return bestSoFar;
							}
						}
					}

					// Test for checkmate or stalemate
					if (snapShot.legalMoves.Count == 0)
					{
						if (snapShot.history != null && snapShot.history.Count > 0 && snapShot.history[snapShot.history.Count-1].OpponentSituationCode == SituationCode.Check)
							return bestSoFar - depth; //bestSoFar + depth;
						else
							return 0;
					}
				}

				// If we haven't returned yet, we have found an accurate minimax score
				// for a position which is neither a checkmate nor a stalemate
				tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Accurate,depth,moveCounter);
				return bestSoFar;
			}

			// int QuiescenceSearch
			// A slight variant of alphabeta which only considers captures and null moves
			// This is necesary because the evaluation function can only be applied to
			// "quiet" positions where the tactical situation (i.e., material balance) is
			// unlikely to change in the near future.
			/// <summary>
			/// Performs quiescence search from the given node.
			/// 
			/// This base implementation is used by legacy engines; it extends search at leaf nodes
			/// by exploring tactical continuations (captures) to reduce horizon effects.
			/// </summary>
			/// <param name="nodeType">Node type: <c>MAXNODE</c> or <c>MINNODE</c>.</param>
			/// <param name="snapShot">Current board position.</param>
			/// <param name="depth">Remaining quiescence depth.</param>
			/// <param name="alpha">Alpha lower bound.</param>
			/// <param name="beta">Beta upper bound.</param>
			/// <returns>Quiescence evaluation score.</returns>
			public int QuiescenceSearch(bool nodeType, SnapShot snapShot, int depth, int alpha, int beta)
			{
				Move move = new Move();
				numQuiescenceNodes++;

				// First things first: let's see if there is already something useful
				// in the transposition table, which might save us from having to search
				// anything at all
				if (tTable.LookupBoard(snapShot,move))
				{
					if (nodeType == MAXNODE)
					{
						if (move.EvaluationType == EvaluationType.Accurate || move.EvaluationType == EvaluationType.Lowerbound)
						{
							if (move.Evaluation >= beta)
							{
								numQuiescenceTTHits++;
								return move.Evaluation;
							}
						}
					}
					else
					{
						if (move.EvaluationType == EvaluationType.Accurate || move.EvaluationType == EvaluationType.Upperbound)
						{
							if (move.Evaluation <= alpha)
							{
								numQuiescenceTTHits++;
								return move.Evaluation;
							}
						}
					}
				}

				int bestSoFar = ALPHABETA_MINVAL;

				// Start with evaluation of the null-move, just to see whether it is more
				// effective than any capture, in which case we must stop looking at
				// captures and damaging our position
				// NOTE: If the quick evaluation is enough to cause a cutoff, we don't store
				// the value in the transposition table.  EvaluateMaterial is so fast that we
				// wouldn't gain anything, and storing the value might force us to erase a
				// more valuable entry in the table.
				bestSoFar = (snapShot.EvaluateMaterial(fromWhosePerspective) >> 3) << 3;

				if (bestSoFar > (beta + EVAL_THRESHOLD) || bestSoFar < (alpha - EVAL_THRESHOLD))
					return bestSoFar;
				else
				{
					snapShot.CalculateLegalMoves();
					bestSoFar = (snapShot.EvaluateComplete(fromWhosePerspective) >> 3) << 3;
				}

				// Now, look at captures
				if (!snapShot.HitPossible || depth == 0)
					return bestSoFar;

				// Sort the moves according to History heuristic values
				snapShot.legalMoves.Sort(snapShot.whoToMove);

				// Case #1: We are searching a Max Node
				if (nodeType == MAXNODE )
				{
					int currentAlpha = alpha;

					// Loop on the successors
					for (int i = 0; i < snapShot.legalMoves.Count; i++)
					{
						move = snapShot.legalMoves[i];

						if (move.PieceHit == PieceType.None)
							continue;

						// Compute a board position resulting from the current successor
						snapShot.PerformMove(move,true);
moveCounter++;
						// And search it in turn
						int moveScore = QuiescenceSearch(!nodeType,snapShot,depth-1,currentAlpha,beta);
moveCounter--;
						snapShot.Rollback(1);

						currentAlpha = Math.Max(currentAlpha,moveScore);

						// Is the current successor better than the previous best?
						if (moveScore > bestSoFar)
						{
							bestSoFar = moveScore;
							// Can we cutoff now?
							if (bestSoFar >= beta)
							{
								tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Upperbound,0,moveCounter);
								// Add this move's efficiency in the HistoryTable
								hTable.AddMove(snapShot.whoToMove,move);
								numQuiescenceCutoffs++;
								return bestSoFar;
							}
						}
					}

					// Test for checkmate or stalemate
					if (snapShot.legalMoves.Count == 0)
					{
						if (snapShot.history != null && snapShot.history.Count > 0 && snapShot.history[snapShot.history.Count-1].OpponentSituationCode == SituationCode.Check)
							return bestSoFar;
						else
							return 0;
					}
				}
				else // Case #2: Min Node
				{
					int currentBeta = beta;

					// Loop on the successors
					for (int i = 0; i < snapShot.legalMoves.Count; i++)
					{
						move = snapShot.legalMoves[i];

						if (move.PieceHit == PieceType.None)
							continue;

						// Compute a board position resulting from the current successor
						snapShot.PerformMove(move,true);
moveCounter++;
						int moveScore = QuiescenceSearch(!nodeType,snapShot,depth-1,alpha,currentBeta);
moveCounter--;
						snapShot.Rollback(1);

						currentBeta = Math.Min(currentBeta,moveScore);

						if (moveScore < bestSoFar)
						{
							bestSoFar = moveScore;
							// Cutoff?
							if (bestSoFar <= alpha)
							{
								tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Upperbound,0,moveCounter);
								hTable.AddMove(snapShot.whoToMove,move);
								numQuiescenceCutoffs++;
								return bestSoFar;
							}
						}
					}

					// Test for checkmate or stalemate
					if (snapShot.legalMoves.Count == 0)
					{
						if (snapShot.history != null && snapShot.history.Count > 0 && snapShot.history[snapShot.history.Count-1].OpponentSituationCode == SituationCode.Check)
							return bestSoFar;
						else
							return 0;
					}
				}

				// If we haven't returned yet, we have found an accurate minimax score
				// for a position which is neither a checkmate nor a stalemate
				tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Accurate,0,moveCounter);
				return bestSoFar;
			}

			// Each picker class needs some way of picking a move!
			/// <summary>
			/// Selects the best move for the current side to move.
			/// </summary>
			/// <param name="snapShot">Position to evaluate.</param>
			/// <param name="openings">Optional opening book to query before search.</param>
			/// <returns>Best move according to the concrete engine implementation.</returns>
			public abstract Move GetBestMove(SnapShot snapShot, OpeningBook openings);

		} // BestMovePicker

/*
		public class AlphaBeta : BestMovePicker
		{
			public AlphaBeta() : base()
			{
			}

			// Implementation of the abstract method defined in the superclass
			public override Move GetBestMove(SnapShot snapShot, OpeningBook openings)
			{
				Console.WriteLine("Getting best move by AlphaBeta algorithm...");

				// First things first: look in the Opening Book, and if it contains a
				// move for this position, don't search anything
				moveCounter++;
				Move move = null;
				if (openings != null)
				{
					move = openings.Query(snapShot);
					if (move != null )
						return move;
				}

				// Store the identity of the moving side, so that we can tell Evaluator
				// from whose perspective we need to evaluate positions
				fromWhosePerspective = snapShot.whoToMove;

				// Should we erase the history table?
				if ((Helper.RandomNumber() % 4 ) == 2)
					hTable.Clear();

				numRegularNodes = 0;
				numQuiescenceNodes = 0;
				numRegularTTHits = 0;
				numQuiescenceTTHits = 0;

				snapShot.CalculateLegalMoves();

				snapShot.legalMoves.Sort(snapShot.whoToMove);

				Move bestMove = null;

				// The following code blocks look a lot like the MAX node case from
				// BestMovePicker.AlphaBeta, with an added twist: we need to store the
				// actual best move, and not only pass around its minimax value
				int bestSoFar = ALPHABETA_MINVAL;
				int currentAlpha = ALPHABETA_MINVAL;

				// Loop on all pseudo-legal moves
				for (int i = 0; i < snapShot.legalMoves.Count; i++)
				{
					move = snapShot.legalMoves[i];

					snapShot.PerformMove(move,true);

					int moveScore = AlphaBeta(MINNODE,snapShot,2,currentAlpha,ALPHABETA_MAXVAL);

					currentAlpha = Math.Max(currentAlpha,moveScore);

					if (moveScore > bestSoFar)
					{
						bestMove = move.Clone();
						bestSoFar = moveScore;
						bestMove.Evaluation = moveScore;
					}
				}

				// Test for checkmate or stalemate
				if (bestSoFar <= ALPHABETA_GIVEUP)
				{
					if (snapShot.legalMoves.Count > 0)
					{
						bool bGameOverPossible = false;
						for (int i = 0; i < snapShot.legalMoves.Count; i++)
						{
							move = snapShot.legalMoves[i];

							snapShot.PerformMove(move);

							for (int j = 0; j < snapShot.legalMoves.Count; j++)
							{
								move = snapShot.legalMoves[j];

								snapShot.PerformMove(move);
								if (snapShot.legalMoves.Count == 0)
									bGameOverPossible = true;

								snapShot.Rollback(1);

								if (bGameOverPossible)
									break;
							}

							snapShot.Rollback(1);

							if (bGameOverPossible)
								break;
						}
						if (bGameOverPossible)
						{
							bestMove = new Move(snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField,snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField);
							bestMove.PieceType = PieceType.King;
							bestMove.MoveCode = MoveCode.Resign;
						}
					}
					else
					{
						// We're in check and our best hope is GIVEUP or worse, so either we are
						// already checkmated or will be soon, without hope of escape
						bestMove = new Move(snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField,snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField);
						bestMove.PieceType = PieceType.King;
						bestMove.MoveCode = MoveCode.Resign;
					}
				}

				Console.Write("  --> Transposition Table hits for regular nodes: ");
				Console.WriteLine(numRegularTTHits + " of " + numRegularNodes);
				Console.Write("  --> Transposition Table hits for quiescence nodes: ");
				Console.WriteLine(numQuiescenceTTHits + " of " + numQuiescenceNodes);

				return bestMove;
			}
		}
*/

		/// <summary>
		/// Legacy MTD(f) search engine implementation kept for comparison/backward compatibility.
		/// </summary>
		public class MTD : BestMovePicker
		{
			// A measure of the effort we are willing to expend on search
			/// <summary>
			/// Gets or sets the maximum quiescence search depth for the legacy MTD engine.
			/// </summary>
			public int MaxQuiescenceDepth
			{
				get
				{
					return maxQuiescenceDepth;
				}
				set
				{
					maxQuiescenceDepth = value;
				}
			}

			private int maxIterationDepth = 15;
			public int MaxIterationDepth
			{
				get
				{
					return maxIterationDepth;
				}
				set
				{
					maxIterationDepth = value;
				}
			}

			private int maxSearchSize = 50000;
			public int MaxSearchSize
			{
				get
				{
					return maxSearchSize;
				}
				set
				{
					maxSearchSize = value;
				}
			}

			/// <summary>
			/// Initializes a legacy MTD search engine instance.
			/// </summary>
			public MTD() : base()
			{
			}

			// Move selection: An iterative-deepening paradigm calling MTD(f) repeatedly
			public override Move GetBestMove(SnapShot snapShot, OpeningBook openings)
			{
				Console.WriteLine("Getting best move by MTD algorithm...");

				// First things first: look in the Opening Book, and if it contains a
				// move for this position, don't search anything
				moveCounter++;
				Move move = null;
				if (openings != null)
				{
					move = openings.Query(snapShot);
					if (move != null )
						return move;
				}

				// Store the identity of the moving side, so that we can tell Evaluator
				// from whose perspective we need to evaluate positions
				fromWhosePerspective = snapShot.whoToMove;

				// Should we erase the history table?
				if ((Helper.RandomNumber() % 6) == 2)
					hTable.Clear();

				// Begin search.  The search's maximum depth is determined on the fly,
				// according to how much effort has been spent; if it's possible to search
				// to depth 8 in 5 seconds, then by all means, do it!
				int bestGuess = 0;
				int iterdepth = 1;

				while (true)
				{
					// Searching to depth 1 is not very effective, so we begin at 2
					iterdepth++;

					// Compute efficiency statistics
					numRegularNodes = 0;
					numQuiescenceNodes = 0;
					numRegularTTHits = 0;
					numQuiescenceTTHits = 0;
					numRegularCutoffs = 0;
					numQuiescenceCutoffs = 0;

					// Look for a move at the current depth
					move = MTD_f(snapShot,bestGuess,iterdepth);
					bestGuess = move.Evaluation;

					// Feedback!
					Console.WriteLine("Iteration of depth " + iterdepth + "; best move = " + move.ToString());
					Console.Write("  --> Transposition Table hits for regular nodes: ");
					Console.WriteLine(numRegularTTHits + " of " + numRegularNodes);
					Console.Write("  --> Transposition Table hits for quiescence nodes: ");
					Console.WriteLine(numQuiescenceTTHits + " of " + numQuiescenceNodes);
					Console.WriteLine("  --> Number of cutoffs for regular nodes: " + numRegularCutoffs);
					Console.WriteLine("  --> Number of cutoffs in quiescence search: " + numQuiescenceCutoffs);

					// Get out if we have searched deep enough
					if ((numRegularNodes + numQuiescenceNodes ) >= maxSearchSize)
						break;
					if (iterdepth >= maxIterationDepth)
						break;
				}

				return move;
			}

			// Use the MTD algorithm to find a good move.  MTD repeatedly calls
			// alphabeta with a zero-width search window, which creates very many quick
			// cutoffs.  If alphabeta fails low, the next call will place the search
			// window lower; in a sense, MTD is a sort of binary search mechanism into
			// the minimax space.
			private Move MTD_f(SnapShot snapShot, int target, int depth)
			{
				int beta;
				Move move;
				int currentEstimate = target;
				int upperbound = ALPHABETA_MAXVAL;
				int lowerbound = ALPHABETA_MINVAL;

				// This is the trick: make repeated calls to alphabeta, zeroing in on the
				// actual minimax value of theBoard by narrowing the bounds
				do 
				{
					if (currentEstimate == lowerbound)
						beta = currentEstimate+1;
					else
						beta = currentEstimate;

					move = UnrolledAlphaBeta(snapShot,depth,beta-1,beta);
					currentEstimate = move.Evaluation;

					if (currentEstimate < beta)
						upperbound = currentEstimate;
					else
						lowerbound = currentEstimate;

				} while (lowerbound < upperbound);

				return move;
			}

			// The standard alphabeta, with the top level "unrolled" so that it can
			// return a Move structure instead of a mere minimax value
			// See BestMovePicker.AlphaBeta for detailed comments on this code
			private Move UnrolledAlphaBeta(SnapShot snapShot, int depth, int alpha, int beta)
			{
				Move bestMove = null;

				snapShot.CalculateLegalMoves();

				snapShot.legalMoves.Sort(snapShot.whoToMove);

				int bestSoFar = ALPHABETA_MINVAL;
				int currentAlpha = alpha;

				Move move = null;

				// Loop on the successors
				for (int i = 0; i < snapShot.legalMoves.Count; i++)
				{
					move = snapShot.legalMoves[i];

					snapShot.PerformMove(move,true);
moveCounter++;
					// And search it in turn
					int moveScore = AlphaBeta(MINNODE,snapShot,depth-1,currentAlpha,beta);
moveCounter--;
					snapShot.Rollback(1);

					currentAlpha = Math.Max(currentAlpha,moveScore);

					// Is the current successor better than the previous best?
					if ( moveScore > bestSoFar)
					{
						bestMove = move.Clone();
						bestSoFar = moveScore;
						bestMove.Evaluation = bestSoFar;

						// Can we cutoff now?
						if (bestSoFar >= beta)
						{
							tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Upperbound,depth,moveCounter);

							// Add this move's efficiency in the HistoryTable
							hTable.AddMove(snapShot.whoToMove,move);
							return bestMove;
						}
					}
				}

				// Test for checkmate or stalemate
				if (bestSoFar <= ALPHABETA_GIVEUP)
				{
					if (snapShot.legalMoves.Count > 0)
					{
						bool bGameOverPossible = false;
						for (int i = 0; i < snapShot.legalMoves.Count; i++)
						{
							move = snapShot.legalMoves[i];

							snapShot.PerformMove(move);

							for (int j = 0; j < snapShot.legalMoves.Count; j++)
							{
								move = snapShot.legalMoves[j];

								snapShot.PerformMove(move);
								if (snapShot.legalMoves.Count == 0)
									bGameOverPossible = true;

								snapShot.Rollback(1);

								if (bGameOverPossible)
									break;
							}

							snapShot.Rollback(1);

							if (bGameOverPossible)
								break;
						}
						if (bGameOverPossible)
						{
							bestMove = new Move(snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField,snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField);
							bestMove.PieceType = PieceType.King;
							bestMove.MoveCode = MoveCode.Resign;
						}
					}
					else
					{
						// We're in check and our best hope is GIVEUP or worse, so either we are
						// already checkmated or will be soon, without hope of escape
						bestMove = new Move(snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField,snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField);
						bestMove.PieceType = PieceType.King;
						bestMove.MoveCode = MoveCode.Resign;
					}
				}

				// If we haven't returned yet, we have found an accurate minimax score
				// for a position which is neither a checkmate nor a stalemate
				tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Accurate,depth,moveCounter);

				return bestMove;
			}
		}

		/// <summary>
	/// MTDv2 (Memory-enhanced Test Driver with 2 calls per iteration) is an iterative-deepening MTD(f) search engine.
	/// 
	/// This engine uses the MTD(f) algorithm (also known as Conspiracy Number Search) which performs zero-width
	/// alpha-beta searches with iteratively refined upper and lower bounds to narrow the search window. This approach
	/// can be more efficient than standard alpha-beta in finding the best move, as it exploits transposition table
	/// results more effectively.
	/// 
	/// Key features:
	/// - Iterative deepening from depth 2 to maxIterationDepth (default 15), incrementing by 2 each iteration
	/// - MTD(f) zero-width search with alpha-beta refinement
	/// - Full alpha-beta search at the root level (UnrolledAlphaBeta)
	/// - Recursive alpha-beta search with transposition table cutoffs
	/// - Quiescence search for tactical positions (captures and checks only)
	/// - Complete game-phase evaluation with endgame heuristics
	/// - Draw detection (insufficient material, threefold repetition, fifty-move rule)
	/// - Move ordering via transposition table, principal variation, and history heuristic
	/// 
	/// Search Pipeline: Iterative Deepening → MTD(f) → UnrolledAlphaBeta → AlphaBeta → QuiescenceSearch → Terminal Evaluation
	/// </summary>
	public class MTDv2 : BestMovePicker
		{
			/// <summary>
			/// If true, outputs search progress information to console (depth, score, node count).
			/// </summary>
			public bool Verbose = false;
			
			private int timeStampCounter = 0;
			
			/// <summary>
			/// Maximum search depth for each iteration. Search continues iteratively
			/// from depth 2 to this value (in steps of 2) until time/node limits are reached.
			/// Default: 15 (approximately 8 iterations from d=2 to d=14).
			/// </summary>
			private int maxIterationDepth = 15;
			
			/// <summary>
			/// Maximum total nodes (regular + quiescence) before search terminates.
			/// Includes both regular alpha-beta nodes and quiescence search nodes.
			/// Default: 50000 nodes.
			/// </summary>
			private int maxSearchSize = 50000;

			/// <summary>
			/// Gets or sets the maximum search depth.
			/// Search runs iteratively from depth 2, incrementing by 2 until this limit is reached.
			/// </summary>
			public int MaxIterationDepth
			{
				get { return maxIterationDepth; }
				set { maxIterationDepth = value; }
			}

			/// <summary>
			/// Gets or sets the maximum search size (total number of nodes across all search layers).
			/// Search terminates when either maxIterationDepth iterations complete or total nodes exceed this limit.
			/// </summary>
			public int MaxSearchSize
			{
				get { return maxSearchSize; }
				set { maxSearchSize = value; }
			}
			/// <summary>
			/// Generates the next unique timestamp for transposition table entries.
			/// Used to manage aging of entries in the transposition table.
			/// </summary>
			/// <returns>A unique timestamp value (monotonically increasing).</returns>
			private int NextTimeStamp()
			{
				unchecked
				{
					timeStampCounter++;
					if (timeStampCounter <= 0)
						timeStampCounter = 1;
					return timeStampCounter;
				}
			}

			/// <summary>
			/// Checks if two moves represent the same source and destination squares.
			/// Handles null moves gracefully.
			/// </summary>
			/// <param name="a">First move to compare.</param>
			/// <param name="b">Second move to compare.</param>
			/// <returns>True if both moves have identical from/to field numbers; false if either is null or different.</returns>
			private bool IsSameMove(Move a, Move b)
			{
				if (a == null || b == null)
					return false;
				return a.FieldNumberFrom == b.FieldNumberFrom && a.FieldNumberTo == b.FieldNumberTo;
			}

			/// <summary>
			/// Evaluates positions with no legal moves (checkmate or stalemate).
			/// 
			/// Checkmate: Side to move is in check and has no legal moves.
			/// Returns negative values (losing) for MAXNODE (side to move loses)
			/// and positive values (winning) for MINNODE (side to move wins from opponent's perspective).
			/// Values increase by depth to prefer faster mates.
			/// 
			/// Stalemate: Side to move is not in check and has no legal moves.
			/// Returns 0 (draw evaluation).
			/// </summary>
			/// <param name="nodeType">MAXNODE for nodes where computer is to move; MINNODE for opponent.</param>
			/// <param name="snapShot">Current board position.</param>
			/// <param name="depth">Current search depth (used to score faster mates higher).</param>
			/// <returns>
			/// Checkmate: ALPHABETA_MINVAL + depth (negative for MAXNODE), or ALPHABETA_MAXVAL - depth (positive for MINNODE).
			/// Stalemate: 0.
			/// </returns>
			private int EvaluateNoLegalMoves(bool nodeType, SnapShot snapShot, int depth)
			{
				int ownKingField = snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField;
				bool inCheck = snapShot.IsFieldChecked(ownKingField)
					|| (snapShot.history != null
					&& snapShot.history.Count > 0
					&& snapShot.history[snapShot.history.Count-1].OpponentSituationCode == SituationCode.Check);

				if (!inCheck)
					return 0;

				if (nodeType == MAXNODE)
					return ALPHABETA_MINVAL + depth;

				return ALPHABETA_MAXVAL - depth;
			}

			/// <summary>
			/// Checks if a position is drawn by any rule (insufficient material, threefold repetition, or fifty-move rule).
			/// Used to short-circuit search and return 0 evaluation for drawn positions.
			/// </summary>
			/// <param name="snapShot">Current board position to evaluate.</param>
			/// <returns>True if the position is drawn by any applicable rule; false otherwise.</returns>
			private bool IsDrawnPosition(SnapShot snapShot)
			{
				return snapShot.IsDrawByInsufficientMaterial() || snapShot.IsDrawByRepetition() || snapShot.IsDrawByFiftyMoveRule();
			}

			/// <summary>
			/// Calculates move ordering score for move ordering heuristic.
			/// Higher scores should be searched first (best move first) to maximize alpha-beta pruning.
			/// 
			/// Prioritizes moves in this order:
			/// 1. Transposition table move (best known move from previous search)
			/// 2. Principal variation move (from iterative deepening)
			/// 3. Captures ordered by MVV/LVA (Most Valuable Victim / Least Valuable Attacker)
			/// 4. Non-capture moves from killer move / history table
			/// </summary>
			/// <param name="snapShot">Current position for context.</param>
			/// <param name="move">Move to score.</param>
			/// <param name="ttMove">Best move from transposition table (if any).</param>
			/// <param name="pvMove">Principal variation move from iterative deepening (if any).</param>
			/// <param name="capturesOnly">If true, only score captures (ignore non-captures).</param>
			/// <returns>Score for move ordering (higher = better).</returns>
			private int GetMoveOrderingScore(SnapShot snapShot, Move move, Move ttMove, Move pvMove, bool capturesOnly)
			{
				int score = hTable.GetScore(snapShot.whoToMove,move);

				if (IsSameMove(move,ttMove))
					score += 2_000_000;

				if (IsSameMove(move,pvMove))
					score += 1_000_000;

				if (capturesOnly || move.PieceHit != PieceType.None)
				{
					int capturedValue = move.PieceHit == PieceType.None ? 0 : Helper.PieceType2Value(move.PieceHit);
					int movingValue = Helper.PieceType2Value((PieceType)(snapShot.situation[move.FieldNumberFrom] & 0x07));
					score += capturedValue * 10 - movingValue;
				}

				return score;
			}

			/// <summary>
			/// Generates a sorted list of legal moves for the given position.
			/// Moves are sorted by move ordering score to improve alpha-beta pruning efficiency.
			/// 
			/// If capturesOnly=false, includes all legal moves.
			/// If capturesOnly=true, includes only capturing moves (used in quiescence search).
			/// </summary>
			/// <param name="snapShot">Current board position.</param>
			/// <param name="capturesOnly">If true, return only capturing moves; if false, return all legal moves.</param>
			/// <param name="ttMove">Transposition table move (searched first if present).</param>
			/// <param name="pvMove">Principal variation move (searched early for refutation).</param>
			/// <returns>List of legal moves sorted by estimated value (best-first order).</returns>
			private List<Move> GetOrderedMoves(SnapShot snapShot, bool capturesOnly, Move ttMove, Move pvMove)
			{
				List<Move> ordered = new List<Move>(snapShot.legalMoves.Count);
				for (int i = 0; i < snapShot.legalMoves.Count; i++)
				{
					Move move = snapShot.legalMoves[i];
					if (!capturesOnly || move.PieceHit != PieceType.None)
						ordered.Add(move);
				}

				ordered.Sort((left,right) =>
				{
					int rightScore = GetMoveOrderingScore(snapShot,right,ttMove,pvMove,capturesOnly);
					int leftScore = GetMoveOrderingScore(snapShot,left,ttMove,pvMove,capturesOnly);
					return rightScore.CompareTo(leftScore);
				});

				return ordered;
			}

			/// <summary>
			/// Finds the best move for the given position using iterative deepening MTD(f) search.
			/// 
			/// Algorithm Overview:
			/// 1. Checks opening book for known moves (if available)
			/// 2. Performs iterative deepening with incrementing depths (2, 4, 6, ..., maxIterationDepth)
			/// 3. For each depth, uses MTD(f) to refine the evaluation with zero-width alpha-beta searches
			/// 4. Returns the best move found before depth/node limits are exceeded
			/// 5. Falls back to depth-2 search if all iterations fail
			/// 
			/// Search terminates when either:
			/// - Maximum iteration depth is reached, OR
			/// - Total nodes (regular + quiescence) exceeds maxSearchSize
			/// 
			/// Transposition table is cleared at depth >= 12 to manage memory and reduce stale entries.
			/// </summary>
			/// <param name="snapShot">Current board position to analyze.</param>
			/// <param name="openings">Opening book for move lookup; can be null.</param>
			/// <returns>
			/// The best move found by search, with Evaluation field set to the estimated position value.
			/// Returns a king move to self if no legal moves exist (resign indicator).
			/// </returns>
			public override Move GetBestMove(SnapShot snapShot, OpeningBook openings)
			{
				numRegularNodes = 0;
				numQuiescenceNodes = 0;
				numRegularCutoffs = 0;
				numRegularTTHits = 0;
				numQuiescenceTTHits = 0;
				moveCounter++;

				if (openings != null)
				{
					Move openingMove = openings.Query(snapShot);
					if (openingMove != null)
						return openingMove;
				}

				Move bestMove = null;
				Move pvMove = null;
				int bestGuess = 0;
				Color fromWhosePerspective = snapShot.whoToMove;
				this.fromWhosePerspective = fromWhosePerspective;

				for (int iterationDepth = 2; iterationDepth <= maxIterationDepth && (numRegularNodes + numQuiescenceNodes) < maxSearchSize; iterationDepth += 2)
				{
					if (iterationDepth >= 12)
						hTable.Clear();

					Move candidate = MTD_f(snapShot,bestGuess,iterationDepth,pvMove);
					if (candidate == null)
						break;

					bestMove = candidate;
					bestGuess = candidate.Evaluation;
					pvMove = candidate.Clone();

					if (Verbose)
					{
						int cp = fromWhosePerspective == Color.White ? bestMove.Evaluation : -bestMove.Evaluation;
						Console.WriteLine("d=" + iterationDepth + " score=" + cp + " nodes=" + (numRegularNodes + numQuiescenceNodes));
					}
				}

				if (bestMove == null)
					bestMove = MTD_f(snapShot,0,2,pvMove);

				if (bestMove == null)
					bestMove = new Move(snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField,snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField);

				moveCounter--;
				return bestMove;
			}

			/// <summary>
			/// MTD(f) - Memory-enhanced Test Driver with f parameter.
			/// 
			/// Implements zero-width alpha-beta search with binary search refinement.
			/// 
			/// Algorithm:
			/// 1. Starts with firstGuess evaluation as initial estimate
			/// 2. Performs zero-width alpha-beta searches (alpha = beta - 1) iteratively
			/// 3. If evaluation is lower than beta, narrows upper bound (searches for lower values)
			/// 4. If evaluation is higher/equal to beta, narrows lower bound (searches for higher values)
			/// 5. Converges when lower bound equals upper bound (found exact score, or proved bounds)
			/// 
			/// Advantages over standard alpha-beta with wider window:
			/// - Exploits transposition table more effectively (zero-width searches create consistent bounds)
			/// - Can be faster overall despite multiple searches, especially with good move ordering
			/// 
			/// Transposition table is critical for efficiency - reuses previous search results
			/// across iterations to avoid redundant computation.
			/// </summary>
			/// <param name="snapShot">Position to evaluate.</param>
			/// <param name="firstGuess">Initial estimate of position value (triggers lower or upper bound search).</param>
			/// <param name="depth">Target search depth (passed to UnrolledAlphaBeta).</param>
			/// <param name="pvMove">Principal variation move for move ordering.</param>
			/// <returns>Best move found with Evaluation set to the converged score.</returns>
			private Move MTD_f(SnapShot snapShot, int firstGuess, int depth, Move pvMove)
			{
				int upperBound = ALPHABETA_MAXVAL;
				int lowerBound = ALPHABETA_MINVAL;
				int currentEstimate = firstGuess;
				Move bestMove = null;

				do
				{
					int beta = (currentEstimate == lowerBound) ? currentEstimate + 1 : currentEstimate;
					bestMove = UnrolledAlphaBeta(snapShot,depth,beta - 1,beta,pvMove);
					if (bestMove == null)
						break;

					currentEstimate = bestMove.Evaluation;
					if (currentEstimate < beta)
						upperBound = currentEstimate;
					else
						lowerBound = currentEstimate;

					pvMove = bestMove;
				}
				while (lowerBound < upperBound);

				return bestMove;
			}

			/// <summary>
			/// Root-level alpha-beta search at the top of the search tree.
			/// 
			/// Expands all legal moves from the current position and searches each one
			/// using the recursive AlphaBeta method. Unlike AlphaBeta, this returns a Move object
			/// (not just an evaluation) and stores the best move with its evaluation for return
			/// to the caller.
			/// 
			/// Includes:
			/// - Transposition table lookup for potential move hints
			/// - Move ordering via transposition table, PV moves, and history heuristics
			/// - Terminal node detection (checkmate/stalemate evaluation)
			/// - Alpha-beta pruning to eliminate branches that cannot improve the result
			/// - Transposition table storage for future searches
			/// 
			/// This is the "unrolled" version because it iterates through moves explicitly
			/// rather than through recursive calls, enabling better move tracking at the root.
			/// </summary>
			/// <param name="snapShot">Current position.</param>
			/// <param name="depth">Search depth (decremented by 1 for each recursive call).</param>
			/// <param name="alpha">Lower bound on acceptable values (maximizing player's worst case).</param>
			/// <param name="beta">Upper bound on acceptable values (minimizing player's worst case).</param>
			/// <param name="pvMove">Principal variation move for move ordering (from iterative deepening).</param>
			/// <returns>Best move found with Evaluation set to the search result.</returns>
			private Move UnrolledAlphaBeta(SnapShot snapShot, int depth, int alpha, int beta, Move pvMove)
			{
				snapShot.CalculateLegalMoves();

				Move ttMove = new Move();
				if (!tTable.LookupBoard(snapShot,ttMove) || ttMove.FieldNumberFrom < 0)
					ttMove = null;

				List<Move> orderedMoves = GetOrderedMoves(snapShot,false,ttMove,pvMove);
				if (orderedMoves.Count == 0)
				{
					int terminal = EvaluateNoLegalMoves(MAXNODE,snapShot,depth);
					Move noMove = new Move(snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField,snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField);
					noMove.PieceType = PieceType.King;
					noMove.MoveCode = MoveCode.Resign;
					noMove.Evaluation = terminal;
					tTable.StoreSnapShot(snapShot,terminal,EvaluationType.Accurate,depth,NextTimeStamp(),noMove);
					return noMove;
				}

				int bestSoFar = ALPHABETA_MINVAL;
				Move bestMove = null;

				for (int i = 0; i < orderedMoves.Count; i++)
				{
					Move move = orderedMoves[i];
					snapShot.PerformMove(move,true);
					moveCounter++;
					int moveScore = AlphaBeta(MINNODE,snapShot,depth - 1,alpha,beta,pvMove);
					moveCounter--;
					snapShot.Rollback(1);

					if (moveScore > bestSoFar)
					{
						bestSoFar = moveScore;
						bestMove = move.Clone();
						bestMove.Evaluation = moveScore;
						alpha = Math.Max(alpha,bestSoFar);

						if (bestSoFar >= beta)
						{
							tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Lowerbound,depth,NextTimeStamp(),bestMove);
							hTable.AddMove(snapShot.whoToMove,move);
							numRegularCutoffs++;
							return bestMove;
						}
					}
				}

				if (bestMove == null)
				{
					bestMove = new Move(snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField,snapShot.whoToMove == Color.White ? snapShot.whiteKingsField : snapShot.blackKingsField);
					bestMove.PieceType = PieceType.King;
					bestMove.MoveCode = MoveCode.Resign;
					bestMove.Evaluation = bestSoFar;
				}

				tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Accurate,depth,NextTimeStamp(),bestMove);
				bestMove.Evaluation = bestSoFar;
				return bestMove;
			}

			/// <summary>
			/// Recursive alpha-beta minimax search with transposition table integration.
			/// 
			/// Core minimax algorithm with alpha-beta pruning to evaluate positions and determine
			/// the best move for either side. Alternates between MAXNODE (computer to move, maximizing)
			/// and MINNODE (opponent to move, minimizing).
			/// 
			/// Search Process:
			/// 1. Increments regular node counter
			/// 2. Checks for drawn positions (insufficient material, repetition, fifty-move rule)
			/// 3. Validates transposition table cutoff opportunities
			/// 4. At depth=0, delegates to quiescence search for tactical continuation
			/// 5. Evaluates all legal moves, pruning branches with beta cutoffs
			/// 6. Stores results in transposition table with bounds (exact, lower, or upper)
			/// 
			/// Alpha-Beta Pruning:
			/// - Alpha: best value found so far for maximizing player (lower bound)
			/// - Beta: upper bound on position value (if exceeded, this branch is refuted)
			/// - Cutoff occurs when alpha >= beta (pruning effective)
			/// 
			/// Terminal nodes (checkmate/stalemate) are evaluated by EvaluateNoLegalMoves,
			/// which handles the distinction between mate and stalemate.
			/// </summary>
			/// <param name="nodeType">MAXNODE (computer to move) or MINNODE (opponent to move).</param>
			/// <param name="snapShot">Current board position.</param>
			/// <param name="depth">Remaining search depth. Depth=0 triggers quiescence search.</param>
			/// <param name="alpha">Lower bound: best value found for maximizing player so far.</param>
			/// <param name="beta">Upper bound: refutation value for this node.</param>
			/// <param name="pvMove">Principal variation move for move ordering.</param>
			/// <returns>Evaluated score of the position (from maximizing player's perspective).</returns>
			private int AlphaBeta(bool nodeType, SnapShot snapShot, int depth, int alpha, int beta, Move pvMove)
			{
				numRegularNodes++;
				if (IsDrawnPosition(snapShot))
					return 0;
				if ((numRegularNodes + numQuiescenceNodes) > maxSearchSize)
					return (snapShot.EvaluateComplete(fromWhosePerspective) >> 3) << 3;

				Move cachedMove = new Move();
				if (tTable.LookupBoard(snapShot,cachedMove) && cachedMove.SearchDepth >= depth)
				{
					if (nodeType == MAXNODE)
					{
						if ((cachedMove.EvaluationType == EvaluationType.Accurate || cachedMove.EvaluationType == EvaluationType.Lowerbound) && cachedMove.Evaluation >= beta)
						{
							numRegularTTHits++;
							return cachedMove.Evaluation;
						}
					}
					else
					{
						if ((cachedMove.EvaluationType == EvaluationType.Accurate || cachedMove.EvaluationType == EvaluationType.Upperbound) && cachedMove.Evaluation <= alpha)
						{
							numRegularTTHits++;
							return cachedMove.Evaluation;
						}
					}
				}

				if (depth == 0)
					return QuiescenceSearch(nodeType,snapShot,maxQuiescenceDepth,alpha,beta,pvMove);

				snapShot.CalculateLegalMoves();
				Move ttMove = new Move();
				if (!tTable.LookupBoard(snapShot,ttMove) || ttMove.FieldNumberFrom < 0)
					ttMove = null;

				List<Move> orderedMoves = GetOrderedMoves(snapShot,false,ttMove,pvMove);

				if (orderedMoves.Count == 0)
					return EvaluateNoLegalMoves(nodeType,snapShot,depth);

				int bestSoFar = nodeType == MAXNODE ? ALPHABETA_MINVAL : ALPHABETA_MAXVAL;
				Move bestMove = null;

				for (int i = 0; i < orderedMoves.Count; i++)
				{
					Move move = orderedMoves[i];
					snapShot.PerformMove(move,true);
					moveCounter++;
					int moveScore = AlphaBeta(nodeType == MAXNODE ? MINNODE : MAXNODE,snapShot,depth - 1,alpha,beta,pvMove);
					moveCounter--;
					snapShot.Rollback(1);

					if (nodeType == MAXNODE)
					{
						if (moveScore > bestSoFar)
						{
							bestSoFar = moveScore;
							bestMove = move;
							alpha = Math.Max(alpha,bestSoFar);
							if (bestSoFar >= beta)
							{
								tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Lowerbound,depth,NextTimeStamp(),bestMove);
								hTable.AddMove(snapShot.whoToMove,move);
								numRegularCutoffs++;
								return bestSoFar;
							}
						}
					}
					else
					{
						if (moveScore < bestSoFar)
						{
							bestSoFar = moveScore;
							bestMove = move;
							beta = Math.Min(beta,bestSoFar);
							if (bestSoFar <= alpha)
							{
								tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Upperbound,depth,NextTimeStamp(),bestMove);
								hTable.AddMove(snapShot.whoToMove,move);
								numRegularCutoffs++;
								return bestSoFar;
							}
						}
					}
				}

				tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Accurate,depth,NextTimeStamp(),bestMove);
				return bestSoFar;
			}

			/// <summary>
			/// Quiescence search for tactical positions - evaluates captures and checks only.
			/// 
			/// Purpose: Avoids the "horizon problem" where quiet moves at the search boundary
			/// can dramatically change evaluation. By continuing to search tactical positions
			/// (captures, checks) until stable, we get more accurate position assessments.
			/// 
			/// Key Features:
			/// 1. Stand-pat evaluation: position without further moves provides lower bound (for max player)
			/// 2. Captures only: search continues with captures, ordered by MVV/LVA
			/// 3. Futility pruning: if material balance is far beyond bounds, return early
			/// 4. Terminal detection: checkmate/stalemate handled same as in full search
			/// 5. Depth-limited: quiescence depth (maxQuiescenceDepth) prevents infinite recursion
			/// 6. Transposition table: results cached for efficiency
			/// 
			/// The function returns a "quiet" evaluation only after no captures improve the position
			/// or maximum tactical depth is exhausted, ensuring tactical stability.
			/// 
			/// Evaluation from computer player's perspective (same as AlphaBeta).
			/// </summary>
			/// <param name="nodeType">MAXNODE (computer to move) or MINNODE (opponent to move).</param>
			/// <param name="snapShot">Current board position.</param>
			/// <param name="depth">Remaining quiescence depth (decremented for each capture). Depth=0 returns stand-pat.</param>
			/// <param name="alpha">Lower bound on acceptable values.</param>
			/// <param name="beta">Upper bound - pruning value.</param>
			/// <param name="pvMove">Principal variation move for move ordering.</param>
			/// <returns>Tactical evaluation of the position after all forcing moves are explored.</returns>
			private int QuiescenceSearch(bool nodeType, SnapShot snapShot, int depth, int alpha, int beta, Move pvMove)
			{
				numQuiescenceNodes++;
				if (IsDrawnPosition(snapShot))
					return 0;

				Move cachedMove = new Move();
				if (tTable.LookupBoard(snapShot,cachedMove))
				{
					if (nodeType == MAXNODE)
					{
						if ((cachedMove.EvaluationType == EvaluationType.Accurate || cachedMove.EvaluationType == EvaluationType.Lowerbound) && cachedMove.Evaluation >= beta)
						{
							numQuiescenceTTHits++;
							return cachedMove.Evaluation;
						}
					}
					else
					{
						if ((cachedMove.EvaluationType == EvaluationType.Accurate || cachedMove.EvaluationType == EvaluationType.Upperbound) && cachedMove.Evaluation <= alpha)
						{
							numQuiescenceTTHits++;
							return cachedMove.Evaluation;
						}
					}
				}

				int materialOnly = (snapShot.EvaluateMaterial(fromWhosePerspective) >> 3) << 3;
				if (materialOnly > (beta + EVAL_THRESHOLD) || materialOnly < (alpha - EVAL_THRESHOLD))
					return materialOnly;

				int standPat = (snapShot.EvaluateComplete(fromWhosePerspective) >> 3) << 3;
				if (standPat > alpha)
					alpha = standPat;
				if (standPat >= beta)
					return standPat;

				snapShot.CalculateLegalMoves();
				if (snapShot.legalMoves.Count == 0)
				{
					int terminal = EvaluateNoLegalMoves(nodeType,snapShot,depth);
					tTable.StoreSnapShot(snapShot,terminal,EvaluationType.Accurate,depth,NextTimeStamp());
					return terminal;
				}

				if (!snapShot.HitPossible || depth == 0)
				{
					tTable.StoreSnapShot(snapShot,standPat,EvaluationType.Accurate,depth,NextTimeStamp());
					return standPat;
				}

				Move ttMove = new Move();
				if (!tTable.LookupBoard(snapShot,ttMove) || ttMove.FieldNumberFrom < 0)
					ttMove = null;

				List<Move> orderedCaptures = GetOrderedMoves(snapShot,true,ttMove,pvMove);
				if (orderedCaptures.Count == 0)
				{
					tTable.StoreSnapShot(snapShot,standPat,EvaluationType.Accurate,depth,NextTimeStamp());
					return standPat;
				}

				int bestSoFar = standPat;
				Move bestMove = null;

				for (int i = 0; i < orderedCaptures.Count; i++)
				{
					Move move = orderedCaptures[i];
					snapShot.PerformMove(move,true);
					moveCounter++;
					int moveScore = QuiescenceSearch(nodeType == MAXNODE ? MINNODE : MAXNODE,snapShot,depth - 1,alpha,beta,pvMove);
					moveCounter--;
					snapShot.Rollback(1);

					if (nodeType == MAXNODE)
					{
						if (moveScore > bestSoFar)
						{
							bestSoFar = moveScore;
							bestMove = move;
							alpha = Math.Max(alpha,bestSoFar);
							if (bestSoFar >= beta)
							{
								tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Lowerbound,depth,NextTimeStamp(),bestMove);
								return bestSoFar;
							}
						}
					}
					else
					{
						if (moveScore < bestSoFar)
						{
							bestSoFar = moveScore;
							bestMove = move;
							beta = Math.Min(beta,bestSoFar);
							if (bestSoFar <= alpha)
							{
								tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Upperbound,depth,NextTimeStamp(),bestMove);
								return bestSoFar;
							}
						}
					}
				}

				tTable.StoreSnapShot(snapShot,bestSoFar,EvaluationType.Accurate,depth,NextTimeStamp(),bestMove);

				return bestSoFar;
			}
		}


		// Data counters to evaluate pawn structure
		private int[] columnOwnPawnCount = new int[8];
		private int[] fieldColorPawnCount = new int[2];
		private int pawnRamCount = 0;
		private int[] columnMostAdvancedOwnPawnFieldNum = new int[8];
		private int[] columnOwnPassedPawnCount = new int[8];
		private int[] columnOpponentPawnCount = new int[8];
		private int[] columnLeastAdvancedOpponentPawnFieldNum = new int[8];

		// If there are too many pawns on squares of the color of its surviving bishops,
		// the bishops may be limited in their movement
		// Given the pawn formations, penalize or bonify the position according to
		// the features it contains
		// Mostly useful in the opening, it's good to move the bishops and knights
		// into play (to control the center) and to castle
		// All other things being equal, having your Knights, Queens and Rooks close
		// to the opponent's king is a good thing
		// Rooks are more effective on the seventh rank, on open files and behind
		// passed pawns
		private int EvaluateSituation(Color color)
		{
			int score = 0;
			int ownNonPawnMaterial = 0;
			int opponentNonPawnMaterial = 0;
			int totalNonPawnMaterial = 0;
			int endgamePhase = 0;
			int ownKingField = -1;
			int ownKingVCoord = -1;
			int ownKingHCoord = -1;
			int ownKingCenterDistance = 0;
			int ownKingCentralizationBonus = 0;
			int castlingWeight = 0;
			int castlingScore = 0;

			int kingVCoord = -1;
			int kingHCoord = -1;
			int pieceVCoord = -1;
			int pieceHCoord = -1;
			
			int index = -1;
			
			int x = 0;

			// in chess, two or more pawns on the same file usually hinder each other,
			// so we assign a minor penalty
			for (int i = 0; i < 8; i++)
			{
				if (columnOwnPawnCount[i] > 1)
					score -= 8;
			}

			// look for an isolated pawn, i.e., one which has no neighbor pawns
			// capable of protecting it from attack at some point in the future
			if (columnOwnPawnCount[0] > 0 && columnOwnPawnCount[1] == 0)
				score -= 15;
			if (columnOwnPawnCount[7] > 0 && columnOwnPawnCount[6] == 0)
				score -= 15;
			for(int i = 1; i < 7; i++)
			{
				if (columnOwnPawnCount[i] > 0 && columnOwnPawnCount[i-1] == 0 && columnOwnPawnCount[i+1] == 0)
					score -= 15;
			}

			// penalize pawn rams, they restrict movement
			score -= pawnRamCount*8;

			if (color == Color.White)
			{
				ownNonPawnMaterial =
					whiteRooksCount*Helper.PieceType2Value(PieceType.Rook) +
					whiteKnightsCount*Helper.PieceType2Value(PieceType.Knight) +
					whiteBishopsCount*Helper.PieceType2Value(PieceType.Bishop) +
					whiteQueensCount*Helper.PieceType2Value(PieceType.Queen);
				opponentNonPawnMaterial =
					blackRooksCount*Helper.PieceType2Value(PieceType.Rook) +
					blackKnightsCount*Helper.PieceType2Value(PieceType.Knight) +
					blackBishopsCount*Helper.PieceType2Value(PieceType.Bishop) +
					blackQueensCount*Helper.PieceType2Value(PieceType.Queen);
				ownKingField = whiteKingsField;
			}
			else
			{
				ownNonPawnMaterial =
					blackRooksCount*Helper.PieceType2Value(PieceType.Rook) +
					blackKnightsCount*Helper.PieceType2Value(PieceType.Knight) +
					blackBishopsCount*Helper.PieceType2Value(PieceType.Bishop) +
					blackQueensCount*Helper.PieceType2Value(PieceType.Queen);
				opponentNonPawnMaterial =
					whiteRooksCount*Helper.PieceType2Value(PieceType.Rook) +
					whiteKnightsCount*Helper.PieceType2Value(PieceType.Knight) +
					whiteBishopsCount*Helper.PieceType2Value(PieceType.Bishop) +
					whiteQueensCount*Helper.PieceType2Value(PieceType.Queen);
				ownKingField = blackKingsField;
			}

			totalNonPawnMaterial = ownNonPawnMaterial + opponentNonPawnMaterial;
			if (totalNonPawnMaterial < 0)
				totalNonPawnMaterial = 0;
			if (totalNonPawnMaterial > 6400)
				totalNonPawnMaterial = 6400;
			endgamePhase = ((6400 - totalNonPawnMaterial) * 256) / 6400;

			score += EvaluateSpaceControl(color,endgamePhase);
			score += EvaluateBlockadeQuality(color,endgamePhase);
			score += EvaluateKeySquareCoordination(color,endgamePhase);
			score += EvaluateBishopPairComplexPressure(color,endgamePhase);
			score += EvaluateHeavyPiecePenetration(color,endgamePhase);

			ownKingVCoord = ownKingField % 8;
			ownKingHCoord = ownKingField >> 3;
			ownKingCenterDistance = Math.Min(Math.Abs(ownKingVCoord - 3),Math.Abs(ownKingVCoord - 4)) + Math.Min(Math.Abs(ownKingHCoord - 3),Math.Abs(ownKingHCoord - 4));
			ownKingCentralizationBonus = (6 - (ownKingCenterDistance << 1));
			score += (ownKingCentralizationBonus * endgamePhase) >> 4;

			if (color == Color.White)
			{
				// assign a small penalty to positions in which there are still all
				// pawns... this incites a single pawn trade (to open a file)
				if (whitePawnsCount == 8)
					score -= 10;

				// look for a passed pawn; i.e., a pawn which can no longer be
				// blocked or attacked by a rival pawn
				if (columnMostAdvancedOwnPawnFieldNum[0] > Math.Max(columnLeastAdvancedOpponentPawnFieldNum[0],columnLeastAdvancedOpponentPawnFieldNum[1]))
				{
					x = (columnMostAdvancedOwnPawnFieldNum[0] % 8) + 1;
					int passedPawnBonus = x*x;
					score += passedPawnBonus + ((passedPawnBonus * endgamePhase) >> 7);
				}
				if (columnMostAdvancedOwnPawnFieldNum[7] > Math.Max(columnLeastAdvancedOpponentPawnFieldNum[7],columnLeastAdvancedOpponentPawnFieldNum[6]))
				{
					x = (columnMostAdvancedOwnPawnFieldNum[7] % 8) + 1;
					int passedPawnBonus = x*x;
					score += passedPawnBonus + ((passedPawnBonus * endgamePhase) >> 7);
				}
				for (int i = 1; i < 7; i++)
				{
					if (columnMostAdvancedOwnPawnFieldNum[i] > columnLeastAdvancedOpponentPawnFieldNum[i] && columnMostAdvancedOwnPawnFieldNum[i] > columnLeastAdvancedOpponentPawnFieldNum[i-1] && columnMostAdvancedOwnPawnFieldNum[i] > columnLeastAdvancedOpponentPawnFieldNum[i+1])
					{
						x = (columnMostAdvancedOwnPawnFieldNum[i] % 8) + 1;
						int passedPawnBonus = x*x;
						score += passedPawnBonus + ((passedPawnBonus * endgamePhase) >> 7);
					}
				}

				// has the player advanced its center pawns?
				if (situation[25] == 32)
					score -= 15;
				if (situation[33] == 32)
					score -= 15;
				// penalize bishops and knights on the back rank
				for(int i = 0; i < 8; i++)
				{
					x = situation[i*8] & 15;
					if (x == 2  || x == 3)
						score -= 10;
				}
				// penalize too-early queen movement
				if (whiteQueensCount > 0 && situation[24] == 0)
				{
					int pieces = 0;
					x = situation[0];
					if (x == 33)
						pieces++;
					x = situation[8];
					if (x == 34)
						pieces++;
					x = situation[16];
					if (x == 35)
						pieces++;
					x = situation[40];
					if (x == 35)
						pieces++;
					x = situation[48];
					if (x == 34)
						pieces++;
					x = situation[56];
					if (x == 33)
						pieces++;
					score -= pieces << 3;
				}
				// finally, incite castling when the enemy has a queen on the board
				if (hasWhiteCastled)
					castlingScore += 10;
				else
				{
					byte k = situation[32];
					byte rq = situation[0];
					byte rk = situation[56];

					if (k != 37 || (rq != 33 && rk != 33))
						castlingScore -= 120;
					else if (k == 37 && rq == 33 && rk == 33)
						castlingScore -= 24;
					else if (k == 37 && rq != 33 && rk == 33)
						castlingScore -= 40;
					else if (k == 37 && rq == 33 && rk != 33)
						castlingScore -= 80;
				}

				castlingWeight = 256 - endgamePhase;
				if (blackQueensCount == 0)
					castlingWeight >>= 1;
				score += (castlingScore * castlingWeight) >> 8;

				kingVCoord = blackKingsField % 8;
				kingHCoord = blackKingsField >> 3;

				for (int i = 0; i < 64; i++ )
				{
					x = situation[i];
					if ((x & 8) == 0)
					{
						switch (x & 7)
						{
							case 1: // rook
								pieceVCoord = i % 8;
								pieceHCoord = i >> 3;
								score -= Math.Min(Math.Abs(kingVCoord - pieceVCoord),Math.Abs(kingHCoord - pieceHCoord)) << 1;

								index = i >> 3;
								// on the seventh rank?
								if ((i % 8) == 7)
									score += 22;
								// on a semi- or completely open file?
								if (columnOwnPawnCount[index] == 0)
								{
									if (columnOpponentPawnCount[index] == 0)
										score += 10;
									else
										score += 4;
								}
								// behind a passed pawn?
								if (columnOwnPassedPawnCount[index] > i)
									score += 25;

								break;
							case 2: // knight
								pieceVCoord = i % 8;
								pieceHCoord = i >> 3;
								score += 5 - Math.Abs(kingVCoord - pieceVCoord) - Math.Abs(kingHCoord - pieceHCoord);
								score += EvaluateMinorOnWeakSquare(i,PieceType.Knight,color,endgamePhase);
								// outpost bonus: knight on rank 5-7, not attackable by opponent pawn
								if (pieceVCoord >= 4) // rank 5,6,7 (0-indexed rank 4,5,6)
								{
									bool isOutpost = true;
									if (pieceHCoord > 0 && columnOpponentPawnCount[pieceHCoord - 1] > 0)
									{
										// check if black pawn is on adjacent file one rank ahead (rank+1)
										int attackerField = (pieceHCoord - 1) * 8 + (pieceVCoord + 1);
										if (attackerField < 64 && (situation[attackerField] == 40 || situation[attackerField] == 56))
											isOutpost = false;
									}
									if (isOutpost && pieceHCoord < 7 && columnOpponentPawnCount[pieceHCoord + 1] > 0)
									{
										int attackerField = (pieceHCoord + 1) * 8 + (pieceVCoord + 1);
										if (attackerField < 64 && (situation[attackerField] == 40 || situation[attackerField] == 56))
											isOutpost = false;
									}
									if (isOutpost)
									{
										// centrality bonus: files c-f (index 2-5) are more valuable
										int centralityBonus = (pieceHCoord >= 2 && pieceHCoord <= 5) ? 10 : 4;
										int outpostBonus = 15 + centralityBonus + pieceVCoord * 2;
										// scale down in endgame (outposts less decisive when queens/rooks gone)
										score += outpostBonus - ((outpostBonus * endgamePhase) >> 9);
									}
								}
								break;
							case 3: // bishop
								score += EvaluateMinorOnWeakSquare(i,PieceType.Bishop,color,endgamePhase);
								// are there a lot of pawns on fields with same color?
								if (Helper.FieldNumber2Color(i) == Color.White)
									score -= fieldColorPawnCount[0] << 3;
								else
									score -= fieldColorPawnCount[1] << 3;
								// diagonal mobility: count unobstructed squares on all 4 diagonals
								{
									int bishopFile = i >> 3;
									int bishopRank = i % 8;
									int mobilityScore = 0;
									int[] diagStepFiles = { 1, 1, -1, -1 };
									int[] diagStepRanks = { 1, -1, 1, -1 };
									for (int d = 0; d < 4; d++)
									{
										int f = bishopFile + diagStepFiles[d];
										int r = bishopRank + diagStepRanks[d];
										while (f >= 0 && f < 8 && r >= 0 && r < 8)
										{
											int sq = f * 8 + r;
											if (situation[sq] != 0)
												break; // blocked
											mobilityScore += 2;
											f += diagStepFiles[d];
											r += diagStepRanks[d];
										}
									}
									score += mobilityScore;
								}
								break;
							case 4: // queen
								pieceVCoord = i % 8;
								pieceHCoord = i >> 3;
								score -= Math.Min(Math.Abs(kingVCoord - pieceVCoord),Math.Abs(kingHCoord - pieceHCoord));
								break;
							default:
								break;
						}
					}
				}
			}
			else // color == black
			{
				// assign a small penalty to positions in which there are still all
				// pawns... this incites a single pawn trade (to open a file)
				if (blackPawnsCount == 8)
					score -= 10;

				// look for a passed pawn; i.e., a pawn which can no longer be
				// blocked or attacked by a rival pawn
				if (columnMostAdvancedOwnPawnFieldNum[0] < Math.Min(columnLeastAdvancedOpponentPawnFieldNum[0],columnLeastAdvancedOpponentPawnFieldNum[1]))
				{
					x = 8 - (columnMostAdvancedOwnPawnFieldNum[0] % 8);
					int passedPawnBonus = x*x;
					score += passedPawnBonus + ((passedPawnBonus * endgamePhase) >> 7);
				}
				if (columnMostAdvancedOwnPawnFieldNum[7] < Math.Min(columnLeastAdvancedOpponentPawnFieldNum[7],columnLeastAdvancedOpponentPawnFieldNum[6]))
				{
					x = 8 - (columnMostAdvancedOwnPawnFieldNum[7] % 8);
					int passedPawnBonus = x*x;
					score += passedPawnBonus + ((passedPawnBonus * endgamePhase) >> 7);
				}
				for (int i = 1; i < 7; i++)
				{
					if (columnMostAdvancedOwnPawnFieldNum[i] < columnLeastAdvancedOpponentPawnFieldNum[i] && columnMostAdvancedOwnPawnFieldNum[i] < columnLeastAdvancedOpponentPawnFieldNum[i-1] && columnMostAdvancedOwnPawnFieldNum[i] < columnLeastAdvancedOpponentPawnFieldNum[i+1])
					{
						x = 8 - (columnMostAdvancedOwnPawnFieldNum[i] % 8);
						int passedPawnBonus = x*x;
						score += passedPawnBonus + ((passedPawnBonus * endgamePhase) >> 7);
					}
				}

				// has the player advanced its center pawns?
				if (situation[30] == 40)
					score -= 15;
				if (situation[38] == 40)
					score -= 15;
				// penalize bishops and knights on the back rank
				for (int i = 1; i < 9; i++)
				{
					x = situation[i*8 - 1] & 15;
					if (x == 10  || x == 11)
						score -= 10;
				}
				// penalize too-early queen movement
				if (blackQueensCount > 0 && situation[31] == 0)
				{
					int pieces = 0;
					x = situation[7];
					if (x == 41)
						pieces++;
					x = situation[15];
					if (x == 42)
						pieces++;
					x = situation[23];
					if (x == 43)
						pieces++;
					x = situation[47];
					if (x == 43)
						pieces++;
					x = situation[55];
					if (x == 42)
						pieces++;
					x = situation[63];
					if (x == 41)
						pieces++;
					score -= pieces << 3;
				}
				// finally, incite castling when the enemy has a queen on the board
				if (hasBlackCastled)
					castlingScore += 10;
				else
				{
					byte k = situation[39];
					byte rq = situation[7];
					byte rk = situation[63];

					if (k != 45 || (rq != 41 && rk != 41))
						castlingScore -= 120;
					else if (k == 45 && rq == 41 && rk == 41)
						castlingScore -= 24;
					else if (k == 45 && rq != 41 && rk == 41)
						castlingScore -= 40;
					else if (k == 45 && rq == 41 && rk != 41)
						castlingScore -= 80;
				}

				castlingWeight = 256 - endgamePhase;
				if (whiteQueensCount == 0)
					castlingWeight >>= 1;
				score += (castlingScore * castlingWeight) >> 8;

				kingVCoord = whiteKingsField % 8;
				kingHCoord = whiteKingsField >> 3;

				for (int i = 0; i < 64; i++ )
				{
					x = situation[i];
					if ((x & 8) == 8)
					{
						switch (x & 7)
						{
							case 1: // rook
								pieceVCoord = i % 8;
								pieceHCoord = i >> 3;
								score -= Math.Min(Math.Abs(kingVCoord - pieceVCoord),Math.Abs(kingHCoord - pieceHCoord)) << 1;

								index = i >> 3;
								// on the seventh rank?
								if ((i % 8) == 0)
									score += 22;
								// on a semi- or completely open file?
								if (columnOwnPawnCount[index] == 0)
								{
									if (columnOpponentPawnCount[index] == 0)
										score += 10;
									else
										score += 4;
								}
								// behind a passed pawn?
								if (columnOwnPassedPawnCount[index] < i)
									score += 25;

								break;
							case 2: // knight
								pieceVCoord = i % 8;
								pieceHCoord = i >> 3;
								score += 5 - Math.Abs(kingVCoord - pieceVCoord) - Math.Abs(kingHCoord - pieceHCoord);
								score += EvaluateMinorOnWeakSquare(i,PieceType.Knight,color,endgamePhase);
								// outpost bonus: knight on rank 3-1 (advance for black = low rank index), not attackable by opponent pawn
								if (pieceVCoord <= 3) // rank 4,3,2 (0-indexed rank 3,2,1)
								{
									bool isOutpost = true;
									if (pieceHCoord > 0 && columnOpponentPawnCount[pieceHCoord - 1] > 0)
									{
										// check if white pawn is on adjacent file one rank ahead for black (rank-1)
										int attackerField = (pieceHCoord - 1) * 8 + (pieceVCoord - 1);
										if (attackerField >= 0 && (situation[attackerField] == 32 || situation[attackerField] == 48))
											isOutpost = false;
									}
									if (isOutpost && pieceHCoord < 7 && columnOpponentPawnCount[pieceHCoord + 1] > 0)
									{
										int attackerField = (pieceHCoord + 1) * 8 + (pieceVCoord - 1);
										if (attackerField >= 0 && (situation[attackerField] == 32 || situation[attackerField] == 48))
											isOutpost = false;
									}
									if (isOutpost)
									{
										int centralityBonus = (pieceHCoord >= 2 && pieceHCoord <= 5) ? 10 : 4;
										int outpostBonus = 15 + centralityBonus + (7 - pieceVCoord) * 2;
										score += outpostBonus - ((outpostBonus * endgamePhase) >> 9);
									}
								}
								break;
							case 3: // bishop
								score += EvaluateMinorOnWeakSquare(i,PieceType.Bishop,color,endgamePhase);
								// are there a lot of pawns on fields with same color?
								if (Helper.FieldNumber2Color(i) == Color.White)
									score -= fieldColorPawnCount[0] << 3;
								else
									score -= fieldColorPawnCount[1] << 3;
								// diagonal mobility: count unobstructed squares on all 4 diagonals
								{
									int bishopFile = i >> 3;
									int bishopRank = i % 8;
									int mobilityScore = 0;
									int[] diagStepFiles = { 1, 1, -1, -1 };
									int[] diagStepRanks = { 1, -1, 1, -1 };
									for (int d = 0; d < 4; d++)
									{
										int f = bishopFile + diagStepFiles[d];
										int r = bishopRank + diagStepRanks[d];
										while (f >= 0 && f < 8 && r >= 0 && r < 8)
										{
											int sq = f * 8 + r;
											if (situation[sq] != 0)
												break; // blocked
											mobilityScore += 2;
											f += diagStepFiles[d];
											r += diagStepRanks[d];
										}
									}
									score += mobilityScore;
								}
								break;
							case 4: // queen
								pieceVCoord = i % 8;
								pieceHCoord = i >> 3;
								score -= Math.Min(Math.Abs(kingVCoord - pieceVCoord),Math.Abs(kingHCoord - pieceHCoord));
								break;
							default:
								break;
						}
					}
				}
			}
			return score;
		}

		private int EvaluateMinorOnWeakSquare(int field, PieceType pieceType, Color color, int endgamePhase)
		{
			int rank = field % 8;
			int file = field >> 3;

			bool inEnemyHalf = (color == Color.White) ? rank >= 4 : rank <= 3;
			if (!inEnemyHalf)
				return 0;

			Color opponent = Helper.OpponentColor(color);
			if (IsSquareAttackedByPawn(field,opponent))
				return 0;

			int bonus = pieceType == PieceType.Knight ? 8 : 6;
			if (file >= 2 && file <= 5 && rank >= 2 && rank <= 5)
				bonus += 4;

			int middlegameWeight = 256 - endgamePhase;
			return (bonus * middlegameWeight) >> 8;
		}

		private int EvaluateSpaceControl(Color color, int endgamePhase)
		{
			int whiteSpace = EvaluatePawnSpaceForColor(Color.White);
			int blackSpace = EvaluatePawnSpaceForColor(Color.Black);
			int relativeSpace = color == Color.White ? whiteSpace - blackSpace : blackSpace - whiteSpace;
			relativeSpace = ClampScore(relativeSpace,-48,48);
			int middlegameWeight = 256 - endgamePhase;
			return (relativeSpace * middlegameWeight) >> 8;
		}

		private int EvaluatePawnSpaceForColor(Color side)
		{
			int space = 0;
			Color opponent = Helper.OpponentColor(side);

			for (int i = 0; i < 64; i++)
			{
				byte piece = situation[i];
				if (piece == 0 || (piece & 7) != 0)
					continue;

				bool sidePawn = ((piece & 8) == 0 && side == Color.White) || ((piece & 8) == 8 && side == Color.Black);
				if (!sidePawn)
					continue;

				int rank = i % 8;
				int file = i >> 3;
				int targetRank = side == Color.White ? rank + 1 : rank - 1;
				if (targetRank < 0 || targetRank > 7)
					continue;

				if (file > 0)
					space += EvaluateSpaceTarget((file - 1) * 8 + targetRank,side,opponent);
				if (file < 7)
					space += EvaluateSpaceTarget((file + 1) * 8 + targetRank,side,opponent);
			}

			return space;
		}

		private int EvaluateSpaceTarget(int targetField, Color color, Color opponent)
		{
			int rank = targetField % 8;
			int file = targetField >> 3;

			bool enemyHalf = (color == Color.White) ? rank >= 3 : rank <= 4;
			if (!enemyHalf)
				return 0;

			int score = 2;
			if (file >= 2 && file <= 5)
				score++;
			if (situation[targetField] == 0)
				score++;
			if (!IsSquareAttackedByPawn(targetField,opponent))
				score++;

			return score;
		}

		private int EvaluateBlockadeQuality(Color color, int endgamePhase)
		{
			int score = 0;
			Color opponent = Helper.OpponentColor(color);

			for (int i = 0; i < 64; i++)
			{
				byte piece = situation[i];
				if (!IsOwnPiece(piece,color) || (piece & 7) != (int)PieceType.Knight)
					continue;

				int rank = i % 8;
				int file = i >> 3;
				int blockedPawnRank = color == Color.White ? rank + 1 : rank - 1;
				if (blockedPawnRank < 0 || blockedPawnRank > 7)
					continue;

				int blockedPawnField = file * 8 + blockedPawnRank;
				if (!IsPawnOfColor(blockedPawnField,opponent))
					continue;

				int bonus = 10;
				if (file >= 2 && file <= 5)
					bonus += 2;
				if (!IsSquareAttackedByPawn(i,opponent))
					bonus += 4;

				int endgameWeight = 128 + (endgamePhase >> 1);
				score += (bonus * endgameWeight) >> 8;
			}

			score = ClampScore(score,0,32);
			return score;
		}

		private int EvaluateKeySquareCoordination(Color color, int endgamePhase)
		{
			int[] keySquares = new int[] { 27, 35, 28, 36 }; // d4, e4, d5, e5
			int[] ownAttackCounts = new int[keySquares.Length];

			for (int i = 0; i < 64; i++)
			{
				byte piece = situation[i];
				if (!IsOwnPiece(piece,color))
					continue;

				int type = piece & 7;
				if (type != (int)PieceType.Knight && type != (int)PieceType.Bishop && type != (int)PieceType.Queen)
					continue;

				for (int k = 0; k < keySquares.Length; k++)
				{
					if (DoesPieceAttackSquare(i,piece,keySquares[k]))
						ownAttackCounts[k]++;
				}
			}

			int score = 0;
			for (int k = 0; k < keySquares.Length; k++)
			{
				int attackers = ownAttackCounts[k];
				if (attackers == 0)
					continue;

				score += attackers;
				if (attackers >= 2)
					score += (attackers - 1) << 1;

				byte occupant = situation[keySquares[k]];
				if (IsOwnPiece(occupant,color) && attackers >= 2)
					score += 2;
			}

			score = ClampScore(score,0,40);

			int middlegameWeight = 256 - endgamePhase;
			return (score * middlegameWeight) >> 7;
		}

		private int EvaluateBishopPairComplexPressure(Color color, int endgamePhase)
		{
			int ownBishops = color == Color.White ? whiteBishopsCount : blackBishopsCount;
			if (ownBishops < 2)
				return 0;

			Color opponent = Helper.OpponentColor(color);
			int opponentLightPawns = CountPawnsOnSquareColor(opponent,Color.White);
			int opponentDarkPawns = CountPawnsOnSquareColor(opponent,Color.Black);
			int imbalance = Math.Abs(opponentLightPawns - opponentDarkPawns);
			imbalance = ClampScore(imbalance,0,4);

			int bishopPairBase = 10;
			int colorComplexPressure = imbalance << 1;
			int score = bishopPairBase + colorComplexPressure;
			score = ClampScore(score,0,24);

			int middlegameWeight = 256 - (endgamePhase >> 1);
			return (score * middlegameWeight) >> 8;
		}

		private int CountPawnsOnSquareColor(Color pawnColor, Color squareColor)
		{
			int count = 0;
			for (int i = 0; i < 64; i++)
			{
				if (!IsPawnOfColor(i,pawnColor))
					continue;

				if (Helper.FieldNumber2Color(i) == squareColor)
					count++;
			}

			return count;
		}

		private int EvaluateHeavyPiecePenetration(Color color, int endgamePhase)
		{
			int score = 0;
			Color opponent = Helper.OpponentColor(color);

			for (int i = 0; i < 64; i++)
			{
				byte piece = situation[i];
				if (!IsOwnPiece(piece,color))
					continue;

				int type = piece & 7;
				if (type != (int)PieceType.Rook && type != (int)PieceType.Queen)
					continue;

				int rank = i % 8;
				int file = i >> 3;
				bool inEnemyCamp = color == Color.White ? rank >= 5 : rank <= 2;
				if (!inEnemyCamp)
					continue;

				int pieceScore = type == (int)PieceType.Queen ? 8 : 6;
				if (rank == (color == Color.White ? 6 : 1))
					pieceScore += 3;
				if (!IsSquareAttackedByPawn(i,opponent))
					pieceScore += 2;

				int forward = color == Color.White ? 1 : -1;
				int frontRank = rank + forward;
				if (frontRank >= 0 && frontRank <= 7)
				{
					int frontField = file * 8 + frontRank;
					if (situation[frontField] == 0)
						pieceScore += 2;
				}

				score += pieceScore;
			}

			score = ClampScore(score,0,48);

			int middlegameWeight = 192 + ((256 - endgamePhase) >> 2);
			return (score * middlegameWeight) >> 8;
		}

		private int ClampScore(int score, int min, int max)
		{
			if (score < min)
				return min;
			if (score > max)
				return max;

			return score;
		}

		private bool IsOwnPiece(byte piece, Color color)
		{
			if (piece == 0)
				return false;

			if (color == Color.White)
				return (piece & 8) == 0;
			else
				return (piece & 8) == 8;
		}

		private bool IsPawnOfColor(int field, Color color)
		{
			byte piece = situation[field];
			if (piece == 0 || (piece & 7) != (int)PieceType.Pawn)
				return false;

			if (color == Color.White)
				return (piece & 8) == 0;
			else
				return (piece & 8) == 8;
		}

		private bool DoesPieceAttackSquare(int sourceField, byte piece, int targetField)
		{
			if (sourceField == targetField)
				return false;

			int sourceFile = sourceField >> 3;
			int sourceRank = sourceField % 8;
			int targetFile = targetField >> 3;
			int targetRank = targetField % 8;
			int deltaFile = targetFile - sourceFile;
			int deltaRank = targetRank - sourceRank;

			int type = piece & 7;
			if (type == (int)PieceType.Knight)
			{
				int adf = Math.Abs(deltaFile);
				int adr = Math.Abs(deltaRank);
				return (adf == 1 && adr == 2) || (adf == 2 && adr == 1);
			}

			if (type == (int)PieceType.Bishop)
				return IsSlidingAttack(sourceField,targetField,true,false);

			if (type == (int)PieceType.Queen)
				return IsSlidingAttack(sourceField,targetField,true,true);

			return false;
		}

		private bool IsSlidingAttack(int sourceField, int targetField, bool allowDiagonal, bool allowStraight)
		{
			int sourceFile = sourceField >> 3;
			int sourceRank = sourceField % 8;
			int targetFile = targetField >> 3;
			int targetRank = targetField % 8;

			int fileDelta = targetFile - sourceFile;
			int rankDelta = targetRank - sourceRank;
			int absFileDelta = Math.Abs(fileDelta);
			int absRankDelta = Math.Abs(rankDelta);

			int stepFile = 0;
			int stepRank = 0;

			if (allowDiagonal && absFileDelta == absRankDelta)
			{
				stepFile = fileDelta > 0 ? 1 : -1;
				stepRank = rankDelta > 0 ? 1 : -1;
			}
			else if (allowStraight && (fileDelta == 0 || rankDelta == 0))
			{
				if (fileDelta == 0)
				{
					stepFile = 0;
					stepRank = rankDelta > 0 ? 1 : -1;
				}
				else
				{
					stepFile = fileDelta > 0 ? 1 : -1;
					stepRank = 0;
				}
			}
			else
			{
				return false;
			}

			int file = sourceFile + stepFile;
			int rank = sourceRank + stepRank;
			while (file != targetFile || rank != targetRank)
			{
				int field = file * 8 + rank;
				if (situation[field] != 0)
					return false;

				file += stepFile;
				rank += stepRank;
			}

			return true;
		}

		private bool IsSquareAttackedByPawn(int field, Color pawnColor)
		{
			int rank = field % 8;
			int file = field >> 3;

			if (pawnColor == Color.White)
			{
				if (rank > 0)
				{
					if (file > 0)
					{
						byte p = situation[(file - 1) * 8 + (rank - 1)];
						if (p != 0 && (p & 7) == 0 && (p & 8) == 0)
							return true;
					}
					if (file < 7)
					{
						byte p = situation[(file + 1) * 8 + (rank - 1)];
						if (p != 0 && (p & 7) == 0 && (p & 8) == 0)
							return true;
					}
				}
			}
			else
			{
				if (rank < 7)
				{
					if (file > 0)
					{
						byte p = situation[(file - 1) * 8 + (rank + 1)];
						if (p != 0 && (p & 7) == 0 && (p & 8) == 8)
							return true;
					}
					if (file < 7)
					{
						byte p = situation[(file + 1) * 8 + (rank + 1)];
						if (p != 0 && (p & 7) == 0 && (p & 8) == 8)
							return true;
					}
				}
			}

			return false;
		}

		private int EvaluateMinorWeakSquareControl(Color color, int endgamePhase)
		{
			int score = 0;
			for (int i = 0; i < 64; i++)
			{
				byte piece = situation[i];
				if (!IsOwnPiece(piece,color))
					continue;

				int type = piece & 7;
				if (type == (int)PieceType.Knight)
					score += EvaluateMinorOnWeakSquare(i,PieceType.Knight,color,endgamePhase);
				else if (type == (int)PieceType.Bishop)
					score += EvaluateMinorOnWeakSquare(i,PieceType.Bishop,color,endgamePhase);
			}

			return score;
		}

		private int ComputeEndgamePhase(Color color)
		{
			int ownNonPawnMaterial;
			int opponentNonPawnMaterial;

			if (color == Color.White)
			{
				ownNonPawnMaterial =
					whiteRooksCount*Helper.PieceType2Value(PieceType.Rook) +
					whiteKnightsCount*Helper.PieceType2Value(PieceType.Knight) +
					whiteBishopsCount*Helper.PieceType2Value(PieceType.Bishop) +
					whiteQueensCount*Helper.PieceType2Value(PieceType.Queen);
				opponentNonPawnMaterial =
					blackRooksCount*Helper.PieceType2Value(PieceType.Rook) +
					blackKnightsCount*Helper.PieceType2Value(PieceType.Knight) +
					blackBishopsCount*Helper.PieceType2Value(PieceType.Bishop) +
					blackQueensCount*Helper.PieceType2Value(PieceType.Queen);
			}
			else
			{
				ownNonPawnMaterial =
					blackRooksCount*Helper.PieceType2Value(PieceType.Rook) +
					blackKnightsCount*Helper.PieceType2Value(PieceType.Knight) +
					blackBishopsCount*Helper.PieceType2Value(PieceType.Bishop) +
					blackQueensCount*Helper.PieceType2Value(PieceType.Queen);
				opponentNonPawnMaterial =
					whiteRooksCount*Helper.PieceType2Value(PieceType.Rook) +
					whiteKnightsCount*Helper.PieceType2Value(PieceType.Knight) +
					whiteBishopsCount*Helper.PieceType2Value(PieceType.Bishop) +
					whiteQueensCount*Helper.PieceType2Value(PieceType.Queen);
			}

			int totalNonPawnMaterial = ownNonPawnMaterial + opponentNonPawnMaterial;
			if (totalNonPawnMaterial < 0)
				totalNonPawnMaterial = 0;
			if (totalNonPawnMaterial > 6400)
				totalNonPawnMaterial = 6400;

			return ((6400 - totalNonPawnMaterial) * 256) / 6400;
		}

		/// <summary>
		/// Holds a detailed, per-term evaluation breakdown for debugging and tuning.
		/// </summary>
		public sealed class EvaluationBreakdown
		{
			public bool IsDrawnPosition { get; set; }
			public int MaterialScore { get; set; }
			public int PositionalScore { get; set; }
			public int SpaceControlScore { get; set; }
			public int MinorWeakSquareScore { get; set; }
			public int BlockadeQualityScore { get; set; }
			public int KeySquareCoordinationScore { get; set; }
			public int BishopPairComplexPressureScore { get; set; }
			public int HeavyPiecePenetrationScore { get; set; }
			public int TrackedHeuristicsScore { get; set; }
			public int ResidualPositionalScore { get; set; }
			public int EndgamePhase { get; set; }
			public int TotalScore { get; set; }
		}

		/// <summary>
		/// Computes a detailed per-term evaluation breakdown for debugging/tuning.
		/// </summary>
		/// <param name="color">Perspective side.</param>
		/// <returns>Material, positional and tracked heuristic subtotals plus final score.</returns>
		public EvaluationBreakdown GetEvaluationBreakdown(Color color)
		{
			if (IsDrawByInsufficientMaterial() || IsDrawByRepetition() || IsDrawByFiftyMoveRule())
			{
				return new EvaluationBreakdown
				{
					IsDrawnPosition = true,
					MaterialScore = 0,
					PositionalScore = 0,
					SpaceControlScore = 0,
					MinorWeakSquareScore = 0,
					BlockadeQualityScore = 0,
					KeySquareCoordinationScore = 0,
					BishopPairComplexPressureScore = 0,
					HeavyPiecePenetrationScore = 0,
					TrackedHeuristicsScore = 0,
					ResidualPositionalScore = 0,
					EndgamePhase = 0,
					TotalScore = 0
				};
			}

			AnalyzePawnSituation(color);

			int endgamePhase = ComputeEndgamePhase(color);
			int material = EvaluateMaterial(color);
			int positional = EvaluateSituation(color);

			int space = EvaluateSpaceControl(color,endgamePhase);
			int minorWeakSquares = EvaluateMinorWeakSquareControl(color,endgamePhase);
			int blockade = EvaluateBlockadeQuality(color,endgamePhase);
			int coordination = EvaluateKeySquareCoordination(color,endgamePhase);
			int bishopPairPressure = EvaluateBishopPairComplexPressure(color,endgamePhase);
			int heavyPenetration = EvaluateHeavyPiecePenetration(color,endgamePhase);

			int tracked = space + minorWeakSquares + blockade + coordination + bishopPairPressure + heavyPenetration;
			int residual = positional - tracked;

			return new EvaluationBreakdown
			{
				IsDrawnPosition = false,
				MaterialScore = material,
				PositionalScore = positional,
				SpaceControlScore = space,
				MinorWeakSquareScore = minorWeakSquares,
				BlockadeQualityScore = blockade,
				KeySquareCoordinationScore = coordination,
				BishopPairComplexPressureScore = bishopPairPressure,
				HeavyPiecePenetrationScore = heavyPenetration,
				TrackedHeuristicsScore = tracked,
				ResidualPositionalScore = residual,
				EndgamePhase = endgamePhase,
				TotalScore = material + positional
			};
		}

		/// <summary>
		/// Formats the evaluation breakdown as a compact multi-line debug string.
		/// </summary>
		/// <param name="color">Perspective side.</param>
		/// <returns>Human-readable breakdown text for logs/debug output.</returns>
		public string GetEvaluationBreakdownString(Color color)
		{
			EvaluationBreakdown breakdown = GetEvaluationBreakdown(color);
			if (breakdown.IsDrawnPosition)
				return "Evaluation breakdown: Drawn position => total 0";

			StringBuilder sb = new StringBuilder();
			sb.AppendLine("Evaluation breakdown:");
			sb.AppendLine($"  EndgamePhase: {breakdown.EndgamePhase}");
			sb.AppendLine($"  Material: {breakdown.MaterialScore}");
			sb.AppendLine($"  Positional: {breakdown.PositionalScore}");
			sb.AppendLine($"    SpaceControl: {breakdown.SpaceControlScore}");
			sb.AppendLine($"    MinorWeakSquares: {breakdown.MinorWeakSquareScore}");
			sb.AppendLine($"    BlockadeQuality: {breakdown.BlockadeQualityScore}");
			sb.AppendLine($"    KeySquareCoordination: {breakdown.KeySquareCoordinationScore}");
			sb.AppendLine($"    BishopPairComplexPressure: {breakdown.BishopPairComplexPressureScore}");
			sb.AppendLine($"    HeavyPiecePenetration: {breakdown.HeavyPiecePenetrationScore}");
			sb.AppendLine($"    TrackedHeuristics: {breakdown.TrackedHeuristicsScore}");
			sb.AppendLine($"    ResidualPositional: {breakdown.ResidualPositionalScore}");
			sb.Append($"  Total: {breakdown.TotalScore}");

			return sb.ToString();
		}

		// Look at pawn positions to be able to detect features such as doubled,
		// isolated or passed pawns
		private void AnalyzePawnSituation(Color color)
		{
			// Reset the counters
			int tmp = color == Color.White ? 0 : 63;
			for (int i = 0; i < 8; i++)
			{
				columnOwnPawnCount[i] = 0;
				columnOpponentPawnCount[i] = 0;
				columnMostAdvancedOwnPawnFieldNum[i] = tmp;
				columnLeastAdvancedOpponentPawnFieldNum[i] = tmp;
				columnOwnPassedPawnCount[i] = tmp;
			}
			fieldColorPawnCount[0] = 0;
			fieldColorPawnCount[1] = 0;
			pawnRamCount = 0;

			int index = 0;
			int x = 0;

			// Now, perform the analysis
			if (color == Color.White)
			{
				for (int i = 0; i < 64; i++)
				{
					if ((i % 8) == 7 || (i % 8) == 0)
						continue;

					x = situation[i];

					// Look for a white pawn first, and count its properties
					if (x == 32 || x == 48)
					{
						// What is the pawn's position?
						index = i >> 3;

						// This pawn is now the most advanced of all white pawns on its file
						columnOwnPawnCount[index]++;
						if (i > columnMostAdvancedOwnPawnFieldNum[index])
							columnMostAdvancedOwnPawnFieldNum[index] = i;

						// Is this pawn on a white or a black square?
						if (Helper.FieldNumber2Color(i) == Color.White)
							fieldColorPawnCount[0]++;
						else
							fieldColorPawnCount[1]++;

						// Look for a "pawn ram", i.e., a situation where a black pawn
						// is located in the square immediately ahead of this one.
						x = situation[i+1];
						if (x == 40 || x == 56)
							pawnRamCount++;
					}
					// Now, look for a BLACK pawn
					else if (x == 40 || x == 56)
					{
						// If the black pawn exists, it is the most backward found so far
						// on its file
						index = i >> 3;
						columnOpponentPawnCount[index]++;
						if (i > columnLeastAdvancedOpponentPawnFieldNum[index])
							columnLeastAdvancedOpponentPawnFieldNum[index] = i;
					}
				}
			}
			else // Analyze from Black's perspective
			{
				for (int i = 0; i < 64; i++)
				{
					if ((i % 8) == 0 || (i % 8) == 7)
						continue;

					x = situation[i];

					// Look for a black pawn first, and count its properties
					if (x == 40 || x == 56)
					{
						// What is the pawn's position?
						index = i >> 3;

						// This pawn is now the most advanced of all black pawns on its file
						columnOwnPawnCount[index]++;
						if (i < columnMostAdvancedOwnPawnFieldNum[index])
							columnMostAdvancedOwnPawnFieldNum[index] = i;

						// Is this pawn on a white or a black square?
						if (Helper.FieldNumber2Color(i) == Color.White)
							fieldColorPawnCount[0]++;
						else
							fieldColorPawnCount[1]++;

						// Look for a "pawn ram", i.e., a situation where a white pawn
						// is located in the square immediately ahead of this one.
						x = situation[i];
						if (x == 32 || x == 48)
							pawnRamCount++;
					}
					// Now, look for a wHITE pawn
					else if (x == 32 || x == 48)
					{
						// If the white pawn exists, it is the most backward found so far
						index = i >> 3;
						columnOpponentPawnCount[index]++;
						if (i < columnLeastAdvancedOpponentPawnFieldNum[index])
							columnLeastAdvancedOpponentPawnFieldNum[index] = i;
					}
				}
			}
		}

		/// <summary>
		/// Evaluates the complete position from the given player's perspective using material and positional terms.
		/// 
		/// Evaluation includes:
		/// 1. Draw detection check - returns 0 if position is drawn by any rule
		/// 2. Material evaluation - pawn/piece value calculation
		/// 3. Positional evaluation - king safety, pawn structure, piece placement, endgame heuristics
		/// 
		/// Positional evaluation is phase-aware (tapered between opening/middlegame and endgame):
		/// - King centralization bonus increases in endgames
		/// - Passed pawn bonuses scale up in endgames
		/// - Castling pressure (rooks on files) tapers in endgames
		/// - Material-relative weights shift from development focus to pawn promotion focus
		/// 
		/// Higher return values indicate better positions for the given color.
		/// This is the primary evaluation routine used during quiescence search leaf nodes.
		/// </summary>
		/// <param name="color">The player from whose perspective the position is evaluated (White or Black).</param>
		/// <returns>
		/// Evaluation score (positive = color is winning, negative = color is losing, 0 = equal or drawn).
		/// Typical ranges: ±100 for pawn advantage, ±300 for minor piece, ±500 for rook, ±900 for queen.
		/// Returns 0 if position is drawn (insufficient material, repetition, fifty-move rule).
		/// </returns>
		public int EvaluateComplete(Color color)
		{
			if (IsDrawByInsufficientMaterial() || IsDrawByRepetition() || IsDrawByFiftyMoveRule())
				return 0;

			AnalyzePawnSituation(color);
			return
				EvaluateMaterial(color)+
				EvaluateSituation(color);
		}


		/// <summary>
		/// Computes the material-only evaluation of the current position from the given perspective.
		/// </summary>
		/// <param name="color">The side from whose perspective the score is computed.</param>
		/// <returns>A positive value indicates a material advantage for <paramref name="color"/>.</returns>
		/// <summary>
		/// Checks if the position is a draw due to insufficient material to achieve checkmate.
		/// 
		/// FIDE Rules recognize draws when one side lacks mating material:
		/// - King vs King (trivial draw)
		/// - King + Single Minor Piece vs King (bishop or knight cannot deliver mate alone)
		/// - King + Two Knights vs King (legal but practically impossible to mate)
		/// - King + Bishop vs King + Bishop on opposite-colored squares (cannot mate, often drawn)
		/// 
		/// This method checks only the impossible-to-mate cases. In positions with:
		/// - Any pawns, rooks, or queens: return false (mating is possible)
		/// - Only kings and minor pieces: apply insufficient material rules
		/// 
		/// Note: King + same-colored bishops may technically lead to mate in some positions,
		/// but FIDE considers it a draw anyway for simplicity. This implementation does not
		/// distinguish bishop colors.
		/// </summary>
		/// <returns>True if the position has insufficient material for either side to deliver checkmate; false otherwise.</returns>
		public bool IsDrawByInsufficientMaterial()
		{
			if (whitePawnsCount > 0 || blackPawnsCount > 0)
				return false;
			if (whiteRooksCount > 0 || blackRooksCount > 0)
				return false;
			if (whiteQueensCount > 0 || blackQueensCount > 0)
				return false;

			int whiteMinors = whiteKnightsCount + whiteBishopsCount;
			int blackMinors = blackKnightsCount + blackBishopsCount;

			if (whiteMinors == 0 && blackMinors == 0)
				return true;
			if (whiteMinors == 1 && blackMinors == 0)
				return true;
			if (whiteMinors == 0 && blackMinors == 1)
				return true;
			if (whiteMinors == 1 && blackMinors == 1)
				return true;
			if (whiteMinors == 2 && blackMinors == 0 && whiteKnightsCount == 2)
				return true;
			if (blackMinors == 2 && whiteMinors == 0 && blackKnightsCount == 2)
				return true;

			return false;
		}

		private int GetRepetitionHashCode()
		{
			int hash = whoToMove == Color.White ? 917519 : 190871;
			for (int i = 0; i < 64; i++)
			{
				byte x = situation[i];
				if (x == 0)
					continue;

				byte normalized = x;
				PieceType pieceType = (PieceType)(x & 7);
				if (pieceType == PieceType.Knight || pieceType == PieceType.Bishop || pieceType == PieceType.Queen)
					normalized = (byte)((x & 8) | (x & 7));

				hash ^= hashKeyComponents[normalized,i];
			}
			return hash;
		}

		private int GetRepetitionLockCode()
		{
			int hash = whoToMove == Color.White ? 339391 : 5575573;
			for (int i = 0; i < 64; i++)
			{
				byte x = situation[i];
				if (x == 0)
					continue;

				byte normalized = x;
				PieceType pieceType = (PieceType)(x & 7);
				if (pieceType == PieceType.Knight || pieceType == PieceType.Bishop || pieceType == PieceType.Queen)
					normalized = (byte)((x & 8) | (x & 7));

				hash ^= hashKeyComponents[normalized == 0 ? 0 : normalized-1,i];
			}
			return hash;
		}

		/// <summary>
		/// Checks if the position has occurred the required number of times (threefold repetition by default).
		/// 
		/// FIDE Rules:
		/// - Threefold repetition (same position 3 times) may be claimed as a draw
		/// - Positions are identical when board state AND side-to-move match
		/// - These instances need not be consecutive
		/// 
		/// Implementation Details:
		/// - Uses specialized GetRepetitionHashCode/GetRepetitionLockCode for position keys
		/// - These keys NORMALIZE piece "moved" flags for non-critical pieces (knights, bishops, queens)
		///   but PRESERVE them for pivotal pieces (pawns, rooks, kings)
		/// - Why? Knights/bishops/queens lose tempo-information irrelevance; pawns/rooks/kings don't
		/// - Walks move history backwards, counting position occurrences at the same side-to-move
		/// - Returns true when requiredOccurrences instances are found
		/// 
		/// Default requiredOccurrences=3 implements chess rules; testing may use 2 for shortened games.
		/// </summary>
		/// <param name="requiredOccurrences">Number of repetitions to trigger draw (default 3 per FIDE rules).</param>
		/// <returns>True if this position has occurred requiredOccurrences times with same side-to-move; false otherwise.</returns>
		public bool IsDrawByRepetition(int requiredOccurrences = 3)
		{
			if (requiredOccurrences < 2)
				requiredOccurrences = 2;

			if (history == null || history.Count < 4)
				return false;

			int hash = this.GetRepetitionHashCode();
			int lockCode = this.GetRepetitionLockCode();
			int occurrenceCount = 1;

			for (int i = 0; i < history.Count; i++)
			{
				Move historicMove = history[i];
				if (historicMove == null || historicMove.SnapShot == null)
					continue;

				SnapShot prior = historicMove.SnapShot;
				if (prior.whoToMove != this.whoToMove)
					continue;

				if (prior.GetRepetitionHashCode() == hash && prior.GetRepetitionLockCode() == lockCode)
				{
					occurrenceCount++;
					if (occurrenceCount >= requiredOccurrences)
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Checks if the position is a draw under the fifty-move rule.
		/// 
		/// FIDE Rules:
		/// - After 50 full moves (100 half-moves/plies) with no pawn moves and no captures,
		///   either player may claim a draw
		/// - Most engines treat this as an automatic draw for search efficiency
		/// 
		/// Implementation:
		/// - Scans move history backwards from the current position
		/// - Counts consecutive half-moves that are:
		///   * NOT pawn moves (pawn captures automatically satisfy pawn-move criterion)
		///   * NOT captures (PieceHit == None)
		/// - Stops counting at first reset event (pawn move OR capture)
		/// - Returns true if 100 consecutive qualifying half-moves are found
		/// 
		/// Edge Cases:
		/// - If move history has fewer than 100 moves, returns false  
		/// - Used in both search and evaluation for draw detection cutoffs
		/// </summary>
		/// <returns>True if the last 100 half-moves contain no pawn moves and no captures; false otherwise.</returns>
		public bool IsDrawByFiftyMoveRule()
		{
			// Fifty-move rule: if fifty consecutive half-moves (per side) occur without a pawn move or capture,
			// the position is drawn. In move history, this means the last 100 half-moves must all be non-pawn, non-capture moves.
			if (history == null || history.Count < 100)
				return false;

			// Walk backwards from the end of history, counting non-pawn, non-capture moves.
			// Stop if we hit a pawn move or capture.
			int halfMovesSinceReset = 0;
			for (int i = history.Count - 1; i >= 0; i--)
			{
				Move move = history[i];
				if (move == null)
					break;

				// If this move was a pawn move or a capture, reset the counter
				if (move.PieceType == PieceType.Pawn || move.PieceHit != PieceType.None)
					break;

				halfMovesSinceReset++;

				// If we've found 100 consecutive half-moves without pawn move or capture, it's a draw
				if (halfMovesSinceReset >= 100)
					return true;
			}

			return false;
		}

		public int EvaluateMaterial(Color color)
		{
			// If both sides are equal, no need to compute anything!
			if (whitePawnsCount == blackPawnsCount && whiteRooksCount == blackRooksCount && whiteKnightsCount == blackKnightsCount && whiteBishopsCount == blackBishopsCount && whiteQueensCount == blackQueensCount && whiteKingsCount == blackKingsCount)
				return 0;

			int white =
				whitePawnsCount*Helper.PieceType2Value(PieceType.Pawn) +
				whiteRooksCount*Helper.PieceType2Value(PieceType.Rook) +
				whiteKnightsCount*Helper.PieceType2Value(PieceType.Knight) +
				whiteBishopsCount*Helper.PieceType2Value(PieceType.Bishop) +
				whiteQueensCount*Helper.PieceType2Value(PieceType.Queen) +
				whiteKingsCount*Helper.PieceType2Value(PieceType.King);
			int black =
				blackPawnsCount*Helper.PieceType2Value(PieceType.Pawn) +
				blackRooksCount*Helper.PieceType2Value(PieceType.Rook) +
				blackKnightsCount*Helper.PieceType2Value(PieceType.Knight) +
				blackBishopsCount*Helper.PieceType2Value(PieceType.Bishop) +
				blackQueensCount*Helper.PieceType2Value(PieceType.Queen) +
				blackKingsCount*Helper.PieceType2Value(PieceType.King);

			int total = white+black;

			// Who is leading the game, material-wise?
			if (black > white)
			{
				// Black leading
				int diff = black-white;
				int val = Math.Min(2400,diff) + (diff * (12000 - total) * blackPawnsCount) / (6400 * (blackPawnsCount + 1));
				if (color == Color.Black)
					return val;
				else
					return -val;
			}
			else
			{
				// White leading
				int diff = white-black;
				int val = Math.Min(2400,diff) + (diff * (12000 - total) * whitePawnsCount) / (6400 * (whitePawnsCount + 1));
				if (color == Color.White)
					return val;
				else
					return -val;
			}
		}

		/// <summary>
		/// Raised when a pawn promotion choice is required.
		/// </summary>
		public event PromotePawnEventHandler PromotePawnEventHandler;
		/// <summary>
		/// Raised when the side to move gives check after a move.
		/// </summary>
		public event CheckEventHandler CheckEventHandler;
		/// <summary>
		/// Raised when the game ends by checkmate or stalemate.
		/// </summary>
		public event GameOverEventHandler GameOverEventHandler;
		/// <summary>
		/// Raised when a piece is captured.
		/// </summary>
		public event PieceHitEventHandler PieceHitEventHandler;

		/// <summary>
		/// Creates a new snapshot in the standard chess start position and precomputes legal moves.
		/// </summary>
		/// <returns>A snapshot ready for play with white to move.</returns>
		public static SnapShot StartUpSnapShot()
		{
			SnapShot s = new SnapShot((byte[])startupSituation.Clone(),Color.White,false,false,true);
			s.CalculateLegalMoves();
			return s;
		}

		/// <summary>
		/// Creates a snapshot from a FEN string.
		/// </summary>
		/// <param name="fen">Forsyth-Edwards Notation text containing board and side-to-move fields.</param>
		/// <returns>A snapshot initialized from the FEN content.</returns>
		/// <exception cref="ArgumentException">Thrown when FEN input is null/empty or invalid.</exception>
		public static SnapShot FromFen(string fen)
		{
			if (string.IsNullOrWhiteSpace(fen))
				throw new ArgumentException("Invalid","fen");

			string[] fields = fen.Trim().Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
			if (fields.Length < 2)
				throw new ArgumentException("Invalid FEN","fen");

			byte[] board = new byte[64];
			string[] ranks = fields[0].Split('/');
			if (ranks.Length != 8)
				throw new ArgumentException("Invalid board section in FEN","fen");

			for (int rankIndex = 0; rankIndex < 8; rankIndex++)
			{
				string rank = ranks[rankIndex];
				int file = 0;

				for (int i = 0; i < rank.Length; i++)
				{
					char c = rank[i];
					if (c >= '1' && c <= '8')
					{
						file += c - '0';
						continue;
					}

					if (file >= 8)
						throw new ArgumentException("Invalid board section in FEN","fen");

					int internalRank = 7 - rankIndex;
					int fieldNumber = file * 8 + internalRank;
					board[fieldNumber] = FenPieceToBoardValue(c);
					file++;
				}

				if (file != 8)
					throw new ArgumentException("Invalid board section in FEN","fen");
			}

			Color whoToMove = fields[1] == "b" ? Color.Black : Color.White;
			if (fields[1] != "w" && fields[1] != "b")
				throw new ArgumentException("Invalid side-to-move section in FEN","fen");

			bool hasWhiteCastled = false;
			bool hasBlackCastled = false;

			if (board[48] == ((byte)PieceType.King | (byte)Mask.NullFlag) || board[16] == ((byte)PieceType.King | (byte)Mask.NullFlag))
				hasWhiteCastled = true;
			if (board[55] == ((byte)PieceType.King | (byte)Mask.Color | (byte)Mask.NullFlag) || board[23] == ((byte)PieceType.King | (byte)Mask.Color | (byte)Mask.NullFlag))
				hasBlackCastled = true;

			return new SnapShot(board,whoToMove,hasWhiteCastled,hasBlackCastled,false);
		}

		private static byte FenPieceToBoardValue(char token)
		{
			Color color;
			char pieceToken;
			if (token >= 'A' && token <= 'Z')
			{
				color = Color.White;
				pieceToken = token;
			}
			else if (token >= 'a' && token <= 'z')
			{
				color = Color.Black;
				pieceToken = (char)(token - 32);
			}
			else
			{
				throw new ArgumentException("Invalid board section in FEN","token");
			}

			PieceType pieceType;
			switch (pieceToken)
			{
				case 'P':
					pieceType = PieceType.Pawn;
					break;
				case 'N':
					pieceType = PieceType.Knight;
					break;
				case 'B':
					pieceType = PieceType.Bishop;
					break;
				case 'R':
					pieceType = PieceType.Rook;
					break;
				case 'Q':
					pieceType = PieceType.Queen;
					break;
				case 'K':
					pieceType = PieceType.King;
					break;
				default:
					throw new ArgumentException("Invalid board section in FEN","token");
			}

			return (byte)((byte)Mask.NullFlag | (byte)pieceType | (byte)color);
		}

		private static byte[] startupSituation = new byte[] {
																33,	32,	0,	0,	0,	0,	40,	41,
																34,	32,	0,	0,	0,	0,	40,	42,
																35,	32,	0,	0,	0,	0,	40,	43,
																36,	32,	0,	0,	0,	0,	40,	44,
																37,	32,	0,	0,	0,	0,	40,	45,
																35,	32,	0,	0,	0,	0,	40,	43,
																34,	32,	0,	0,	0,	0,	40,	42,
																33,	32,	0,	0,	0,	0,	40,	41	};
		private static int[,] hashKeyComponents = new int[((int)(Byte.MaxValue))+1,64];

		static SnapShot()
		{
			for (int i = 0; i < ((int)(Byte.MaxValue))+1; i++)
				for (int j = 0; j < 64; j++)
					hashKeyComponents[i,j] = Helper.RandomNumber();
		}

		public override int GetHashCode()
		{
/*
			string situationString = (whoToMove == Color.White ? "w:" : "b:");
			for (int i = 0; i < 64; i++)
				situationString += situation[i].ToString()+",";
			return situationString.GetHashCode();
*/
			int hash = whoToMove == Color.White ? 178294 : 43984531;
			for (int i = 0; i < 64; i++)
			{
				byte x = situation[i];
				if (x != 0)
					hash ^= hashKeyComponents[x,i];
			}
			return hash;
		}

		public int GetHashLockCode()
		{
/*
			string situationString = (whoToMove == Color.White ? "x:" : "y:");
			for (int i = 0; i < 64; i++)
				situationString += situation[i].ToString()+".";
			return situationString.GetHashCode();
*/
			int hash = whoToMove == Color.White ? 609342 : 7455925;
			for (int i = 0; i < 64; i++)
			{
				byte x = situation[i];
				if (x != 0)
					hash ^= hashKeyComponents[x-1,i];
			}
			return hash;
		}

		private byte[] situation = null;

		private Color whoToMove = Color.White;
		public Color WhoToMove
		{
			get
			{
				return whoToMove;
			}
		}
		private MoveList legalMoves = new MoveList();
		public MoveList LegalMoves
		{
			get
			{
				return legalMoves.Clone();
			}
		}

		private SituationCode currentSituationCode = SituationCode.Normal;
		/// <summary>
		/// Gets the currently assessed game situation for the side to move.
		/// </summary>
		public SituationCode CurrentSituationCode
		{
			get
			{
				return currentSituationCode;
			}
		}

		private static string PieceToken(PieceType pieceType, Color pieceColor, bool useUnicode)
		{
			if (useUnicode)
			{
				byte piece = (byte)(((int)pieceType & (int)Mask.PieceType) | ((int)pieceColor & (int)Mask.Color) | (int)Mask.MovedFlag);
				return PieceToUnicodeChar(piece).ToString();
			}

			switch (pieceType)
			{
				case PieceType.Pawn:
					return "P";
				case PieceType.Knight:
					return "N";
				case PieceType.Bishop:
					return "B";
				case PieceType.Rook:
					return "R";
				case PieceType.Queen:
					return "Q";
				default:
					return "?";
			}
		}

		private static void AppendCapturedPiece(StringBuilder sb, PieceType pieceType, Color pieceColor, int count, bool useUnicode)
		{
			if (count <= 0)
				return;

			sb.Append(PieceToken(pieceType, pieceColor, useUnicode)).Append('x').Append(count).Append(' ');
		}

		private static string BuildCapturedPiecesString(int pawns, int rooks, int knights, int bishops, int queens, Color capturedPieceColor, bool useUnicode)
		{
			StringBuilder sb = new StringBuilder();
			AppendCapturedPiece(sb, PieceType.Queen, capturedPieceColor, queens, useUnicode);
			AppendCapturedPiece(sb, PieceType.Rook, capturedPieceColor, rooks, useUnicode);
			AppendCapturedPiece(sb, PieceType.Bishop, capturedPieceColor, bishops, useUnicode);
			AppendCapturedPiece(sb, PieceType.Knight, capturedPieceColor, knights, useUnicode);
			AppendCapturedPiece(sb, PieceType.Pawn, capturedPieceColor, pawns, useUnicode);

			if (sb.Length == 0)
				return "none";

			return sb.ToString().TrimEnd();
		}

		internal static string BuildCapturedPiecesStringForTests(int pawns, int rooks, int knights, int bishops, int queens, Color capturedPieceColor, bool useUnicode)
		{
			return BuildCapturedPiecesString(pawns, rooks, knights, bishops, queens, capturedPieceColor, useUnicode);
		}

		/// <summary>
		/// Builds a compact summary of pieces captured by each side.
		/// </summary>
		/// <param name="useUnicode">If true, uses Unicode chess symbols; otherwise ASCII letters.</param>
		/// <returns>Summary text for captured material by White and Black.</returns>
		public string GetCapturedPiecesSummary(bool useUnicode)
		{
			int capturedByWhitePawns = 8 - blackPawnsCount;
			int capturedByWhiteRooks = 2 - blackRooksCount;
			int capturedByWhiteKnights = 2 - blackKnightsCount;
			int capturedByWhiteBishops = 2 - blackBishopsCount;
			int capturedByWhiteQueens = 1 - blackQueensCount;

			int capturedByBlackPawns = 8 - whitePawnsCount;
			int capturedByBlackRooks = 2 - whiteRooksCount;
			int capturedByBlackKnights = 2 - whiteKnightsCount;
			int capturedByBlackBishops = 2 - whiteBishopsCount;
			int capturedByBlackQueens = 1 - whiteQueensCount;

			return "Captured by White: " + BuildCapturedPiecesString(capturedByWhitePawns, capturedByWhiteRooks, capturedByWhiteKnights, capturedByWhiteBishops, capturedByWhiteQueens, Color.Black, useUnicode)
				+ " | Captured by Black: " + BuildCapturedPiecesString(capturedByBlackPawns, capturedByBlackRooks, capturedByBlackKnights, capturedByBlackBishops, capturedByBlackQueens, Color.White, useUnicode);
		}

		/// <summary>
		/// Gets captured-pieces summary in ASCII format.
		/// </summary>
		public string CapturedPiecesSummary
		{
			get
			{
				return GetCapturedPiecesSummary(false);
			}
		}

		private int whitePiecesCount = 16;
		private int blackPiecesCount = 16;

		private int whitePawnsCount = 8;
		private int blackPawnsCount = 8;
		private int whiteRooksCount = 2;
		private int blackRooksCount = 2;
		private int whiteKnightsCount = 2;
		private int blackKnightsCount = 2;
		private int whiteBishopsCount = 2;
		private int blackBishopsCount = 2;
		private int whiteQueensCount = 1;
		private int blackQueensCount = 1;
		private int whiteKingsCount = 1;
		private int blackKingsCount = 1;

		private int whiteKingsField = 32;
		private int blackKingsField = 39;
		private bool hasWhiteCastled = false;
		private bool hasBlackCastled = false;
		private MoveList history = new MoveList();
		
		private SnapShot()
		{
		}

		private SnapShot(byte[] situation, Color whoToMove, bool hasWhiteCastled, bool hasBlackCastled, bool internalUsage)
		{
			if (situation == null || situation.Length != 64)
				throw new ArgumentException("Invalid","situation");
			this.situation = situation;
			this.whoToMove = whoToMove;

			if (!internalUsage)
			{
				this.hasWhiteCastled = hasWhiteCastled;
				this.hasBlackCastled = hasBlackCastled;

				whitePiecesCount = 0;
				blackPiecesCount = 0;

				whitePawnsCount = 0;
				blackPawnsCount = 0;
				whiteRooksCount = 0;
				blackRooksCount = 0;
				whiteKnightsCount = 0;
				blackKnightsCount = 0;
				whiteBishopsCount = 0;
				blackBishopsCount = 0;
				whiteQueensCount = 0;
				blackQueensCount = 0;
				whiteKingsCount = 0;
				blackKingsCount = 0;

				int p = 0;
				int x = 0;

				for (int i = 0; i < 64; i++)
				{
					p = this.situation[i];
					this.situation[i] = (byte)(p & 63);
					switch (this.situation[i] & 47)
					{
						case 37:
							if (hasWhiteCastled)
							{
								this.situation[i] = (byte)(p | 16);
							}
							whiteKingsField = i;
							whiteKingsCount++;
							whitePiecesCount++;
							break;
						case 45:
							if (hasBlackCastled)
							{
								this.situation[i] = (byte)(p | 16);
							}
							blackKingsField = i;
							blackKingsCount++;
							blackPiecesCount++;
							break;
						case 36:
							whiteQueensCount++;
							whitePiecesCount++;
							break;
						case 44:
							blackQueensCount++;
							blackPiecesCount++;
							break;
						case 35:
							whiteBishopsCount++;
							whitePiecesCount++;
							break;
						case 43:
							blackBishopsCount++;
							blackPiecesCount++;
							break;
						case 34:
							whiteKnightsCount++;
							whitePiecesCount++;
							break;
						case 42:
							blackKnightsCount++;
							blackPiecesCount++;
							break;
						case 33:
							whiteRooksCount++;
							whitePiecesCount++;
							break;
						case 41:
							blackRooksCount++;
							blackPiecesCount++;
							break;
						case 32:
							x = i % 8;
							if (x == 0)
							{
								this.situation[i] = 0;
								break;
							}
							else if (x == 2)
							{
								this.situation[i] = (byte)(p | 16);
							}
							else
							{
								this.situation[i] = (byte)(p & 47);
							}
							whitePawnsCount++;
							whitePiecesCount++;
							break;
						case 40:
							x = i % 8;
							if (x == 7)
							{
								this.situation[i] = 0;
								break;
							}
							else if (x == 5)
							{
								this.situation[i] = (byte)(p | 16);
							}
							else
							{
								this.situation[i] = (byte)(p & 47);
							}
							blackPawnsCount++;
							blackPiecesCount++;
							break;
						default:
							this.situation[i] = 0;
							break;
					}
				}
				CalculateLegalMoves();
			}
			this.history.SuppressDuplicates = false;
		}
		
		private void CalculateVerticalMovesForeward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 7)
				return;

			int		i		= field;
		    int		x		= 0;
			bool	stop	= false;
			int		color	= situation[field] & 8;
		    while (!stop)
		    {
		        i = i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical);
		    	x = situation[i];
		        if (x != 0 && (x & 8) == color)
		        {
		        	stop = true;
		        }
		        else
		        {
		        	if (x != 0 && (x & 8) != color)
		        	{
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.HittingPiece;
		        		m.PieceHit = (PieceType)(x & 7);
		        		legalMoves.Add(m);
		        		stop = true;
		             }
		             else
		             {
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.NormalMove;
		        		legalMoves.Add(m);
		             }
		        }
		        if ((i % 8) == 7)
		          stop = true;
		    }
		} // calculateVerticalMovesForeward

		private void CalculateVerticalMovesBackward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 0)
				return;

			int		i		= field;
		    int		x		= 0;
			bool	stop	= false;
			int		color	= situation[field] & 8;
		    while (!stop)
		    {
		        i = i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical);
		    	x = situation[i];
		        if (x != 0 && (x & 8) == color)
		        {
		        	stop = true;
		        }
		        else
		        {
		        	if (x != 0 && (x & 8) != color)
		        	{
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.HittingPiece;
		        		m.PieceHit = (PieceType)(x & 7);
		        		legalMoves.Add(m);
		        		stop = true;
		             }
		             else
		             {
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.NormalMove;
		        		legalMoves.Add(m);
		             }
		        }
		        if ((i % 8) == 0)
		          stop = true;
		    }
		} // getVerticalMovesBackward

		private void CalculateHorizontalMovesLeft(int field) // view of white
		{
			if (field < 0 || field > 63 || field < 8)
				return;

			int		i		= field;
		    int		x		= 0;
			bool	stop	= false;
			int		color	= situation[field] & 8;
		    while (!stop)
		    {
		        i = i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Horizontal);
		    	x = situation[i];
		        if (x != 0 && (x & 8) == color)
		        {
		        	stop = true;
		        }
		        else
		        {
		        	if (x != 0 && (x & 8) != color)
		        	{
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.HittingPiece;
		        		m.PieceHit = (PieceType)(x & 7);
		        		legalMoves.Add(m);
		        		stop = true;
		             }
		             else
		             {
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.NormalMove;
		        		legalMoves.Add(m);
		             }
		        }
		        if (i < 8)
		          stop = true;
		    }
		} // getHorizontalMovesLeft

		private void CalculateHorizontalMovesRight(int field) // view of white
		{
			if (field < 0 || field > 63 || field > 55)
				return;

			int		i		= field;
		    int		x		= 0;
			bool	stop	= false;
			int		color	= situation[field] & 8;
		    while (!stop)
		    {
		        i = i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Horizontal);
		    	x = situation[i];
		        if (x != 0 && (x & 8) == color)
		        {
		        	stop = true;
		        }
		        else
		        {
		        	if (x != 0 && (x & 8) != color)
		        	{
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.HittingPiece;
		        		m.PieceHit = (PieceType)(x & 7);
		        		legalMoves.Add(m);
		        		stop = true;
		             }
		             else
		             {
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.NormalMove;
		        		legalMoves.Add(m);
		             }
		        }
		        if (i > 55)
		          stop = true;
		    }
		} // getHorizontalMovesRight

		private void CalculateDiagonal1MovesForeward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 7 || field > 55)
				return;

			int		i		= field;
		    int		x		= 0;
			bool	stop	= false;
			int		color	= situation[field] & 8;
		    while (!stop)
		    {
		        i = i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal1);
		    	x = situation[i];
		        if (x != 0 && (x & 8) == color)
		        {
		        	stop = true;
		        }
		        else
		        {
		        	if (x != 0 && (x & 8) != color)
		        	{
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.HittingPiece;
		        		m.PieceHit = (PieceType)(x & 7);
		        		legalMoves.Add(m);
		        		stop = true;
		             }
		             else
		             {
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.NormalMove;
		        		legalMoves.Add(m);
		             }
		        }
		        if ((i % 8) == 7 || i > 55)
		          stop = true;
		    }
		} // getDiagonal1MovesForeward

		private void CalculateDiagonal1MovesBackward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 0 || field < 8)
				return;

			int		i		= field;
		    int		x		= 0;
			bool	stop	= false;
			int		color	= situation[field] & 8;
		    while (!stop)
		    {
		        i = i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal1);
		    	x = situation[i];
		        if (x != 0 && (x & 8) == color)
		        {
		        	stop = true;
		        }
		        else
		        {
		        	if (x != 0 && (x & 8) != color)
		        	{
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.HittingPiece;
		        		m.PieceHit = (PieceType)(x & 7);
		        		legalMoves.Add(m);
		        		stop = true;
		             }
		             else
		             {
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.NormalMove;
		        		legalMoves.Add(m);
		             }
		        }
		        if ((i % 8) == 0 || i < 8)
		          stop = true;
		    }
		} // getDiagonal1MovesBackward

		private void CalculateDiagonal2MovesForeward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 7 || field < 8)
				return;

			int		i		= field;
		    int		x		= 0;
			bool	stop	= false;
			int		color	= situation[field] & 8;
		    while (!stop)
		    {
		        i = i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal2);
		    	x = situation[i];
		        if (x != 0 && (x & 8) == color)
		        {
		        	stop = true;
		        }
		        else
		        {
		        	if (x != 0 && (x & 8) != color)
		        	{
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.HittingPiece;
		        		m.PieceHit = (PieceType)(x & 7);
		        		legalMoves.Add(m);
		        		stop = true;
		             }
		             else
		             {
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.NormalMove;
		        		legalMoves.Add(m);
		             }
		        }
		        if ((i % 8) == 7 || i < 8)
		          stop = true;
		    }
		} // getDiagonal2MovesForeward

		private void CalculateDiagonal2MovesBackward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 0 || field > 55)
				return;

			int		i		= field;
		    int		x		= 0;
			bool	stop	= false;
			int		color	= situation[field] & 8;
		    while (!stop)
		    {
		        i = i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal2);
		    	x = situation[i];
		        if (x != 0 && (x & 8) == color)
		        {
		        	stop = true;
		        }
		        else
		        {
		        	if (x != 0 && (x & 8) != color)
		        	{
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.HittingPiece;
		        		m.PieceHit = (PieceType)(x & 7);
		        		legalMoves.Add(m);
		        		stop = true;
		             }
		             else
		             {
		        		Move m = new Move(field,i);
		        		m.MoveCode = MoveCode.NormalMove;
		        		legalMoves.Add(m);
		             }
		        }
		        if ((i % 8) == 0 || i > 55)
		          stop = true;
		    }
		} // getDiagonal2MovesBackward

		private void CalculatePawnsMoves(int field) // view of white
		{
			if (field < 0 || field > 63)
				return;

			int	i		= field;
			int	color	= situation[field] & 8;

		    if (color == 0) // white
		    {
		        if ((situation[field] & 16) == 0 && (field % 8) == 1) // not moved yet
		        {
		            i = field+2*Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical);
		            if (situation[i] == 0 && situation[i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical)] == 0) // no pieces on the way
		            {
				    	Move m = new Move(field,i);
				        m.MoveCode = MoveCode.NormalMove;
				        legalMoves.Add(m);
		            }
		        }
		        if ((field % 8) != 7)
		        {
		            i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical);
		            if (situation[i] == 0) // no piece on the way
		            {
		            	if ((i % 8) == 7) // promotion
		            	{
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.PromotePawn;
				        	legalMoves.Add(m);
		            	}
		            	else
		            	{
					    	Move m = new Move(field,i);
					        m.MoveCode = MoveCode.NormalMove;
					        legalMoves.Add(m);
		            	}
		            }
		        }
		        if (field > 7 && (field % 8) != 7)
		        {
		        	i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal2);
		            if ((situation[i] & 8) != 0) // black fig. on field
		            {
		            	if ((i % 8) == 7)
		            	{
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.PromotePawnHittingPiece;
				        	m.PieceHit = (PieceType)(situation[i] & 7);
				        	legalMoves.Add(m);
		            	}
		            	else
		            	{
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.HittingPiece;
				        	m.PieceHit = (PieceType)(situation[i] & 7);
				        	legalMoves.Add(m);
		            	}
		            }
		        }
		        if (field < 56 && (field % 8) != 7)
		        {
		            i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal1);
		            if ((situation[i] & 8) != 0) // black fig. on field
		            {
		            	if ((i % 8) == 7)
		            	{
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.PromotePawnHittingPiece;
				        	m.PieceHit = (PieceType)(situation[i] & 7);
				        	legalMoves.Add(m);
		            	}
		                else
		                {
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.HittingPiece;
				        	m.PieceHit = (PieceType)(situation[i] & 7);
				        	legalMoves.Add(m);
		                }
		            }
		        }
		        // special case en passant:
		        if ((field % 8) == 4)
		        {
		        	if (field > 7)
		        	{
		        		i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal2);
		                if (situation[i] == 0 && situation[i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical)] == 56)
		                {
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.EnPassant;
				        	m.PieceHit = PieceType.Pawn;
				        	legalMoves.Add(m);
		                }
		        	}
		            if (field < 56)
		            {
		            	i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal1);
		                if (situation[i] == 0 && situation[i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical)] == 56)
		                {
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.EnPassant;
				        	m.PieceHit = PieceType.Pawn;
				        	legalMoves.Add(m);
		                }
		            }
		        }
		    }
		    else // black
		    {
		    	if ((situation[field] & 16) == 0 && (field % 8) == 6) // not moved yet
		    	{
		            i = field-2*Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical);
		            if (situation[i] == 0 && situation[i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical)] == 0) // no piece on the way
		            {
				    	Move m = new Move(field,i);
				        m.MoveCode = MoveCode.NormalMove;
				        legalMoves.Add(m);
		            }
		    	}
		        if ((field % 8) != 0)
		        {
		        	i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical);
		        	if (situation[i] == 0) // no piece on the way
		        	{
		        		if ((i % 8) == 0)
		        		{
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.PromotePawn;
				        	legalMoves.Add(m);
		        		}
		        		else
		        		{
				    		Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.NormalMove;
				        	legalMoves.Add(m);
		        		}
		        	}
		        }
		        if (field > 7 && (field % 8) != 0)
		        {
		            i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal1);
		        	if ((situation[i] & 8) == 0 && situation[i] != 0)
		        	{
		        		if ((i % 8) == 0)
		        		{
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.PromotePawnHittingPiece;
				        	m.PieceHit = (PieceType)(situation[i] & 7);
				        	legalMoves.Add(m);
		            	}
		            	else
		            	{
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.HittingPiece;
				        	m.PieceHit = (PieceType)(situation[i] & 7);
				        	legalMoves.Add(m);
		        		}
		        	}
		        }
		        if (field < 56 && (field % 8) != 0)
		        {
		        	i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal2);
		        	if ((situation[i] & 8) == 0 && situation[i] != 0)
		        	{
		        		if ((i % 8) == 0)
		        		{
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.PromotePawnHittingPiece;
				        	m.PieceHit = (PieceType)(situation[i] & 7);
				        	legalMoves.Add(m);
		        		}
		        		else
		        		{
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.HittingPiece;
				        	m.PieceHit = (PieceType)(situation[i] & 7);
				        	legalMoves.Add(m);
		        		}
		        	}
		        }
		        // special case en passant:
		        if ((field % 8) == 3)
		        {
		        	if (field > 7)
		        	{
		                i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal1);
		        		if (situation[i] == 0 && situation[i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical)] == 48)
		        		{
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.EnPassant;
				        	m.PieceHit = PieceType.Pawn;
				        	legalMoves.Add(m);
		        		}
		        	}
		            if (field < 56)
		            {
		            	i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal2);
		                if (situation[i] == 0 && situation[i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical)] == 48)
		                {
				        	Move m = new Move(field,i);
				        	m.MoveCode = MoveCode.EnPassant;
				        	m.PieceHit = PieceType.Pawn;
				        	legalMoves.Add(m);
		                }
		            }
		        }
		    }
		} // getPawnsMoves

		private void CalculateKnightsMoves(int field) // view of white
		{
			if (field < 0 || field > 63)
				return;

			int		i		= field;
			int		color	= situation[field] & 8;

			if ((field % 8) >= 2 && (field >> 3) >= 1)
			{
				i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight2);
				if ((situation[i] & 8) != color && situation[i] != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(situation[i] & 7);
					legalMoves.Add(m);
				}
				else if (situation[i] == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) <= 5 && (field >> 3) <= 6)
			{
				i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight2);
				if ((situation[i] & 8) != color && situation[i] != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(situation[i] & 7);
					legalMoves.Add(m);
				}
				else if (situation[i] == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) <= 5 && (field >> 3) >= 1)
			{
				i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight1);
				if ((situation[i] & 8) != color && situation[i] != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(situation[i] & 7);
					legalMoves.Add(m);
				}
				else if (situation[i] == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) >= 2 && (field >> 3) <= 6)
			{
				i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight1);
				if ((situation[i] & 8) != color && situation[i] != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(situation[i] & 7);
					legalMoves.Add(m);
				}
				else if (situation[i] == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) <= 6 && (field >> 3) >= 2)
			{
				i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight3);
				if ((situation[i] & 8) != color && situation[i] != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(situation[i] & 7);
					legalMoves.Add(m);
				}
				else if (situation[i] == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) >= 1 && (field >> 3) <= 5)
			{
				i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight3);
				if ((situation[i] & 8) != color && situation[i] != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(situation[i] & 7);
					legalMoves.Add(m);
				}
				else if (situation[i] == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) <= 6 && (field >> 3) <= 5)
			{
				i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight4);
				if ((situation[i] & 8) != color && situation[i] != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(situation[i] & 7);
					legalMoves.Add(m);
				}
				else if (situation[i] == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) >= 1 && (field >> 3) >= 2)
			{
				i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight4);
				if ((situation[i] & 8) != color && situation[i] != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(situation[i] & 7);
					legalMoves.Add(m);
				}
				else if (situation[i] == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
		} // getKnightsMoves

		private bool CheckedByVerticalForeward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 7)
				return false;

			bool		check	= false;
			int		i	= field;
			int		x	= 0;
			int		count	= 0;
			bool		stop	= false;
			int		color	= situation[field] & 8;
			while (!stop)
			{
				i = i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical);
				x = situation[i];
				count++;
				if ((x & 8) == color && x != 0)
					stop = true;
				else
				{
					if ((x & 8) != color && x != 0)
					{
						if (count == 1)
						{
							if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 1)
								check = true;
						}
						else
						{
							if ((x & 7) == 4 || (x & 7) == 1)
								check = true;
						}
						stop = true;
					}
				}
				if ((i % 8) == 7)
					stop = true;
			}
			return check;
		}

		private bool CheckedByVerticalBackward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 0)
				return false;

			bool		check	= false;
			int		i	= field;
			int		x	= 0;
			int		count	= 0;
			bool		stop	= false;
			int		color	= situation[field] & 8;
			while (!stop)
			{
				i = i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical);
				x = situation[i];
				count++;
				if ((x & 8) == color && x != 0)
					stop = true;
				else
				{
					if ((x & 8) != color && x != 0)
					{
						if (count == 1)
						{
							if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 1)
								check = true;
						}
						else
						{
							if ((x & 7) == 4 || (x & 7) == 1)
								check = true;
						}
						stop = true;
					}
				}
				if ((i % 8) == 0)
					stop = true;
			}
			return check;
		}

		private bool CheckedByHorizontalLeft(int field) // view of white
		{
			if (field < 0 || field > 63 || field < 8)
				return false;

			bool		check	= false;
			int		i	= field;
			int		x	= 0;
			int		count	= 0;
			bool		stop	= false;
			int		color	= situation[field] & 8;
			while (!stop)
			{
				i = i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Horizontal);
				x = situation[i];
				count++;
				if ((x & 8) == color && x != 0)
					stop = true;
				else
				{
					if ((x & 8) != color && x != 0)
					{
						if (count == 1)
						{
							if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 1)
								check = true;
						}
						else
						{
							if ((x & 7) == 4 || (x & 7) == 1)
								check = true;
						}
						stop = true;
					}
				}
				if (i < 8)
					stop = true;
			}
			return check;
		}

		private bool CheckedByHorizontalRight(int field) // view of white
		{
			if (field < 0 || field > 63 || field > 55)
				return false;

			bool		check	= false;
			int		i	= field;
			int		x	= 0;
			int		count	= 0;
			bool		stop	= false;
			int		color	= situation[field] & 8;
			while (!stop)
			{
				i = i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Horizontal);
				x = situation[i];
				count++;
				if ((x & 8) == color && x != 0)
					stop = true;
				else
				{
					if ((x & 8) != color && x != 0)
					{
						if (count == 1)
						{
							if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 1)
								check = true;
						}
						else
						{
							if ((x & 7) == 4 || (x & 7) == 1)
								check = true;
						}
						stop = true;
					}
				}
				if (i > 55)
					stop = true;
			}
			return check;
		}

		private bool CheckedByDiagonal1Foreward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 7 || field > 55)
				return false;

			bool		check	= false;
			int		i	= field;
			int		x	= 0;
			int		count	= 0;
			bool		stop	= false;
			int		color	= situation[field] & 8;
			while (!stop)
			{
				i = i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal1);
				x = situation[i];
				count++;
				if ((x & 8) == color && x != 0)
					stop = true;
				else
				{
					if ((x & 8) != color && x != 0)
					{
						if (count == 1)
						{
							if ((x & 8) == 8)
							{
								if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 3 || (x & 7) == 0)
									check = true;
							}
							else
							{
								if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 3)
									check = true;
							}
						}
						else
						{
							if ((x & 7) == 4 || (x & 7) == 3)
								check = true;
						}
						stop = true;
					}
				}
				if ((i % 8) == 7 || i > 55)
					stop = true;
			}
			return check;
		}

		private bool CheckedByDiagonal1Backward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 0 || field < 8)
				return false;

			bool		check	= false;
			int		i	= field;
			int		x	= 0;
			int		count	= 0;
			bool		stop	= false;
			int		color	= situation[field] & 8;
			while (!stop)
			{
				i = i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal1);
				x = situation[i];
				count++;
				if ((x & 8) == color && x != 0)
					stop = true;
				else
				{
					if ((x & 8) != color && x != 0)
					{
						if (count == 1)
						{
							if ((x & 8) == 0)
							{
								if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 3 || (x & 7) == 0)
									check = true;
							}
							else
							{
								if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 3)
									check = true;
							}
						}
						else
						{
							if ((x & 7) == 4 || (x & 7) == 3)
								check = true;
						}
						stop = true;
					}
				}
				if ((i % 8) == 0 || i < 8)
					stop = true;
			}
			return check;
		}

		private bool CheckedByDiagonal2Foreward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 7 || field < 8)
				return false;

			bool		check	= false;
			int		i	= field;
			int		x	= 0;
			int		count	= 0;
			bool		stop	= false;
			int		color	= situation[field] & 8;
			while (!stop)
			{
				i = i-Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal2);
				x = situation[i];
				count++;
				if ((x & 8) == color && x != 0)
					stop = true;
				else
				{
					if ((x & 8) != color && x != 0)
					{
						if (count == 1)
						{
							if ((x & 8) == 8)
							{
								if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 3 || (x & 7) == 0)
									check = true;
							}
							else
							{
								if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 3)
									check = true;
							}
						}
						else
						{
							if ((x & 7) == 4 || (x & 7) == 3)
								check = true;
						}
						stop = true;
					}
				}
				if ((i % 8) == 7 || i < 8)
					stop = true;
			}
			return check;
		}

		private bool CheckedByDiagonal2Backward(int field) // view of white
		{
			if (field < 0 || field > 63 || (field % 8) == 0 || field > 55)
				return false;

			bool		check	= false;
			int		i	= field;
			int		x	= 0;
			int		count	= 0;
			bool		stop	= false;
			int		color	= situation[field] & 8;
			while (!stop)
			{
				i = i+Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal2);
				x = situation[i];
				count++;
				if ((x & 8) == color && x != 0)
					stop = true;
				else
				{
					if ((x & 8) != color && x != 0)
					{
						if (count == 1)
						{
							if ((x & 8) == 0)
							{
								if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 3 || (x & 7) == 0)
									check = true;
							}
							else
							{
								if ((x & 7) == 5 || (x & 7) == 4 || (x & 7) == 3)
									check = true;
							}
						}
						else
						{
							if ((x & 7) == 4 || (x & 7) == 3)
								check = true;
						}
						stop = true;
					}
				}
				if ((i % 8) == 0 || i > 55)
					stop = true;
			}
			return check;
		}

		private bool CheckedByKnight(int field) // view of white
		{
			if (field < 0 || field > 63)
				return false;
			
			int	i	= 0;
			bool	check	= false;
			int	color	= situation[field] & 8;
			
			if (!check)
				if ((field % 8) >= 2 && (field >> 3) >= 1)
				{
					i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight2);
					if ((situation[i] & 8) != color && (situation[i] & 7) == 2)
						check = true;
				}
			if (!check)
				if ((field % 8) <= 5 && (field >> 3) <= 6)
				{
					i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight2);
					if ((situation[i] & 8) != color && (situation[i] & 7) == 2)
						check = true;
				}
			if (!check)
				if ((field % 8) <= 5 && (field >> 3) >= 1)
				{
					i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight1);
					if ((situation[i] & 8) != color && (situation[i] & 7) == 2)
						check = true;
				}
			if (!check)
				if ((field % 8) >= 2 && (field >> 3) <= 6)
				{
					i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight1);
					if ((situation[i] & 8) != color && (situation[i] & 7) == 2)
						check = true;
				}
			if (!check)
				if ((field % 8) <= 6 && (field >> 3) >= 2)
				{
					i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight3);
					if ((situation[i] & 8) != color && (situation[i] & 7) == 2)
						check = true;
				}
			if (!check)
				if ((field % 8) >= 1 && (field >> 3) <= 5)
				{
					i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight3);
					if ((situation[i] & 8) != color && (situation[i] & 7) == 2)
						check = true;
				}
			if (!check)
				if ((field % 8) <= 6 && (field >> 3) <= 5)
				{
					i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight4);
					if ((situation[i] & 8) != color && (situation[i] & 7) == 2)
						check = true;
				}
			if (!check)
				if ((field % 8) >= 1 && (field >> 3) >= 2)
				{
					i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Knight4);
					if ((situation[i] & 8) != color && (situation[i] & 7) == 2)
						check = true;
				}
			return check;
		}

		private bool IsFieldChecked(int field) // view of white
		{
			bool check = false;
			if (!check)
				check = CheckedByDiagonal1Foreward(field);
			if (!check)
				check = CheckedByDiagonal1Backward(field);
			if (!check)
				check = CheckedByDiagonal2Foreward(field);
			if (!check)
				check = CheckedByDiagonal2Backward(field);
			if (!check)
				check = CheckedByVerticalForeward(field);
			if (!check)
				check = CheckedByVerticalBackward(field);
			if (!check)
				check = CheckedByHorizontalLeft(field);
			if (!check)
				check = CheckedByHorizontalRight(field);
			if (!check)
				check = CheckedByKnight(field);
			return check;
		}

		private void CalculateKingsMoves(int field) // view of white
		{
			if (field < 0 || field > 63)
				return;

			int	i	= field;
			int	x	= 0;
			int	color	= situation[field] & 8;

			if ((field % 8) != 7)
			{
				i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical);
				x = situation[i];
				if ((x & 8) != color && x != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(x & 7);
					legalMoves.Add(m);
				}
				else if (x == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) != 0)
			{
				i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical);
				x = situation[i];
				if ((x & 8) != color && x != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(x & 7);
					legalMoves.Add(m);
				}
				else if (x == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if (field > 7)
			{
				i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Horizontal);
				x = situation[i];
				if ((x & 8) != color && x != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(x & 7);
					legalMoves.Add(m);
				}
				else if (x == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if (field < 56)
			{
				i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Horizontal);
				x = situation[i];
				if ((x & 8) != color && x != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(x & 7);
					legalMoves.Add(m);
				}
				else if (x == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) != 7 && field < 56)
			{
				i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal1);
				x = situation[i];
				if ((x & 8) != color && x != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(x & 7);
					legalMoves.Add(m);
				}
				else if (x == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) != 0 && field > 7)
			{
				i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal1);
				x = situation[i];
				if ((x & 8) != color && x != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(x & 7);
					legalMoves.Add(m);
				}
				else if (x == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) != 7 && field > 7)
			{
				i = field-Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal2);
				x = situation[i];
				if ((x & 8) != color && x != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(x & 7);
					legalMoves.Add(m);
				}
				else if (x == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			if ((field % 8) != 0 && field < 56)
			{
				i = field+Helper.MoveDirectionCode2Step(MoveDirectionCode.Diagonal2);
				x = situation[i];
				if ((x & 8) != color && x != 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.HittingPiece;
					m.PieceHit = (PieceType)(x & 7);
					legalMoves.Add(m);
				}
				else if (x == 0)
				{
					Move m = new Move(field,i);
					m.MoveCode = MoveCode.NormalMove;
					legalMoves.Add(m);
				}
			}
			// special case of castling
			if ((situation[field] & 16) == 0 && !IsFieldChecked(field))
			{
				bool stop = false;
				if (color == 0) // white
				{
					// big castling:
					stop = false;
					if (situation[0] == 33 && situation[8] == 0 && situation[16] == 0 && situation[24] == 0)
					{
						if (!stop)
						{
							situation[field] = 0;
							situation[24] = 37;
							if (IsFieldChecked(24))
								stop = true;
							situation[24] = 0;
							situation[field] = 37;
						}
						if (!stop)
						{
							situation[field] = 0;
							situation[16] = 37;
							if (IsFieldChecked(16))
								stop = true;
							situation[16] = 0;
							situation[field] = 37;
						}
						if (!stop)
						{
							situation[field] = 0;
							situation[8] = 37;
							if (IsFieldChecked(8))
								stop = true;
							situation[8] = 0;
							situation[field] = 37;
						}
						if (!stop)
						{
							Move m = new Move(field,8);
							m.MoveCode = MoveCode.BigCastling;
							legalMoves.Add(m);
						}
					}
					// small castling
					stop = false;
					if (situation[56] == 33 && situation[48] == 0 && situation[40] == 0)
					{
						if (!stop)
						{
							situation[field] = 0;
							situation[40] = 37;
							if (IsFieldChecked(40))
								stop = true;
							situation[40] = 0;
							situation[field] = 37;
						}
						if (!stop)
						{
							situation[field] = 0;
							situation[48] = 37;
							if (IsFieldChecked(48))
								stop = true;
							situation[48] = 0;
							situation[field] = 37;
						}
						if (!stop)
						{
							Move m = new Move(field,48);
							m.MoveCode = MoveCode.SmallCastling;
							legalMoves.Add(m);
						}
					}
				}
				if (color == 8) // black
				{
					// big castling
					stop = false;
					if (situation[7] == 41 && situation[15] == 0 && situation[23] == 0 && situation[31] == 0)
					{
						if (!stop)
						{
							situation[field] = 0;
							situation[31] = 45;
							if (IsFieldChecked(31))
								stop = true;
							situation[31] = 0;
							situation[field] = 45;
						}
						if (!stop)
						{
							situation[field] = 0;
							situation[23] = 45;
							if (IsFieldChecked(23))
								stop = true;
							situation[23] = 0;
							situation[field] = 45;
						}
						if (!stop)
						{
							situation[field] = 0;
							situation[15] = 45;
							if (IsFieldChecked(15))
								stop = true;
							situation[15] = 0;
							situation[field] = 45;
						}
						if (!stop)
						{
							Move m = new Move(field,15);
							m.MoveCode = MoveCode.BigCastling;
							legalMoves.Add(m);
						}
					}
					// small castling
					stop = false;
					if (situation[63] == 41 && situation[55] == 0 && situation[47] == 0)
					{
						if (!stop)
						{
							situation[field] = 0;
							situation[47] = 45;
							if (IsFieldChecked(47))
								stop = true;
							situation[47] = 0;
							situation[field] = 45;
						}
						if (!stop)
						{
							situation[field] = 0;
							situation[55] = 45;
							if (IsFieldChecked(55))
								stop = true;
							situation[55] = 0;
							situation[field] = 45;
						}
						if (!stop)
						{
							Move m = new Move(field,55);
							m.MoveCode = MoveCode.SmallCastling;
							legalMoves.Add(m);
						}
					}
				}
			}
		} // getKingsMoves

		private void CalculateLegalMoves()
		{
			legalMoves.Clear();

			int color = 0; // white
			if (whoToMove == Color.Black)
				color = 8;

			for (int i = 0; i < 64; i++)
			{
				int x = situation[i] & 7;
				if (situation[i] == 0 || !(x == 5 || x == 4 || x == 3 || x == 2 || x == 1 || x == 0))
				{
					situation[i] = 0;
					continue;
				}
				if ((situation[i] & 8) == color)
				{
					switch (x)
					{
						case 0: // Pawn
							CalculatePawnsMoves(i);
							break;
						case 1: // Rook
							CalculateVerticalMovesForeward(i);
							CalculateVerticalMovesBackward(i);
							CalculateHorizontalMovesLeft(i);
							CalculateHorizontalMovesRight(i);
							break;
						case 2: // Knight
							CalculateKnightsMoves(i);
							break;
						case 3: // Bishop
							CalculateDiagonal1MovesForeward(i);
							CalculateDiagonal1MovesBackward(i);
							CalculateDiagonal2MovesForeward(i);
							CalculateDiagonal2MovesBackward(i);
							break;
						case 4: // Queen
							CalculateDiagonal1MovesForeward(i);
							CalculateDiagonal1MovesBackward(i);
							CalculateDiagonal2MovesForeward(i);
							CalculateDiagonal2MovesBackward(i);
							CalculateVerticalMovesForeward(i);
							CalculateVerticalMovesBackward(i);
							CalculateHorizontalMovesLeft(i);
							CalculateHorizontalMovesRight(i);
							break;
						default: // King
							CalculateKingsMoves(i);
							break;
					}
				}
			}

			for (int i = 0; i < legalMoves.Count; i++)
			{
				int checkIndex = -1;
				Move m = legalMoves[i];
				m.OwnSituationCode = SituationCode.Normal;
				m.OpponentSituationCode = SituationCode.Normal;
				this.PerformMove(m,true);
				if (m.OwnSituationCode == SituationCode.Check)
					checkIndex = i;
				this.Rollback(1);
				if (checkIndex != -1)
				{
					legalMoves.RemoveAt(i);
					i--;
					continue;
				}
				m.PieceType = (PieceType)(situation[m.FieldNumberFrom] & 7);
			}
		} // calculateLegalMoves

		/// <summary>
		/// Returns a board rendering string in legacy chess-font format.
		/// </summary>
		/// <returns>Chess-font formatted board text.</returns>
		public string GetChessFontString()
        {
            //@"!""""""""""""""""#" + "\r\n" +
            //    @"�tMvWlVmT%" + "\r\n" +
            //    @"�OoOoOoOo%" + "\r\n" +
            //    @"� + + + +%" + "\r\n" +
            //    @"�+ + + + %" + "\r\n" +
            //    @"� + + + +%" + "\r\n" +
            //    @"�+ + + + %" + "\r\n" +
            //    @"�pPpPpPpP%" + "\r\n" +
            //    @"�RnBqKbNr%" + "\r\n" +
            //    @"/��������)";
            return "";
        }
		
		/// <summary>
		/// Returns a detailed ASCII board view including side to move, last move, and legal SAN moves.
		/// </summary>
		/// <returns>Formatted multi-line board representation.</returns>
		public override string ToString()
		{
			string temp = "";
			temp = "     A    B    C    D    E    F    G    H  "+Environment.NewLine;
			temp += "    _______________________________________ "+Environment.NewLine;
			for (int i = 8; i > 0; i--)
			{
				temp += "   |    |    |    |    |    |    |    |    |"+Environment.NewLine+" "+i.ToString()+" |";
				for (int j = 0; j < 8; j++)
				{
					int k = j*8+i;
					if (situation[k-1] != 0)
					{
						if ((situation[k-1] & 8) != 0)
							temp += " b";
						else
							temp += " w";
						switch (situation[k-1] & 7)
						{
							case 0:
								temp += Helper.PieceType2ShortName(PieceType.Pawn)+" |";
								break;
							case 1:
								temp += Helper.PieceType2ShortName(PieceType.Rook)+" |";
								break;
							case 2:
								temp += Helper.PieceType2ShortName(PieceType.Knight)+" |";
								break;
							case 3:
								temp += Helper.PieceType2ShortName(PieceType.Bishop)+" |";
								break;
							case 4:
								temp += Helper.PieceType2ShortName(PieceType.Queen)+" |";
								break;
							case 5:
								temp += Helper.PieceType2ShortName(PieceType.King)+" |";
								break;
							default:
								temp += "? |";
								break;
						}
					}
					else
						temp += "    |";
				}
				temp += " "+i.ToString();
				temp += Environment.NewLine+"   |____|____|____|____|____|____|____|____|"+Environment.NewLine;
			}
			temp += Environment.NewLine+"     A    B    C    D    E    F    G    H  "+Environment.NewLine;
			if (history != null && history.Count > 0)
			{
				Move lastMove = history[history.Count-1];
				string lastMoveSan = lastMove.SnapShot.MoveToSan(lastMove);
				temp += Environment.NewLine+"Last move done: "+lastMoveSan+Environment.NewLine;
			}
			temp += Environment.NewLine+(whoToMove == Color.White ? "White" : "Black")+" to move now..."+Environment.NewLine;
			temp += Environment.NewLine+"Legal moves:"+Environment.NewLine+legalMoves.ToSanString(this.Clone());

			return temp;
		}

		private static char PieceToAsciiChar(byte piece)
		{
			if (piece == 0)
				return '.';

			bool isBlack = (piece & 8) != 0;
			char c;
			switch ((PieceType)(piece & 7))
			{
				case PieceType.Pawn:
					c = 'P';
					break;
				case PieceType.Rook:
					c = 'R';
					break;
				case PieceType.Knight:
					c = 'N';
					break;
				case PieceType.Bishop:
					c = 'B';
					break;
				case PieceType.Queen:
					c = 'Q';
					break;
				case PieceType.King:
					c = 'K';
					break;
				default:
					c = '?';
					break;
			}
			return isBlack ? Char.ToLower(c) : c;
		}

		private static char PieceToUnicodeChar(byte piece)
		{
			if (piece == 0)
				return '·';

			bool isBlack = (piece & 8) != 0;
			switch ((PieceType)(piece & 7))
			{
				case PieceType.Pawn:
					return isBlack ? '♙' : '♟';
				case PieceType.Rook:
					return isBlack ? '♖' : '♜';
				case PieceType.Knight:
					return isBlack ? '♘' : '♞';
				case PieceType.Bishop:
					return isBlack ? '♗' : '♝';
				case PieceType.Queen:
					return isBlack ? '♕' : '♛';
				case PieceType.King:
					return isBlack ? '♔' : '♚';
				default:
					return '?';
			}
		}

		/// <summary>
		/// Returns a user-friendly board representation with optional Unicode symbols and context details.
		/// </summary>
		/// <param name="useUnicode">If true, uses Unicode chess symbols with checkerboard field contrast; otherwise uses ASCII piece letters.</param>
		/// <param name="includeLegalMoves">If true, appends legal SAN moves to the output.</param>
		/// <param name="includeHistory">If true, appends the recent move history.</param>
		/// <param name="historySteps">Maximum number of history entries to include when <paramref name="includeHistory"/> is true.</param>
		/// <param name="whiteAtBottom">If true, renders from White's perspective; otherwise from Black's perspective.</param>
		/// <param name="includeSnapshotHeader">If true, includes the snapshot title block.</param>
		/// <param name="includeStatusLine">If true, includes the side-to-move and situation status line.</param>
		/// <param name="includeLastMoveLine">If true, includes a single last-move line when history exists.</param>
		/// <returns>A formatted board and game-state string for user-facing display.</returns>
		public string ToUserFriendlyString(bool useUnicode = true, bool includeLegalMoves = true, bool includeHistory = false, int historySteps = 8, bool whiteAtBottom = true, bool includeSnapshotHeader = true, bool includeStatusLine = true, bool includeLastMoveLine = true)
		{
			StringBuilder sb = new StringBuilder();
			if (includeSnapshotHeader)
			{
				sb.AppendLine("tSHess Snapshot");
				sb.AppendLine("-------------");
			}

			if (includeStatusLine)
				sb.AppendLine("To move: " + (whoToMove == Color.White ? "White" : "Black") + " | Status: " + currentSituationCode.ToString());

			if (includeLastMoveLine && history != null && history.Count > 0)
			{
				try
				{
					Move lastMove = history[history.Count-1];
					string san = lastMove.SnapShot.MoveToSan(lastMove);
					sb.AppendLine("Last move: " + san);
				}
				catch
				{
					sb.AppendLine("Last move: " + history[history.Count-1].ToString());
				}
			}

			sb.AppendLine();

			int rankStart = whiteAtBottom ? 7 : 0;
			int rankEnd = whiteAtBottom ? -1 : 8;
			int rankStep = whiteAtBottom ? -1 : 1;
			int fileStart = whiteAtBottom ? 0 : 7;
			int fileEnd = whiteAtBottom ? 8 : -1;
			int fileStep = whiteAtBottom ? 1 : -1;

			if (useUnicode)
			{
				// Full-width Unicode mode: every frame character occupies exactly 2 terminal
				// columns, matching the width of chess piece glyphs, so the grid stays aligned.
				// Frame chars: ｜(U+FF5C) ＋(U+FF0B) －(U+FF0D) 　(U+3000 ideographic space)
				// File letters: Ａ-Ｈ (U+FF21-U+FF28), Rank digits: １-８ (U+FF11-U+FF18)
				// Empty squares use guaranteed full-width CJK chars by parity
				// (light: ・ U+30FB Katakana Middle Dot, dark: 　 U+3000 Ideographic Space).
				const string fwSep = "　　＋－＋－＋－＋－＋－＋－＋－＋－＋";
				sb.Append("　　　");
				for (int file = fileStart; file != fileEnd; file += fileStep)
					sb.Append((char)(0xFF21 + file)).Append('　');
				sb.AppendLine();
				sb.AppendLine(fwSep);

				for (int rank = rankStart; rank != rankEnd; rank += rankStep)
				{
					sb.Append((char)(0xFF10 + rank + 1)).Append('　');
					for (int file = fileStart; file != fileEnd; file += fileStep)
					{
						int field = file * 8 + rank;
						byte sq = situation[field];
						bool isDarkSquare = ((file + rank) & 1) == 0;
						char piece = sq == 0 ? (isDarkSquare ? '　' : '・') : PieceToUnicodeChar(sq);
						sb.Append('｜').Append(piece);
					}
					sb.Append('｜').Append('　').Append((char)(0xFF10 + rank + 1)).AppendLine();
					sb.AppendLine(fwSep);
				}

				sb.Append("　　　");
				for (int file = fileStart; file != fileEnd; file += fileStep)
					sb.Append((char)(0xFF21 + file)).Append('　');
				sb.AppendLine();
			}
			else
			{
				string sep = "  +---+---+---+---+---+---+---+---+";
				sb.Append("    ");
				for (int file = fileStart; file != fileEnd; file += fileStep)
					sb.Append((char)('A' + file) + "   ");
				sb.AppendLine();
				sb.AppendLine(sep);

				for (int rank = rankStart; rank != rankEnd; rank += rankStep)
				{
					sb.Append((rank + 1).ToString() + " ");
					for (int file = fileStart; file != fileEnd; file += fileStep)
					{
						int field = file * 8 + rank;
						char piece = PieceToAsciiChar(situation[field]);
						sb.Append("| ");
						sb.Append(piece);
						sb.Append(" ");
					}
					sb.AppendLine("| " + (rank + 1).ToString());
					sb.AppendLine(sep);
				}

				sb.Append("    ");
				for (int file = fileStart; file != fileEnd; file += fileStep)
					sb.Append((char)('A' + file) + "   ");
				sb.AppendLine();
			}

			if (includeLegalMoves)
			{
				sb.AppendLine();
				sb.AppendLine("Legal moves:");
				sb.AppendLine(legalMoves.ToSanString(this.Clone()));
			}

			if (includeHistory && history != null && history.Count > 0)
			{
				if (historySteps < 1)
					historySteps = 1;
				historySteps = (historySteps > history.Count ? history.Count : historySteps);
				sb.AppendLine();
				sb.AppendLine("History (last " + historySteps.ToString() + " of " + history.Count.ToString() + " plies):");
				sb.AppendLine(GetHistorySanString(true, historySteps));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns a compact user-facing board representation without snapshot header/status metadata.
		/// </summary>
		/// <param name="useUnicode">If true, uses Unicode rendering.</param>
		/// <param name="includeLegalMoves">If true, appends legal SAN moves.</param>
		/// <param name="includeHistory">If true, appends recent move history.</param>
		/// <param name="historySteps">Maximum number of history plies to include.</param>
		/// <param name="whiteAtBottom">If true, renders from White's perspective.</param>
		/// <returns>Compact game-output board text.</returns>
		public string ToGameOutputString(bool useUnicode = true, bool includeLegalMoves = true, bool includeHistory = false, int historySteps = 8, bool whiteAtBottom = true)
		{
			return ToUserFriendlyString(useUnicode, includeLegalMoves, includeHistory, historySteps, whiteAtBottom, false, false, false);
		}

		/// <summary>
		/// Returns SAN-formatted move history.
		/// </summary>
		/// <param name="includeMoveNumbers">If true, includes move numbers in output.</param>
		/// <param name="maxHalfMoves">Maximum plies to include; negative includes full history.</param>
		/// <returns>SAN move sequence string, or empty string when no history exists.</returns>
		public string GetHistorySanString(bool includeMoveNumbers = true, int maxHalfMoves = -1)
		{
			if (history == null || history.Count == 0)
				return "";

			int startIndex = 0;
			if (maxHalfMoves > 0 && maxHalfMoves < history.Count)
				startIndex = history.Count - maxHalfMoves;

			StringBuilder sb = new StringBuilder();
			for (int i = startIndex; i < history.Count; i++)
			{
				Move move = history[i];
				string san;
				try
				{
					san = move.SnapShot.MoveToSan(move);
				}
				catch
				{
					san = move.ToString();
				}

				if (!includeMoveNumbers)
				{
					if (sb.Length > 0)
						sb.Append(' ');
					sb.Append(san);
					continue;
				}

				int halfMoveFromStart = i - startIndex;
				int moveNumber = (i / 2) + 1;
				if ((halfMoveFromStart % 2) == 0)
				{
					if (sb.Length > 0)
						sb.Append(' ');
					sb.Append(moveNumber.ToString()).Append(". ").Append(san);
				}
				else
				{
					sb.Append(' ').Append(san);
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Returns the standard board representation with appended trailing history entries.
		/// </summary>
		/// <param name="lastHistoryStepsIncluded">Number of latest history entries to append.</param>
		/// <returns>Board text with optional history appendix.</returns>
		public string ToString(int lastHistoryStepsIncluded)
		{
			string retVal = this.ToString();
			if (history != null && lastHistoryStepsIncluded > 0)
			{
				lastHistoryStepsIncluded = (lastHistoryStepsIncluded > history.Count ? history.Count : lastHistoryStepsIncluded);
				retVal += Environment.NewLine+Environment.NewLine+"History (last "+lastHistoryStepsIncluded.ToString()+" of "+history.Count.ToString()+" steps):"+Environment.NewLine;
				for (int i = history.Count-lastHistoryStepsIncluded; i < history.Count; i++)
					retVal += history[i].ToString(true)+(i+1 == history.Count ? "" : Environment.NewLine);
			}
			return retVal;
		}

		private bool HitPossible
		{
			get
			{
				for (int i = 0; i < legalMoves.Count; i++)
					if (legalMoves[i].PieceHit != PieceType.None)
						return true;
				return false;
			}
		}

		/// <summary>
		/// Gets full coordinate-notation history as multi-line text.
		/// </summary>
		public string History
		{
			get
			{
				string retVal = "";
				if (history != null)
					for (int i = 0; i < history.Count; i++)
						retVal += history[i].ToString(true)+Environment.NewLine;
				return retVal;
			}
		}
		
		/// <summary>
		/// Creates a deep clone of this snapshot, including board state, counters, legal moves, and move history.
		/// </summary>
		/// <returns>Independent snapshot copy.</returns>
		public SnapShot Clone()
		{
			SnapShot s = new SnapShot((byte[])this.situation.Clone(),this.whoToMove,false,false,true);
			s.currentSituationCode = this.currentSituationCode;
			s.whitePiecesCount = this.whitePiecesCount;
			s.blackPiecesCount = this.blackPiecesCount;
			s.whitePawnsCount = this.whitePawnsCount;
			s.blackPawnsCount = this.blackPawnsCount;
			s.whiteRooksCount = this.whiteRooksCount;
			s.blackRooksCount = this.blackRooksCount;
			s.whiteKnightsCount = this.whiteKnightsCount;
			s.blackKnightsCount = this.blackKnightsCount;
			s.whiteBishopsCount = this.whiteBishopsCount;
			s.blackBishopsCount = this.blackBishopsCount;
			s.whiteQueensCount = this.whiteQueensCount;
			s.blackQueensCount = this.blackQueensCount;
			s.whiteKingsCount = this.whiteKingsCount;
			s.blackKingsCount = this.blackKingsCount;
			s.whiteKingsField = this.whiteKingsField;
			s.blackKingsField = this.blackKingsField;
			s.hasWhiteCastled = this.hasWhiteCastled;
			s.hasBlackCastled = this.hasBlackCastled;
			s.legalMoves = this.legalMoves.Clone();
			if (this.history == null)
				s.history = null;
			else
			{
				for (int i = 0; i < this.history.Count; i++)
				{
					Move m = this.history[i];
					SnapShot backup = m.SnapShot;
					m.SnapShot = null;
					s.history.Add(m.Clone());
					m.SnapShot = backup;
				}
			}
			return s;
		}

		private void Incorporate(SnapShot snapShot)
		{
			if (snapShot == null)
				throw new ArgumentException("Invalid","snapShot");
			this.situation = snapShot.situation;
			this.whoToMove = snapShot.whoToMove;
			this.currentSituationCode = snapShot.currentSituationCode;
			this.whitePiecesCount = snapShot.whitePiecesCount;
			this.blackPiecesCount = snapShot.blackPiecesCount;
			this.whitePawnsCount = snapShot.whitePawnsCount;
			this.blackPawnsCount = snapShot.blackPawnsCount;
			this.whiteRooksCount = snapShot.whiteRooksCount;
			this.blackRooksCount = snapShot.blackRooksCount;
			this.whiteKnightsCount = snapShot.whiteKnightsCount;
			this.blackKnightsCount = snapShot.blackKnightsCount;
			this.whiteBishopsCount = snapShot.whiteBishopsCount;
			this.blackBishopsCount = snapShot.blackBishopsCount;
			this.whiteQueensCount = snapShot.whiteQueensCount;
			this.blackQueensCount = snapShot.blackQueensCount;
			this.whiteKingsCount = snapShot.whiteKingsCount;
			this.blackKingsCount = snapShot.blackKingsCount;
			this.whiteKingsField = snapShot.whiteKingsField;
			this.blackKingsField = snapShot.blackKingsField;
			this.hasWhiteCastled = snapShot.hasWhiteCastled;
			this.hasBlackCastled = snapShot.hasBlackCastled;
			this.legalMoves = snapShot.legalMoves;
			this.history = snapShot.history;
		}

		/// <summary>
		/// Reverts the current snapshot by a number of half-moves using recorded history.
		/// </summary>
		/// <param name="countMoves">Number of moves to revert; values less than one are ignored.</param>
		public void Rollback(int countMoves)
		{
			if (countMoves < 1)
				return;
			MoveList historyBackup = this.history;
			if (countMoves > historyBackup.Count)
				countMoves = historyBackup.Count;
			this.Incorporate(historyBackup[historyBackup.Count-countMoves].SnapShot);
			for (int i = 0; i < countMoves; i++)
				historyBackup.RemoveAt(historyBackup.Count-1);
			this.history = historyBackup;
		}
		
		/// <summary>
		/// Applies a legal move to the current snapshot and updates derived state.
		/// </summary>
		/// <param name="move">The move to apply; it must exist in the current legal move list.</param>
		/// <exception cref="ArgumentException">Thrown when <paramref name="move"/> is null or not legal in the current position.</exception>
		public void PerformMove(Move move)
		{
			PerformMove(move,false);
		}

		private void PerformMove(Move move, bool internalUsage)
		{
			if (move == null)
				throw new ArgumentException("Invalid","move");
			Move origMove = legalMoves[move.FieldNumberFrom,move.FieldNumberTo];
			if (origMove == null)
				throw new ArgumentException("Invalid","move");
			MoveList historyBackup = this.history;
			this.history = null;
			SnapShot snapShotBackup = this.Clone();
			this.history = historyBackup;

			int	fFrom	= origMove.FieldNumberFrom;
			int	fTo	= origMove.FieldNumberTo;
			int	color	= situation[fFrom] & 8;

			switch (origMove.MoveCode)
			{
				case MoveCode.SmallCastling:
					if (color == 0)
					{
						situation[fFrom] = 0;
						situation[fTo] = 53;
						situation[40] = situation[56];
						situation[56] = 0;
						hasWhiteCastled = true;
					}
					else
					{
						situation[fFrom] = 0;
						situation[fTo] = 61;
						situation[47] = situation[63];
						situation[63] = 0;
						hasBlackCastled = true;
					}
					break;
				case MoveCode.BigCastling:
					if (color == 0)
					{
						situation[fFrom] = 0;
						situation[fTo] = 53;
						situation[16] = situation[0];
						situation[0] = 0;
						hasWhiteCastled = true;
					}
					else
					{
						situation[fFrom] = 0;
						situation[fTo] = 61;
						situation[23] = situation[7];
						situation[7] = 0;
						hasBlackCastled = true;
					}
					break;
				case MoveCode.EnPassant:
					if (color == 0)
					{
						situation[fTo-Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical)] = 0;
						situation[fTo] = situation[fFrom];
						situation[fFrom] = 0;
						blackPawnsCount--;
					}
					else
					{
						situation[fTo+Helper.MoveDirectionCode2Step(MoveDirectionCode.Vertical)] = 0;
						situation[fTo] = situation[fFrom];
						situation[fFrom] = 0;
						whitePawnsCount--;
					}
					break;
				case MoveCode.PromotePawn:
					{
						PromotePawnEventArgs ea = new PromotePawnEventArgs();
						// If promotion piece was specified in the move (e.g., from SAN notation), use it
						// Otherwise, let the event handler decide (for interactive UI)
						if (origMove.PromotionPieceType.HasValue)
						{
							ea.PromoteTo = origMove.PromotionPieceType.Value;
						}
						else if (!internalUsage && PromotePawnEventHandler != null)
						{
							PromotePawnEventHandler(this,ea);
						}
						byte newPiece = (byte)((PieceType)ea.PromoteTo);
						newPiece = (byte)(newPiece | color | 16 | 32);
						situation[fTo] = newPiece;
						situation[fFrom] = 0;
						if (color == 0)
						{
							whitePawnsCount--;
							whiteQueensCount++;
						}
						else
						{
							blackPawnsCount--;
							blackQueensCount++;
						}
					}
					break;
				case MoveCode.PromotePawnHittingPiece:
					{
						PromotePawnEventArgs ea = new PromotePawnEventArgs();
						// If promotion piece was specified in the move (e.g., from SAN notation), use it
						// Otherwise, let the event handler decide (for interactive UI)
						if (origMove.PromotionPieceType.HasValue)
						{
							ea.PromoteTo = origMove.PromotionPieceType.Value;
						}
						else if (!internalUsage && PromotePawnEventHandler != null)
						{
							PromotePawnEventHandler(this,ea);
						}
						byte newPiece = (byte)((PieceType)ea.PromoteTo);
						newPiece = (byte)(newPiece| color | 16 | 32);
						situation[fTo] = newPiece;
						situation[fFrom] = 0;
						if (color == 0)
						{
							whitePawnsCount--;
							whiteQueensCount++;
						}
						else
						{
							blackPawnsCount--;
							blackQueensCount++;
						}
					}
					break;
				case MoveCode.HittingPiece:
				default:
					if ((situation[fFrom] & 7) == 0) // pawn
					{
						if (color == 0) // white
						{
							if ((situation[fFrom] & 16) != 0) // 2nd move
								situation[fTo] = 32;
							else
							{
								if ((fFrom % 8) == 1) // 1st move
									situation[fTo] = 48;
								else
									situation[fTo] = 32; // 3rd or higher move
							}
						}
						else // black
						{
							if ((situation[fFrom] & 16) != 0) // 2nd move
								situation[fTo] = 40;
							else
							{
								if ((fFrom % 8) == 6) // 1st move
									situation[fTo] = 56;
								else
									situation[fTo] = 40; // 3rd or higher move
							}
						}
					}
					else // any other piece
					{
						if ((situation[fFrom] & 16) != 0)
							situation[fTo] = situation[fFrom]; // 2nd or higher move
						else
							situation[fTo] = (byte)(situation[fFrom]+16); // 1st move
					}
					situation[fFrom] = 0;
					break;
			}

			if (origMove.MoveCode == MoveCode.HittingPiece || origMove.MoveCode == MoveCode.PromotePawnHittingPiece)
			{
				if (color == 0)
				{
					switch (origMove.PieceHit)
					{
						case PieceType.Pawn:
							blackPawnsCount--;
							break;
						case PieceType.Rook:
							blackRooksCount--;
							break;
						case PieceType.Knight:
							blackKnightsCount--;
							break;
						case PieceType.Bishop:
							blackBishopsCount--;
							break;
						case PieceType.Queen:
							blackQueensCount--;
							break;
						case PieceType.King:
							blackKingsCount--;
							break;
						default:
							break;
					}
					blackPiecesCount--;
				}
				else
				{
					switch (origMove.PieceHit)
					{
						case PieceType.Pawn:
							whitePawnsCount--;
							break;
						case PieceType.Rook:
							whiteRooksCount--;
							break;
						case PieceType.Knight:
							whiteKnightsCount--;
							break;
						case PieceType.Bishop:
							whiteBishopsCount--;
							break;
						case PieceType.Queen:
							whiteQueensCount--;
							break;
						case PieceType.King:
							whiteKingsCount--;
							break;
						default:
							break;
					}
					whitePiecesCount--;
				}
			}

			bool raiseCheckEvent = false;
			whiteKingsField = -1;
			blackKingsField = -1;
			for (int i = 0; i < 64; i++)
			{
				int x = situation[i] & 7;
				int xColor = situation[i] & 8;
				if (x == 5)
				{
					if (xColor == 0)
					{
						whiteKingsField = i;
						bool isChecked = IsFieldChecked(whiteKingsField);
						if (isChecked)
						{
							if (xColor != color)
								raiseCheckEvent = true;
						}
						if (internalUsage)
						{
							if (color == 0)
							{
								if (isChecked)
									origMove.OwnSituationCode = SituationCode.Check;
								else
									origMove.OwnSituationCode = SituationCode.Normal;
							}
							else
							{
								if (isChecked)
									origMove.OpponentSituationCode = SituationCode.Check;
								else
									origMove.OpponentSituationCode = SituationCode.Normal;
							}
						}
					}
					else
					{
						blackKingsField = i;
						bool isChecked = IsFieldChecked(blackKingsField);
						if (isChecked)
						{
							if (xColor != color)
								raiseCheckEvent = true;
						}
						if (internalUsage)
						{
							if (color == 0)
							{
								if (isChecked)
									origMove.OpponentSituationCode = SituationCode.Check;
								else
									origMove.OpponentSituationCode = SituationCode.Normal;
							}
							else
							{
								if (isChecked)
									origMove.OwnSituationCode = SituationCode.Check;
								else
									origMove.OwnSituationCode = SituationCode.Normal;
							}
						}
					}
				}
				if (whiteKingsField != -1 && blackKingsField != -1)
					break;
			}
			if (whiteKingsField == -1 || blackKingsField == -1)
				throw new Exception("Snapshot is invalid: Not both kings found!");

			this.whoToMove = Helper.OpponentColor(color == 0 ? Color.White : Color.Black);

			Move cloneMove = origMove.Clone();
			cloneMove.SnapShot = snapShotBackup;
			this.history.Add(cloneMove);

			if (!internalUsage)
			{
				CalculateLegalMoves();
				if (origMove.PieceHit != PieceType.None && PieceHitEventHandler != null)
					PieceHitEventHandler(this,new PieceHitEventArgs(origMove.PieceHit));
				if (legalMoves.Count > 0)
				{
					if (raiseCheckEvent && CheckEventHandler != null)
						CheckEventHandler(this,new CheckEventArgs());
				}
				else
				{
					if (origMove.OpponentSituationCode == SituationCode.Check)
						currentSituationCode = SituationCode.Checkmate;
					else
						currentSituationCode = SituationCode.Stalemate;
					if (GameOverEventHandler != null)
						GameOverEventHandler(this,new GameOverEventArgs());
				}
			}
		} // PerformMove

		/// <summary>
		/// Converts a legal move in the current position into Standard Algebraic Notation (SAN).
		/// </summary>
		/// <param name="move">The move to convert.</param>
		/// <returns>The SAN representation for the move in the current board context.</returns>
		/// <exception cref="ArgumentException">Thrown when <paramref name="move"/> is null.</exception>
		public string MoveToSan(Move move)
		{
			if (move == null)
				throw new ArgumentException("Move cannot be null");

			// Handle castling
			if (move.MoveCode == MoveCode.SmallCastling)
				return "O-O";
			if (move.MoveCode == MoveCode.BigCastling)
				return "O-O-O";

			int fromField = move.FieldNumberFrom;
			int toField = move.FieldNumberTo;
			PieceType pieceType = (PieceType)(situation[fromField] & 7);

			string result = "";

			// Add piece notation (except for pawns)
			if (pieceType != PieceType.Pawn)
			{
				result += Helper.PieceType2ShortName(pieceType);

				// Check if disambiguation is needed
				int matchCount = 0;

				foreach (Move m in legalMoves)
				{
					if (m.FieldNumberTo != toField)
						continue;
					if ((situation[m.FieldNumberFrom] & 7) != (int)pieceType)
						continue;
					matchCount++;
				}

				if (matchCount > 1)
				{
					HorizontalCoordinateCode fromFile = Helper.FieldNumber2HorizontalCoordinateCode(fromField);
					VerticalCoordinateCode fromRank = Helper.FieldNumber2VerticalCoordinateCode(fromField);

					// Check if different files can make the move
					int sameFileMoves = 0;
					foreach (Move m in legalMoves)
					{
						if (m.FieldNumberTo != toField)
							continue;
						if ((situation[m.FieldNumberFrom] & 7) != (int)pieceType)
							continue;
						if (Helper.FieldNumber2HorizontalCoordinateCode(m.FieldNumberFrom) == fromFile)
							sameFileMoves++;
					}

					if (sameFileMoves == 1)
					{
						// Disambiguate by file
						result += (char)(fromFile + 'a');
					}
					else
					{
						// Disambiguate by rank
						result += (char)(fromRank + '1');
					}
				}
			}
			else
			{
				// For pawn captures, add the source file
				if (move.MoveCode == MoveCode.HittingPiece || move.MoveCode == MoveCode.PromotePawnHittingPiece || move.MoveCode == MoveCode.EnPassant)
				{
					HorizontalCoordinateCode fromFile = Helper.FieldNumber2HorizontalCoordinateCode(fromField);
					result += (char)(fromFile + 'a');
				}
			}

			// Add capture indicator
			if (move.MoveCode == MoveCode.HittingPiece || move.MoveCode == MoveCode.PromotePawnHittingPiece || move.MoveCode == MoveCode.EnPassant)
				result += "x";

			// Add destination square
			result += Helper.FieldNumber2String(toField).ToLower();

			// Add promotion (default to Queen if not specified)
			if (move.MoveCode == MoveCode.PromotePawn || move.MoveCode == MoveCode.PromotePawnHittingPiece)
			{
				result += "=Q";
			}

			return result;
		}

		/// <summary>
		/// Parses Standard Algebraic Notation (SAN) in the current position and returns the matching legal move.
		/// </summary>
		/// <param name="san">A SAN move token such as <c>e4</c>, <c>Nf3</c>, <c>O-O</c>, or <c>axb8=Q</c>.</param>
		/// <returns>The matching legal move for the current board state.</returns>
		/// <exception cref="ArgumentException">Thrown when the SAN is empty, invalid, ambiguous, or not legal in the current position.</exception>
		public Move SanToMove(string san)
		{
			if (string.IsNullOrEmpty(san))
				throw new ArgumentException("SAN string cannot be empty");

			san = san.Trim();
			
			// Remove trailing check/checkmate indicators
			while (san.Length > 0 && (san[san.Length - 1] == '+' || san[san.Length - 1] == '#'))
				san = san.Substring(0, san.Length - 1);

			// Handle castling
			if (san == "O-O" || san == "0-0")
			{
				foreach (Move m in legalMoves)
				{
					if (m.MoveCode == MoveCode.SmallCastling)
						return m;
				}
				throw new ArgumentException("Kingside castling not available");
			}

			if (san == "O-O-O" || san == "0-0-0")
			{
				foreach (Move m in legalMoves)
				{
					if (m.MoveCode == MoveCode.BigCastling)
						return m;
				}
				throw new ArgumentException("Queenside castling not available");
			}

			// Parse regular moves
			// Format: [Piece][disambiguation][x]DestSquare[=Promotion]
			// Piece: K, Q, R, B, N (empty for pawn)
			// disambiguation: optional file (a-h) or rank (1-8)
			// x: capture indicator (optional)
			// DestSquare: required, file (a-h) + rank (1-8)
			// Promotion: =Q, =R, =B, =N (optional)

			int idx = 0;
			PieceType piecetype = PieceType.None;
			char? disambiguationFile = null;
			char? disambiguationRank = null;
			bool isCapture = false;
			string destSquare = "";
			PromotionPieceType? promotion = null;

			// Parse piece type (uppercase letters at position 0)
			if (idx < san.Length && char.IsUpper(san[idx]))
			{
				switch (san[idx])
				{
					case 'K':
						piecetype = PieceType.King;
						idx++;
						break;
					case 'Q':
						piecetype = PieceType.Queen;
						idx++;
						break;
					case 'R':
						piecetype = PieceType.Rook;
						idx++;
						break;
					case 'B':
						piecetype = PieceType.Bishop;
						idx++;
						break;
					case 'N':
						piecetype = PieceType.Knight;
						idx++;
						break;
					default:
						piecetype = PieceType.Pawn;
						break;
				}
			}
			else
			{
				piecetype = PieceType.Pawn;
			}

			// For pawn moves, special handling
			if (piecetype == PieceType.Pawn)
			{
				// Check for pawn capture: file letter + 'x' + destination
				if (idx < san.Length && san[idx] >= 'a' && san[idx] <= 'h' && 
					idx + 1 < san.Length && san[idx + 1] == 'x')
				{
					disambiguationFile = san[idx];
					idx += 2; // skip file and 'x'
					isCapture = true;
				}

				// Parse destination (must be next 2 chars: file + rank)
				if (idx + 1 < san.Length && san[idx] >= 'a' && san[idx] <= 'h' && san[idx + 1] >= '1' && san[idx + 1] <= '8')
				{
					destSquare = san.Substring(idx, 2);
					idx += 2;
				}
				else
				{
					throw new ArgumentException("Invalid SAN: no destination square found");
				}
			}
			else
			{
				// For piece moves, find the destination square working backwards
				// Find promotion indicator if present
				int promIdx = san.IndexOf('=');
				int endIdx = (promIdx >= 0) ? promIdx : san.Length;

				// Extract last 2 chars before promotion as destination (if they are file + rank)
				if (endIdx >= 2 && san[endIdx - 2] >= 'a' && san[endIdx - 2] <= 'h' &&
					san[endIdx - 1] >= '1' && san[endIdx - 1] <= '8')
				{
					destSquare = san.Substring(endIdx - 2, 2);
				}
				else
				{
					throw new ArgumentException("Invalid SAN: no valid destination square found");
				}

				// Check for 'x' before destination
				int xIdx = san.IndexOf('x');
				if (xIdx >= 0 && xIdx < endIdx - 2)
				{
					isCapture = true;
					// disambiguation is between piece and 'x'
					string between = san.Substring(idx, xIdx - idx);
					ParseDisambiguation(between, out disambiguationFile, out disambiguationRank);
				}
				else if (xIdx < 0)
				{
					// No 'x', so everything between piece and destination is disambiguation
					string between = san.Substring(idx, endIdx - 2 - idx);
					ParseDisambiguation(between, out disambiguationFile, out disambiguationRank);
				}
			}

			// Parse promotion
			if (idx < san.Length && san[idx] == '=')
			{
				idx++;
				if (idx < san.Length)
				{
					switch (san[idx])
					{
						case 'Q':
							promotion = PromotionPieceType.Queen;
							break;
						case 'R':
							promotion = PromotionPieceType.Rook;
							break;
						case 'B':
							promotion = PromotionPieceType.Bishop;
							break;
						case 'N':
							promotion = PromotionPieceType.Knight;
							break;
					}
				}
			}

			// Find destination field number
			int destFieldNumber = Helper.Coordinates2FieldNumber(
				Helper.HChar2HorizontalCoordinateCode(char.ToUpper(destSquare[0])),
				Helper.VChar2VerticalCoordinateCode(destSquare[1])
			);

			// Find matching move from legal moves
			Move matchingMove = null;
			int matchCount = 0;

			foreach (Move m in legalMoves)
			{
				if (m.FieldNumberTo != destFieldNumber)
					continue;

				// Check piece type
				PieceType movePieceType = (PieceType)(situation[m.FieldNumberFrom] & 7);
				if (movePieceType != piecetype)
					continue;

				// Check capture flag
				bool moveIsCapture = (m.MoveCode == MoveCode.HittingPiece || m.MoveCode == MoveCode.PromotePawnHittingPiece || m.MoveCode == MoveCode.EnPassant);
				if (isCapture && !moveIsCapture)
					continue;

				// Check promotion
				if (promotion.HasValue && m.MoveCode != MoveCode.PromotePawn && m.MoveCode != MoveCode.PromotePawnHittingPiece)
					continue;
				if (!promotion.HasValue && (m.MoveCode == MoveCode.PromotePawn || m.MoveCode == MoveCode.PromotePawnHittingPiece))
					continue;

				// Check disambiguation
				if (disambiguationFile.HasValue)
				{
					HorizontalCoordinateCode fromFile = Helper.FieldNumber2HorizontalCoordinateCode(m.FieldNumberFrom);
					if ((char)(fromFile + 'a') != disambiguationFile.Value)
						continue;
				}
				if (disambiguationRank.HasValue)
				{
					VerticalCoordinateCode fromRank = Helper.FieldNumber2VerticalCoordinateCode(m.FieldNumberFrom);
					if ((char)(fromRank + '1') != disambiguationRank.Value)
						continue;
				}

				matchingMove = m;
				matchCount++;
			}

			if (matchCount == 0)
				throw new ArgumentException("No legal move matches SAN: " + san);
			if (matchCount > 1)
				throw new ArgumentException("Ambiguous SAN notation (multiple moves match, need disambiguation): " + san);

			// If promotion was specified in SAN, set it on the move
			if (promotion.HasValue)
			{
				matchingMove.PromotionPieceType = promotion.Value;
			}

			return matchingMove;
		}

		private void ParseDisambiguation(string between, out char? file, out char? rank)
		{
			file = null;
			rank = null;

			foreach (char c in between)
			{
				if (c >= 'a' && c <= 'h')
					file = c;
				else if (c >= '1' && c <= '8')
					rank = c;
			}
		}


/*
		public Move GetBestMoveAlphaBeta(string openingBookFileName, OpeningBookFormat format)
		{
			OpeningBook openings = null;
			openings = (OpeningBook)openingBooks[openingBookFileName];
			if (openings == null)
			{
				bool bOK = false;
				openings = new OpeningBook();
				bOK = openings.Load(openingBookFileName, format);
				if (bOK)
					openingBooks[openingBookFileName] = openings;
				else
					openings = null;
			}
			return AlphaBetaPicker.GetBestMove(this.Clone(),openings);
		}
*/

		/// <summary>
		/// Computes best move using the legacy MTD engine, optionally backed by an opening book.
		/// </summary>
		/// <param name="openingBookFileName">Opening book file path/key.</param>
		/// <param name="format">Opening book notation format.</param>
		/// <returns>Best move returned by the MTD search engine.</returns>
		public Move GetBestMoveMTD(string openingBookFileName, OpeningBookFormat format)
		{
			OpeningBook openings = null;
			openings = (OpeningBook)openingBooks[openingBookFileName];
			if (openings == null)
			{
				bool bOK = false;
				openings = new OpeningBook();
				bOK = openings.Load(openingBookFileName, format);
				if (bOK)
					openingBooks[openingBookFileName] = openings;
				else
					openings = null;
			}
			return MTDPicker.GetBestMove(this.Clone(),openings);
		}

		/// <summary>
		/// Computes best move using the MTDv2 engine, optionally backed by an opening book.
		/// </summary>
		/// <param name="openingBookFileName">Opening book file path/key.</param>
		/// <param name="format">Opening book notation format.</param>
		/// <returns>Best move returned by the MTDv2 search engine.</returns>
		public Move GetBestMoveMTDv2(string openingBookFileName, OpeningBookFormat format)
		{
			OpeningBook openings = null;
			openings = (OpeningBook)openingBooks[openingBookFileName];
			if (openings == null)
			{
				bool bOK = false;
				openings = new OpeningBook();
				bOK = openings.Load(openingBookFileName, format);
				if (bOK)
					openingBooks[openingBookFileName] = openings;
				else
					openings = null;
			}
			return MTDPickerV2.GetBestMove(this.Clone(),openings);
		}

		//	}

	}

}
