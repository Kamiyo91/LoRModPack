using System.Linq;

namespace ModPack21341.Characters.Kamiyo.CardAbilities
{
    //MioMemory
    public class DiceCardSelfAbility_ModPack21341Init19 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[Single Use]\nCan only be used at [Emotion Level 5] and when there are [no other allies alive]\n[On Use]Summon Mio's Memory next Scene.";

        public override bool OnChooseCard(BattleUnitModel owner)
        {
            return owner.emotionDetail.EmotionLevel >= 5 &&
                   BattleObjectManager.instance.GetAliveList(Faction.Player).All(x => x == owner);
        }
    }
}