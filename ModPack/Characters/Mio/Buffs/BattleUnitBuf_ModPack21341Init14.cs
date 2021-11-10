namespace ModPack21341.Characters.Mio.Buffs
{
    //CorruptionResist
    public class BattleUnitBuf_ModPack21341Init14 : BattleUnitBuf
    {
        public BattleUnitBuf_ModPack21341Init14()
        {
            stack = 0;
        }

        public override BufPositiveType positiveType => BufPositiveType.Positive;
        public override int paramInBufDesc => 0;
        protected override string keywordId => "ModPack21341Init9";
        protected override string keywordIconId => "IndexRelease";
    }
}