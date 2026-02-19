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

namespace tSHess.Engine
{
	public enum Color : int
	{
		White	= 0,
		Black	= 8
	}

	public enum EvaluationType : int
	{
		Null		= -1,
		Accurate	= 0,
		Upperbound	= 1,
		Lowerbound	= 2
	}

	public enum MoveType : int
	{
		Null		= -1,
		White		= Color.White,
		Black		= Color.Black
	}

	public enum PlayerMode : int
	{
		Human		= 0,
		Computer	= 1
	}

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

	public enum Mask : int
	{
		PieceType	= 7,
		Color		= 8,
		MovedFlag	= 16,
		NullFlag	= 32
	}

	public enum PromotionPieceType : int
	{
		Queen	= PieceType.Queen,
		Bishop	= PieceType.Bishop,
		Knight	= PieceType.Knight,
		Rook	= PieceType.Rook
	}

	public enum SituationCode : int
	{
		Unknown		= -1,
		Normal		= 0,
		Check		= 1,
		Checkmate	= 2,
		Stalemate	= 3,
	}

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

		public static int PieceType2Value(PieceType pieceType)
		{
			return pieceType2Value[(int)pieceType];
		}

		public static Color OpponentColor(Color color)
		{
			if (color == Color.White)
				return Color.Black;
			else
				return Color.White;
		}

		public static string PieceType2LongName(PieceType pieceType)
		{
			return pieceType2LongName[(int)pieceType];
		}

		public static string PieceType2ShortName(PieceType pieceType)
		{
			return pieceType2ShortName[(int)pieceType];
		}

		public static int MoveDirectionCode2Step(MoveDirectionCode c)
		{
			return moveDirectionCode2Step[(int)c];
		}

		public static Color FieldNumber2Color(int f)
		{
			return fieldNumber2Color[f];
		}

		public static HorizontalCoordinateCode FieldNumber2HorizontalCoordinateCode(int f)
		{
			return fieldNumber2HorizontalCoordinateCode[f];
		}

		public static VerticalCoordinateCode FieldNumber2VerticalCoordinateCode(int f)
		{
			return fieldNumber2VerticalCoordinateCode[f];
		}

		public static string FieldNumber2String(int f)
		{
			return ((char)(fieldNumber2HorizontalCoordinateCode[f] + 'A')).ToString()+((char)(fieldNumber2VerticalCoordinateCode[f] + '1')).ToString();
		}

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

		public static HorizontalCoordinateCode HChar2HorizontalCoordinateCode(char c)
		{
			if (c < 'A' || c > 'H')
				throw new ArgumentException("Out of range","c");
			return (HorizontalCoordinateCode)(c - 'A');
		}

		public static VerticalCoordinateCode VChar2VerticalCoordinateCode(char c)
		{
			if (c < '1' || c > '8')
				throw new ArgumentException("Out of range","c");
			return (VerticalCoordinateCode)(c - '1');
		}

		public static string Coordinates2String(HorizontalCoordinateCode hc, VerticalCoordinateCode vc)
		{
			return ((char)(hc + 'A')).ToString()+((char)(vc + '1')).ToString();
		}

		public static int Coordinates2FieldNumber(HorizontalCoordinateCode hc, VerticalCoordinateCode vc)
		{
			return ((int)hc)*8 + (int)vc;
		}
		
		private static Random random = new Random((int)DateTime.Now.Ticks);
		public static int RandomNumber(int maxNumber)
		{
			return random.Next(maxNumber);
		}
		public static int RandomNumber(int minNumber, int maxNumber)
		{
			return random.Next(minNumber,maxNumber);
		}
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

		public OpeningBook()
		{
			openingBookEntryTable = new OpeningBookEntry[TABLE_SIZE];
			for (int i = 0; i < TABLE_SIZE; i++)
				openingBookEntryTable[i] = new OpeningBookEntry();
		}

		// Querying the table for a ready-made move to play.  Return null if there
		// is none
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

		// Loading the table from a file
		public bool Load(string fileName)
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

		// Validate an openings file. Returns a list of error messages (empty if valid).
		public List<string> ValidateOpenings(string fileName)
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

		// Construction
		public TranspositionTable()
		{
			table = new TranspositionEntry[TABLE_SIZE];
			for (int i = 0; i < TABLE_SIZE; i++)
				table[i] = new TranspositionEntry();
		}

		// boolean LookupBoard( jcBoard theBoard, jcMove theMove )
		// Verify whether there is a stored evaluation for a given board.
		// If so, return TRUE and copy the appropriate values into the
		// output parameter
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
			return true;
		}

		// public StoreBoard( theBoard, eval, evalType, depth, timeStamp )
		// Store a good evaluation found through alphabeta for a certain board position
		public bool StoreSnapShot(SnapShot snapShot, int evaluation, EvaluationType evaluationType, int searchDepth, int timeStamp)
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
			return true;
		}
	} // TranspositionTable


	public class MoveHistoryTable
	{
		private int[,,] history = new int[2,64,64];

		// This is a singleton class; the same history can be shared by two AI's
		private static MoveHistoryTable singleInstance;

		static MoveHistoryTable()
		{
			singleInstance = new MoveHistoryTable();
		}

		public class MoveComparer : IComparer
		{
			private int colorIndex = 0;
			public int Compare(object x, object y)
			{
				Move mx = x as Move;
				Move my = y as Move;
				if (GetInstance().history[colorIndex,mx.FieldNumberFrom,mx.FieldNumberTo] > GetInstance().history[colorIndex,my.FieldNumberFrom,my.FieldNumberTo])
					return -1;
				else
					return 1;
			}
			public MoveComparer(Color color)
			{
				if (color == Color.White)
					colorIndex = 0;
				else
					colorIndex = 1;
			}
		}

		public static MoveHistoryTable GetInstance()
		{
			return singleInstance;
		}

		public void AddMove(Color color, Move move)
		{
			if (move == null)
				return;

			if (color == Color.White)
				history[0,move.FieldNumberFrom,move.FieldNumberTo]++;
			else
				history[1,move.FieldNumberFrom,move.FieldNumberTo]++;
		}

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

		public override bool Equals(object o)
		{
			Move m = o as Move;
			if (m != null && m.FieldNumberFrom == this.FieldNumberFrom && m.FieldNumberTo == this.FieldNumberTo)
				return true;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override string ToString()
		{
			return Helper.FieldNumber2String(this.FieldNumberFrom)+"-"+Helper.FieldNumber2String(this.FieldNumberTo);
		}

		public string ToString(bool includeSnapShot)
		{
			string retVal = "";
			if (includeSnapShot && this.SnapShot != null)
					retVal = this.SnapShot.ToString()+": -> ";
			retVal = retVal+this.ToString();
			return retVal;
		}
		
		public Move Inverse()
		{
			return new Move(this.FieldNumberTo,this.FieldNumberFrom);
		}

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
			return m;
		}

		public Move()
		{
		}

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
	
	public class MoveList : ICollection
	{
		private ArrayList moves = new ArrayList();

		private bool suppressDuplicates = true;
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

		public virtual void CopyTo(Array array, int index)
		{
			moves.CopyTo(array,index);
		}

		public virtual int Count
		{
			get
			{
				return moves.Count;
			}
		}

		public virtual void Sort(Color color)
		{
			SortedList sl = new SortedList(new MoveHistoryTable.MoveComparer(color));
			for (int i = 0; i < Count; i++)
				sl.Add(this[i],null);
			this.Clear();
			for (int i = 0; i < sl.Count; i++)
				this.Add((Move)sl.GetKey(i));
		}

		public virtual IEnumerator GetEnumerator()
		{
			return moves.GetEnumerator();
		}

		public virtual bool IsSynchronized
		{
			get
			{
				return moves.IsSynchronized;
			}
		}

		public virtual object SyncRoot
		{
			get
			{
				return moves.SyncRoot;
			}
		}

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

		public virtual void Add(Move move)
		{
			if (move == null || (suppressDuplicates && this.Contains(move)))
				return;
			moves.Add(move);
		}

		public virtual void Merge(MoveList moves)
		{
			if (moves == null)
				return;
			for (int i = 0; i < moves.Count; i++)
				this.Add(moves[i]);
		}

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

		public virtual void RemoveAt(int index)
		{
			if (moves.Count == 0 || index < 0 || index+1 > moves.Count)
				return;
			moves.RemoveAt(index);
		}

		public virtual void Clear()
		{
			moves.Clear();
		}

		public virtual Move this[int index]
		{
			get
			{
				if (moves.Count == 0 || index < 0 || index+1 > moves.Count)
					return null;
				return (Move)moves[index];
			}
		}

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

		public MoveList Clone()
		{
			MoveList ml = new MoveList();
			ml.suppressDuplicates = this.suppressDuplicates;
			for (int i = 0; i < this.Count; i++)
				ml.Add(this[i].Clone());
			return ml;
		}

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
	} // MoveList


	public delegate void PieceHitEventHandler(object sender, PieceHitEventArgs e);
	
	public class PieceHitEventArgs : EventArgs
	{
		private PieceType pieceHit = PieceType.None;
		public PieceType PieceHit
		{
			get
			{
				return pieceHit;
			}
		}
		public PieceHitEventArgs(PieceType pieceHit)
		{
			if (pieceHit == PieceType.None)
				throw new ArgumentException("Invalid","pieceHit");
			this.pieceHit = pieceHit;
		}
	}

	public delegate void PromotePawnEventHandler(object sender, PromotePawnEventArgs e);
	
	public class PromotePawnEventArgs : EventArgs
	{
		public PromotionPieceType PromoteTo = PromotionPieceType.Queen;
	}

	public delegate void CheckEventHandler(object sender, CheckEventArgs e);
	
	public class CheckEventArgs : EventArgs
	{
	}

	public delegate void GameOverEventHandler(object sender, GameOverEventArgs e);
	
	public class GameOverEventArgs : EventArgs
	{
	}


	public class SnapShot
	{
		private static BestMovePicker MTDPicker = new MTD();
//		private static BestMovePicker AlphaBetaPicker = new AlphaBeta();
		private static Hashtable openingBooks = new Hashtable();

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

			// Construction
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

		public class MTD : BestMovePicker
		{
			// A measure of the effort we are willing to expend on search
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
				// assign a small penalty to positions in which there are still all
				// pawns... this incites a single pawn trade (to open a file)
				if (whitePawnsCount == 8)
					score -= 10;

				// look for a passed pawn; i.e., a pawn which can no longer be
				// blocked or attacked by a rival pawn
				if (columnMostAdvancedOwnPawnFieldNum[0] > Math.Max(columnLeastAdvancedOpponentPawnFieldNum[0],columnLeastAdvancedOpponentPawnFieldNum[1]))
				{
					x = (columnMostAdvancedOwnPawnFieldNum[0] % 8) + 1;
					score += x*x;
				}
				if (columnMostAdvancedOwnPawnFieldNum[7] > Math.Max(columnLeastAdvancedOpponentPawnFieldNum[7],columnLeastAdvancedOpponentPawnFieldNum[6]))
				{
					x = (columnMostAdvancedOwnPawnFieldNum[7] % 8) + 1;
					score += x*x;
				}
				for (int i = 1; i < 7; i++)
				{
					if (columnMostAdvancedOwnPawnFieldNum[i] > columnLeastAdvancedOpponentPawnFieldNum[i] && columnMostAdvancedOwnPawnFieldNum[i] > columnLeastAdvancedOpponentPawnFieldNum[i-1] && columnMostAdvancedOwnPawnFieldNum[i] > columnLeastAdvancedOpponentPawnFieldNum[i+1])
					{
						x = (columnMostAdvancedOwnPawnFieldNum[i] % 8) + 1;
						score += x*x;
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
					score += 10;
				else
				{
					byte k = situation[32];
					byte rq = situation[0];
					byte rk = situation[56];

					if (k != 37 || (rq != 33 && rk != 33))
						score -= 120;
					else if (k == 37 && rq == 33 && rk == 33)
						score -= 24;
					else if (k == 37 && rq != 33 && rk == 33)
						score -= 40;
					else if (k == 37 && rq == 33 && rk != 33)
						score -= 80;
				}

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
								break;
							case 3: // bishop
								// are there a lot of pawns on fields with same color?
								if (Helper.FieldNumber2Color(i) == Color.White)
									score -= fieldColorPawnCount[0] << 3;
								else
									score -= fieldColorPawnCount[1] << 3;
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
					score += x*x;
				}
				if (columnMostAdvancedOwnPawnFieldNum[7] < Math.Min(columnLeastAdvancedOpponentPawnFieldNum[7],columnLeastAdvancedOpponentPawnFieldNum[6]))
				{
					x = 8 - (columnMostAdvancedOwnPawnFieldNum[7] % 8);
					score += x*x;
				}
				for (int i = 1; i < 7; i++)
				{
					if (columnMostAdvancedOwnPawnFieldNum[i] < columnLeastAdvancedOpponentPawnFieldNum[i] && columnMostAdvancedOwnPawnFieldNum[i] < columnLeastAdvancedOpponentPawnFieldNum[i-1] && columnMostAdvancedOwnPawnFieldNum[i] < columnLeastAdvancedOpponentPawnFieldNum[i+1])
					{
						x = 8 - (columnMostAdvancedOwnPawnFieldNum[i] % 8);
						score += x*x;
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
					score += 10;
				else
				{
					byte k = situation[39];
					byte rq = situation[7];
					byte rk = situation[63];

					if (k != 45 || (rq != 41 && rk != 41))
						score -= 120;
					else if (k == 45 && rq == 41 && rk == 41)
						score -= 24;
					else if (k == 45 && rq != 41 && rk == 41)
						score -= 40;
					else if (k == 45 && rq == 41 && rk != 41)
						score -= 80;
				}

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
								break;
							case 3: // bishop
								// are there a lot of pawns on fields with same color?
								if (Helper.FieldNumber2Color(i) == Color.White)
									score -= fieldColorPawnCount[0] << 3;
								else
									score -= fieldColorPawnCount[1] << 3;
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

		// Look at pawn positions to be able to detect features such as doubled,
		// isolated or passed pawns
		private void AnalyzePawnSituation(Color color)
		{
			// Reset the counters
			int tmp = 0;
			if (color == Color.White)
				tmp = 63;
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

		// Compute the board's material balance, from the point of view of the "color" player
		public int EvaluateComplete(Color color)
		{
			AnalyzePawnSituation(color);
			return
				EvaluateMaterial(color)+
				EvaluateSituation(color);
		}


		// Compute the board's material balance, from the point of view of the "color" player
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

		public event PromotePawnEventHandler PromotePawnEventHandler;
		public event CheckEventHandler CheckEventHandler;
		public event GameOverEventHandler GameOverEventHandler;
		public event PieceHitEventHandler PieceHitEventHandler;

		public static SnapShot StartUpSnapShot()
		{
			SnapShot s = new SnapShot((byte[])startupSituation.Clone(),Color.White,false,false,true);
			s.CalculateLegalMoves();
			return s;
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
				temp += Environment.NewLine+"Last move done: "+history[history.Count-1].ToString()+Environment.NewLine;
			temp += Environment.NewLine+(whoToMove == Color.White ? "White" : "Black")+" to move now..."+Environment.NewLine;
			temp += Environment.NewLine+"Legal moves:"+Environment.NewLine+legalMoves.ToString();

			return temp;
		}

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
						if (!internalUsage && PromotePawnEventHandler != null)
							PromotePawnEventHandler(this,ea);
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
						if (!internalUsage && PromotePawnEventHandler != null)
							PromotePawnEventHandler(this,ea);
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

/*
		public Move GetBestMoveAlphaBeta(string openingBookFileName)
		{
			OpeningBook openings = null;
			openings = (OpeningBook)openingBooks[openingBookFileName];
			if (openings == null)
			{
				bool bOK = false;
				openings = new OpeningBook();
				bOK = openings.Load(openingBookFileName);
				if (bOK)
					openingBooks[openingBookFileName] = openings;
				else
					openings = null;
			}
			return AlphaBetaPicker.GetBestMove(this.Clone(),openings);
		}
*/

		public Move GetBestMoveMTD(string openingBookFileName)
		{
			OpeningBook openings = null;
			openings = (OpeningBook)openingBooks[openingBookFileName];
			if (openings == null)
			{
				bool bOK = false;
				openings = new OpeningBook();
				bOK = openings.Load(openingBookFileName);
				if (bOK)
					openingBooks[openingBookFileName] = openings;
				else
					openings = null;
			}
			return MTDPicker.GetBestMove(this.Clone(),openings);
		}

		//	}

	}

}
