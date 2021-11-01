using Sound;
using UnityEngine;

namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_ImmortalForTestingBuf : BattleUnitBuf
    {
        public override bool IsImmortal() => true;
        public override bool IsInvincibleHp(BattleUnitModel attacker) => true;
        public override bool IsInvincibleBp(BattleUnitModel attacker) => true;
        public override void OnRoundStart() => _owner.cardSlotDetail.RecoverPlayPoint(_owner.cardSlotDetail.GetMaxPlayPoint());
        public override void BeforeRollDice(BattleDiceBehavior behavior) => behavior.ApplyDiceStatBonus(
            new DiceStatBonus
            {
                power = 30
            });

    }
    public class BattleUnitBuf_HayateImmortal: BattleUnitBuf
    {
        public override bool IsImmortal() => true;
    }
    public class BattleUnitBuf_NullImmunity : BattleUnitBuf
    {
        public override bool IsImmune(KeywordBuf buf) => buf == KeywordBuf.NullifyPower;
        public override void OnRoundEnd() => Destroy();
    }
    public class BattleUnitBuf_ImmortalBuffUntilRoundEnd : BattleUnitBuf
    {
        public override bool IsImmortal() => true;
        public override bool IsInvincibleHp(BattleUnitModel attacker) => true;
        public override void OnRoundEnd() => Destroy();
    }
    public class BattleUnitBuf_ImmortalBuff: BattleUnitBuf
    {
        public override bool IsImmortal() => true;
        public override bool IsInvincibleHp(BattleUnitModel attacker) => true;
        public override bool IsInvincibleBp(BattleUnitModel attacker) => true;
    }
    public class BattleUnitBuf_UntargetableBuf : BattleUnitBuf
    {
        public override bool IsTargetable() => false;
    }
    public class BattleUnitBuf_CustomInstantIndexRelease : BattleUnitBuf
    {
        private GameObject _aura;
        public override KeywordBuf bufType => KeywordBuf.IndexRelease;
        protected override string keywordId => "IndexRelease";
        protected override string keywordIconId => "IndexRelease";
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 1
            });
        }
        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            SetParticle(owner);
        }
        private void SetParticle(BattleUnitModel owner)
        {
            if (_aura != null) return;
            var @object = Resources.Load("Prefabs/Battle/SpecialEffect/IndexRelease_Aura");
            if (@object != null)
            {
                var gameObject = Object.Instantiate(@object) as GameObject;
                gameObject.transform.parent = owner.view.charAppearance.transform;
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.localScale = Vector3.one;
                var component = gameObject.GetComponent<IndexReleaseAura>();
                if (component != null)
                {
                    component.Init(owner.view);
                }
                _aura = gameObject;
            }
            var object2 = Resources.Load("Prefabs/Battle/SpecialEffect/IndexRelease_ActivateParticle");
            if (object2 != null)
            {
                var gameObject2 = Object.Instantiate(object2) as GameObject;
                gameObject2.transform.parent = owner.view.charAppearance.transform;
                gameObject2.transform.localPosition = Vector3.zero;
                gameObject2.transform.localRotation = Quaternion.identity;
                gameObject2.transform.localScale = Vector3.one;
            }
            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Buf/Effect_Index_Unlock", false, 1f, null);
        }
    }
}
