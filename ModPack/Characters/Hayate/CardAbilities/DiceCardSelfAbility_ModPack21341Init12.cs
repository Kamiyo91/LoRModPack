namespace ModPack21341.Characters.Hayate.CardAbilities
{
    //Str1Light2
    public class DiceCardSelfAbility_ModPack21341Init12 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]Restore 2 Light,Gain 1 Strength next Scene";

        public override void OnUseCard()
        {
            owner.cardSlotDetail.RecoverPlayPoint(2);
            owner.bufListDetail.AddKeywordBufByCard(KeywordBuf.Strength, 1, owner);
        }
    }
}