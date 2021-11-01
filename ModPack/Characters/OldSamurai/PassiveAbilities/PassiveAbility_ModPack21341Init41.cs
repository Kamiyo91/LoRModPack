using LOR_DiceSystem;
using ModPack21341.Characters.OldSamurai.Buffs;
using ModPack21341.Characters.OldSamurai.CardAbilities;
using ModPack21341.Harmony;

namespace ModPack21341.Characters.OldSamurai.PassiveAbilities
{
    //ControlledBreathing
    public class PassiveAbility_ModPack21341Init41 : PassiveAbilityBase
    {
        private int _count;
        private int _enemyCount = 3;
        private bool _lightUse;

        private void AddDeepBreathingCard()
        {
            _ = owner.personalEgoDetail.GetHand().Exists(x => x.GetID() == new LorId(ModPack21341Init.PackageId, 900))
                ? _count = 0
                : _count++;
            if (_count != 3) return;
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 900));
            _count = 0;
        }

        private void RemoveDeepBreathingBuff()
        {
            if (owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init42) is
                PassiveAbility_ModPack21341Init42
                passiveAbilityDeepBreath)
                owner.passiveDetail.DestroyPassive(passiveAbilityDeepBreath);
        }

        public override void OnRoundEnd()
        {
            AddDeepBreathingCard();
            RemoveDeepBreathingBuff();
        }

        public override void OnRoundStart()
        {
            if (!_lightUse) return;
            _lightUse = false;
            owner.cardSlotDetail.SpendCost(2);
        }

        public override void OnRoundEndTheLast()
        {
            if (owner.faction != Faction.Enemy && !owner.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_ModPack21341Init20))
                return;

            _enemyCount++;
            if (owner.RollSpeedDice().FindAll(x => !x.breaked).Count <= 0 || owner.IsBreakLifeZero()) return;

            if (_enemyCount <= 2) return;
            if (owner.cardSlotDetail.PlayPoint < 2) return;
            UseDeepBreathingCardNpc();
        }

        private void UseDeepBreathingCardNpc()
        {
            _lightUse = true;
            _enemyCount = 0;
            owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 900));
            DiceCardSelfAbility_ModPack21341Init24.Activate(owner);
        }

        public override void OnWaveStart()
        {
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 900));
        }


        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            owner.currentDiceAction.ignorePower = true;
            EmotionDiceRoll(behavior);
            AtkDiceRoll(behavior);
            DefDiceRoll(behavior);
            PowernullDiceRoll(behavior);
        }

        private void EmotionDiceRoll(BattleDiceBehavior behavior)
        {
            var diceMaxRollAdder = 0;
            if (owner.emotionDetail.EmotionLevel > 2)
                diceMaxRollAdder = 1;
            behavior.ApplyDiceStatBonus(new DiceStatBonus {min = 1, max = diceMaxRollAdder});
        }

        private void AtkDiceRoll(BattleDiceBehavior behavior)
        {
            var negativeNum = 0;
            var positiveNum = 0;
            if (behavior.Detail == BehaviourDetail.Slash || behavior.Detail == BehaviourDetail.Hit ||
                behavior.Detail == BehaviourDetail.Penetrate)
            {
                negativeNum = owner.bufListDetail.GetKewordBufStack(KeywordBuf.Weak);
                positiveNum = owner.bufListDetail.GetKewordBufStack(KeywordBuf.Strength);
                if (positiveNum > 0)
                    positiveNum /= 3;
            }

            var diceMinRollAdder = positiveNum - negativeNum;
            if (diceMinRollAdder < 0 && behavior.GetDiceVanillaMin() - diceMinRollAdder < 1)
                diceMinRollAdder = behavior.GetDiceVanillaMin() * -1 + 1;
            behavior.ApplyDiceStatBonus(new DiceStatBonus {min = diceMinRollAdder, max = positiveNum});
        }

        private void DefDiceRoll(BattleDiceBehavior behavior)
        {
            var negativeNum = 0;
            var positiveNum = 0;
            if (behavior.Detail == BehaviourDetail.Guard || behavior.Detail == BehaviourDetail.Evasion)
            {
                negativeNum = owner.bufListDetail.GetKewordBufStack(KeywordBuf.Disarm);
                positiveNum = owner.bufListDetail.GetKewordBufStack(KeywordBuf.Endurance);
                if (positiveNum > 0)
                    positiveNum /= 3;
            }

            var diceMinRollAdder = positiveNum - negativeNum;
            if (diceMinRollAdder < 0 && behavior.GetDiceVanillaMin() - diceMinRollAdder < 1)
                diceMinRollAdder = behavior.GetDiceVanillaMin() * -1 + 1;
            behavior.ApplyDiceStatBonus(new DiceStatBonus {min = diceMinRollAdder, max = positiveNum});
        }

        private void PowernullDiceRoll(BattleDiceBehavior behavior)
        {
            if (!owner.bufListDetail.GetActivatedBufList().Exists(x => x.bufType == KeywordBuf.NullifyPower)) return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus {min = 1, max = 1});
        }
    }
}