using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomMapUtility;
using HarmonyLib;
using UnityEngine;

namespace ModPack21341.StageManager.MapManager.BlackSilenceEgoMapManager
{
    public class BlackSilenceEgoMapManager : CustomCreatureMapManager
    {
        private GameObject _aura;
        public override void InitializeMap()
        {
            base.InitializeMap();
            sephirahType = SephirahType.None;
            sephirahColor = Color.black;
        }
        public void BoomFirst()
        {
            var gameObject = Instantiate(SingletonBehavior<BlackSilence4thMapManager>.Instance.areaBoomEffect);
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
