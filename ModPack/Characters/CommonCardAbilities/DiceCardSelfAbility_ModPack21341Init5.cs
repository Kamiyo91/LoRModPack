using ModPack21341.Harmony;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CommonCardAbilities
{
    //Neutral
    public class DiceCardSelfAbility_ModPack21341Init5 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Play]Add Emotion [Neutral] in Passives and remove other Emotion Passives this Scene\n[Neutral]:\nDraw one additional page and Restore 1 Light each Scene.";

        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
            EmotionalBurstUtilities.RemoveEmotionalBurstCards(unit);
        }

        public static void Activate(BattleUnitModel unit)
        {
            EmotionalBurstUtilities.RemoveAllEmotionalPassives(unit);
            AddNeutralPassive(unit);
        }

        private static void AddNeutralPassive(BattleUnitModel unit)
        {
            unit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 20));
            if (unit.faction == Faction.Player) unit.passiveDetail.OnCreated();
        }
    }
}