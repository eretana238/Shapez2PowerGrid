# Development

Windows-focused setup below. For other platforms, follow the [Shapez 2 modding documentation](https://tobspr-games.notion.site/shapez2-modding-documentation).

## Prerequisites

- [Shapez 2](https://store.steampowered.com/app/2162800/shapez_2/) (PC, Windows)
- [.NET SDK](https://dotnet.microsoft.com/download) — targets `netstandard2.1`
- [Shapez Shifter](https://steamcommunity.com/sharedfiles/filedetails/?id=3542611357) (Steam Workshop) — required dependency
- [HarmonyX](https://steamcommunity.com/sharedfiles/filedetails/?id=3720742050) (Steam Workshop) — required dependency

## Environment Variables

The `.csproj` references game and Shifter assemblies via environment variables instead of NuGet. Set all three before building or opening the solution in an IDE.

**Official guide:** [Shapez 2 modding documentation](https://tobspr-games.notion.site/shapez2-modding-documentation) — canonical steps for finding paths, IDE setup, and first build.

| Variable | Purpose |
|---|---|
| `SPZ2_PATH` | Game install folder containing `SPZGameAssembly.dll` and other assemblies referenced in `PowerGrid.csproj` |
| `SPZ2_PERSISTENT` | Persistent data root — typically `%USERPROFILE%\AppData\LocalLow\tobspr Games\shapez 2`. `OutputPath` in the `.csproj` writes the built mod to `mods\Powergrid\` under this folder |
| `SPZ2_SHIFTER` | Full path to `ShapezShifter.dll` from the subscribed Shapez Shifter Workshop item |

Example (**Windows** User variables — adjust paths to your install):

```powershell
[System.Environment]::SetEnvironmentVariable("SPZ2_PATH", "C:\Program Files (x86)\Steam\steamapps\common\shapez 2", "User")
[System.Environment]::SetEnvironmentVariable("SPZ2_PERSISTENT", "$env:USERPROFILE\AppData\LocalLow\tobspr Games\shapez 2", "User")
[System.Environment]::SetEnvironmentVariable("SPZ2_SHIFTER", "C:\Program Files (x86)\Steam\steamapps\workshop\content\2162800\3542611357\ShapezShifter.dll", "User")
```

Restart your terminal and IDE after setting variables. If references fail to resolve, verify each path exists on disk and matches the [official modding docs](https://tobspr-games.notion.site/shapez2-modding-documentation).

## Build

```powershell
dotnet build PowerGrid.csproj -c Release
```

After a successful build, enable **PowerGrid** in the in-game mod list and restart if prompted.

## Tooling

- **Shapez Shifter** — recommended API layer for buildings, patching, and backwards compatibility.
- Add assembly ref via `SPZ2_SHIFTER`. Dependency is declared in `manifest.json`.
- **Publicizer** (`Krafs.Publicizer` in `.csproj`) exposes private/internal game members. Log output: `logs/krafs.log` (gitignored).
- Sample mods: [shapez2-mod-samples](https://github.com/tobspr-games/shapez2-mod-samples) — DiagonalCutter, BiggerPlatforms, SandboxIslands.

## Project Layout

```text
PowerGrid/
├── Power/              Network manager, producers, consumers, profiles
├── MicroReactor/       First generator simulation
├── Rendering/          Building renderers and belt lane draw data
├── Hud/                Player power HUD and HUD patches
├── UI/                 Custom UI factory
├── Toolbar/            Toolbar tab integration
├── Resources/          FBX meshes, icons (copied to output)
├── manifest.json       Mod metadata and dependencies
├── translations.json   In-game strings
└── Docs/               Design, roadmap, and reference docs
```

## Publishing

```xml
<!-- In .csproj -->
<Target Name="SteamPublish">
  <Exec Command='sh .\Steam\SteamPublish.sh "$(OutputPath)'/>
</Target>
```

```powershell
dotnet msbuild .\PowerGrid.csproj -t:SteamPublish -v:detailed
```

- Script requires `preview.png` in the Steam publish folder.
- Add mod dependencies on the Steam Workshop page so users get the "Subscribe to All" prompt.

## Common Errors

| Error | Fix |
|---|---|
| No `IMod` implementation found | Add class implementing `IMod`, or ignore if no init needed |
| Assembly loaded twice | Remove duplicate local + Steam copy of same mod |
| Mods not in list | Validate game files, restart Steam, re-subscribe |
| Unresolved references | Check env vars; restart IDE; use Rider's "Diagnose Reference" |

See also [Modding Reference](modding-reference.md) for Shapez 2 architecture and resource loading, and the [official Shapez 2 modding documentation](https://tobspr-games.notion.site/shapez2-modding-documentation) for environment setup and publishing.
