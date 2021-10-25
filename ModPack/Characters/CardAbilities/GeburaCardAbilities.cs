namespace ModPack21341.Characters.CardAbilities
{
    public class DiceCardSelfAbility_GeburaEgoAwaken : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can be used only at Emotion Level 4 or above\nManifest E.G.O. next Scene";
        public override void OnUseCard()
        {
            card.card.exhaust = true;
            owner.cardSlotDetail.RecoverPlayPointByCard(6);
            owner.breakDetail.RecoverBreak(owner.breakDetail.GetDefaultBreakGauge());
            if (!owner.passiveDetail.HasPassive<PassiveAbility_GeburaRedMistEgo>() && !owner.passiveDetail.HasPassiveInReady<PassiveAbility_GeburaRedMistEgo>())
            {
                owner.passiveDetail.AddPassive(new PassiveAbility_GeburaRedMistEgo());
            }
        }
        public override bool OnChooseCard(BattleUnitModel owner) => !owner.bufListDetail.HasAssimilation() && owner.emotionDetail.EmotionLevel > 3 && base.OnChooseCard(owner);
        
    }
}
