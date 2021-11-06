﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using LOR_DiceSystem;
using Mod;
using ModPack21341.Utilities;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace ModPack21341.Harmony
{
    public class ModPack21341Init : ModInitializer
    {
        public const string PackageId = "ModPack21341.Mod";
        public static string Path;
        public static readonly Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();

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
            method = typeof(ModPack21341Init).GetMethod("UISettingInvenEquipPageListSlot_SetBooksData");
            harmony.Patch(typeof(UISettingInvenEquipPageListSlot).GetMethod("SetBooksData", AccessTools.all),
                new HarmonyMethod(method));
            method = typeof(ModPack21341Init).GetMethod("UIInvenEquipPageListSlot_SetBooksData");
            harmony.Patch(typeof(UIInvenEquipPageListSlot).GetMethod("SetBooksData", AccessTools.all),
                new HarmonyMethod(method));
            method = typeof(ModPack21341Init).GetMethod("UISpriteDataManager_GetStoryIcon");
            harmony.Patch(typeof(UISpriteDataManager).GetMethod("GetStoryIcon", AccessTools.all),
                new HarmonyMethod(method));
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
                case 10000012:
                    __result = ArtWorks["ModPack21341Init8"];
                    return false;
                case 10000013:
                    __result = Resources.Load<Sprite>("Sprites/Books/Thumb/102");
                    return false;
                case 10000014:
                    __result = ArtWorks["ModPack21341Init6"];
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
                    __result = ArtWorks["ModPack21341Init7"];
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

        public static bool UIInvenEquipPageListSlot_SetBooksData(UISettingInvenEquipPageListSlot __instance,
            List<BookModel> books, UIStoryKeyData storyKey)
        {
            if (storyKey.workshopId != PackageId) return true;
            var image = (Image) __instance.GetType().GetField("img_IconGlow", AccessTools.all).GetValue(__instance);
            var image2 = (Image) __instance.GetType().GetField("img_Icon", AccessTools.all).GetValue(__instance);
            var textMeshProUGUI = (TextMeshProUGUI) __instance.GetType().GetField("txt_StoryName", AccessTools.all)
                .GetValue(__instance);
            var listRoot =
                (UIEquipPageScrollList) __instance.GetType().GetField("listRoot", AccessTools.all).GetValue(__instance);
            var list = (List<UIOriginEquipPageSlot>) __instance.GetType().GetField("equipPageSlotList", AccessTools.all)
                .GetValue(__instance);
            if (books.Count >= 0)
            {
                image.enabled = true;
                image2.enabled = true;
                image2.sprite = ArtWorks["ModPack21341Init4"];
                image.sprite = ArtWorks["ModPack21341Init4"];
                textMeshProUGUI.text = "Kamiyo's Mod Pack";
            }

            __instance.SetFrameColor(UIColorManager.Manager.GetUIColor(UIColor.Default));
            var list2 = new List<BookModel>((List<BookModel>) typeof(UIInvenEquipPageListSlot)
                .GetMethod("ApplyFilterBooksInStory", AccessTools.all).Invoke(__instance, new object[]
                {
                    books
                }));
            __instance.SetEquipPagesData(list2);
            var bookModel = list2.Find(x => x == UI.UIController.Instance.CurrentUnit.bookItem);
            if (listRoot.CurrentSelectedBook == null && bookModel != null) listRoot.CurrentSelectedBook = bookModel;
            if (listRoot.CurrentSelectedBook != null)
            {
                var uiOriginEquipPageSlot = list.Find(x => x.BookDataModel == listRoot.CurrentSelectedBook);
                if (uiOriginEquipPageSlot != null) uiOriginEquipPageSlot.SetHighlighted(true, true);
            }

            __instance.SetSlotSize();
            return false;
        }

        public static bool UISettingInvenEquipPageListSlot_SetBooksData(UISettingInvenEquipPageListSlot __instance,
            List<BookModel> books, UIStoryKeyData storyKey)
        {
            if (storyKey.workshopId != PackageId) return true;
            var image = (Image) __instance.GetType().GetField("img_IconGlow", AccessTools.all).GetValue(__instance);
            var image2 = (Image) __instance.GetType().GetField("img_Icon", AccessTools.all).GetValue(__instance);
            var textMeshProUGUI = (TextMeshProUGUI) __instance.GetType().GetField("txt_StoryName", AccessTools.all)
                .GetValue(__instance);
            var listRoot =
                (UIEquipPageScrollList) __instance.GetType().GetField("listRoot", AccessTools.all).GetValue(__instance);
            var list = (List<UIOriginEquipPageSlot>) __instance.GetType().GetField("equipPageSlotList", AccessTools.all)
                .GetValue(__instance);
            if (books.Count >= 0)
            {
                image.enabled = true;
                image2.enabled = true;
                image2.sprite = ArtWorks["ModPack21341Init4"];
                image.sprite = ArtWorks["ModPack21341Init4"];
                textMeshProUGUI.text = "Kamiyo's Mod Pack";
            }

            __instance.SetFrameColor(UIColorManager.Manager.GetUIColor(UIColor.Default));
            var list2 = new List<BookModel>((List<BookModel>) typeof(UIInvenEquipPageListSlot)
                .GetMethod("ApplyFilterBooksInStory", AccessTools.all).Invoke(__instance, new object[]
                {
                    books
                }));
            __instance.SetEquipPagesData(list2);
            var bookModel = list2.Find(x => x == UI.UIController.Instance.CurrentUnit.bookItem);
            if (listRoot.CurrentSelectedBook == null && bookModel != null) listRoot.CurrentSelectedBook = bookModel;
            if (listRoot.CurrentSelectedBook != null)
            {
                var uioriginEquipPageSlot = list.Find(x => x.BookDataModel == listRoot.CurrentSelectedBook);
                if (uioriginEquipPageSlot != null) uioriginEquipPageSlot.SetHighlighted(true, true);
            }

            __instance.SetSlotSize();
            return false;
        }

        public static bool UISpriteDataManager_GetStoryIcon(UISpriteDataManager __instance,
            ref UIIconManager.IconSet __result, string story)
        {
            if (!ArtWorks.ContainsKey(story)) return true;
            __result = new UIIconManager.IconSet
            {
                type = story,
                icon = ArtWorks[story],
                iconGlow = ArtWorks[story]
            };
            return false;
        }
    }
}