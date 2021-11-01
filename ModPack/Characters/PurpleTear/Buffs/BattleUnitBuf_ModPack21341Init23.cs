using LOR_DiceSystem;
using ModPack21341.Characters.PurpleTear.PassiveAbilities;

namespace ModPack21341.Characters.PurpleTear.Buffs
{
    //CustomPurpleSlash
    public class BattleUnitBuf_ModPack21341Init23 : BattleUnitBuf
    {
        public override KeywordBuf bufType => KeywordBuf.PurpleSlash;

        protected override string keywordId => !PassiveExists() ? "" : "StanceSlash";

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            if (!PassiveExists() || behavior.Detail != BehaviourDetail.Slash)
                return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 2,
                dmgRate = 50
            });
        }

        private bool PassiveExists()
        {
            return _owner.passiveDetail.HasPassive<PassiveAbility_ModPack21341Init47>();
        }
    }
}