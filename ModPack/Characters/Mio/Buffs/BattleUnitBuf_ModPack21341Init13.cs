using ModPack21341.Utilities;
using Sound;

namespace ModPack21341.Characters.Mio.Buffs
{
    //CorruptedGodAuraRelease
    public class BattleUnitBuf_ModPack21341Init13 : BattleUnitBuf
    {
        public BattleUnitBuf_ModPack21341Init13()
        {
            stack = 0;
        }

        public override BufPositiveType positiveType => BufPositiveType.Negative;
        public override bool isAssimilation => true;
        public override int paramInBufDesc => 0;
        protected override string keywordId => "ModPack21341Init8";
        protected override string keywordIconId => "Final_BigBird_Darkness";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(
                new DiceStatBonus
                {
                    power = 4
                });
        }

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            InitAuraAndPlaySound();
        }

        private void InitAuraAndPlaySound()
        {
            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Kali_Change");
            SoundEffectPlayer.PlaySound("Creature/Angry_Meet");
            UnitUtilities.MakeEffect(_owner, "6/BigBadWolf_Emotion_Aura", 1f, _owner);
        }

        public override void OnRoundEnd()
        {
            TakeDamageByEffect();
        }

        private void TakeDamageByEffect()
        {
            _owner.TakeDamage(80, DamageType.Emotion);
            _owner.breakDetail.TakeBreakDamage(95, DamageType.Emotion);
        }
    }
}