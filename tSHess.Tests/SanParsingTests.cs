using System;
using System.Collections.Generic;
using tSHess.Engine;
using Xunit;

namespace tSHess.Tests
{
    public class SanParsingTests
    {
        private static void RunSANTest(SnapShot snapshot, string[] moves)
        {
            var errors = new List<string>();
            foreach (string san in moves)
            {
                try
                {
                    Move move = snapshot.SANToMove(san);
                    snapshot.PerformMove(move);
                }
                catch (Exception ex)
                {
                    errors.Add($"'{san}': {ex.Message} | Legal: {snapshot.LegalMoves}");
                    break;
                }
            }
            Assert.Empty(errors);
        }

        [Fact]
        public void Test01_SicilianNajdorf()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "e4", "c5", "Nf3", "d6", "d4", "cxd4", "Nxd4", "Nf6", "Nc3", "a6", "g3", "e5" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test02_RuyLopezWithCastling()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "e4", "e5", "Nf3", "Nc6", "Bb5", "a6", "Ba4", "Nf6", "O-O", "Be7", "Re1", "b5", "Bb3", "d6" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test03_QueensGambit()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "d4", "d5", "c4", "e6", "Nf3", "Nf6", "Bg5", "Be7", "e3", "O-O" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test04_KnightDisambiguation()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "Nf3", "Nf6", "Nc3", "Nc6", "d4", "d5", "Nxd5", "Nxd5", "e4" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test05_MultipleCapturesAndPieceInteractions()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "e4", "c5", "Nf3", "Nc6", "d4", "cxd4", "Nxd4", "e5", "Nxc6", "bxc6", "Nc3", "Nf6" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test06_PawnAdvancementTowardPromotion()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = {
                "e4", "c5", "Nf3", "d6", "d4", "cxd4", "Nxd4", "Nf6", "Nc3", "a6",
                "Be3", "e5", "Nf3", "Be7", "g3", "O-O", "Qd2", "Nbd7", "O-O-O", "b5"
            };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test07_QueensideCastling()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "d4", "d5", "Nc3", "Nc6", "Bf4", "Bf5", "Qd2", "Qd7", "O-O-O" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test08_RookFileDisambiguation()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "a4", "a5", "Ra3", "e6", "Rh3", "Ke7", "Rg3", "Kd6", "Rf3", "Kc5" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test09_EnPassantCapture()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "e4", "a6", "e5", "d5", "exd6" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test10_PawnPromotionToQueen()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "axb8=Q" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test11_PawnPromotionToRook()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "axb8=R" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test12_PawnPromotionToBishop()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "axb8=B" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test13_PawnPromotionToKnight()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "axb8=N" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test14_EndgameWithMixedCaptures()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = {
                "e4", "c5", "Nf3", "d6", "d4", "cxd4", "Nxd4", "Nf6",
                "Nc3", "a6", "Bg5", "e6", "f4", "Be7", "Qf3", "Nbd7"
            };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test15_BishopMovesAndDisambiguation()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = {
                "d4", "d5", "Bf4", "Nf6", "Nf3", "c6", "c3", "Bf5",
                "Nbd2", "Nbd7", "e3", "e6", "Bd3", "Bxd3"
            };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test16_CheckSuffixParsing()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "e4", "e5", "Qh5", "Nc6", "Bc4", "Nf6", "Qxf7+" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test17_CheckmateSuffixParsing()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "f3", "e5", "g4", "Qh4#" };
            RunSANTest(snapshot, moves);
        }

        [Fact]
        public void Test18_CastlingWithZeroNotation()
        {
            SnapShot snapshot = SnapShot.StartUpSnapShot();
            string[] moves = { "e4", "e5", "Nf3", "Nc6", "Bc4", "Bc5", "0-0" };
            RunSANTest(snapshot, moves);
        }
    }
}
