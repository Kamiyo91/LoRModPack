using System;

namespace ModPack21341.StageManager.MapManager.DlgManager
{
    public class MioDlgManager : CreatureMapManager
    {
        private int _min;
        private int _max;
        private int _lastRnd;

        public void Init(int mn, int mx)
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
            CreateDialog();
            SingletonBehavior<CreatureDlgManagerUI>.Instance.Init(true);
        }
        public override void CreateDialog()
        {
            var rnd = new Random();
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
