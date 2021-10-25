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
        public static string packageId = "ModPack21341.Mod";
        public static string path;
        public static Dictionary<string, Sprite> ArtWorks = new Dictionary<string, Sprite>();
        public static Dictionary<string, AudioClip> CustomSound;
        public override void OnInitializeMod()
        {
            var harmony = new HarmonyLib.Harmony("LOR.ModPack21341_MOD");
            var method = typeof(ModPack21341Init).GetMethod("BookModel_SetXmlInfo");
            harmony.Patch(typeof(BookModel).GetMethod("SetXmlInfo", AccessTools.all), null, new HarmonyMethod(method)); ;
            path = Path.GetDirectoryName(
                Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
            method = typeof(ModPack21341Init).GetMethod("BookModel_GetThumbSprite");
            harmony.Patch(typeof(BookModel).GetMethod("GetThumbSprite", AccessTools.all), new HarmonyMethod(method));
            MapUtilities.GetArtWorks(new DirectoryInfo(path + "/ArtWork"));
            CustomSound = AudioUtilities.PrepareAudioClips();
            UnitUtilities.AddBuffInfo();
            RemoveError();
        }
        public static void RemoveError()
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
                    if (list.Exists(x => errorLog.Contains(x)))
                    {
                        list2.Add(errorLog);
                    }
                }
            }

            foreach (var item in list2)
            {
                Singleton<ModContentManager>.Instance.GetErrorLogs().Remove(item);
            }
        }
        public static bool BookModel_GetThumbSprite(BookModel __instance, ref Sprite __result)
        {
            if (__instance.BookId == new LorId(packageId, 10000001) || __instance.BookId == new LorId(packageId, 10000002))
            {
                __result = Resources.Load<Sprite>("Sprites/Books/Thumb/243003");
                return false;
            }
            if (__instance.BookId == new LorId(packageId, 10000005))
            {
                __result = Resources.Load<Sprite>("Sprites/Books/Thumb/170313");
                return false;
            }
            if (__instance.BookId == new LorId(packageId, 10000013))
            {
                __result = Resources.Load<Sprite>("Sprites/Books/Thumb/102");
                return false;
            }
            if (__instance.BookId == new LorId(packageId, 10000014))
            {
                __result = ArtWorks["Angela_Default"];
                return false;
            }
            if (__instance.BookId == new LorId(packageId, 10000015))
            {
                __result = Resources.Load<Sprite>("Sprites/Books/Thumb/8");
                return false;
            }
            if (__instance.BookId == new LorId(packageId, 10000016))
            {
                __result = Resources.Load<Sprite>("Sprites/Books/Thumb/250022");
                return false;
            }
            return true;
        }
        public static void BookModel_SetXmlInfo(BookModel __instance, BookXmlInfo ____classInfo,
            ref List<DiceCardXmlInfo> ____onlyCards)
        {
            if (__instance.BookId.packageId == packageId)
            {
                ____onlyCards.AddRange(____classInfo.EquipEffect.OnlyCard.Select(id =>
                    ItemXmlDataList.instance.GetCardItem(new LorId(packageId, id))));
            }
        }
    }
}
