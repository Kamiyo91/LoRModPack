using System.Collections.Generic;
using System.Linq;
using ModPack21341.Characters.Angela.Buffs;
using ModPack21341.Harmony;

namespace ModPack21341.Characters.Angela.PassiveAbilities
{
    //AngelaEnemyUnit
    public class PassiveAbility_ModPack21341Init1 : PassiveAbilityBase
    {
        private readonly List<LorId> _egoCards = new List<LorId>
        {
            new LorId(ModPack21341Init.PackageId, 918),
            new LorId(ModPack21341Init.PackageId, 919),
            new LorId(ModPack21341Init.PackageId, 920),
            new LorId(ModPack21341Init.PackageId, 921),
            new LorId(ModPack21341Init.PackageId, 922),
            new LorId(ModPack21341Init.PackageId, 923),
            new LorId(ModPack21341Init.PackageId, 924),
            new LorId(ModPack21341Init.PackageId, 925),
            new LorId(ModPack21341Init.PackageId, 926)
        };

        private bool _bufRemoved;
        private bool _cardUsed;
        private int _egoCard;
        private bool _phase2Activated;

        public override void OnWaveStart()
        {
            InitAngelaPhase();
        }

        private void InitAngelaPhase()
        {
            if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count > 1)
            {
                _bufRemoved = false;
                _phase2Activated = false;
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init2());
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init1());
            }
            else
            {
                _bufRemoved = true;
                _phase2Activated = true;
            }
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (_egoCards.Contains(curCard.card.GetID()))
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
        }

        public override void OnRoundEndTheLast()
        {
            if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count < 2 && !_bufRemoved) ChangeAngelaPhase();
            if (_phase2Activated)
                owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            _cardUsed = false;
        }

        private void ChangeAngelaPhase()
        {
            _bufRemoved = true;
            _phase2Activated = true;
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ModPack21341Init2));
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ModPack21341Init1));
        }

        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            ChooseEgoCard(ref origin);
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        private void ChooseEgoCard(ref BattleDiceCardModel origin)
        {
            if (!_phase2Activated || _cardUsed) return;
            _egoCard = RandomUtil.SelectOne(_egoCards.Select(x => x.id).ToList());
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, _egoCard)));
            _cardUsed = true;
        }
    }
}