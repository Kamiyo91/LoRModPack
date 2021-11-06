using HarmonyLib;
using ModPack21341.Harmony;
using ModPack21341.Utilities;
using Sound;

namespace ModPack21341.Characters.Hayate.Buffs
{
    //TrueGodAura
    public class BattleUnitBuf_ModPack21341Init10 : BattleUnitBuf
    {
        public BattleUnitBuf_ModPack21341Init10()
        {
            stack = 0;
        }

        public override bool isAssimilation => true;
        public override int paramInBufDesc => 0;
        protected override string keywordId => "TrueGod";

        public override string bufActivatedText =>
            "Power +2, Gain and loss of [Entertain Me] stacks are doubled";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(
                new DiceStatBonus
                {
                    power = 2
                });
        }

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            typeof(BattleUnitBuf).GetField("_bufIcon", AccessTools.all)
                ?.SetValue(this, ModPack21341Init.ArtWorks["ModPack21341Init4"]);
            typeof(BattleUnitBuf).GetField("_iconInit", AccessTools.all)?.SetValue(this, true);
            InitAuraAndPlaySound();
            var buf = owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_ModPack21341Init8) as
                BattleUnitBuf_ModPack21341Init8;
            buf?.SetValue(2);
        }

        private void InitAuraAndPlaySound()
        {
            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Kali_Change");
            UnitUtilities.MakeEffect(_owner, "6/BigBadWolf_Emotion_Aura", 1f, _owner);
        }
    }
}