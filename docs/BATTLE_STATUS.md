# Battle System Status

Short version: the server now has a minimal, runnable battle path for testing, but it is still not a full Wonderland Online combat implementation.

## What works now

- `AC11` is compiled and can start a simple PK/training battle request.
- `AC50` is compiled and accepts a basic attack action while the player is in battle.
- `Battle.StartBattle()` now sends the battle setup packet (`11,250`) and battle start packet (`11,10`).
- `Battle.PLayer_BattleAction()` records a player's action, sends a ready acknowledgement, resolves simple physical damage, broadcasts attack/HP packets, and starts another round when both sides are still alive.
- `Player` now implements the `Fighter` interface, so real players can be added to `BattleScene` sides.
- `BattleScene` now assigns grid slots and can actually add/remove/find fighters.
- A lightweight `TrainingFighter` NPC fighter exists for local testing.

## Test command

Use this in chat from a connected character:

```text
:battle test
```

That starts a one-player training battle against a dummy NPC. The dummy uses simple stats and auto-queues its basic attack each round.

## Client/request support

The battle request action code is also wired:

- `AC11,2` with PK type `2` starts a player-vs-player battle if the target character is on the same loaded map and not already in battle.
- `AC11,2` with PK type `3` starts a training/NPC-style battle against a simple server-side dummy fighter.
- `AC11,1` can remove the player from battle/run away.
- `AC50,1` submits a basic battle action.

## Remaining limitations

This is intentionally a minimal first pass. The following are still not complete:

- No real skill table integration; all submitted actions currently resolve as a basic physical hit.
- No pet combat.
- No real NPC map spawning/random encounters yet.
- No battle persistence/active battle collection on `GameMap`; this path is driven by the player/battle references.
- No rewards, EXP, drops, quest hooks, or battle result screen logic beyond ending the battle.
- No GM permission gate around the new developer test command yet.

## Suggested next steps

1. Add a real GM permission gate before exposing `:battle test`, `:item`, or `:warp` outside a private/dev server.
2. Wire map NPC definitions and NPC click handling into `AC11,2` PK type `3` instead of always using the dummy fighter.
3. Load skill data and attach selected skill IDs to `BattleAction` so `AC50` can resolve real skills.
4. Add rewards/EXP/drop callbacks to `EndBattle()`.
