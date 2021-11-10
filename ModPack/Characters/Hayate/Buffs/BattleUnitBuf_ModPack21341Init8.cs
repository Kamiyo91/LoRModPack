using HarmonyLib;
using ModPack21341.Harmony;

namespace ModPack21341.Characters.Hayate.Buffs
{
    //EntertainMeBuf
    public class BattleUnitBuf_ModPack21341Init8 : BattleUnitBuf
    {
        private int _addValue;
        public override BufPositiveType positiveType => BufPositiveType.Positive;

        protected override string keywordId =>
            _owner.faction == Faction.Player ? "ModPack21341Init13" : "ModPack21341Init15";

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all)
                ?.SetValue(this, ModPack21341Init.ArtWorks["ModPack21341Init5"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all)?.SetValue(this, true);
            _addValue = 1;
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (stack >= 90)
                behavior.ApplyDiceStatBonus(
                    new DiceStatBonus
                    {
                        power = 1
                    });
        }

        public void SetValue(int value)
        {
            _addValue = value;
        }

        public override void OnSuccessAttack(BattleDiceBehavior behavior)
        {
            if (stack + _addValue > 100)
                stack = 100;
            else
                stack += _addValue;
        }

        public override void BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (attacker == null) return;
            if (stack - _addValue < 0)
                stack = 0;
            else
                stack -= _addValue;
        }
    }
}