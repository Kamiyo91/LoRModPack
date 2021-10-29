using CustomMapUtility;
using UnityEngine;

namespace ModPack21341.StageManager.MapManager.KamiyoStageMaps
{
    public class Kamiyo2PlayerMapManager : CustomCreatureMapManager
    {
        protected internal override string[] CustomBGMs
        {
            get { return new[] { "KamiyoPhase2.mp3" }; }
        }

        public override void InitializeMap()
        {
            base.InitializeMap();
            sephirahType = SephirahType.None;
            sephirahColor = Color.black;
        }
    }
}
