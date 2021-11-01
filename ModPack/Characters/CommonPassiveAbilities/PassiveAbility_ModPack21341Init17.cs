using System.Linq;

namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //Sad
    public class PassiveAbility_ModPack21341Init17 : PassiveAbilityBase
    {
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
                    name = "Sad";
                    _stack = 1;
                    break;
                case 2:
                    name = "Depressed";
                    _stack = 2;
                    break;
                case 3:
                    name = "Miserable";
                    _stack = 3;
                    break;
            }

            desc =
                $"Gain {_stack} [Endurance] and {_stack * 2} [Protection], inflict on self {_stack} [Bind] each Scene.At the end of each Scene change all Emotions Coin Type in [Negative Coin]";
        }

        public override void OnRoundStartAfter()
        {
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Endurance, _stack);
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Binding, _stack);
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Protection, _stack * 2);
        }

        public override void OnRoundEnd()
        {
            ChangeAllCoinsToNegativeType();
        }

        private void ChangeAllCoinsToNegativeType()
        {
            owner.emotionDetail.totalEmotionCoins.RemoveAll(x => x.CoinType == EmotionCoinType.Positive);
            var allEmotionCoins = owner.emotionDetail.AllEmotionCoins.Where(x => x.CoinType == EmotionCoinType.Positive)
                .ToList();
            foreach (var coin in allEmotionCoins)
            {
                owner.emotionDetail.AllEmotionCoins.Remove(coin);
                owner.battleCardResultLog?.AddEmotionCoin(EmotionCoinType.Negative,
                    owner.emotionDetail.CreateEmotionCoin(EmotionCoinType.Negative));
            }
        }
    }
}