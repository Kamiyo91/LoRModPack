using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.MioStageMaps;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Mio.CardAbilities
{
    //SakuraMirage
    public class DiceCardSelfAbility_ModPack21341Init23 : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can only be used at Emotion level 4 or above and [Ego's Aura] is required";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 4 &&
                   owner.bufListDetail.HasAssimilation();
        }

        public override void OnStartBattle()
        {
            if (owner.faction != Faction.Player ||
                SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.isEgo) return;
            ChangeToMioEgoMap();
        }

        private static void ChangeToMioEgoMap()
        {
            MapUtilities.ChangeMap(new MapModel
            {
                Stage = "Mio",
                StageId = 2,
                OneTurnEgo = true,
                IsPlayer = true,
                Component = new ModPack21341InitMioMapManager(),
                Bgy = 0.2f
            });
        }
    }
}