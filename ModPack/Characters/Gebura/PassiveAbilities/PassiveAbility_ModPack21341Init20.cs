using Battle.CreatureEffect;
using LOR_DiceSystem;
using LOR_XML;
using ModPack21341.Characters.Gebura.Buffs;
using Sound;
using UnityEngine;

namespace ModPack21341.Characters.Gebura.PassiveAbilities
{
    //GeburaRedMistEgo
    public class PassiveAbility_ModPack21341Init20 : PassiveAbilityBase
    {
        private readonly string _path = "6/RedHood_Emotion_Aura";
        private CreatureEffect _aura;
        private bool _bDoneEffect;
        private bool _egoCancel;
        private AudioClip[] _oldEnemytheme;
        private int _roundDamage;

        public override void OnRoundStart()
        {
            PlayChangingEffect();
            if (owner.faction != Faction.Enemy) return;
            var emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team
                .emotionTotalCoinNumber;
            Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus =
                emotionTotalCoinNumber + 1;
        }

        public override void OnCreated()
        {
            if (owner.bufListDetail.HasAssimilation())
            {
                owner.passiveDetail.DestroyPassive(this);
                return;
            }

            name = Singleton<PassiveDescXmlList>.Instance.GetName(250422);
            desc = Singleton<PassiveDescXmlList>.Instance.GetDesc(250422);
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init7());
            if (owner.faction == Faction.Player) owner.personalEgoDetail.AddCard(607021);
        }

        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            behavior.ApplyDiceStatBonus(new DiceStatBonus
            {
                power = 2
            });
        }

        public override bool OnBreakGageZero()
        {
            _egoCancel = true;
            if (owner.faction == Faction.Enemy) owner.UnitData.floorBattleData.param3 = 1;
            return false;
        }

        public override void AfterGiveDamage(int damage)
        {
            _roundDamage += damage;
        }

        public override void OnRoundEnd()
        {
            if (_roundDamage < 40)
                owner.TakeBreakDamage((int) (owner.breakDetail.GetDefaultBreakGauge() * 0.4f), DamageType.Passive);
            _roundDamage = 0;
        }

        public override void OnRoundEndTheLast()
        {
            if (!_egoCancel) return;
            if (_aura != null) _aura.ManualDestroy();
            owner.personalEgoDetail.RemoveCard(607021);
            owner.passiveDetail.DestroyPassive(this);
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                owner.view.ResetSkin();
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ModPack21341Init7));
            if (_oldEnemytheme != null && owner.faction == Faction.Player)
            {
                SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(_oldEnemytheme);
                Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = 0;
            }

            if (owner.faction == Faction.Enemy) owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_2");
            ResetResist();
        }

        private void ResetResist()
        {
            owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Normal);
            owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Normal);
            owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Endure);
            owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Normal);
            owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Normal);
            owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Endure);
        }

        public override void OnKill(BattleUnitModel target)
        {
            owner.bufListDetail.AddKeywordBufThisRoundByEtc(KeywordBuf.RedMist, 1);
        }

        private void SetParticle()
        {
            var @object = Resources.Load("Prefabs/Battle/SpecialEffect/RedMistRelease_ActivateParticle");
            if (@object != null)
                if (Object.Instantiate(@object) is GameObject gameObject)
                {
                    gameObject.transform.parent = owner.view.charAppearance.transform;
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.identity;
                    gameObject.transform.localScale = Vector3.one;
                }

            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Kali_Change");
        }

        private void PlayChangingEffect()
        {
            if (_bDoneEffect) return;
            owner.bufListDetail.GetReadyBuf(KeywordBuf.Burn)?.Destroy();
            _bDoneEffect = true;
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                owner.view.ChangeSkin("TheRedMist");
            owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
            if (_aura == null)
                _aura = SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect(_path, 1f, owner.view,
                    owner.view);
            SetParticle();
            var emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team
                .emotionTotalCoinNumber;
            Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus =
                emotionTotalCoinNumber + 1;
            if (owner.faction != Faction.Player) return;
            var array = new AudioClip[3];
            var audioClip = Resources.Load<AudioClip>("Sounds/Battle/RedMistBgm");
            if (audioClip != null)
            {
                for (var i = 0; i < array.Length; i++) array[i] = audioClip;

                _oldEnemytheme = SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(array);
                SingletonBehavior<BattleSoundManager>.Instance.ChangeEnemyTheme(0);
                return;
            }

            Debug.LogError("Bgm Not found : red mist");
        }
    }
}