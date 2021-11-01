namespace ModPack21341.Characters.Angela.PassiveAbilities
{
    //AngelaRegen
    public class PassiveAbility_ModPack21341Init2 : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            owner.allyCardDetail.DrawCards(2);
        }

        public override void OnRoundStart()
        {
            owner.cardSlotDetail.RecoverPlayPoint(1);
        }

        public override void OnDrawCard()
        {
            owner.allyCardDetail.DrawCards(1);
        }
    }
}