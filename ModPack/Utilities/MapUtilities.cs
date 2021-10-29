using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CustomMapUtility;
using HarmonyLib;
using ModPack21341.Harmony;
using ModPack21341.Models;
using UnityEngine;

namespace ModPack21341.Utilities
{
    public static class MapUtilities
    {
        private static bool ChangeMapCheck(MapModel model)
        {
            if (!Singleton<StageController>.Instance.CanChangeMap()) return true;
            if (!model.OneTurnEgo && CanChangeMapCustom(2)) return true;
            if (model.StageId != 0 && CanChangeMapCustom(model.StageId)) return true;
            return false;
        }
        public static void ChangeMap(MapModel model,Faction faction = Faction.Player)
        {
            Singleton<StageController>.Instance.CheckMapChange();
            if (ChangeMapCheck(model)) return;
            CustomMapHandler.InitCustomMap(model.Stage, model.Component, model.IsPlayer, model.InitBgm, model.Bgx, model.Bgy, model.Fx, model.Fy);
            if (model.IsPlayer && !model.OneTurnEgo)
            {
                CustomMapHandler.ChangeToCustomEgoMapByAssimilation(model.Stage,faction);
                return;
            }
            CustomMapHandler.ChangeToCustomEgoMap(model.Stage,faction);
        }
        public static void RemoveValueInEgoMap(string name)
        {
            var mapList = (List<string>)typeof(StageController).GetField("_addedEgoMap",
                AccessTools.all)?.GetValue(Singleton<StageController>.Instance);
            mapList?.RemoveAll(x => x.Contains(name));
        }
        private static void PutValueInEgoMap(string name)
        {
            var mapList = (List<string>)typeof(StageController).GetField("_addedEgoMap",
                AccessTools.all)?.GetValue(Singleton<StageController>.Instance);
            mapList?.Add(name);
        }
        public static void ActiveCreatureBattleCamFilterComponent()
        {
            var battleCamera = (Camera)typeof(BattleCamManager).GetField("_effectCam",
                AccessTools.all)?.GetValue(SingletonBehavior<BattleCamManager>.Instance);
            if (!(battleCamera is null)) battleCamera.GetComponent<CameraFilterPack_Drawing_Paper3>().enabled = true;
        }

        private static void RemoveValueInAddedMap(string name, bool removeAll = false)
        {
            var mapList = (List<MapManager>)typeof(BattleSceneRoot).GetField("_addedMapList",
                AccessTools.all)?.GetValue(SingletonBehavior<BattleSceneRoot>.Instance);
            if (removeAll)
                mapList?.Clear();
            else
                mapList?.RemoveAll(x => x.name.Contains(name));
        }

        private static void EnemyTeamEmotionCoinValueChange()
        {
            var emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team
                .emotionTotalCoinNumber;
            Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus =
                emotionTotalCoinNumber + 1;
        }

        public static void PrepareChangeBGM(string bgmName, ref Task ChangeBGM)
        {
            ChangeBGM = Task.Run(() =>
            {
                SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.mapBgm =
                    CustomMapHandler.CustomBgmParse(new[]
                    {
                        bgmName
                    });
            });
        }

        private static bool CanChangeMapCustom(int id) => Singleton<StageController>.Instance.GetStageModel().ClassInfo.id == new LorId(ModPack21341Init.PackageId, id);

        public static void CheckAndChangeBGM(ref Task ChangeBGM)
        {
            if (ChangeBGM == null) return;
            ChangeBGM.Wait();
            SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(SingletonBehavior<BattleSceneRoot>.Instance
                .currentMapObject.mapBgm);
            ChangeBGM = null;
        }
        public static void ReturnFromEgoMap(string mapName, BattleUnitModel caller, int originalStageId,bool specialCase = false)
        {
            if (caller.faction == Faction.Enemy && specialCase == false || Singleton<StageController>.Instance.GetStageModel().ClassInfo.id ==
                new LorId(ModPack21341Init.PackageId, originalStageId)) return;
            RemoveValueInAddedMap(mapName);
            Singleton<StageController>.Instance.CheckMapChange();
            if (SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.isEgo)
            {
                SingletonBehavior<BattleSceneRoot>.Instance.ChangeToSephirahMap(
                    Singleton<StageController>.Instance.CurrentFloor, true);
                Singleton<StageController>.Instance.CheckMapChange();
            }
            SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(SingletonBehavior<BattleSceneRoot>
                .Instance.currentMapObject.mapBgm);
            SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
        }
        public static void GetArtWorks(DirectoryInfo dir)
        {
            if (dir.GetDirectories().Length != 0)
            {
                var directories = dir.GetDirectories();
                foreach (var t in directories)
                {
                    GetArtWorks(t);
                }
            }

            foreach (var fileInfo in dir.GetFiles())
            {
                var texture2D = new Texture2D(2, 2);
                texture2D.LoadImage(File.ReadAllBytes(fileInfo.FullName));
                var value = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height),
                    new Vector2(0f, 0f));
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                ModPack21341Init.ArtWorks[fileNameWithoutExtension] = value;
            }
        }
    }
}
