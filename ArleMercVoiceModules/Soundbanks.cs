using System.IO;
using R2API;

namespace ArleMercVoice
{
    public static class SoundBanks
    {
        private static bool initialized;
        public static string SoundBankDirectory => Files.assemblyDir;
        public static void Init()
        {
            if (initialized)
            {
                return;
            }
            initialized = true;
            Stream stream = new FileStream(SoundBankDirectory + "\\ArleMercSoundbank.bnk", FileMode.Open);
            byte[] array = new byte[stream.Length];
            stream.Read(array, 0, array.Length);
            SoundAPI.SoundBanks.Add(array);
        }
    }
}
