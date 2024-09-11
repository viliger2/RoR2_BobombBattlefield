using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SM64BBF
{
    public static class Config
    {
        public struct BobombSpawning
        {
            public static ConfigEntry<int> SelectionWeight;
            public static ConfigEntry<int> MinimumStageCount;

            public static ConfigEntry<bool> SpawnArena;
            public static ConfigEntry<bool> SpawnAncientLoft;
            public static ConfigEntry<bool> SpawnBlackBeach;
            public static ConfigEntry<bool> SpawnDampCaveSimple;
            public static ConfigEntry<bool> SpawnFoggySwamp;
            public static ConfigEntry<bool> SpawnFrozenWall;
            public static ConfigEntry<bool> SpawnGolemPlains;
            public static ConfigEntry<bool> SpawnGooLake;
            public static ConfigEntry<bool> SpawnLakes;
            public static ConfigEntry<bool> SpawnRootJungle;
            public static ConfigEntry<bool> SpawnShipGraveyard;
            public static ConfigEntry<bool> SpawnSkyMeadow;
            public static ConfigEntry<bool> SpawnSnowyForest;
            public static ConfigEntry<bool> SpawnSulfurPools;
            public static ConfigEntry<bool> SpawnWispGraveyard;

            public static ConfigEntry<bool> SpawnSimulacrum;

            public static ConfigEntry<string> SpawnCustomStages;
        }

        public struct TreeInteractable
        {
            public static ConfigEntry<float> CoinWeight;
            public static ConfigEntry<float> OneUpWeight;
            public static ConfigEntry<float> StarmanWeight;
            public static ConfigEntry<float> NothingWeight;
        }

        public static void PopulateConfig(ConfigFile config)
        {
            BobombSpawning.SelectionWeight = config.Bind("Bobomb Spawn", "Bobomb Card Selection Weight", 6, "Selection weight for Bobomb Director Card. Does not affect spawning on Bobomb Battlefield, only for other stages.");
            BobombSpawning.MinimumStageCount = config.Bind("Bobomb Spawn", "Bobomb Card Minimal Stage Count", 0, "How many stages players must clear before Bobombs start spawning. Does not affect spawning on Bobomb Battlefield, only for other stages.");

            BobombSpawning.SpawnArena = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in arena", false, "Bobombs can spawn in arena (internal name for Void Fields)");
            BobombSpawning.SpawnAncientLoft = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in ancientloft", false, "Bobombs can spawn in ancientloft (internal name for Aphelian Sanctuary)");
            BobombSpawning.SpawnBlackBeach = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in blackbeach", false, "Bobombs can spawn in blackbeach (both 1 and 2) (internal name for Distant Roost)");
            BobombSpawning.SpawnDampCaveSimple = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in dampcavesimple", false, "Bobombs can spawn in dampcavesimple (internal name for Abyssal Depths)");
            BobombSpawning.SpawnFoggySwamp = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in foggyswamp", false, "Bobombs can spawn in foggyswamp (internal name for Wetlands Aspect)");
            BobombSpawning.SpawnFrozenWall = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in frozenwall", false, "Bobombs can spawn in foggyswamp (internal name for Rallypoint Delta)");
            BobombSpawning.SpawnGolemPlains = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in golemplains", false, "Bobombs can spawn in golemplains (both 1 and 2) (internal name for Titanic Plains)");
            BobombSpawning.SpawnGooLake = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in goolake", false, "Bobombs can spawn in goolake (internal name for Abandoned Aqueduct)");
            BobombSpawning.SpawnLakes = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in lakes", false, "Bobombs can spawn in lakes (internal name for Verdant Falls)");
            BobombSpawning.SpawnRootJungle = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in rootjungle", false, "Bobombs can spawn in rootjungle (internal name for Sundered Grove, aka Stadia Stage)");
            BobombSpawning.SpawnShipGraveyard = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in shipgraveyard", false, "Bobombs can spawn in shipgraveyard (internal name for Siren's Call)");
            BobombSpawning.SpawnSkyMeadow = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in skymeadow", false, "Bobombs can spawn in skymeadow (internal name for Sky Meadow (duh))");
            BobombSpawning.SpawnSnowyForest = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in snowyforest", false, "Bobombs can spawn in snowyforest (internal name for Siphoned Forest)");
            BobombSpawning.SpawnSulfurPools = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in sulfurpools", false, "Bobombs can spawn in sulfurpools (internal name for Sulfur Pools, oh hey another one)");
            BobombSpawning.SpawnWispGraveyard = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in wispgraveyard", false, "Bobombs can spawn in wispgraveyard (internal name for Scorched Acres)");

            BobombSpawning.SpawnSimulacrum = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in Simulacrum", false, "Bobombs can spawn in Simulacrum, covers all Simulacrum stages.");

            BobombSpawning.SpawnCustomStages = config.Bind("Bobomb Spawn", "Bobomb Can Spawn in custom stages", "", "To make Bobombs spawn in custom stages, add internal names of custom stages separated by \",\". You can find their name by installing DebugToolkit and then entering \"list_scene\", it will give you the list of all available scenes as stages. For example, Fogbound Lagoon is FBLScene, Catacombs is catacombs_DS1_Catacombs, Bobomb Battlefield is sm64_bbf_SM64_BBF");

            TreeInteractable.CoinWeight = config.Bind("Tree Interactable", "Coin Weight", 40f, "Weight to spawn Coin when shaking the tree. Higher the value - higher the chance.");
            TreeInteractable.OneUpWeight = config.Bind("Tree Interactable", "One Up Weight", 5f, "Weight to spawn 1UP when shaking the tree. Higher the value - higher the chance.");
            TreeInteractable.StarmanWeight = config.Bind("Tree Interactable", "Starman Weight", 10f, "Weight to spawn Starman when shaking the tree. Higher the value - higher the chance.");
            TreeInteractable.NothingWeight = config.Bind("Tree Interactable", "Nothing Weight", 45f, "Weight to get nothing when shaking the tree. Higher the value - higher the chance.");
        }

    }
}
