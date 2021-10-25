using LOR_DiceSystem;
using LOR_XML;
using System.Collections.Generic;
using System.Linq;
using ModPack21341.Harmony;
using ModPack21341.Utilities;
using System;
using ModPack21341.Characters.CardAbilities;
using ModPack21341.Models;

namespace ModPack21341.Characters
{
    public class PassiveAbility_Kurosawa_Blade : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail != BehaviourDetail.Slash) return;
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.ApplyDiceStatBonus(new DiceStatBonus { power = 2 });
        }

        public override void OnSucceedAttack(BattleDiceBehavior behavior) => RecoverHpAndStagger();

        private void RecoverHpAndStagger()
        {
            owner.RecoverHP(2);
            owner.breakDetail.RecoverBreak(2);
        }
    }
    public class PassiveAbility_Mask_of_Perception : PassiveAbilityBase
    {
        private bool _buffCheck;
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail != BehaviourDetail.Evasion) return;
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.ApplyDiceStatBonus(new DiceStatBonus { power = 2 });
            if (!behavior.IsParrying() || _buffCheck) return;
            AddHasteBuff();
        }

        private void AddHasteBuff()
        {
            _buffCheck = true;
            owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Quickness, 1, owner);
        }
        public override void OnRoundStartAfter() => _buffCheck = false;

    }
    public class PassiveAbility_EvadeCounter : PassiveAbilityBase
    {
        private bool _recoveryCheck;
        public override void OnStartBattle()
        {
            _recoveryCheck = false;
            UnitUtilities.ReadyCounterCard(owner, 15);
        }
        public override void OnWinParrying(BattleDiceBehavior behavior)
        {
            if (behavior.card.card.GetID().id != 15) return;
            if (_recoveryCheck) return;
            _recoveryCheck = true;
            UnitUtilities.SetPassiveCombatLog(this, owner);
            owner.cardSlotDetail.RecoverPlayPoint(1);
        }
    }
    public class PassiveAbility_SlashCounter : PassiveAbilityBase
    {
        public override void OnStartBattle() => UnitUtilities.ReadyCounterCard(owner, 24);
    }
    public class PassiveAbility_CheckDeck : PassiveAbilityBase
    {
        private bool _awakenedDeckIsActive;
        private List<int> _awakenedDeck;
        private List<int> _originalDeck;
        private bool _activated;
        public override void Init(BattleUnitModel self)
        {
            base.Init(self);
            Hide();
            _awakenedDeckIsActive = false;
            _originalDeck = owner.allyCardDetail.GetAllDeck().Select(x => x.GetID().id).ToList();
            _activated = !owner.passiveDetail.PassiveList.Exists(x => x is PassiveAbility_RolandUnit);
        }

        public override void OnRoundStart()
        {
            if (_activated)
                UnitUtilities.DeckVariantActivated(owner);
        }

        public void ChangeDeck()
        {
            owner.view.speedDiceSetterUI.DeselectAll();
            var count = owner.allyCardDetail.GetHand().Count;
            if (_awakenedDeckIsActive)
            {
                UnitUtilities.ChangeDeck(owner, _originalDeck);
                _awakenedDeckIsActive = false;
            }
            else
            {
                UnitUtilities.ChangeDeck(owner, _awakenedDeck);
                _awakenedDeckIsActive = true;
            }
            owner.allyCardDetail.DrawCards(count);
        }

        public void ChangeToTheBlackSilenceMaskDeck()
        {
            owner.view.speedDiceSetterUI.DeselectAll();
            var count = owner.allyCardDetail.GetHand().Count;
            ChangeDeckBlack();
            owner.allyCardDetail.DrawCards(count);
        }

        private void ChangeDeckBlack()
        {
            owner.allyCardDetail.ExhaustAllCards();
            foreach (var cardId in _awakenedDeck.Where(cardId => cardId > 100))
            {
                owner.allyCardDetail.AddNewCardToDeck(cardId);
            }
            foreach (var cardId in _awakenedDeck.Where(cardId => cardId < 100))
            {
                owner.allyCardDetail.AddNewCardToDeck(new LorId(ModPack21341Init.packageId, cardId));
            }
        }
        public void SaveAwakenedDeck(List<int> awakenedDeck) => _awakenedDeck = awakenedDeck;

    }
    public class PassiveAbility_Hod : PassiveAbilityBase
    {
        private BattleDialogueModel _dlg;
        public override void OnWaveStart() => ChangeHodDialog();
        private void ChangeHodDialog()
        {
            _dlg = owner.UnitData.unitData.battleDialogModel;
            owner.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.packageId, 200));
            owner.view.DisplayDlg(DialogType.START_BATTLE, "0");
        }
        public override void OnBattleEnd() => owner.UnitData.unitData.battleDialogModel = _dlg;
    }
    public class PassiveAbility_Loneliness : PassiveAbilityBase
    {
        public override void OnRoundEnd()
        {
            if (BattleObjectManager.instance.GetAliveList(owner.faction).Count == 1)
            {
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 3);
            }
        }
    }

    #region EmotionalBurstPassive
    public class PassiveAbility_Happy : PassiveAbilityBase
    {
        private int _stack;
        private static Random _rndChance = new Random();
        public int GetStack() => _stack;
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
                $"Gain {_stack * 2} [Haste] each Scene.[On Dice Roll]Boost the *maximum* Dice Roll by {_stack * 2} or Lower the *maximum* Dice Roll by {_stack * 2} at {_stack * 10}% chance.At the end of each Scene change all Emotions Coin Type in [Positive Coin]";
        }
        public override void OnRoundStartAfter()
        {
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.Quickness, _stack);
        }
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            var number = _rndChance.Next(0, 100);
            var value = _stack * 2;
            var buffTypeNegative = number <= _stack * 10;
            if (behavior.GetDiceVanillaMax() - value < behavior.GetDiceVanillaMin() && buffTypeNegative)
                value = behavior.GetDiceVanillaMax() - behavior.GetDiceVanillaMin();
            isNegative = buffTypeNegative;
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.ApplyDiceStatBonus(buffTypeNegative
                ? new DiceStatBonus { max = value * -1 }
                : new DiceStatBonus { max = value });

        }
        public override void OnRoundEnd() => ChangeAllCoinsToPositiveType();
        private void ChangeAllCoinsToPositiveType()
        {
            owner.emotionDetail.totalEmotionCoins.RemoveAll(x => x.CoinType == EmotionCoinType.Negative);
            var allEmotionCoins = owner.emotionDetail.AllEmotionCoins.Where(x => x.CoinType == EmotionCoinType.Negative).ToList();
            foreach (var coin in allEmotionCoins)
            {
                owner.emotionDetail.AllEmotionCoins.Remove(coin);
                owner.battleCardResultLog?.AddEmotionCoin(EmotionCoinType.Positive,
                    owner.emotionDetail.CreateEmotionCoin(EmotionCoinType.Positive));
            }
        }
    }
    public class PassiveAbility_Sad : PassiveAbilityBase
    {
        private int _stack;
        public int GetStack() => _stack;
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

        public override void OnRoundEnd() => ChangeAllCoinsToNegativeType();
        private void ChangeAllCoinsToNegativeType()
        {
            owner.emotionDetail.totalEmotionCoins.RemoveAll(x => x.CoinType == EmotionCoinType.Positive);
            var allEmotionCoins = owner.emotionDetail.AllEmotionCoins.Where(x => x.CoinType == EmotionCoinType.Positive).ToList();
            foreach (var coin in allEmotionCoins)
            {
                owner.emotionDetail.AllEmotionCoins.Remove(coin);
                owner.battleCardResultLog?.AddEmotionCoin(EmotionCoinType.Negative,
                    owner.emotionDetail.CreateEmotionCoin(EmotionCoinType.Negative));
            }
        }
    }
    public class PassiveAbility_Angry : PassiveAbilityBase
    {
        private int _stack;
        public int GetStack() => _stack;
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
            owner.battleCardResultLog?.AddEmotionCoin(EmotionCoinType.Negative, owner.emotionDetail.CreateEmotionCoin(EmotionCoinType.Negative));
            return base.BeforeTakeDamage(attacker, dmg);
        }
    }

    public class PassiveAbility_Neutral : PassiveAbilityBase
    {
        public override void OnRoundStartAfter()
        {
            owner.allyCardDetail.DrawCards(1);
            owner.cardSlotDetail.RecoverPlayPoint(1);
        }
    }
    public class PassiveAbility_EmotionalBurst : PassiveAbilityBase
    {
        private int _count;
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
            if (owner.faction != Faction.Enemy || owner.RollSpeedDice().FindAll(x => !x.breaked).Count <= 0 || owner.IsBreakLifeZero())
            {
                return;
            }
            if (_count > 3)
                InitBufEnemy();
            ActiveBuffEnemy();
        }

        private void ActiveBuffEnemy()
        {
            switch (_enemyBuff)
            {
                case Angry:
                    DiceCardSelfAbility_Angry.Activate(owner);
                    break;
                case Sad:
                    DiceCardSelfAbility_Sad.Activate(owner);
                    break;
                case Neutral:
                    DiceCardSelfAbility_Neutral.Activate(owner);
                    break;
                case Happy:
                    DiceCardSelfAbility_Happy.Activate(owner);
                    break;
            }
            _count++;
        }

        private const EmotionBufType Happy = EmotionBufType.Happy;
        private const EmotionBufType Sad = EmotionBufType.Sad;
        private const EmotionBufType Angry = EmotionBufType.Angry;
        private const EmotionBufType Neutral = EmotionBufType.Neutral;
        private readonly List<EmotionBufType> _onlyEnemy = new List<EmotionBufType> { Happy, Sad, Angry, Neutral };
        private EmotionBufType _enemyBuff;
    }
    #endregion
}
