﻿//discord: daemon8363
//I don't know how to make a license, but you are free to modify, use, or copy this code. I don't care. Contact me, prefferably on discord, if you need anything.

using System;
using System.IO;
using BepInEx;
using UnityEngine;
using System.Reflection;

using HarmonyLib;
using System.Linq;

using UltraTimeManipulation.Patches;
using System.Collections.Generic;

//TODO: 
//slowdown all game audio when slowdown activated... how the hell is this going to be done LOOK AT: muffle music underwater
//better vfx
//sparks are faster when moving??? DUE TO LAG????
//doesnt slow down when you die
//better slowdown... stop doing it on tick idiot
//organize code
//change recharge to restart instead of death
//mouse 5 dont work probably
//plugin config

//    foreach (AudioMixer audioMixer in this.audmix)     parryFlash
//      audioMixer.SetFloat("allPitch", 1f);

//DO LATER: ability for to slow down time for everyone but you

#nullable disable
namespace UltraTimeManipulation;

[BepInPlugin("UltraTimeManipulation", "UltraTimeManipulation", "0.01")]
public class Plugin : BaseUnityPlugin
{
    //THESE VALUES SHOULD BE OVERWRITTEN LATER BY CONFIG, PAY NO MIND
    public static System.Random rnd = new System.Random();

    public static bool modEnabled = true; //disables CG score submission when on

    public static float realTimeDelta = 1f;

    public static float rechargeMultiplier = 0.75f; 
    public static float maxTimeAccumulated = 6.0f; //real time seconds
    public static float rampUpTime = 0.2f; //time it takes until the full slowdown effect is reached
    public static float slowdownMult = 0.4f; //max slowdown mult

    public static bool keyToggleFunctionality = false; //whether the button makes it toggle or whether you need to hold for slowdown
    public static KeyCode slowdownCode = KeyCode.Mouse3; //this is actually normally called the mouse4 button, we love zero-indexing !!!

    public static float volumeMult = 0.7f;
    public static bool soundEnabled = true;
    public static bool soundSlowdownEnabled = true;
    public static float soundSlowDown = 1.0f;
    public static float soundSlowDownRelativePitch = 1.0f;

    public static bool visualsEnabled = true;
    public static float slowdownColorCompression = 1.0f;
    public static float slowdownDarkness = 1.0f;

    public static bool legacyDisplay = true; //bottom left asterisks display, looks like shit
    public static bool HUDFlashing = true;
    public static bool HUDSparks = true;
    public static float HUDSparksFadeOutTime = 2.0f;
    public static float HUDScale = 1;
    public static bool minimalHUD = false;
    public static int HUDQuadrant = 3; //default bottom left

    public static string DefaultParentFolder = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";

    public static string DefaultImageFolder = $"{Path.Combine(DefaultParentFolder!, "img")}";
    public static string underlayImageFile = $"{Path.Combine(DefaultImageFolder!, "underlay.png")}";
    public static string underlayTopImageFile = $"{Path.Combine(DefaultImageFolder!, "underlayTop.png")}";
    public static string filledBarImageFile = $"{Path.Combine(DefaultImageFolder!, "filledBar.png")}";
    public static string emptyBarImageFile = $"{Path.Combine(DefaultImageFolder!, "emptyBar.png")}";
    public static string sparkFolder = $"{Path.Combine(DefaultImageFolder!, "sparks")}";

    //because its easier to do it this way.
    public static string spark0ImageFile = $"{Path.Combine(sparkFolder!, "spark0.png")}";
    public static string spark10ImageFile = $"{Path.Combine(sparkFolder!, "spark10.png")}";
    public static string spark20ImageFile = $"{Path.Combine(sparkFolder!, "spark20.png")}";
    public static string spark30ImageFile = $"{Path.Combine(sparkFolder!, "spark30.png")}";
    public static string spark40ImageFile = $"{Path.Combine(sparkFolder!, "spark40.png")}";
    public static string spark50ImageFile = $"{Path.Combine(sparkFolder!, "spark50.png")}";
    public static string spark60ImageFile = $"{Path.Combine(sparkFolder!, "spark60.png")}";
    public static string spark70ImageFile = $"{Path.Combine(sparkFolder!, "spark70.png")}";
    public static string spark80ImageFile = $"{Path.Combine(sparkFolder!, "spark80.png")}";
    public static string spark90ImageFile = $"{Path.Combine(sparkFolder!, "spark90.png")}";

    public static string DefaultSoundFolder = $"{Path.Combine(DefaultParentFolder!, "audio")}";
    public static string speedupSound   = $"{Path.Combine(DefaultSoundFolder!, "speedup.wav")}";
    public static string slowdownSound   = $"{Path.Combine(DefaultSoundFolder!, "slowdown.wav")}";

    CustomSoundPlayer csp;

    private readonly Harmony harmony = new Harmony("daemon.UltraTimeManipulation");

    void determineTimeDelta() //determines the real deltaTime (Time.deltaTime accounts for timescale). Also, nessecary for when the timescale is zero. Used for UI mainly.
    {
        if(currentSlowdownMult != 0 && !inMenu()) {realTimeDelta = Time.deltaTime / currentSlowdownMult;}
        else {realTimeDelta = Time.unscaledDeltaTime;} //1.0f / 60.0f;
    }
    bool inMenu()
    {
        //Debug.Log(Time.deltaTime);
        return Time.deltaTime < 0.0001;
    }

    private void Awake()
    {
        PluginConfig.UltraTimeManipulationConfig();
        csp = gameObject.AddComponent<CustomSoundPlayer>();
        //this.harmony.PatchAll(typeof (Effects));
        this.harmony.PatchAll();
        Logger.LogInfo("UltraTimeManipulation Started");

    }

    //copied from other mods, should work
    [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitCyberGrindScore))]
    [HarmonyPrefix]
    public static bool no(LeaderboardController __instance) {return !modEnabled;}

    [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitLevelScore))]
    [HarmonyPrefix]
    public static bool nope(LeaderboardController __instance){return !modEnabled;}

    [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitFishSize))] //watch out guys, hes gonna catch a size 2 !!!!!
    [HarmonyPrefix]
    public static bool notevenfish(LeaderboardController __instance){return !modEnabled;}

    public static bool modEnabledTemporary = true; //enables/disables mods effects, used to disable it when in menus, etc.
    public static bool IsGameplayScene() //copied from UltraTweaker
    {
        string[] NonGameplay = {"Intro","Bootstrap","Main Menu","Level 2-S","Intermission1","Intermission2"};
        return !NonGameplay.Contains(SceneHelper.CurrentScene);
    }

    Texture2D underlayTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D underlayTopTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D filledBarTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D emptyBarTexture = new Texture2D(0, 0, TextureFormat.RGBA32, false);

    //Because rotating a texture is a pain in the ass, this is what I opted for... 10 different spark images pre-rotated
    Texture2D spark0Texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D spark10Texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D spark20Texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D spark30Texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D spark40Texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D spark50Texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D spark60Texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D spark70Texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D spark80Texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    Texture2D spark90Texture = new Texture2D(0, 0, TextureFormat.RGBA32, false);
    public void Start()
    {   //load all the images and stuffsss maaan
        underlayTexture.LoadImage(File.ReadAllBytes(underlayImageFile));
        underlayTopTexture.LoadImage(File.ReadAllBytes(underlayTopImageFile));
        filledBarTexture.LoadImage(File.ReadAllBytes(filledBarImageFile));
        emptyBarTexture.LoadImage(File.ReadAllBytes(emptyBarImageFile));

        spark0Texture.LoadImage(File.ReadAllBytes(spark0ImageFile));
        spark10Texture.LoadImage(File.ReadAllBytes(spark10ImageFile));
        spark20Texture.LoadImage(File.ReadAllBytes(spark20ImageFile));
        spark30Texture.LoadImage(File.ReadAllBytes(spark30ImageFile));
        spark40Texture.LoadImage(File.ReadAllBytes(spark40ImageFile));
        spark50Texture.LoadImage(File.ReadAllBytes(spark50ImageFile));
        spark60Texture.LoadImage(File.ReadAllBytes(spark60ImageFile));
        spark70Texture.LoadImage(File.ReadAllBytes(spark70ImageFile));
        spark80Texture.LoadImage(File.ReadAllBytes(spark80ImageFile));
        spark90Texture.LoadImage(File.ReadAllBytes(spark90ImageFile));
    }

    private float OnGUIx(int i) {return (100 + 40 * i) * HUDScale;} //for use in OnGUI
    Double timeUntilFlash = 0.0f;
    Double timeUntilFlashEnd = 0.20f;

    Double timeUntilMiniFlashRefresh = 0.1f;
    Double[] valueMultsSmallFlash = new double[11];
    Color[] miniFlashes = new Color[11];
    int sparkSideCounter = 0; //spark right or left side counter
    Texture2D currentSparkTex;

    float sparkTimer = 0f;

    private class particleInfo
    {
        public float x, y, xVel, yVel, timeLeft;
        public Color color;
        public particleInfo(float x, float y, float xVel, float yVel, float timeLeft)
        {
            this.x = x; this.y = y; this.xVel = xVel; this.yVel = yVel; this.timeLeft = timeLeft;
            float r = 0.45f;
            float g = 0.8f + 0.2f * (float)rnd.NextDouble();
            float b = 0.8f + 0.2f * (float)rnd.NextDouble();
            float a = 0.6f + 0.4f * (float)rnd.NextDouble();
            color = new Color(r,g,b,a);
        }
    }

    List<particleInfo> particles = new List<particleInfo>();

    void OnGUI() //called every frame
    {
        if(!modEnabled || !modEnabledTemporary){return;}
        //this displays the amount of charge left
        if(legacyDisplay)
        {
            GUIStyle boxStyle = new GUIStyle();
            boxStyle.normal.textColor = Color.cyan;
            boxStyle.fontSize = (int)Math.Round(48 * HUDScale * 4);
            boxStyle.alignment = TextAnchor.MiddleCenter;
            GUI.backgroundColor = Color.black; //does nothing?

            string boxString = "";
            for (int i = 0; i < Math.Round((10.0 / maxTimeAccumulated) * (maxTimeAccumulated - timeAccumulated)); i++) //10 stars max
            {
                boxString += "*";
            }
            UnityEngine.GUI.Box(new Rect(60*4*HUDScale, Screen.height - (60 + 10)*4*HUDScale, 150*4*HUDScale, 60*4*HUDScale), boxString, boxStyle);
        }
        else
        {   
            int HUDTop = 0; //boolean values but in int for math convenience
            int HUDRight = 0;

            if(HUDQuadrant == 1) {HUDTop = 1; HUDRight = 1;}
            else if (HUDQuadrant == 2) {HUDTop = 1; HUDRight = 0;}
            else if (HUDQuadrant == 3) {HUDTop = 0; HUDRight = 0;}
            else if (HUDQuadrant == 4) {HUDTop = 0; HUDRight = 1;}

            if(!minimalHUD) {UnityEngine.GUI.DrawTexture(new Rect(Screen.width * HUDRight, Screen.height *  (1 - HUDTop), underlayTexture.width * HUDScale * (1 - (2 * HUDRight)), underlayTexture.height * HUDScale * ((2 * HUDTop) - 1)), underlayTexture, ScaleMode.StretchToFill);}

            //flashing effect.
            Double valueMultBigFlash = 1.0f;
            Color tint = new Color((float)valueMultBigFlash, (float)valueMultBigFlash, (float)valueMultBigFlash, 1);
            if(HUDFlashing) 
            {
                valueMultBigFlash = 1.0f;
                timeUntilFlash += -realTimeDelta;
                if(timeUntilFlash < 0)
                {
                    timeUntilFlashEnd += -realTimeDelta;
                    valueMultBigFlash = 1.0f - 0.9 * (1.0f - 10.0f * Math.Abs(0.1f - timeUntilFlashEnd));
                    if(timeUntilFlashEnd < 0)
                    {
                        timeUntilFlash = 1.90f * rnd.NextDouble() + 1.80f; 
                        if(rnd.NextDouble() < 0.38) {timeUntilFlash = 0.05 + 0.20 * rnd.NextDouble();} //38% chance to flicker again
                        timeUntilFlashEnd = 0.20f;
                    }
                }
                tint = new Color((float)valueMultBigFlash, (float)valueMultBigFlash, (float)valueMultBigFlash, 1);

                timeUntilMiniFlashRefresh += -realTimeDelta;
                if(timeUntilMiniFlashRefresh < 0)
                {
                    timeUntilMiniFlashRefresh = 0.1;
                    for (int i = 0; i < miniFlashes.Length; i++)
                    {
                        valueMultsSmallFlash[i] = 0.90 + 0.10 * rnd.NextDouble();
                    }
                }
                for(int i = 0; i < miniFlashes.Length; i++) //handled down here so that it applies every tick instead of every 0.1s
                {
                    miniFlashes[i] = new Color((float)(valueMultsSmallFlash[i] * valueMultBigFlash), (float)(valueMultsSmallFlash[i] * valueMultBigFlash), (float)(valueMultsSmallFlash[i] * valueMultBigFlash), 1);
                }
            }
            else
            {
                valueMultBigFlash = 1.0f;
                tint = new Color((float)valueMultBigFlash, (float)valueMultBigFlash, (float)valueMultBigFlash, 1);
                for (int i = 0; i < valueMultsSmallFlash.Length; i++)
                {
                    valueMultsSmallFlash[i] = 1.0f;
                    miniFlashes[i] = new Color(1.0f, 1.0f, 1.0f, 1);
                }
            }

            UnityEngine.GUI.DrawTexture(new Rect(Screen.width * HUDRight, Screen.height * (1 - HUDTop), underlayTexture.width * HUDScale * (1 - (2 * HUDRight)), underlayTexture.height * HUDScale * ((2 * HUDTop) - 1)), underlayTopTexture, ScaleMode.StretchToFill, true, 0, tint, 0, 0);
            int filledBars = (int)Math.Round((11.0 / maxTimeAccumulated) * (maxTimeAccumulated - timeAccumulated));

            //x is OnGUIx
            float y = 44 * HUDScale;
            float width = filledBarTexture.width * HUDScale; //same for filled and unfilled
            float height = emptyBarTexture.height * HUDScale;

            for(int i = 0; i < filledBars; i++)
            {
                UnityEngine.GUI.DrawTexture(new Rect((Screen.width - OnGUIx(i)) * HUDRight + OnGUIx(i) * (1 - HUDRight), y * HUDTop + (Screen.height - y) * (1 - HUDTop), width * (1 - (2 * HUDRight)), height * ((2 * HUDTop) - 1)), filledBarTexture, ScaleMode.StretchToFill, true, 0, miniFlashes[i], 0, 0);
            }
            for(int i = filledBars; i < 11; i++)
            {
                UnityEngine.GUI.DrawTexture(new Rect((Screen.width - OnGUIx(i)) * HUDRight + OnGUIx(i) * (1 - HUDRight), y * HUDTop + (Screen.height - y) * (1 - HUDTop), width * (1 - (2 * HUDRight)), height * ((2 * HUDTop) - 1)), emptyBarTexture, ScaleMode.StretchToFill, true, 0, miniFlashes[i], 0, 0);
            }

            if(HUDSparks)
            {
                //add sparks to list
                if((timeInRampup > 0f || (slowdownKeyActive && !inMenu())) && timeAccumulated < maxTimeAccumulated)
                {
                    sparkTimer += -realTimeDelta;
                    float xPos;
                    float yPos;
                    if(sparkTimer < 0)
                    {
                        sparkSideCounter++;
                        if(sparkSideCounter % 3 == 0)
                        {
                            xPos = (Screen.width - 36 * HUDScale) * HUDRight + 36 * HUDScale * (1 - HUDRight);
                            yPos = (Screen.height - 92 * HUDScale) * (1 - HUDTop) + HUDTop * HUDScale * 92;
                        }
                        else if(sparkSideCounter % 3 == 1)
                        {
                            xPos = (Screen.width - 592 * HUDScale) * HUDRight + 592 * HUDScale * (1 - HUDRight);
                            yPos = (Screen.height - 144 * HUDScale) * (1 - HUDTop) + HUDTop * HUDScale * 144;
                        }
                        else 
                        {
                            xPos = (Screen.width - 612 * HUDScale) * HUDRight + 612 * HUDScale * (1 - HUDRight);
                            yPos = (Screen.height - 80 * HUDScale) * (1 - HUDTop) + HUDTop * HUDScale * 80;
                        }
                        sparkTimer = 0.04f;

                        Double theta = rnd.NextDouble() * Math.PI * 2;
                        Double speed = 200 + 100 * rnd.NextDouble();

                        Double xVel = speed * Math.Cos(theta) * 0.75; //0.75 added to make it fall more down and less to sides
                        Double yVel = speed * Math.Sin(theta);

                        /*Double xVel = 200.0f * rnd.NextDouble() - 100f; //in px / s 
                        Double yVel = -120f * rnd.NextDouble() + 75f; //in px / s
                        if(HUDTop == 0) {yVel *= -1;}

                        //make particles start with at least minimum 60 vel
                        Double speed = Math.Sqrt(xVel * xVel + yVel * yVel);
                        if(speed < 60 && speed != 0)
                        {
                            xVel *= (60 / speed);
                            yVel *= (60 / speed);
                        }*/

                        particles.Add(new particleInfo(xPos, yPos, (float)xVel, (float)yVel, HUDSparksFadeOutTime));
                    }
                }
                //render
                for (int i = 0; i < particles.Count; i++)
                {
                    particleInfo part = particles[i];
                    Double angle = Math.Atan(part.yVel/part.xVel); //(1,0) is theta = 0, goes how you would expect.
                    if(part.xVel < 0){angle = Math.PI + angle;}
                    if(angle < 0){angle += Math.PI * 2;}
                    if(angle > Math.PI * 2) {angle += -Math.PI * 2;}

                    //this is far from ideal but its easy for me to follow
                    currentSparkTex = spark0Texture;
                    if(angle > Math.PI) {angle += -Math.PI;} //abuse the symmetry of the sprite
    
                    if(angle < 5 * Math.PI / 180.0f) {currentSparkTex = spark90Texture;} //from 0 to 5
                    else if(angle < 15 * Math.PI / 180.0f) {currentSparkTex = spark80Texture;} //from 5 to 15 
                    else if(angle < 25 * Math.PI / 180.0f) {currentSparkTex = spark70Texture;} //from 15 to 25 
                    else if(angle < 35 * Math.PI / 180.0f) {currentSparkTex = spark60Texture;} //from 25 to 35 
                    else if(angle < 45 * Math.PI / 180.0f) {currentSparkTex = spark50Texture;} //from 35 to 45
                    else if(angle < 55 * Math.PI / 180.0f) {currentSparkTex = spark40Texture;} //from 45 to 55 
                    else if(angle < 65 * Math.PI / 180.0f) {currentSparkTex = spark30Texture;} //from 55 to 65 
                    else if(angle < 75 * Math.PI / 180.0f) {currentSparkTex = spark20Texture;} //from 65 to 75 
                    else if(angle < 85 * Math.PI / 180.0f) {currentSparkTex = spark10Texture;} //from 75 to 85 
                    else if(angle < 95 * Math.PI / 180.0f) {currentSparkTex = spark0Texture;} //from 85 to 95
                    //at this point now we also need to mirror flip along y axis. Done later
                    else if(angle < 105 * Math.PI / 180.0f) {currentSparkTex = spark10Texture;} //from 95 to 105 
                    else if(angle < 115 * Math.PI / 180.0f) {currentSparkTex = spark20Texture;} //from 105 to 115 
                    else if(angle < 125 * Math.PI / 180.0f) {currentSparkTex = spark30Texture;} //from 115 to 125 
                    else if(angle < 135 * Math.PI / 180.0f) {currentSparkTex = spark40Texture;} //from 125 to 135
                    else if(angle < 145 * Math.PI / 180.0f) {currentSparkTex = spark50Texture;} //from 135 to 145 
                    else if(angle < 155 * Math.PI / 180.0f) {currentSparkTex = spark60Texture;} //from 145 to 155 
                    else if(angle < 165 * Math.PI / 180.0f) {currentSparkTex = spark70Texture;} //from 155 to 165 
                    else if(angle < 175 * Math.PI / 180.0f) {currentSparkTex = spark80Texture;} //from 165 to 175 
                    else if(angle < 180 * Math.PI / 180.0f) {currentSparkTex = spark90Texture;} //from 175 to 180 

                    int flip = 0; //boolean but int for math convenience
                    if(angle > 90 * Math.PI / 180.0f) {flip = 1;}
                    float drawWidth = (1 - 2 * flip) * currentSparkTex.width * HUDScale * 1.5f;
                    float drawHeight = currentSparkTex.height * HUDScale * 1.5f;
                    UnityEngine.GUI.DrawTexture(new Rect(part.x - drawWidth / 2, part.y - drawHeight / 2, drawWidth, drawHeight), currentSparkTex, ScaleMode.StretchToFill, true, 0, new Color(part.color.r,part.color.g,part.color.b,part.color.a * part.timeLeft / HUDSparksFadeOutTime), 0, 0); //might have bad looking black borders at high HUD?
                }
                //time down and delete if need be
                for (int i = 0; i < particles.Count; i++)
                {
                    particleInfo part = particles[i];
                    part.timeLeft = part.timeLeft - realTimeDelta;
                    if(part.timeLeft < 0)
                    {
                        particles.RemoveAt(i);
                    }
                }
                //move particles
                for (int i = 0; i < particles.Count; i++)
                {
                    particleInfo part = particles[i];
                    part.x += part.xVel * realTimeDelta * HUDScale * 4;
                    part.y += -part.yVel * realTimeDelta * HUDScale * 4; //negative because of how DrawTexture works.
                    part.yVel += -90 * realTimeDelta * (2 * HUDTop - 1); //value of -90 is arbitrary. Gravity is opposite when HUDTop == 0.

                    if(part.x < 0 || part.x > Screen.width || part.y > Screen.height) {particles.RemoveAt(i);}
                }
            }
        }
    }

    Boolean speedupPlayedForcefully = false;
    Boolean slowdownKeyActive = false;
    public static float currentSlowdownMult = 1.0f;
    public static float timeAccumulated = 0f; //real time spent in slowdown
    public static float timeInRampup = 0.0f; //reaches up to rampUpTime, min of 0, allows for transition effect of slowdown

    Boolean slowdownEnded = false;
    public void Update()
    {
        determineTimeDelta();
        if(IsGameplayScene()) {modEnabledTemporary = true;} //inefficent
        else {modEnabledTemporary = false;}

        if(!modEnabled || !modEnabledTemporary) {return;}
                
        if(MonoSingleton<NewMovement>.Instance.hp <= 0){timeAccumulated = 0f;}

        if(keyToggleFunctionality == false)
        {
            if(Input.GetKey(slowdownCode)){slowdownKeyActive = true;}
            else{slowdownKeyActive = false;}

            if(Input.GetKeyDown(slowdownCode))
            {
                if(soundEnabled && !inMenu()) {csp.PlaySound(slowdownSound);}
                speedupPlayedForcefully = false;
            }

            if(Input.GetKeyUp(slowdownCode) && timeAccumulated < maxTimeAccumulated)
            {
                if(soundEnabled && !inMenu() && speedupPlayedForcefully == false) {csp.PlaySound(speedupSound);}
            }
            if(timeAccumulated >= maxTimeAccumulated && !speedupPlayedForcefully)
            {
                if(soundEnabled) {csp.PlaySound(speedupSound);}
                speedupPlayedForcefully = true; 
            }
        }
        if(keyToggleFunctionality == true)
        {
            if(Input.GetKeyDown(slowdownCode))
            {
                slowdownKeyActive = !slowdownKeyActive;
                if(soundEnabled) 
                {
                    if(slowdownKeyActive) {csp.PlaySound(slowdownSound);}
                    else {csp.PlaySound(speedupSound);}
                }
            }
            if(timeAccumulated >= maxTimeAccumulated) {slowdownKeyActive = false; csp.PlaySound(speedupSound);} //automatically turns off key when in toggle mode
        }

        if(MonoSingleton<NewMovement>.Instance.hp <= 0) {return;} //TimeScale is not messed with when hp <= 0 so that death slowdown happens. May cause issues potentially?
        if(Time.timeScale > slowdownMult - 0.01f && (timeInRampup >= 0 && slowdownEnded == false)) //if the time is not going super slow, and we recently pressed the button, then...
        { 
            Time.timeScale = Math.Min(currentSlowdownMult, 1); //potentially conflicts with other stuff. 
            if(timeInRampup == 0) //end behavior, so that we properly reset the timescale.
            {
                Time.timeScale = 1;
                slowdownEnded = true;
            }
        }

        if(slowdownKeyActive && timeAccumulated < maxTimeAccumulated && speedupPlayedForcefully == false)
        {
            float timeAdd = Time.deltaTime / currentSlowdownMult; 
            if(timeAdd > 0.1f){timeAdd = 0.1f;} //Helps for very low values of slowdownMult not tax a lot of charge. Otherwise impactful when you are below 10fps.
            timeAccumulated += timeAdd; 

            timeInRampup += Time.deltaTime / currentSlowdownMult;
            if (timeInRampup > rampUpTime) {timeInRampup = rampUpTime;}

            slowdownEnded = false;
        } 
        else 
        { 
            timeAccumulated += -Time.deltaTime * rechargeMultiplier / currentSlowdownMult; 

            timeInRampup += -Time.deltaTime / currentSlowdownMult;
            if (timeInRampup < 0) {timeInRampup = 0;}
        } 

        if(slowdownKeyActive && speedupPlayedForcefully == true) //this makes it so we recharge the meter after running out and then being forced out of slow mode. We are never in slow mode here.
        {
            timeAccumulated += -Time.deltaTime * rechargeMultiplier / currentSlowdownMult; 
        }

        currentSlowdownMult = slowdownMult + (1 - slowdownMult) * Math.Max((rampUpTime - timeInRampup), 0) / rampUpTime; //this is okay because rampUpTime is strictly above zero.
        if(timeAccumulated <= 0) {timeAccumulated = 0;}
    }
}
