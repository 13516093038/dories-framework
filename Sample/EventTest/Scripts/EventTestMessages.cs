namespace GameFramework.Test.EventTest.Scripts
{
    // 事件类型由项目自己定义，建议用 struct 避免分配

    public struct GoldChanged
    {
        public int Gold;
    }

    public struct PlayerHit
    {
        public int Damage;
    }
}
