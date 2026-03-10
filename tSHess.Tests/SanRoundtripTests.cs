using System;
using System.Collections.Generic;
using tSHess.Engine;
using Xunit;

namespace tSHess.Tests
{
    public class SanRoundtripTests
    {
        private static void ValidateRoundTripAtPosition(SnapShot snap, List<string> errors)
        {
            MoveList legal = snap.LegalMoves;
            for (int i = 0; i < legal.Count; i++)
            {
                Move legalMove = legal[i];
                try
                {
                    string san = snap.MoveToSan(legalMove);
                    Move reparsed = snap.SanToMove(san);
                    if (reparsed == null ||
                        reparsed.FieldNumberFrom != legalMove.FieldNumberFrom ||
                        reparsed.FieldNumberTo != legalMove.FieldNumberTo)
                    {
                        errors.Add($"roundtrip mismatch for {legalMove} -> '{san}' -> {reparsed}");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"exception on {legalMove}: {ex.Message}");
                }
            }
        }

        private static void RunRoundtripScenario(SnapShot snapshot, string[] moves, List<string> errors)
        {
            ValidateRoundTripAtPosition(snapshot, errors);
            foreach (string san in moves)
            {
                try
                {
                    Move move = snapshot.SanToMove(san);
                    snapshot.PerformMove(move);
                    ValidateRoundTripAtPosition(snapshot, errors);
                }
                catch (Exception ex)
                {
                    errors.Add($"scenario move parse failed at '{san}': {ex.Message}");
                    break;
                }
            }
        }

        [Fact]
        public void Scenario1_Sicilian()
        {
            var errors = new List<string>();
            RunRoundtripScenario(
                SnapShot.StartUpSnapShot(),
                new[] { "e4", "c5", "Nf3", "d6", "d4", "cxd4", "Nxd4", "Nf6", "Nc3", "a6" },
                errors);
            Assert.Empty(errors);
        }

        [Fact]
        public void Scenario2_RuyLopez()
        {
            var errors = new List<string>();
            RunRoundtripScenario(
                SnapShot.StartUpSnapShot(),
                new[] { "e4", "e5", "Nf3", "Nc6", "Bb5", "a6", "Ba4", "Nf6", "O-O", "Be7" },
                errors);
            Assert.Empty(errors);
        }

        [Fact]
        public void Scenario3_EnPassant()
        {
            var errors = new List<string>();
            RunRoundtripScenario(
                SnapShot.StartUpSnapShot(),
                new[] { "e4", "a6", "e5", "d5", "exd6" },
                errors);
            Assert.Empty(errors);
        }

        [Fact]
        public void Scenario4_Promotion()
        {
            var errors = new List<string>();
            RunRoundtripScenario(
                SnapShot.StartUpSnapShot(),
                new[] { "a4", "b5", "axb5", "a6", "bxa6", "Nf6", "a7", "d5", "axb8=Q" },
                errors);
            Assert.Empty(errors);
        }

        [Fact]
        public void Scenario5_CheckCheckmateInput()
        {
            var errors = new List<string>();
            RunRoundtripScenario(
                SnapShot.StartUpSnapShot(),
                new[] { "f3", "e5", "g4", "Qh4#" },
                errors);
            Assert.Empty(errors);
        }
    }
}
