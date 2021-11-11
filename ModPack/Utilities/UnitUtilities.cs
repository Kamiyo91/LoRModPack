using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using ModPack21341.Characters.CommonBuffs;
using ModPack21341.Characters.CommonPassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.Models;
using TMPro;
using UI;
using UnityEngine;
using DialogType = LOR_XML.DialogType;
using Random = UnityEngine.Random;

namespace ModPack21341.Utilities
{
    public static class UnitUtilities
    {
        private static readonly List<int> PersonalCardList = new List<int>{900,901,903,904,906,907,
            908, 909, 911, 912, 913, 914, 915, 916, 917, 928, 930, 931, 932 };
        private static readonly List<int> EgoPersonalCardList = new List<int> { 902, 905, 910, 927, 929 };
        private static readonly List<int> OnlyPageCardList = new List<int>
        {
            8,9,10,11,12,23,17,18,22,43,32,34,36,46,
            47,49,50,51,53,56
        };
        private static readonly List<int> AngelaNpcCardList = new List<int> { 918, 919, 920, 921, 922, 923, 924, 925, 926 };
        private static readonly List<int> SamuraiCardList = new List<int> { 8, 9, 10, 11, 12 };
        private static readonly List<int> KamiyoCardList = new List<int> { 32, 34, 36, 46 };
        private static readonly List<int> MioCardList = new List<int> { 23, 17, 18, 22 };
        private static readonly List<int> HayateCardList = new List<int> { 47, 49, 50, 51, 53, 56 };
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

        public static void AddOriginalPlayerUnitPlayerSide(int index, int emotionLevel)
        {
            var allyUnit = Singleton<StageController>.Instance.CreateLibrarianUnit_fromBattleUnitData(index);
            allyUnit.OnWaveStart();
            allyUnit.allyCardDetail.DrawCards(allyUnit.UnitData.unitData.GetStartDraw());
            allyUnit.emotionDetail.SetEmotionLevel(emotionLevel);
            allyUnit.emotionDetail.Reset();
            allyUnit.cardSlotDetail.RecoverPlayPoint(allyUnit.cardSlotDetail.GetMaxPlayPoint());
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

        public static void ApplyEmotionCards(BattleUnitModel unit, IEnumerable<BattleEmotionCardModel> emotionCardList)
        {
            foreach (var card in emotionCardList) unit.emotionDetail.ApplyEmotionCard(card.XmlInfo);
        }

        public static IEnumerable<BattleEmotionCardModel> GetEmotionCardByUnit(BattleUnitModel unit)
        {
            return unit.emotionDetail.PassiveList.ToList();
        }

        public static IEnumerable<BattleEmotionCardModel> SaveEmotionCards(List<BattleEmotionCardModel> emotionCardList)
        {
            var playerUnitsAlive = BattleObjectManager.instance.GetList(Faction.Player);
            foreach (var emotionCard in playerUnitsAlive.SelectMany(x => x.emotionDetail.PassiveList)
                .Where(emotionCard => !emotionCardList.Exists(x => x.XmlInfo.Equals(emotionCard.XmlInfo))))
                emotionCardList.Add(emotionCard);
            return emotionCardList;
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
            return new List<int> { 7, 7, 3, 3, 4, 4, 5, 5, 6 };
        }

        public static List<int> GetMioCardsId()
        {
            return new List<int> { 20, 20, 21, 22, 23, 1, 1, 13, 13 };
        }

        private static List<int> GetKamiyoCardsId()
        {
            return new List<int> { 32, 36, 31, 31, 33, 33, 34, 34, 46 };
        }

        public static List<int> GetBlackSilenceMaskCardsId()
        {
            return new List<int> { 705206, 705207, 705208, 39, 40, 41, 42, 702001, 702004 };
        }

        private static List<int> GetHayateCardsId()
        {
            return new List<int> { 49, 51, 53, 48, 50, 50, 56, 52, 52, 52 };
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

        public static void AddUnitSephiraOnly(StageLibraryFloorModel instance, StageModel stage, UnitDataModel data)
        {
            var list = (List<UnitBattleDataModel>)instance.GetType().GetField("_unitList", AccessTools.all)
                ?.GetValue(instance);
            var unitBattleDataModel = new UnitBattleDataModel(stage, data);
            if (!unitBattleDataModel.unitData.isSephirah) return;
            unitBattleDataModel.Init();
            list?.Add(unitBattleDataModel);
        }

        public static void ClearCharList(StageLibraryFloorModel instance)
        {
            var list = (List<UnitBattleDataModel>)instance.GetType().GetField("_unitList", AccessTools.all)
                ?.GetValue(instance);
            list?.Clear();
        }

        public static void FillBaseUnit(StageLibraryFloorModel floor)
        {
            var modelTeam = (List<UnitBattleDataModel>)typeof(StageLibraryFloorModel).GetField("_unitList",
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
            var txtAbnormalityDlg = (TextMeshProUGUI)typeof(BattleDialogUI).GetField("_txtAbnormalityDlg",
                AccessTools.all)?.GetValue(instance);
            txtAbnormalityDlg.text = dialog;
            txtAbnormalityDlg.fontMaterial.SetColor("_GlowColor",
                SingletonBehavior<BattleManagerUI>.Instance.negativeCoinColor);
            txtAbnormalityDlg.color = SingletonBehavior<BattleManagerUI>.Instance.negativeTextColor;
            var canvas = (Canvas)typeof(BattleDialogUI).GetField("_canvas",
                AccessTools.all)?.GetValue(instance);
            canvas.enabled = true;
            component.interactable = true;
            component.blocksRaycasts = true;
            txtAbnormalityDlg.GetComponent<AbnormalityDlgEffect>().Init();
            var _ = (Coroutine)typeof(BattleDialogUI).GetField("_routine",
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
                    return new List<int> { 1, 2, 3 };
                case 1:
                    return new List<int> { 0, 2, 3 };
                case 2:
                    return new List<int> { 0, 1, 3 };
                default:
                    return new List<int> { 0, 1, 2 };
            }
        }

        private static void SetBaseKeywordCard(LorId id, ref Dictionary<LorId, DiceCardXmlInfo> cardDictionary,
            ref List<DiceCardXmlInfo> cardXmlList)
        {
            var keywordsList = GetKeywordsList(id.id).ToList();
            var diceCardXmlInfo2 = CardOptionChange(cardDictionary[id], new List<CardOption>(), true, keywordsList);
            cardDictionary[id] = diceCardXmlInfo2;
            cardXmlList.Add(diceCardXmlInfo2);
        }
        private static void SetCustomCardOption(CardOption option, LorId id, bool keywordsRequired, ref Dictionary<LorId, DiceCardXmlInfo> cardDictionary, ref List<DiceCardXmlInfo> cardXmlList)
        {
            var keywordsList = new List<string>();
            if (keywordsRequired) keywordsList = GetKeywordsList(id.id).ToList();
            var diceCardXmlInfo2 = CardOptionChange(cardDictionary[id], new List<CardOption> { option }, keywordsRequired, keywordsList);
            cardDictionary[id] = diceCardXmlInfo2;
            cardXmlList.Add(diceCardXmlInfo2);
        }
        private static void SetRangeSpecial(LorId id, ref Dictionary<LorId, DiceCardXmlInfo> cardDictionary, ref List<DiceCardXmlInfo> cardXmlList)
        {
            var diceCardXmlInfo2 = CardOptionChange(cardDictionary[id], new List<CardOption>(), false, null, "", "", 0, true);
            cardDictionary[id] = diceCardXmlInfo2;
            cardXmlList.Add(diceCardXmlInfo2);
        }
        private static void SetAngelaCard(LorId id, ref Dictionary<LorId, DiceCardXmlInfo> cardDictionary, ref List<DiceCardXmlInfo> cardXmlList)
        {
            var skinName = "";
            var mapName = "";
            var skinHeight = 0;
            SetAngelaCardInfo(id.id, ref skinName, ref mapName, ref skinHeight);
            var diceCardXmlInfo2 = CardOptionChange(cardDictionary[id], new List<CardOption>(), false, null, skinName, mapName, skinHeight);
            cardDictionary[id] = diceCardXmlInfo2;
            cardXmlList.Add(diceCardXmlInfo2);
        }

        private static IEnumerable<string> GetKeywordsList(int id)
        {
            if (KamiyoCardList.Contains(id))
                return new List<string> { "ModPack21341Init6", "ModPack21341Init1" };
            if (MioCardList.Contains(id))
                return new List<string> { "ModPack21341Init6", "ModPack21341Init2" };
            if (HayateCardList.Contains(id))
                return new List<string> { "ModPack21341Init6", "ModPack21341Init3" };
            return SamuraiCardList.Contains(id) ? new List<string> { "ModPack21341Init6", "ModPack21341Init4" } : new List<string> { "ModPack21341Init6" };
        }
        private static void SetAngelaCardInfo(int id, ref string skinName, ref string mapName, ref int skinHeight)
        {
            switch (id)
            {
                case 918:
                    skinName = "EGO_ScorchedGirl";
                    mapName = "ScorchedGirl";
                    break;
                case 919:
                    skinName = "EGO_Murderer";
                    mapName = "Murderer";
                    break;
                case 920:
                    skinName = "EGO_SpiderBud";
                    mapName = "SpiderBud";
                    break;
                case 921:
                    skinName = "EGO_Orchestra";
                    break;
                case 922:
                    skinName = "EGO_Greed_Reason";
                    mapName = "KingOfGreed";
                    break;
                case 923:
                    skinName = "EGO_Nosferatu";
                    mapName = "Nosferatu";
                    break;
                case 924:
                    skinName = "EGO_Ozma";
                    mapName = "Ozma";
                    break;
                case 925:
                    skinName = "EGO_ApocalypseBird";
                    mapName = "ApocalypseBird";
                    skinHeight = 500;
                    break;
                case 926:
                    skinName = "EGO_OneBadManyGood";
                    mapName = "WhiteNight";
                    break;
            }
        }
        private static DiceCardXmlInfo CardOptionChange(DiceCardXmlInfo cardXml, List<CardOption> option, bool keywordRequired, List<string> keywords,
            string skinName = "", string mapName = "", int skinHeight = 0, bool changeRange = false)
        {
            var spec = new DiceCardSpec
            {
                affection = cardXml.Spec.affection,
                Cost = cardXml.Spec.Cost,
                emotionLimit = cardXml.Spec.emotionLimit,
                Ranged = CardRange.Special
            };
            return new DiceCardXmlInfo(cardXml.id)
            {
                workshopID = cardXml.workshopID,
                workshopName = cardXml.workshopName,
                Artwork = cardXml.Artwork,
                Chapter = cardXml.Chapter,
                category = cardXml.category,
                DiceBehaviourList = cardXml.DiceBehaviourList,
                _textId = cardXml._textId,
                optionList = option.Any() ? option : cardXml.optionList,
                Priority = cardXml.Priority,
                Rarity = cardXml.Rarity,
                Script = cardXml.Script,
                ScriptDesc = cardXml.ScriptDesc,
                Spec = changeRange ? spec : cardXml.Spec,
                SpecialEffect = cardXml.SpecialEffect,
                SkinChange = string.IsNullOrEmpty(skinName) ? cardXml.SkinChange : skinName,
                SkinChangeType = cardXml.SkinChangeType,
                SkinHeight = skinHeight != 0 ? skinHeight : cardXml.SkinHeight,
                MapChange = string.IsNullOrEmpty(mapName) ? cardXml.MapChange : mapName,
                PriorityScript = cardXml.PriorityScript,
                Keywords = keywordRequired ? keywords : cardXml.Keywords
            };
        }
        public static void ChangeCardItem(ItemXmlDataList instance)
        {
            var dictionary = (Dictionary<LorId, DiceCardXmlInfo>)instance.GetType()
                .GetField("_cardInfoTable", AccessTools.all).GetValue(instance);
            var list = (List<DiceCardXmlInfo>)instance.GetType()
                .GetField("_cardInfoList", AccessTools.all).GetValue(instance);
            foreach (var (key, _) in dictionary.Where(x => x.Key.packageId == ModPack21341Init.PackageId).ToList())
            {
                if (PersonalCardList.Contains(key.id))
                {
                    SetCustomCardOption(CardOption.Personal, key, false, ref dictionary, ref list);
                    continue;
                }

                if (EgoPersonalCardList.Contains(key.id))
                {
                    SetCustomCardOption(CardOption.EgoPersonal, key, false, ref dictionary, ref list);
                    continue;
                }

                if (OnlyPageCardList.Contains(key.id))
                {
                    SetCustomCardOption(CardOption.OnlyPage, key, true, ref dictionary, ref list);
                    continue;
                }

                if (AngelaNpcCardList.Contains(key.id))
                {
                    SetAngelaCard(key, ref dictionary, ref list);
                    continue;
                }

                if (key.id == 920)
                {
                    SetRangeSpecial(key, ref dictionary, ref list);
                    continue;
                }
                SetBaseKeywordCard(key, ref dictionary, ref list);
            }
        }

        public static void ChangeDialogItem(BattleDialogXmlList instance)
        {
            var dictionary = (Dictionary<string, BattleDialogRoot>)instance.GetType()
                .GetField("_dictionary", AccessTools.all).GetValue(instance);
            foreach (var item in dictionary.SelectMany(x => x.Value.characterList).Where(y => y.id.packageId == ModPack21341Init.PackageId))
            {
                if (item.id.id == 200)
                {
                    var dlg = new BattleDialog
                    {
                        dialogID = "0",
                        dialogContent = "It doesn't look good.I'll do my best!"
                    };
                    var dlgList = new List<BattleDialog> {dlg};
                    item.dialogTypeList.Add(new BattleDialogType
                        {dialogType = DialogType.SPECIAL_EVENT, dialogList = dlgList});
                }
                if (item.id.id == 3)
                {
                    var dlg1 = new BattleDialog
                    {
                        dialogID = "0",
                        dialogContent = "Everyone who stand in my way must vanish!!!"
                    };
                    var dlg2 = new BattleDialog
                    {
                        dialogID = "1",
                        dialogContent = "I..I...Ahhhhh!"
                    };
                    var dlgList = new List<BattleDialog> { dlg1,dlg2 };
                    item.dialogTypeList.Add(new BattleDialogType
                        { dialogType = DialogType.SPECIAL_EVENT, dialogList = dlgList });
                }
                if (item.id.id == 201)
                {
                    var dlg1 = new BattleDialog
                    {
                        dialogID = "0",
                        dialogContent = "The situation seems really bad."
                    };
                    var dlg2 = new BattleDialog
                    {
                        dialogID = "1",
                        dialogContent = "I must stop her now"
                    };
                    var dlgList = new List<BattleDialog> { dlg1, dlg2 };
                    item.dialogTypeList.Add(new BattleDialogType
                        { dialogType = DialogType.SPECIAL_EVENT, dialogList = dlgList });
                }
                if (item.id.id == 202)
                {
                    var dlg1 = new BattleDialog
                    {
                        dialogID = "0",
                        dialogContent = "It's not over yet!"
                    };
                    var dlg2 = new BattleDialog
                    {
                        dialogID = "1",
                        dialogContent = "Not bad.Let's see how you'll handle this now!"
                    };
                    var dlgList = new List<BattleDialog> { dlg1, dlg2 };
                    item.dialogTypeList.Add(new BattleDialogType
                        { dialogType = DialogType.SPECIAL_EVENT, dialogList = dlgList });
                }
                if (item.id.id == 14)
                {
                    var dlg1 = new BattleDialog
                    {
                        dialogID = "0",
                        dialogContent = "I'll not fall!Not now!"
                    };
                    var dlg2 = new BattleDialog
                    {
                        dialogID = "1",
                        dialogContent = "Think you can keep up with me?..."
                    };
                    var dlgList = new List<BattleDialog> { dlg1, dlg2 };
                    item.dialogTypeList.Add(new BattleDialogType
                        { dialogType = DialogType.SPECIAL_EVENT, dialogList = dlgList });
                }
                if (item.id.id == 8 || item.id.id == 9)
                {
                    var dlg1 = new BattleDialog
                    {
                        dialogID = "0",
                        dialogContent = "Playtime is over."
                    };
                    var dlg2 = new BattleDialog
                    {
                        dialogID = "1",
                        dialogContent = "Time to end this."
                    };
                    var dlgList = new List<BattleDialog> { dlg1, dlg2 };
                    item.dialogTypeList.Add(new BattleDialogType
                        { dialogType = DialogType.SPECIAL_EVENT, dialogList = dlgList });
                }
            }
        }
    }
}