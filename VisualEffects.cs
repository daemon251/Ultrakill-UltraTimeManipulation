using HarmonyLib;
using UnityEngine;
using System;

#nullable disable
namespace UltraTimeManipulation.Patches
{
    [HarmonyPatch(typeof (NewMovement))]
    internal class Effects
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void Darken()
        {
            if(Plugin.visualsEnabled)
            {
                float slowModeProgress = Plugin.timeInRampup / Plugin.rampUpTime;
                int baseValue = 0;
                float exponentMultiplier = 0f;
                switch (MonoSingleton<PrefsManager>.Instance.GetInt("colorCompression"))
                {
                    case 0:
                        baseValue = 2048;
                        exponentMultiplier = 8.0f;
                        break;
                    case 1:
                        baseValue = 64;
                        exponentMultiplier = 3.0f;
                        break;
                    case 2:
                        baseValue = 32;
                        exponentMultiplier = 2.0f;
                        break;
                    case 3:
                        baseValue = 16;
                        exponentMultiplier = 1.0f;
                        break;
                    case 4:
                        baseValue = 8;
                        exponentMultiplier = 0.0f;
                        break;
                    case 5:
                        baseValue = 3;
                        exponentMultiplier = 0.5f; //I guess? Feels weird for it to be 0 cause then there is zero visual effect. Not that setting it to 1 does anything anyway
                        break;
                }
                exponentMultiplier += Plugin.slowdownColorCompression;
                int value = (int)(baseValue / Math.Pow(2.0, slowModeProgress * exponentMultiplier)); //doing it exponentionally instead of linearly gives a much better effect IMO. Ranges from 2048 to 8 with slowdownDarkness at 0
                if(value < 0) {value = 0;}
                if(value > baseValue) {value = baseValue;}
                Shader.SetGlobalInt("_ColorPrecision", value);

                float baseGamma = MonoSingleton<PrefsManager>.Instance.GetFloat("gamma");
                float value2 = baseGamma - slowModeProgress * Plugin.slowdownDarkness;

                Shader.SetGlobalFloat("_Gamma", value2);
                //Shader.SetGlobalFloat("_Outline", 4f);
                //Shader.SetGlobalFloat("_ForceOutline", 4f);
            }
        }
    }
}
