namespace ModPack21341.Characters.Kamiyo.CardAbilities
{
    //BurningField1
    public class DiceCardSelfAbility_ModPack21341Init15 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]Inflict 1 burn to all enemies";

        public override void OnUseCard()
        {
            foreach (var unit in BattleObjectManager.instance.GetAliveList(owner.faction == Faction.Player
                ? Faction.Enemy
                : Faction.Player)) unit.bufListDetail.AddKeywordBufByCard(KeywordBuf.Burn, 1, unit);
        }
    }
}