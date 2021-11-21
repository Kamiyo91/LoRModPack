namespace ModPack21341.Characters.Mio.CardAbilities
{
    //LoseHP14Power2
    public class DiceCardSelfAbility_ModPack21341Init36 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Use] Deal 14 damage to self: all dice on this page gain +2 Power";

        public override void OnUseCard()
        {
            card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 2
            });
            owner.TakeDamage(14, DamageType.Card_Ability, owner);
        }
    }
}