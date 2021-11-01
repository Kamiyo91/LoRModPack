using ModPack21341.Characters.Roland.PassiveAbilities;

namespace ModPack21341.Characters.Roland.CardAbilities
{
    //BlackSIlenceCustomSpecial
    public class DiceCardSelfAbility_ModPack21341Init30 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "This page can be used after using all 9 Combat pages of the Black Silence. On hit, inflict 5 Bleed, 3 Paralysis, and 3 Fragile next Scene.";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            if (owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init48) is
                PassiveAbility_ModPack21341Init48
                passiveAbility) return passiveAbility.IsActivatedSpecialCard();
            return owner.UnitData.unitData.EnemyUnitId == 60005;
        }

        public override void OnUseCard()
        {
            var passiveAbility =
                owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init48) as
                    PassiveAbility_ModPack21341Init48;
            passiveAbility?.ResetUsedCount();
        }

        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            if (card?.target == null) return;
            card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Bleeding, 5, owner);
            card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Binding, 3, owner);
            card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Vulnerable, 3, owner);
        }
    }
}