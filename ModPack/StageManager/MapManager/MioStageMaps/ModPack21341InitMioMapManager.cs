using ModPack21341.Utilities.CustomMapUtility.Assemblies;
using UnityEngine;
using Random = System.Random;

namespace ModPack21341.StageManager.MapManager.MioStageMaps
{
    public class ModPack21341InitMioMapManager : CustomCreatureMapManager
    {
        private bool _dlgActivated;
        private int _lastRnd;
        private int _max;
        private int _min;

        protected internal override string[] CustomBgMs
        {
            get { return new[] {"MioPhase1.mp3"}; }
        }

        public override void InitializeMap()
        {
            base.InitializeMap();
            sephirahType = SephirahType.None;
            sephirahColor = Color.black;
            CheckPhaseDlg();
        }

        private void CheckPhaseDlg()
        {
            if (Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_ModPack21341Init2 stage)
                _dlgActivated = stage.GetPhaseStatus();
            else
                _dlgActivated = false;
        }

        public void InitDlg(int mn, int mx)
        {
            _lastRnd = -1;
            _min = mn;
            _max = mx;
            _dlgIdx = 0;
            // Phase 2
            _creatureDlgIdList.Add("This isn't how it should have ended!");
            _creatureDlgIdList.Add("Ahhhh!!!");
            _creatureDlgIdList.Add("Father, I'll not forgive you...");
            _creatureDlgIdList.Add("No one will stand in my way!");
            _dlgActivated = true;
            CreateDialog();
            SingletonBehavior<CreatureDlgManagerUI>.Instance.Init(true);
        }

        public override void CreateDialog()
        {
            if (!_dlgActivated) return;
            var rnd = new Random();
            _dlgIdx = rnd.Next(_min, _max);
            while (_lastRnd == _dlgIdx && _lastRnd != -1 && _dlgIdx != 0) _dlgIdx = rnd.Next(_min, _max);
            var text = _creatureDlgIdList[_dlgIdx];
            _dlgEffect = SingletonBehavior<CreatureDlgManagerUI>.Instance.SetDlg(text);
            _lastRnd = _dlgIdx;
        }
    }
}