namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_RedMistEgo : BattleUnitBuf
    {
        public override KeywordBuf bufType => KeywordBuf.RedMistEgo;
        protected override string keywordId => "RedMistEgo";
        public override bool isAssimilation => true;
        public BattleUnitBuf_RedMistEgo() => stack = 0;
        
    }
}
