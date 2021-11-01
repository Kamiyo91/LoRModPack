using ModPack21341.Utilities;

namespace ModPack21341.Characters.Angela.PassiveAbilities
{
    //AngelaUnit
    public class PassiveAbility_ModPack21341Init3 : PassiveAbilityBase
    {
        private BattleDialogueModel _dlg;
        private string _originalSkinName;

        public override void OnWaveStart()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                UnitUtilities.PrepareSephirahSkin(owner, 21, "Angela", owner.faction == Faction.Enemy,
                    ref _originalSkinName, ref _dlg);
            AddCardsWaveStart();
        }

        private void AddCardsWaveStart()
        {
            if (owner.emotionDetail.EmotionLevel == 3)
            {
                owner.personalEgoDetail.AddCard(9910011);
                owner.personalEgoDetail.AddCard(9910012);
                owner.personalEgoDetail.AddCard(9910013);
            }

            if (owner.emotionDetail.EmotionLevel == 4)
            {
                owner.personalEgoDetail.AddCard(9910011);
                owner.personalEgoDetail.AddCard(9910012);
                owner.personalEgoDetail.AddCard(9910013);
                owner.personalEgoDetail.AddCard(9910014);
                owner.personalEgoDetail.AddCard(9910015);
                owner.personalEgoDetail.AddCard(9910016);
            }

            if (owner.emotionDetail.EmotionLevel != 5) return;
            owner.personalEgoDetail.AddCard(9910011);
            owner.personalEgoDetail.AddCard(9910012);
            owner.personalEgoDetail.AddCard(9910013);
            owner.personalEgoDetail.AddCard(9910014);
            owner.personalEgoDetail.AddCard(9910015);
            owner.personalEgoDetail.AddCard(9910016);
            owner.personalEgoDetail.AddCard(9910017);
            owner.personalEgoDetail.AddCard(9910018);
            owner.personalEgoDetail.AddCard(9910019);
        }

        private void AddCardOnLvUpEmotion()
        {
            if (owner.emotionDetail.EmotionLevel == 3)
            {
                owner.personalEgoDetail.AddCard(9910011);
                owner.personalEgoDetail.AddCard(9910012);
                owner.personalEgoDetail.AddCard(9910013);
            }

            if (owner.emotionDetail.EmotionLevel == 4)
            {
                owner.personalEgoDetail.AddCard(9910014);
                owner.personalEgoDetail.AddCard(9910015);
                owner.personalEgoDetail.AddCard(9910016);
            }

            if (owner.emotionDetail.EmotionLevel != 5) return;
            owner.personalEgoDetail.AddCard(9910017);
            owner.personalEgoDetail.AddCard(9910018);
            owner.personalEgoDetail.AddCard(9910019);
        }

        public override void OnLevelUpEmotion()
        {
            AddCardOnLvUpEmotion();
        }

        public override void OnBattleEnd()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem &&
                owner.faction == Faction.Player)
                UnitUtilities.ReturnToTheOriginalBaseSkin(owner, _originalSkinName, _dlg);
        }
    }
}