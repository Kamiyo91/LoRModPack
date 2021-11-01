using ModPack21341.Utilities;

namespace ModPack21341.Characters.Hayate.PassiveAbilities
{
    //HighDivinity
    public class PassiveAbility_ModPack21341Init25 : PassiveAbilityBase
    {
        public override void OnRoundStartAfter()
        {
            UnitUtilities.DrawUntilX(owner, 5);
        }
    }
}