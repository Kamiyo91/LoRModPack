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
        public static void ChangeMap(MapModel model)
        {
            Singleton<StageController>.Instance.CheckMapChange();
            if (!Singleton<StageController>.Instance.CanChangeMap()) return;
            if (!model.OneTurnEgo && CanChangeMapCustom(2)) return;
            CustomMapHandler.InitCustomMap(model.Stage, model.Component);
            if (model.IsPlayer && !model.OneTurnEgo)
            {
                PutValueInEgoMap(model.Stage);
            }
            SingletonBehavior<BattleSceneRoot>.Instance.ChangeToSpecialMap(model.Stage, model.ChangingEffect, model.Scale);
        }

        private static void PutValueInEgoMap(string name)
        {
            var type = typeof(StageController);
            var finalType = type.GetField("_addedEgoMap",
                AccessTools.all);
            var mapList = (List<string>)finalType.GetValue(Singleton<StageController>.Instance);
            mapList.Add(name);
            finalType.SetValue(Singleton<StageController>.Instance, mapList);
        }
        public static void ActiveCreatureBattleCamFilterComponent()
        {
            var type = typeof(BattleCamManager);
            var finalType = type.GetField("_effectCam",
                AccessTools.all);
            var battleCamera = (Camera)finalType.GetValue(SingletonBehavior<BattleCamManager>.Instance);
            battleCamera.GetComponent<CameraFilterPack_Drawing_Paper3>().enabled = true;
            finalType.SetValue(SingletonBehavior<BattleCamManager>.Instance, battleCamera);
        }

        private static void RemoveValueInAddedMap(string name, bool removeAll = false)
        {
            var type = typeof(BattleSceneRoot);
            var finalType = type.GetField("_addedMapList",
                AccessTools.all);
            var mapList = (List<MapManager>)finalType.GetValue(SingletonBehavior<BattleSceneRoot>.Instance);
            if (removeAll)
                mapList.Clear();
            else
                mapList.RemoveAll(x => x.name.Contains(name));
            finalType.SetValue(SingletonBehavior<BattleSceneRoot>.Instance, mapList);
        }

        private static void EnemyTeamEmotionCoinValueChange()
        {
            var emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team
                .emotionTotalCoinNumber;
            Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus =
                emotionTotalCoinNumber + 1;
        }

        public static void PrepareChangeBGM(string bgmName,ref Task ChangeBGM)
        {
            ChangeBGM = Task.Run(() =>
            {
                SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.mapBgm =
                    CustomMapHandler.CustomBgmParse(new []
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
        public static void ReturnFromEgoMap(string mapName,BattleUnitModel caller,int originalStageId)
        {
            if (caller.faction == Faction.Enemy || Singleton<StageController>.Instance.GetStageModel().ClassInfo.id ==
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
