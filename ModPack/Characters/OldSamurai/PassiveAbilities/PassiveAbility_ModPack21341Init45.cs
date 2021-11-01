namespace ModPack21341.Characters.OldSamurai.PassiveAbilities
{
    //StendyBreathing
    public class PassiveAbility_ModPack21341Init45 : PassiveAbilityBase
    {
        private int _damage;

        public override float DmgFactor(int dmg, DamageType type = DamageType.ETC, KeywordBuf keyword = KeywordBuf.None)
        {
            if (type == DamageType.Attack)
                _damage += dmg;
            return base.DmgFactor(dmg, type, keyword);
        }

        private void RecoverAndResetCount()
        {
            if (_damage == 0)
            {
                owner.RecoverHP(10);
                owner.breakDetail.RecoverBreak(10);
            }

            _damage = 0;
        }

        public override void OnRoundEnd()
        {
            RecoverAndResetCount();
        }
    }
}