using System;
using UnityEngine;

namespace ModPack21341.Characters.Roland.DiceEffects
{
    //BlackSilenceCustomEgoAreaStrong
    public class BehaviourAction_ModPack21341Init1 : BehaviourActionBase
    {
        public override FarAreaEffect SetFarAreaAtkEffect(BattleUnitModel self)
        {
            _self = self;
            var farAreaeffectBlackSilence4ThAreaStrong =
                new GameObject().AddComponent<FarAreaeffect_ModPack21341Init1>();
            farAreaeffectBlackSilence4ThAreaStrong.Init(self, Array.Empty<object>());
            return farAreaeffectBlackSilence4ThAreaStrong;
        }
    }
}