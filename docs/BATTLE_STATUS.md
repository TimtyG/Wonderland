# Battle System Status

Short version: the battle system is only partially present and is not wired into the running server yet.

## Why battles do not currently work

### Battle action codes are not compiled into the active server

The main project only compiles these action-code handlers:

- `AC0`
- `AC02`
- `AC06`
- `AC09`
- `AC12`
- `AC20`
- `AC63`

The battle-related handlers (`AC11` for PK / battle requests and `AC50` for battle actions) are present as source files, but they are not included in the main project file. They also still reference older namespaces/types, so simply adding them to the project is not enough.

### Battle start packets are mostly commented out

`Game.Battle.Battle.StartBattle()` sets `BattleState = Active`, but most of the packet-sending code that should tell clients to enter battle is commented out.

### Battle action processing is stubbed

`Game.Battle.Battle.PLayer_BattleAction()` currently has its implementation commented out, so player attack/skill orders are not processed into actual combat packets or damage results.

### Maps do not manage battle instances

`GameMap` has a commented-out `Battles` collection. There is no active map-level loop that owns active battles and calls `Battle.Process()`.

### NPC battle hooks are missing

NPC/map support is incomplete. `SendMapInfo()` has an empty `Send Npc` region, and the old `SendNpcs()` call is commented. Without map NPCs and an active NPC battle hook, random/NPC battles cannot start in normal gameplay.

## What exists

The repository does include useful pieces:

- `Game.Battle.Battle`
- `Game.Battle.BattleScene`
- `BattleAction`
- fighter interfaces/enums
- partial player battle callback code
- old source for battle-related action codes

These are a starting point, not a finished system.

## Recommended implementation order

1. Port `AC11` and `AC50` to the active `Network.ActionCodes` / `Game` types and add them to `Wonderland Private Server.csproj`.
2. Add a `ConcurrentDictionary<int, Battle>` or similar active-battle collection back to `GameMap`.
3. Implement map methods to start PK and NPC battles, add fighters to each side, call `StartBattle()`, and tick `Battle.Process()`.
4. Rebuild `StartBattle()` packets so the client actually enters the battle UI.
5. Implement `PLayer_BattleAction()` and damage/skill resolution.
6. Add NPC spawning/sending first if the goal is NPC/random encounters.
7. Add GM-only battle test commands after a GM permission gate exists.

Until those pieces are done, expect chat/warp/item commands to work but battle interactions to fail or do nothing.
