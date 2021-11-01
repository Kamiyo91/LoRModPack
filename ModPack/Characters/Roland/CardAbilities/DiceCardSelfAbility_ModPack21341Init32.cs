using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.BlackSilenceEgoMapManager;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Roland.CardAbilities
{
    //BlackSilenceMaskEgoScream
    public class DiceCardSelfAbility_ModPack21341Init32 : DiceCardSelfAbilityBase
    {
        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.bufListDetail.HasAssimilation();
        }

        public override void OnUseCard()
        {
            owner.view.SetAltSkin("BlackSilence4");
        }

        public override void OnStartBattle()
        {
            if (SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.isEgo) return;
            ChangeToBlackSilenceEgoMap(owner);
        }

        private static void ChangeToBlackSilenceEgoMap(BattleUnitModel owner)
        {
            MapUtilities.ChangeMap(new MapModel
            {
                Stage = "BlackSilenceMassEgo",
                OneTurnEgo = true,
                IsPlayer = true,
                Component = new ModPack21341InitBlackSilenceEgoMapManager(),
                InitBgm = false,
                Fy = 0.285f
            }, owner.faction);
        }

        public override void OnEndBattle()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin))
                owner.view.SetAltSkin("BlackSilence3");
            else
                owner.view.CreateSkin();
        }
    }
}