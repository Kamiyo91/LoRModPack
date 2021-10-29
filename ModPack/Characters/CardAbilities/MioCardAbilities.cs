using ModPack21341.Models;
using ModPack21341.StageManager.MapManager.MioStageMaps;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CardAbilities
{
    public class DiceCardSelfAbility_GodAura_Card : DiceCardSelfAbilityBase
    {
        public static string Desc = "[Single Use]\nCan only be used at Emotion level 4 or above\n[On Use] Unleash Ego's power, recover full Stagger Resist and full Light next Scene.";

        public override bool OnChooseCard(BattleUnitModel owner) => owner.emotionDetail.EmotionLevel >= 4 &&
                                                                    !owner.bufListDetail.HasAssimilation();
    }
    public class DiceCardSelfAbility_SakuraMirage : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can only be used at Emotion level 4 or above and [Ego's Aura] is required";

        public override bool OnChooseCard(BattleUnitModel owner) => owner.emotionDetail.EmotionLevel >= 4 &&
                                                                    owner.bufListDetail.HasAssimilation();
        public override void OnStartBattle()
        {
            if (owner.faction != Faction.Player || SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject.isEgo) return;
            ChangeToMioEgoMap();
        }

        private static void ChangeToMioEgoMap() => MapUtilities.ChangeMap(new MapModel
        {
            Stage = "Mio",
            StageId = 2,
            OneTurnEgo = true,
            IsPlayer = true,
            Component = new MioMapManager(),
            Bgy = 0.2f
        });
    }

    public class DiceCardSelfAbility_Emotion_Lv5_Fragment : DiceCardSelfAbilityBase
    {
        public static string Desc = "[Single Use]\nCan only be used at [Emotion Level 5] and [Ego's Aura] is required";

        public override bool OnChooseCard(BattleUnitModel owner) => owner.emotionDetail.EmotionLevel >= 5 &&
                                                                    owner.bufListDetail.HasAssimilation();
    }
    public class DiceCardAbility_Death : DiceCardAbilityBase
    {
        public static string Desc = "[On Clash Win] Destroy all dice the opponent has\n[On Hit] Deal 45 Damage to the Target, if the Target goes below 15% Hp, kill it";
        public override void OnSucceedAttack() => DealDamageCheckKill();
        private void DealDamageCheckKill()
        {
            if (card.target?.hp - 45 < card.target?.MaxHp * 0.15f)
            {
                card.target?.Die();
            }
            else
            {
                card.target?.TakeDamage(45, DamageType.Card_Ability);
            }
        }
        public override void OnWinParrying() => card.target?.currentDiceAction?.DestroyDice(DiceMatch.AllDice, DiceUITiming.AttackAfter);
        
    }
}
