using System.Linq;
using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;

namespace ModPack21341.Characters.CardAbilities
{
    public class DiceCardAbility_ZeroBlade : DiceCardAbilityBase
    {
    }
    public class DiceCardSelfAbility_SummonGhosts : DiceCardSelfAbilityBase
    {
        public static string Desc = "[Single Use]\nCan only be used at [Emotion Level 5] and when there are [no other allies alive],[Awakening] is required\n[On Use]Summon 3 Samurai Ghosts next Scene. They are [Uncontrollable] and they can't gain any [Emotion Coin]";
        //public override bool OnChooseCard(BattleUnitModel owner) => owner.emotionDetail.EmotionLevel >= 5 && BattleObjectManager.instance.GetAliveList(Faction.Player).All(x => x == owner) && owner.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_ReviveCheckBuf);
    }
    public class DiceCardSelfAbility_DeepBreathing : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Play]Boost the *maximum* Dice Roll by 3 until the end of the Scene.After use add this Card back in hand after 4 Scenes";
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
        }

        public static void Activate(BattleUnitModel unit)
        {
            unit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 4));
            if (unit.faction == Faction.Player && !unit.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_SummonedUnitOldSamurai))
                unit.passiveDetail.OnCreated();
        }
    }
}
