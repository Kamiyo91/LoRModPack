using ModPack21341.Characters.CommonBuffs;

namespace ModPack21341.Characters.CommonCardAbilities
{
    //CustomInstantIndexrelease
    public class DiceCardSelfAbility_ModPack21341Init2 : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can only be used at Emotion Level 3 or higher\n[On Play]Release Locked Potential";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 3;
        }

        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
        }

        private static void Activate(BattleUnitModel unit)
        {
            unit.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init3());
        }
    }
}