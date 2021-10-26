using ModPack21341.Characters.Buffs;

namespace ModPack21341.Characters.CardAbilities
{
    public class DiceCardSelfAbility_CustomMyoBerseker : DiceCardSelfAbilityBase
    {
        public override void OnUseCard()
        {
            foreach (var battleDiceCardModel in owner.allyCardDetail.GetAllDeck())
            {
                battleDiceCardModel.ChangeFarToNearForMyo();
            }

            owner.allyCardDetail.AddNewCard(608001);
            owner.bufListDetail.AddBufWithoutDuplication(
                new BattleUnitBuf_MyoBerserkCustomCheck
                {
                    stack = 0
                });
            card.card.ReserveExhaust();
        }
    }
}
