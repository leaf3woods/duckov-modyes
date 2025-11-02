using Modding.Core.MusicPlayer.Base;
using System.Collections.Generic;
using UnityEngine;

namespace Modding.MusicEarphone.Utilities
{
    public class MusicPlayerInputScanner : MonoBehaviour
    {
        public static MusicPlayerInputScanner Instance { get; private set; } = null!;

        public void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private Dictionary<KeyCode, float> _lastKeyPress = new Dictionary<KeyCode, float>();
        private readonly float ignorance = 0.25f;
        public void Update()
        {
            if (!PluginCore.MusicPlayer.IsPlaying) return;
            var currentTime = Time.time;
            if (Input.GetKeyDown(KeyCode.RightControl) &&
                (!_lastKeyPress.ContainsKey(KeyCode.RightControl) ||
                    (currentTime - _lastKeyPress[KeyCode.RightControl] >= ignorance)))
            {
                PluginCore.MusicPlayer.TogglePause();
                _lastKeyPress[KeyCode.RightControl] = currentTime;
                PluginCore.ShowBubbleOnMainCharacter($"已{(PluginCore.MusicPlayer.IsPasued ? "已暂停" : "恢复")}播放!");
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow) &&
                (!_lastKeyPress.ContainsKey(KeyCode.LeftArrow) ||
                    (currentTime - _lastKeyPress[KeyCode.LeftArrow] >= ignorance)))
            {
                PluginCore.MusicPlayer.Previous();
                _lastKeyPress[KeyCode.LeftArrow] = currentTime;
                PluginCore.ShowBubbleOnMainCharacter();

            }
            if (Input.GetKeyDown(KeyCode.RightArrow) &&
                (!_lastKeyPress.ContainsKey(KeyCode.RightArrow) ||
                    (currentTime - _lastKeyPress[KeyCode.RightArrow] >= ignorance)))
            {
                PluginCore.MusicPlayer.Next();
                _lastKeyPress[KeyCode.RightArrow] = currentTime;
                PluginCore.ShowBubbleOnMainCharacter();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow) &&
                (!_lastKeyPress.ContainsKey(KeyCode.UpArrow) ||
                    (currentTime - _lastKeyPress[KeyCode.UpArrow] >= ignorance)))
            {
                var currentMode = (int)PluginCore.MusicPlayer.LoopMode;
                PluginCore.MusicPlayer.LoopMode = (LoopMode)(++currentMode % 4);
                _lastKeyPress[KeyCode.UpArrow] = currentTime;
                PluginCore.ShowBubbleOnMainCharacter($"已切换至：[{PluginCore.MusicPlayer.LoopModePlainText}]!");
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) &&
                (!_lastKeyPress.ContainsKey(KeyCode.DownArrow) ||
                    (currentTime - _lastKeyPress[KeyCode.DownArrow] >= ignorance)))
            {
                var currentMode = (int)PluginCore.MusicPlayer.LoopMode;
                PluginCore.MusicPlayer.LoopMode = (LoopMode)(--currentMode + 4 % 4);
                _lastKeyPress[KeyCode.DownArrow] = currentTime;
                PluginCore.ShowBubbleOnMainCharacter($"已切换至：[{PluginCore.MusicPlayer.LoopModePlainText}]!");
            }
        }
    }
}
