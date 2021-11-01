using ModPack21341.Utilities;

namespace ModPack21341.Characters.Binah.PassiveAbilities
{
    //BinahUnit
    public class PassiveAbility_ModPack21341Init5 : PassiveAbilityBase
    {
        private BattleDialogueModel _dlg;
        private string _originalSkinName;

        public override void OnWaveStart()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                UnitUtilities.PrepareSephirahSkin(owner, 8, "Binah", owner.faction == Faction.Enemy,
                    ref _originalSkinName, ref _dlg, true);
        }

        public override void OnBattleEnd()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                UnitUtilities.ReturnToTheOriginalBaseSkin(owner, _originalSkinName, _dlg);
        }
    }
}