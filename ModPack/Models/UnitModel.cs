using System.Collections.Generic;

namespace ModPack21341.Models
{
    public class UnitModel
    {
        public virtual int Index { get; set; }
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Pos { get; set; }
        public virtual DeckActionType DeckActionType { get; set; } = DeckActionType.NoAction;
        public virtual List<int> CardsId { get; set; } = new List<int>();
        public virtual bool LockedEmotion { get; set; } = false;
        public virtual int MaxEmotionLevel { get; set; } = 0;
        public virtual bool CreatureMapIsActivated { get; set; } = false;
        public virtual int EmotionLevel { get; set; } = 0;
        public virtual int SpeedMin { get; set; } = 3;
        public virtual int SpeedMax { get; set; } = 8;
        public virtual int MaxLight { get; set; } = 4;
        public virtual int CurrentLight { get; set; } = 4;
        public virtual bool CustomDialog { get; set; } = false;
        public virtual int DialogId { get; set; } = 0;
        public virtual bool UseDefaultHead { get; set; } = true;
        public virtual bool AddEmotionPassive { get; set; } = true;
        public virtual MapSize ScaleSize { get; set; } = MapSize.L;
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
