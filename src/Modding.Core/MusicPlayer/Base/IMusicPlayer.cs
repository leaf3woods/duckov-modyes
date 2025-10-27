using System.Collections.Generic;

namespace Modding.Core.MusicPlayer.Base
{
    public interface IMusicPlayer<TMusic> 
        where TMusic : IMusic
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? User { get; set; }

        public bool IsPlaying { get; }
        public bool IsMute { get; }


        public float Volume { get; set; }
        public LoopMode LoopMode { get; set; }
        public ShuffleMode ShuffleMode { get; set; }

        public void Load(IEnumerable<TMusic> musics);
        public void Start(LoopMode mode = LoopMode.Single,
            float volume = 0.5f,
            ShuffleMode shuffle = ShuffleMode.FisherYates,
            bool mute = false);

        public void Play(int index = -1);

        public void Next();

        public void Previous();
        public void TogglePause();
        public void Stop();

        public void ToggleMute(bool mute);

    }

    public enum LoopMode
    {
        None,
        Single,
        Random,
        Loop,
    }

    public enum ShuffleMode
    {
        FisherYates,
        Random,
        Loop,
    }
}
