using com.github.luckofthelefty.LethalHUDMessages.Messages;
using GameNetcodeStuff;
using HarmonyLib;

namespace com.github.luckofthelefty.LethalHUDMessages.Patches;

internal static class EventPatch
{
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayerClientRpc))]
    [HarmonyPostfix]
    private static void DamagePlayer(PlayerControllerB __instance, int damageNumber)
    {
        if (!NetworkUtils.ShouldProcess($"damage_{__instance.GetInstanceID()}")) return;
        if (!ConfigManager.CriticalDamageMessages.Value) return;
        if (__instance == null || __instance.isPlayerDead) return;

        if (__instance.health > 20) return;

        string playerName = __instance.playerUsername ?? "Unknown";
        string message = EventMessages.GetCriticalDamage(playerName);
        MessageSender.Send(message, MessageTier.Event);
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ShipLeaveAutomatically))]
    [HarmonyPostfix]
    private static void ShipLeaving()
    {
        if (!ConfigManager.ShipLeavingMessages.Value) return;

        string message = EventMessages.GetShipLeaving();
        MessageSender.Send(message, MessageTier.Event);
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.VoteShipToLeaveEarly))]
    [HarmonyPostfix]
    private static void VoteToLeave()
    {
        if (!ConfigManager.VoteToLeaveMessages.Value) return;

        string message = EventMessages.GetVoteToLeave();
        MessageSender.Send(message, MessageTier.Event);
    }

    [HarmonyPatch(typeof(ShipTeleporter), nameof(ShipTeleporter.PressTeleportButtonClientRpc))]
    [HarmonyPostfix]
    private static void TeleporterUsed(ShipTeleporter __instance)
    {
        if (!NetworkUtils.ShouldProcess($"teleporter_{__instance.GetInstanceID()}")) return;
        if (!ConfigManager.TeleporterMessages.Value) return;

        string message = EventMessages.GetTeleporter(__instance.isInverseTeleporter);
        MessageSender.Send(message, MessageTier.Event);
    }

    [HarmonyPatch(typeof(TimeOfDay), nameof(TimeOfDay.SyncNewProfitQuotaClientRpc))]
    [HarmonyPostfix]
    private static void QuotaFulfilled()
    {
        if (!ConfigManager.QuotaFulfilledMessages.Value) return;

        string message = EventMessages.GetQuotaFulfilled();
        MessageSender.Send(message, MessageTier.Event);
    }
}
