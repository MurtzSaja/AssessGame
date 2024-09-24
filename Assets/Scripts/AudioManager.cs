using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AssessGame
{
    internal class AudioManager : MonoBehaviour
    {
        public enum AudioType
        {
            Flip,
            Matching,
            MisMatch,
            GameOver
        }
        [Serializable]
        private class AudioClipData
        {
            public AudioClip clip;
            public AudioType type;
        }

        private static AudioManager instance;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private List<AudioClipData> audioData;

        internal static AudioManager Instance { get => instance;private set => instance = value; }

        internal void Init()
        {
            if (instance != null && instance != this )
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        internal void PlaySFXOnShot(AudioType audioType)
        {
            audioSource.PlayOneShot(audioData.Where(x => x.type.Equals(audioType)).First().clip);
        }
    }
}
