namespace ModPack21341.Characters.Kamiyo.CardAbilities
{
    //EmotionLv5Kamiyo
    public class DiceCardSelfAbility_ModPack21341Init17 : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can only be used at [Emotion Level 5] and [Alter Ego's Aura] is required";
        private bool _motionChanged;

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 5 && owner.bufListDetail.HasAssimilation();
        }

        public override void OnEndAreaAttack()
        {
            if (!_motionChanged) return;
            _motionChanged = false;
            owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
        }

        public override void OnApplyCard()
        {
            if (!string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) ||
                owner.UnitData.unitData.bookItem != owner.UnitData.unitData.CustomBookItem) return;
            _motionChanged = true;
            owner.view.charAppearance.ChangeMotion(ActionDetail.Penetrate);
        }

        public override void OnReleaseCard()
        {
            _motionChanged = false;
            owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
        }
    }
}