namespace ModPack21341.Characters.Mio.CardAbilities
{
    //GodAuraCard
    public class DiceCardSelfAbility_ModPack21341Init22 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[Single Use]\nCan only be used at Emotion level 4 or above\n[On Use] Unleash Ego's power, recover full Stagger Resist and full Light next Scene.";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 4 &&
                   !owner.bufListDetail.HasAssimilation();
        }
    }
}