using com.github.luckofthelefty.LethalHUDMessages.Messages;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace com.github.luckofthelefty.LethalHUDMessages.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
internal static class DeathPatch
{
    [HarmonyPatch(nameof(PlayerControllerB.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void KillPlayerClientRpcPatch(PlayerControllerB __instance, int playerId, int causeOfDeath)
    {
        if (!NetworkUtils.ShouldProcess($"death_{playerId}")) return;

        var playerScript = StartOfRound.Instance?.allPlayerScripts != null
            && playerId >= 0
            && playerId < StartOfRound.Instance.allPlayerScripts.Length
                ? StartOfRound.Instance.allPlayerScripts[playerId]
                : __instance;

        if (playerScript == null || !playerScript.isPlayerDead) return;

        string playerName = playerScript.playerUsername ?? "Unknown";

        if (MonsterKillPatch.TryConsumeMonsterKill(playerId, out string enemyName))
        {
            if (!ConfigManager.IsEnemyBlacklisted(enemyName))
            {
                string monsterMsg = MonsterMessages.GetDeathMessage(enemyName, playerName);
                MessageSender.Send(monsterMsg);
                return;
            }
        }

        string attackingEnemy = FindAttackingEnemy(playerScript);
        if (attackingEnemy != null && !ConfigManager.IsEnemyBlacklisted(attackingEnemy))
        {
            string monsterMsg = MonsterMessages.GetDeathMessage(attackingEnemy, playerName);
            MessageSender.Send(monsterMsg);
            return;
        }

        string deathMsg = DeathMessages.Get((CauseOfDeath)causeOfDeath, playerName);
        MessageSender.Send(deathMsg);
    }

    private static string FindAttackingEnemy(PlayerControllerB playerScript)
    {
        if (playerScript.inAnimationWithEnemy != null)
            return GetEnemyName(playerScript.inAnimationWithEnemy);

        var enemies = Object.FindObjectsOfType<EnemyAI>();
        if (enemies == null) return null;

        foreach (var enemy in enemies)
        {
            if (enemy?.enemyType == null) continue;

            if (enemy.inSpecialAnimationWithPlayer == playerScript)
                return GetEnemyName(enemy);

            if (enemy.targetPlayer == playerScript && enemy.movingTowardsTargetPlayer)
                return GetEnemyName(enemy);
        }

        return null;
    }

    private static string GetEnemyName(EnemyAI enemy)
    {
        if (enemy?.enemyType == null) return null;
        return enemy.enemyType.enemyName ?? enemy.GetType().Name;
    }
}
