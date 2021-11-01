using ModPack21341.Utilities;

namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //SlashCounter
    public class PassiveAbility_ModPack21341Init18 : PassiveAbilityBase
    {
        public override void OnStartBattle()
        {
            UnitUtilities.ReadyCounterCard(owner, 24);
        }
    }
}