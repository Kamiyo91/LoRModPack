using System;
using System.Linq;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //Happy
    public class PassiveAbility_ModPack21341Init13 : PassiveAbilityBase
    {
        private static readonly Random RndChance = new Random();
        private int _stack;

        public int GetStack()
        {
            return _stack;
        }

        public void ChangeNameAndSetStacks(int stack)
        {
            switch (stack)
            {
                case 1:
                    name = "Happy";
                    _stack = 1;
                    break;
                case 2:
                    name = "Ecstatic";
                    _stack = 2;
                    break;
                case 3:
                    name = "Manic";
                    _stack = 3;
                    break;
            }

            desc =
                $"Gain {_stack} [Haste] each Scene.[On Dice Roll]Boost the *maximum* Dice Roll by {_stack} or Lower the *maximum* Dice Roll by {_stack} at {_stack * 10}% chance.At the end of each Scene change all Emotions Coin Type in [Positive Coin]";
        }

        public override void OnRoundStartAfter()
        {
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Quickness, _stack);
        }

        public void RemoveBuff()
        {
            EmotionalBurstUtilities.DecreaseStacksBufType(owner, KeywordBuf.Quickness, _stack);
        }

        public void AfterInit()
        {
            OnRoundStartAfter();
        }

        public void InstantIncrease()
        {
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Quickness, 1);
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            var number = RndChance.Next(0, 100);
            var value = _stack;
            var buffTypeNegative = number <= _stack * 10;
            if (behavior.GetDiceVanillaMax() - value < behavior.GetDiceVanillaMin() && buffTypeNegative)
                value = behavior.GetDiceVanillaMax() - behavior.GetDiceVanillaMin();
            isNegative = buffTypeNegative;
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.ApplyDiceStatBonus(buffTypeNegative
                ? new DiceStatBonus {max = value * -1}
                : new DiceStatBonus {max = value});
        }

        public override void OnRoundEnd()
        {
            ChangeAllCoinsToPositiveType();
        }

        private void ChangeAllCoinsToPositiveType()
        {
            owner.emotionDetail.totalEmotionCoins.RemoveAll(x => x.CoinType == EmotionCoinType.Negative);
            var allEmotionCoins = owner.emotionDetail.AllEmotionCoins.Where(x => x.CoinType == EmotionCoinType.Negative)
                .ToList();
            foreach (var coin in allEmotionCoins)
            {
                owner.emotionDetail.AllEmotionCoins.Remove(coin);
                owner.battleCardResultLog?.AddEmotionCoin(EmotionCoinType.Positive,
                    owner.emotionDetail.CreateEmotionCoin(EmotionCoinType.Positive));
            }
        }
    }
}