namespace GameFramework.Runtime.GameBase.GameProfile
{
    public interface IGameProfileInfo
    {
        /// <summary>
        /// 游戏版本
        /// </summary>
        public string GameVersion { get; }

        /// <summary>
        /// 游戏Build版本
        /// </summary>
        public string BuildVersion { get; }

        /// <summary>
        /// 平台
        /// </summary>
        public string Platform { get; }

        /// <summary>
        /// 子平台
        /// </summary>
        public string SubPlatform { get; }
    }
}
