using RoR2;
using RoR2.Achievements.Artifacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SM64BBF.Achievments
{
    [RegisterAchievement("SM64_BBF_ObtainArtifactBobombsOnDeathAchievment", "Artifacts.BBF_BobombsOnDeath", null, 3u, null)]
    public class ObtainArtifactBobombsOnDeathAchievment : BaseObtainArtifactAchievement
    {
        public override ArtifactDef artifactDef => SM64BBFContent.BobombOnDeath;
    }
}
