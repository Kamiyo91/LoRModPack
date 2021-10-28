namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_ImmortalForTestingBuf : BattleUnitBuf
    {
        public override bool IsImmortal() => true;
        public override bool IsInvincibleHp(BattleUnitModel attacker) => true;
        public override bool IsInvincibleBp(BattleUnitModel attacker) => true;
        public override void OnRoundStart() => _owner.cardSlotDetail.RecoverPlayPoint(_owner.cardSlotDetail.GetMaxPlayPoint());
    }
    public class BattleUnitBuf_NullImmunity : BattleUnitBuf
    {
        public override bool IsImmune(KeywordBuf buf) => buf == KeywordBuf.NullifyPower;
        public override void OnRoundEnd() => Destroy();
    }
    public class BattleUnitBuf_ImmortalBuffUntiLRoundEnd : BattleUnitBuf
    {
        public override bool IsImmortal() => true;
        public override bool IsInvincibleHp(BattleUnitModel attacker) => true;
        public override void OnRoundEnd() => Destroy();
    }
    public class BattleUnitBuf_ImmortalBuff: BattleUnitBuf
    {
        public override bool IsImmortal() => true;
        public override bool IsInvincibleHp(BattleUnitModel attacker) => true;
        public override bool IsInvincibleBp(BattleUnitModel attacker) => true;
    }
    public class BattleUnitBuf_UntargetableBuf : BattleUnitBuf
    {
        public override bool IsTargetable() => false;
    }
}
