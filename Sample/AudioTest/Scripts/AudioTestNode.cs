using Cysharp.Threading.Tasks;
using Dories.Audio.Runtime;
using GameFramework.Runtime.GameBase;
using UnityEngine;

namespace GameFramework.Test.AudioTest.Scripts
{
    public class AudioTestNode : MonoBehaviour
    {
        // 收集规则 AddressByFileName，地址即音频文件名
        private const string BgmA = "BgmA";
        private const string BgmB = "BgmB";
        private const string Click = AudioTestProcedure.Click;

        private GUIStyle _buttonStyle;
        private GUIStyle _labelStyle;

        private int _loopSoundId;

        private async UniTaskVoid StartLoop()
        {
            if (_loopSoundId == 0)
                _loopSoundId = await GameEntry.Audio.PlaySoundAsync(Click, loop: true);
        }

        private void OnGUI()
        {
            if (GameEntry.Procedure?.CurrentProcedure is not AudioTestProcedure) return;

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 24 };
                _labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 24 };
            }

            if (GUI.Button(new Rect(10, 10, 200, 80), "Play BGM A", _buttonStyle))
            {
                GameEntry.Audio.PlayMusicAsync(BgmA).Forget();
            }

            if (GUI.Button(new Rect(10, 100, 200, 80), "Play BGM B", _buttonStyle))
            {
                GameEntry.Audio.PlayMusicAsync(BgmB).Forget();
            }

            if (GUI.Button(new Rect(10, 190, 200, 80), "Stop Music", _buttonStyle))
            {
                GameEntry.Audio.StopMusic();
            }

            if (GUI.Button(new Rect(220, 10, 200, 80), "Play Click", _buttonStyle))
            {
                GameEntry.Audio.PlaySoundAsync(Click).Forget();
            }

            if (GUI.Button(new Rect(220, 100, 200, 80), "Click 3D", _buttonStyle))
            {
                GameEntry.Audio.PlaySoundAtAsync(Click, Random.insideUnitSphere * 10f).Forget();
            }

            if (GUI.Button(new Rect(220, 190, 200, 80), _loopSoundId == 0 ? "Start Loop" : "Stop Loop", _buttonStyle))
            {
                if (_loopSoundId == 0)
                {
                    StartLoop().Forget();
                }
                else
                {
                    GameEntry.Audio.StopSound(_loopSoundId);
                    _loopSoundId = 0;
                }
            }

            DrawChannel(AudioChannel.Music, 300);
            DrawChannel(AudioChannel.Sound, 360);

            GUI.Label(new Rect(10, 420, 600, 40),
                $"Music: {GameEntry.Audio.CurrentMusic ?? "-"}  Sounds: {GameEntry.Audio.ActiveSoundCount}", _labelStyle);
        }

        private void DrawChannel(AudioChannel channel, float y)
        {
            GUI.Label(new Rect(10, y, 100, 40), channel.ToString(), _labelStyle);
            float volume = GUI.HorizontalSlider(new Rect(120, y + 15, 200, 30), GameEntry.Audio.GetVolume(channel), 0f, 1f);
            if (!Mathf.Approximately(volume, GameEntry.Audio.GetVolume(channel)))
                GameEntry.Audio.SetVolume(channel, volume);

            bool muted = GUI.Toggle(new Rect(340, y, 120, 40), GameEntry.Audio.IsMuted(channel), "Mute", _buttonStyle);
            if (muted != GameEntry.Audio.IsMuted(channel))
                GameEntry.Audio.SetMute(channel, muted);
        }
    }
}
