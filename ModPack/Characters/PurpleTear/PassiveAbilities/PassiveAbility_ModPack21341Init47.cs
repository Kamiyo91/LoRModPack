using ModPack21341.Characters.PurpleTear.Buffs;
using ModPack21341.Harmony;
using Sound;

namespace ModPack21341.Characters.PurpleTear.PassiveAbilities
{
    //CustomPTSkinStance
    public class PassiveAbility_ModPack21341Init47 : PassiveAbilityBase
    {
        public PurpleStance CurrentStance { get; private set; }

        public override void OnWaveStart()
        {
            InitPurple();
        }

        public override void OnRoundStart()
        {
            base.OnRoundStart();
            switch (CurrentStance)
            {
                case PurpleStance.Slash:
                    owner.UnitData.historyInWave.purpleTearForm_Sla++;
                    return;
                case PurpleStance.Penetrate:
                    owner.UnitData.historyInWave.purpleTearForm_Pen++;
                    return;
                case PurpleStance.Hit:
                    owner.UnitData.historyInWave.purpleTearForm_Hit++;
                    return;
                case PurpleStance.Defense:
                    owner.UnitData.historyInWave.purpleTearForm_Def++;
                    return;
                default:
                    return;
            }
        }

        public override void OnUnitCreated()
        {
            InitPurple();
        }

        private void InitPurple()
        {
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 914));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 915));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 916));
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 917));
            switch (RandomUtil.Range(0, 3))
            {
                case 0:
                    ChangeStance_slash();
                    return;
                case 1:
                    ChangeStance_penetrate();
                    return;
                case 2:
                    ChangeStance_hit();
                    return;
                case 3:
                    ChangeStance_defense();
                    return;
                default:
                    return;
            }
        }

        private void RemoveAllStanceBuf()
        {
            owner.bufListDetail.RemoveBufAll(KeywordBuf.PurpleSlash);
            owner.bufListDetail.RemoveBufAll(KeywordBuf.PurplePenetrate);
            owner.bufListDetail.RemoveBufAll(KeywordBuf.PurpleHit);
            owner.bufListDetail.RemoveBufAll(KeywordBuf.PurpleDefense);
        }

        public void ChangeStance_slash()
        {
            owner.UnitData.historyInWave.purpleTearForm_Sla++;
            RemoveAllStanceBuf();
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init23());
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
            {
                owner.view.SetAltSkin(owner.UnitData.unitData.appearanceType == Gender.F
                    ? "ThePurpleTearJ_F"
                    : "ThePurpleTearJ");
                owner.view.StartEgoSkinChangeEffect("Character");
            }

            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Purple_Change");
            CurrentStance = PurpleStance.Slash;
            if (owner.faction != Faction.Player) return;
            owner.view.speedDiceSetterUI.DeselectAll();
            var count = owner.allyCardDetail.GetHand().Count;
            var deckForBattle = owner.UnitData.unitData.GetDeckForBattle();
            owner.ChangeBaseDeck(deckForBattle, count);
        }

        public void ChangeStance_penetrate()
        {
            owner.UnitData.historyInWave.purpleTearForm_Pen++;
            RemoveAllStanceBuf();
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init24());
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
            {
                owner.view.SetAltSkin(owner.UnitData.unitData.appearanceType == Gender.F
                    ? "ThePurpleTearZ_F"
                    : "ThePurpleTearZ");
                owner.view.StartEgoSkinChangeEffect("Character");
            }

            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Purple_Change");
            CurrentStance = PurpleStance.Penetrate;
            if (owner.faction != Faction.Player) return;
            owner.view.speedDiceSetterUI.DeselectAll();
            var count = owner.allyCardDetail.GetHand().Count;
            var deckForBattle = owner.UnitData.unitData.GetDeckForBattle(1);
            owner.ChangeBaseDeck(deckForBattle, count);
        }

        public void ChangeStance_hit()
        {
            owner.UnitData.historyInWave.purpleTearForm_Hit++;
            RemoveAllStanceBuf();
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init22());
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
            {
                owner.view.SetAltSkin(owner.UnitData.unitData.appearanceType == Gender.F
                    ? "ThePurpleTearH_F"
                    : "ThePurpleTearH");
                owner.view.StartEgoSkinChangeEffect("Character");
            }

            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Purple_Change");
            CurrentStance = PurpleStance.Hit;
            if (owner.faction != Faction.Player) return;
            owner.view.speedDiceSetterUI.DeselectAll();
            var count = owner.allyCardDetail.GetHand().Count;
            var deckForBattle = owner.UnitData.unitData.GetDeckForBattle(2);
            owner.ChangeBaseDeck(deckForBattle, count);
        }

        public void ChangeStance_defense()
        {
            owner.UnitData.historyInWave.purpleTearForm_Def++;
            RemoveAllStanceBuf();
            owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init21());
            if (string.IsNullOrEmpty(owner.UnitData.unitData.workshopSkin) &&
                owner.UnitData.unitData.bookItem == owner.UnitData.unitData.CustomBookItem)
            {
                owner.view.SetAltSkin(owner.UnitData.unitData.appearanceType == Gender.F
                    ? "ThePurpleTearG_F"
                    : "ThePurpleTearG");
                owner.view.StartEgoSkinChangeEffect("Character");
            }

            SingletonBehavior<SoundEffectManager>.Instance.PlayClip("Battle/Purple_Change");
            CurrentStance = PurpleStance.Defense;
            if (owner.faction != Faction.Player) return;
            owner.view.speedDiceSetterUI.DeselectAll();
            var count = owner.allyCardDetail.GetHand().Count;
            var deckForBattle = owner.UnitData.unitData.GetDeckForBattle(3);
            owner.ChangeBaseDeck(deckForBattle, count);
        }
    }
}