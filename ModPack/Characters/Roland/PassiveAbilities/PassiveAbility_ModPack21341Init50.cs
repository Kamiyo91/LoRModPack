using ModPack21341.Characters.CommonBuffs;
using ModPack21341.Characters.CommonPassiveAbilities;
using ModPack21341.Characters.Roland.Buffs;
using ModPack21341.Harmony;
using ModPack21341.Utilities;
using Sound;

namespace ModPack21341.Characters.Roland.PassiveAbilities
{
    //RolandUnit
    public class PassiveAbility_ModPack21341Init50 : PassiveAbilityBase
    {
        private bool _blackCheck;
        private int _count;
        private BattleDialogueModel _dlg;
        private bool _halfHpReached;
        private string _originalSkinName;
        private bool _specialActivated;
        private bool _usedMassEgo;
        private bool _oneUseCard;

        public override void OnBattleEnd()
        {
            if (!string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin)) return;
            UnitUtilities.ReturnToTheOriginalBaseSkin(owner, _originalSkinName, _dlg);
        }

        public override bool BeforeTakeDamage(BattleUnitModel attacker, int dmg)
        {
            if (owner.faction != Faction.Enemy || !(owner.hp - dmg <= owner.MaxHp * 0.5f) || _blackCheck ||
                _halfHpReached)
                return base.BeforeTakeDamage(attacker, dmg);
            InitEgoChange();
            return base.BeforeTakeDamage(attacker, dmg);
        }

        public override void OnRoundStart()
        {
            if (!_usedMassEgo) return;
            _usedMassEgo = false;
            MapUtilities.ReturnFromEgoMap("BlackSilenceMassEgo", owner, 2, true);
        }

        private void InitEgoChange()
        {
            _halfHpReached = true;
            _blackCheck = true;
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init4());
            owner.SetHp(owner.MaxHp / 2);
            owner.cardSlotDetail.RecoverPlayPointByCard(6);
            owner.breakDetail.RecoverBreak(owner.breakDetail.GetDefaultBreakGauge());
        }

        private void CheckRolandUnitAndChangeSkin()
        {
            if (!owner.passiveDetail.HasPassive<PassiveAbility_ModPack21341Init48>() &&
                !owner.passiveDetail.HasPassive<PassiveAbility_10012>())
                owner.passiveDetail.DestroyPassive(this);
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin))
                UnitUtilities.PrepareSephirahSkin(owner, 10, "Roland", owner.faction == Faction.Enemy,
                    ref _originalSkinName, ref _dlg, true, "BlackSilence", true);
        }

        public override void OnWaveStart()
        {
            CheckRolandUnitAndChangeSkin();
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 911));
            if (owner.faction != Faction.Enemy || !(owner.hp < owner.MaxHp * 0.5f)) return;
            _halfHpReached = true;
            ChangeToBlackSilence();
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 29) ||
                curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 26))
            {
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
                if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 26))
                {
                    _count = 0;
                    _usedMassEgo = true;
                }
            }

            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 910))
                _usedMassEgo = true;
            if (curCard.card.GetID() != new LorId(ModPack21341Init.PackageId, 911)) return;
            owner.personalEgoDetail.RemoveCard(curCard.card.GetID());
            _blackCheck = true;
        }

        public override void OnRoundEndTheLast()
        {
            _specialActivated = false;
            _oneUseCard = false;
            if (owner.faction == Faction.Enemy)
            {
                owner.allyCardDetail.ExhaustCard(new LorId(ModPack21341Init.PackageId, 26));
                owner.allyCardDetail.ExhaustCard(new LorId(ModPack21341Init.PackageId, 29));
            }
            if (_blackCheck)
                ChangeToBlackSilence();
            if (owner.faction == Faction.Enemy && owner.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_ModPack21341Init25))
                _count++;
        }

        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            if (owner.faction != Faction.Enemy) return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            if (owner.bufListDetail.GetActivatedBufList()
                    .Find(x => x is PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount) is
                PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount buf)
            {
                UseFuriosoCard(ref origin, buf);
                return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
            }

            UseEgoMassAttackCard(ref origin);
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        private void UseFuriosoCard(ref BattleDiceCardModel origin, BattleUnitBuf buf)
        {
            if (buf.stack < 9 || _specialActivated || owner.cardSlotDetail.PlayPoint < 3) return;
            _specialActivated = true;
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, 29)));
        }

        private void UseEgoMassAttackCard(ref BattleDiceCardModel origin)
        {
            if (!owner.bufListDetail.GetActivatedBufList()
                    .Exists(x => x is BattleUnitBuf_ModPack21341Init25 && _count >= 4) ||
                owner.cardSlotDetail.PlayPoint < 5 || _oneUseCard)
                return;
            _oneUseCard = true;
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, 26)));
        }

        private void ChangeToBlackSilence()
        {
            _blackCheck = false;
            if (owner.bufListDetail.GetActivatedBufList()
                .Find(x => x is PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount) is PassiveAbility_10012
                .BattleUnitBuf_blackSilenceSpecialCount)
                owner.bufListDetail.RemoveBufAll(typeof(PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount));
            if (owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_ModPack21341Init48) is
                PassiveAbility_ModPack21341Init48
                passiveAbilityBase) owner.passiveDetail.DestroyPassive(passiveAbilityBase);
            if (owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_10012) is PassiveAbility_10012
                passiveAbilityBaseOriginal) owner.passiveDetail.DestroyPassive(passiveAbilityBaseOriginal);
            foreach (var battleDiceCardModel in owner.allyCardDetail.GetAllDeck())
                battleDiceCardModel.RemoveBuf<PassiveAbility_10012.BattleDiceCardBuf_blackSilenceEgoCount>();
            owner.personalEgoDetail.RemoveCard(702010);
            owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 913));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 910));
            ChangeToBlackSilenceMaskDeck();
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init25());
            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Kali_Change");
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin))
                owner.view.ChangeSkin("BlackSilence3");
            if (owner.faction == Faction.Enemy) _count = 4;
        }

        private void ChangeToBlackSilenceMaskDeck()
        {
            if (!(owner.passiveDetail.AddPassive(new LorId(ModPack21341Init.PackageId, 10)) is
                PassiveAbility_ModPack21341Init8
                passive)) return;
            passive.Init(owner);
            passive.SaveAwakenedDeck(UnitUtilities.GetBlackSilenceMaskCardsId());
            passive.ChangeToTheBlackSilenceMaskDeck();
        }
    }
}