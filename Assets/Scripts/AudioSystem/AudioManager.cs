using System.Collections.Generic;
using UnityEngine;

namespace Dao.AudioSystem
{
    public class AudioManager : Singleton<AudioManager>
    {
        private Dictionary<string, AudioClip> m_audios;


        public void AddAudio(string name, AudioClip clip)
        {
            m_audios ??= new();
            m_audios.TryAdd(name, clip);
        }

        public void RemoveAudio(string name)
        {
            m_audios.Remove(name);
        }

        public void Play(string name)
        {
            
        }
    }
}
