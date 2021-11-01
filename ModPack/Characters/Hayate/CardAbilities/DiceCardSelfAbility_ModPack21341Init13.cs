namespace ModPack21341.Characters.Hayate.CardAbilities
{
    //TrueGodAura
    public class DiceCardSelfAbility_ModPack21341Init13 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "Can be used at Emotion Level 4 or above\n[On Use] Unleash The True Power of a God,recover full Stagger Resist and full Light next Scene.";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 4;
        }
    }
}