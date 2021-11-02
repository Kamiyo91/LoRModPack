using ModPack21341.Characters.CommonPassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CommonCardAbilities
{
    //Happy
    public class DiceCardSelfAbility_ModPack21341Init4 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Play]Add Emotion [Happy] in Passives([Using it more times will increase its effects]) and remove other Emotion Passives this Scene\n[Happy]:\nGain 1/2/3 [Haste] each Scene.[On Dice Roll]Boost the *maximum* Dice Roll by 1/2/3 or Lower the *maximum* Dice Roll by 1/2/3 at 10%/20%/30% chance.At the end of each Scene change all Emotions Coin Type in [Positive Coin]";

        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
            EmotionalBurstUtilities.RemoveEmotionalBurstCards(unit);
        }

        public static void Activate(BattleUnitModel unit)
        {
            EmotionalBurstUtilities.RemoveAllEmotionalPassives(unit, EmotionBufType.Happy);
            if (unit.passiveDetail.PassiveList.Find(x =>
                x is PassiveAbility_ModPack21341Init13) is PassiveAbility_ModPack21341Init13 passiveHappy)
            {
                var stacks = passiveHappy.GetStack();
                if (stacks < 3)
                    passiveHappy.ChangeNameAndSetStacks(stacks + 1);
                return;
            }

            var passive =
                unit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 19)) as
                    PassiveAbility_ModPack21341Init13;
            passive?.ChangeNameAndSetStacks(1);
            if (unit.faction == Faction.Player) unit.passiveDetail.OnCreated();
        }
    }
}