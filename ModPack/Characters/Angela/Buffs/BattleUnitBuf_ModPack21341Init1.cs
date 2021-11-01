namespace ModPack21341.Characters.Angela.Buffs
{
    //BufImmortal
    public class BattleUnitBuf_ModPack21341Init1 : BattleUnitBuf
    {
        public override bool IsImmortal()
        {
            return true;
        }

        public override bool IsInvincibleHp(BattleUnitModel attacker)
        {
            return true;
        }

        public override bool IsInvincibleBp(BattleUnitModel attacker)
        {
            return true;
        }
    }
}