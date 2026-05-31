# Running Wonderland Private Server

This repository is an old Windows/.NET Framework private-server control app for Wonderland Online. It can be made buildable, but a clean checkout is not a complete playable server by itself because the game client and some runtime game data/map assets are not included.

## What this app is

- Main app: `Wonderland Private Server.exe`, a Windows Forms server-control program.
- Target framework: .NET Framework 4.5.
- Main listener: TCP `0.0.0.0:6414`.
- Client protocol: classic Wonderland Online PC protocol. The login handler rejects clients whose login version is below `1096`.
- Map loading: compiled map plugin DLLs from the runtime `Maps` folder.

## What you need

### Build machine

Use Windows or a Windows VM with:

1. Visual Studio 2012 or newer. Visual Studio 2015/2017 is usually the easiest fit for this solution format.
2. .NET Framework 4.5 targeting pack/developer pack.
3. NuGet package restore enabled.
4. Optional but useful: MySQL/MariaDB server if you choose MySQL mode.

### Runtime files

Place these next to the built EXE, unless Visual Studio copies them for you:

- `DLLS/PhoenixData.dll`
- `DLLS/Phoenix.Core.dll`
- `DLLS/RCLibrary.dll`
- `DLLS/Wlo.Core.dll`
- `DLLS/GupdtSrv.dll`
- `x86/SQLite.Interop.dll`
- `x64/SQLite.Interop.dll`

The repository already contains those DLL files. The project file also references the NuGet packages listed in `packages.config`.

### Runtime game data not included here

You must provide:

- `Data/itemDat.wpdat`
- Map plugin DLLs under `Maps/`
- A compatible classic Wonderland Online PC client patched/configured to connect to your server IP on TCP port `6414`

Without `Data/itemDat.wpdat`, startup fails very early. Without map DLLs, character login will fail when the player is placed on the login map.

## Database setup

The server has a database configuration model with these connection fields:

- `User`
- `Pass`
- `DataBase`
- `Port`
- `ServerIP`
- `Server_Type`

It also has user-table mapping fields:

- `TableName_Ref` defaults conceptually to `user`
- `Username_Ref` defaults conceptually to `username`
- `Password_Ref` defaults conceptually to `password`
- `UserID_Ref` defaults conceptually to `userID`
- `CharacterID1_Ref` defaults conceptually to `character1ID`
- `CharacterID2_Ref` defaults conceptually to `character2ID`
- `IM_Ref` defaults conceptually to `IM`
- `Char_Delete_Code_Ref` defaults conceptually to `char_delete_code`
- `PassVerification`: `0` for normal password verification, `1` for IPBoard 3.x-style salted hashes

Settings are loaded from:

```text
%APPDATA%\PServer\Config.settings.wlo
```

The code can verify/create several character/gameplay tables, but it expects an account/user table to already exist and contain login accounts.

## Build steps

1. Open `Wonderland Private Server.sln` in Visual Studio.
2. Restore NuGet packages if prompted.
3. Build the solution in `Debug|Any CPU` first.
4. If Visual Studio complains about missing references, point the references at the DLLs in this repository's `DLLS` directory.
5. Copy/provide `Data/itemDat.wpdat` into the output directory, e.g. `bin/Debug/Data/itemDat.wpdat`.
6. Copy/provide compiled map DLLs into `bin/Debug/Maps/`.
7. Configure `%APPDATA%\PServer\Config.settings.wlo` or use the UI if it successfully saves settings.
8. Run `bin/Debug/Wonderland Private Server.exe`.
9. Configure the client to connect to your server IP on TCP port `6414`.

## Preflight helper

From PowerShell on Windows, run:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/preflight.ps1
```

The script checks for common missing files and tools, but it cannot supply missing game data or client files.

## Known limitations

- The repo does not include a Wonderland Online client.
- The repo does not include `Data/itemDat.wpdat`.
- The repo does not include ready-to-use compiled map plugin DLLs in the runtime `Maps` folder.
- The codebase is old and depends on .NET Framework/WinForms, so modern cross-platform `dotnet run` is not the right path.
- Some code paths are incomplete/commented out; expect additional debugging after the project first builds.

## GM / developer commands

See `docs/GM_COMMANDS.md` for the current command parser. At the moment, the active commands are developer-style `:item add <itemId> [amount]` and `:warp <mapId> <x> <y>` commands; there is not yet a full permissioned GM system or NPC-spawn command.

## Troubleshooting the startup log

### `Connection not successful unable to authenticate users connecting to server`

This is a database configuration problem, not an `itemDat` problem. The server has loaded the data files and then failed `UserDatabase.TestConnection()`.

Check `%APPDATA%\PServer\Config.settings.wlo` and make sure the `DB` section points at a real MySQL/MariaDB or SQLite database. For MySQL/MariaDB the common values are:

- `ServerIP`: `127.0.0.1` when the database runs on the same machine
- `Port`: `3306`
- `User`: the database user, for example `root` or a dedicated `wlo` user
- `Pass`: that database user's password
- `DataBase`: the schema/database name, for example `wonderland`
- `Server_Type`: MySQL mode, usually serialized by the app as the enum value for `MySQl`

The account table must exist before players can log in. By default the code expects a table like `user` with columns matching `username`, `password`, `userID`, `character1ID`, `character2ID`, `IM`, and `char_delete_code`. For a local MySQL/MariaDB test database, start with `sql/mysql_user_schema.sql`, then update or remove the development-only test account before exposing the server.

### `Git update check failed` or `An error occurred while sending the request`

That message comes from the Git updater, not from the game database. Update checks are non-essential for running the server. The current code defaults update checks to `Never` and logs update failures as non-fatal warnings so they do not hide the real database result.

### `[PluginSystem][Info] - Found 0 Maps`

The server did not find compiled map plugin DLLs in the runtime `Maps` folder. You can still bind the login port, but character login will fail when the world server tries to place the player on a map. Place compiled map DLLs under `Maps/` beside the server EXE.
