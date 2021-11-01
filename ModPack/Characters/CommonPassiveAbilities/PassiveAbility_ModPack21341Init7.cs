using ModPack21341.Utilities;

namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //Angry
    public class PassiveAbility_ModPack21341Init7 : PassiveAbilityBase
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
                    name = "Angry";
                    _stack = 1;
                    break;
                case 2:
                    name = "Enraged";
                    _stack = 2;
                    break;
                case 3:
                    name = "Furious";
                    _stack = 3;
                    break;
            }

            desc =
                $"Gain {_stack} [Strength],inflict on self {_stack} [Disarm] and {_stack * 2} [Fragile] each Scene.Each time this Character takes damage Gain 1 [Negative Emotion Coin]";
        }

        public override void OnRoundStartAfter()
        {
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, _stack);
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Disarm, _stack);
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Vulnerable, _stack * 2);
        }

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            UnitUtilities.SetPassiveCombatLog(this, owner);
            owner.battleCardResultLog?.AddEmotionCoin(EmotionCoinType.Negative,
                owner.emotionDetail.CreateEmotionCoin(EmotionCoinType.Negative));
            return base.BeforeTakeDamage(attacker, dmg);
        }
    }
}