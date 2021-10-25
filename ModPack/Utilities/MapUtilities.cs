using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using ModPack21341.Harmony;
using ModPack21341.Models;
using UnityEngine;

namespace ModPack21341.Utilities
{
    public static class MapUtilities
    {
        public static MapManager PrepareMapComponent(MapModel model)
        {
            switch (model.StageType)
            {
                case Models.StageType.InvitationType:
                    var gameObjectInvitation = Util.LoadPrefab("InvitationMaps/InvitationMap_" + model.Name,
                        SingletonBehavior<BattleSceneRoot>.Instance.transform);
                    gameObjectInvitation.name = "InvitationMap_" + model.Stage;
                    model.Component = NewManager(gameObjectInvitation, model.ExtraSettings);
                    InitCustomMap(model);
                    break;
                case Models.StageType.CreatureType:
                    var gameObjectCreature = Util.LoadPrefab("CreatureMaps/CreatureMap_" + model.Name,
                        SingletonBehavior<BattleSceneRoot>.Instance.transform);
                    gameObjectCreature.name = "CreatureMap_" + model.Stage;
                    model.Component = NewManager(gameObjectCreature, model.ExtraSettings);
                    InitCustomMap(model);
                    break;
            }
            return model.Component;
        }
        public static void ChangeMap(MapModel model)
        {
            if (!model.OneTurnEgo)
                EnemyTeamEmotionCoinValueChange();
            Singleton<StageController>.Instance.CheckMapChange();
            var component = PrepareMapComponent(model);
            SingletonBehavior<BattleSceneRoot>.Instance.AddEgoMap(component);
            if (!Singleton<StageController>.Instance.CanChangeMap()) return;
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
        public static void EnemyTeamEmotionCoinValueChange()
        {
            var emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team
                .emotionTotalCoinNumber;
            Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus =
                emotionTotalCoinNumber + 1;
        }

        public static void ReturnFromEgoMap(string mapName,BattleUnitModel caller,int originalStageId)
        {
            if (caller.faction == Faction.Enemy || Singleton<StageController>.Instance.GetStageModel().ClassInfo.id ==
                new LorId(ModPack21341Init.packageId, originalStageId)) return;
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
        private static MapManager NewManager(GameObject mapObject, MapExtraSettings model)
        {
            var original = mapObject.GetComponent<MapManager>();
            if (!(mapObject.AddComponent(model.MapManagerType) is MapManager newManager)) return null;
            newManager.isActivated = original.isActivated;
            newManager.isEnabled = original.isEnabled;
            newManager.mapSize = model.MapSize;
            newManager.isCreature = model.IsCreature;
            newManager.isBossPhase = original.isBossPhase;
            newManager.isEgo = original.isEgo;
            newManager.sephirahType = original.sephirahType;
            newManager.borderFrame = original.borderFrame;
            newManager.backgroundRoot = original.backgroundRoot;
            newManager.sephirahColor = original.sephirahColor;
            newManager.scratchPrefabs = original.scratchPrefabs;
            newManager.wallCratersPrefabs = original.wallCratersPrefabs;
            Object.Destroy(original);
            return newManager;
        }

        private static void InitCustomMap(MapModel model)
        {
            model.Component.mapBgm = new AudioClip[3];
            model.Component.mapBgm[0] = ModPack21341Init.CustomSound[model.BgmName];
            model.Component.mapBgm[1] = ModPack21341Init.CustomSound[model.BgmName];
            model.Component.mapBgm[2] = ModPack21341Init.CustomSound[model.BgmName];
            foreach (var component in model.Component.GetComponentsInChildren<Component>())
            {
                switch (component)
                {
                    case SpriteRenderer renderer when renderer.name == "BG":
                        {
                            var texture = ModPack21341Init.ArtWorks[model.ArtworkBG].texture;
                            renderer.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height),
                                new Vector2(model.BgFx, model.BgFy), renderer.sprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect);
                            break;
                        }
                    case SpriteRenderer renderer when renderer.name.Contains("Floor"):
                        {
                            var texture2 = ModPack21341Init.ArtWorks[model.ArtworkFloor].texture;
                            renderer.sprite = Sprite.Create(texture2, new Rect(0f, 0f, texture2.width, texture2.height),
                                new Vector2(model.FloorFx, model.FloorFy), renderer.sprite.pixelsPerUnit, 0U, SpriteMeshType.FullRect);
                            break;
                        }
                    default:
                        {
                            if (component.name != model.Component.name && !component.name.Contains("Background") &&
                                !component.name.Contains("Frame") && !component.name.Contains("GroundSprites") &&
                                component.name != "Camera" && component.name != "BG" && !component.name.Contains("Floor") &&
                                !component.name.Contains("CaptureTest"))
                            {
                                component.gameObject.SetActive(false);
                            }

                            break;
                        }
                }
            }
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
