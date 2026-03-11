using System;
using tSHess.Engine;
using Xunit;

namespace tSHess.Tests
{
    public class HelperTests
    {
        [Fact]
        public void OpponentColor_WhiteAndBlack_RoundTrips()
        {
            Assert.Equal(Color.Black, Helper.OpponentColor(Color.White));
            Assert.Equal(Color.White, Helper.OpponentColor(Color.Black));
        }

        [Fact]
        public void PieceTypeMappings_ReturnExpectedValues()
        {
            Assert.Equal(100, Helper.PieceType2Value(PieceType.Pawn));
            Assert.Equal(900, Helper.PieceType2Value(PieceType.Queen));
            Assert.Equal("Knight", Helper.PieceType2LongName(PieceType.Knight));
            Assert.Equal("N", Helper.PieceType2ShortName(PieceType.Knight));
        }

        [Fact]
        public void PieceTypeMappings_InvalidNone_Throws()
        {
            Assert.ThrowsAny<Exception>(() => Helper.PieceType2LongName(PieceType.None));
            Assert.ThrowsAny<Exception>(() => Helper.PieceType2ShortName(PieceType.None));
            Assert.ThrowsAny<Exception>(() => Helper.PieceType2Value(PieceType.None));
        }

        [Fact]
        public void CoordinateConversions_Boundaries_AreCorrect()
        {
            Assert.Equal(0, Helper.Coordinates2FieldNumber(HorizontalCoordinateCode.HA, VerticalCoordinateCode.V1));
            Assert.Equal(63, Helper.Coordinates2FieldNumber(HorizontalCoordinateCode.HH, VerticalCoordinateCode.V8));
            Assert.Equal("A1", Helper.FieldNumber2String(0));
            Assert.Equal("H8", Helper.FieldNumber2String(63));
            Assert.Equal("C4", Helper.Coordinates2String(HorizontalCoordinateCode.HC, VerticalCoordinateCode.V4));
        }

        [Fact]
        public void FieldNumberCoordinateMappings_AreConsistentForAllSquares()
        {
            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    int field = file * 8 + rank;
                    Assert.Equal((HorizontalCoordinateCode)file, Helper.FieldNumber2HorizontalCoordinateCode(field));
                    Assert.Equal((VerticalCoordinateCode)rank, Helper.FieldNumber2VerticalCoordinateCode(field));
                }
            }
        }

        [Fact]
        public void CoordinateCharParsers_ValidInputs_Work()
        {
            Assert.Equal(HorizontalCoordinateCode.HA, Helper.HChar2HorizontalCoordinateCode('A'));
            Assert.Equal(HorizontalCoordinateCode.HH, Helper.HChar2HorizontalCoordinateCode('H'));
            Assert.Equal(VerticalCoordinateCode.V1, Helper.VChar2VerticalCoordinateCode('1'));
            Assert.Equal(VerticalCoordinateCode.V8, Helper.VChar2VerticalCoordinateCode('8'));
        }

        [Fact]
        public void CoordinateCharParsers_InvalidInputs_Throw()
        {
            Assert.Throws<ArgumentException>(() => Helper.HChar2HorizontalCoordinateCode('@'));
            Assert.Throws<ArgumentException>(() => Helper.HChar2HorizontalCoordinateCode('a'));
            Assert.Throws<ArgumentException>(() => Helper.VChar2VerticalCoordinateCode('0'));
            Assert.Throws<ArgumentException>(() => Helper.VChar2VerticalCoordinateCode('9'));
        }

        [Fact]
        public void String2Move_ParsesCoordinateMove()
        {
            Move move = Helper.String2Move("A2-A4");

            Assert.Equal(1, move.FieldNumberFrom);
            Assert.Equal(3, move.FieldNumberTo);
        }

        [Fact]
        public void FieldColorPattern_AlternatesByRankAndFileParity()
        {
            for (int file = 0; file < 8; file++)
            {
                for (int rank = 0; rank < 8; rank++)
                {
                    int field = file * 8 + rank;
                    int parity = (file + rank) % 2;
                    Color expected = parity == 0 ? Color.Black : Color.White;
                    Assert.Equal(expected, Helper.FieldNumber2Color(field));
                }
            }
        }
    }
}