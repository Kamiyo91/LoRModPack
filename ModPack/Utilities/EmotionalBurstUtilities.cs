using ModPack21341.Characters;
using ModPack21341.Harmony;
using ModPack21341.Models;

namespace ModPack21341.Utilities
{
    public class EmotionalBurstUtilities
    {
        public static void RemoveEmotionalBurstCards(BattleUnitModel unit)
        {
            unit.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 906));
            unit.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 907));
            unit.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 908));
            unit.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 909));
        }
        public static void AddEmotionalBurstCards(BattleUnitModel unit)
        {
            unit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 906));
            unit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 907));
            unit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 908));
            unit.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 909));
        }
        public static void RemoveAllEmotionalPassives(BattleUnitModel unit, EmotionBufType type = EmotionBufType.Neutral)
        {
            if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Neutral) is PassiveAbility_Neutral passiveAbilityNeutral)
            {
                unit.passiveDetail.DestroyPassive(passiveAbilityNeutral); ;
            }

            if (type != EmotionBufType.Happy)
            {
                if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Happy) is PassiveAbility_Happy
                    passiveAbilityBaseHappy)
                {
                    unit.passiveDetail.DestroyPassive(passiveAbilityBaseHappy);
                }
            }

            if (type != EmotionBufType.Angry)
            {
                if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Angry) is PassiveAbility_Angry
                    passiveAbilityBaseAngry)
                {
                    unit.passiveDetail.DestroyPassive(passiveAbilityBaseAngry);
                }
            }

            if (type == EmotionBufType.Sad) return;
            {
                if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Sad) is PassiveAbility_Sad
                    passiveAbilityBaseSad)
                {
                    unit.passiveDetail.DestroyPassive(passiveAbilityBaseSad);
                }
            }
        }
    }
}
