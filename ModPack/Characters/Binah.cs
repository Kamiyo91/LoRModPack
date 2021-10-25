using System.Linq;
using ModPack21341.Utilities;

namespace ModPack21341.Characters
{
    public class PassiveAbility_FragmentOfAnArbiterExtraBinah : PassiveAbilityBase
    {
        public override void OnWaveStart()
        {
            CheckBinahUnitAndFragmentStatus();
            OnRoundEndTheLast();
        }

        private void CheckBinahUnitAndFragmentStatus()
        {
            if (!owner.passiveDetail.HasPassive<PassiveAbility_BinahUnit>() && !owner.passiveDetail.HasPassive<PassiveAbility_10010>())
            {
                owner.passiveDetail.DestroyPassive(this);
            }
            if (owner.emotionDetail.EmotionLevel < FragmentEmoLevel)
            {
                _fragmentActivated = 0;
            }
            if (_fragmentActivated == 2)
            {
                _fragmentActivated = 1;
            }
        }
        private void ActiveFragment()
        {
            var passive = owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_10011);
            owner.passiveDetail.DestroyPassive(passive);
            owner.passiveDetail.AddPassive(new PassiveAbility_180005());
            var hand = owner.allyCardDetail.GetHand();
            var deck = owner.allyCardDetail.GetDeck();
            deck.AddRange(owner.allyCardDetail.GetDiscarded());
            deck.AddRange(owner.allyCardDetail.GetUse());
            owner.allyCardDetail.ExhaustAllCards();
            foreach (var cardId in hand.Select(battleDiceCardModel => battleDiceCardModel.GetID().id))
            {
                switch (cardId)
                {
                    case 607201:
                        owner.allyCardDetail.AddNewCard(706201);
                        break;
                    case 607202:
                        owner.allyCardDetail.AddNewCard(706202);
                        break;
                    case 607203:
                        owner.allyCardDetail.AddNewCard(706203);
                        break;
                    case 607204:
                        owner.allyCardDetail.AddNewCard(706204);
                        break;
                    case 607205:
                        owner.allyCardDetail.AddNewCard(706205);
                        break;
                    default:
                        owner.allyCardDetail.AddNewCard(cardId);
                        break;
                }
            }
            foreach (var cardId2 in from battleDiceCardModel2 in deck let id2 = battleDiceCardModel2.GetID().id let name2 = battleDiceCardModel2.GetName() select id2)
            {
                switch (cardId2)
                {
                    case 607201:
                        owner.allyCardDetail.AddNewCardToDeck(706201);
                        break;
                    case 607202:
                        owner.allyCardDetail.AddNewCardToDeck(706202);
                        break;
                    case 607203:
                        owner.allyCardDetail.AddNewCardToDeck(706203);
                        break;
                    case 607204:
                        owner.allyCardDetail.AddNewCardToDeck(706204);
                        break;
                    case 607205:
                        owner.allyCardDetail.AddNewCardToDeck(706205);
                        break;
                    default:
                        owner.allyCardDetail.AddNewCard(cardId2);
                        break;
                }
            }
            Hide();
            if (_fragmentActivated == 0)
            {
                owner.RecoverHP(FragmentHp);
            }
            owner.breakDetail.RecoverBreak(FragmentBp);
            _fragmentActivated = 2;
        }
        public override void OnRoundEndTheLast()
        {
            if (_fragmentActivated >= 2 || owner.emotionDetail.EmotionLevel < FragmentEmoLevel) return;
            ActiveFragment();
        }

        private const int FragmentEmoLevel = 3;
        private const int FragmentHp = 15;
        private const int FragmentBp = 15;
        private byte _fragmentActivated;
    }

    public class PassiveAbility_BinahUnit : PassiveAbilityBase
    {
        private BattleDialogueModel _dlg;
        private string _originalSkinName;
        public override void OnWaveStart()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                UnitUtilities.PrepareSephirahSkin(owner, 8, "Binah", owner.faction == Faction.Enemy, ref _originalSkinName, ref _dlg, true);
        }
        public override void OnBattleEnd()
        {
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
                UnitUtilities.ReturnToTheOriginalBaseSkin(owner, _originalSkinName, _dlg);
        }
    }
}
