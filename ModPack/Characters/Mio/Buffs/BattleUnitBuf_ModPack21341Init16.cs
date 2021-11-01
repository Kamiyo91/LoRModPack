namespace ModPack21341.Characters.Mio.Buffs
{
    //NullImmunity
    public class BattleUnitBuf_ModPack21341Init16 : BattleUnitBuf
    {
        public override bool IsImmune(KeywordBuf buf)
        {
            return buf == KeywordBuf.NullifyPower;
        }

        public override void OnRoundEnd()
        {
            Destroy();
        }
    }
}