using System.Collections.Generic;
using System.Linq;
using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;
using ModPack21341.Utilities;
using Sound;

namespace ModPack21341.Characters
{
    public class PassiveAbility_RolandEnemyDesc : PassiveAbilityBase
    {

    }
    public class PassiveAbility_RolandUnit : PassiveAbilityBase
    {
        private bool _blackCheck;
        private string _originalSkinName;
        private BattleDialogueModel _dlg;
        private bool _halfHpReached;
        private int _count;
        private bool _specialActivated;
        public override void OnBattleEnd()
        {
            if (!string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin)) return;
            UnitUtilities.ReturnToTheOriginalBaseSkin(owner, _originalSkinName, _dlg);
        }

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (owner.faction != Faction.Enemy || !(owner.hp - dmg <= owner.MaxHp * 0.5f) || _blackCheck || _halfHpReached)
                return base.BeforeTakeDamage(attacker, dmg);
            InitEgoChange();
            return base.BeforeTakeDamage(attacker, dmg);
        }

        private void InitEgoChange()
        {
            _halfHpReached = true;
            _blackCheck = true;
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ImmortalBuffUntiLRoundEnd());
            owner.SetHp(owner.MaxHp / 2);
            owner.cardSlotDetail.RecoverPlayPointByCard(6);
            owner.breakDetail.RecoverBreak(owner.breakDetail.GetDefaultBreakGauge());
        }
        private void CheckRolandUnitAndChangeSkin()
        {
            if (!owner.passiveDetail.HasPassive<PassiveAbility_Orlando>() && !owner.passiveDetail.HasPassive<PassiveAbility_10012>())
                owner.passiveDetail.DestroyPassive(this);
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin))
                UnitUtilities.PrepareSephirahSkin(owner, 10, "Roland", owner.faction == Faction.Enemy, ref _originalSkinName, ref _dlg, true, "BlackSilence", true);
        }
        public override void OnWaveStart()
        {
            CheckRolandUnitAndChangeSkin();
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.packageId, 911));
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID().id == 29 || curCard.card.GetID().id == 26)
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
            if (curCard.card.GetID() != new LorId(ModPack21341Init.packageId, 911)) return;
            owner.personalEgoDetail.RemoveCard(curCard.card.GetID());
            _blackCheck = true;
        }
        public override void OnRoundEndTheLast()
        {
            _specialActivated = false;
            if (_blackCheck)
                ChangeToBlackSilence();
            if (owner.faction == Faction.Enemy && owner.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_BlackMaskSilence))
                _count++;
        }
        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            if (owner.faction != Faction.Enemy) return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            if (owner.bufListDetail.GetActivatedBufList()
                .Find(x => x is PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount) is PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount buf)
            {
                UseFuriosoCard(ref origin,buf);
                return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            }
            UseEgoMassAttackCard(ref origin);
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        private void UseFuriosoCard(ref BattleDiceCardModel origin, BattleUnitBuf buf)
        {
            if (buf.stack < 9 || _specialActivated || owner.cardSlotDetail.PlayPoint < 3) return;
            _specialActivated = true;
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.packageId, 29)));
        }

        private void UseEgoMassAttackCard(ref BattleDiceCardModel origin)
        {
            if (!owner.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_BlackMaskSilence && _count >= 4) || owner.cardSlotDetail.PlayPoint < 5)
                return;
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.packageId, 26)));
            _count = 0;
        }
        private void ChangeToBlackSilence()
        {
            _blackCheck = false;
            if (owner.bufListDetail.GetActivatedBufList()
                .Find(x => x is PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount) is PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount)
            {
                owner.bufListDetail.RemoveBufAll(typeof(PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount));
            }
            if (owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Orlando) is PassiveAbility_Orlando passiveAbilityBase)
            {
                owner.passiveDetail.DestroyPassive(passiveAbilityBase);
            }
            if (owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_10012) is PassiveAbility_10012 passiveAbilityBaseOriginal)
            {
                owner.passiveDetail.DestroyPassive(passiveAbilityBaseOriginal);
            }
            foreach (var battleDiceCardModel in owner.allyCardDetail.GetAllDeck())
            {
                battleDiceCardModel.RemoveBuf<PassiveAbility_10012.BattleDiceCardBuf_blackSilenceEgoCount>();
            }
            owner.personalEgoDetail.RemoveCard(702010);
            owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.packageId, 913));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.packageId, 910));
            ChangeToBlackSilenceMaskDeck();
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_BlackMaskSilence());
            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Kali_Change");
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin))
                owner.view.ChangeSkin("BlackSilence3");
            if (owner.faction == Faction.Enemy) _count = 4;
        }

        private void ChangeToBlackSilenceMaskDeck()
        {
            if (!(owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.packageId, 10)) is PassiveAbility_CheckDeck
                passive)) return;
            passive.Init(owner);
            passive.SaveAwakenedDeck(UnitUtilities.GetBlackSilenceMaskCardsId());
            passive.ChangeToTheBlackSilenceMaskDeck();
        }
    }
    public class PassiveAbility_Orlando : PassiveAbilityBase
    {
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            var cardId = curCard.card.GetID();
            if (cardId.IsWorkshop())
            {
                return;
            }
            var id2 = cardId.id;
            if (!_usedCount.Contains(cardId) && (id2 >= 702001 && id2 <= 702009 || id2 >= 706101 && id2 <= 706109))
            {
                _usedCount.Add(cardId);
            }
        }
        public override void OnWaveStart()
        {
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.packageId, 913));
        }
        public override void OnRoundStart()
        {
            foreach (var battleDiceCardModel in owner.allyCardDetail.GetAllDeck())
            {
                battleDiceCardModel.RemoveBuf<PassiveAbility_10012.BattleDiceCardBuf_blackSilenceEgoCount>();
            }
            foreach (var battleDiceCardModel2 in from battleDiceCardModel2 in owner.allyCardDetail.GetAllDeck() let id = battleDiceCardModel2.GetID() where !id.IsWorkshop() let id2 = id.id where !_usedCount.Contains(id) && ((id2 >= 702001 && id2 <= 702009) || (id2 >= 706101 && id2 <= 706109)) select battleDiceCardModel2)
            {
                battleDiceCardModel2.AddBuf(new PassiveAbility_10012.BattleDiceCardBuf_blackSilenceEgoCount());
            }
            owner.bufListDetail.RemoveBufAll(typeof(PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount));
            owner.bufListDetail.AddBuf(new PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount
            {
                stack = _usedCount.Count
            });
        }
        public bool IsActivatedSpecialCard()
        {
            return _usedCount.Count >= 9;
        }
        public void ResetUsedCount()
        {
            _usedCount.Clear();
        }
        private List<LorId> _usedCount = new List<LorId>();
    }
    public class PassiveAbility_TheBlackSilence : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            owner.allyCardDetail.DrawCards(2);
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (_count == 2)
            {
                curCard.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
                {
                    power = 2
                });
                _count = 0;
            }
            else
            {
                _count++;
            }
            owner.bufListDetail.RemoveBufAll(typeof(PassiveAbility_10013.BattleUnitBuf_blackSilenceCardCount));
            if (_count > 0)
            {
                owner.bufListDetail.AddBuf(new PassiveAbility_10013.BattleUnitBuf_blackSilenceCardCount
                {
                    stack = _count
                });
            }
        }
        private int _count;
    }
}
