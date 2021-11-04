namespace ModPack21341.Characters.Hayate.CardAbilities
{
    //Speed8Power2
    public class DiceCardSelfAbility_ModPack21341Init33 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Use] If Speed is 8 or higher,all dice on this page gain +2 Power";

        public override void OnUseCard()
        {
            if (card.speedDiceResultValue >= 8)
                card.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 2
                });
        }
    }
}