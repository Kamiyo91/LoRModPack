using LOR_DiceSystem;
using ModPack21341.Characters.CommonBuffs;
using ModPack21341.Harmony;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.Gebura.PassiveAbilities
{
    //GeburaUnit
    public class PassiveAbility_ModPack21341Init21 : PassiveAbilityBase
    {
        private int _count;
        private BattleDialogueModel _dlg;
        private bool _egoTransform;
        private bool _oneUseCard;
        private string _originalSkinName;

        public override void OnWaveStart()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                UnitUtilities.PrepareSephirahSkin(owner, 6, "Gebura", owner.faction == Faction.Enemy,
                    ref _originalSkinName, ref _dlg, true, "Kali", true);
            if (!owner.personalEgoDetail.ExistsCard(607022)) owner.personalEgoDetail.AddCard(607022);
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 912));
            if (owner.faction != Faction.Enemy || !(owner.hp < owner.MaxHp * 0.5f)) return;
            _egoTransform = true;
            owner.passiveDetail.AddPassive(new PassiveAbility_ModPack21341Init20());
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
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init4());
            owner.SetHp(owner.MaxHp / 2);
            owner.cardSlotDetail.RecoverPlayPointByCard(6);
            owner.breakDetail.RecoverBreak(owner.breakDetail.GetDefaultBreakGauge());
        }

        public override void OnStartBattle()
        {
            RemoveImmortalBuff();
        }

        private void RemoveImmortalBuff()
        {
            if (owner.bufListDetail.GetActivatedBufList().Find(x => x is BattleUnitBuf_ModPack21341Init4) is
                BattleUnitBuf_ModPack21341Init4 buf)
                owner.bufListDetail.RemoveBuf(buf);
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() == new LorId(ModPack21341Init.PackageId, 912))
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
            if (owner.faction != Faction.Enemy || !_egoTransform || _count < 4 || owner.cardSlotDetail.PlayPoint < 6 ||
                _oneUseCard)
                return;
            _oneUseCard = true;
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(607001)));
        }

        public override void OnRoundEndTheLast()
        {
            _oneUseCard = false;
            if (owner.faction == Faction.Enemy) owner.allyCardDetail.ExhaustCard(new LorId(607001));
        }

        public override void OnRoundEnd()
        {
            switch (_egoTransform)
            {
                case true when !owner.passiveDetail.HasPassive<PassiveAbility_ModPack21341Init20>() &&
                               !owner.passiveDetail.HasPassiveInReady<PassiveAbility_ModPack21341Init20>():
                    owner.passiveDetail.AddPassive(new PassiveAbility_ModPack21341Init20());
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
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem &&
                owner.faction == Faction.Player)
                UnitUtilities.ReturnToTheOriginalBaseSkin(owner, _originalSkinName, _dlg);
        }

        public override void OnRoundStart()
        {
            UpdateResist();
        }

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

        private bool HasEgoPassive()
        {
            return owner.passiveDetail.HasPassive<PassiveAbility_ModPack21341Init20>();
        }
    }
}