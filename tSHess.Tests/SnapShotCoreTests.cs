using System;
using tSHess.Engine;
using Xunit;

namespace tSHess.Tests
{
    public class SnapShotCoreTests
    {
        [Fact]
        public void StartUpSnapShot_HasExpectedInitialState()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();

            Assert.Equal(Color.White, snapshot.WhoToMove);
            Assert.Equal(20, snapshot.LegalMoves.Count);
        }

        [Fact]
        public void SanToMove_ParsesSimplePawnMove()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();

            Move move = snapshot.SanToMove("e4");

            Assert.Equal(33, move.FieldNumberFrom);
            Assert.Equal(35, move.FieldNumberTo);
        }

        [Fact]
        public void MoveToSan_RoundTripsSimpleMove()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            Move move = snapshot.SanToMove("e4");

            string san = snapshot.MoveToSan(move);

            Assert.Equal("e4", san);
        }

        [Fact]
        public void SanToMove_ParsesCapture()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("d5"));

            Move move = snapshot.SanToMove("exd5");

            Assert.Equal(35, move.FieldNumberFrom);
            Assert.Equal(28, move.FieldNumberTo);
            Assert.Equal(MoveCode.HittingPiece, move.MoveCode);
        }

        [Fact]
        public void SanToMove_ParsesCheckSuffix()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("e5"));
            snapshot.PerformMove(snapshot.SanToMove("Qh5"));
            snapshot.PerformMove(snapshot.SanToMove("Nc6"));
            snapshot.PerformMove(snapshot.SanToMove("Bc4"));
            snapshot.PerformMove(snapshot.SanToMove("Nf6"));

            Move move = snapshot.SanToMove("Qxf7+");

            Assert.NotNull(move);
            Assert.Equal(MoveCode.HittingPiece, move.MoveCode);
        }

        [Fact]
        public void SanToMove_ParsesCastlingWithZeroNotation()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("e5"));
            snapshot.PerformMove(snapshot.SanToMove("Nf3"));
            snapshot.PerformMove(snapshot.SanToMove("Nc6"));
            snapshot.PerformMove(snapshot.SanToMove("Bc4"));
            snapshot.PerformMove(snapshot.SanToMove("Bc5"));

            Move move = snapshot.SanToMove("0-0");

            Assert.Equal(MoveCode.SmallCastling, move.MoveCode);
        }

        [Fact]
        public void PerformMove_ChangesTurnAndRecalculatesLegalMoves()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();

            snapshot.PerformMove(snapshot.SanToMove("e4"));

            Assert.Equal(Color.Black, snapshot.WhoToMove);
            Assert.NotEmpty(snapshot.LegalMoves);
        }

        [Fact]
        public void Rollback_RestoresEarlierPosition()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string original = snapshot.ToString();

            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("e5"));
            snapshot.Rollback(2);

            Assert.Equal(Color.White, snapshot.WhoToMove);
            Assert.Equal(20, snapshot.LegalMoves.Count);
            Assert.Equal(original, snapshot.ToString());
        }

        [Fact]
        public void Clone_CreatesIndependentCopy()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            SnapShot clone = snapshot.Clone();

            clone.PerformMove(clone.SanToMove("e4"));

            Assert.Equal(Color.White, snapshot.WhoToMove);
            Assert.Equal(Color.Black, clone.WhoToMove);
        }

        [Fact]
        public void SanToMove_InvalidNotation_Throws()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();

            Assert.Throws<ArgumentException>(() => snapshot.SanToMove("z9"));
            Assert.Throws<ArgumentException>(() => snapshot.SanToMove(""));
        }
    }
}