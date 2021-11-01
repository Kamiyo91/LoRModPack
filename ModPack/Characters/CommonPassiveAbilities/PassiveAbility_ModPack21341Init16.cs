namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //Neutral
    public class PassiveAbility_ModPack21341Init16 : PassiveAbilityBase
    {
        public override void OnRoundStartAfter()
        {
            owner.allyCardDetail.DrawCards(1);
            owner.cardSlotDetail.RecoverPlayPoint(1);
        }
    }
}