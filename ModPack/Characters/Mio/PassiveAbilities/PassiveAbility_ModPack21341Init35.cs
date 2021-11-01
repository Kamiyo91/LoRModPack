using ModPack21341.Characters.Mio.Buffs;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Mio.PassiveAbilities
{
    //CorruptionResist
    public class PassiveAbility_ModPack21341Init35 : PassiveAbilityBase
    {
        private bool _dmgResist;

        public override void Init(BattleUnitModel self)
        {
            base.Init(self);
            Hide();
        }

        public override void OnRoundStart()
        {
            _dmgResist = true;
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init17());
        }

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (!_dmgResist || attacker == null || !attacker.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_ModPack21341Init13)) return base.BeforeTakeDamage(attacker, dmg);
            UnitUtilities.SetPassiveCombatLog(this, owner);
            _dmgResist = false;
            return base.BeforeTakeDamage(attacker, dmg);
        }

        public override void OnRoundEnd()
        {
            RemoveResistBuf();
        }

        private void RemoveResistBuf()
        {
            if (owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_ModPack21341Init17) is
                BattleUnitBuf_ModPack21341Init17 buff)
                owner.bufListDetail.RemoveBuf(buff);
        }
    }
}