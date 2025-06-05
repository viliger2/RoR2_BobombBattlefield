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

            public static ConfigEntry<string> BobombSpawnStages;
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

            BobombSpawning.BobombSpawnStages = config.Bind("Bobomb Spawn", "Stages to Spawn In", "", "Additional stages (beyond bobomb battlefield) that Bobombs can spawn in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            TreeInteractable.CoinWeight = config.Bind("Tree Interactable", "Coin Weight", 40f, "Weight to spawn Coin when shaking the tree. Higher the value - higher the chance.");
            TreeInteractable.OneUpWeight = config.Bind("Tree Interactable", "One Up Weight", 5f, "Weight to spawn 1UP when shaking the tree. Higher the value - higher the chance.");
            TreeInteractable.StarmanWeight = config.Bind("Tree Interactable", "Starman Weight", 5f, "Weight to spawn Starman when shaking the tree. Higher the value - higher the chance.");
            TreeInteractable.NothingWeight = config.Bind("Tree Interactable", "Nothing Weight", 50f, "Weight to get nothing when shaking the tree. Higher the value - higher the chance.");
        }

    }
}
