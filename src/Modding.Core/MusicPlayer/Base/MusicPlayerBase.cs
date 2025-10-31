using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Modding.Core.MusicPlayer.Base
{
    public abstract class MusicPlayerBase<TMusic> : IMusicPlayer<TMusic>
        where TMusic : IMusic
    {
        protected List<TMusic> Musics { get; set; } = new List<TMusic>();
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? User { get; set; }                                        
        public float Volume { get; set; }

        public bool IsPlaying { get; protected set; } = false;
        public bool IsPasued { get; protected set; } = false;

        public bool EnableFadeOut { get; set; } = false;

        public bool IsMute { get; protected set; }
        public LoopMode LoopMode { get; set; } = LoopMode.None;
        public ShuffleMode ShuffleMode { get; set; } = ShuffleMode.FisherYates;
        public int Count => Musics.Count;

        protected int p1 = -1;
        protected int p2 = -1;
        private bool _onFadeIn = false;
        private bool _onFadeOut = false;

        /// <summary>
        /// 当前播放项
        /// </summary>
        public (int index, TMusic music) Current => (p2 >= 0 && p2 < Musics.Count) ? (p2, Musics[p2]) : default!;
        protected Random _random = new Random();

        public virtual void Load(IEnumerable<TMusic> musics)
        {
            Musics.AddRange(musics);
        }

        public abstract void Next();

        public abstract void Previous();

        public abstract void Start(LoopMode mode = LoopMode.Single,
            float volume = 0.5f,
            ShuffleMode shuffle = ShuffleMode.FisherYates,
            bool mute = false);

        public abstract void Play(int index = -1);

        public abstract void Stop();

        public abstract void ToggleMute(bool? mute = null);

        public abstract void TogglePause(bool? paused = null);

        public abstract void ApplyVolume(float? volume = null, bool apply = true);

        protected List<TMusic> Shuffle(List<TMusic> array)
        {
            var n = array.Count;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1); // [0, n] 之间的随机索引
                (array[k], array[n]) = (array[n], array[k]); // 交换
            }
            return array;
        }

        protected TMusic[] Shuffle(TMusic[] array)
        {
            var n = array.Length;
            while (n > 1)
            {
                n--;
                int k = _random.Next(n + 1); // [0, n] 之间的随机索引
                (array[k], array[n]) = (array[n], array[k]); // 交换
            }
            return array;
        }

        public string LoopModePlainText
        {
            get => LoopMode switch
            {
                LoopMode.None => "停止",
                LoopMode.Single => "单曲循环",
                LoopMode.Random => "随机循环",
                LoopMode.Loop => "列表循环",
                _ => "未知"
            };
        }

        public async Task FadeAsync(float duration, bool outing = true)
        {
            if ((_onFadeIn && !outing) || (_onFadeOut && outing)) return;
            const float ignorance = 0.02f;

            var steps = (int)Math.Ceiling(Volume / ignorance);
            var stepTime = duration / steps;
            
            if (!outing)
            {
                _onFadeIn = true;
                for (int i = 0; i < steps; i++)
                {
                    if (_onFadeOut) break;
                    var volume = (float)i / steps;
                    ApplyVolume(volume, false);
                    await Task.Delay((int)(stepTime * 1000));
                }
                _onFadeIn = false;
            }
            else
            {
                _onFadeOut = true;
                for (int i = 0; i < steps; i++)
                {
                    if(_onFadeIn) break;
                    var volume = Volume - (float)i / steps;
                    if (volume < ignorance)
                    {
                        ApplyVolume(0, false);
                        break;
                    }
                    ApplyVolume(volume, false);
                    await Task.Delay((int)(stepTime * 1000));
                }
                _onFadeOut = false;
            }     
        }

        public async Task FadeOutAsync(float duration, StopMode mode)
        { 
            await FadeAsync(duration, true);
            if (mode == StopMode.Stop)
            {
                IsPlaying = false;
                Stop();
            }
            else
            {
                IsPasued = true;
                TogglePause(true);
            }
            ApplyVolume(Volume);
        }

        public async Task FadeInAsync(float duration)
        {
            var volume = Volume;
            ApplyVolume(0);
            IsPlaying = true;    
            await FadeAsync(duration, false);
            Volume = volume;
        }
    }
}
