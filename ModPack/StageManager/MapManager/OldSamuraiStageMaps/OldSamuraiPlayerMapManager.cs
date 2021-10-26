using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModPack21341.StageManager.MapManager.OldSamuraiStageMaps
{
    public class OldSamuraiPlayerMapManager : OldSamuraiMapManager
    {
        protected internal override string[] CustomBGMs
        {
            get { return new[] { "Hornet.mp3" }; }
        }
    }
}
