using CustomMapUtility;
using UnityEngine;

namespace ModPack21341.StageManager.MapManager.HayateStageMaps
{
    public class HayateMapManager : CustomCreatureMapManager
    {
        protected internal override string[] CustomBGMs
        {
            get { return new[] { "HayatePhase1.mp3" }; }
        }
        public override void InitializeMap()
        {
            base.InitializeMap();
            sephirahType = SephirahType.None;
            sephirahColor = Color.black;
        }

        public void ChangeBgmName(string name) => CustomBGMs.SetValue(new[] {"test"}, 1);
    }
}
