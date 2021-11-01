using System.Linq;
using ModPack21341.Characters.OldSamurai.Buffs;

namespace ModPack21341.Characters.OldSamurai.CardAbilities
{
    //SummonGhosts
    public class DiceCardSelfAbility_ModPack21341Init25 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[Single Use]\nCan only be used at [Emotion Level 5] and when there are [no other allies alive],[Awakening] is required\n[On Use]Summon 3 Samurai Ghosts next Scene. They are [Uncontrollable] and they can't gain any [Emotion Coin]";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 5 &&
                   BattleObjectManager.instance.GetAliveList(Faction.Player).All(x => x == owner) && owner.bufListDetail
                       .GetActivatedBufList().Exists(x => x is BattleUnitBuf_ModPack21341Init19);
        }
    }
}