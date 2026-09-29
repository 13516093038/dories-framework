using GameFramework.Runtime.GameBase.GameProfile;
using GameFramework.Runtime.Utils;
using UnityEngine;

namespace GameFramework.Runtime.GameBase
{
    public class GameEntry : MonoBehaviour
    {
        [SerializeField, Range(30, 120)] private float _targetFrameRate = 60f;
        [SerializeField, Range(0f, 10f)] private float _timeScale = 1f;

        [SerializeField] private string _gameProfileInfo;

        public static IGameProfileInfo GameProfileInfo {get; private set;}

        private void Awake()
        {
            GameProfileInfo = TypeUtil.CreateTypeInstance<IGameProfileInfo>(_gameProfileInfo);

            Debug.Log($"GameProfileInfo: {GameProfileInfo.GameVersion}");
        }
    }
}
