using System;
using System.Collections.Generic;

namespace com.github.luckofthelefty.LethalHUDMessages.Messages;

internal static class MonsterMessages
{
    private static readonly Random _rng = new Random();

    // Death messages — when a specific monster kills a player
    private static readonly Dictionary<string, List<string>> DeathPools = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
    {
        ["Flowerman"] = new List<string>
        {
            "$$ went on a one-way date with the Bracken.",
            "$$ was added to the Bracken's private collection.",
            "The Bracken just wanted a hug. $$ didn't survive it.",
            "$$ shouldn't have looked at the Bracken.",
            "$$ was dragged into the darkness. Bye $$!",
            "$$ found out the Bracken doesn't like eye contact.",
            "The Bracken snapped $$'s neck like a glow stick.",
            "$$ was invited to the Bracken's room. No one comes back."
        },
        ["Jester"] = new List<string>
        {
            "Pop goes $$!",
            "$$ found out what's in the box.",
            "The Jester wasn't joking around with $$.",
            "$$ didn't laugh at the Jester's joke. Fatal mistake.",
            "The music stopped. So did $$.",
            "$$ was the punchline to a very bad joke.",
            "Jack popped out of the box. $$ popped out of existence.",
            "The Jester cranked $$'s life away."
        },
        ["ForestGiant"] = new List<string>
        {
            "$$ was a light snack for the Forest Giant.",
            "The Giant added $$ to their meal plan.",
            "$$ found out they're bite-sized.",
            "$$ was eaten alive. Yikes.",
            "The Forest Giant didn't even chew. Just swallowed $$.",
            "$$ was picked up like a chicken nugget.",
            "The Giant used $$ as a toothpick. Then ate the toothpick.",
            "$$ was the Giant's appetizer."
        },
        ["MouthDog"] = new List<string>
        {
            "$$ was too loud.",
            "The dog didn't need eyes to find $$.",
            "$$ learned that 'be quiet' wasn't a suggestion.",
            "An Eyeless Dog heard $$'s last breath.",
            "$$ forgot the number one rule: shut up.",
            "The Eyeless Dog ate $$ in one bite.",
            "$$'s footsteps were a dinner bell.",
            "$$ whispered. It wasn't quiet enough."
        },
        ["Crawler"] = new List<string>
        {
            "A Thumper ran $$ over like a freight train.",
            "$$ was steamrolled by a Thumper.",
            "The Thumper didn't even slow down for $$.",
            "$$ zigged when they should've zagged. Thumper didn't care.",
            "$$ was in the Thumper's lane. Bad idea.",
            "$$ heard the thumping. Then felt it.",
            "The Thumper used $$ as a speed bump.",
            "$$ was roadkill."
        },
        ["Blob"] = new List<string>
        {
            "$$ was dissolved like a bath bomb.",
            "The Blob absorbed $$. Delicious.",
            "$$ melted into the Hygrodere.",
            "$$ became one with the slime.",
            "The Blob didn't have a mouth, but it ate $$ anyway.",
            "$$ was digested alive. Slowly.",
            "$$ walked into the Blob. Didn't walk out.",
            "The slime claimed $$. Gross."
        },
        ["Centipede"] = new List<string>
        {
            "A Snare Flea suffocated $$ with love.",
            "$$ got a face full of Snare Flea.",
            "The Snare Flea wore $$ like a hat. $$ didn't make it.",
            "$$ was smothered by a ceiling bug.",
            "The Snare Flea hugged $$'s face. Permanently.",
            "$$ looked up. Shouldn't have.",
            "A Snare Flea dropped on $$'s head. Game over.",
            "$$ was facefhugged. Alien style."
        },
        ["SandSpider"] = new List<string>
        {
            "$$ was wrapped up like a burrito by a Spider.",
            "The Spider turned $$ into a cocoon.",
            "$$ walked into a web and never walked out.",
            "$$ became Spider food.",
            "The Spider gift-wrapped $$ for later.",
            "$$ was caught in the Spider's pantry.",
            "$$ got webbed up and chomped.",
            "A Spider made $$ its next meal."
        },
        ["BaboonHawk"] = new List<string>
        {
            "A Baboon Hawk shanked $$.",
            "$$ was jumped by a Baboon Hawk. In broad daylight.",
            "The Baboon Hawk chose violence. $$ was the victim.",
            "$$ got pecked to death. Embarrassing.",
            "A Baboon Hawk stabbed $$ in the back. Literally.",
            "$$ was mugged by a bird monkey.",
            "The Baboon Hawk said 'your scrap or your life.' $$ had scrap.",
            "$$ underestimated the murder bird."
        },
        ["NutcrackerEnemy"] = new List<string>
        {
            "The Nutcracker shot $$ point blank.",
            "$$ caught a shotgun blast from the Nutcracker.",
            "The Nutcracker's aim was true. $$ is dead.",
            "$$ was executed by a toy soldier.",
            "The Nutcracker cracked more than nuts.",
            "$$ was gunned down by a Christmas decoration.",
            "The Nutcracker gave $$ a lead present.",
            "$$ found out the Nutcracker's gun is real."
        },
        ["SpringMan"] = new List<string>
        {
            "$$ blinked. The Coilhead didn't.",
            "The Coilhead caught $$ slipping.",
            "$$ forgot the one rule about Coilheads.",
            "Don't blink. $$ blinked.",
            "The Coilhead spring-loaded $$ into the afterlife.",
            "$$ looked away for one second. One.",
            "The Coilhead was patient. $$ was not.",
            "$$ lost the staring contest with the Coilhead."
        },
        ["MaskedPlayerEnemy"] = new List<string>
        {
            "$$ was killed by their own doppelganger.",
            "The Masked got $$ from behind.",
            "$$ trusted the wrong teammate. It was a Mimic.",
            "A Mimic wearing a friend's face killed $$.",
            "$$ was backstabbed by a copycat.",
            "The Masked fooled everyone. Especially $$.",
            "$$ didn't notice their friend had no personality.",
            "The imposter killed $$. That's pretty sus."
        },
        ["DressGirl"] = new List<string>
        {
            "The Ghost Girl finally caught $$.",
            "$$ couldn't escape the Ghost Girl.",
            "She got $$. It was only a matter of time.",
            "$$ was haunted to death. Literally.",
            "The little girl wasn't so little. $$ is dead.",
            "$$ heard giggling. Then nothing. Forever.",
            "The Ghost Girl played with $$ until they broke.",
            "$$ was the Ghost Girl's favorite toy."
        },
        ["Butler"] = new List<string>
        {
            "The Butler served $$ their last meal.",
            "$$ was stabbed by the help.",
            "The Butler did it. In the facility. With a knife.",
            "$$ got a five-star stabbing from the Butler.",
            "The Butler took $$'s order. It was death.",
            "$$ tipped the Butler. The Butler tipped them over.",
            "The Butler's service was killer. Literally.",
            "$$ found out the Butler doesn't take 'no' for an answer."
        },
        ["ClaySurgeon"] = new List<string>
        {
            "The Barber gave $$ a little off the top. And the middle. And the bottom.",
            "$$ got the closest shave of their life. Their last, too.",
            "The Barber snipped $$ in half. Clean cut.",
            "$$ walked into the Barber's shop. Didn't walk out in one piece.",
            "The Barber performed surgery on $$. Without anesthesia.",
            "$$ was trimmed down to size by the Barber.",
            "The Barber's scissors found $$. Snip snip.",
            "$$ was the Barber's latest masterpiece."
        },
        ["CaveDweller"] = new List<string>
        {
            "The Maneater ate $$. Shocker.",
            "$$ thought the baby was cute. It wasn't a baby.",
            "The Maneater grew up fast. $$ didn't.",
            "$$ adopted a baby. The baby ate them.",
            "The cute little creature devoured $$.",
            "$$ learned that not all babies are innocent.",
            "The Maneater baited $$. Hook, line, and sinker.",
            "$$ was consumed by what they tried to protect."
        },
        ["HoarderBug"] = new List<string>
        {
            "A Hoarding Bug killed $$ over a piece of junk.",
            "$$ touched the Hoarding Bug's stuff. Fatal mistake.",
            "The Hoarding Bug chose violence to protect its hoard.",
            "$$ was murdered by a bug over a lightbulb.",
            "Don't mess with a Hoarding Bug's collection. $$ learned that.",
            "$$ died fighting a bug over loot. Embarrassing.",
            "The Hoarding Bug wasn't sharing with $$.",
            "$$ was killed by the world's angriest hoarder."
        },
        ["SandWorm"] = new List<string>
        {
            "$$ was swallowed whole by an Earth Leviathan.",
            "The ground opened up and ate $$.",
            "$$ didn't hear the rumbling. Now they're underground.",
            "An Earth Leviathan had $$ for lunch.",
            "$$ was consumed by a giant worm. That's a sentence.",
            "The Earth Leviathan came from below. $$ went below.",
            "$$ was in the wrong place when the worm surfaced.",
            "The Leviathan emerged. $$ did not."
        },
        ["RadMech"] = new List<string>
        {
            "An Old Bird stepped on $$. Crunch.",
            "$$ was incinerated by an Old Bird's rockets.",
            "The Old Bird turned $$ into a scorch mark.",
            "$$ was stomped flat by a robot apocalypse.",
            "An Old Bird locked onto $$. $$ is now dust.",
            "$$ picked a fight with a giant mech. It went poorly.",
            "The Old Bird blasted $$ into next week.",
            "$$ was terminated. Old Bird style."
        },
        ["RedLocustBees"] = new List<string>
        {
            "$$ disturbed the hive. The bees disturbed $$'s life.",
            "The Circuit Bees swarmed $$. Buzz buzz, bye bye.",
            "$$ tried to steal honey. Got stung to death.",
            "A swarm of angry bees ended $$'s career.",
            "$$ found out those bees aren't just for show.",
            "The Circuit Bees chose $$ as their target. Ouch.",
            "$$ was stung approximately one million times.",
            "Don't touch the beehive. $$ touched the beehive."
        },
        ["ButlerBees"] = new List<string>
        {
            "The Butler's hornets finished what the Butler started on $$.",
            "$$ was swarmed by Mask Hornets.",
            "The Mask Hornets stung $$ to death.",
            "$$ killed the Butler. The hornets killed $$.",
            "Mask Hornets avenged the Butler by killing $$.",
            "$$ thought killing the Butler was the hard part.",
            "The hornets came for $$. Revenge is sweet.",
            "The Butler's bees made sure $$ didn't celebrate."
        }
    };

    // Encounter messages — non-lethal interactions (Tier 3a)
    private static readonly Dictionary<string, List<string>> EncounterPools = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
    {
        ["Flowerman"] = new List<string>
        {
            "The Bracken has its eyes on $$.",
            "$$ made eye contact with the Bracken. Bold.",
            "The Bracken is stalking $$. Act natural.",
            "$$ felt something breathing on their neck...",
            "The Bracken is watching $$. Don't turn around.",
            "$$ glimpsed something tall in the shadows."
        },
        ["Jester"] = new List<string>
        {
            "The Jester's music box is winding down...",
            "Do you hear that music? $$ does.",
            "Pop goes the... oh no.",
            "The Jester is cranking up. Time to leave.",
            "That music box sounds closer...",
            "The Jester is getting ready for a show."
        },
        ["SpringMan"] = new List<string>
        {
            "$$ looked away from the Coilhead. Bold move.",
            "Don't. Blink.",
            "The Coilhead moved. Someone wasn't watching.",
            "$$ heard a spring. Keep your eyes open.",
            "The Coilhead is closer than it was a second ago.",
            "Nobody blink. NOBODY BLINK."
        },
        ["MaskedPlayerEnemy"] = new List<string>
        {
            "Something is wearing $$'s face...",
            "Wait... there are two $$s?",
            "That doesn't look like the real $$.",
            "A Mimic is impersonating $$. Don't trust anyone.",
            "$$'s evil twin just showed up.",
            "Who's the real $$? Good luck."
        },
        ["Centipede"] = new List<string>
        {
            "A Snare Flea latched onto $$'s head!",
            "$$ has an unwanted hat.",
            "Something dropped from the ceiling onto $$!",
            "$$ is being suffocated by a ceiling bug!",
            "A Snare Flea is hugging $$'s face!",
            "$$ can't see. Or breathe. Snare Flea."
        },
        ["SandSpider"] = new List<string>
        {
            "$$ walked right into a spider's web.",
            "$$ triggered a trip wire. The Spider knows.",
            "The Spider felt something in its web...",
            "$$ just rang the Spider's doorbell.",
            "$$ is tangled in webs. The Spider is coming.",
            "A web just caught $$. Dinner is served."
        },
        ["ForestGiant"] = new List<string>
        {
            "A Forest Giant snatched up $$!",
            "$$ was picked up like an action figure!",
            "The Giant grabbed $$! This is bad!",
            "$$ is in the Giant's hand. Pray.",
            "A Forest Giant has $$! Someone do something!",
            "The Giant is holding $$ like a snack. Because they are one."
        },
        ["Crawler"] = new List<string>
        {
            "$$ just got bodied by a Thumper.",
            "A Thumper clipped $$!",
            "$$ took a hit from a Thumper. Walk it off.",
            "The Thumper charged $$! They're still standing... barely.",
            "$$ was sideswiped by a Thumper.",
            "A Thumper just freight-trained $$."
        },
        ["BaboonHawk"] = new List<string>
        {
            "A Baboon Hawk chose violence against $$.",
            "The Baboon Hawks are getting aggressive with $$.",
            "$$ is being targeted by Baboon Hawks.",
            "A Baboon Hawk stabbed at $$!",
            "$$ caught the attention of the murder birds.",
            "The Baboon Hawks don't like $$."
        },
        ["DressGirl"] = new List<string>
        {
            "Something has taken an interest in $$...",
            "$$ is being haunted. Good luck with that.",
            "The Ghost Girl chose $$. F.",
            "$$ hears giggling. No one else does.",
            "She's following $$. Don't look behind you.",
            "$$ feels a presence. It's not friendly."
        },
        ["MouthDog"] = new List<string>
        {
            "Something heard $$...",
            "An Eyeless Dog perked up near $$. Shhhh.",
            "$$ made a noise. The Dog noticed.",
            "An Eyeless Dog is sniffing around $$.",
            "Shhh! The Dog is near $$!",
            "$$ needs to be very, very quiet right now."
        },
        ["Blob"] = new List<string>
        {
            "$$ is getting slimed!",
            "The Blob touched $$. Gross.",
            "$$ stepped in the Hygrodere.",
            "The slime is on $$. Move!",
            "$$ is being slowly consumed. Move faster!",
            "A Blob is dissolving $$'s shoes."
        },
        ["NutcrackerEnemy"] = new List<string>
        {
            "The Nutcracker opened fire!",
            "Shots fired! The Nutcracker is shooting!",
            "The Nutcracker has $$ in its sights!",
            "Duck! The Nutcracker is locked and loaded!",
            "The toy soldier is shooting! Take cover!",
            "The Nutcracker's shotgun just went off!"
        },
        ["Butler"] = new List<string>
        {
            "The Butler is following $$. Politely. Menacingly.",
            "$$ has the Butler's full attention.",
            "The Butler is getting closer to $$...",
            "The Butler offered $$ a drink. $$ should run.",
            "$$ noticed the Butler's knife. Time to go.",
            "The Butler is serving $$ a death stare."
        },
        ["ClaySurgeon"] = new List<string>
        {
            "The Barber is eyeing $$...",
            "$$ is next in the Barber's chair.",
            "Snip snip. The Barber found $$.",
            "The Barber's scissors are getting closer to $$.",
            "$$ can hear scissors behind them...",
            "The Barber wants to give $$ a trim."
        },
        ["CaveDweller"] = new List<string>
        {
            "$$ picked up something... alive.",
            "That baby $$ found looks hungry.",
            "The Maneater is growing. $$ should drop it.",
            "$$ adopted something they shouldn't have.",
            "The cute baby near $$ is getting bigger...",
            "The Maneater is near $$. Don't feed it."
        },
        ["HoarderBug"] = new List<string>
        {
            "A Hoarding Bug is upset with $$!",
            "$$ got too close to the Hoarding Bug's stuff.",
            "The Hoarding Bug is angry at $$!",
            "$$ touched the bug's loot. Bad move.",
            "A Hoarding Bug is defending its hoard from $$!",
            "$$ angered the world's most territorial bug."
        },
        ["SandWorm"] = new List<string>
        {
            "The ground is shaking near $$...",
            "Something massive is moving underground near $$!",
            "$$ felt the earth rumble. That's not good.",
            "An Earth Leviathan is surfacing near $$!",
            "RUN! The ground is opening near $$!",
            "$$ is standing on very dangerous ground."
        },
        ["RadMech"] = new List<string>
        {
            "An Old Bird spotted $$!",
            "The Old Bird is targeting $$!",
            "$$ has a giant robot problem.",
            "An Old Bird is charging at $$!",
            "The mech locked onto $$. Run!",
            "$$ is in an Old Bird's crosshairs."
        },
        ["RedLocustBees"] = new List<string>
        {
            "$$ got too close to the beehive!",
            "The Circuit Bees are angry at $$!",
            "$$ disturbed the hive! Bees incoming!",
            "A swarm of bees is chasing $$!",
            "The Circuit Bees don't want $$ near their hive!",
            "$$ poked the bee's nest. Classic."
        },
        ["ButlerBees"] = new List<string>
        {
            "Mask Hornets are swarming $$!",
            "The Butler's hornets are after $$!",
            "A swarm of Mask Hornets appeared near $$!",
            "$$ is being chased by angry hornets!",
            "The Mask Hornets have targeted $$!",
            "Hornets everywhere! $$ is in trouble!"
        }
    };

    // Generic fallback pools for unknown/modded enemies
    private static readonly List<string> GenericMonsterDeath = new List<string>
    {
        "$$ was killed by a ##.",
        "## claimed another victim: $$.",
        "$$ had an unfortunate encounter with a ##.",
        "A ## ended $$'s career.",
        "$$ was no match for the ##.",
        "## destroyed $$. Completely.",
        "$$ was eliminated by a ##.",
        "The ## made short work of $$."
    };

    private static readonly List<string> GenericMonsterEncounter = new List<string>
    {
        "A ## has been disturbed...",
        "$$ caught the attention of a ##.",
        "A ## noticed $$. That can't be good.",
        "The ## is agitated. $$ is nearby.",
        "$$ is being targeted by a ##.",
        "A ## has its sights on $$."
    };

    internal static string GetDeathMessage(string enemyName, string username)
    {
        if (DeathPools.TryGetValue(enemyName, out var pool))
        {
            return pool[_rng.Next(pool.Count)].Replace("$$", username);
        }

        // Fallback for unknown/modded enemies
        string msg = GenericMonsterDeath[_rng.Next(GenericMonsterDeath.Count)];
        return msg.Replace("$$", username).Replace("##", enemyName);
    }

    internal static string GetEncounterMessage(string enemyName, string username)
    {
        if (EncounterPools.TryGetValue(enemyName, out var pool))
        {
            return pool[_rng.Next(pool.Count)].Replace("$$", username);
        }

        // Fallback for unknown/modded enemies
        if (!ConfigManager.CustomEnemyEncounterMessages.Value) return null;

        string msg = GenericMonsterEncounter[_rng.Next(GenericMonsterEncounter.Count)];
        return msg.Replace("$$", username).Replace("##", enemyName);
    }

    internal static bool HasDeathPool(string enemyName)
    {
        return DeathPools.ContainsKey(enemyName);
    }

    internal static bool HasEncounterPool(string enemyName)
    {
        return EncounterPools.ContainsKey(enemyName);
    }
}
