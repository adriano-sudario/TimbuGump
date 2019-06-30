using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimbuGump.Sounds
{
    public class Sfx
    {
        static Dictionary<string, SoundEffectInstance> songs = new Dictionary<string, SoundEffectInstance>();


        public static void Load(string key, SoundEffect song)
        {
            if (songs.ContainsKey(key))
                return;

            songs.Add(key, song.CreateInstance());
        }

        public static void Play(string key, bool isLoopig = false)
        {
            if (isLoopig)
                songs[key].IsLooped = true;

            songs[key].Play();
        }

        public static void Stop(string key)
        {
            songs[key].Stop();
        }
    }
}
