namespace ModPack21341.Characters.Hayate.CardAbilities
{
    //Fingersnap
    public class DiceCardSelfAbility_ModPack21341Init9 : DiceCardSelfAbilityBase
    {
        public static string Desc =
            "[On Use] Kill the Target and make them disappear from the field at the end of the Scene. This page cannot be redirected";

        public override void OnStartBattle()
        {
            owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
            card.target.Die(owner);
        }

        public override bool IsTargetChangable(BattleUnitModel attacker)
        {
            return false;
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