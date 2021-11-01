namespace ModPack21341.Characters.Roland.CardAbilities
{
    //BlackSilenceMaskEgo
    public class DiceCardSelfAbility_ModPack21341Init31 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "Can be used at Emotion Level 4 or above\n[On Use] Unleash the power of the Black Silence's Mask next Scene";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 4 && !owner.bufListDetail.HasAssimilation();
        }
    }
}