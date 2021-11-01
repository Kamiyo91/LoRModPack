using ModPack21341.Utilities;

namespace ModPack21341.Characters.Mio.PassiveAbilities
{
    //FragmentOfGod
    public class PassiveAbility_ModPack21341Init36 : PassiveAbilityBase
    {
        public override void OnRoundStartAfter()
        {
            UnitUtilities.DrawUntilX(owner, 4);
        }

        public override void OnRoundEnd()
        {
            owner.cardSlotDetail.RecoverPlayPoint(1);
        }
    }
}