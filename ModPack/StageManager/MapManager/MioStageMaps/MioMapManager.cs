using CustomMapUtility;
using UnityEngine;

namespace ModPack21341.StageManager.MapManager.MioStageMaps
{
    public class MioMapManager : CustomCreatureMapManager
    {
        private int _min;
        private int _max;
        private int _lastRnd;
        private bool _dlgActivated;
        protected internal override string[] CustomBGMs
        {
            get { return new[] { "MioPhase1.mp3" }; }
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
            if (Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_Mio stage)
            {
                _dlgActivated = stage.GetPhaseStatus();
            }
            else
            {
                _dlgActivated = false;
            }
        }
        public void InitDlg(int mn,int mx)
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
            var rnd = new System.Random();
            _dlgIdx = rnd.Next(_min, _max);
            while (_lastRnd == _dlgIdx && _lastRnd != -1 && _dlgIdx != 0)
            {
                _dlgIdx = rnd.Next(_min, _max);
            }
            var text = _creatureDlgIdList[_dlgIdx];
            _dlgEffect = SingletonBehavior<CreatureDlgManagerUI>.Instance.SetDlg(text);
            _lastRnd = _dlgIdx;
        }
    }
}
