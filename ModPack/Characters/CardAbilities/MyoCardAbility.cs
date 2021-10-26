using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;

namespace ModPack21341.Characters.CardAbilities
{
    public class DiceCardSelfAbility_CustomMyoBerserk : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use] Convert all Ranged pages in the deck into Melee pages.Add a copy of [Feral Knives] to hand. Starting with the next Scene,all dice this character plays gain +1 Power";
        public override void OnUseCard()
        {
            foreach (var battleDiceCardModel in owner.allyCardDetail.GetAllDeck())
            {
                battleDiceCardModel.ChangeFarToNearForMyo();
            }
            if (!owner.allyCardDetail.GetAllDeck().Exists(x => x.GetID().id == 608001))
                owner.allyCardDetail.AddNewCard(new LorId(ModPack21341Init.PackageId, 44));
            owner.bufListDetail.AddBufWithoutDuplication(
                new BattleUnitBuf_MyoBerserkCustomCheck
                {
                    stack = 0
                });
            card.card.ReserveExhaust();
        }

        public override bool BeforeAddToHand(BattleUnitModel unit, BattleDiceCardModel self)
        {
            if (!owner.allyCardDetail.GetAllDeck().Exists(x => x.GetID().id == 608017)) return true;
            owner.allyCardDetail.ExhaustACardAnywhere(self);
            return false;
        }
    }
}
