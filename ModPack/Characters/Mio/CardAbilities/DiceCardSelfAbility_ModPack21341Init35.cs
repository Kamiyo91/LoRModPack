namespace ModPack21341.Characters.Mio.CardAbilities
{
    //LoseHP7Power1
    public class DiceCardSelfAbility_ModPack21341Init35 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Use] Deal 7 damage to self: all dice on this page gain +1 Power";

        public override void OnUseCard()
        {
            card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 1
            });
            owner.TakeDamage(7, DamageType.Card_Ability, owner);
        }
    }
}