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
        public float Bgx { get; set; } = 0.5f;
        public float Bgy { get; set; } = 0.5f;
        public float Fx { get; set; } = 0.5f;
        public float Fy { get; set; } = 407.5f / 1080f;
        public bool InitBgm { get; set; } = true;
    }
}
