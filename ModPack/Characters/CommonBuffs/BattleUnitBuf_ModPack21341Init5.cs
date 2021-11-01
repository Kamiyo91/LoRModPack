namespace ModPack21341.Characters.CommonBuffs
{
    //ImmortalForTestingBuf
    public class BattleUnitBuf_ModPack21341Init5 : BattleUnitBuf
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

        public override void OnRoundStart()
        {
            _owner.cardSlotDetail.RecoverPlayPoint(_owner.cardSlotDetail.GetMaxPlayPoint());
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(
                new DiceStatBonus
                {
                    power = 30
                });
        }
    }
}