using com.github.luckofthelefty.LethalHUDMessages.Messages;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace com.github.luckofthelefty.LethalHUDMessages.Patches;

[HarmonyPatch]
internal static class DiscoveryPatch
{
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.SwitchToBehaviourClientRpc))]
    [HarmonyPostfix]
    private static void EnemyBehaviorChanged(EnemyAI __instance, int stateIndex)
    {
        if (__instance == null) return;
        if (!NetworkUtils.ShouldProcess($"discovery_{__instance.GetInstanceID()}_{stateIndex}")) return;

        var netObj = __instance.GetComponent<NetworkObject>();
        if (netObj == null) return;

        ulong id = netObj.NetworkObjectId;

        if (stateIndex >= 1)
        {
            bool wasAlreadyDiscovered = DiscoveryTracker.IsDiscovered(id);
            DiscoveryTracker.MarkDiscovered(id);

            if (!wasAlreadyDiscovered && ConfigManager.MonsterEncounterMessages.Value && ConfigManager.CustomEnemyEncounterMessages.Value)
            {
                string enemyName = __instance.enemyType?.enemyName ?? __instance.GetType().Name;

                if (ConfigManager.IsEnemyBlacklisted(enemyName)) return;

                if (!MonsterMessages.HasEncounterPool(enemyName))
                {
                    string playerName = GetNearestPlayerName(__instance);

                    string message = MonsterMessages.GetEncounterMessage(enemyName, playerName);
                    if (message != null)
                    {
                        MessageSender.Send(message, MessageTier.Event);
                    }
                }
            }
        }
    }

    private static string GetNearestPlayerName(EnemyAI enemy)
    {
        if (enemy.targetPlayer != null)
            return enemy.targetPlayer.playerUsername ?? "Unknown";

        if (enemy.inSpecialAnimationWithPlayer != null)
            return enemy.inSpecialAnimationWithPlayer.playerUsername ?? "Unknown";

        if (StartOfRound.Instance?.allPlayerScripts != null)
        {
            float closestDist = float.MaxValue;
            PlayerControllerB closest = null;

            foreach (var player in StartOfRound.Instance.allPlayerScripts)
            {
                if (player == null || player.isPlayerDead || !player.isPlayerControlled) continue;

                float dist = Vector3.Distance(enemy.transform.position, player.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = player;
                }
            }

            if (closest != null)
                return closest.playerUsername ?? "Unknown";
        }

        return "someone";
    }

    [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ReviveDeadPlayers))]
    [HarmonyPostfix]
    private static void ResetOnNewRound()
    {
        DiscoveryTracker.Reset();
        MonsterEncounterPatch.ResetCooldowns();
    }
}
