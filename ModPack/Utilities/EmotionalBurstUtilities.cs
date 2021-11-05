using System.Linq;
using ModPack21341.Characters.CommonPassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.Models;

namespace ModPack21341.Utilities
{
    public static class EmotionalBurstUtilities
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

        public static void RemoveAllEmotionalPassives(BattleUnitModel unit,
            EmotionBufType type = EmotionBufType.Neutral)
        {
            if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init16) is
                PassiveAbility_ModPack21341Init16
                passiveAbilityNeutral)
                unit.passiveDetail.DestroyPassive(passiveAbilityNeutral);

            if (type != EmotionBufType.Happy)
                if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init13) is
                    PassiveAbility_ModPack21341Init13
                    passiveAbilityBaseHappy)
                {
                    passiveAbilityBaseHappy.RemoveBuff();
                    unit.passiveDetail.DestroyPassive(passiveAbilityBaseHappy);
                }

            if (type != EmotionBufType.Angry)
                if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init7) is
                    PassiveAbility_ModPack21341Init7
                    passiveAbilityBaseAngry)
                {
                    passiveAbilityBaseAngry.RemoveBuff();
                    unit.passiveDetail.DestroyPassive(passiveAbilityBaseAngry);
                }

            if (type == EmotionBufType.Sad) return;
            {
                if (!(unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init17) is
                    PassiveAbility_ModPack21341Init17
                    passiveAbilityBaseSad)) return;
                passiveAbilityBaseSad.RemoveBuff();
                unit.passiveDetail.DestroyPassive(passiveAbilityBaseSad);
            }
        }

        public static void DecreaseStacksBufType(BattleUnitModel owner, KeywordBuf bufType, int stacks)
        {
            var buf = owner.bufListDetail.GetActivatedBufList().FirstOrDefault(x => x.bufType == bufType);
            if (buf != null) buf.stack -= stacks;
            if (buf != null && buf.stack < 1) owner.bufListDetail.RemoveBuf(buf);
        }
    }
}