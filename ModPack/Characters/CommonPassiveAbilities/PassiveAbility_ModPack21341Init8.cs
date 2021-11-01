using System.Collections.Generic;
using System.Linq;
using ModPack21341.Characters.Roland.PassiveAbilities;
using ModPack21341.Harmony;
using ModPack21341.Utilities;

namespace ModPack21341.Characters.CommonPassiveAbilities
{
    //CheckDeck
    public class PassiveAbility_ModPack21341Init8 : PassiveAbilityBase
    {
        private bool _activated;
        private List<int> _awakenedDeck;
        private bool _awakenedDeckIsActive;
        private List<int> _originalDeck;

        public override void Init(BattleUnitModel self)
        {
            base.Init(self);
            Hide();
            _awakenedDeckIsActive = false;
            _originalDeck = owner.allyCardDetail.GetAllDeck().Select(x => x.GetID().id).ToList();
            _activated = !owner.passiveDetail.PassiveList.Exists(x => x is PassiveAbility_ModPack21341Init50);
        }

        public override void OnRoundStart()
        {
            if (_activated)
                UnitUtilities.DeckVariantActivated(owner);
        }

        public void ChangeDeck()
        {
            owner.view.speedDiceSetterUI.DeselectAll();
            var count = owner.allyCardDetail.GetHand().Count;
            if (_awakenedDeckIsActive)
            {
                UnitUtilities.ChangeDeck(owner, _originalDeck);
                _awakenedDeckIsActive = false;
            }
            else
            {
                UnitUtilities.ChangeDeck(owner, _awakenedDeck);
                _awakenedDeckIsActive = true;
            }

            owner.allyCardDetail.DrawCards(count);
        }

        public void ChangeToTheBlackSilenceMaskDeck()
        {
            owner.view.speedDiceSetterUI.DeselectAll();
            var count = owner.allyCardDetail.GetHand().Count;
            ChangeDeckBlack();
            owner.allyCardDetail.DrawCards(count);
        }

        private void ChangeDeckBlack()
        {
            owner.allyCardDetail.ExhaustAllCards();
            foreach (var cardId in _awakenedDeck.Where(cardId => cardId > 100))
                owner.allyCardDetail.AddNewCardToDeck(cardId);
            foreach (var cardId in _awakenedDeck.Where(cardId => cardId < 100))
                owner.allyCardDetail.AddNewCardToDeck(new LorId(ModPack21341Init.PackageId, cardId));
        }

        public void SaveAwakenedDeck(List<int> awakenedDeck)
        {
            _awakenedDeck = awakenedDeck;
        }
    }
}