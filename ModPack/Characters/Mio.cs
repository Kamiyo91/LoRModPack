using System.Linq;
using LOR_XML;
using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;
using ModPack21341.StageManager;
using ModPack21341.Utilities;
using UnityEngine;

namespace ModPack21341.Characters
{
    public class PassiveAbility_CorruptionResist : PassiveAbilityBase
    {
        private bool _dmgResist;
        public override void Init(BattleUnitModel self)
        {
            base.Init(self);
            Hide();
        }

        public override void OnRoundStart()
        {
            _dmgResist = true;
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_OneHitResist());
        }

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (!_dmgResist || attacker == null || !attacker.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_CorruptedGodAuraRelease)) return base.BeforeTakeDamage(attacker, dmg);
            UnitUtilities.SetPassiveCombatLog(this, owner);
            _dmgResist = false;
            return base.BeforeTakeDamage(attacker, dmg);
        }

        public override void OnRoundEnd() => RemoveResistBuf();

        private void RemoveResistBuf()
        {
            if (owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_OneHitResist) is
                BattleUnitBuf_OneHitResist buff)
            {
                owner.bufListDetail.RemoveBuf(buff);
            }
        }
    }
    public class PassiveAbility_God_Fragment : PassiveAbilityBase
    {
        private bool _deathCheck;
        private bool _auraCheck;
        private bool _phaseChanged;
        private bool _usedMassEgo;
        private bool _specialCase;
        private BattleDialogueModel _dlg;
        private BookModel _originalBook;

        public override void OnBattleEnd()
        {
            if (owner.faction == Faction.Player && !_specialCase)
                UnitUtilities.ReturnToTheOriginalPlayerUnit(owner, _originalBook, _dlg);
        }

        public void SetSpecialCase() => _specialCase = true;
        private void SetPhaseChange()
        {
            _phaseChanged = true;
            _auraCheck = true;
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ImmortalBuffUntiLRoundEnd());
            owner.SetHp(owner.MaxHp / 2);
            if (Singleton<StageController>.Instance.EnemyStageManager != null && Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_Mio stage)
                stage.SetPhaseChange();
        }

        private void SetMassAttackReady()
        {
            owner.RecoverHP(owner.MaxHp * 45 / 100);
            if (owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_MioEnemyDesc) is PassiveAbility_MioEnemyDesc passive)
            {
                passive.SetCountValue(4);
            }
        }

        private void ImmortalOnLethalDamage()
        {
            _deathCheck = true;
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ImmortalBuffUntiLRoundEnd());
            owner.SetHp(1);
            owner.breakDetail.nextTurnBreak = false;
            owner.breakDetail.ResetGauge();
            owner.breakDetail.RecoverBreakLife(1, true);
            owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "0");
        }
        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (owner.faction == Faction.Enemy && owner.hp - dmg <= owner.MaxHp * 0.5f && !_phaseChanged)
            {
                SetPhaseChange();
            }
            if (!(owner.hp - dmg <= 0) || _deathCheck)
                return base.BeforeTakeDamage(attacker, dmg);
            ImmortalOnLethalDamage();
            if (owner.faction == Faction.Enemy)
            {
                SetMassAttackReady();
            }
            else
            {
                owner.RecoverHP(owner.MaxHp * 70 / 100);
            }
            return base.BeforeTakeDamage(attacker, dmg);
        }
        public override void OnStartBattle() => RemoveImmortalBuff();
        private void RemoveImmortalBuff()
        {
            if (owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_ImmortalBuffUntiLRoundEnd) is
                BattleUnitBuf_ImmortalBuffUntiLRoundEnd buf)
                owner.bufListDetail.RemoveBuf(buf);
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            OnAttackEgoCardUse(curCard);
            OnEgoCardUse(curCard);
        }

        private void OnAttackEgoCardUse(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 904))
            {
                owner.personalEgoDetail.RemoveCard(curCard.card.GetID());
            }
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 19) || curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 25))
            {
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
            }
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 905))
                _usedMassEgo = true;
        }
        private void OnEgoCardUse(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() != new LorId(ModPack21341Init.PackageId, 903)) return;
            _auraCheck = true;
            owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 903));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 904));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 905));
        }

        private void StartEgoTransform()
        {
            _auraCheck = false;
            owner.breakDetail.ResetGauge();
            owner.breakDetail.RecoverBreakLife(1, true);
            owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            if (owner.faction == Faction.Enemy)
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_CorruptedGodAuraRelease());
            else
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_GodAuraRelease());
            owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "1");
        }

        private void ActiveAwakeningDeckPassive()
        {
            if (!(owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 10)) is PassiveAbility_CheckDeck
                passive)) return;
            passive.Init(owner);
            passive.SaveAwakenedDeck(UnitUtilities.GetMioCardsId());
            if (owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
            {
                var skinId = owner.faction == Faction.Player ? 10000200 : 10000201;
                UnitUtilities.ChangeCustomSkin(owner, skinId);
                UnitUtilities.RefreshCombatUI();
            }
            passive.ChangeDeck();
        }
        public override void OnRoundEndTheLast()
        {
            if (!_auraCheck) return;
            StartEgoTransform();
            ActiveAwakeningDeckPassive();
        }
        public override void OnRoundStart()
        {
            if (!_usedMassEgo) return;
            _usedMassEgo = false;
            MapUtilities.ReturnFromEgoMap("Mio", owner, 2);
        }
        public override void OnWaveStart()
        {
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 903));
            InitVariables();
            InitDlgAndCheckSkin();
        }

        private void InitDlgAndCheckSkin()
        {
            if (owner.faction == Faction.Player)
                owner.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, 202));
            if (!string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) ||
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem) return;
            owner.view.ChangeSkin(owner.UnitData.unitData.CustomBookItem.GetCharacterName());
        }
        private void InitVariables()
        {
            _specialCase = false;
            _deathCheck = false;
            _phaseChanged = false;
            _dlg = owner.UnitData.unitData.battleDialogModel;
            _originalBook = owner.UnitData.unitData.GetCustomBookItemData();
        }
        public void SetOriginalBookForStoryBattle(BookModel book) => _originalBook = book;
    }
    public class PassiveAbility_MioEnemyDesc : PassiveAbilityBase
    {
        private bool _awakened;
        private int _count = 4;

        public override void OnRoundEndTheLast_ignoreDead() => CheckMassAttackCard();


        private void CheckMassAttackCard()
        {
            if (_awakened)
                _ = owner.allyCardDetail.GetHand().Exists(x => x.GetID() == new LorId(ModPack21341Init.PackageId, 25)) ? _count = 4 : _count++;
        }
        public void SetAwakened(bool status) => _awakened = status;
        public void SetCountValue(int value) => _count = value;
        public override void OnWaveStart()
        {
            _awakened = false;
        }
        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            PutMassAttackCardOnDice(ref origin);
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        private void PutMassAttackCardOnDice(ref BattleDiceCardModel origin)
        {
            if (!_awakened || _count < 4 || owner.IsBreakLifeZero() || !BattleObjectManager.instance
                .GetAliveList(Faction.Player).Any(x => x.bufListDetail.IsTargetable())) return;
            _count = 0;
            origin = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, 25)));
        }

        public override void OnLevelUpEmotion() => AddAttackEgoCard();

        private void AddAttackEgoCard()
        {
            if (owner.emotionDetail.EmotionLevel == 5)
                owner.allyCardDetail.AddNewCard(new LorId(ModPack21341Init.PackageId, 19));
        }
    }
    public class PassiveAbility_FragmentOfGod : PassiveAbilityBase
    {
        public override void OnRoundStartAfter()
        {
            UnitUtilities.DrawUntilX(owner, 4);
        }

        public override void OnRoundEnd()
        {
            owner.cardSlotDetail.RecoverPlayPoint(1);
        }
    }

    public class PassiveAbility_MioMirror : PassiveAbilityBase
    {
        private bool _usedMassEgo;
        public override void OnWaveStart()
        {
            if (!owner.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_GodAuraRelease))
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_GodAuraRelease());
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 904));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 905));
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 904))
            {
                owner.personalEgoDetail.RemoveCard(curCard.card.GetID());
            }
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 19) || curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 25))
            {
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
            }
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 905))
                _usedMassEgo = true;
        }
        public override void OnRoundStart()
        {
            if (!_usedMassEgo) return;
            _usedMassEgo = false;
            MapUtilities.ReturnFromEgoMap("Mio", owner, 2);
        }
    }
}
