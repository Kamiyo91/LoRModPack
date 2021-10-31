using HarmonyLib;
using LOR_DiceSystem;
using ModPack21341.Harmony;
using ModPack21341.Utilities;
using Sound;

namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_EntertainMeBuf : BattleUnitBuf
    {
        private int _addValue;
        public override BufPositiveType positiveType => BufPositiveType.Positive;
        protected override string keywordId => "EntertainMe";

        public override string bufActivatedText => _owner.faction == Faction.Player
            ? "On 90 or more Stacks,gain Power+1"
            : "Come on!Show me what you got!";

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all)?.SetValue(this, ModPack21341Init.ArtWorks["EM"]);
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

        public void SetValue(int value) => _addValue = value;
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
    public class BattleUnitBuf_TrueGodAura : BattleUnitBuf
    {
        public BattleUnitBuf_TrueGodAura() => stack = 0;
        public override bool isAssimilation => true;
        public override int paramInBufDesc => 0;
        protected override string keywordId => "TrueGod";
        public override string bufActivatedText =>
            "Power +2, Gain and loss of [Entertain Me] stacks are doubled";

        public override void BeforeRollDice(BattleDiceBehavior behavior) => behavior.ApplyDiceStatBonus(
            new DiceStatBonus
            {
                power = 2
            });

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all)?.SetValue(this, ModPack21341Init.ArtWorks["TrueGod"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all)?.SetValue(this, true);
            InitAuraAndPlaySound();
            var buf = owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_EntertainMeBuf) as
                    BattleUnitBuf_EntertainMeBuf;
            buf?.SetValue(2);
        }
        private void InitAuraAndPlaySound()
        {
            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Kali_Change");
            UnitUtilities.MakeEffect(_owner, "6/BigBadWolf_Emotion_Aura", 1f, _owner);
        }
    }
}
