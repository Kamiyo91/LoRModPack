namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //Loneliness
    public class PassiveAbility_ModPack21341Init15 : PassiveAbilityBase
    {
        public override void OnRoundEnd()
        {
            if (BattleObjectManager.instance.GetAliveList(owner.faction).Count == 1)
                owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Endurance, 3);
        }
    }
}