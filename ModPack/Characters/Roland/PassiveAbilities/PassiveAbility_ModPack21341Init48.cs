using System.Collections.Generic;
using System.Linq;
using ModPack21341.Harmony;

namespace ModPack21341.Characters.Roland.PassiveAbilities
{
    //Orlando
    public class PassiveAbility_ModPack21341Init48 : PassiveAbilityBase
    {
        private readonly List<LorId> _usedCount = new List<LorId>();

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            var cardId = curCard.card.GetID();
            if (cardId.IsWorkshop()) return;
            var id2 = cardId.id;
            if (!_usedCount.Contains(cardId) && (id2 >= 702001 && id2 <= 702009 || id2 >= 706101 && id2 <= 706109))
                _usedCount.Add(cardId);
        }

        public override void OnWaveStart()
        {
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 913));
        }

        public override void OnRoundStart()
        {
            foreach (var battleDiceCardModel in owner.allyCardDetail.GetAllDeck())
                battleDiceCardModel.RemoveBuf<PassiveAbility_10012.BattleDiceCardBuf_blackSilenceEgoCount>();
            foreach (var battleDiceCardModel2 in from battleDiceCardModel2 in owner.allyCardDetail.GetAllDeck()
                let id = battleDiceCardModel2.GetID()
                where !id.IsWorkshop()
                let id2 = id.id
                where !_usedCount.Contains(id) && (id2 >= 702001 && id2 <= 702009 || id2 >= 706101 && id2 <= 706109)
                select battleDiceCardModel2)
                battleDiceCardModel2.AddBuf(new PassiveAbility_10012.BattleDiceCardBuf_blackSilenceEgoCount());
            owner.bufListDetail.RemoveBufAll(typeof(PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount));
            owner.bufListDetail.AddBuf(new PassiveAbility_10012.BattleUnitBuf_blackSilenceSpecialCount
            {
                stack = _usedCount.Count
            });
        }

        public bool IsActivatedSpecialCard()
        {
            return _usedCount.Count >= 9;
        }

        public void ResetUsedCount()
        {
            _usedCount.Clear();
        }
    }
}