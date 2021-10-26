using System.Collections.Generic;

namespace ModPack21341.Models
{
    public sealed class UnitModel
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int Pos { get; set; }
        public DeckActionType DeckActionType { get; set; } = DeckActionType.NoAction;
        public List<int> CardsId { get; set; } = new List<int>();
        public bool LockedEmotion { get; set; }
        public int MaxEmotionLevel { get; set; } = 0;
        public bool CreatureMapIsActivated { get; set; }
        public int EmotionLevel { get; set; }
        public int SpeedMin { get; set; } = 3;
        public int SpeedMax { get; set; } = 8;
        public int MaxLight { get; set; } = 4;
        public int CurrentLight { get; set; } = 4;
        public bool CustomDialog { get; set; }
        public int DialogId { get; set; }
        public bool UseDefaultHead { get; set; } = true;
        public bool AddEmotionPassive { get; set; } = true;
        public MapSize ScaleSize { get; set; } = MapSize.L;
    }
    public enum DeckActionType
    {
        NoAction,
        Change
    }
    public enum EmotionBufType
    {
        Happy,
        Sad,
        Angry,
        Neutral
    }
}
