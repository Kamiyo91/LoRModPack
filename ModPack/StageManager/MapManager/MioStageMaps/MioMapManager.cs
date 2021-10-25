namespace ModPack21341.StageManager.MapManager.MioStageMaps
{
    public class MioMapManager : global::MapManager
    {
        public override void EnableMap(bool b)
        {
            base.EnableMap(b);
            gameObject.SetActive(b);
        }
    }
}
