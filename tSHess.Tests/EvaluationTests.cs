using tSHess.Engine;
using Xunit;

namespace tSHess.Tests
{
    public class EvaluationTests
    {
        [Fact]
        public void EvaluateMaterial_StartupPosition_IsZeroForBothSides()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();

            Assert.Equal(0, snapshot.EvaluateMaterial(Color.White));
            Assert.Equal(0, snapshot.EvaluateMaterial(Color.Black));
        }

        [Fact]
        public void EvaluateMaterial_AfterSymmetricMoves_RemainsZero()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("e5"));

            Assert.Equal(0, snapshot.EvaluateMaterial(Color.White));
            Assert.Equal(0, snapshot.EvaluateMaterial(Color.Black));
        }

        [Fact]
        public void EvaluateMaterial_AfterWinningPawn_IsPositiveForWinningSide()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("d5"));
            snapshot.PerformMove(snapshot.SanToMove("exd5"));

            Assert.True(snapshot.EvaluateMaterial(Color.White) > 0);
            Assert.True(snapshot.EvaluateMaterial(Color.Black) < 0);
        }

        [Fact]
        public void EvaluateComplete_StartupPosition_IsConsistentAcrossPerspective()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            int white = snapshot.EvaluateComplete(Color.White);
            int black = snapshot.EvaluateComplete(Color.Black);

            Assert.Equal(white, black);
        }

        [Fact]
        public void EvaluateComplete_AfterWinningPawn_FavorsWinningSide()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            snapshot.PerformMove(snapshot.SanToMove("e4"));
            snapshot.PerformMove(snapshot.SanToMove("d5"));
            snapshot.PerformMove(snapshot.SanToMove("exd5"));

            Assert.True(snapshot.EvaluateComplete(Color.White) > snapshot.EvaluateComplete(Color.Black));
        }
    }
}