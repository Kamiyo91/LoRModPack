using ModPack21341.Harmony;

namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //CustomInstantIndexRelease
    public class PassiveAbility_ModPack21341Init9 : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            if (owner.passiveDetail.HasPassive<PassiveAbility_250115>() ||
                owner.passiveDetail.HasPassiveInReady<PassiveAbility_250115>())
                owner.passiveDetail.DestroyPassive(this);
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 932));
        }
    }
}