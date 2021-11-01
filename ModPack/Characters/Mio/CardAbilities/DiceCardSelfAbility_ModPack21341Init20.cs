using ModPack21341.Characters.Mio.Buffs;

namespace ModPack21341.Characters.Mio.CardAbilities
{
    //CostDown1SelfNull
    public class DiceCardSelfAbility_ModPack21341Init20 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Combat Start]Remove status aliment [Nullify Power] and become [Immune] to it until the end of the Scene.\n[On Use] Gain 1 [Strength] next Scene";

        public override void OnUseCard()
        {
            owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, owner);
        }

        public override void OnStartBattle()
        {
            owner.bufListDetail.RemoveBufAll(KeywordBuf.NullifyPower);
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init16());
        }
    }
}