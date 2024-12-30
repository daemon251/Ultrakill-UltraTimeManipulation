using System;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace UltraTimeManipulation
{
    //ADAPTED FROM https://github.com/GAMINGNOOBdev/UltraRankSounds/tree/master
    //dont believe this needs to be an object, change later
    public class CustomSoundPlayer : MonoBehaviour
    {
        private static CustomSoundPlayer _instance = null;
        public static CustomSoundPlayer Instance
        {
            get { return _instance; }
            set { throw new NotImplementedException(); }
        }

        public AudioSource source;
        private string soundPath;

        private void Start()
        {
            _instance = this;
            source = gameObject.AddComponent<AudioSource>();
        }

        public void PlaySound(string file)
        {
            if (string.IsNullOrEmpty(file))
                return;

            if (!File.Exists(file))
            {
                Debug.LogError($"Could not find audio file '{file}'");
                return;
            }
            soundPath = file;
            gameObject.SetActive(true);
            StartCoroutine(PlaySoundRoutine());
        }
        //int b = 0;
        private IEnumerator PlaySoundRoutine()
        {
           // WaitUntil songFinished = new(() => Application.isFocused && !source.isPlaying);

            FileInfo fileInfo = new(soundPath);
            AudioType audioType = AudioType.WAV;

            using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(new Uri(soundPath).AbsoluteUri, audioType);
            DownloadHandlerAudioClip handler = request.downloadHandler as DownloadHandlerAudioClip;
            handler.streamAudio = false;
            request.SendWebRequest();
            yield return request;

            source.Stop(); //clear sound
            source.clip = handler.audioClip;
            source.volume = 1.0f * Plugin.volumeMult * MonoSingleton<PrefsManager>.Instance.GetFloat("sfxVolume"); 
            source.Play();
            //int a = 5 / b; //the coroutine unpauses upon either an error or finishes communicating... the latter is fucked for now and intentionally causing a runtime error makes it work. This is probably the most retarded piece of code I have ever witnessed. Removing this bricks the mod after using the slowdown key once.
            //yield return songFinished;
            //gameObject.SetActive(false);
            //UnityEngine.Object.Destroy(handler.audioClip);
        }
        
    }

}