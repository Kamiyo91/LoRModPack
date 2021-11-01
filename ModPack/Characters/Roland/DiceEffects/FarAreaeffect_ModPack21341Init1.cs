using ModPack21341.StageManager.MapManager.BlackSilenceEgoMapManager;
using Sound;
using UnityEngine;

namespace ModPack21341.Characters.Roland.DiceEffects
{
    //BlackSilenceCustomEgoAreaStrong
    public class FarAreaeffect_ModPack21341Init1 : FarAreaEffect
    {
        private bool _damaged;
        private float _elapsed;
        private bool _ended;
        private ModPack21341InitBlackSilenceEgoMapManager _map;

        private ModPack21341InitBlackSilenceEgoMapManager Map
        {
            get
            {
                if (_map == null)
                    _map =
                        SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject as
                            ModPack21341InitBlackSilenceEgoMapManager;
                return _map;
            }
        }

        public override void OnGiveDamage()
        {
            base.OnGiveDamage();
            var map = Map;
            if (map != null) map.BoomFirst();
            PrintSound();
            isRunning = false;
        }

        public override void Init(BattleUnitModel self, params object[] args)
        {
            base.Init(self, args);
            SoundEffectPlayer.PlaySound("Battle/Roland_Phase4_CryStart");
        }

        public virtual void PrintSound()
        {
            SoundEffectPlayer.PlaySound("Battle/Roland_Phase2_Windblast");
        }

        public override void OnEffectEnd()
        {
            base.OnEffectEnd();
            _isDoneEffect = true;
            gameObject.SetActive(false);
        }

        protected override void Update()
        {
            base.Update();
            _elapsed += Time.deltaTime;
            if (!_damaged && _elapsed >= 0.4f)
            {
                _damaged = true;
                OnGiveDamage();
            }

            if (!_ended && _elapsed >= 0.8f)
            {
                _ended = true;
                OnEffectEnd();
            }
        }
    }
}