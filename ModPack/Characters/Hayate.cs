using System.Linq;
using LOR_XML;
using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;
using ModPack21341.Utilities;

namespace ModPack21341.Characters
{
    public class PassiveAbility_Hayate : PassiveAbilityBase
    {
        private BattleUnitBuf_EntertainMeBuf _buf;
        private bool _phase2;
        private bool _changeBuffs;
        private bool _useCard;
        private bool _lastPrePhase;
        private bool _finalPhase;
        private bool _auraChange;
        public override void OnWaveStart()
        {
            _phase2 = false;
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 931));
            if(owner.faction == Faction.Enemy) owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_HayateImmortal());
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_GodAuraRelease());
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_EntertainMeBuf());
            _buf = owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_EntertainMeBuf) as
                    BattleUnitBuf_EntertainMeBuf;
        }

        public override void OnRoundEnd()
        {
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
            if(_auraChange)
                UnitUtilities.ActiveAwakeningDeckPassive(owner,"Hayate");
        }

        public bool GetPhase2Status() => _phase2;
        public override void OnRoundStart()
        {
            if (_auraChange)
            {
                _auraChange = false;
                owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_GodAuraRelease));
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_TrueGodAura());
                owner.breakDetail.ResetGauge();
                owner.breakDetail.RecoverBreakLife(1, true);
                owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            }
            if (!_changeBuffs) return;
            _changeBuffs = false;
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_GodAuraRelease));
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_TrueGodAura());
            if (owner.faction != Faction.Enemy) return;
            owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "0");
        }
        public override void OnRoundStartAfter()
        {
            if (!_lastPrePhase || _finalPhase) return;
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
        }

        public void SetFinalPhase(bool value) => _finalPhase = value;
        public override BattleUnitModel ChangeAttackTarget(BattleDiceCardModel card, int idx)
        {
            if (card.GetID().id != 54) return base.ChangeAttackTarget(card, idx);
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
            {
                owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 930));
            }

            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 54) || curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 55))
            {
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
            }
        }

        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            if(_lastPrePhase && !_finalPhase || owner.faction == Faction.Player) return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            if (_useCard)
            {
                _useCard = false;
                origin = BattleDiceCardModel.CreatePlayingCard(
                    ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, 54)));
                return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            }
            if (_buf.stack < 40 || owner.faction != Faction.Enemy || owner.IsBreakLifeZero())
                return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            _buf.stack = 0;
            origin = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, 54)));
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        private void ChangePhase()
        {
            _useCard = true;
            _phase2 = true;
            _changeBuffs = true;
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ImmortalBuffUntilRoundEnd());
            owner.RecoverHP(owner.MaxHp);
            owner.breakDetail.ResetGauge();
            owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 47));
        }
    }
    public class PassiveAbility_TrueKurosawaBlade : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.ApplyDiceStatBonus(new DiceStatBonus { power = 1 });
        }

        public override void OnSucceedAttack(BattleDiceBehavior behavior) => RecoverHpAndStagger();

        private void RecoverHpAndStagger()
        {
            owner.RecoverHP(3);
            owner.breakDetail.RecoverBreak(3);
        }
    }
    public class PassiveAbility_HayateShimmering : PassiveAbilityBase
    {
        public override void OnRoundStart() => SetCards();
        public override int SpeedDiceNumAdder() => 4;
        private void SetCards()
        {
            owner.allyCardDetail.ExhaustAllCards();
            AddNewCard(new LorId(ModPack21341Init.PackageId, 49));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 53));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 56));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 48));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 48));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 52));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 52));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 51));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 50));
        }
        private void AddNewCard(LorId id)
        {
            var card = owner.allyCardDetail.AddTempCard(id);
            card?.SetCostToZero();
        }
    }
    public class PassiveAbility_HighDivinity : PassiveAbilityBase
    {
        public override void OnRoundStartAfter()
        {
            UnitUtilities.DrawUntilX(owner, 5);
        }
    }
}
