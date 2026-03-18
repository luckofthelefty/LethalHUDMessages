using System;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalHUDMessages.Messages;

internal static class EventMessages
{
    private static readonly Random _rng = new Random();

    // Player Events
    private static readonly List<string> CriticalDamage = new List<string>
    {
        "$$ is critically injured!",
        "$$ is barely hanging on!",
        "$$ is one hit from death!",
        "$$'s health is critical!",
        "$$ needs help. NOW.",
        "$$ is clinging to life!"
    };

    // Ship Events
    private static readonly List<string> ShipLeaving = new List<string>
    {
        "The ship is leaving! Run!",
        "Ship's departing! Move it or lose it!",
        "Last call! The ship is out of here!",
        "The autopilot doesn't wait. The ship is leaving!",
        "Ship is leaving! Every employee for themselves!",
        "LEAVE. NOW. SHIP. GOING."
    };

    private static readonly List<string> VoteToLeave = new List<string>
    {
        "Someone wants out. Can't blame them.",
        "A vote to leave has been cast. Coward or genius?",
        "Someone's had enough of this moon.",
        "Vote to leave! Someone chose life.",
        "An employee has requested immediate departure.",
        "Someone would like to leave. Understandable."
    };

    private static readonly List<string> Teleporter = new List<string>
    {
        "The teleporter was activated!",
        "Someone just got beamed up!",
        "Teleporter fired! Hope they made it.",
        "Beam me up! The teleporter was used.",
        "The teleporter just went off!",
        "Someone's taking the express route back."
    };

    private static readonly List<string> InverseTeleporter = new List<string>
    {
        "The inverse teleporter was used! Good luck in there.",
        "Someone just got sent into the facility blind!",
        "Inverse teleporter activated! Hope they packed lunch.",
        "The inverse teleporter fired! Godspeed.",
        "Someone volunteered for a random drop. Brave.",
        "Inverse teleporter! Destination: probably death."
    };

    private static readonly List<string> QuotaFulfilled = new List<string>
    {
        "Quota met! The Company is pleased... for now.",
        "Profit quota fulfilled! You live another cycle.",
        "The Company acknowledges your adequate performance.",
        "Quota complete! The Company demands more next time.",
        "You met the quota! Don't celebrate. There's always more.",
        "The shareholders are satisfied. Temporarily."
    };

    // Monster Encounters
    private static readonly List<string> TurretFiring = new List<string>
    {
        "Turret locked on! Take cover!",
        "The turret is firing! Duck!",
        "Someone's in the turret's crosshairs!",
        "Turret going berserk!",
        "The turret has chosen violence!",
        "TURRET! EVERYONE DOWN!"
    };

    internal static string GetCriticalDamage(string username) =>
        CriticalDamage[_rng.Next(CriticalDamage.Count)].Replace("$$", username);

    internal static string GetShipLeaving() =>
        ShipLeaving[_rng.Next(ShipLeaving.Count)];

    internal static string GetVoteToLeave() =>
        VoteToLeave[_rng.Next(VoteToLeave.Count)];

    internal static string GetTeleporter(bool isInverse) =>
        isInverse
            ? InverseTeleporter[_rng.Next(InverseTeleporter.Count)]
            : Teleporter[_rng.Next(Teleporter.Count)];

    internal static string GetQuotaFulfilled() =>
        QuotaFulfilled[_rng.Next(QuotaFulfilled.Count)];

    internal static string GetTurretFiring() =>
        TurretFiring[_rng.Next(TurretFiring.Count)];
}
