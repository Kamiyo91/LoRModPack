using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using HarmonyLib;
using LOR_XML;
using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;
using ModPack21341.Models;
using UI;

namespace ModPack21341.Utilities
{
    public static class UnitUtilities
    {
        public static void ChangeDeck(BattleUnitModel owner, List<int> newDeck)
        {
            owner.allyCardDetail.ExhaustAllCards();
            foreach (var cardId in newDeck)
            {
                owner.allyCardDetail.AddNewCardToDeck(new LorId(ModPack21341Init.PackageId, cardId));
            }
        }
        public static void PhaseChangeAllPlayerUnitRecoverBonus(int hp, int stagger, int light, bool fullLightRecover = false)
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
        public static BattleUnitModel AddOriginalPlayerUnitPlayerSide(UnitModel unit)
        {
            var allyUnit = Singleton<StageController>.Instance.CreateLibrarianUnit_fromBattleUnitData(unit.Index);
            allyUnit.OnWaveStart();
            allyUnit.allyCardDetail.DrawCards(allyUnit.UnitData.unitData.GetStartDraw());
            if (unit.CreatureMapIsActivated)
                allyUnit.view.ChangeScale(unit.ScaleSize);
            return allyUnit;
        }
        public static void AddNewUnitEnemySide(UnitModel unit)
        {
            var unitWithIndex = Singleton<StageController>.Instance.AddNewUnit(Faction.Enemy, new LorId(ModPack21341Init.PackageId, unit.Id), unit.Pos);
            if (unit.CreatureMapIsActivated)
                unitWithIndex.view.ChangeScale(unit.ScaleSize);
            unitWithIndex.emotionDetail.SetEmotionLevel(unit.EmotionLevel);
            if (unit.LockedEmotion)
                unitWithIndex.emotionDetail.SetMaxEmotionLevel(unit.MaxEmotionLevel);
            if (unit.DeckActionType != DeckActionType.NoAction)
                ChangeDeck(unitWithIndex, unit.CardsId);
            unitWithIndex.cardSlotDetail.RecoverPlayPoint(unit.CurrentLight);
            unitWithIndex.allyCardDetail.DrawCards(unitWithIndex.UnitData.unitData.GetStartDraw());
            unitWithIndex.formation = new FormationPosition(unitWithIndex.formation._xmlInfo);
            if (!unit.AddEmotionPassive) return;
            foreach (var emotionCard in BattleObjectManager.instance.GetAliveList(Faction.Player).FirstOrDefault()
                .emotionDetail.PassiveList.Where(x =>
                    x.XmlInfo.TargetType == EmotionTargetType.AllIncludingEnemy))
            {
                unitWithIndex.emotionDetail.ApplyEmotionCard(emotionCard.XmlInfo);
            }
        }
        public static BattleUnitModel AddNewUnitPlayerSide(StageLibraryFloorModel floor, UnitModel unit, ref BookModel originalModel, ref BattleDialogueModel dlg)
        {
            var allyUnit = Singleton<StageController>.Instance.CreateLibrarianUnit_fromBattleUnitData(unit.Index);
            allyUnit.passiveDetail.DestroyPassiveAll();
            allyUnit.UnitData.unitData.SetTemporaryPlayerUnitByBook(new LorId(ModPack21341Init.PackageId, unit.Id));
            allyUnit.UnitData.unitData.SetTempName(unit.Name);
            originalModel = allyUnit.customBook;
            allyUnit.index = unit.Pos;
            allyUnit.RecoverHP(allyUnit.MaxHp);
            allyUnit.breakDetail.ResetGauge();
            allyUnit.UnitData.unitData.workshopSkin = "";
            if (unit.UseDefaultHead)
                allyUnit.UnitData.unitData.customizeData.SetCustomData(false);
            allyUnit.UnitData.unitData.EquipCustomCoreBook(new BookModel(Singleton<BookXmlList>.Instance.GetData(new LorId(ModPack21341Init.PackageId, unit.Id))));
            allyUnit.view.CreateSkin();
            allyUnit.emotionDetail.SetEmotionLevel(unit.EmotionLevel);
            if (unit.LockedEmotion)
                allyUnit.emotionDetail.SetMaxEmotionLevel(unit.MaxEmotionLevel);
            else
                allyUnit.emotionDetail.Reset();
            if (unit.DeckActionType != DeckActionType.NoAction)
                ChangeDeck(allyUnit, unit.CardsId);
            allyUnit.allyCardDetail.DrawCards(allyUnit.UnitData.unitData.GetStartDraw());
            allyUnit.Book.SetMaxPlayPoint(unit.MaxLight);
            allyUnit.Book.SetSpeedDiceMax(unit.SpeedMax);
            allyUnit.Book.SetSpeedDiceMin(unit.SpeedMin);
            allyUnit.cardSlotDetail.RecoverPlayPoint(unit.CurrentLight);
            dlg = allyUnit.UnitData.unitData.battleDialogModel;
            allyUnit.formation = floor.GetFormationPosition(unit.Pos);
            if (unit.CreatureMapIsActivated)
                allyUnit.view.ChangeScale(unit.ScaleSize);
            allyUnit.passiveDetail.PassiveList.AddRange(allyUnit.UnitData.unitData.bookItem.CreatePassiveList());
            foreach (var passive in allyUnit.passiveDetail.PassiveList)
                passive.Init(allyUnit);
            if (unit.AddEmotionPassive)
            {
                foreach (var emotionCard in BattleObjectManager.instance.GetAliveList(Faction.Player).FirstOrDefault()
                    .emotionDetail.PassiveList.Where(x =>
                        x.XmlInfo.TargetType == EmotionTargetType.All ||
                        x.XmlInfo.TargetType == EmotionTargetType.AllIncludingEnemy))
                {
                    allyUnit.emotionDetail.ApplyEmotionCard(emotionCard.XmlInfo);
                }
            }
            allyUnit.OnWaveStart();
            if (unit.CustomDialog)
                allyUnit.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, unit.DialogId));
            return allyUnit;
        }
        public static void ReturnToTheOriginalPlayerUnit(BattleUnitModel unit, BookModel originalBook, BattleDialogueModel originalDialog, bool isDead = false)
        {
            unit.UnitData.unitData.customizeData.SetCustomData(true);
            unit.UnitData.unitData.ResetTempName();
            unit.UnitData.unitData.EquipBook(unit.Book);
            if (originalBook != null)
                unit.UnitData.unitData.EquipCustomCoreBook(originalBook);
            unit.UnitData.unitData.battleDialogModel = originalDialog;
            if (isDead)
                unit.Die();
        }

        public static void ReturnToTheOriginalBaseSkin(BattleUnitModel owner, string originalSkinName, BattleDialogueModel dlg)
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

        public static void PrepareSephirahSkin(BattleUnitModel owner, int id, string charName, bool isNpc,ref string originalSkinName, ref BattleDialogueModel dlg, bool baseDlg = false, string charName2 = null, bool doubleName = false)
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
                unit.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ImmortalForTestingBuf());
                unit.emotionDetail.SetEmotionLevel(5);
            }
        }
        public static void ReadyCounterCard(BattleUnitModel owner, int id)
        {
            var card = BattleDiceCardModel.CreatePlayingCard(ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, id)));
            owner.cardSlotDetail.keepCard.AddBehaviours(card, card.CreateDiceCardBehaviorList());
            owner.allyCardDetail.ExhaustCardInHand(card);
        }
        public static void MakeEffect(BattleUnitModel unit, string path, float sizeFactor = 1f,
            BattleUnitModel target = null, float destroyTime = -1f)
        {
            try
            {
                SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect(path, sizeFactor, unit.view, target?.view, destroyTime);
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
        public static List<int> GetSamuraiCardsId() => new List<int> { 7, 7, 3, 3, 4, 4, 5, 5, 6 };

        public static List<int> GetMioCardsId() => new List<int> { 20, 20, 21, 22, 23, 1, 1, 13, 13 };

        public static List<int> GetBlackSilenceMaskCardsId() => new List<int> { 705206, 705207, 705208, 39, 40, 41, 42, 702001, 702004 };
        public static void AddBuffInfo()
        {
            var dictionary = typeof(BattleEffectTextsXmlList).GetField("_dictionary", AccessTools.all)?.GetValue(Singleton<BattleEffectTextsXmlList>.Instance) as Dictionary<string, BattleEffectText>;
            var files = new DirectoryInfo(ModPack21341Init.Path + "/BattleEffectTexts").GetFiles();
            foreach (var t in files)
            {
                using (var stringReader = new StringReader(File.ReadAllText(t.FullName)))
                {
                    var battleEffectTextRoot = (BattleEffectTextRoot)new XmlSerializer(typeof(BattleEffectTextRoot)).Deserialize(stringReader);
                    foreach (var battleEffectText in battleEffectTextRoot.effectTextList)
                    {
                        dictionary?.Add(battleEffectText.ID, battleEffectText);
                    }
                }
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
            if (num > 0)
            {
                owner.allyCardDetail.DrawCards(num);
            }
        }
    }
}
