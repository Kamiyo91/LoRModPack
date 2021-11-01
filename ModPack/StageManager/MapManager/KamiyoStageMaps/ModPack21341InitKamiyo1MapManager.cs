using ModPack21341.Utilities.CustomMapUtility.Assemblies;
using UnityEngine;

namespace ModPack21341.StageManager.MapManager.KamiyoStageMaps
{
    public class ModPack21341InitKamiyo1MapManager : CustomCreatureMapManager
    {
        protected internal override string[] CustomBgMs
        {
            get { return new[] {"KamiyoPhase1.mp3"}; }
        }

        public override void InitializeMap()
        {
            base.InitializeMap();
            sephirahType = SephirahType.None;
            sephirahColor = Color.black;
        }
    }
}