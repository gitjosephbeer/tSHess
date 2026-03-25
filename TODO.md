# tSHess Optimization TODO

## Current Baseline
- [x] SEE + pawn hash integrated
- [x] Stable benchmark reference captured
  - PVS vs MTD: ~62.2% faster (runs=7, depth=7)

## Tier 2 Work Log
- [x] King safety evaluation
- [x] Pawn hash cache
- [x] Full SEE capture ordering
- [x] SEE tuning finalized (kept simple baseline formula)
- [x] Null Move Pruning (NMP) implemented
  - Latest quick check: ~61.8% faster (runs=3, depth=7)
- [x] LMR prototype tested
- [x] LMR disabled for now (not beneficial with current tuning)

## Next Steps
- [ ] Implement aspiration windows in iterative deepening
- [ ] Re-benchmark after aspiration windows (runs=3 depth=7)
- [ ] If positive, run long confirmation (runs=7 depth=7)
- [ ] Revisit LMR with stricter gating after aspiration windows
  - Candidate: apply only on quiet non-check moves at deeper nodes
- [ ] Consider transposition table replacement strategy tuning

## Tier 3 (Strength-Focused)
- [ ] 🔄 Add automated A/B self-play harness (SPRT-style pass/fail) **IN PROGRESS**
  - Goal: validate Elo gain before keeping a heuristic
- [ ] Build a regression suite of tactical FENs
  - Track solved positions + node count per release
- [ ] Add quiescence safety improvements
  - Delta pruning / SEE-based bad-capture filtering to reduce tactical noise
- [ ] Improve move ordering beyond killers/history
  - Counter-move heuristic and continuation history tables
- [ ] Add verification search for aggressive pruning
  - Verify null-move fail-high in zugzwang-prone structures
- [ ] Improve time management in search loop
  - Add soft/hard time limits and panic-time extension in tactical positions
- [ ] Add draw/repetition handling hardening
  - Explicit threefold and 50-move rule behavior in search/eval decisions
- [ ] Expand evaluation with low-risk terms
  - Mobility, bishop pair, rook on open/semi-open file, passed-pawn scaling
- [ ] Add endgame-specific logic
  - Basic king opposition and rook endgame heuristics
- [ ] (Optional) Add Syzygy tablebase probing for 5-man/6-man endings

## Benchmark Routine (Reminder)
- [ ] Build Release
- [ ] Delete old benchmark-report.txt before each run
- [ ] Run compare benchmark with --no-book
- [ ] Archive report after each significant change

## Guardrails
- [ ] Keep all tests green (173/173)
- [ ] If performance regresses, revert/toggle feature quickly
- [ ] Prefer one search change at a time + A/B benchmark
