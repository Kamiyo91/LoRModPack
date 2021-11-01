namespace ModPack21341.Characters.Mio.Buffs
{
    //OneHitResist
    public class BattleUnitBuf_ModPack21341Init17 : BattleUnitBuf
    {
        private bool _hitTaken;

        public override bool IsInvincibleHp(BattleUnitModel attacker)
        {
            return true;
        }

        public override bool IsInvincibleBp(BattleUnitModel attacker)
        {
            return true;
        }

        public override void BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            CheckFirstHitByCorruptedSelf(attacker);
        }

        private void CheckFirstHitByCorruptedSelf(BattleUnitModel attacker)
        {
            if (_hitTaken && attacker != null && attacker.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_ModPack21341Init13))
                Destroy();
            else if (attacker != null)
                _hitTaken = true;
        }
    }
}