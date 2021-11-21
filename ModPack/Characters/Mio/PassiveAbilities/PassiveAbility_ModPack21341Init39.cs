using ModPack21341.Characters.Mio.Buffs;
using ModPack21341.Harmony;

namespace ModPack21341.Characters.Mio.PassiveAbilities
{
    //MioMirror
    public class PassiveAbility_ModPack21341Init39 : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            if (!owner.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_ModPack21341Init15))
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init15());
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 904));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 905));
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 904))
                owner.personalEgoDetail.RemoveCard(curCard.card.GetID());
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 19) ||
                curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 25))
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
        }
    }
}