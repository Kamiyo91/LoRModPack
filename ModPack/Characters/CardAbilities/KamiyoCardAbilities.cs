using System.Linq;

namespace ModPack21341.Characters.CardAbilities
{
    public class DiceCardSelfAbility_AlterEgo_Card : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can be used at Emotion Level 4 or above\n[On Use] Unleash Alter Ego's Power next Scene";

        public override bool OnChooseCard(BattleUnitModel owner) => owner.emotionDetail.EmotionLevel >= 4 && !owner.bufListDetail.HasAssimilation() &&
                                                                    owner.passiveDetail.PassiveList.Exists(x =>
                                                                        x is PassiveAbility_Power_of_the_Unknown);

    }
    public class DiceCardSelfAbility_Emotion_Lv5 : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can only be used at [Emotion Level 5] and [Alter Ego's Aura] is required";

        public override bool OnChooseCard(BattleUnitModel owner) => owner.emotionDetail.EmotionLevel >= 5 &&
                                                                    owner.bufListDetail.HasAssimilation() &&
                                                                    owner.passiveDetail.PassiveList.Exists(x =>
                                                                        x is PassiveAbility_Power_of_the_Unknown);

    }
    public class DiceCardSelfAbility_MioMemory : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[Single Use]\nCan only be used at [Emotion Level 5] and when there are [no other allies alive]\n[On Use]Summon Mio's Memory next Scene.";

        public override bool OnChooseCard(BattleUnitModel owner) => owner.emotionDetail.EmotionLevel >= 5 &&
                                                                     BattleObjectManager.instance
                                                                         .GetAliveList(Faction.Player)
                                                                         .All(x => x == owner);

    }
    public class DiceCardSelfAbility_BurningField3 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]Inflict 3 burn to all enemies";
        public override void OnUseCard()
        {
            foreach (var unit in BattleObjectManager.instance.GetAliveList(owner.faction == Faction.Player ? Faction.Enemy : Faction.Player))
            {
                unit.bufListDetail.AddKeywordBufByCard(KeywordBuf.Burn,3,unit);
            }
        }
    }
    public class DiceCardSelfAbility_BurningField1 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]Inflict 1 burn to all enemies";
        public override void OnUseCard()
        {
            foreach (var unit in BattleObjectManager.instance.GetAliveList(owner.faction == Faction.Player ? Faction.Enemy : Faction.Player))
            {
                unit.bufListDetail.AddKeywordBufByCard(KeywordBuf.Burn, 1, unit);
            }
        }
    }

    public class DiceCardSelfAbility_KurosawaFire : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]Restore 1 Light.If target has 6 or more Burn, gain +1 Power";
        public override void OnUseCard()
        {
            owner.cardSlotDetail.RecoverPlayPoint(1);
            var target = card.target;
            var activatedBuf = target?.bufListDetail.GetActivatedBuf(KeywordBuf.Burn);
            if (activatedBuf == null || activatedBuf.stack < 6) return;
            var currentDiceAction = owner.currentDiceAction;
            currentDiceAction?.ApplyDiceStatBonus(DiceMatch.AllDice, new DiceStatBonus
            {
                power = 1
            });
        }
    }
}
