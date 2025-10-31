using FMOD;
using FMODUnity;
using Modding.Core.MusicPlayer.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modding.Core.MusicPlayer.FMod
{
    public class FModMusicPlayer<TInfo> :
        MusicPlayerBase<FModMusic<TInfo>>
    {
        private static readonly Dictionary<IntPtr, WeakReference<FModMusicPlayer<TInfo>>> _players =
            new Dictionary<IntPtr, WeakReference<FModMusicPlayer<TInfo>>>();
        public override void Start(
            LoopMode mode = LoopMode.Single,
            float volume = 0.5f,
            ShuffleMode shuffle = ShuffleMode.FisherYates,
            bool mute = false)
        {
            LoopMode = mode;
            Volume = volume;
            ShuffleMode = shuffle;
            IsMute = false;
        }

        private Sound _currentSound;
        private Channel _currentChannel;
        private ChannelGroup _currentChannelGroup = new ChannelGroup();

        // 播放随机下一首
        public override void Next()
        {
            switch (LoopMode)
            {
                case LoopMode.None:
                case LoopMode.Single:
                case LoopMode.Loop:
                    Play(++p2 % Musics.Count);
                    break;
                case LoopMode.Random:
                    if (p1 == p2)
                    {
                        // 已经播放到最后，需要随机扩展下一首
                        p1 = (p1 + 1) % Musics.Count;
                        p2 = p1;

                        // 取半区随机偏移
                        int k = _random.Next(Musics.Count / 2 + 1);
                        int swapIndex = (p1 + k) % Musics.Count;

                        // 交换元素
                        (Musics[p1], Musics[swapIndex]) = (Musics[swapIndex], Musics[p1]);
                    }
                    else
                    {
                        // 处于回溯状态，仅前进播放
                        p2 = (p2 + 1) % Musics.Count;
                    }
                    Play(p2);
                    break;
            }
        }

        /// <summary>
        /// -1: 播放当前
        /// </summary>
        /// <param name="index"></param>
        public override void Play(int index)
        {
            if (Musics.Count == 0) throw new InvalidOperationException("music list is empty");
            if(IsPlaying) Stop();
            TogglePause(false);          
            if(index < 0)
            {
                switch (LoopMode)
                {
                    case LoopMode.None:
                        break;
                    case LoopMode.Single:
                        Play(index == -2 ? p2 % Musics.Count : ++p2 % Musics.Count);
                        break;
                    case LoopMode.Loop:
                    case LoopMode.Random:
                        Next();
                        break;
                }
            }
            else
            {
                p2 = index;
                var path = Musics[index].Sound;
                Task.Run(() => {
                    var result = RuntimeManager.CoreSystem.createSound(path, MODE.DEFAULT | MODE.LOOP_OFF, out _currentSound);
                    if (result != RESULT.OK) throw new InvalidOperationException($"create sound failure, path: {path}");
                    RuntimeManager.CoreSystem.playSound(_currentSound, _currentChannelGroup, false, out _currentChannel);
                    _currentChannel.setVolume(Volume);
                    _currentChannel.setCallback(ChannelCallback);

                    var key = _currentChannel.handle;
                    _players[key] = new WeakReference<FModMusicPlayer<TInfo>>(this);
                });
            }
            IsPlaying = true;
        }

        private static RESULT ChannelCallback(
            IntPtr channelControl,
            CHANNELCONTROL_TYPE controlType,
            CHANNELCONTROL_CALLBACK_TYPE callbackType,
            IntPtr commandData1,
            IntPtr commandData2)
        {
            if (controlType == CHANNELCONTROL_TYPE.CHANNEL &&
                callbackType == CHANNELCONTROL_CALLBACK_TYPE.END)
            {
                if (_players.TryGetValue(channelControl, out var weakRef) &&
                    weakRef.TryGetTarget(out var player))
                {
                    Task.Run(() => player.Play(-2));
                }
            }
            return RESULT.OK;
        }

        public override void ApplyVolume(float? volume = null, bool apply = true)
        {
            if(!apply)
            {
                _currentChannel.setVolume(volume ?? Volume);
            }
            else
            {
                Volume = volume ?? Volume;
                _currentChannel.setVolume(Volume);
            }
        }

        public override void TogglePause(bool? paused = null)
        {
            if (IsPlaying)
            {
                IsPasued = paused is null ? !IsPasued : paused.Value;
                _currentChannel.setPaused(IsPasued);
            }
        }

        public override void Previous()
        {
            if (p2 == -1) p2 = 0; // 初始化

            p2 = (--p2 + Musics.Count) % Musics.Count;
            Play(p2);
        }

        public override void Stop()
        {
            if (_currentChannel.hasHandle())
            {
                _players.Remove(_currentChannel.handle);
            }
            _currentChannel.stop();
            _currentChannel.clearHandle();
            _currentSound.release();
            _currentSound.clearHandle();
            IsPlaying = false;
        }

        public override void ToggleMute(bool? mute = null)
        {
            IsMute = mute is null ? !IsMute : mute.Value;
            _currentChannel.setMute(true);
        }

        /// <summary>
        ///     支持的音乐格式
        /// </summary>

        public static string[] SupportedTypes = { "*.mp3", "*.flac", "*.aac", "*.m4a" };
    }
}
