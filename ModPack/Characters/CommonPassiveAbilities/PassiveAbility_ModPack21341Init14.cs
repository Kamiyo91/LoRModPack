using LOR_XML;
using ModPack21341.Harmony;

namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //Hod
    public class PassiveAbility_ModPack21341Init14 : PassiveAbilityBase
    {
        private BattleDialogueModel _dlg;

        public override void OnWaveStart()
        {
            ChangeHodDialog();
        }

        private void ChangeHodDialog()
        {
            _dlg = owner.UnitData.unitData.battleDialogModel;
            owner.UnitData.unitData.InitBattleDialogByDefaultBook(new LorId(ModPack21341Init.PackageId, 200));
            owner.view.DisplayDlg(DialogType.START_BATTLE, "0");
        }

        public override void OnBattleEnd()
        {
            owner.UnitData.unitData.battleDialogModel = _dlg;
        }
    }
}