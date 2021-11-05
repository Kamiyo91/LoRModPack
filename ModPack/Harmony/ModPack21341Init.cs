using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LOR_DiceSystem;
using Mod;
using ModPack21341.Utilities;
using UnityEngine;

namespace ModPack21341.Harmony
{
    public class ModPack21341Init : ModInitializer
    {
        public const string PackageId = "ModPack21341.Mod";
        public static string Path;
        private static bool _keywordCalled;
        public static readonly Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();
        private static Dictionary<string, List<string>> _cardKeywords = new Dictionary<string, List<string>>();
        private static readonly List<int> KamiyoCards = new List<int> {32, 33, 34, 35, 36, 46};
        private static readonly List<int> MioCards = new List<int> {15, 16, 17, 18, 21, 22, 1, 23};
        private static readonly List<int> HayateCards = new List<int> {49, 50, 51, 52, 53, 56, 47};
        private static readonly List<int> SamuraiCards = new List<int> {8, 9, 10, 11, 12};


        public override void OnInitializeMod()
        {
            var harmony = new HarmonyLib.Harmony("LOR.ModPack21341_MOD");
            var method = typeof(ModPack21341Init).GetMethod("BookModel_SetXmlInfo");
            harmony.Patch(typeof(BookModel).GetMethod("SetXmlInfo", AccessTools.all), null, new HarmonyMethod(method));
            Path = System.IO.Path.GetDirectoryName(
                Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            method = typeof(ModPack21341Init).GetMethod("BookModel_GetThumbSprite");
            harmony.Patch(typeof(BookModel).GetMethod("GetThumbSprite", AccessTools.all), new HarmonyMethod(method));
            method = typeof(ModPack21341Init).GetMethod("StageLibraryFloorModel_InitUnitList");
            harmony.Patch(typeof(StageLibraryFloorModel).GetMethod("InitUnitList", AccessTools.all),
                new HarmonyMethod(method));
            method = typeof(ModPack21341Init).GetMethod("KeywordListUI_Init");
            harmony.Patch(typeof(KeywordListUI).GetMethod("Init", AccessTools.all), new HarmonyMethod(method));
            MapUtilities.GetArtWorks(new DirectoryInfo(Path + "/ArtWork"));
            UnitUtilities.AddBuffInfo();
            RemoveError();
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

        public static bool BookModel_GetThumbSprite(BookModel __instance, ref Sprite __result)
        {
            if (__instance.BookId.packageId != PackageId) return true;
            switch (__instance.BookId.id)
            {
                case 10000001:
                case 10000002:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/243003");
                    return false;
                case 10000005:
                    __result = ArtWorks["Knife_Default"];
                    return false;
                case 10000013:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/102");
                    return false;
                case 10000014:
                    __result = ArtWorks["Angela_Default"];
                    return false;
                case 10000015:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/8");
                    return false;
                case 10000016:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/250022");
                    return false;
                case 10000006:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/250035");
                    return false;
                case 10000009:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/250024");
                    return false;
                case 10000010:
                    __result = ArtWorks["Hayate_Default"];
                    return false;
                default:
                    return true;
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

        public static bool StageLibraryFloorModel_InitUnitList(StageLibraryFloorModel __instance, StageModel stage,
            LibraryFloorModel floor)
        {
            if (stage.ClassInfo.id.packageId != PackageId) return true;
            foreach (var unitDataModel in floor.GetUnitDataList())
                switch (stage.ClassInfo.id.id)
                {
                    case 1:
                        UnitUtilities.AddUnitSephiraOnly(__instance, stage, unitDataModel);
                        return false;
                    case 6:
                        UnitUtilities.AddUnitSephiraOnly(__instance, stage, unitDataModel);
                        return false;
                }

            return true;
        }

        public static bool KeywordListUI_Init(KeywordListUI __instance, DiceCardXmlInfo cardInfo,
            IEnumerable<DiceBehaviour> behaviourList)
        {
            if (cardInfo.id.packageId != PackageId) return true;
            var array = (KeywordUI[]) __instance.GetType().GetField("keywordList", AccessTools.all)
                ?.GetValue(__instance);
            if (array == null) return true;
            foreach (var t in array) t.gameObject.SetActive(false);
            var dictionary = new Dictionary<string, int>();
            if (!_keywordCalled)
            {
                _cardKeywords = UnitUtilities.GetCardKeywordListXml();
                _keywordCalled = true;
            }

            var cardKeywords = _cardKeywords;
            if (cardKeywords == null) return true;

            var num = 0;
            if (KamiyoCards.Contains(cardInfo.id.id) && !dictionary.ContainsKey("ModPack21341Init1"))
            {
                dictionary.Add("ModPack21341Init1", 1);
                array[num].Init(_cardKeywords["ModPack21341Init1"][0], _cardKeywords["ModPack21341Init1"][1]);
                num++;
            }

            if (HayateCards.Contains(cardInfo.id.id) && !dictionary.ContainsKey("ModPack21341Init3"))
            {
                dictionary.Add("ModPack21341Init3", 1);
                array[num].Init(_cardKeywords["ModPack21341Init3"][0], _cardKeywords["ModPack21341Init3"][1]);
                num++;
            }

            if (MioCards.Contains(cardInfo.id.id) && !dictionary.ContainsKey("ModPack21341Init2"))
            {
                dictionary.Add("ModPack21341Init2", 1);
                array[num].Init(_cardKeywords["ModPack21341Init2"][0], _cardKeywords["ModPack21341Init2"][1]);
                num++;
            }

            if (SamuraiCards.Contains(cardInfo.id.id) && !dictionary.ContainsKey("ModPack21341Init4"))
            {
                dictionary.Add("ModPack21341Init4", 1);
                array[num].Init(_cardKeywords["ModPack21341Init4"][0], _cardKeywords["ModPack21341Init4"][1]);
                num++;
            }

            if (cardInfo.id.id == 43 && !dictionary.ContainsKey("ModPack21341Init5"))
            {
                dictionary.Add("ModPack21341Init6", 1);
                array[num].Init(_cardKeywords["ModPack21341Init6"][0], _cardKeywords["ModPack21341Init6"][1]);
                num++;
                dictionary.Add("ModPack21341Init5", 1);
                array[num].Init(_cardKeywords["ModPack21341Init5"][0], _cardKeywords["ModPack21341Init5"][1]);
                num++;
            }

            if (cardInfo.id.id == 14 || cardInfo.id.id == 57 && !dictionary.ContainsKey("ModPack21341Init6"))
            {
                dictionary.Add("ModPack21341Init6", 1);
                array[num].Init(_cardKeywords["ModPack21341Init6"][0], _cardKeywords["ModPack21341Init6"][1]);
                num++;
            }

            UnitUtilities.AddOriginalAbilitiesKeywords(__instance, array, dictionary, cardInfo, behaviourList, num);
            return false;
        }
    }
}