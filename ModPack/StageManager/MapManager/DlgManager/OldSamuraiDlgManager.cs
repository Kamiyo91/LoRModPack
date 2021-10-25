﻿using System;

namespace ModPack21341.StageManager.MapManager.DlgManager
{
    public class OldSamuraiDlgManager : CreatureMapManager
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

        public void DestroyAndInit(int mn, int mx)
        {
            OnDestroy();
            Init(mn, mx);
        }
    }
}
