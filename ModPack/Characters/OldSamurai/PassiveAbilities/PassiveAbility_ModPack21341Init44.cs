using System.Linq;
using ModPack21341.Characters.OldSamurai.Buffs;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.StageManager;
using ModPack21341.StageManager.MapManager.OldSamuraiStageMaps;
using ModPack21341.Utilities;
using ModPack21341.Utilities.CustomMapUtility.Assemblies;

namespace ModPack21341.Characters.OldSamurai.PassiveAbilities
{
    //OldSamurai
    public class PassiveAbility_ModPack21341Init44 : PassiveAbilityBase
    {
        private StageLibraryFloorModel _floor;
        private bool _ghostMapRemoved;
        private bool _lethalDamage;
        private bool _mapChanged;
        private bool _oldSamuraiPhaseChangeCheck;
        private bool _summonGhosts;
        private bool _summonGhostUsed;

        public override void OnRoundStart()
        {
            if (owner.faction == Faction.Player && !_ghostMapRemoved && _summonGhostUsed &&
                BattleObjectManager.instance.GetAliveList(Faction.Player).All(x => x == owner))
                RemoveSamuraiEgoMap();
            if (owner.faction == Faction.Player && _lethalDamage && !owner.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_ModPack21341Init19))
                owner.bufListDetail.AddBuf(new BattleUnitBuf_ModPack21341Init19());
        }

        private static void ChangeToSamuraiEgoMap()
        {
            MapUtilities.ChangeMap(new MapModel
            {
                Stage = "OldSamurai",
                StageId = 1,
                IsPlayer = true,
                Component = new ModPack21341InitOldSamuraiPlayerMapManager(),
                Bgy = 0.2f
            });
        }

        private void RemoveSamuraiEgoMap()
        {
            _ghostMapRemoved = true;
            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ModPack21341Init18));
            MapUtilities.RemoveValueInEgoMap("OldSamurai");
            MapUtilities.ReturnFromEgoMap("OldSamurai", owner, 1);
            SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(SingletonBehavior<BattleSceneRoot>
                .Instance.currentMapObject.mapBgm);
            SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
        }

        public void PhaseChanged()
        {
            _oldSamuraiPhaseChangeCheck = true;
        }

        public override void OnWaveStart()
        {
            PreLoadBasicVar();
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 902));
        }

        private void PreLoadBasicVar()
        {
            _lethalDamage = false;
            _summonGhostUsed = false;
            var currentStageFloorModel = Singleton<StageController>.Instance.GetCurrentStageFloorModel();
            _floor = Singleton<StageController>.Instance.GetStageModel().GetFloor(currentStageFloorModel.Sephirah);
            UnitUtilities.FillUnitData(new UnitModel
            {
                Id = 10000003,
                Name = "Samurai's Ghost",
                DialogId = 2
            }, _floor);
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (curCard.card.GetID() != new LorId(ModPack21341Init.PackageId, 902)) return;
            PrepareEgo();
        }

        private void PrepareEgo()
        {
            _summonGhosts = true;
            _mapChanged = true;
            owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 902));
        }

        private void SummonSamuraiGhost()
        {
            _summonGhosts = false;
            _summonGhostUsed = true;
            var indexList = UnitUtilities.GetSamuraiGhostIndex(owner.index);
            foreach (var unit in BattleObjectManager.instance.GetList(Faction.Player)
                .Where(x => indexList.Contains(x.index))) BattleObjectManager.instance.UnregisterUnit(unit);
            for (var i = 0; i < 3; i++)
                UnitUtilities.AddNewUnitPlayerSide(_floor, new UnitModel
                {
                    Name = "Samurai's Ghost",
                    Pos = indexList[i],
                    LockedEmotion = true,
                    Sephirah = _floor.Sephirah
                });
            UnitUtilities.RefreshCombatUI();
        }

        public override void OnRoundEnd()
        {
            if (!_summonGhosts || owner.faction == Faction.Enemy) return;
            SummonSamuraiGhost();
        }

        public override void OnBattleEnd()
        {
            if (!_summonGhostUsed) return;
            ReturnToTheOriginalPlayerTeam();
        }

        private void ReturnToTheOriginalPlayerTeam()
        {
            UnitUtilities.RemoveUnitData(_floor, "Samurai's Ghost");
        }

        private void Revive()
        {
            _lethalDamage = true;
            owner.Revive(owner.MaxHp);
            owner.moveDetail.ReturnToFormationByBlink(true);
            owner.breakDetail.ResetGauge();
            owner.breakDetail.RecoverBreakLife(1, true);
            owner.breakDetail.nextTurnBreak = false;
            owner.cardSlotDetail.RecoverPlayPoint(owner.cardSlotDetail.GetMaxPlayPoint());
        }

        public override void OnRoundEndTheLast()
        {
            if (!_mapChanged) return;
            _mapChanged = false;
            owner.bufListDetail.AddBufWithoutDuplication(new BattleUnitBuf_ModPack21341Init18());
            ChangeToSamuraiEgoMap();
            CustomMapHandler.SetEnemyTheme("Hornet.mp3", true);
        }

        public override void OnRoundEndTheLast_ignoreDead()
        {
            if (_lethalDamage || !owner.IsDead() || owner.faction == Faction.Enemy) return;
            Revive();
            UnitUtilities.ActiveAwakeningDeckPassive(owner, "OldSamurai");
            owner.allyCardDetail.DrawCards(4);
        }

        private void SetStageFinish()
        {
            foreach (var battleUnitModel in BattleObjectManager.instance.GetAliveList(owner.faction)
                .Where(x => x != owner))
                battleUnitModel.Die();

            var enemyStageManager = Singleton<StageController>.Instance.EnemyStageManager;
            if (enemyStageManager is EnemyTeamStageManager_ModPack21341Init1 oldSamurai) oldSamurai.SetFinishable();
        }

        private void KillAllGhostOnPlayerDeath()
        {
            foreach (var battleUnitModel in BattleObjectManager.instance.GetAliveList(owner.faction)
                .Where(x => x != owner))
            {
                battleUnitModel.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_ModPack21341Init20));
                battleUnitModel.Die();
            }
        }

        public override void OnDie()
        {
            if (owner.faction == Faction.Enemy && _oldSamuraiPhaseChangeCheck)
                SetStageFinish();
            if (owner.faction == Faction.Player && _summonGhostUsed)
                KillAllGhostOnPlayerDeath();
        }
    }
}