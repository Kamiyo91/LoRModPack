namespace ModPack21341.Models
{
    public class MapModel
    {
        public MapManager Component { get; set; }
        public string Stage { get; set; }
        public bool ChangingEffect { get; set; } = true;
        public bool Scale { get; set; } = false;
        public bool IsPlayer { get; set; }
        public bool OneTurnEgo { get; set; }
    }
}
