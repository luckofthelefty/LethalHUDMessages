using com.github.luckofthelefty.LethalHUDMessages.Messages;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using Unity.Netcode;

namespace com.github.luckofthelefty.LethalHUDMessages.Patches;

internal static class MonsterEncounterPatch
{
    private static readonly Dictionary<ulong, float> _cooldowns = new Dictionary<ulong, float>();
    private const float CooldownSeconds = 15f;

    private static bool IsOnCooldown(ulong networkObjectId)
    {
        if (_cooldowns.TryGetValue(networkObjectId, out float lastTime))
        {
            if (UnityEngine.Time.time - lastTime < CooldownSeconds)
                return true;
        }
        return false;
    }

    private static void SetCooldown(ulong networkObjectId)
    {
        _cooldowns[networkObjectId] = UnityEngine.Time.time;
    }

    internal static void ResetCooldowns()
    {
        _cooldowns.Clear();
    }

    private static bool CanSendEncounter(EnemyAI enemy)
    {
        if (!ConfigManager.MonsterEncounterMessages.Value) return false;
        if (enemy == null) return false;
        if (!NetworkUtils.ShouldProcess($"encounter_{enemy.GetInstanceID()}")) return false;

        var netObj = enemy.GetComponent<NetworkObject>();
        if (netObj == null) return false;

        ulong id = netObj.NetworkObjectId;
        if (!DiscoveryTracker.IsDiscovered(id)) return false;
        if (IsOnCooldown(id)) return false;

        return true;
    }

    private static void SendEncounter(EnemyAI enemy, string playerName)
    {
        string enemyName = enemy.enemyType?.enemyName ?? enemy.GetType().Name;
        string message = MonsterMessages.GetEncounterMessage(enemyName, playerName);
        if (message == null) return;

        var netObj = enemy.GetComponent<NetworkObject>();
        if (netObj != null) SetCooldown(netObj.NetworkObjectId);

        MessageSender.Send(message, MessageTier.Event);
    }

    private static string GetPlayerName(int playerObjectIndex)
    {
        if (StartOfRound.Instance?.allPlayerScripts == null
            || playerObjectIndex < 0
            || playerObjectIndex >= StartOfRound.Instance.allPlayerScripts.Length)
            return "Unknown";

        return StartOfRound.Instance.allPlayerScripts[playerObjectIndex]?.playerUsername ?? "Unknown";
    }

    private static string GetPlayerName(PlayerControllerB playerScript)
    {
        return playerScript?.playerUsername ?? "Unknown";
    }

    private static string GetTargetPlayerName(EnemyAI enemy)
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

                float dist = UnityEngine.Vector3.Distance(enemy.transform.position, player.transform.position);
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

    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationClientRpc))]
    [HarmonyPostfix]
    private static void BrackenGrab(FlowermanAI __instance, int playerObjectId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerObjectId));
    }

    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.SetJesterInitialValues))]
    [HarmonyPostfix]
    private static void JesterCranking(JesterAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    [HarmonyPatch(typeof(SpringManAI), nameof(SpringManAI.SetAnimationGoClientRpc))]
    [HarmonyPostfix]
    private static void CoilheadMoving(SpringManAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.CreateMimicClientRpc))]
    [HarmonyPostfix]
    private static void MaskedMimic(MaskedPlayerEnemy __instance)
    {
        if (!CanSendEncounter(__instance)) return;

        string playerName = __instance.mimickingPlayer != null
            ? GetPlayerName(__instance.mimickingPlayer)
            : GetTargetPlayerName(__instance);

        SendEncounter(__instance, playerName);
    }

    [HarmonyPatch(typeof(CentipedeAI), nameof(CentipedeAI.ClingToPlayerClientRpc))]
    [HarmonyPostfix]
    private static void SnareFleaCling(CentipedeAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;

        string playerName = __instance.clingingToPlayer != null
            ? GetPlayerName(__instance.clingingToPlayer)
            : GetTargetPlayerName(__instance);

        SendEncounter(__instance, playerName);
    }

    [HarmonyPatch(typeof(SandSpiderAI), nameof(SandSpiderAI.PlayerTripWebClientRpc))]
    [HarmonyPostfix]
    private static void SpiderWebTrip(SandSpiderAI __instance, int playerNum)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerNum));
    }

    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void GiantGrab(ForestGiantAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    [HarmonyPatch(typeof(CrawlerAI), nameof(CrawlerAI.HitPlayerClientRpc))]
    [HarmonyPostfix]
    private static void ThumperHit(CrawlerAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    [HarmonyPatch(typeof(BaboonBirdAI), nameof(BaboonBirdAI.StabPlayerDeathAnimClientRpc))]
    [HarmonyPostfix]
    private static void BaboonHawkStab(BaboonBirdAI __instance, int playerObject)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerObject));
    }

    [HarmonyPatch(typeof(DressGirlAI), nameof(DressGirlAI.ChooseNewHauntingPlayerClientRpc))]
    [HarmonyPostfix]
    private static void GhostGirlHaunt(DressGirlAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;

        string playerName = __instance.hauntingPlayer != null
            ? GetPlayerName(__instance.hauntingPlayer)
            : GetTargetPlayerName(__instance);

        SendEncounter(__instance, playerName);
    }

    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void EyelessDogEncounter(MouthDogAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    [HarmonyPatch(typeof(BlobAI), nameof(BlobAI.SlimeKillPlayerEffectClientRpc))]
    [HarmonyPostfix]
    private static void BlobContact(BlobAI __instance, int playerKilled)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerKilled));
    }

    [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.FireGunClientRpc))]
    [HarmonyPostfix]
    private static void NutcrackerShot(NutcrackerEnemyAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    [HarmonyPatch(typeof(ButlerEnemyAI), nameof(ButlerEnemyAI.StabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void ButlerStab(ButlerEnemyAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    [HarmonyPatch(typeof(ClaySurgeonAI), nameof(ClaySurgeonAI.KillPlayerClientRpc))]
    [HarmonyPostfix]
    private static void BarberEncounter(ClaySurgeonAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.SwitchToBehaviourClientRpc))]
    [HarmonyPostfix]
    private static void BehaviourStateEncounter(EnemyAI __instance, int stateIndex)
    {
        if (__instance is CaveDwellerAI && stateIndex >= 1)
        {
            if (!CanSendEncounter(__instance)) return;
            SendEncounter(__instance, GetTargetPlayerName(__instance));
        }
        else if (__instance is HoarderBugAI && stateIndex >= 2)
        {
            if (!CanSendEncounter(__instance)) return;
            SendEncounter(__instance, GetTargetPlayerName(__instance));
        }
        else if (__instance is RedLocustBees && stateIndex >= 1)
        {
            if (!CanSendEncounter(__instance)) return;
            SendEncounter(__instance, GetTargetPlayerName(__instance));
        }
        else if (__instance is ButlerBeesEnemyAI && stateIndex >= 1)
        {
            if (!CanSendEncounter(__instance)) return;
            SendEncounter(__instance, GetTargetPlayerName(__instance));
        }
    }

    [HarmonyPatch(typeof(SandWormAI), nameof(SandWormAI.EmergeClientRpc))]
    [HarmonyPostfix]
    private static void SandWormEmerge(SandWormAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    [HarmonyPatch(typeof(RadMechAI), nameof(RadMechAI.GrabPlayerClientRpc))]
    [HarmonyPostfix]
    private static void OldBirdGrab(RadMechAI __instance, int playerId)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetPlayerName(playerId));
    }

    [HarmonyPatch(typeof(RadMechAI), nameof(RadMechAI.ShootGunClientRpc))]
    [HarmonyPostfix]
    private static void OldBirdShoot(RadMechAI __instance)
    {
        if (!CanSendEncounter(__instance)) return;
        SendEncounter(__instance, GetTargetPlayerName(__instance));
    }

    [HarmonyPatch(typeof(Turret), nameof(Turret.SetToModeClientRpc))]
    [HarmonyPostfix]
    private static void TurretMode(Turret __instance, int mode)
    {
        if (!NetworkUtils.ShouldProcess($"turret_{__instance.GetInstanceID()}")) return;
        if (!ConfigManager.MonsterEncounterMessages.Value) return;

        if (mode < 2) return;

        string message = EventMessages.GetTurretFiring();
        MessageSender.Send(message, MessageTier.Event);
    }
}
