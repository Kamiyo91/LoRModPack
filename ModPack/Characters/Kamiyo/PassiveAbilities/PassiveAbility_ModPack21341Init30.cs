using ModPack21341.Harmony;

namespace ModPack21341.Characters.Kamiyo.PassiveAbilities
{
    //KamiyoShimmering
    public class PassiveAbility_ModPack21341Init30 : PassiveAbilityBase
    {
        public override void OnRoundStartAfter()
        {
            SetCards();
        }

        public override int SpeedDiceNumAdder()
        {
            return 2;
        }

        private void SetCards()
        {
            owner.allyCardDetail.ExhaustAllCards();
            AddNewCard(new LorId(ModPack21341Init.PackageId, 33));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 33));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 34));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 34));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 31));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 31));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 36));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 46));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 32));
        }

        private void AddNewCard(LorId id)
        {
            var card = owner.allyCardDetail.AddTempCard(id);
            card?.SetCostToZero();
        }
    }
}