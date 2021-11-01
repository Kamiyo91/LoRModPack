using ModPack21341.Utilities.CustomMapUtility.Assemblies;
using UnityEngine;

namespace ModPack21341.StageManager.MapManager.BlackSilenceEgoMapManager
{
    public class ModPack21341InitBlackSilenceEgoMapManager : CustomCreatureMapManager
    {
        private GameObject _aura;
        private BlackSilence4thMapManager _mapGameObject;

        public override void InitializeMap()
        {
            base.InitializeMap();
            sephirahType = SephirahType.None;
            sephirahColor = Color.black;
            var map = Util.LoadPrefab("InvitationMaps/InvitationMap_BlackSilence4",
                SingletonBehavior<BattleSceneRoot>.Instance.transform);
            _mapGameObject = map.GetComponent<global::MapManager>() as BlackSilence4thMapManager;
            Destroy(map);
        }

        public void BoomFirst()
        {
            var gameObject = Instantiate(_mapGameObject.areaBoomEffect);
            var battleUnitModel = BattleObjectManager.instance.GetList(Faction.Enemy)[0];
            gameObject.transform.SetParent(battleUnitModel.view.gameObject.transform);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            gameObject.AddComponent<AutoDestruct>().time = 4f;
            gameObject.SetActive(true);
        }

        public void BoomSecond()
        {
            BoomFirst();
            DestroyAura();
        }

        private void DestroyAura()
        {
            if (_aura == null) return;
            Destroy(_aura);
            _aura = null;
        }
    }
}