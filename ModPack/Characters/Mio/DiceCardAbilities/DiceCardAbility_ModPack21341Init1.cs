namespace ModPack21341.Characters.Mio.DiceCardAbilities
{
    //Death
    public class DiceCardAbility_ModPack21341Init1 : DiceCardAbilityBase
    {
        public static string Desc =
            "[On Clash Win] Destroy all dice the opponent has\n[On Hit] Deal 45 Damage to the Target, if the Target goes below 15% Hp, kill it";

        public override void OnSucceedAttack()
        {
            DealDamageCheckKill();
        }

        private void DealDamageCheckKill()
        {
            if (card.target?.hp - 45 < card.target?.MaxHp * 0.15f)
                card.target?.Die();
            else
                card.target?.TakeDamage(45, DamageType.Card_Ability);
        }

        public override void OnWinParrying()
        {
            card.target?.currentDiceAction?.DestroyDice(DiceMatch.AllDice, DiceUITiming.AttackAfter);
        }
    }
}