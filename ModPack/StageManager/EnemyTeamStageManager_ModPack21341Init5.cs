using System.Linq;
using System.Threading.Tasks;
using ModPack21341.Characters.Hayate.Buffs;
using ModPack21341.Characters.Hayate.PassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.StageManager.MapManager.HayateStageMaps;
using ModPack21341.Utilities;
using ModPack21341.Utilities.CustomMapUtility.Assemblies;

namespace ModPack21341.StageManager
{
    //Hayate Solo
    public class EnemyTeamStageManager_ModPack21341Init5 : EnemyTeamStageManager
    {
        private Task _changeBgm;
        private BattleUnitModel _hayateModel;
        private PassiveAbility_ModPack21341Init22 _hayatePassive;
        private bool _phaseChanged;
        private bool _startFight;

        public override void OnWaveStart()
        {
            CustomMapHandler.InitCustomMap("Hayate", new ModPack21341InitHayateMapManager(), false, true, 0.5f, 0.3f,
                0.5f, 0.475f);
            CustomMapHandler.EnforceMap();
            Singleton<StageController>.Instance.CheckMapChange();
            _hayateModel = BattleObjectManager.instance.GetAliveList(Faction.Enemy).FirstOrDefault();
            if (_hayateModel?.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init22) is
                PassiveAbility_ModPack21341Init22 hayatePassive)
                _hayatePassive = hayatePassive;
            _phaseChanged = false;
            _startFight = false;
        }

        public override void OnRoundStart()
        {
            CustomMapHandler.EnforceMap();
            MapUtilities.CheckAndChangeBgm(ref _changeBgm);
        }

        public override void OnRoundEndTheLast()
        {
            CheckPhase();
        }

        public override void OnRoundStart_After()
        {
            if (!_startFight)
            {
                _startFight = true;
                _hayatePassive.SetOriginalPhaseIgnore();
            }

            if (_hayateModel.bufListDetail.GetActivatedBufList().Exists(x => x is BattleUnitBuf_ModPack21341Init10))
                MapUtilities.ActiveCreatureBattleCamFilterComponent();
        }

        private void CheckPhase()
        {
            if (_phaseChanged || !(_hayateModel.hp < _hayateModel.MaxHp * 0.5f)) return;
            _phaseChanged = true;
            _hayateModel.RecoverHP(_hayateModel.MaxHp);
            _hayateModel.breakDetail.ResetGauge();
            _hayateModel.breakDetail.RecoverBreakLife(1, true);
            _hayateModel.breakDetail.nextTurnBreak = false;
            _hayateModel.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 52));
            _hayateModel.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ModPack21341Init9));
            _hayatePassive.ActiveEgo();
            _hayatePassive.SetPhase2Solo();
            MapUtilities.PrepareChangeBgm("HayatePhase2.mp3", ref _changeBgm);
        }
    }
}