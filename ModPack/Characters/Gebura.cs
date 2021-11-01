using Battle.CreatureEffect;
using LOR_DiceSystem;
using LOR_XML;
using ModPack21341.Characters.Buffs;
using Sound;
using ModPack21341.Harmony;
using ModPack21341.Utilities;
using UnityEngine;

namespace ModPack21341.Characters
{
    public class PassiveAbility_GeburaUnit : PassiveAbilityBase
    {
        private BattleDialogueModel _dlg;
        private string _originalSkinName;
        private bool _egoTransform;
        private int _count;
        public override void OnWaveStart()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
            {
                UnitUtilities.PrepareSephirahSkin(owner, 6, "Gebura", owner.faction == Faction.Enemy, ref _originalSkinName, ref _dlg, true, "Kali", true);
            }
            if (!owner.personalEgoDetail.ExistsCard(607022))
            {
                owner.personalEgoDetail.AddCard(607022);
            }
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 912));
            if (owner.faction != Faction.Enemy || !(owner.hp < owner.MaxHp * 0.5f)) return;
            _egoTransform = true;
            owner.passiveDetail.AddPassive(new PassiveAbility_GeburaRedMistEgo());
        }

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (owner.faction != Faction.Enemy || !(owner.hp - dmg <= owner.MaxHp * 0.5f) || _egoTransform)
                return base.BeforeTakeDamage(attacker, dmg);
            InitForcedEgoChange();
            return base.BeforeTakeDamage(attacker, dmg);
        }

        private void InitForcedEgoChange()
        {
            _egoTransform = true;
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ImmortalBuffUntilRoundEnd());
            owner.SetHp(owner.MaxHp / 2);
            owner.cardSlotDetail.RecoverPlayPointByCard(6);
            owner.breakDetail.RecoverBreak(owner.breakDetail.GetDefaultBreakGauge());
        }
        public override void OnStartBattle() => RemoveImmortalBuff();
        private void RemoveImmortalBuff()
        {
            if (owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_ImmortalBuffUntilRoundEnd) is
                BattleUnitBuf_ImmortalBuffUntilRoundEnd buf)
                owner.bufListDetail.RemoveBuf(buf);
        }
        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId,912))
                owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 912));
            if (curCard.card.GetID().id == 607001 && curCard.card.GetID().IsBasic())
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
        }

        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            UseEgoMassAttack(ref origin);
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        private void UseEgoMassAttack(ref BattleDiceCardModel origin)
        {
            if (owner.faction != Faction.Enemy || !_egoTransform || _count < 4 || owner.cardSlotDetail.PlayPoint < 6)
                return;
            _count = 0;
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(607001)));
        }

        public override void OnRoundEnd()
        {
            switch (_egoTransform)
            {
                case true when !owner.passiveDetail.HasPassive<PassiveAbility_GeburaRedMistEgo>() && !owner.passiveDetail.HasPassiveInReady<PassiveAbility_GeburaRedMistEgo>():
                    owner.passiveDetail.AddPassive(new PassiveAbility_GeburaRedMistEgo());
                    _count = 4;
                    return;
                case false:
                    return;
                default:
                    owner.allyCardDetail.DrawCards(2);
                    owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
                    _count++;
                    break;
            }
        }

        public override void OnBattleEnd()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem && owner.faction == Faction.Player)
                UnitUtilities.ReturnToTheOriginalBaseSkin(owner, _originalSkinName, _dlg);
        }
        public override void OnRoundStart() => UpdateResist();

        private void UpdateResist()
        {
            var detail = RandomUtil.SelectOne(BehaviourDetail.Slash, BehaviourDetail.Penetrate, BehaviourDetail.Hit);
            if (!HasEgoPassive()) return;
            owner.Book.SetResistHP(BehaviourDetail.Slash, AtkResist.Endure);
            owner.Book.SetResistHP(BehaviourDetail.Penetrate, AtkResist.Endure);
            owner.Book.SetResistHP(BehaviourDetail.Hit, AtkResist.Endure);
            owner.Book.SetResistBP(BehaviourDetail.Slash, AtkResist.Endure);
            owner.Book.SetResistBP(BehaviourDetail.Penetrate, AtkResist.Endure);
            owner.Book.SetResistBP(BehaviourDetail.Hit, AtkResist.Endure);
            owner.Book.SetResistHP(detail, AtkResist.Normal);
            owner.Book.SetResistBP(detail, AtkResist.Normal);
        }

        private bool HasEgoPassive() => owner.passiveDetail.HasPassive<PassiveAbility_GeburaRedMistEgo>();

    }
    public class PassiveAbility_GeburaRedMistEgo : PassiveAbilityBase
    {
        private AudioClip[] _oldEnemytheme;
        private int _roundDamage;
        private bool _egoCancel;
        private string _path = "6/RedHood_Emotion_Aura";
        private CreatureEffect _aura;
        private bool _bDoneEffect;
        public override void OnRoundStart()
        {
            PlayChangingEffect();
            if (owner.faction != Faction.Enemy) return;
            var emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalCoinNumber;
            Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = emotionTotalCoinNumber + 1;
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
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_RedMistEgo());
            if (owner.faction == Faction.Player)
            {
                owner.personalEgoDetail.AddCard(607021);
            }
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
            if (owner.faction == Faction.Enemy)
            {
                owner.UnitData.floorBattleData.param3 = 1;
            }
            return false;
        }
        public override void AfterGiveDamage(int damage)
        {
            _roundDamage += damage;
        }
        public override void OnRoundEnd()
        {
            if (_roundDamage < 40)
            {
                owner.TakeBreakDamage((int)(owner.breakDetail.GetDefaultBreakGauge() * 0.4f), DamageType.Passive);
            }
            _roundDamage = 0;
        }
        public override void OnRoundEndTheLast()
        {
            if (!_egoCancel) return;
            if (_aura != null)
            {
                _aura.ManualDestroy();
            }
            owner.personalEgoDetail.RemoveCard(607021);
            owner.passiveDetail.DestroyPassive(this);
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                owner.view.ResetSkin();
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_RedMistEgo));
            if (_oldEnemytheme != null && owner.faction == Faction.Player)
            {
                SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(_oldEnemytheme);
                Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = 0;
            }
            if (owner.faction == Faction.Enemy)
            {
                owner.view.DisplayDlg(DialogType.SPECIAL_EVENT, "SPECIAL_EVENT_2");
            }
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
            {
                if (Object.Instantiate(@object) is GameObject gameObject)
                {
                    gameObject.transform.parent = owner.view.charAppearance.transform;
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.transform.localRotation = Quaternion.identity;
                    gameObject.transform.localScale = Vector3.one;
                }
            }
            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Kali_Change");
        }
        private void PlayChangingEffect()
        {
            if (_bDoneEffect)
            {
                return;
            }
            owner.bufListDetail.GetReadyBuf(KeywordBuf.Burn)?.Destroy();
            _bDoneEffect = true;
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                owner.view.ChangeSkin("TheRedMist");
            owner.view.charAppearance.ChangeMotion(ActionDetail.Default);
            if (_aura == null)
            {
                _aura = SingletonBehavior<DiceEffectManager>.Instance.CreateCreatureEffect(_path, 1f, owner.view, owner.view);
            }
            SetParticle();
            var emotionTotalCoinNumber = Singleton<StageController>.Instance.GetCurrentStageFloorModel().team.emotionTotalCoinNumber;
            Singleton<StageController>.Instance.GetCurrentWaveModel().team.emotionTotalBonus = emotionTotalCoinNumber + 1;
            if (owner.faction != Faction.Player) return;
            var array = new AudioClip[3];
            var audioClip = Resources.Load<AudioClip>("Sounds/Battle/RedMistBgm");
            if (audioClip != null)
            {
                for (var i = 0; i < array.Length; i++)
                {
                    array[i] = audioClip;
                }

                _oldEnemytheme = SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(array);
                SingletonBehavior<BattleSoundManager>.Instance.ChangeEnemyTheme(0);
                return;
            }

            Debug.LogError("Bgm Not found : red mist");
        }
    }
}
