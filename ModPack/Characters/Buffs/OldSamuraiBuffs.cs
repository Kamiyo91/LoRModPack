using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.OldSamuraiStageMaps;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_ReviveCheckBuf : BattleUnitBuf
    {
    }
    public class BattleUnitBuf_SummonedUnitOldSamurai : BattleUnitBuf
    {
        public override bool IsControllable => false;
    }
    public class BattleUnitBuf_OldSamuraiSummonChangeMap : BattleUnitBuf
    {
        private bool _init;
        public override bool isAssimilation => true;
        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            _init = false;
        }
        public override void OnRoundEndTheLast()
        {
            if (_init) return;
            _init = true;
            ChangeToSamuraiEgoMap();
        }
        public override void OnRoundStart()
        {
            SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
        }

        private static void ChangeToSamuraiEgoMap() => MapUtilities.ChangeMap(new MapModel
        {
            Stage = "OldSamurai",
            IsPlayer = true,
            Component = new OldSamuraiPlayerMapManager(),
            Bgy = 0.2f
        });
    }
}
