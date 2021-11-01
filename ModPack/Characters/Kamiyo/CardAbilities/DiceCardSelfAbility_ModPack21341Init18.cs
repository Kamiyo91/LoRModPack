namespace ModPack21341.Characters.Kamiyo.CardAbilities
{
    //KurosawaFire
    public class DiceCardSelfAbility_ModPack21341Init18 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]Restore 1 Light.If target has 6 or more Burn, gain +1 Power";

        public override void OnUseCard()
        {
            owner.cardSlotDetail.RecoverPlayPoint(1);
            var target = card.target;
            var activatedBuf = target?.bufListDetail.GetActivatedBuf(KeywordBuf.Burn);
            if (activatedBuf == null || activatedBuf.stack < 6) return;
            var currentDiceAction = owner.currentDiceAction;
            currentDiceAction?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 1
            });
        }
    }
}