using Sound;

namespace ModPack21341.Characters.Mio.Buffs
{
    //GodAuraRelease
    public class BattleUnitBuf_ModPack21341Init15 : BattleUnitBuf
    {
        public BattleUnitBuf_ModPack21341Init15()
        {
            stack = 0;
        }

        public override BufPositiveType positiveType => BufPositiveType.Positive;
        public override bool isAssimilation => true;
        public override int paramInBufDesc => 0;
        protected override string keywordId => "ModPack21341Init7";
        protected override string keywordIconId => "KeterFinal_Light";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(
                new DiceStatBonus
                {
                    power = 1
                });
        }

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

        public override void OnRoundEnd()
        {
            RecoverHpAndStagger();
        }

        private void RecoverHpAndStagger()
        {
            _owner.RecoverHP(3);
            _owner.breakDetail.RecoverBreak(3);
        }
    }
}