# LethalHUDMessages

A BepInEx mod for Lethal Company that displays randomized death, monster encounter, and event messages using the game's built-in HUD notifications instead of chat.

## Features

- **Death Messages** — Randomized messages for all 16 causes of death and 21+ specific monsters
- **Monster Encounter Messages** — Alerts for non-lethal encounters (grabs, haunts, hits, behavior changes) with 15-second cooldown
- **Event Messages** — Critical damage, ship leaving, vote to leave, teleporter use, quota fulfilled, turret firing
- **Two Display Styles:**
  - **Tip** (default) — Bottom-right popup with "DEATH" or "EVENT" header
  - **GlobalNotification** — Bottom-center banner with color-coded backgrounds (red for deaths, yellow for events)
- **Message Queue** — Messages display for a minimum of 4 seconds each; rapid messages queue up instead of overwriting
- **Modded Enemy Support** — Generic fallback messages for custom/modded enemies
- **Enemy Blacklist** — Filter out enemies you don't want messages for
- **Debug Test Mode** — Press F8-F11 to cycle through message types without needing to die or encounter monsters

All messages are **host/server only** and use the game's native notification system (no custom UI).

## Configuration

Config file location:
```
BepInEx/config/com.github.luckofthelefty.LethalHUDMessages.cfg
```

| Section | Setting | Default | Description |
|---------|---------|---------|-------------|
| Display | NotificationStyle | `Tip` | `Tip` = bottom-right popup. `GlobalNotification` = bottom-center banner. |
| Debug | EnableTestMode | `false` | Enable test hotkeys (F8-F11). |
| Player Events | CriticalDamageMessages | `false` | Alert when a player's health drops to critical. |
| Ship Events | ShipLeavingMessages | `false` | Alert when the ship is leaving. |
| Ship Events | VoteToLeaveMessages | `false` | Alert when someone votes to leave early. |
| Ship Events | TeleporterMessages | `false` | Alert when the teleporter is used. |
| Ship Events | QuotaFulfilledMessages | `false` | Alert when the profit quota is met. |
| Monster Encounters | MonsterEncounterMessages | `true` | Show non-lethal monster encounter messages. |
| Monster Encounters | CustomEnemyEncounterMessages | `true` | Show fallback messages for modded/custom enemies. |
| Monster Encounters | EnemyBlacklist | *(empty)* | Comma-separated enemy names to ignore. |

## Test Mode

With `EnableTestMode = true`, use these hotkeys in-game:

| Key | Action |
|-----|--------|
| F8 | Cycle through death messages (by cause of death) |
| F9 | Cycle through monster kill messages |
| F10 | Cycle through monster encounter messages |
| F11 | Cycle through event messages |

## GlobalNotification Colors

When using the `GlobalNotification` display style, the banner background changes based on message type:

| Type | Background | Text |
|------|-----------|------|
| Death | Red | White |
| Event | Yellow | Black |

## Installation

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx) for Lethal Company
2. Place `com.github.luckofthelefty.LethalHUDMessages.dll` in `BepInEx/plugins/`
3. Launch the game

No other mods are required.

## Building

```bash
dotnet restore
dotnet build
```

Output: `LethalHUDMessages/bin/Debug/netstandard2.1/com.github.luckofthelefty.LethalHUDMessages.dll`

## Support

This mod is provided as-is. **Support is limited to none.** Feel free to fork and modify for your own use.

## License

MIT
