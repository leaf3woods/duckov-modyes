using Modding.Core.MusicPlayer.Base;

namespace Modding.Core.MusicPlayer.FMod
{
    public class FModMusic<TInfo> : IMusic
    {
        public TInfo Info { get; set; } = default!;
        public string Sound { get; set; } = null!;
    }
}
