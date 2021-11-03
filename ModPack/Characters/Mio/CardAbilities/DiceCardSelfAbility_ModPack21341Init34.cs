namespace ModPack21341.Characters.Hayate.CardAbilities
{
    //Speed6Power1
    public class DiceCardSelfAbility_ModPack21341Init34 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Use] If Speed is 6 or higher,all dice on this page gain +1 Power";

        public override void OnUseCard()
        {
            if (card.speedDiceResultValue >= 6)
            {
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 1
                });
            }
        }
    }
}