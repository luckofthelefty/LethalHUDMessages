using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalHUDMessages;

internal enum DisplayStyle
{
    Tip,
    GlobalNotification
}

internal static class ConfigManager
{
    // Display
    internal static ConfigEntry<DisplayStyle> NotificationStyle { get; private set; }

    // Player Events
    internal static ConfigEntry<bool> CriticalDamageMessages { get; private set; }

    // Ship Events
    internal static ConfigEntry<bool> ShipLeavingMessages { get; private set; }
    internal static ConfigEntry<bool> VoteToLeaveMessages { get; private set; }
    internal static ConfigEntry<bool> TeleporterMessages { get; private set; }
    internal static ConfigEntry<bool> QuotaFulfilledMessages { get; private set; }

    // Debug
    internal static ConfigEntry<bool> EnableTestMode { get; private set; }

    // Monster Encounters
    internal static ConfigEntry<bool> MonsterEncounterMessages { get; private set; }
    internal static ConfigEntry<bool> CustomEnemyEncounterMessages { get; private set; }
    internal static ConfigEntry<string> EnemyBlacklist { get; private set; }

    private static HashSet<string> _blacklistCache;

    internal static bool IsEnemyBlacklisted(string enemyName)
    {
        if (_blacklistCache == null) RebuildBlacklistCache();
        return _blacklistCache.Contains(enemyName.ToLowerInvariant());
    }

    private static void RebuildBlacklistCache()
    {
        _blacklistCache = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (EnemyBlacklist?.Value == null) return;

        foreach (string entry in EnemyBlacklist.Value.Split(','))
        {
            string trimmed = entry.Trim();
            if (trimmed.Length > 0)
                _blacklistCache.Add(trimmed.ToLowerInvariant());
        }
    }

    internal static void Initialize(ConfigFile config)
    {
        // Display
        NotificationStyle = config.Bind(
            "Display", "NotificationStyle", DisplayStyle.Tip,
            "How to display messages. Tip = bottom-right popup with header. GlobalNotification = blue banner at bottom-center.");

        // Debug
        EnableTestMode = config.Bind(
            "Debug", "EnableTestMode", false,
            "Enable test mode. F8=Death, F9=Monster Kill, F10=Monster Encounter, F11=Events.");

        // Player Events
        CriticalDamageMessages = config.Bind(
            "Player Events", "CriticalDamageMessages", false,
            "Show a message when a player takes critical damage.");

        // Ship Events
        ShipLeavingMessages = config.Bind(
            "Ship Events", "ShipLeavingMessages", false,
            "Show a message when the ship is leaving.");

        VoteToLeaveMessages = config.Bind(
            "Ship Events", "VoteToLeaveMessages", false,
            "Show a message when someone votes to leave.");

        TeleporterMessages = config.Bind(
            "Ship Events", "TeleporterMessages", false,
            "Show a message when the teleporter is used.");

        QuotaFulfilledMessages = config.Bind(
            "Ship Events", "QuotaFulfilledMessages", false,
            "Show a message when the profit quota is met.");

        // Monster Encounters
        MonsterEncounterMessages = config.Bind(
            "Monster Encounters", "MonsterEncounterMessages", true,
            "Show messages when players encounter monsters.");

        CustomEnemyEncounterMessages = config.Bind(
            "Monster Encounters", "CustomEnemyEncounterMessages", true,
            "Show generic encounter messages for modded/unknown enemies.");

        EnemyBlacklist = config.Bind(
            "Monster Encounters", "EnemyBlacklist", "",
            "Comma-separated list of enemy names to suppress messages for.");
    }
}
