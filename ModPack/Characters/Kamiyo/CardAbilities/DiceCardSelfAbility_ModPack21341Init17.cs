namespace ModPack21341.Characters.Kamiyo.CardAbilities
{
    //EmotionLv5Kamiyo
    public class DiceCardSelfAbility_ModPack21341Init17 : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can only be used at [Emotion Level 5] and [Alter Ego's Aura] is required";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 5 && owner.bufListDetail.HasAssimilation();
        }
    }
}