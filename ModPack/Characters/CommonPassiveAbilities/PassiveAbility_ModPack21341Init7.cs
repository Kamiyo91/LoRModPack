using System.Linq;
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
                $"Gain {_stack} [Strength],inflict on self {_stack} [Disarm] and {_stack * 3} [Fragile] each Scene.Each time this Character takes damage Gain 1 [Negative Emotion Coin]";
        }

        public override void OnRoundStartAfter()
        {
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, _stack);
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Disarm, _stack);
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Vulnerable, _stack * 3);
        }

        public void RemoveBuff()
        {
            EmotionalBurstUtilities.DecreaseStacksBufType(owner, KeywordBuf.Strength, _stack);
            EmotionalBurstUtilities.DecreaseStacksBufType(owner, KeywordBuf.Disarm, _stack);
            EmotionalBurstUtilities.DecreaseStacksBufType(owner, KeywordBuf.Vulnerable, _stack * 3);
        }

        public void InstantIncrease()
        {
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Strength, 1);
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Disarm, 1);
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Vulnerable, 3);
        }

        public void DecreaseStacksBufType(KeywordBuf bufType, int stacks)
        {
            var buf = owner.bufListDetail.GetActivatedBufList().FirstOrDefault(x => x.bufType == bufType);
            if (buf != null) buf.stack -= stacks;
            if (buf != null && buf.stack < 1) owner.bufListDetail.RemoveBuf(buf);
        }

        public void AfterInit()
        {
            OnRoundStartAfter();
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