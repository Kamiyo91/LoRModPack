using System;

namespace ModPack21341.Models
{
    public class MapModel
    {
        public MapManager Component { get; set; }
        public string Name { get; set; }
        public string Stage { get; set; }
        public string BgmName { get; set; }
        public string ArtworkBG { get; set; }
        public string ArtworkFloor { get; set; }
        public float BgFx { get; set; }
        public float BgFy { get; set; }
        public float FloorFx { get; set; }
        public float FloorFy { get; set; }
        public StageType StageType { get; set; } = StageType.InvitationType;
        public bool ChangingEffect { get; set; } = true;
        public bool Scale { get; set; } = false;
        public bool IsPlayer { get; set; } = false;
        public bool OneTurnEgo { get; set; } = false;
        public MapExtraSettings ExtraSettings { get; set; } = new MapExtraSettings();
    }

    public class MapExtraSettings
    {
        public Type MapManagerType { get; set; }
        public bool IsCreature { get; set; } = false;
        public MapSize MapSize { get; set; } = MapSize.L;
    }
    public enum StageType
    {
        CreatureType,
        InvitationType
    }
}
