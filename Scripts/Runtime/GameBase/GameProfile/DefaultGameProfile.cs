using UnityEngine;

namespace GameFramework.Runtime.GameBase.GameProfile
{
    public class DefaultGameProfile : IGameProfileInfo
    {
        public string GameVersion => Application.version;
        public string BuildVersion => Application.buildGUID;
        public string Platform => Application.platform.ToString();
        public string SubPlatform => Application.platform.ToString();
    }
}