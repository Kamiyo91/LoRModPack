using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModPack21341.Characters.Roland.DiceEffects
{
    //BlackSilenceCustomEgoShortSword
    public class BehaviourAction_ModPack21341Init5 : BehaviourActionBase
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
