using ModPack21341.Characters.CommonPassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CommonCardAbilities
{
    //Angry
    public class DiceCardSelfAbility_ModPack21341Init1 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Play]Add Emotion [Angry] in Passives([Using it more times will increase its effects]) and remove other Emotion Passives this Scene\n[Angry]:\nGain 1/2/3 [Strength],inflict on self 1/2/3 [Disarm] and 3/6/9 [Fragile] each Scene.Each time this Character takes damage Gain 1 [Negative Emotion Coin]";

        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
            EmotionalBurstUtilities.RemoveEmotionalBurstCards(unit);
        }

        public static void Activate(BattleUnitModel unit)
        {
            EmotionalBurstUtilities.RemoveAllEmotionalPassives(unit, EmotionBufType.Angry);
            if (unit.passiveDetail.PassiveList.Find(x =>
                x is PassiveAbility_ModPack21341Init7) is PassiveAbility_ModPack21341Init7 passiveAngry)
            {
                var stacks = passiveAngry.GetStack();
                if (stacks < 3)
                    passiveAngry.ChangeNameAndSetStacks(stacks + 1);
                return;
            }

            var passive =
                unit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 21)) as
                    PassiveAbility_ModPack21341Init7;
            passive?.ChangeNameAndSetStacks(1);
            if (unit.faction == Faction.Player) unit.passiveDetail.OnCreated();
        }
    }
}