using ModPack21341.Utilities.CustomMapUtility.Assemblies;
using UnityEngine;
using Random = System.Random;

namespace ModPack21341.StageManager.MapManager.OldSamuraiStageMaps
{
    public class ModPack21341InitOldSamuraiMapManager : CustomCreatureMapManager
    {
        private bool _dlgActivated;
        private int _lastRnd;
        private int _max;
        private int _min;

        protected internal override string[] CustomBgMs
        {
            get { return new[] {"Reflection.mp3"}; }
        }

        public override void InitializeMap()
        {
            base.InitializeMap();
            sephirahType = SephirahType.None;
            sephirahColor = Color.cyan;
            _dlgActivated =
                Singleton<StageController>.Instance.EnemyStageManager is EnemyTeamStageManager_ModPack21341Init1;
        }

        public void InitDlg(int mn, int mx)
        {
            _lastRnd = -1;
            _min = mn;
            _max = mx;
            _dlgIdx = 0;
            // Phase 1
            _creatureDlgIdList.Add("This place is...where I belong...");
            _creatureDlgIdList.Add("No one should be here.");
            _creatureDlgIdList.Add("Let's see how you handle the way of the Blade.");
            _creatureDlgIdList.Add("So.Are you ready?");
            //Both Phases
            _creatureDlgIdList.Add("...");
            // Phase 2
            _creatureDlgIdList.Add("I'll not surrender.");
            _creatureDlgIdList.Add("You are quite strong.");
            _creatureDlgIdList.Add("My old companions...");
            _creatureDlgIdList.Add("When we are together no one can stop us");
            _dlgActivated = true;
            CreateDialog();
            SingletonBehavior<CreatureDlgManagerUI>.Instance.Init(true);
        }

        public override void CreateDialog()
        {
            if (!_dlgActivated) return;
            var rnd = new Random();
            _dlgIdx = rnd.Next(_min, _max);
            while (_lastRnd == _dlgIdx && _lastRnd != -1) _dlgIdx = rnd.Next(_min, _max);
            var text = _creatureDlgIdList[_dlgIdx];
            _dlgEffect = SingletonBehavior<CreatureDlgManagerUI>.Instance.SetDlg(text);
            _lastRnd = _dlgIdx;
        }

        private void ChangeDlg(int mn, int mx)
        {
            _lastRnd = -1;
            _min = mn;
            _max = mx;
            _dlgIdx = 0;
            CreateDialog();
            SingletonBehavior<CreatureDlgManagerUI>.Instance.Init(true);
        }

        public void ChangeDlgPhase(int mn, int mx)
        {
            OnDestroy();
            ChangeDlg(mn, mx);
        }
    }
}