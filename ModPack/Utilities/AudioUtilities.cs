using System.Collections.Generic;
using System.IO;
using ModPack21341.Harmony;
using ModPack21341.Models;
using NAudio.Wave;
using UnityEngine;

namespace ModPack21341.Utilities
{
    public class AudioUtilities
    {
        public static AudioClip Mp3toAudioClip(string path)
        {
            var sourceProvider = new Mp3FileReader(path);
            WaveFileWriter.CreateWaveFile(path + ".wav", sourceProvider);
            var wav = new Wav(File.ReadAllBytes(path + ".wav"));
            var audioClip = AudioClip.Create("cove", wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);
            File.Delete(path + ".wav");
            return audioClip;
        }
        public static void ChangeEnemyTeamTheme(string bgmName)
        {
            var currentMapManager = SingletonBehavior<BattleSceneRoot>.Instance.currentMapObject;
            currentMapManager.mapBgm[0] = ModPack21341Init.CustomSound[bgmName];
            currentMapManager.mapBgm[1] = ModPack21341Init.CustomSound[bgmName];
            currentMapManager.mapBgm[2] = ModPack21341Init.CustomSound[bgmName];
            SingletonBehavior<BattleSoundManager>.Instance.SetEnemyTheme(currentMapManager.mapBgm);
            SingletonBehavior<BattleSoundManager>.Instance.CheckTheme();
        }
        public static Dictionary<string, AudioClip> PrepareAudioClips() => new Dictionary<string, AudioClip>
            {
                {
                    "Reflection",
                    Mp3toAudioClip(ModPack21341Init.path + "/BGM/OldSamurai/Reflection.mp3")
                },
                {
                    "Hornet",
                    Mp3toAudioClip(ModPack21341Init.path + "/BGM/OldSamurai/Hornet.mp3")
                },
                {
                    "MioPhase1",
                    Mp3toAudioClip(ModPack21341Init.path + "/BGM/Mio/Phase1.mp3")
                },
                {
                    "MioPhase2",
                    Mp3toAudioClip(ModPack21341Init.path + "/BGM/Mio/Phase2.mp3")
                }
            };

    }
}
