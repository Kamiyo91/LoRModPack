using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CardAbilities
{
    public class DiceCardSelfAbility_costDown1selfNull : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Combat Start]Remove status aliment [Nullify Power] and become [Immune] to it until the end of the Scene.\n[On Use] Gain 1 [Strength] next Scene";
        public override void OnUseCard() => owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, owner);
        
        public override void OnStartBattle()
        {
            owner.bufListDetail.RemoveBufAll(KeywordBuf.NullifyPower);
            owner.bufListDetail.AddBuf(new BattleUnitBuf_NullImmunity());
        }
    }
    public class DiceCardSelfAbility_SwitchDeck : DiceCardSelfAbilityBase
    {
        public static string Desc = "Usable one time for Scene\n[On Play]Switch Deck with the [Original] or [Awakened] version";
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_CheckDeck) is PassiveAbility_CheckDeck
                passive)
            {
                passive.ChangeDeck();
            }
            self.exhaust = true;
        }
    }
    public class DiceCardSelfAbility_Happy : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Play]Add Emotion [Happy] in Passives([Using it more times will increase its effects]) and remove other Emotion Passives this Scene\n[Happy]:\nGain 1/2/3 [Haste] each Scene.[On Dice Roll]Boost the *maximum* Dice Roll by 1/2/3 or Lower the *maximum* Dice Roll by 1/2/3 at 10%/20%/30% chance.At the end of each Scene change all Emotions Coin Type in [Positive Coin]";
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
            EmotionalBurstUtilities.RemoveEmotionalBurstCards(unit);
        }

        public static void Activate(BattleUnitModel unit)
        {
            EmotionalBurstUtilities.RemoveAllEmotionalPassives(unit, EmotionBufType.Happy);
            if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Happy) is PassiveAbility_Happy passiveHappy)
            {
                var stacks = passiveHappy.GetStack();
                if (stacks < 3)
                    passiveHappy.ChangeNameAndSetStacks(stacks + 1);
                return;
            }
            var passive = unit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 19)) as PassiveAbility_Happy;
            passive?.ChangeNameAndSetStacks(1);
            unit.passiveDetail.OnCreated();
        }
    }
    public class DiceCardSelfAbility_Sad : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Play]Add Emotion [Sad] in Passives([Using it more times will increase its effects]) and remove other Emotion Passives this Scene\n[Sad]:\nGain 1/2/3 [Endurance] and 2/4/6 [Protection], inflict on self 1/2/3 [Bind] each Scene.At the end of each Scene change all Emotions Coin Type in [Negative Coin]";
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
            EmotionalBurstUtilities.RemoveEmotionalBurstCards(unit);
        }

        public static void Activate(BattleUnitModel unit)
        {
            EmotionalBurstUtilities.RemoveAllEmotionalPassives(unit, EmotionBufType.Sad);
            if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Sad) is PassiveAbility_Sad passiveSad)
            {
                var stacks = passiveSad.GetStack();
                if (stacks < 3)
                    passiveSad.ChangeNameAndSetStacks(stacks + 1);
                return;
            }
            var passive = unit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 22)) as PassiveAbility_Sad;
            passive?.ChangeNameAndSetStacks(1);
            unit.passiveDetail.OnCreated();
        }
    }
    public class DiceCardSelfAbility_Angry : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Play]Add Emotion [Angry] in Passives([Using it more times will increase its effects]) and remove other Emotion Passives this Scene\n[Angry]:\nGain 1/2/3 [Strength],inflict on self 1/2/3 [Disarm] and 3/6/9 [Fragile] each Scene.Each time this Character takes damage Gain 1 [Negative Emotion Coin]";
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
            EmotionalBurstUtilities.RemoveEmotionalBurstCards(unit);
        }

        public static void Activate(BattleUnitModel unit)
        {
            EmotionalBurstUtilities.RemoveAllEmotionalPassives(unit, EmotionBufType.Angry);
            if (unit.passiveDetail.PassiveList.Find(x => x is PassiveAbility_Angry) is PassiveAbility_Angry passiveAngry)
            {
                var stacks = passiveAngry.GetStack();
                if (stacks < 3)
                    passiveAngry.ChangeNameAndSetStacks(stacks + 1);
                return;
            }
            var passive = unit.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 21)) as PassiveAbility_Angry;
            passive?.ChangeNameAndSetStacks(1);
            unit.passiveDetail.OnCreated();
        }
    }
    public class DiceCardSelfAbility_Neutral : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Play]Add Emotion [Neutral] in Passives and remove other Emotion Passives this Scene\n[Neutral]:\nDraw one additional page and Restore 1 Light each Scene.";
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
            unit.passiveDetail.OnCreated();
        }
    }
    public class DiceCardSelfAbility_CustomInstantIndexRelease : DiceCardSelfAbilityBase
    {
        public static string Desc = "Can only be used at Emotion Level 3 or higher\n[On Play]Release Locked Potential";
        public override bool OnChooseCard(BattleUnitModel owner) => owner.emotionDetail.EmotionLevel >= 3;
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            Activate(unit);
            self.exhaust = true;
        }

        private static void Activate(BattleUnitModel unit)
        {
            unit.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_CustomInstantIndexRelease());
        }
    }
}
