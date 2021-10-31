using ModPack21341.Utilities;
using Sound;

namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_CorruptedGodAuraRelease : BattleUnitBuf
    {
        public BattleUnitBuf_CorruptedGodAuraRelease() => stack = 0;
        public override BufPositiveType positiveType => BufPositiveType.Negative;
        public override bool isAssimilation => true;
        public override int paramInBufDesc => 0;
        protected override string keywordId => "CorruptedMio";
        protected override string keywordIconId => "Final_BigBird_Darkness";
        public override string bufActivatedText =>
            "Power +4 - Lose 80 Hp and 95 Stagger Resist each Scene";

        public override void BeforeRollDice(BattleDiceBehavior behavior) => behavior.ApplyDiceStatBonus(
                new DiceStatBonus
                {
                    power = 4
                });

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
        public override void OnRoundEnd() => TakeDamageByEffect();
        private void TakeDamageByEffect()
        {
            _owner.TakeDamage(80, DamageType.Emotion);
            _owner.breakDetail.TakeBreakDamage(95, DamageType.Emotion);
        }
    }
    public class BattleUnitBuf_CorruptionResist : BattleUnitBuf
    {
        public BattleUnitBuf_CorruptionResist() => stack = 0;
        public override BufPositiveType positiveType => BufPositiveType.Positive;
        public override int paramInBufDesc => 0;
        protected override string keywordId => "Purification";
        protected override string keywordIconId => "IndexRelease";
        public override string bufActivatedText =>
            "Once per scene nullify Damage if the attacker is Corrupted Self";
    }
    public class BattleUnitBuf_GodAuraRelease : BattleUnitBuf
    {
        public BattleUnitBuf_GodAuraRelease() => stack = 0;
        public override BufPositiveType positiveType => BufPositiveType.Positive;
        public override bool isAssimilation => true;
        public override int paramInBufDesc => 0;
        protected override string keywordId => "Mio";
        protected override string keywordIconId => "KeterFinal_Light";

        public override string bufActivatedText =>
            "Power +1 - Recover 3 Hp and Stagger Resist at the end of each scene";

        public override void BeforeRollDice(BattleDiceBehavior behavior) => behavior.ApplyDiceStatBonus(
                new DiceStatBonus
                {
                    power = 1
                });
        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            InitAuraAndPlaySound();
        }
        private void InitAuraAndPlaySound()
        {
            SingletonBehavior<DiceEffectManager>.Instance.CreateNewFXCreatureEffect(
                "5_T/FX_IllusionCard_5_T_Happiness", 1f, _owner.view, _owner.view);
            SoundEffectPlayer.PlaySound("Creature/Greed_MakeDiamond");
        }
        public override void OnRoundEnd() => RecoverHpAndStagger();
        private void RecoverHpAndStagger()
        {
            _owner.RecoverHP(3);
            _owner.breakDetail.RecoverBreak(3);
        }
    }
    public class BattleUnitBuf_OneHitResist : BattleUnitBuf
    {
        private bool _hitTaken;
        public override bool IsInvincibleHp(BattleUnitModel attacker) => true;
        public override bool IsInvincibleBp(BattleUnitModel attacker) => true;
        public override void BeforeTakeDamage(BattleUnitModel attacker, int dmg) => CheckFirstHitByCorruptedSelf(attacker);
        private void CheckFirstHitByCorruptedSelf(BattleUnitModel attacker)
        {
            if (_hitTaken && attacker != null && attacker.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_CorruptedGodAuraRelease))
                Destroy();
            else if (attacker != null)
                _hitTaken = true;
        }
    }
}
