using ModPack21341.Characters.CommonPassiveAbilities;

namespace ModPack21341.Characters.CommonCardAbilities
{
    //SwitchDeck
    public class DiceCardSelfAbility_ModPack21341Init7 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "Usable one time for Scene\n[On Play]Switch Deck with the [Original] or [Awakened] version";

        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init8) is
                PassiveAbility_ModPack21341Init8
                passive)
                passive.ChangeDeck();
            self.exhaust = true;
        }
    }
}