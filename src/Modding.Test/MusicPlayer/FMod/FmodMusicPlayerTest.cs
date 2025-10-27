using Microsoft.VisualStudio.TestTools.UnitTesting;
using Modding.Core.MusicPlayer.Base;
using Modding.Core.MusicPlayer.FMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modding.Core.MusicPlayer.FMod.Tests
{
    [TestClass()]
    public class FmodMusicPlayerTest
    {
        [TestMethod()]
        public void StartTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void NextTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PlayTest()
        {
            var _musicPlayer = new FModMusicPlayer<string>();
            var bgmDir = Path.Combine(Environment.CurrentDirectory, "MyBGM");
            if (!Directory.Exists(bgmDir)) Directory.CreateDirectory(bgmDir);
            var musics = FModMusicPlayer<string>.SupportedMusicExtensions
                .SelectMany(pa => Directory.GetFiles(bgmDir, pa))
                .Select(f => new FModMusic<string>
                {
                    Info = Path.GetFileNameWithoutExtension(f),
                    Sound = f
                });
            _musicPlayer.Load(musics);
            _musicPlayer.LoopMode = LoopMode.Loop;
            _musicPlayer.Play(-1);
            Assert.Fail();
        }

        [TestMethod()]
        public void ApplyVolumeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void TogglePauseTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void PreviousTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void StopTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void ToggleMuteTest()
        {
            Assert.Fail();
        }
    }
}