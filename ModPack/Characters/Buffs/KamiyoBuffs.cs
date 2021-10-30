using Sound;
using UnityEngine;

namespace ModPack21341.Characters.Buffs
{
    public class BattleUnitBuf_RedAuraRelease : BattleUnitBuf
    {
        private const string Path = "6/RedHood_Emotion_Aura";
        private Battle.CreatureEffect.CreatureEffect _aura;
        public BattleUnitBuf_RedAuraRelease() => stack = 0;
        public override bool isAssimilation => true;
        public override int paramInBufDesc => 0;
        protected override string keywordId => "Kamiyo";
        protected override string keywordIconId => "RedHood_Rage";
        public override string bufActivatedText => "Power + 1 - Inflict 3 Burn to self at the start of each Scene";
        public override void BeforeRollDice(BattleDiceBehavior behavior) => behavior.ApplyDiceStatBonus(
            new DiceStatBonus
            {
                power = 1
            });

        public override void Init(BattleUnitModel owner)
        {
            base.Init(owner);
            PlayChangingEffect(owner);
        }
        public override void OnRoundStart()
        {
            _owner.bufListDetail.AddKeywordBufByEtc(KeywordBuf.Burn,3,_owner);
        }
        private void PlayChangingEffect(BattleUnitModel owner)
        {
            owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
            if (_aura == null)
                _aura = SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect(Path, 1f, owner.view,
                    owner.view);
            var original = Resources.Load("Prefabs/Battle/SpecialEffect/RedMistRelease_ActivateParticle");
            if (original != null)
            {
                var gameObject = Object.Instantiate(original) as GameObject;
                gameObject.transform.parent = owner.view.charAppearance.transform;
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.localScale = Vector3.one;
            }

            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Kali_Change");
        }
    }
}
