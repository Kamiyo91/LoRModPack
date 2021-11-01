using ModPack21341.Characters.OldSamurai.Buffs;
using ModPack21341.Harmony;

namespace ModPack21341.Characters.OldSamurai.CardAbilities
{
    //DeepBreathing
    public class DiceCardSelfAbility_ModPack21341Init24 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Play]Boost the *maximum* Dice Roll by 3 until the end of the Scene.After use add this Card back in hand after 4 Scenes";

        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
        }

        public static void Activate(BattleUnitModel unit)
        {
            unit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 4));
            if (unit.faction == Faction.Player && !unit.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_ModPack21341Init20))
                unit.passiveDetail.OnCreated();
        }
    }
}