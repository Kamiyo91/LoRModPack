using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using HarmonyLib;
using LOR_XML;
using ModPack21341.Characters.CommonBuffs;
using ModPack21341.Characters.CommonPassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.Models;
using TMPro;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ModPack21341.Utilities
{
    public static class UnitUtilities
    {
        public static void ChangeDeck(BattleUnitModel owner, IEnumerable<int> newDeck)
        {
            owner.allyCardDetail.ExhaustAllCards();
            foreach (var cardId in newDeck)
                owner.allyCardDetail.AddNewCardToDeck(new LorId(ModPack21341Init.PackageId, cardId));
        }

        public static void PhaseChangeAllPlayerUnitRecoverBonus(int hp, int stagger, int light,
            bool fullLightRecover = false)
        {
            foreach (var unit in BattleObjectManager.instance.GetAliveList(Faction.Player))
            {
                unit.RecoverHP(hp);
                unit.breakDetail.RecoverBreak(stagger);
                var finalLightRecover = fullLightRecover ? unit.cardSlotDetail.GetMaxPlayPoint() : light;
                unit.cardSlotDetail.RecoverPlayPoint(finalLightRecover);
            }
        }

        public static void RefreshCombatUI(bool forceReturn = false)
        {
            foreach (var (battleUnit, num) in BattleObjectManager.instance.GetList()
                .Select((value, i) => (value, i)))
            {
                SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(battleUnit.UnitData.unitData, num, true);
                if (forceReturn)
                    battleUnit.moveDetail.ReturnToFormationByBlink(true);
            }

            BattleObjectManager.instance.InitUI();
        }

        public static void AddOriginalPlayerUnitPlayerSide(int index)
        {
            var allyUnit = Singleton<StageController>.Instance.CreateLibrarianUnit_fromBattleUnitData(index);
            allyUnit.OnWaveStart();
            allyUnit.allyCardDetail.DrawCards(allyUnit.UnitData.unitData.GetStartDraw());
            AddEmotionPassives(allyUnit);
        }

        public static BattleUnitModel AddNewUnitEnemySide(UnitModel unit)
        {
            var unitWithIndex = Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy,
                new LorId(ModPack21341Init.PackageId, unit.Id), unit.Pos);
            unitWithIndex.emotionDetail.SetEmotionLevel(unit.EmotionLevel);
            if (unit.LockedEmotion)
                unitWithIndex.emotionDetail.SetMaxEmotionLevel(unit.MaxEmotionLevel);
            else
                unitWithIndex.emotionDetail.Reset();
            unitWithIndex.cardSlotDetail.RecoverPlayPoint(unitWithIndex.cardSlotDetail.GetMaxPlayPoint());
            unitWithIndex.allyCardDetail.DrawCards(unitWithIndex.UnitData.unitData.GetStartDraw());
            unitWithIndex.formation = new FormationPosition(unitWithIndex.formation._xmlInfo);
            if (unit.AddEmotionPassive)
                AddEmotionPassives(unitWithIndex);
            if (unit.OnWaveStart)
                unitWithIndex.OnWaveStart();
            return unitWithIndex;
        }

        private static void AddEmotionPassives(BattleUnitModel unit)
        {
            var playerUnitsAlive = BattleObjectManager.instance.GetAliveList(Faction.Player);
            if (!playerUnitsAlive.Any()) return;
            foreach (var emotionCard in playerUnitsAlive.FirstOrDefault()
                .emotionDetail.PassiveList.Where(x =>
                    x.XmlInfo.TargetType == EmotionTargetType.AllIncludingEnemy ||
                    x.XmlInfo.TargetType == EmotionTargetType.All))
            {
                if (unit.faction == Faction.Enemy &&
                    emotionCard.XmlInfo.TargetType == EmotionTargetType.All) continue;
                unit.emotionDetail.ApplyEmotionCard(emotionCard.XmlInfo);
            }
        }

        public static BattleUnitModel AddNewUnitPlayerSide(StageLibraryFloorModel floor, UnitModel unit)
        {
            var unitData = new UnitDataModel(new LorId(ModPack21341Init.PackageId, unit.Id), floor.Sephirah);
            unitData.SetCustomName(unit.Name);
            var allyUnit = BattleObjectManager.CreateDefaultUnit(Faction.Player);
            allyUnit.index = unit.Pos;
            allyUnit.grade = unitData.grade;
            allyUnit.formation = floor.GetFormationPosition(allyUnit.index);
            var unitBattleData = new UnitBattleDataModel(Singleton<StageController>.Instance.GetStageModel(), unitData);
            unitBattleData.Init();
            allyUnit.SetUnitData(unitBattleData);
            allyUnit.OnCreated();
            BattleObjectManager.instance.RegisterUnit(allyUnit);
            allyUnit.passiveDetail.OnUnitCreated();
            allyUnit.emotionDetail.SetEmotionLevel(unit.EmotionLevel);
            if (unit.LockedEmotion)
                allyUnit.emotionDetail.SetMaxEmotionLevel(unit.MaxEmotionLevel);
            else
                allyUnit.emotionDetail.Reset();
            allyUnit.allyCardDetail.DrawCards(allyUnit.UnitData.unitData.GetStartDraw());
            allyUnit.cardSlotDetail.RecoverPlayPoint(allyUnit.cardSlotDetail.GetMaxPlayPoint());
            if (unit.AddEmotionPassive)
                AddEmotionPassives(allyUnit);
            allyUnit.OnWaveStart();
            SingletonBehavior<UICharacterRenderer>.Instance.SetCharacter(allyUnit.UnitData.unitData, allyUnit.index,
                true);
            return allyUnit;
        }

        public static void ReturnToTheOriginalPlayerUnit(BattleUnitModel unit, BookModel originalBook,
            BattleDialogueModel originalDialog)
        {
            unit.UnitData.unitData.customizeData.SetCustomData(true);
            unit.UnitData.unitData.ResetTempName();
            unit.UnitData.unitData.EquipBook(unit.Book);
            if (originalBook != null)
                unit.UnitData.unitData.EquipCustomCoreBook(originalBook);
            unit.UnitData.unitData.battleDialogModel = originalDialog;
        }

        public static void ReturnToTheOriginalBaseSkin(BattleUnitModel owner, string originalSkinName,
            BattleDialogueModel dlg)
        {
            owner.UnitData.unitData.CustomBookItem.SetCharacterName(originalSkinName);
            owner.UnitData.unitData.customizeData.SetCustomData(true);
            owner.UnitData.unitData.ResetTempName();
            owner.UnitData.unitData.battleDialogModel = dlg;
        }

        public static void ChangeCustomSkin(BattleUnitModel owner, int skinId)
        {
            owner.UnitData.unitData.SetTemporaryPlayerUnitByBook(new LorId(ModPack21341Init.PackageId, skinId));
            owner.view.CreateSkin();
        }

        public static void PrepareSephirahSkin(BattleUnitModel owner, int id, string charName, bool isNpc,
            ref string originalSkinName, ref BattleDialogueModel dlg, bool baseDlg = false, string charName2 = null,
            bool doubleName = false)
        {
            originalSkinName = owner.UnitData.unitData.CustomBookItem.GetCharacterName();
            dlg = owner.UnitData.unitData.battleDialogModel;
            owner.UnitData.unitData.SetTempName(charName);
            if (!isNpc)
                owner.UnitData.unitData.customizeData.SetCustomData(false);
            owner.view.SetAltSkin(doubleName ? charName2 : charName);
            owner.UnitData.unitData.CustomBookItem.SetCharacterName(doubleName ? charName2 : charName);
            RefreshCombatUI();
            owner.UnitData.unitData.InitBattleDialogByDefaultBook(baseDlg
                ? new LorId(id)
                : new LorId(ModPack21341Init.PackageId, id));
            owner.view.DisplayDlg(DialogType.START_BATTLE, "0");
        }

        public static void TestingUnitValues()
        {
            var playerUnit = BattleObjectManager.instance.GetAliveList(Faction.Player);
            if (playerUnit == null) return;
            foreach (var unit in playerUnit)
            {
                if (!unit.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_ModPack21341Init5))
                    unit.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init5());
                unit.emotionDetail.SetEmotionLevel(5);
            }
        }

        public static void ReadyCounterCard(BattleUnitModel owner, int id)
        {
            var card = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, id)));
            owner.cardSlotDetail.keepCard.AddBehaviours(card, card.CreateDiceCardBehaviorList());
            owner.allyCardDetail.ExhaustCardInHand(card);
        }

        public static void MakeEffect(BattleUnitModel unit, string path, float sizeFactor = 1f,
            BattleUnitModel target = null, float destroyTime = -1f)
        {
            try
            {
                SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect(path, sizeFactor, unit.view,
                    target?.view, destroyTime);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void DeckVariantActivated(BattleUnitModel owner)
        {
            if (owner.personalEgoDetail.GetHand().Exists(x => x.GetID().id == 901)) return;
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 901));
        }

        public static List<int> GetSamuraiCardsId()
        {
            return new List<int> {7, 7, 3, 3, 4, 4, 5, 5, 6};
        }

        public static List<int> GetMioCardsId()
        {
            return new List<int> {20, 20, 21, 22, 23, 1, 1, 13, 13};
        }

        public static List<int> GetKamiyoCardsId()
        {
            return new List<int> {32, 36, 31, 31, 33, 33, 34, 34, 46};
        }

        public static List<int> GetBlackSilenceMaskCardsId()
        {
            return new List<int> {705206, 705207, 705208, 39, 40, 41, 42, 702001, 702004};
        }

        public static List<int> GetHayateCardsId()
        {
            return new List<int> {49, 51, 53, 48, 48, 50, 50, 56, 52, 52};
        }

        public static void AddBuffInfo()
        {
            var dictionary =
                typeof(BattleEffectTextsXmlList).GetField("_dictionary", AccessTools.all)
                    ?.GetValue(Singleton<BattleEffectTextsXmlList>.Instance) as Dictionary<string, BattleEffectText>;
            var files = new DirectoryInfo(ModPack21341Init.Path + "/BattleEffectTexts").GetFiles();
            foreach (var t in files)
                using (var stringReader = new StringReader(File.ReadAllText(t.FullName)))
                {
                    var battleEffectTextRoot =
                        (BattleEffectTextRoot) new XmlSerializer(typeof(BattleEffectTextRoot))
                            .Deserialize(stringReader);
                    foreach (var battleEffectText in battleEffectTextRoot.effectTextList)
                        dictionary?.Add(battleEffectText.ID, battleEffectText);
                }
        }

        public static void SetPassiveCombatLog(PassiveAbilityBase passive, BattleUnitModel owner)
        {
            var battleCardResultLog = owner.battleCardResultLog;
            battleCardResultLog?.SetPassiveAbility(passive);
        }

        public static void DrawUntilX(BattleUnitModel owner, int x)
        {
            var count = owner.allyCardDetail.GetHand().Count;
            var num = x - count;
            if (num > 0) owner.allyCardDetail.DrawCards(num);
        }

        //NotUsed - For Custom Unit on Selection Screen
        private static UnitBattleDataModel AddCustomFixUnitModel(StageModel stage, LibraryFloorModel floor,
            UnitModel unit)
        {
            var lorId = new LorId(ModPack21341Init.PackageId, unit.Id);
            var unitDataModel = new UnitDataModel(lorId, floor.Sephirah, true);
            unitDataModel.SetTemporaryPlayerUnitByBook(lorId);
            unitDataModel.isSephirah = false;
            unitDataModel.SetCustomName(unit.Name);
            unitDataModel.CreateDeckByDeckInfo();
            unitDataModel.forceItemChangeLock = true;
            var unitBattleDataModel = new UnitBattleDataModel(stage, unitDataModel);
            unitBattleDataModel.Init();
            unitDataModel.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, unit.DialogId));
            return unitBattleDataModel;
        }

        public static void AddUnitSephiraOnly(StageLibraryFloorModel instance, StageModel stage, UnitDataModel data)
        {
            var list = (List<UnitBattleDataModel>) instance.GetType().GetField("_unitList", AccessTools.all)
                ?.GetValue(instance);
            var unitBattleDataModel = new UnitBattleDataModel(stage, data);
            if (!unitBattleDataModel.unitData.isSephirah) return;
            unitBattleDataModel.Init();
            list?.Add(unitBattleDataModel);
        }

        //NotUsed - For Custom Unit on Selection Screen
        public static void FillUnitDataSingle(UnitModel unit, StageLibraryFloorModel floor)
        {
            var modelTeam = (List<UnitBattleDataModel>) typeof(StageLibraryFloorModel).GetField("_unitList",
                    AccessTools.all)
                ?.GetValue(Singleton<StageController>.Instance.GetStageModel().GetFloor(floor.Sephirah));
            modelTeam?.Add(AddCustomFixUnitModel(Singleton<StageController>.Instance.GetStageModel(), floor._floorModel,
                unit));
        }

        public static void FillBaseUnit(StageLibraryFloorModel floor)
        {
            var modelTeam = (List<UnitBattleDataModel>) typeof(StageLibraryFloorModel).GetField("_unitList",
                    AccessTools.all)
                ?.GetValue(Singleton<StageController>.Instance.GetStageModel().GetFloor(floor.Sephirah));
            var stage = Singleton<StageController>.Instance.GetStageModel();
            modelTeam?.AddRange(floor._floorModel.GetUnitDataList()
                .Where(x => x.OwnerSephirah == floor.Sephirah && !x.isSephirah)
                .Select(unit => InitUnitDefault(stage, unit)));
        }

        private static UnitBattleDataModel InitUnitDefault(StageModel stage, UnitDataModel data)
        {
            var unitBattleDataModel = new UnitBattleDataModel(stage, data);
            unitBattleDataModel.Init();
            return unitBattleDataModel;
        }

        public static void BattleAbDialog(MonoBehaviour instance, List<AbnormalityCardDialog> dialogs)
        {
            var component = instance.GetComponent<CanvasGroup>();
            var dialog = dialogs[Random.Range(0, dialogs.Count)].dialog;
            var txtAbnormalityDlg = (TextMeshProUGUI) typeof(BattleDialogUI).GetField("_txtAbnormalityDlg",
                AccessTools.all)?.GetValue(instance);
            txtAbnormalityDlg.text = dialog;
            txtAbnormalityDlg.fontMaterial.SetColor("_GlowColor",
                SingletonBehavior<BattleManagerUI>.Instance.negativeCoinColor);
            txtAbnormalityDlg.color = SingletonBehavior<BattleManagerUI>.Instance.negativeTextColor;
            var canvas = (Canvas) typeof(BattleDialogUI).GetField("_canvas",
                AccessTools.all)?.GetValue(instance);
            canvas.enabled = true;
            component.interactable = true;
            component.blocksRaycasts = true;
            txtAbnormalityDlg.GetComponent<AbnormalityDlgEffect>().Init();
            var _ = (Coroutine) typeof(BattleDialogUI).GetField("_routine",
                AccessTools.all)?.GetValue(instance);
            var method = typeof(BattleDialogUI).GetMethod("AbnormalityDlgRoutine", AccessTools.all);
            instance.StartCoroutine(method.Invoke(instance, new object[0]) as IEnumerator);
        }

        public static void ActiveAwakeningDeckPassive(BattleUnitModel owner, string deckName)
        {
            if (!(owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 10)) is
                PassiveAbility_ModPack21341Init8
                passive)) return;
            passive.Init(owner);
            passive.SaveAwakenedDeck(GetDeck(deckName));
            passive.ChangeDeck();
        }

        private static List<int> GetDeck(string deckName)
        {
            switch (deckName)
            {
                case "Kamiyo":
                    return GetKamiyoCardsId();
                case "Mio":
                    return GetMioCardsId();
                case "OldSamurai":
                    return GetSamuraiCardsId();
                case "Hayate":
                    return GetHayateCardsId();
            }

            return null;
        }

        public static List<int> GetSamuraiGhostIndex(int originalUnitIndex)
        {
            switch (originalUnitIndex)
            {
                case 0:
                    return new List<int> {1, 2, 3};
                case 1:
                    return new List<int> {0, 2, 3};
                case 2:
                    return new List<int> {0, 1, 3};
                default:
                    return new List<int> {0, 1, 2};
            }
        }
    }
}