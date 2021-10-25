namespace ModPack21341.StageManager.MapManager.OldSamuraiStageMaps
{
    public class OldSamuraiMapManager : global::MapManager
    {
        public override void EnableMap(bool b)
        {
            base.EnableMap(b);
            gameObject.SetActive(b);
        }
    }
}
