using ModPack21341.Characters.OldSamurai.Buffs;

namespace ModPack21341.Characters.OldSamurai.PassiveAbilities
{
    //GhostSamurai
    public class PassiveAbility_ModPack21341Init43 : PassiveAbilityBase
    {
        private void AddGhostUnitBuffs()
        {
            owner.bufListDetail.AddBuf(new BattleUnitBuf_KeterFinal_LibrarianAura());
            if (owner.faction == Faction.Player) owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init20());
        }

        private void CleanGhostUnitBuffs()
        {
            if (owner.bufListDetail.GetActivatedBufList()
                    .Find(x => x is BattleUnitBuf_KeterFinal_LibrarianAura) is BattleUnitBuf_KeterFinal_LibrarianAura
                bufAura)
                bufAura.Destroy();

            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_KeterFinal_LibrarianAura));
            if (owner.faction == Faction.Player)
                owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ModPack21341Init20));
        }

        public override void OnRoundStart()
        {
            AddGhostUnitBuffs();
        }

        public override void OnDie()
        {
            CleanGhostUnitBuffs();
        }
    }
}