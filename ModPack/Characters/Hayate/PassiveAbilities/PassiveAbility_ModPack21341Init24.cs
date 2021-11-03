using ModPack21341.Harmony;

namespace ModPack21341.Characters.Hayate.PassiveAbilities
{
    //HayateShimmering
    public class PassiveAbility_ModPack21341Init24 : PassiveAbilityBase
    {
        public override void OnRoundStart()
        {
            SetCards();
        }

        public override int SpeedDiceNumAdder()
        {
            return 4;
        }

        private void SetCards()
        {
            owner.allyCardDetail.ExhaustAllCards();
            AddNewCard(new LorId(ModPack21341Init.PackageId, 49));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 53));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 56));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 48));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 52));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 52));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 52));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 51));
            AddNewCard(new LorId(ModPack21341Init.PackageId, 50));
        }

        private void AddNewCard(LorId id)
        {
            var card = owner.allyCardDetail.AddTempCard(id);
            card?.SetCostToZero();
        }
    }
}