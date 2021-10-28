using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.BlackSilenceEgoMapManager;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CardAbilities
{
    public class DiceCardSelfAbility_BlackSilenceMaskEgo : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "Can be used at Emotion Level 4 or above\n[On Use] Unleash the power of the Black Silence's Mask next Scene";

        public override bool OnChooseCard(BattleUnitModel owner) =>
            owner.emotionDetail.EmotionLevel >= 4 && !owner.bufListDetail.HasAssimilation();
    }
    public class DiceCardSelfAbility_BlackSilenceMaskEgoScream : DiceCardSelfAbilityBase
    {
        public override bool OnChooseCard(BattleUnitModel owner) => owner.bufListDetail.HasAssimilation();

        public override void OnUseCard()
        {
            owner.view.SetAltSkin("BlackSilence4");
        }

        public override void OnStartBattle()
        {
            if (SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.isEgo) return;
            ChangeToBlackSilenceEgoMap(owner);
        }
        private static void ChangeToBlackSilenceEgoMap(BattleUnitModel owner) => MapUtilities.ChangeMap(new MapModel
        {
            Stage = "BlackSilenceMassEgo",
            OneTurnEgo = true,
            IsPlayer = true,
            Component = new BlackSilenceEgoMapManager(),
            InitBgm = false,
            Fy = 0.285f
        },owner.faction);
        public override void OnEndBattle()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin))
            {
                owner.view.SetAltSkin("BlackSilence3");
            }
            else
            {
                owner.view.CreateSkin();
            }
        }
    }
    public class DiceCardSelfAbility_BlackSilenceCustomSpecial : DiceCardSelfAbilityBase
    {
        public static string Desc = "This page can be used after using all 9 Combat pages of the Black Silence. On hit, inflict 5 Bleed, 3 Paralysis, and 3 Fragile next Scene.";
        public override bool OnChooseCard(BattleUnitModel owner)
        {
            if (owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Orlando) is PassiveAbility_Orlando passiveAbility)
            {
                return passiveAbility.IsActivatedSpecialCard();
            }
            return owner.UnitData.unitData.EnemyUnitId == 60005;
        }
        public override void OnUseCard()
        {
            var passiveAbility = owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Orlando) as PassiveAbility_Orlando;
            passiveAbility?.ResetUsedCount();
        }
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            if (card?.target == null) return;
            card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 5, owner);
            card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Binding, 3, owner);
            card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable, 3, owner);
        }
    }
}
