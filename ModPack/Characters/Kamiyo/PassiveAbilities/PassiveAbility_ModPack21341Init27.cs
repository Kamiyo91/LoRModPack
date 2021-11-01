using ModPack21341.Utilities;

namespace ModPack21341.Characters.Kamiyo.PassiveAbilities
{
    //IncompleteDemigod
    public class PassiveAbility_ModPack21341Init27 : PassiveAbilityBase
    {
        public override void OnRoundStartAfter()
        {
            UnitUtilities.DrawUntilX(owner, 3);
            owner.RecoverHP(2);
            owner.breakDetail.RecoverBreak(2);
        }
    }
}