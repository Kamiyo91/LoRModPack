using System;
using System.Collections.Generic;
using ModPack21341.StageManager.MapManager.BlackSilenceEgoMapManager;
using Sound;
using UnityEngine;

namespace ModPack21341.Characters.DiceEffects
{
    public class BehaviourAction_BlackSilenceCustomEgo_Gauntlet1 : BehaviourActionBase
    {
        public override List<RencounterManager.MovingAction> GetMovingAction(ref RencounterManager.ActionAfterBehaviour self, ref RencounterManager.ActionAfterBehaviour opponent)
        {
            if (self.result != Result.Win || self.data.actionType != ActionType.Atk || opponent.behaviourResultData == null || opponent.behaviourResultData.IsFarAtk())
                return base.GetMovingAction(ref self, ref opponent);
            var list = new List<RencounterManager.MovingAction>();
            var movingAction = new RencounterManager.MovingAction(ActionDetail.Penetrate, CharMoveState.MoveForward, 30f, false, 0.9f);
            movingAction.SetEffectTiming(EffectTiming.PRE, EffectTiming.PRE, EffectTiming.PRE);
            new RencounterManager.MovingAction(ActionDetail.Penetrate, CharMoveState.Stop, 0f, true, 0.1f).SetEffectTiming(EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT);
            list.Add(movingAction);
            opponent.infoList.Add(new RencounterManager.MovingAction(ActionDetail.Damaged, CharMoveState.Stop, 1f, false, 0.5f));
            return list;
        }
    }

    public class BehaviourAction_BlackSilenceCustomEgo_Gauntlet2 : BehaviourActionBase
    {
        public override List<RencounterManager.MovingAction> GetMovingAction(
            ref RencounterManager.ActionAfterBehaviour self, ref RencounterManager.ActionAfterBehaviour opponent)
        {
            if (self.result != Result.Win || self.data.actionType != ActionType.Atk ||
                opponent.behaviourResultData == null || opponent.behaviourResultData.IsFarAtk())
                return base.GetMovingAction(ref self, ref opponent);
            var list = new List<RencounterManager.MovingAction>();
            var movingAction =
                new RencounterManager.MovingAction(ActionDetail.Penetrate, CharMoveState.MoveForward, 30f, false, 0.9f);
            movingAction.SetEffectTiming(EffectTiming.PRE, EffectTiming.PRE, EffectTiming.PRE);
            new RencounterManager.MovingAction(ActionDetail.Penetrate, CharMoveState.Stop, 0f, true, 0.1f)
                .SetEffectTiming(EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT);
            list.Add(movingAction);
            opponent.infoList.Add(
                new RencounterManager.MovingAction(ActionDetail.Damaged, CharMoveState.Stop, 1f, false, 0.5f));
            return list;
        }

        public class BehaviourAction_BlackSilenceCustomEgo_ShortSword : BehaviourActionBase
        {
            public override List<RencounterManager.MovingAction> GetMovingAction(
                ref RencounterManager.ActionAfterBehaviour self, ref RencounterManager.ActionAfterBehaviour opponent)
            {
                if (self.result != Result.Win || self.data.actionType != ActionType.Atk ||
                    opponent.behaviourResultData != null || opponent.behaviourResultData.IsFarAtk())
                    return base.GetMovingAction(ref self, ref opponent);
                var list = new List<RencounterManager.MovingAction>();
                var movingAction = new RencounterManager.MovingAction(ActionDetail.Slash, CharMoveState.MoveForward,
                    30f, false, 0.9f)
                { customEffectRes = "FX_PC_RolRang_Dagger" };
                movingAction.SetEffectTiming(EffectTiming.PRE, EffectTiming.PRE, EffectTiming.PRE);
                new RencounterManager.MovingAction(ActionDetail.Slash, CharMoveState.Stop, 0f, true, 0.1f)
                    .SetEffectTiming(EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT, EffectTiming.NOT_PRINT);
                list.Add(movingAction);
                opponent.infoList.Add(new RencounterManager.MovingAction(ActionDetail.Damaged, CharMoveState.Stop, 1f,
                    false, 0.5f));
                return list;
            }
        }
    }

    public class FarAreaeffect_BlackSilence_CustomEgo_Area_Strong : FarAreaEffect
    {
        private BlackSilenceEgoMapManager _map;
        private float elapsed;
        private bool damaged;
        private bool ended;
        private BlackSilenceEgoMapManager Map
        {
            get
            {
                if (_map == null)
                {
                    _map = (SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject as BlackSilenceEgoMapManager);
                }
                return _map;
            }
        }
        public override void OnGiveDamage()
        {
            base.OnGiveDamage();
            var map = Map;
            if (map != null)
            {
                map.BoomFirst();
            }
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
            elapsed += Time.deltaTime;
            if (!damaged && elapsed >= 0.4f)
            {
                damaged = true;
                OnGiveDamage();
            }
            if (!ended && elapsed >= 0.8f)
            {
                ended = true;
                OnEffectEnd();
            }
        }
    }

    public class FarAreaeffect_BlackSilence_CustomEgo_Area_Strong_Final : FarAreaEffect
    {
        private BlackSilenceEgoMapManager _map;
        private float elapsed;
        private bool damaged;
        private bool ended;
        private BlackSilenceEgoMapManager Map
        {
            get
            {
                if (_map == null)
                {
                    _map =
                        (SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject as BlackSilenceEgoMapManager);
                }

                return _map;
            }
        }
        public override void OnGiveDamage()
        {
            base.OnGiveDamage();
            var map = Map;
            if (map != null)
            {
                map.BoomSecond();
            }

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
            elapsed += Time.deltaTime;
            if (!damaged && elapsed >= 0.4f)
            {
                damaged = true;
                OnGiveDamage();
            }

            if (!ended && elapsed >= 1f)
            {
                ended = true;
                OnEffectEnd();
            }
        }
    }
    public class BehaviourAction_BlackSilence_CustomEgo_Area_Strong : BehaviourActionBase
    {
        public override FarAreaEffect SetFarAreaAtkEffect(BattleUnitModel self)
        {
            _self = self;
            var farAreaeffect_BlackSilence_4th_Area_Strong = new GameObject().AddComponent<FarAreaeffect_BlackSilence_CustomEgo_Area_Strong>();
            farAreaeffect_BlackSilence_4th_Area_Strong.Init(self, Array.Empty<object>());
            return farAreaeffect_BlackSilence_4th_Area_Strong;
        }
    }
    public class BehaviourAction_BlackSilence_CustomEgo_Area_Strong_Final : BehaviourActionBase
    {
        public override FarAreaEffect SetFarAreaAtkEffect(BattleUnitModel self)
        {
            _self = self;
            var farAreaeffect_BlackSilence_4th_Area_Strong_Final = new GameObject().AddComponent<FarAreaeffect_BlackSilence_CustomEgo_Area_Strong_Final>();
            farAreaeffect_BlackSilence_4th_Area_Strong_Final.Init(self, Array.Empty<object>());
            return farAreaeffect_BlackSilence_4th_Area_Strong_Final;
        }
    }

}
