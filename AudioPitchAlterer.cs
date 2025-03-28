using System;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System.Linq;

namespace UltraTimeManipulation;
public class AudioSourcePitchAlterer : MonoBehaviour
{
    public AudioSource audioSource = null; //set outside
    private void Start()
    {

    }

    public float currentBasePitch = 0f; 
    public float lastPitch = 0f;

    public static unsafe int FloatToInt32Bits(float f) //bit data represented as int
    {
        return *( (int*)&f ); 
    }

    public static string[] arrMusicSourceNames = {"MusicChanger", "BossTheme", "BattleTheme", "CleanTheme", "VersusIntro", "Versus 2", "UndergroundHum", "SlowMo", "Sourire"};
    public static string[] arrMusicSongNames = {};
    public static string[] arrParrySourceNames = {"ParryLight(Clone)", "PunchSpecial(Clone)"};

    //dont think this is needed anymore
    public static string[] arrIgnoreSourceNames = {};//{"ElectricChargeBubble(Clone)", "WallCheck", "ChargeEffect", "Hammer", "HologramDisplay", "Barrel_L"}; //we dont mess with pitch of these sounds cause they just dont work

    private void Update()
    {
        if(audioSource == null) {return;} //build on
        if(MonoSingleton<AudioMixerController>.Instance == null) {return;}
        AudioMixerController amc = MonoSingleton<AudioMixerController>.Instance;

        float soundSlowDownMult      = (Plugin.currentSlowdownMult * Plugin.soundSlowdownFactor)      + 1f * (1 - Plugin.soundSlowdownFactor);
        float musicSlowDownMult      = (Plugin.currentSlowdownMult * Plugin.musicSlowdownFactor)      + 1f * (1 - Plugin.musicSlowdownFactor);
        float parrySoundSlowDownMult = (Plugin.currentSlowdownMult * Plugin.parrySoundSlowdownFactor) + 1f * (1 - Plugin.parrySoundSlowdownFactor);

        if(Plugin.modEnabled && Plugin.soundEnabled && Plugin.soundDistortionEnabled && audioSource.isPlaying)
        {
            if(Plugin.muffleMusicSlowDown && amc.musicSound != null) {amc.musicSound.SetFloat("lowPassVolume", -80f * (1 - Plugin.timeInRampup / Plugin.rampUpTime));}
            else if(amc.musicSound != null) {amc.musicSound.SetFloat("lowPassVolume", -80f);}
            bool audioIsSong = false;
            if(audioSource.clip != null) {audioIsSong = audioSource.name.ToLower().Contains("music") || audioSource.name.ToLower().Contains("song") || audioSource.clip.name.ToLower().Contains("music") || audioSource.clip.name.ToLower().Contains("song");}

            bool ignoreSource = arrIgnoreSourceNames.Contains(audioSource.name);
            bool musicSource = false;
            if(audioSource.clip != null) {musicSource = arrMusicSourceNames.Contains(audioSource.name) || arrMusicSongNames.Contains(audioSource.clip.name) || audioIsSong == true;}
            bool parrySource = arrParrySourceNames.Contains(audioSource.name);
            //bool otherSource = !(ignoreSource || musicSource || parrySource);

            if(FloatToInt32Bits(audioSource.pitch) != FloatToInt32Bits(lastPitch)) //compare bit-to-bit equality (using int equality)
            {
                currentBasePitch = audioSource.pitch;
                //Plugin.logger.LogInfo("a " + audioSource.name + audioSource.pitch);
            }

            if(ignoreSource) {} //do nothing
            else if(musicSource) {audioSource.pitch = musicSlowDownMult * currentBasePitch;}
            else if(parrySource) {audioSource.pitch = parrySoundSlowDownMult * currentBasePitch;}
            else {audioSource.pitch = soundSlowDownMult * currentBasePitch;}
            lastPitch = audioSource.pitch;
        }
    }
}

