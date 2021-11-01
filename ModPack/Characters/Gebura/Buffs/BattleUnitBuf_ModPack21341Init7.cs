namespace ModPack21341.Characters.Gebura.Buffs
{
    //RedMistEgo
    public class BattleUnitBuf_ModPack21341Init7 : BattleUnitBuf
    {
        public BattleUnitBuf_ModPack21341Init7()
        {
            stack = 0;
        }

        public override KeywordBuf bufType => KeywordBuf.RedMistEgo;
        protected override string keywordId => "RedMistEgo";
        public override bool isAssimilation => true;
    }
}