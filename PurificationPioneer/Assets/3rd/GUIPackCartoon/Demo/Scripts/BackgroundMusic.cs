// Copyright (C) 2015, 2016 ricimi - All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement.
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ricimi
{
    // This class manages the audio source used to play the looping background song
    // in the demo. The player can choose to mute the music, and this preference is
    // persisted via Unity's PlayerPrefs.
    public class BackgroundMusic : MonoBehaviour
    {
        public List<AudioClip> bgMusicClips;

        public static BackgroundMusic Instance;

        public float Volume;
        private AudioSource m_audioSource;

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
                m_audioSource = GetComponent<AudioSource>();
                m_audioSource.ignoreListenerVolume = true;
               
               
            }
        }

        public void ChangeBGmusic(int index)
        {
       
            if (index >= bgMusicClips.Count)
                return;
            AudioClip clip = bgMusicClips[index];
            m_audioSource.clip = clip;
            m_audioSource.Play();
        }

        private void Start()
        {
            m_audioSource.volume = PlayerPrefs.GetInt("music_on") * Volume;
            AudioListener.volume = PlayerPrefs.GetInt("sound_on");
            ChangeBGmusic(PlayerPrefs.GetInt("world"));

        }

        public void FadeIn()
        {
            if (PlayerPrefs.GetInt("music_on") == 1)
            {
                StartCoroutine(FadeAudio(1.0f * Volume, Fade.In));
            }
        }

        public void FadeOut()
        {
            if (PlayerPrefs.GetInt("music_on") == 1)
            {
                StartCoroutine(FadeAudio(1.0f * Volume, Fade.Out));
            }
        }

        private enum Fade
        {
            In,
            Out
        }

        private IEnumerator FadeAudio(float time, Fade fadeType)
        {

            var start = fadeType == Fade.In ? 0.0f : Volume;

            var end = fadeType == Fade.In ? Volume : 0.0f;
            var i = 0.0f;
            var step = 5.0f / time;

            while (i <= 1.0f)
            {
                i += step * Time.deltaTime;
                m_audioSource.volume = Mathf.Lerp(start, end, i);
                yield return new WaitForSeconds(step * Time.deltaTime);
            }


        }

      
    }
}
