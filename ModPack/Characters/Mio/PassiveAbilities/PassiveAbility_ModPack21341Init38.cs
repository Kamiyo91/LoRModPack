using System.Linq;
using ModPack21341.Harmony;

namespace ModPack21341.Characters.Mio.PassiveAbilities
{
    //MioEnemyDesc
    public class PassiveAbility_ModPack21341Init38 : PassiveAbilityBase
    {
        private bool _awakened;
        private int _count = 4;

        public override void OnRoundEndTheLast_ignoreDead()
        {
            CheckMassAttackCard();
        }


        private void CheckMassAttackCard()
        {
            if (_awakened)
                _ = owner.allyCardDetail.GetHand().Exists(x => x.GetID() == new LorId(ModPack21341Init.PackageId, 25))
                    ? _count = 4
                    : _count++;
        }

        public void SetAwakened(bool status)
        {
            _awakened = status;
        }

        public void SetCountValue(int value)
        {
            _count = value;
        }

        public override void OnWaveStart()
        {
            _awakened = false;
        }

        public override BattleDiceCardModel OnSelectCardAuto(BattleDiceCardModel origin, int currentDiceSlotIdx)
        {
            PutMassAttackCardOnDice(ref origin);
            return base.OnSelectCardAuto(origin, currentDiceSlotIdx);
        }

        private void PutMassAttackCardOnDice(ref BattleDiceCardModel origin)
        {
            if (!_awakened || _count < 4 || owner.IsBreakLifeZero() || !BattleObjectManager.instance
                .GetAliveList(Faction.Player).Any(x => x.bufListDetail.IsTargetable())) return;
            _count = 0;
            origin = BattleDiceCardModel.CreatePlayingCard(
                ItemXmlDataList.instance.GetCardItem(new LorId(ModPack21341Init.PackageId, 25)));
        }

        public override void OnLevelUpEmotion()
        {
            AddAttackEgoCard();
        }

        private void AddAttackEgoCard()
        {
            if (owner.emotionDetail.EmotionLevel == 5)
                owner.allyCardDetail.AddNewCard(new LorId(ModPack21341Init.PackageId, 19));
        }
    }
}