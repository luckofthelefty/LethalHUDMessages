using System.Collections.Generic;
using GameNetcodeStuff;
using HarmonyLib;

namespace com.github.luckofthelefty.LethalHUDMessages.Patches;

internal static class MonsterKillPatch
{
    private static readonly Dictionary<int, string> _recentMonsterKills = new Dictionary<int, string>();

    internal static void RegisterMonsterKill(int playerId, string enemyName)
    {
        _recentMonsterKills[playerId] = enemyName;
    }

    internal static bool TryConsumeMonsterKill(int playerId, out string enemyName)
    {
        if (_recentMonsterKills.TryGetValue(playerId, out enemyName))
        {
            _recentMonsterKills.Remove(playerId);
            return true;
        }
        enemyName = null;
        return false;
    }

    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationClientRpc))]
    [HarmonyPostfix]
    private static void BrackenKill(int playerObjectId) => RegisterMonsterKill(playerObjectId, "Flowerman");

    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void JesterKill(int playerId) => RegisterMonsterKill(playerId, "Jester");

    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void GiantKill(int playerId) => RegisterMonsterKill(playerId, "ForestGiant");

    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void EyelessDogKill(int playerId) => RegisterMonsterKill(playerId, "MouthDog");

    [HarmonyPatch(typeof(BlobAI), nameof(BlobAI.SlimeKillPlayerEffectClientRpc))]
    [HarmonyPostfix]
    private static void BlobKill(int playerKilled) => RegisterMonsterKill(playerKilled, "Blob");

    [HarmonyPatch(typeof(BaboonBirdAI), nameof(BaboonBirdAI.StabPlayerDeathAnimClientRpc))]
    [HarmonyPostfix]
    private static void BaboonHawkKill(int playerObject) => RegisterMonsterKill(playerObject, "BaboonHawk");

    [HarmonyPatch(typeof(ButlerEnemyAI), nameof(ButlerEnemyAI.StabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void ButlerKill(int playerId) => RegisterMonsterKill(playerId, "Butler");

    [HarmonyPatch(typeof(ClaySurgeonAI), nameof(ClaySurgeonAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void BarberKill(ClaySurgeonAI __instance)
    {
        var player = __instance.targetPlayer;
        if (player == null) return;
        int playerId = (int)player.playerClientId;
        RegisterMonsterKill(playerId, "ClaySurgeon");
    }

    [HarmonyPatch(typeof(RadMechAI), nameof(RadMechAI.GrabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void OldBirdKill(int playerId) => RegisterMonsterKill(playerId, "RadMech");
}
