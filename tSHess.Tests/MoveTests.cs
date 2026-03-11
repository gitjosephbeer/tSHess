using System;
using tSHess.Engine;
using Xunit;

namespace tSHess.Tests
{
    public class MoveTests
    {
        [Fact]
        public void Constructor_WithValidFields_SetsFromAndTo()
        {
            Move move = new Move(0, 9);

            Assert.Equal(0, move.FieldNumberFrom);
            Assert.Equal(9, move.FieldNumberTo);
        }

        [Fact]
        public void Constructor_WithInvalidFields_Throws()
        {
            Assert.Throws<ArgumentException>(() => new Move(-1, 9));
            Assert.Throws<ArgumentException>(() => new Move(0, 64));
        }

        [Fact]
        public void EqualsAndHashCode_SameCoordinates_AreEqual()
        {
            Move left = new Move(0, 1);
            Move right = new Move(0, 1);

            Assert.True(left.Equals(right));
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }

        [Fact]
        public void Equals_WithNullOrDifferentMove_ReturnsFalse()
        {
            Move move = new Move(0, 1);

            Assert.False(move.Equals(null));
            Assert.False(move.Equals(new Move(1, 2)));
        }

        [Fact]
        public void ToString_UsesBoardCoordinates()
        {
            Move move = new Move(0, 1);

            Assert.Equal("A1-A2", move.ToString());
        }

        [Fact]
        public void Inverse_SwapsFromAndTo()
        {
            Move move = new Move(8, 24);

            Move inverse = move.Inverse();
            Assert.Equal(24, inverse.FieldNumberFrom);
            Assert.Equal(8, inverse.FieldNumberTo);
        }

        [Fact]
        public void Clone_CopiesAllPublicDataAndCreatesIndependentInstance()
        {
            Move move = new Move(8, 24)
            {
                PieceType = PieceType.Pawn,
                MoveCode = MoveCode.NormalMove,
                PieceHit = PieceType.None,
                OwnSituationCode = SituationCode.Normal,
                OpponentSituationCode = SituationCode.Check,
                Evaluation = 25,
                EvaluationType = EvaluationType.Accurate,
                SearchDepth = 3,
                PromotionPieceType = PromotionPieceType.Knight,
                SnapShot = SnapShot.StartUpSnapShot()
            };

            Move clone = move.Clone();

            Assert.NotSame(move, clone);
            Assert.Equal(move.FieldNumberFrom, clone.FieldNumberFrom);
            Assert.Equal(move.FieldNumberTo, clone.FieldNumberTo);
            Assert.Equal(move.PieceType, clone.PieceType);
            Assert.Equal(move.MoveCode, clone.MoveCode);
            Assert.Equal(move.OwnSituationCode, clone.OwnSituationCode);
            Assert.Equal(move.OpponentSituationCode, clone.OpponentSituationCode);
            Assert.Equal(move.Evaluation, clone.Evaluation);
            Assert.Equal(move.EvaluationType, clone.EvaluationType);
            Assert.Equal(move.SearchDepth, clone.SearchDepth);
            Assert.Equal(move.PromotionPieceType, clone.PromotionPieceType);
            Assert.NotSame(move.SnapShot, clone.SnapShot);
        }

        [Fact]
        public void DefaultConstructor_UsesUnsetCoordinates()
        {
            Move move = new Move();

            Assert.Equal(-1, move.FieldNumberFrom);
            Assert.Equal(-1, move.FieldNumberTo);
            Assert.Equal(PieceType.None, move.PieceType);
            Assert.Equal(MoveCode.Unknown, move.MoveCode);
        }
    }
}