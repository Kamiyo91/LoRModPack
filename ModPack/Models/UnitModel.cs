namespace ModPack21341.Models
{
    public sealed class UnitModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OverrideName { get; set; }
        public int Pos { get; set; }
        public SephirahType Sephirah { get; set; }
        public bool LockedEmotion { get; set; }
        public int MaxEmotionLevel { get; set; } = 0;
        public int EmotionLevel { get; set; }
        public int DialogId { get; set; }
        public bool AddEmotionPassive { get; set; } = true;
        public bool OnWaveStart { get; set; } = false;
    }
}