using System.Linq;

namespace ModPack21341.Characters.Kamiyo.PassiveAbilities
{
    //KamiyoDesc
    public class PassiveAbility_ModPack21341Init28 : PassiveAbilityBase
    {
        public override void OnDie()
        {
            BattleObjectManager.instance.GetList(Faction.Enemy).FirstOrDefault(x => x != owner)?.Die();
        }
    }
}