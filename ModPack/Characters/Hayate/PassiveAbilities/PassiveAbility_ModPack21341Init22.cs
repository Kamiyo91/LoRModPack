using System.Linq;
using LOR_XML;
using ModPack21341.Characters.CommonBuffs;
using ModPack21341.Characters.Hayate.Buffs;
using ModPack21341.Characters.Mio.Buffs;
using ModPack21341.Harmony;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Hayate.PassiveAbilities
{
    //Hayate
    public class PassiveAbility_ModPack21341Init22 : PassiveAbilityBase
    {
        private bool _auraChange;
        private BattleUnitBuf_ModPack21341Init8 _buf;
        private bool _changeBuffs;
        private bool _deleteEnemy;
        private bool _finalPhase;
        private bool _lastPrePhase;
        private bool _oneTurnCard;
        private bool _phase2;
        private bool _showLastPrePhaseDlg;
        private BattleUnitModel _target;
        private bool _useCard;

        public override void OnWaveStart()
        {
            _phase2 = false;
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 931));
            if (owner.faction == Faction.Enemy)
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init9());
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init15());
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init8());
            _buf = owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_ModPack21341Init8) as
                BattleUnitBuf_ModPack21341Init8;
        }

        public override void OnRoundEnd()
        {
            _oneTurnCard = false;
            if (_phase2 && owner.faction == Faction.Enemy)
                _buf.stack += 10;
            else
                _buf.stack += 5;
            if (_lastPrePhase && !_finalPhase)
            {
                owner.breakDetail.RecoverBreakLife(1, true);
                owner.breakDetail.nextTurnBreak = false;
            }

            if (!_changeBuffs) return;
            owner.breakDetail.RecoverBreakLife(1, true);
            owner.breakDetail.nextTurnBreak = false;
        }

        public override void OnRoundEndTheLast()
        {
            if (owner.faction == Faction.Enemy)
                owner.allyCardDetail.ExhaustCard(new LorId(ModPack21341Init.PackageId, 54));
            if (_deleteEnemy)
            {
                _deleteEnemy = false;
                BattleObjectManager.instance.UnregisterUnit(_target);
                _target = null;
                UnitUtilities.RefreshCombatUI();
            }

            if (_auraChange)
                UnitUtilities.ActiveAwakeningDeckPassive(owner, "Hayate");
        }

        public bool GetPhase2Status()
        {
            return _phase2;
        }

        public override void OnRoundStart()
        {
            if (_auraChange)
            {
                _auraChange = false;
                owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ModPack21341Init15));
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init10());
                owner.breakDetail.ResetGauge();
                owner.breakDetail.RecoverBreakLife(1, true);
                owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            }

            if (!_changeBuffs) return;
            _changeBuffs = false;
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ModPack21341Init15));
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init10());
            if (owner.faction != Faction.Enemy) return;
            owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "0");
        }

        public override void OnRoundStartAfter()
        {
            if (!_lastPrePhase || _finalPhase) return;
            if (_showLastPrePhaseDlg)
            {
                _showLastPrePhaseDlg = false;
                owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "1");
            }

            owner.allyCardDetail.ExhaustAllCards();
            owner.allyCardDetail.AddTempCard(new LorId(ModPack21341Init.PackageId, 55));
            owner.breakDetail.RecoverBreakLife(1, true);
            _buf.stack = 0;
        }

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (owner.hp - dmg <= owner.MaxHp * 0.5f && !_phase2 && owner.faction == Faction.Enemy)
                ChangePhase();
            if (owner.hp - dmg <= 1 && !_lastPrePhase && owner.faction == Faction.Enemy)
                ChangeToFinalPhase();
            return base.BeforeTakeDamage(attacker, dmg);
        }

        private void ChangeToFinalPhase()
        {
            _lastPrePhase = true;
            _showLastPrePhaseDlg = true;
        }

        public void SetFinalPhase(bool value)
        {
            _finalPhase = value;
        }

        public override BattleUnitModel ChangeAttackTarget(BattleDiceCardModel card, int idx)
        {
            if (card.GetID() != new LorId(ModPack21341Init.PackageId, 54)) return base.ChangeAttackTarget(card, idx);
            if (BattleObjectManager.instance
                .GetAliveList(Faction.Player).Any(x => !x.UnitData.unitData.isSephirah))
                return RandomUtil.SelectOne(BattleObjectManager.instance.GetAliveList(Faction.Player)
                    .Where(x => !x.UnitData.unitData.isSephirah).ToList());
            return base.ChangeAttackTarget(card, idx);
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 931))
            {
                _auraChange = true;
                owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 931));
                owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 930));
            }

            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 930))
                owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 930));

            if (curCard.card.GetID() != new LorId(ModPack21341Init.PackageId, 54) &&
                curCard.card.GetID() != new LorId(ModPack21341Init.PackageId, 55)) return;
            _buf.stack = 0;
            if (_useCard) _useCard = false;
            owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
            if (curCard.card.GetID() != new LorId(ModPack21341Init.PackageId, 54)) return;
            _deleteEnemy = true;
            _target = curCard.target;
        }

        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            if (_lastPrePhase || owner.faction == Faction.Player)
                return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            if (_useCard && !_oneTurnCard)
            {
                _oneTurnCard = true;
                origin = BattleDiceCardModel.CreatePlayingCard(
                    ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, 54)));
                return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            }

            if (_buf.stack < 40 || owner.faction != Faction.Enemy || owner.IsBreakLifeZero() || _oneTurnCard)
                return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            _oneTurnCard = true;
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, 54)));
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        private void ChangePhase()
        {
            _useCard = true;
            _phase2 = true;
            _changeBuffs = true;
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init4());
            owner.RecoverHP(owner.MaxHp);
            owner.breakDetail.ResetGauge();
            owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 47));
        }
    }
}