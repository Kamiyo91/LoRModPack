using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.KamiyoStageMaps;
using ModPack21341.Utilities;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModPack21341.Characters
{
    public class PassiveAbility_Power_of_the_Unknown : PassiveAbilityBase
    {
        private bool _deathCheck;
        private bool _auraCheck;
        private bool _egoActivated;
        private bool _phaseChanged;
        private bool _summonMio;
        private bool _summonMioUsed;
        private bool _mapChange;
        private bool _egoMap;
        private bool _dialogActivated;
        private int _count;
        private BattleDialogueModel _dlg;
        private BookModel _originalBook;
        private StageLibraryFloorModel _floor;
        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (!(owner.hp - dmg <= 0) || _deathCheck) return base.BeforeTakeDamage(attacker, dmg);
            if (owner.faction == Faction.Player || _phaseChanged) PrepareEgoChange(true);
            return base.BeforeTakeDamage(attacker, dmg);
        }
        public void Restart()
        {
            _count = 4;
            _egoActivated = false;
            PrepareEgoChange();
            EgoChange();
        }
        private void PrepareEgoChange(bool lethalDamage = false)
        {
            if (!_egoActivated)
            {
                _dialogActivated = true;
                _egoActivated = true;
                _auraCheck = true;
                owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 928));
                owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 929));
                owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 927));
                if (owner.faction == Faction.Player)
                {
                    owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 38));
                    owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 12));
                }
            }
            if (!lethalDamage && !_deathCheck) return;
            _deathCheck = true;
            owner.SetHp(1);
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ImmortalBuffUntiLRoundEnd());
            if (owner.faction == Faction.Player) owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "0");
            if (owner.faction == Faction.Player)
                owner.RecoverHP(owner.MaxHp * 60 / 100);
            else
                owner.RecoverHP(owner.MaxHp * 35 / 100);
            owner.breakDetail.nextTurnBreak = false;
            owner.breakDetail.ResetGauge();
        }

        public override void OnRoundStart()
        {
            if (owner.faction == Faction.Player && !_egoMap && _summonMioUsed &&
                BattleObjectManager.instance.GetAliveList(Faction.Player).All(x => x == owner))
            {
                RemoveEgoMap();
            }
            if (_summonMioUsed)
                SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
            if (!_dialogActivated) return;
            _dialogActivated = false;
            BattleAbDialog(owner.view.dialogUI);
        }

        private static void BattleAbDialog(BattleDialogUI instance)
        {
            var abnormalityCardDialogList = new List<AbnormalityCardDialog>
            {
                new AbnormalityCardDialog {id = "Kamiyo", dialog = "You need me,Don't you?"},
                new AbnormalityCardDialog {id = "Kamiyo", dialog = "Leave it to me!Time to show what I can do!"},
                new AbnormalityCardDialog {id = "Kamiyo", dialog = "Mhm...I guess is my turn now!"}
            };
            var component = instance.GetComponent<CanvasGroup>();
            var dialog = abnormalityCardDialogList[Random.Range(0, abnormalityCardDialogList.Count)].dialog;
            var txtAbnormalityDlg = (TextMeshProUGUI)typeof(BattleDialogUI).GetField("_txtAbnormalityDlg",
                AccessTools.all)?.GetValue(instance);
            txtAbnormalityDlg.text = dialog;
            txtAbnormalityDlg.fontMaterial.SetColor("_GlowColor", SingletonBehavior<BattleManagerUI>.Instance.negativeCoinColor);
            txtAbnormalityDlg.color = SingletonBehavior<BattleManagerUI>.Instance.negativeTextColor;
            var canvas = (Canvas)typeof(BattleDialogUI).GetField("_canvas",
                AccessTools.all)?.GetValue(instance);
            canvas.enabled = true;
            component.interactable = true;
            component.blocksRaycasts = true;
            txtAbnormalityDlg.GetComponent<AbnormalityDlgEffect>().Init();
            var routine = (Coroutine)typeof(BattleDialogUI).GetField("_routine",
                AccessTools.all)?.GetValue(instance);
            var method = typeof(BattleDialogUI).GetMethod("AbnormalityDlgRoutine", AccessTools.all);
            instance.StartCoroutine(method.Invoke(instance, new object[0]) as IEnumerator);
        }
        private void RemoveEgoMap()
        {
            _egoMap = true;
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_OldSamuraiSummonChangeMap));
            MapUtilities.RemoveValueInEgoMap("Kamiyo2");
            MapUtilities.ReturnFromEgoMap("Kamiyo2", owner, 1);
            SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(SingletonBehavior<BattleSceneRoot>
                .Instance.currentMapObject.mapBgm);
            SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
        }
        public override void OnRoundEndTheLast()
        {
            if (_phaseChanged && owner.faction == Faction.Enemy)
                owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            CheckEgoMapUse();
            if (!_auraCheck) return;
            EgoChange();
            ActiveAwakeningDeckPassive();
        }
        public override void OnDie()
        {
            if (!_summonMioUsed || owner.faction == Faction.Enemy && _phaseChanged) return;
            var mio = BattleObjectManager.instance.GetAliveList(owner.faction).FirstOrDefault(x => x != owner);
            mio?.Die();
        }
        private void CheckEgoMapUse()
        {
            if (!_mapChange) return;
            _mapChange = false;
            ChangeToEgoMap();
        }
        private static void ChangeToEgoMap() => MapUtilities.ChangeMap(new MapModel
        {
            Stage = "Kamiyo2",
            StageId = 5,
            IsPlayer = true,
            Component = new Kamiyo2PlayerMapManager(),
            Bgy = 0.475f,
            Fy = 0.225f
        });
        private void EgoChange()
        {
            if (!owner.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_RedAuraRelease))
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_RedAuraRelease());
            if (owner.faction == Faction.Player) owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "1");
            _auraCheck = false;
        }
        public override void OnWaveStart()
        {
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 928));
            InitVar();
            if (owner.faction != Faction.Player) return;
            owner.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, 14));
            owner.view.DisplayDlg(DialogType.START_BATTLE, "0");
        }
        public void SetPhaseChanged()
        {
            _phaseChanged = true;
            _count = 4;
        }
        private void ReturnToTheOriginalPlayerTeam() => UnitUtilities.RemoveUnitData(_floor, "Mio's Memory");
        private void SummonMio()
        {
            _summonMio = false;
            _summonMioUsed = true;
            _mapChange = true;
            var index = owner.index == 1 ? 0 : 1;
            foreach (var unit in BattleObjectManager.instance.GetList(Faction.Player).Where(x => x.index == index))
            {
                BattleObjectManager.instance.UnregisterUnit(unit);
            }
            UnitUtilities.AddNewUnitPlayerSide(_floor, new UnitModel
            {
                Name = "Mio's Memory",
                Pos = index,
                EmotionLevel = 5,
                Sephirah = _floor.Sephirah
            });
            UnitUtilities.RefreshCombatUI();
        }
        private void ActiveAwakeningDeckPassive()
        {
            if (!(owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 10)) is PassiveAbility_CheckDeck
                passive)) return;
            passive.Init(owner);
            passive.SaveAwakenedDeck(UnitUtilities.GetKamiyoCardsId());
            if (owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem && owner.faction == Faction.Player)
            {
                UnitUtilities.ChangeCustomSkin(owner, 10000202);
                UnitUtilities.RefreshCombatUI();
            }
            passive.ChangeDeck();
        }
        public void SetEgoReady()
        {
            _auraCheck = true;
            _egoActivated = true;
        }

        public override void OnRoundEnd()
        {
            if (owner.faction == Faction.Enemy && _phaseChanged)
                _count++;
            if (owner.faction == Faction.Enemy)
                Debug.LogError($"Kamiyo Mass Count {_count}");
            if (_summonMio)
                SummonMio();
        }

        private void InitVar()
        {
            _summonMio = false;
            _phaseChanged = false;
            _egoActivated = false;
            _deathCheck = false;
            _summonMioUsed = false;
            _dlg = owner.UnitData.unitData.battleDialogModel;
            var currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            _floor = Singleton<StageController>.Instance.GetStageModel().GetFloor(currentStageFloorModel.Sephirah);
            _originalBook = owner.UnitData.unitData.GetCustomBookItemData();
            UnitUtilities.FillUnitData(new UnitModel
            {
                Id = 10000008,
                Name = "Mio's Memory",
                DialogId = 2
            }, _floor);
            if (!string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) ||
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem) return;
            owner.view.ChangeSkin(owner.UnitData.unitData.CustomBookItem.GetCharacterName());
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 928))
            {
                PrepareEgoChange();
            }
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 929))
            {
                _summonMio = true;
                owner.personalEgoDetail.RemoveCard(curCard.card.GetID());
            }
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 45))
            {
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
            }
        }

        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            if (owner.faction != Faction.Enemy || owner.IsBreakLifeZero() || !_phaseChanged || _count < 3)
                return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            origin = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, 45)));
            _count = 0;
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        public override void OnBattleEnd()
        {
            owner.UnitData.unitData.battleDialogModel = _dlg;
            if (owner.faction == Faction.Player)
                UnitUtilities.ReturnToTheOriginalPlayerUnit(owner, _originalBook, _dlg);
            if (!_summonMioUsed) return;
            ReturnToTheOriginalPlayerTeam();
        }
    }

    public class PassiveAbility_Incomplete_Demigod : PassiveAbilityBase
    {
        public override void OnRoundStartAfter()
        {
            UnitUtilities.DrawUntilX(owner, 3);
            owner.RecoverHP(2);
            owner.breakDetail.RecoverBreak(2);
        }
    }
    public class PassiveAbility_Mask_of_Perception : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (behavior.Detail != BehaviourDetail.Evasion) return;
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.ApplyDiceStatBonus(new DiceStatBonus { power = 1 });
        }

        public override void OnStartTargetedOneSide(BattlePlayingCardDataInUnitModel attackerCard)
        {
            base.OnStartTargetedOneSide(attackerCard);
            attackerCard?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                max = -1
            });
        }

        public override void OnStartParrying(BattlePlayingCardDataInUnitModel card)
        {
            base.OnStartParrying(card);
            BattlePlayingCardDataInUnitModel battlePlayingCardDataInUnitModel;
            if (card == null)
            {
                battlePlayingCardDataInUnitModel = null;
            }
            else
            {
                var target = card.target;
                battlePlayingCardDataInUnitModel = target?.currentDiceAction;
            }
            var battlePlayingCardDataInUnitModel2 = battlePlayingCardDataInUnitModel;
            battlePlayingCardDataInUnitModel2?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                max = -1
            });
        }
    }

    public class PassiveAbility_OverflowingFire : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            foreach (var unit in BattleObjectManager.instance.GetAliveList(owner.faction == Faction.Player ? Faction.Enemy : Faction.Player))
            {
                unit.bufListDetail.AddKeywordBufByCard(KeywordBuf.Burn, 2, unit);
            }
        }
        public override void OnSucceedAttack(BattleDiceBehavior behavior)
        {
            behavior.card.target.bufListDetail.AddKeywordBufByCard(KeywordBuf.Burn, 1, behavior.card.target);
        }
    }
    public class PassiveAbility_KamiyoShimmering : PassiveAbilityBase
    {
        public override void OnRoundStartAfter() => SetCards();
        public override int SpeedDiceNumAdder() => 2;
        private void SetCards()
        {
            owner.allyCardDetail.ExhaustAllCards();
            AddNewCard(new LorId(ModPack21341Init.PackageId, 33));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 33));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 34));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 34));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 31));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 31));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 36));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 46));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 32));
        }
        private void AddNewCard(LorId id)
        {
            var card = owner.allyCardDetail.AddTempCard(id);
            card?.SetCostToZero();
        }
    }
}
