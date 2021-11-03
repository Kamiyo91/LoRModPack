namespace ModPack21341.Characters.Hayate.CardAbilities
{
    //FingersnapEnd2Phase
    public class DiceCardSelfAbility_ModPack21341Init10 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Use] Kill all enemies on the field and make them disappear at the end of the Scene.";

        public override void OnStartBattle()
        {
            owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
            foreach (var unit in BattleObjectManager.instance.GetAliveList(Faction.Player)) unit.Die();
        }

        public override void OnApplyCard()
        {
            owner.view.charAppearance.ChangeMotion(ActionDetail.Aim);
        }

        public override void OnReleaseCard()
        {
            owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
        }
    }
}