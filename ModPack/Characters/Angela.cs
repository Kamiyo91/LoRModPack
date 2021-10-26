using System.Collections.Generic;
using System.Linq;
using ModPack21341.Characters.Buffs;
using ModPack21341.Harmony;
using ModPack21341.Utilities;

namespace ModPack21341.Characters
{
    public class PassiveAbility_AngelaEnemyUnit : PassiveAbilityBase
    {
        private bool _bufRemoved;
        private bool _phase2Activated;
        private bool _cardUsed;
        private int _egoCard;

        private readonly List<int> _egoCards = new List<int>
            {30, 31, 32, 33, 34, 35, 36, 37, 38};

        public override void OnWaveStart() => InitAngelaPhase();

        private void InitAngelaPhase()
        {
            if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count > 1)
            {
                _bufRemoved = false;
                _phase2Activated = false;
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_UntargetableBuf());
                owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ImmortalBuff());
            }
            else
            {
                _bufRemoved = true;
                _phase2Activated = true;
            }
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if(_egoCards.Contains(curCard.card.GetID().id))
                owner.allyCardDetail.ExhaustACardAnywhere(curCard.card);
        }

        public override void OnRoundEndTheLast()
        {

            if (BattleObjectManager.instance.GetAliveList(Faction.Enemy).Count < 2 && !_bufRemoved)
            {
                ChangeAngelaPhase();
            }
            if (_phase2Activated)
                owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
            _cardUsed = false;
        }

        private void ChangeAngelaPhase()
        {
            _bufRemoved = true;
            _phase2Activated = true;
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_UntargetableBuf));
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ImmortalBuff));
        }
        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            ChooseEgoCard(ref origin);
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        private void ChooseEgoCard(ref BattleDiceCardModel origin)
        {
            if (!_phase2Activated || _cardUsed) return;
            _egoCard = RandomUtil.SelectOne(_egoCards);
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, _egoCard)));
            _cardUsed = true;
        }
    }
    public class PassiveAbility_AngelaUnit : PassiveAbilityBase
    {
        private BattleDialogueModel _dlg;
        private string _originalSkinName;
        public override void OnWaveStart()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
            {
                UnitUtilities.PrepareSephirahSkin(owner, 21, "Angela",owner.faction == Faction.Enemy, ref _originalSkinName, ref _dlg);
            }
            AddCardsWaveStart();
        }
        private void AddCardsWaveStart()
        {
            if (owner.emotionDetail.EmotionLevel == 3)
            {
                owner.personalEgoDetail.AddCard(9910011);
                owner.personalEgoDetail.AddCard(9910012);
                owner.personalEgoDetail.AddCard(9910013);
            }

            if (owner.emotionDetail.EmotionLevel == 4)
            {
                owner.personalEgoDetail.AddCard(9910011);
                owner.personalEgoDetail.AddCard(9910012);
                owner.personalEgoDetail.AddCard(9910013);
                owner.personalEgoDetail.AddCard(9910014);
                owner.personalEgoDetail.AddCard(9910015);
                owner.personalEgoDetail.AddCard(9910016);
            }

            if (owner.emotionDetail.EmotionLevel != 5) return;
            owner.personalEgoDetail.AddCard(9910011);
            owner.personalEgoDetail.AddCard(9910012);
            owner.personalEgoDetail.AddCard(9910013);
            owner.personalEgoDetail.AddCard(9910014);
            owner.personalEgoDetail.AddCard(9910015);
            owner.personalEgoDetail.AddCard(9910016);
            owner.personalEgoDetail.AddCard(9910017);
            owner.personalEgoDetail.AddCard(9910018);
            owner.personalEgoDetail.AddCard(9910019);
        }

        private void AddCardOnLvUpEmotion()
        {
            if (owner.emotionDetail.EmotionLevel == 3)
            {
                owner.personalEgoDetail.AddCard(9910011);
                owner.personalEgoDetail.AddCard(9910012);
                owner.personalEgoDetail.AddCard(9910013);
            }
            if (owner.emotionDetail.EmotionLevel == 4)
            {
                owner.personalEgoDetail.AddCard(9910014);
                owner.personalEgoDetail.AddCard(9910015);
                owner.personalEgoDetail.AddCard(9910016);
            }

            if (owner.emotionDetail.EmotionLevel != 5) return;
            owner.personalEgoDetail.AddCard(9910017);
            owner.personalEgoDetail.AddCard(9910018);
            owner.personalEgoDetail.AddCard(9910019);
        }

        public override void OnLevelUpEmotion() => AddCardOnLvUpEmotion();
        public override void OnBattleEnd()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                UnitUtilities.ReturnToTheOriginalBaseSkin(owner,_originalSkinName,_dlg);
        }
    }
    public class PassiveAbility_AngelaRegen : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            owner.allyCardDetail.DrawCards(2);
        }
        public override void OnRoundStart()
        {
            owner.cardSlotDetail.RecoverPlayPoint(1);
        }
        public override void OnDrawCard()
        {
            owner.allyCardDetail.DrawCards(1);
        }
    }
    public class PassiveAbility_DoubleEmotion : PassiveAbilityBase
    {
        public override void OnDie() => RemoveBuffAndAuraToAll();

        private void RemoveBuffAndAuraToAll()
        {
            foreach (var battleUnitModel in BattleObjectManager.instance.GetAliveList(owner.faction).Where(x => x != owner))
            {
                if (battleUnitModel.bufListDetail.GetActivatedBufList()
                    .Find(x => x is BattleUnitBuf_KeterFinal_LibrarianAura) is BattleUnitBuf_KeterFinal_LibrarianAura bufAura)
                {
                    bufAura.Destroy();
                }
                battleUnitModel.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_KeterFinal_DoubleEmotion));
                battleUnitModel.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_KeterFinal_LibrarianAura));
            }
        }
        public override void OnRoundStart() => GiveBuf();
        
        private void GiveBuf()
        {
            foreach (var battleUnitModel in BattleObjectManager.instance.GetAliveList(owner.faction).Where(battleUnitModel => battleUnitModel.bufListDetail.GetActivatedBuf(KeywordBuf.KeterFinal_DoubleEmotion) == null))
            {
                battleUnitModel.bufListDetail.AddBuf(new BattleUnitBuf_KeterFinal_DoubleEmotion());
                battleUnitModel.bufListDetail.AddBuf(new BattleUnitBuf_KeterFinal_LibrarianAura());
            }
        }
    }
}
