using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using HarmonyLib;
using LOR_DiceSystem;
using LOR_XML;
using Mod;
using ModPack21341.Utilities;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace ModPack21341.Harmony
{
    public class ModPack21341Init : ModInitializer
    {
        public const string PackageId = "ModPack21341.Mod";
        private static string _path;
        public static readonly Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();
        private static string _language;

        public override void OnInitializeMod()
        {
            var harmony = new HarmonyLib.Harmony("LOR.ModPack21341_MOD");
            var method = typeof(ModPack21341Init).GetMethod("BookModel_SetXmlInfo");
            harmony.Patch(typeof(BookModel).GetMethod("SetXmlInfo", AccessTools.all), null, new HarmonyMethod(method));
            _path = Path.GetDirectoryName(
                Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            method = typeof(ModPack21341Init).GetMethod("BookModel_GetThumbSprite");
            harmony.Patch(typeof(BookModel).GetMethod("GetThumbSprite", AccessTools.all), null,
                new HarmonyMethod(method));
            method = typeof(ModPack21341Init).GetMethod("StageLibraryFloorModel_InitUnitList");
            harmony.Patch(typeof(StageLibraryFloorModel).GetMethod("InitUnitList", AccessTools.all),
                null, new HarmonyMethod(method));
            method = typeof(ModPack21341Init).GetMethod("UISettingInvenEquipPageListSlot_SetBooksData");
            harmony.Patch(typeof(UISettingInvenEquipPageListSlot).GetMethod("SetBooksData", AccessTools.all),
                null, new HarmonyMethod(method));
            method = typeof(ModPack21341Init).GetMethod("UIInvenEquipPageListSlot_SetBooksData");
            harmony.Patch(typeof(UIInvenEquipPageListSlot).GetMethod("SetBooksData", AccessTools.all),
                null, new HarmonyMethod(method));
            method = typeof(ModPack21341Init).GetMethod("UISpriteDataManager_GetStoryIcon");
            harmony.Patch(typeof(UISpriteDataManager).GetMethod("GetStoryIcon", AccessTools.all),
                null, new HarmonyMethod(method));
            method = typeof(ModPack21341Init).GetMethod("BattleUnitInformationUI_PassiveList_SetData");
            harmony.Patch(typeof(BattleUnitInformationUI_PassiveList).GetMethod("SetData", AccessTools.all),
                new HarmonyMethod(method));
            _language = GlobalGameManager.Instance.CurrentOption.language;
            MapUtilities.GetArtWorks(new DirectoryInfo(_path + "/ArtWork"));
            UnitUtilities.ChangeCardItem(ItemXmlDataList.instance);
            UnitUtilities.ChangeDialogItem(BattleDialogXmlList.Instance);
            AddLocalize();
            RemoveError();
        }

        private static void AddLocalize()
        {
            var dictionary =
                typeof(BattleEffectTextsXmlList).GetField("_dictionary", AccessTools.all)
                    ?.GetValue(Singleton<BattleEffectTextsXmlList>.Instance) as Dictionary<string, BattleEffectText>;
            var files = new DirectoryInfo(_path + "/Localize/" + _language + "/EffectTexts").GetFiles();
            foreach (var t in files)
                using (var stringReader = new StringReader(File.ReadAllText(t.FullName)))
                {
                    var battleEffectTextRoot =
                        (BattleEffectTextRoot) new XmlSerializer(typeof(BattleEffectTextRoot))
                            .Deserialize(stringReader);
                    foreach (var battleEffectText in battleEffectTextRoot.effectTextList)
                        dictionary?.Add(battleEffectText.ID, battleEffectText);
                }

            //if (_language == "en") return;
            //{
            //    files = new DirectoryInfo(_path + "/Localize/" + _language + "/BattlesCards").GetFiles();
            //    foreach (var t in files)
            //        using (var stringReader2 = new StringReader(File.ReadAllText(t.FullName)))
            //        {
            //            var battleCardDescRoot =
            //                (BattleCardDescRoot)new XmlSerializer(typeof(BattleCardDescRoot)).Deserialize(
            //                    stringReader2);
            //            using (var enumerator =
            //                ItemXmlDataList.instance.GetAllWorkshopData()[PackageId].GetEnumerator())
            //            {
            //                while (enumerator.MoveNext())
            //                {
            //                    var card = enumerator.Current;
            //                    card.workshopName = battleCardDescRoot.cardDescList.Find(x => x.cardID == card.id.id)
            //                        .cardName;
            //                }
            //            }

            //            typeof(ItemXmlDataList).GetField("_cardInfoTable", AccessTools.all)
            //                .GetValue(ItemXmlDataList.instance);
            //            using (var enumerator2 = ItemXmlDataList.instance.GetCardList()
            //                .FindAll(x => x.id.packageId == PackageId).GetEnumerator())
            //            {
            //                while (enumerator2.MoveNext())
            //                {
            //                    var card = enumerator2.Current;
            //                    card.workshopName = battleCardDescRoot.cardDescList.Find(x => x.cardID == card.id.id)
            //                        .cardName;
            //                    ItemXmlDataList.instance.GetCardItem(card.id).workshopName = card.workshopName;
            //                }
            //            }
            //        }

            //    files = new DirectoryInfo(_path + "/Localize/" + _language + "/CharactersName").GetFiles();
            //    foreach (var t in files)
            //        using (var stringReader3 = new StringReader(File.ReadAllText(t.FullName)))
            //        {
            //            var charactersNameRoot =
            //                (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
            //                    stringReader3);
            //            using (var enumerator3 =
            //                Singleton<EnemyUnitClassInfoList>.Instance.GetAllWorkshopData()[PackageId].GetEnumerator())
            //            {
            //                while (enumerator3.MoveNext())
            //                {
            //                    var enemy = enumerator3.Current;
            //                    enemy.name = charactersNameRoot.nameList.Find(x => x.ID == enemy.id.id).name;
            //                    Singleton<EnemyUnitClassInfoList>.Instance.GetData(enemy.id).name = enemy.name;
            //                }
            //            }
            //        }

            //    files = new DirectoryInfo(_path + "/Localize/" + _language + "/Books").GetFiles();
            //    foreach (var t in files)
            //        using (var stringReader4 = new StringReader(File.ReadAllText(t.FullName)))
            //        {
            //            var bookDescRoot =
            //                (BookDescRoot)new XmlSerializer(typeof(BookDescRoot)).Deserialize(stringReader4);
            //            using (var enumerator4 = Singleton<BookXmlList>.Instance.GetAllWorkshopData()[PackageId]
            //                .GetEnumerator())
            //            {
            //                while (enumerator4.MoveNext())
            //                {
            //                    var bookXml = enumerator4.Current;
            //                    bookXml.InnerName = bookDescRoot.bookDescList.Find(x => x.bookID == bookXml.id.id)
            //                        .bookName;
            //                }
            //            }

            //            using (var enumerator5 = Singleton<BookXmlList>.Instance.GetList()
            //                .FindAll(x => x.id.packageId == PackageId).GetEnumerator())
            //            {
            //                while (enumerator5.MoveNext())
            //                {
            //                    var bookXml = enumerator5.Current;
            //                    bookXml.InnerName = bookDescRoot.bookDescList.Find(x => x.bookID == bookXml.id.id)
            //                        .bookName;
            //                    Singleton<BookXmlList>.Instance.GetData(bookXml.id).InnerName = bookXml.InnerName;
            //                }
            //            }

            //            (typeof(BookDescXmlList).GetField("_dictionaryWorkshop", AccessTools.all)
            //                    .GetValue(Singleton<BookDescXmlList>.Instance) as Dictionary<string, List<BookDesc>>)
            //                [PackageId] = bookDescRoot.bookDescList;
            //        }

            //    files = new DirectoryInfo(_path + "/Localize/" + _language + "/DropBooks").GetFiles();
            //    foreach (var t in files)
            //        using (var stringReader5 = new StringReader(File.ReadAllText(t.FullName)))
            //        {
            //            var charactersNameRoot2 =
            //                (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
            //                    stringReader5);
            //            using (var enumerator6 = Singleton<DropBookXmlList>.Instance.GetAllWorkshopData()[PackageId]
            //                .GetEnumerator())
            //            {
            //                while (enumerator6.MoveNext())
            //                {
            //                    var dropBook = enumerator6.Current;
            //                    dropBook.workshopName =
            //                        charactersNameRoot2.nameList.Find(x => x.ID == dropBook.id.id).name;
            //                }
            //            }

            //            using (var enumerator7 = Singleton<DropBookXmlList>.Instance.GetList()
            //                .FindAll(x => x.id.packageId == PackageId).GetEnumerator())
            //            {
            //                while (enumerator7.MoveNext())
            //                {
            //                    var dropBook = enumerator7.Current;
            //                    dropBook.workshopName =
            //                        charactersNameRoot2.nameList.Find(x => x.ID == dropBook.id.id).name;
            //                    Singleton<DropBookXmlList>.Instance.GetData(dropBook.id).workshopName =
            //                        dropBook.workshopName;
            //                }
            //            }
            //        }

            //    files = new DirectoryInfo(_path + "/Localize/" + _language + "/StageName").GetFiles();
            //    foreach (var t in files)
            //        using (var stringReader6 = new StringReader(File.ReadAllText(t.FullName)))
            //        {
            //            var charactersNameRoot3 =
            //                (CharactersNameRoot)new XmlSerializer(typeof(CharactersNameRoot)).Deserialize(
            //                    stringReader6);
            //            using (var enumerator8 = Singleton<StageClassInfoList>.Instance.GetAllWorkshopData()[PackageId]
            //                .GetEnumerator())
            //            {
            //                while (enumerator8.MoveNext())
            //                {
            //                    var stage = enumerator8.Current;
            //                    stage.stageName = charactersNameRoot3.nameList.Find(x => x.ID == stage.id.id).name;
            //                }
            //            }
            //        }

            //    files = new DirectoryInfo(_path + "/Localize/" + _language + "/PassiveDesc").GetFiles();
            //    foreach (var t in files)
            //        using (var stringReader7 = new StringReader(File.ReadAllText(t.FullName)))
            //        {
            //            var passiveDescRoot =
            //                (PassiveDescRoot)new XmlSerializer(typeof(PassiveDescRoot)).Deserialize(stringReader7);
            //            using (var enumerator9 = Singleton<PassiveXmlList>.Instance.GetDataAll()
            //                .FindAll(x => x.id.packageId == PackageId).GetEnumerator())
            //            {
            //                while (enumerator9.MoveNext())
            //                {
            //                    var passive = enumerator9.Current;
            //                    passive.name = passiveDescRoot.descList.Find(x => x.ID == passive.id.id).name;
            //                    passive.desc = passiveDescRoot.descList.Find(x => x.ID == passive.id.id).desc;
            //                }
            //            }
            //        }

            //    files = new DirectoryInfo(_path + "/Localize/" + _language + "/BattleCardAbilities").GetFiles();
            //    foreach (var t in files)
            //        using (var stringReader8 = new StringReader(File.ReadAllText(t.FullName)))
            //        {
            //            foreach (var battleCardAbilityDesc in
            //                ((BattleCardAbilityDescRoot)new XmlSerializer(typeof(BattleCardAbilityDescRoot))
            //                    .Deserialize(stringReader8)).cardDescList)
            //                Singleton<BattleCardAbilityDescXmlList>.Instance.GetData(battleCardAbilityDesc.id).desc =
            //                    battleCardAbilityDesc.desc;
            //        }
            //}
        }

        private static void RemoveError()
        {
            var list = new List<string>();
            var list2 = new List<string>();
            list.Add("0Harmony");
            list.Add("NAudio");
            using (var enumerator = Singleton<ModContentManager>.Instance.GetErrorLogs().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var errorLog = enumerator.Current;
                    if (list.Exists(x => errorLog.Contains(x))) list2.Add(errorLog);
                }
            }

            foreach (var item in list2) Singleton<ModContentManager>.Instance.GetErrorLogs().Remove(item);
        }

        public static void BookModel_GetThumbSprite(BookModel __instance, ref Sprite __result)
        {
            if (__instance.BookId.packageId != PackageId) return;
            switch (__instance.BookId.id)
            {
                case 10000001:
                case 10000002:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/243003");
                    return;
                case 10000005:
                case 10000012:
                    __result = ArtWorks["ModPack21341Init8"];
                    return;
                case 10000013:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/102");
                    return;
                case 10000014:
                    __result = ArtWorks["ModPack21341Init6"];
                    return;
                case 10000015:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/8");
                    return;
                case 10000016:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/250022");
                    return;
                case 10000006:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/250035");
                    return;
                case 10000009:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/250024");
                    return;
                case 10000010:
                    __result = ArtWorks["ModPack21341Init7"];
                    return;
                default:
                    return;
            }
        }

        public static void BookModel_SetXmlInfo(BookModel __instance, BookXmlInfo ____classInfo,
            ref List<DiceCardXmlInfo> ____onlyCards)
        {
            if (__instance.BookId.packageId == PackageId)
                ____onlyCards.AddRange(____classInfo.EquipEffect.OnlyCard.Select(id =>
                    ItemXmlDataList.instance.GetCardItem(new LorId(PackageId, id))));
            if (__instance.BookId.id == 250024 && __instance.BookId.IsBasic())
                ____onlyCards.Add(ItemXmlDataList.instance.GetCardItem(new LorId(PackageId, 43)));
        }

        public static void StageLibraryFloorModel_InitUnitList(StageLibraryFloorModel __instance, StageModel stage,
            LibraryFloorModel floor)
        {
            if (stage.ClassInfo.id.packageId != PackageId) return;
            foreach (var unitDataModel in floor.GetUnitDataList())
                switch (stage.ClassInfo.id.id)
                {
                    case 1:
                        UnitUtilities.ClearCharList(__instance);
                        UnitUtilities.AddUnitSephiraOnly(__instance, stage, unitDataModel);
                        return;
                    case 6:
                        UnitUtilities.ClearCharList(__instance);
                        UnitUtilities.AddUnitSephiraOnly(__instance, stage, unitDataModel);
                        return;
                }
        }

        public static void UIInvenEquipPageListSlot_SetBooksData(UISettingInvenEquipPageListSlot __instance,
            List<BookModel> books, UIStoryKeyData storyKey)
        {
            if (storyKey.workshopId != PackageId) return;
            var image = (Image) __instance.GetType().GetField("img_IconGlow", AccessTools.all).GetValue(__instance);
            var image2 = (Image) __instance.GetType().GetField("img_Icon", AccessTools.all).GetValue(__instance);
            var textMeshProUGUI = (TextMeshProUGUI) __instance.GetType().GetField("txt_StoryName", AccessTools.all)
                .GetValue(__instance);
            if (books.Count < 0) return;
            image.enabled = true;
            image2.enabled = true;
            image2.sprite = ArtWorks["ModPack21341Init4"];
            image.sprite = ArtWorks["ModPack21341Init4"];
            textMeshProUGUI.text = "Kamiyo's Mod Pack";
        }

        public static void UISettingInvenEquipPageListSlot_SetBooksData(UISettingInvenEquipPageListSlot __instance,
            List<BookModel> books, UIStoryKeyData storyKey)
        {
            if (storyKey.workshopId != PackageId) return;
            var image = (Image) __instance.GetType().GetField("img_IconGlow", AccessTools.all).GetValue(__instance);
            var image2 = (Image) __instance.GetType().GetField("img_Icon", AccessTools.all).GetValue(__instance);
            var textMeshProUGUI = (TextMeshProUGUI) __instance.GetType().GetField("txt_StoryName", AccessTools.all)
                .GetValue(__instance);
            if (books.Count < 0) return;
            image.enabled = true;
            image2.enabled = true;
            image2.sprite = ArtWorks["ModPack21341Init4"];
            image.sprite = ArtWorks["ModPack21341Init4"];
            textMeshProUGUI.text = "Kamiyo's Mod Pack";
        }

        public static void UISpriteDataManager_GetStoryIcon(UISpriteDataManager __instance,
            ref UIIconManager.IconSet __result, string story)
        {
            if (!ArtWorks.ContainsKey(story)) return;
            __result = new UIIconManager.IconSet
            {
                type = story,
                icon = ArtWorks[story],
                iconGlow = ArtWorks[story]
            };
        }

        public static bool BattleUnitInformationUI_PassiveList_SetData(BattleUnitInformationUI_PassiveList __instance,
            List<PassiveAbilityBase> passivelist)
        {
            var list = (List<BattleUnitInformationUI_PassiveList.BattleUnitInformationPassiveSlot>) __instance.GetType()
                .GetField("passiveSlotList", AccessTools.all)?.GetValue(__instance);
            for (var i = list.Count; i < passivelist.Count; i++)
            {
                var battleUnitInformationPassiveSlot =
                    new BattleUnitInformationUI_PassiveList.BattleUnitInformationPassiveSlot();
                var rectTransform = Object.Instantiate(list[0].Rect, list[0].Rect.parent);
                battleUnitInformationPassiveSlot.Rect = rectTransform;
                for (var j = 0; j < battleUnitInformationPassiveSlot.Rect.childCount; j++)
                    if (battleUnitInformationPassiveSlot.Rect.GetChild(j).gameObject.name.Contains("Glow"))
                        battleUnitInformationPassiveSlot.img_IconGlow = battleUnitInformationPassiveSlot.Rect
                            .GetChild(j).gameObject.GetComponent<Image>();
                    else if (battleUnitInformationPassiveSlot.Rect.GetChild(j).gameObject.name.Contains("Desc"))
                        battleUnitInformationPassiveSlot.txt_PassiveDesc = battleUnitInformationPassiveSlot.Rect
                            .GetChild(j).gameObject.GetComponent<TextMeshProUGUI>();
                    else
                        battleUnitInformationPassiveSlot.img_Icon =
                            rectTransform.GetChild(j).gameObject.GetComponent<Image>();
                rectTransform.gameObject.SetActive(false);
                list.Add(battleUnitInformationPassiveSlot);
            }

            var list2 =
                (List<BattleUnitInformationUI_PassiveList.BattleUnitInformationPassiveSlot>)
                typeof(BattleUnitInformationUI_PassiveList).GetField("passiveSlotList", AccessTools.all)
                    ?.GetValue(__instance);
            var rectTransform2 = (RectTransform) typeof(BattleUnitInformationUI_PassiveList)
                .GetField("rect_passiveListPanel", AccessTools.all)?.GetValue(__instance);
            foreach (var battleUnitInformationPassiveSlot2 in list2.Where(battleUnitInformationPassiveSlot2 =>
                battleUnitInformationPassiveSlot2.Rect.gameObject.activeSelf))
                battleUnitInformationPassiveSlot2.Rect.gameObject.SetActive(false);
            var num = 0;
            var k = 0;
            var num2 = 0f;
            while (k < passivelist.Count)
            {
                if (k > list2.Count) return false;
                if (passivelist[k].isHide)
                {
                    k++;
                }
                else
                {
                    list2[num].SetData(passivelist[k]);
                    list2[num].Rect.gameObject.SetActive(true);
                    num2 += list2[num].Rect.sizeDelta.y * 0.2f + 20f;
                    num++;
                    k++;
                }
            }

            if (k == 0)
            {
                __instance.SetActive(false);
                return false;
            }

            __instance.SetActive(true);
            num2 = num2 < 380f ? 380f : num2 + 100f;
            if (rectTransform2 is null) return false;
            var sizeDelta = rectTransform2.sizeDelta;
            sizeDelta.y = num2;
            rectTransform2.sizeDelta = sizeDelta;
            rectTransform2.anchoredPosition = Vector3.zero;
            return false;
        }
    }
}