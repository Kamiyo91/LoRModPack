using CustomMapUtility;
using UnityEngine;

namespace ModPack21341.StageManager.MapManager.KamiyoStageMaps
{
    public class Kamiyo2MapManager : CustomCreatureMapManager
    {
        private int _min;
        private int _max;
        private int _lastRnd;
        private bool _dlgActivated;
        protected internal override string[] CustomBGMs
        {
            get { return new[] { "KamiyoPhase2.mp3" }; }
        }

        public override void InitializeMap()
        {
            base.InitializeMap();
            sephirahType = SephirahType.None;
            sephirahColor = Color.black;
        }
        public void InitDlg(int mn, int mx)
        {
            _lastRnd = -1;
            _min = mn;
            _max = mx;
            _dlgIdx = 0;
            // Phase 2
            _creatureDlgIdList.Add("You need me...don't you?");
            _creatureDlgIdList.Add("I'll slaughter you all!");
            _creatureDlgIdList.Add("That day...I ran away...");
            _creatureDlgIdList.Add("Why?There is no need to say another word");
            _creatureDlgIdList.Add("...");
            _creatureDlgIdList.Add("I'll not surrender.");
            _creatureDlgIdList.Add("When she died I could only run, I couldn't do anything!");
            _dlgActivated = true;
            CreateDialog();
            SingletonBehavior<CreatureDlgManagerUI>.Instance.Init(true);
        }
        public override void CreateDialog()
        {
            if (!_dlgActivated) return;
            var rnd = new System.Random();
            _dlgIdx = rnd.Next(_min, _max);
            while (_lastRnd == _dlgIdx && _lastRnd != -1)
            {
                _dlgIdx = rnd.Next(_min, _max);
            }
            var text = _creatureDlgIdList[_dlgIdx];
            _dlgEffect = SingletonBehavior<CreatureDlgManagerUI>.Instance.SetDlg(text);
            _lastRnd = _dlgIdx;
        }
    }
}
