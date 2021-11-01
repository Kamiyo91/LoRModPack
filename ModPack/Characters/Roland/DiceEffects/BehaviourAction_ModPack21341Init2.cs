using System;
using UnityEngine;

namespace ModPack21341.Characters.Roland.DiceEffects
{
    //BlackSilenceCustomEgoAreaStrongFinal
    public class BehaviourAction_ModPack21341Init2 : BehaviourActionBase
    {
        public override FarAreaEffect SetFarAreaAtkEffect(BattleUnitModel self)
        {
            _self = self;
            var farAreaeffectBlackSilence4ThAreaStrongFinal =
                new GameObject().AddComponent<FarAreaEffect_ModPack21341Init2>();
            farAreaeffectBlackSilence4ThAreaStrongFinal.Init(self, Array.Empty<object>());
            return farAreaeffectBlackSilence4ThAreaStrongFinal;
        }
    }
}