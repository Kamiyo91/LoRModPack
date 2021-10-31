using System.Collections.Generic;
using System.Linq;
using LOR_DiceSystem;
using ModPack21341.Characters.Buffs;
using ModPack21341.Characters.CardAbilities;
using ModPack21341.Harmony;
using ModPack21341.Models;
using ModPack21341.StageManager;
using ModPack21341.StageManager.MapManager.OldSamuraiStageMaps;
using ModPack21341.Utilities;

namespace ModPack21341.Characters
{
    public class PassiveAbility_GhostSamurai : PassiveAbilityBase
    {
        private void AddGhostUnitBuffs()
        {
            owner.bufListDetail.AddBuf(new BattleUnitBuf_KeterFinal_LibrarianAura());
            if (owner.faction == Faction.Player)
            {
                owner.bufListDetail.AddBuf(new BattleUnitBuf_SummonedUnitOldSamurai());
            }
        }
        private void CleanGhostUnitBuffs()
        {
            if (owner.bufListDetail.GetActivatedBufList()
                    .Find(x => x is BattleUnitBuf_KeterFinal_LibrarianAura) is BattleUnitBuf_KeterFinal_LibrarianAura
                bufAura)
            {
                bufAura.Destroy();
            }

            owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_KeterFinal_LibrarianAura));
            if (owner.faction == Faction.Player)
                owner.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_SummonedUnitOldSamurai));
        }
        public override void OnRoundStart() => AddGhostUnitBuffs();

        public override void OnDie() => CleanGhostUnitBuffs();
    }
    public class PassiveAbility_OldSamurai : PassiveAbilityBase
    {
        private bool _lethalDamage;
        private bool _oldSamuraiPhaseChangeCheck;
        private bool _summonGhosts;
        private bool _summonGhostUsed;
        private bool _ghostMapRemoved;
        private bool _mapChanged;
        private StageLibraryFloorModel _floor;

        public override void OnRoundStart()
        {
            if (owner.faction == Faction.Player && !_ghostMapRemoved && _summonGhostUsed &&
                BattleObjectManager.instance.GetAliveList(Faction.Player).All(x => x == owner))
            {
                RemoveSamuraiEgoMap();
            }
            if (owner.faction == Faction.Player && _lethalDamage && !owner.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_ReviveCheckBuf))
            {
                owner.bufListDetail.AddBuf(new BattleUnitBuf_ReviveCheckBuf());
            }
        }
        private static void ChangeToSamuraiEgoMap() => MapUtilities.ChangeMap(new MapModel
        {
            Stage = "OldSamurai",
            StageId = 1,
            IsPlayer = true,
            Component = new OldSamuraiPlayerMapManager(),
            Bgy = 0.2f
        });
        private void RemoveSamuraiEgoMap()
        {
            _ghostMapRemoved = true;
            MapUtilities.RemoveValueInEgoMap("OldSamurai");
            MapUtilities.ReturnFromEgoMap("OldSamurai", owner, 1);
            SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(SingletonBehavior<BattleSceneRoot>
                .Instance.currentMapObject.mapBgm);
            SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
        }
        public void PhaseChanged() => _oldSamuraiPhaseChangeCheck = true;

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
            var indexList = new List<int>();
            foreach (var (battleUnit, num) in BattleObjectManager.instance.GetList(Faction.Player)
                .Select((value, i) => (value, i)))
            {
                if (num == owner.index) continue;
                indexList.Add(num);
                if (indexList.Count > 2) break;
            }
            foreach (var unit in BattleObjectManager.instance.GetList(Faction.Player).Where(x => indexList.Contains(x.index)))
            {
                BattleObjectManager.instance.UnregisterUnit(unit);
            }
            for (var i = 0; i < 3; i++)
            {
                UnitUtilities.AddNewUnitPlayerSide(_floor, new UnitModel
                {
                    Name = "Samurai's Ghost",
                    Pos = indexList[i],
                    LockedEmotion = true,
                    Sephirah = _floor.Sephirah
                });
            }
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
        private void ReturnToTheOriginalPlayerTeam() => UnitUtilities.RemoveUnitData(_floor, "Samurai's Ghost");
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
            ChangeToSamuraiEgoMap();
        }

        public override void OnRoundEndTheLast_ignoreDead()
        {
            if (_lethalDamage || !owner.IsDead() || owner.faction == Faction.Enemy) return;
            Revive();
            UnitUtilities.ActiveAwakeningDeckPassive(owner,"OldSamurai");
            owner.allyCardDetail.DrawCards(4);
        }

        private void SetStageFinish()
        {
            foreach (var battleUnitModel in BattleObjectManager.instance.GetAliveList(owner.faction)
                .Where(x => x != owner))
            {
                battleUnitModel.Die();
            }

            var enemyStageManager = Singleton<StageController>.Instance.EnemyStageManager;
            if (enemyStageManager is EnemyTeamStageManager_OldSamurai oldSamurai)
            {
                oldSamurai.SetFinishable();
            }

        }

        private void KillAllGhostOnPlayerDeath()
        {
            foreach (var battleUnitModel in BattleObjectManager.instance.GetAliveList(owner.faction)
                .Where(x => x != owner))
            {
                battleUnitModel.bufListDetail.RemoveBufAll(typeof(BattleUnitBuf_SummonedUnitOldSamurai));
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

    public class PassiveAbility_StendyBreathing : PassiveAbilityBase
    {
        private int _damage;

        public override float DmgFactor(int dmg, DamageType type = DamageType.ETC, KeywordBuf keyword = KeywordBuf.None)
        {
            if (type == DamageType.Attack)
                _damage += dmg;
            return base.DmgFactor(dmg, type, keyword);
        }

        private void RecoverAndResetCount()
        {
            if (_damage == 0)
            {
                owner.RecoverHP(10);
                owner.breakDetail.RecoverBreak(10);
            }

            _damage = 0;
        }

        public override void OnRoundEnd() => RecoverAndResetCount();
    }

    public class PassiveAbility_ControlledBreathing : PassiveAbilityBase
    {
        private int _count;
        private int _enemyCount = 3;
        private bool _lightUse;

        private void AddDeepBreathingCard()
        {
            _ = owner.personalEgoDetail.GetHand().Exists(x => x.GetID() == new LorId(ModPack21341Init.PackageId, 900))
                ? _count = 0
                : _count++;
            if (_count != 3) return;
            owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 900));
            _count = 0;

        }

        private void RemoveDeepBreathingBuff()
        {
            if (owner.passiveDetail.PassiveList.Find(x => x is PassiveAbility_DeepBreath) is PassiveAbility_DeepBreath
                passiveAbilityDeepBreath)
            {
                owner.passiveDetail.DestroyPassive(passiveAbilityDeepBreath);
            }
        }
        public override void OnRoundEnd()
        {
            AddDeepBreathingCard();
            RemoveDeepBreathingBuff();
        }
        public override void OnRoundStart()
        {
            if (!_lightUse) return;
            _lightUse = false;
            owner.cardSlotDetail.SpendCost(2);
        }

        public override void OnRoundEndTheLast()
        {
            if (owner.faction != Faction.Enemy && !owner.bufListDetail.GetActivatedBufList()
                .Exists(x => x is BattleUnitBuf_SummonedUnitOldSamurai))
            {
                return;
            }

            _enemyCount++;
            if (owner.RollSpeedDice().FindAll(x => !x.breaked).Count <= 0 || owner.IsBreakLifeZero())
            {
                return;
            }

            if (_enemyCount <= 2) return;
            if (owner.cardSlotDetail.PlayPoint < 2) return;
            UseDeepBreathingCardNpc();
        }

        private void UseDeepBreathingCardNpc()
        {
            _lightUse = true;
            _enemyCount = 0;
            owner.personalEgoDetail.RemoveCard(new LorId(ModPack21341Init.PackageId, 900));
            DiceCardSelfAbility_DeepBreathing.Activate(owner);
        }
        public override void OnWaveStart() => owner.personalEgoDetail.AddCard(new LorId(ModPack21341Init.PackageId, 900));


        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            owner.currentDiceAction.ignorePower = true;
            EmotionDiceRoll(behavior);
            AtkDiceRoll(behavior);
            DefDiceRoll(behavior);
            PowernullDiceRoll(behavior);
        }

        private void EmotionDiceRoll(BattleDiceBehavior behavior)
        {
            var diceMaxRollAdder = 0;
            if (owner.emotionDetail.EmotionLevel > 2)
                diceMaxRollAdder = 1;
            behavior.ApplyDiceStatBonus(new DiceStatBonus { min = 1, max = diceMaxRollAdder });
        }

        private void AtkDiceRoll(BattleDiceBehavior behavior)
        {
            var negativeNum = 0;
            var positiveNum = 0;
            if (behavior.Detail == BehaviourDetail.Slash || behavior.Detail == BehaviourDetail.Hit || behavior.Detail == BehaviourDetail.Penetrate)
            {
                negativeNum = owner.bufListDetail.GetKewordBufStack(KeywordBuf.Weak);
                positiveNum = owner.bufListDetail.GetKewordBufStack(KeywordBuf.Strength);
                if (positiveNum > 0)
                    positiveNum /= 3;
            }
            var diceMinRollAdder = positiveNum - negativeNum;
            if (diceMinRollAdder < 0 && behavior.GetDiceVanillaMin() - diceMinRollAdder < 1)
                diceMinRollAdder = behavior.GetDiceVanillaMin() * -1 + 1;
            behavior.ApplyDiceStatBonus(new DiceStatBonus { min = diceMinRollAdder, max = positiveNum });
        }

        private void DefDiceRoll(BattleDiceBehavior behavior)
        {
            var negativeNum = 0;
            var positiveNum = 0;
            if (behavior.Detail == BehaviourDetail.Guard || behavior.Detail == BehaviourDetail.Evasion)
            {
                negativeNum = owner.bufListDetail.GetKewordBufStack(KeywordBuf.Disarm);
                positiveNum = owner.bufListDetail.GetKewordBufStack(KeywordBuf.Endurance);
                if (positiveNum > 0)
                    positiveNum /= 3;
            }
            var diceMinRollAdder = positiveNum - negativeNum;
            if (diceMinRollAdder < 0 && behavior.GetDiceVanillaMin() - diceMinRollAdder < 1)
                diceMinRollAdder = behavior.GetDiceVanillaMin() * -1 + 1;
            behavior.ApplyDiceStatBonus(new DiceStatBonus { min = diceMinRollAdder, max = positiveNum });
        }

        private void PowernullDiceRoll(BattleDiceBehavior behavior)
        {
            if (!owner.bufListDetail.GetActivatedBufList().Exists(x => x.bufType == KeywordBuf.NullifyPower)) return;
            behavior.ApplyDiceStatBonus(new DiceStatBonus { min = 1, max = 1 });
        }
    }

    public class PassiveAbility_DeepBreath : PassiveAbilityBase
    {
        public override void BeforeRollDice(BattleDiceBehavior behavior)
        {
            UnitUtilities.SetPassiveCombatLog(this, owner);
            behavior.ApplyDiceStatBonus(
                new DiceStatBonus
                {
                    max = 3
                });
        }
    }

    public class PassiveAbility_ZeroBlade : PassiveAbilityBase
    {
        private bool _counterReload;
        public override void OnStartBattle() => UnitUtilities.ReadyCounterCard(owner, 2);
        public override void OnLoseParrying(BattleDiceBehavior behavior) => _counterReload = false;
        public override void OnDrawParrying(BattleDiceBehavior behavior) => _counterReload = false;

        public override void OnWinParrying(BattleDiceBehavior behavior) =>
            _counterReload = behavior.abilityList.Exists(x => x is DiceCardAbility_ZeroBlade);

        public override void OnEndBattle(BattlePlayingCardDataInUnitModel curCard)
        {
            if (!_counterReload) return;
            _counterReload = false;
            UnitUtilities.SetPassiveCombatLog(this, owner);
            UnitUtilities.ReadyCounterCard(owner, 2);
        }
    }
}
