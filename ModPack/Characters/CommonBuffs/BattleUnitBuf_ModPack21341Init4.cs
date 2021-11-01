namespace ModPack21341.Characters.CommonBuffs
{
    //ImmortalBuffUntilRoundEnd
    public class BattleUnitBuf_ModPack21341Init4 : BattleUnitBuf
    {
        public override bool IsImmortal()
        {
            return true;
        }

        public override bool IsInvincibleHp(BattleUnitModel attacker)
        {
            return true;
        }

        public override void OnRoundEnd()
        {
            Destroy();
        }
    }
}