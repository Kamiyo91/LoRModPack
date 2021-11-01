using System.Collections.Generic;
using ModPack21341.Characters.CommonCardAbilities;
using ModPack21341.Models;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //EmotionalBurst
    public class PassiveAbility_ModPack21341Init11 : PassiveAbilityBase
    {
        private const EmotionBufType Happy = EmotionBufType.Happy;
        private const EmotionBufType Sad = EmotionBufType.Sad;
        private const EmotionBufType Angry = EmotionBufType.Angry;
        private const EmotionBufType Neutral = EmotionBufType.Neutral;
        private readonly List<EmotionBufType> _onlyEnemy = new List<EmotionBufType> {Happy, Sad, Angry, Neutral};
        private int _count;
        private EmotionBufType _enemyBuff;

        public override void OnWaveStart()
        {
            InitBufEnemy();
            if (owner.faction != Faction.Enemy) return;
            ActiveBuffEnemy();
            _count++;
        }

        private void InitBufEnemy()
        {
            _enemyBuff = RandomUtil.SelectOne(_onlyEnemy);
            _count = 0;
        }

        public override void OnRoundStart()
        {
            if (owner.faction != Faction.Player) return;
            PrepareEmotionalBurstCards();
        }

        private void PrepareEmotionalBurstCards()
        {
            EmotionalBurstUtilities.RemoveEmotionalBurstCards(owner);
            EmotionalBurstUtilities.AddEmotionalBurstCards(owner);
        }

        public override void OnRoundEnd()
        {
            if (owner.faction != Faction.Enemy || owner.RollSpeedDice().FindAll(x => !x.breaked).Count <= 0 ||
                owner.IsBreakLifeZero()) return;
            if (_count > 3)
                InitBufEnemy();
            ActiveBuffEnemy();
        }

        private void ActiveBuffEnemy()
        {
            switch (_enemyBuff)
            {
                case Angry:
                    DiceCardSelfAbility_ModPack21341Init1.Activate(owner);
                    break;
                case Sad:
                    DiceCardSelfAbility_ModPack21341Init6.Activate(owner);
                    break;
                case Neutral:
                    DiceCardSelfAbility_ModPack21341Init5.Activate(owner);
                    break;
                case Happy:
                    DiceCardSelfAbility_ModPack21341Init4.Activate(owner);
                    break;
            }

            _count++;
        }
    }
}