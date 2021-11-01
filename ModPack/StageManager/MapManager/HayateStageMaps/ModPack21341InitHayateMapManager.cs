using ModPack21341.Utilities.CustomMapUtility.Assemblies;
using UnityEngine;

namespace ModPack21341.StageManager.MapManager.HayateStageMaps
{
    public class ModPack21341InitHayateMapManager : CustomCreatureMapManager
    {
        protected internal override string[] CustomBgMs
        {
            get { return new[] {"HayatePhase1.mp3"}; }
        }

        public override void InitializeMap()
        {
            base.InitializeMap();
            sephirahType = SephirahType.None;
            sephirahColor = Color.black;
        }
    }
}