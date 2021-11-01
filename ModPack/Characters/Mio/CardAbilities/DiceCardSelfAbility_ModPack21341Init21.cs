namespace ModPack21341.Characters.Mio.CardAbilities
{
    //EmotionLv5Fragment
    public class DiceCardSelfAbility_ModPack21341Init21 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[Single Use]\nCan only be used at [Emotion Level 5] and [Ego's Aura] is required";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 5 &&
                   owner.bufListDetail.HasAssimilation();
        }
    }
}