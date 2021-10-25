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

        public override void OnRoundStart()
        {
            if (_init) return;
            _init = true;
            ChangeToSamuraiEgoMap();
        }
        private static void ChangeToSamuraiEgoMap() => MapUtilities.ChangeMap(new MapModel
        {
            Name = "RedHood",
            Stage = "OldSamurai",
            ArtworkBG = "OldSamurai_Background",
            ArtworkFloor = "OldSamurai_Floor",
            BgFx = 0.5f,
            BgFy = 0.2f,
            FloorFx = 0.5f,
            FloorFy = 0.2f,
            BgmName = "Hornet",
            StageType = Models.StageType.CreatureType,
            IsPlayer = true,
            ExtraSettings = new MapExtraSettings { MapManagerType = typeof(OldSamuraiMapManager) }
        });
    }
}
