using ModPack21341.Characters.CommonPassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CommonCardAbilities
{
    //Sad
    public class DiceCardSelfAbility_ModPack21341Init6 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Play]Add Emotion [Sad] in Passives([Using it more times will increase its effects]) and remove other Emotion Passives this Scene\n[Sad]:\nGain 1/2/3 [Endurance] and 2/4/6 [Protection], inflict on self 1/2/3 [Bind] each Scene.At the end of each Scene change all Emotions Coin Type in [Negative Coin]";

        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
            EmotionalBurstUtilities.RemoveEmotionalBurstCards(unit);
        }

        public static void Activate(BattleUnitModel unit)
        {
            EmotionalBurstUtilities.RemoveAllEmotionalPassives(unit, EmotionBufType.Sad);
            if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init17) is
                PassiveAbility_ModPack21341Init17 passiveSad)
            {
                var stacks = passiveSad.GetStack();
                if (stacks >= 3) return;
                passiveSad.ChangeNameAndSetStacks(stacks + 1);
                passiveSad.InstantIncrease();
                return;
            }

            var passive =
                unit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 22)) as
                    PassiveAbility_ModPack21341Init17;
            passive?.ChangeNameAndSetStacks(1);
            passive?.AfterInit();
            if (unit.faction == Faction.Player) unit.passiveDetail.OnCreated();
        }
    }
}