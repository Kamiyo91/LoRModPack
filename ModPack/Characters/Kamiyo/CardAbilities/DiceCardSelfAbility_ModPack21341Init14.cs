namespace ModPack21341.Characters.Kamiyo.CardAbilities
{
    //AlterEgoCard
    public class DiceCardSelfAbility_ModPack21341Init14 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[Single Use]\nCan be used at Emotion Level 4 or above\n[On Use] Unleash Alter Ego's Power,recover full Stagger Resist and full Light next Scene.";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 4 && !owner.bufListDetail.HasAssimilation();
        }
    }
}