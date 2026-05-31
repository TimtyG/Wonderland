# GM / Developer Commands

This codebase has a very small command parser, but it is not a complete GM system.

## Where commands are handled

Runtime chat/developer commands are handled in `Src/Network/ActionCodes/AC02.cs` when the client sends action code `2`, sub-action `2`. The handler reads a string, splits it on spaces, and switches on the first word.

## Commands currently implemented

### Add an item

```text
:item add <itemId> [amount]
```

Examples:

```text
:item add 34076
:item add 1001 5
```

Notes:

- `itemId` is parsed as a `UInt16`.
- `amount` is optional and defaults to `1`.
- Item `34076` is special-cased so the player only receives it if it is not already in inventory/equipped.

### Start a test battle

```text
:battle test
```

Aliases:

```text
:battle start
```

Notes:

- Starts a one-player training battle against a dummy NPC fighter.
- This is a developer smoke-test command for the battle packet/action flow; it is not a full NPC encounter system.
- There is still no active GM permission check, so keep this on a private/dev server until a permission gate is added.

### Warp yourself

```text
:warp <mapId> <x> <y>
```

Example:

```text
:warp 60000 1000 1000
```

Notes:

- `mapId`, `x`, and `y` are parsed as `UInt16`.
- The command teleports the command sender through their current map's teleport flow.
- The destination map still has to be available to the server. If no map plugin DLL exists for the target map, the warp will not be useful.

## What is not implemented

There is no complete permissioned GM command framework in the active code.

Important limitations:

- No active GM rank check is performed before `:item` or `:warp` runs.
- The `User.GMlvl` field exists, but the getter is private and the active player-level `GM` property is commented out.
- The old `GmBot`/`Cupid` bot classes are commented out.
- NPC spawning is not implemented as a command. Map/NPC support is plugin/code driven, and `SendMapInfo` still has its NPC send section empty/commented.
- There is a `GMRiceBall(UInt16 npcid)` helper in the old RiceBall code, but the player RiceBall property is commented out, so it is not currently exposed as a working command.

## Recommended next step before exposing commands

Before using these commands on a public server, add a real permission gate. A safe approach is:

1. Add a public `IsGM` or `GMLevel` getter to `User`/`Player`.
2. Load that value from your account database.
3. In `AC02.Recv2`, reject commands beginning with `:` unless the player is a GM.
4. Log all successful command usage.

Until that exists, treat `:item` and `:warp` as developer/test commands only.
