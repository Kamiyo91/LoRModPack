using ModPack21341.Characters.Gebura.PassiveAbilities;

namespace ModPack21341.Characters.Gebura.CardAbilities
{
    //GeburaEgoAwaken
    public class DiceCardSelfAbility_ModPack21341Init8 : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can be used only at Emotion Level 4 or above\nManifest E.G.O. next Scene";

        public override void OnUseCard()
        {
            card.card.exhaust = true;
            owner.cardSlotDetail.RecoverPlayPointByCard(6);
            owner.breakDetail.RecoverBreak(owner.breakDetail.GetDefaultBreakGauge());
            if (!owner.passiveDetail.HasPassive<PassiveAbility_ModPack21341Init20>() &&
                !owner.passiveDetail.HasPassiveInReady<PassiveAbility_ModPack21341Init20>())
                owner.passiveDetail.AddPassive(new PassiveAbility_ModPack21341Init20());
        }

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return !owner.bufListDetail.HasAssimilation() && owner.emotionDetail.EmotionLevel > 3 &&
                   base.OnChooseCard(owner);
        }
    }
}