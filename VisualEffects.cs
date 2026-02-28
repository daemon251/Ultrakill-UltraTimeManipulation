using HarmonyLib;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.XR;
using UnityEngine.U2D;

namespace UltraTimeManipulation;

[HarmonyPatch(typeof(NewMovement))]
internal class Effects
{
    [HarmonyPatch("Update")]
    [HarmonyPostfix]
    private static void Darken()
    {
        if(Plugin.modEnabled && Plugin.visualsEnabled)
        {
            float slowModeProgress = Plugin.timeInRampup / Plugin.rampUpTime;
            float effectMultiplier = slowModeProgress;

            if(Plugin.slowdownFades)
            {
                if(Plugin.speedupPlayedForcefully == true) 
                {
                    effectMultiplier = 0f; //dont do any coloring while charging after use
                }
                if(Plugin.maxTimeAccumulated - Plugin.timeAccumulated < Plugin.slowdownFadeTime) {effectMultiplier = effectMultiplier * (Plugin.maxTimeAccumulated - Plugin.timeAccumulated) / Plugin.slowdownFadeTime;}
                if(effectMultiplier > 1) {effectMultiplier = 1;}
            }

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
            int value = (int)(baseValue / Math.Pow(2.0, effectMultiplier * exponentMultiplier)); //doing it exponentionally instead of linearly gives a much better effect IMO. Ranges from 2048 to 8 with slowdownDarkness at 0
            if(value < 0) {value = 0;}
            if(value > baseValue) {value = baseValue;}
            Shader.SetGlobalInt("_ColorPrecision", value);

            float baseGamma = MonoSingleton<PrefsManager>.Instance.GetFloat("gamma");

            float value2 = baseGamma - effectMultiplier * Plugin.slowdownDarkness;

            Shader.SetGlobalFloat("_Gamma", value2);
            //Shader.SetGlobalFloat("_Outline", 4f);
            //Shader.SetGlobalFloat("_ForceOutline", 4f);
        }
    }
}

public class ShaderEffects
{
    public static Camera mainCamera;
    public static Camera hudCamera;
    public static Shader targetShader;
    public static Shader handShader;
    public static bool visualsCustomShadersEnabled = false;
    public static bool colorHands = true;
    public static bool slowdownColorsTargets = false;
    public static Color colorHandsColor = Color.black;
    public static float colorChangeDepth = 1f;
    public static float visualNoiseIntensity;
    public static float visualNoiseFrequency;
    public static float grainIntensity;
    public static float colorGrainIntensity;

    //public static RenderTexture renderTexture;
    //public static bool handColorShaderEnabled = false; //temp value
    public static Dictionary<GameObject, Material> ActiveHandShaderDict = new Dictionary<GameObject, Material>();

    [HarmonyPatch(typeof(GunControl), "ForceWeapon")]
    public class GunControlForceWeaponPatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            if(colorHands == true)
            {
                AddShaderToWeapon();
            }
        }
    }

    public static int[] getWeaponSlotVariation(GameObject weapon)
    {
        int slot = -1;
        if(MonoSingleton<GunControl>.Instance != null) {slot = MonoSingleton<GunControl>.Instance.currentSlotIndex;}
        int variation = -1;
        if(weapon != null)
        {
            if(weapon.TryGetComponent<Revolver>(out Revolver wep))
            {
                variation = wep.gunVariation;
            }
            else if(weapon.TryGetComponent<Shotgun>(out Shotgun wep2))
            {
                variation = wep2.variation;
            }
            else if(weapon.TryGetComponent<ShotgunHammer>(out ShotgunHammer wep3))
            {
                variation = wep3.variation;
            }
            else if(weapon.TryGetComponent<Nailgun>(out Nailgun wep4))
            {
                variation = wep4.variation;
                if(variation == 1) {variation = 0;}
                else if(variation == 0) {variation = 1;}
            }
            else if(weapon.TryGetComponent<Railcannon>(out Railcannon wep5))
            {
                variation = wep5.variation;
            }
            else if(weapon.TryGetComponent<RocketLauncher>(out RocketLauncher wep6))
            {
                variation = wep6.variation;
            }
        }
        int[] output = {slot, variation};
        return output;
    }
    public static List<GameObject> WeaponAlreadyShadedList = new List<GameObject>(); //needs to be cleaned
    public static List<GameObject> ArmAlreadyShadedList = new List<GameObject>(); //needs to be cleaned
    public static void AddShaderToLineRenderer(LineRenderer lr, Shader sh)
    {
        Material newMat = new Material(sh);
        Material[] importedMaterialArr = lr.GetMaterialArray();
        Material[] exportMaterialArr = new Material[importedMaterialArr.Length + 1];
        for(int i = 0; i < importedMaterialArr.Length; i++)
        {
            exportMaterialArr[i] = importedMaterialArr[i];
        }
        exportMaterialArr[importedMaterialArr.Length] = newMat;

        //newMat.SetTexture("_MainTex", importedMaterialArr[0].GetTexture("_MainTex"));
        lr.SetMaterialArray(exportMaterialArr);
    }
    public static void AddShaderToSkinnedMeshRenderer(SkinnedMeshRenderer smr, Shader sh)
    {
        Material newMat = new Material(sh);
        Material[] importedMaterialArr = smr.GetMaterialArray();
        Material[] exportMaterialArr = new Material[importedMaterialArr.Length + 1];
        for(int i = 0; i < importedMaterialArr.Length; i++)
        {
            exportMaterialArr[i] = importedMaterialArr[i];
        }
        exportMaterialArr[importedMaterialArr.Length] = newMat;

        //newMat.SetTexture("_MainTex", importedMaterialArr[0].GetTexture("_MainTex"));
        smr.SetMaterialArray(exportMaterialArr);
    }
    public static void AddShaderToMeshRenderer(MeshRenderer mr, Shader sh)
    {
        Material newMat = new Material(sh);
        Material[] importedMaterialArr = mr.GetMaterialArray();
        Material[] exportMaterialArr = new Material[importedMaterialArr.Length + 1];
        for(int i = 0; i < importedMaterialArr.Length; i++)
        {
            exportMaterialArr[i] = importedMaterialArr[i];
        }
        exportMaterialArr[importedMaterialArr.Length] = newMat;

        //newMat.SetTexture("_MainTex", importedMaterialArr[0].GetTexture("_MainTex"));
        mr.SetMaterialArray(exportMaterialArr);
    }
    public static void AddShaderToSpriteRenderer(SpriteRenderer sr, Shader sh)
    {
        Material newMat = new Material(sh);
        Material[] importedMaterialArr = sr.GetMaterialArray();
        Material[] exportMaterialArr = new Material[importedMaterialArr.Length + 1];
        for(int i = 0; i < importedMaterialArr.Length; i++)
        {
            exportMaterialArr[i] = importedMaterialArr[i];
        }
        exportMaterialArr[importedMaterialArr.Length] = newMat;

        //newMat.SetTexture("_MainTex", importedMaterialArr[0].GetTexture("_MainTex"));
        sr.SetMaterialArray(exportMaterialArr);
    }
    public static void AddShaderToParticleSystemRenderer(ParticleSystemRenderer psr, Shader sh)
    {
        Material newMat = new Material(sh);
        Material[] importedMaterialArr = psr.GetMaterialArray();
        Material[] exportMaterialArr = new Material[importedMaterialArr.Length + 1];
        for(int i = 0; i < importedMaterialArr.Length; i++)
        {
            exportMaterialArr[i] = importedMaterialArr[i];
        }
        exportMaterialArr[importedMaterialArr.Length] = newMat;

        //newMat.SetTexture("_MainTex", importedMaterialArr[0].GetTexture("_MainTex"));
        psr.SetMaterialArray(exportMaterialArr);
    }
    
    public static void AddGhostShotgunRenderer(GameObject weaponObj)
    {
        SkinnedMeshRenderer smr = null;
        GameObject renderingObject = null;
        if(weaponObj.GetComponent<Shotgun>() != null)
        {
            GameObject rig = weaponObj.transform.GetChild(2).gameObject;
            GameObject shotgun = rig.transform.GetChild(1).gameObject;
            renderingObject = shotgun;
            smr = shotgun.GetComponent<SkinnedMeshRenderer>();
        }
        else if(weaponObj.GetComponent<ShotgunHammer>() != null)
        {
            GameObject rig = weaponObj.transform.GetChild(1).gameObject;
            GameObject shotgunHammer = rig.transform.GetChild(1).gameObject;
            renderingObject = shotgunHammer;
            smr = shotgunHammer.GetComponent<SkinnedMeshRenderer>();
        }
        if(weaponObj.transform.Find("UltraTimeManipulationGhostRenderer") == null && smr != null)
        {
            //GameObject newGameObject = new GameObject("UltraTimeManipulationGhostRenderer");
            //newGameObject.transform.parent = weaponObj.transform;

            //SkinnedMeshRenderer smrNew = newGameObject.GetOrAddComponent<SkinnedMeshRenderer>();
            //smrNew = smr;
            //smrNew.enabled = true;

            GameObject newGameObject = UnityEngine.Object.Instantiate(renderingObject);
            newGameObject.name = "UltraTimeManipulationGhostRenderer";
            newGameObject.transform.parent = weaponObj.transform;

            SkinnedMeshRenderer smrNew = newGameObject.GetOrAddComponent<SkinnedMeshRenderer>();

            Material newMat = new Material(handShader);
            smrNew.material = newMat;
            for(int i = 0; i < smrNew.materials.Length; i++)
            {
                smrNew.materials[i] = newMat;
            }
        }
    }
    public static void AddShaderToWeapon()
    {
        GameObject weaponObj = MonoSingleton<GunControl>.Instance.currentWeapon;
        int[] data = getWeaponSlotVariation(weaponObj);
        int slot = data[0]; int variation = data[1];
        if(slot > 0 && weaponObj != null && WeaponAlreadyShadedList.Contains(weaponObj) == false)
        {
            if(slot == 1)
            {
                //the names are different for alt version but its fine, same process anyway
                GameObject rig = weaponObj.transform.GetChild(0).gameObject;
                GameObject RevolverBody = rig.transform.GetChild(1).gameObject;
                GameObject RevolverCylinder = rig.transform.GetChild(2).gameObject;
                GameObject RightArm = rig.transform.GetChild(3).gameObject;

                SkinnedMeshRenderer RevolverBodyRenderer = RevolverBody.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer RevolverCylinderRenderer = RevolverCylinder.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer RightArmRenderer = RightArm.GetComponent<SkinnedMeshRenderer>();

                AddShaderToSkinnedMeshRenderer(RevolverBodyRenderer, handShader);
                AddShaderToSkinnedMeshRenderer(RevolverCylinderRenderer, handShader);
                AddShaderToSkinnedMeshRenderer(RightArmRenderer, handShader);

                //fixes alt version, specifically this is the arm
                if(weaponObj.GetComponent<Revolver>().altVersion == true)
                {
                    GameObject LastObj = rig.transform.GetChild(4).gameObject;
                    SkinnedMeshRenderer LastObjRender = LastObj.GetComponent<SkinnedMeshRenderer>();
                    AddShaderToSkinnedMeshRenderer(LastObjRender, handShader);
                }
            }
            else if(slot == 2)
            {
                AddGhostShotgunRenderer(weaponObj); //far from ideal but required because shotguns and shotgunHammers have submeshes, which prevents the trick from working
                //it shouldn't cost much performance though, the renderer does the same thing regardless probably

                /*if(weaponObj.GetComponent<Shotgun>() != null)
                {
                    GameObject rig = weaponObj.transform.GetChild(2).gameObject;
                    GameObject shotgun = rig.transform.GetChild(1).gameObject;
                    SkinnedMeshRenderer smr = shotgun.GetComponent<SkinnedMeshRenderer>();
                    AddShaderToSkinnedMeshRenderer(smr, handShader);
                    AddShaderToSkinnedMeshRenderer(smr, handShader);
                    AddShaderToSkinnedMeshRenderer(smr, handShader);
                    AddShaderToSkinnedMeshRenderer(smr, handShader);
                }
                else if(weaponObj.GetComponent<ShotgunHammer>() != null)
                {
                    GameObject rig = weaponObj.transform.GetChild(1).gameObject;
                    GameObject shotgunHammer = rig.transform.GetChild(1).gameObject;
                    SkinnedMeshRenderer smr = shotgunHammer.GetComponent<SkinnedMeshRenderer>();
                    AddShaderToSkinnedMeshRenderer(smr, handShader);
                }*/
                if(variation == 2)
                {
                    //fix saw
                    MeshRenderer mrSaw = null;
                    MeshRenderer mrSawBlade = null;
                    if(weaponObj.GetComponent<Shotgun>() != null)
                    {
                        GameObject rig = weaponObj.transform.GetChild(2).gameObject;
                        GameObject armature = rig.transform.GetChild(0).gameObject;
                        GameObject mainbone = armature.transform.GetChild(0).gameObject;
                        GameObject frontbone = mainbone.transform.GetChild(0).gameObject;
                        GameObject csAttachPoint = frontbone.transform.GetChild(2).gameObject;
                        GameObject chainsaw = csAttachPoint.transform.GetChild(0).gameObject;
                        mrSaw = chainsaw.GetComponent<MeshRenderer>();

                        GameObject chainsawBlade = chainsaw.transform.GetChild(0).gameObject;
                        mrSawBlade = chainsawBlade.GetComponent<MeshRenderer>();
                    }
                    else if(weaponObj.GetComponent<ShotgunHammer>() != null)
                    {
                        GameObject rig = weaponObj.transform.GetChild(1).gameObject;
                        GameObject armature = rig.transform.GetChild(0).gameObject;
                        GameObject root = armature.transform.GetChild(0).gameObject;
                        GameObject csAttachPoint = root.transform.GetChild(5).gameObject;
                        GameObject chainsaw = csAttachPoint.transform.GetChild(0).gameObject;
                        mrSaw = chainsaw.GetComponent<MeshRenderer>();

                        GameObject chainsawBlade = chainsaw.transform.GetChild(0).gameObject;
                        mrSawBlade = chainsawBlade.GetComponent<MeshRenderer>();
                    }
                    AddShaderToMeshRenderer(mrSaw, handShader);
                    AddShaderToMeshRenderer(mrSawBlade, handShader);
                }
            }
            else if(slot == 3)
            {
                if(weaponObj.GetComponent<Nailgun>().altVersion == false)
                {
                    GameObject rig = weaponObj.transform.GetChild(0).gameObject;
                    GameObject nailgun = rig.transform.GetChild(1).gameObject;
                    SkinnedMeshRenderer smr = nailgun.GetComponent<SkinnedMeshRenderer>();
                    AddShaderToSkinnedMeshRenderer(smr, handShader);

                    GameObject armature = rig.transform.GetChild(0).gameObject;
                    GameObject main = armature.transform.GetChild(0).gameObject;

                    GameObject barrelL = main.transform.GetChild(0).gameObject;
                    GameObject barrelR = main.transform.GetChild(1).gameObject;

                    MeshRenderer smr2 = barrelL.GetComponent<MeshRenderer>();
                    AddShaderToMeshRenderer(smr2, handShader);
                    MeshRenderer smr3 = barrelR.GetComponent<MeshRenderer>();
                    AddShaderToMeshRenderer(smr3, handShader);
                }
                else
                {
                    GameObject rig = weaponObj.transform.GetChild(0).gameObject;
                    GameObject nailgun = rig.transform.GetChild(1).gameObject;
                    SkinnedMeshRenderer smr = nailgun.GetComponent<SkinnedMeshRenderer>();
                    AddShaderToSkinnedMeshRenderer(smr, handShader);

                    GameObject armature = rig.transform.GetChild(0).gameObject;
                    GameObject main = armature.transform.GetChild(0).gameObject;
                    GameObject blade = main.transform.GetChild(2).gameObject;

                    MeshRenderer mr = blade.GetComponent<MeshRenderer>();
                    AddShaderToMeshRenderer(mr, handShader);
                }
            }
            else if(slot == 4)
            {
                GameObject rig = weaponObj.transform.GetChild(0).gameObject;
                GameObject railcannon = rig.transform.GetChild(6).gameObject;
                SkinnedMeshRenderer smr = railcannon.GetComponent<SkinnedMeshRenderer>();
                AddShaderToSkinnedMeshRenderer(smr, handShader);

                GameObject bigPip = rig.transform.GetChild(1).gameObject;
                GameObject pip1 = rig.transform.GetChild(2).gameObject;
                GameObject pip2 = rig.transform.GetChild(3).gameObject;
                GameObject pip3 = rig.transform.GetChild(4).gameObject;
                GameObject pip4 = rig.transform.GetChild(5).gameObject;

                SkinnedMeshRenderer smr2 = bigPip.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer smr3 = pip1.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer smr4 = pip2.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer smr5 = pip3.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer smr6 = pip4.GetComponent<SkinnedMeshRenderer>();

                AddShaderToSkinnedMeshRenderer(smr2, handShader);
                AddShaderToSkinnedMeshRenderer(smr3, handShader);
                AddShaderToSkinnedMeshRenderer(smr4, handShader);
                AddShaderToSkinnedMeshRenderer(smr5, handShader);
                AddShaderToSkinnedMeshRenderer(smr6, handShader);

                GameObject armature = rig.transform.GetChild(0).gameObject;
                GameObject baseObj = armature.transform.GetChild(0).gameObject;
                if(weaponObj.GetComponent<Railcannon>().variation == 0)
                {
                    GameObject fullCharge = baseObj.transform.GetChild(6).gameObject;
                    GameObject particleSystem = fullCharge.transform.GetChild(0).gameObject;
                    ParticleSystemRenderer psr = particleSystem.GetComponent<ParticleSystemRenderer>();
                    //AddShaderToParticleSystemRenderer(psr, handShader);
                }
                if(weaponObj.GetComponent<Railcannon>().variation == 1)
                {
                    GameObject fullCharge = baseObj.transform.GetChild(6).gameObject;
                    GameObject particleSystem = fullCharge.transform.GetChild(0).gameObject;
                    ParticleSystemRenderer psr = particleSystem.GetComponent<ParticleSystemRenderer>();
                    //AddShaderToParticleSystemRenderer(psr, handShader);

                    GameObject spear = fullCharge.transform.GetChild(1).gameObject;
                    GameObject spearReal = spear.transform.GetChild(0).gameObject;
                    MeshRenderer mr = spearReal.GetComponent<MeshRenderer>();
                    AddShaderToMeshRenderer(mr, handShader);
                }
                if(weaponObj.GetComponent<Railcannon>().variation == 2)
                {
                    GameObject fullCharge = baseObj.transform.GetChild(0).gameObject;

                    GameObject charge2 = fullCharge.transform.GetChild(1).gameObject;
                    GameObject particleSystem = fullCharge.transform.GetChild(2).gameObject;

                    SpriteRenderer sr = charge2.GetComponent<SpriteRenderer>();
                    ParticleSystemRenderer psr = particleSystem.GetComponent<ParticleSystemRenderer>();

                    sr.material.shader = handShader;
                    sr.material.SetInt("_UltraTimeManipulationUseOpacity", 0);


                    //AddShaderToSpriteRenderer(sr, handShader);
                    //AddShaderToParticleSystemRenderer(psr, handShader);
                }
            }
            else if(slot == 5)
            {
                GameObject rig = weaponObj.transform.GetChild(0).gameObject;
                GameObject rl = rig.transform.GetChild(1).gameObject;
                SkinnedMeshRenderer smr = rl.GetComponent<SkinnedMeshRenderer>();
                AddShaderToSkinnedMeshRenderer(smr, handShader);
            }
            WeaponAlreadyShadedList.Add(weaponObj); //true or false doesnt matter really, just whether the entry exists.
        }
    }
    public static void AddShaderToArms()
    {
        GameObject weaponObj = MonoSingleton<GunControl>.Instance.currentWeapon;
        GameObject guns = weaponObj.transform.parent.gameObject;
        GameObject mainCameraObj = guns.transform.parent.gameObject;
        GameObject punch = mainCameraObj.transform.GetChild(3).gameObject;
        GameObject hookArm = punch.transform.GetChild(1).gameObject;
        GameObject blueArm = punch.transform.GetChild(2).gameObject;
        GameObject redArm = punch.transform.GetChild(3).gameObject;

        if(ArmAlreadyShadedList.Contains(hookArm) == false)
        {
            LineRenderer lr = hookArm.GetComponent<LineRenderer>();
            AddShaderToLineRenderer(lr, handShader);

            GameObject greenArmFinal = hookArm.transform.GetChild(0).gameObject;
            GameObject arm = greenArmFinal.transform.GetChild(0).gameObject;
            GameObject hook = greenArmFinal.transform.GetChild(2).gameObject;

            SkinnedMeshRenderer smr = arm.GetComponent<SkinnedMeshRenderer>();
            SkinnedMeshRenderer smr2 = hook.GetComponent<SkinnedMeshRenderer>();

            AddShaderToSkinnedMeshRenderer(smr, handShader);
            AddShaderToSkinnedMeshRenderer(smr2, handShader);
            ArmAlreadyShadedList.Add(hookArm);
        }
        if(ArmAlreadyShadedList.Contains(blueArm) == false)
        {
            GameObject feedbacker = blueArm.transform.GetChild(0).gameObject;
            GameObject feedbackerReal = feedbacker.transform.GetChild(1).gameObject;

            SkinnedMeshRenderer smr = feedbackerReal.GetComponent<SkinnedMeshRenderer>();
            AddShaderToSkinnedMeshRenderer(smr, handShader);
            ArmAlreadyShadedList.Add(blueArm);
        }
        if(ArmAlreadyShadedList.Contains(redArm) == false)
        {
            GameObject arm = redArm.transform.GetChild(0).gameObject;
            SkinnedMeshRenderer smr = arm.GetComponent<SkinnedMeshRenderer>();
            AddShaderToSkinnedMeshRenderer(smr, handShader);
            ArmAlreadyShadedList.Add(redArm);
        }
    }

    public static void enableHandColorShader()
    {
        //hudCamera.SetReplacementShader(handShader, "RenderType");
        if(colorHands == true)
        {
            AddShaderToWeapon();
            AddShaderToArms();

            //handColorShaderEnabled = true;
        }
    }
    public static void disablehandColorShader()
    {
        //hudCamera.ResetReplacementShader();
        //handColorShaderEnabled = false;
    }
    public static void LoadShader()
    {
        AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.DefaultParentFolder!, "shaderBundle.bundle"));
        targetShader = bundle.LoadAsset<Shader>("Assets/shaders/targetShader.shader");
        handShader = bundle.LoadAsset<Shader>("Assets/shaders/handShader.shader");
    }

    public static void DiscoverCamera()
    {
        GameObject gameObject1 = GameObject.Find("Player/Main Camera/HUD Camera");
        if(gameObject1 != null) 
        {
            hudCamera = gameObject1.GetComponent<Camera>();

        }
        GameObject gameObject2 = GameObject.Find("Player/Main Camera");
        if(gameObject2 != null) 
        {
            mainCamera = gameObject2.GetComponent<Camera>();
        }
    }

    //public static List<Material> materialList = new List<Material>();
    public static void UpdateShaderValues()
    {
        float slowModeProgress = Plugin.timeInRampup / Plugin.rampUpTime;
        float colorProgress = slowModeProgress;

        if(Plugin.slowdownFades)
        {
            if(Plugin.speedupPlayedForcefully == true) 
            {
                colorProgress = 0f; //dont do any coloring while charging after use
            }
            if(Plugin.maxTimeAccumulated - Plugin.timeAccumulated < Plugin.slowdownFadeTime) 
            {
                colorProgress = colorProgress * (Plugin.maxTimeAccumulated - Plugin.timeAccumulated) / Plugin.slowdownFadeTime;
            }
        }

        float baseGamma = MonoSingleton<PrefsManager>.Instance.GetFloat("gamma");
        float difference = colorProgress * Plugin.slowdownDarkness;

        Shader.SetGlobalFloat("_UltraTimeManipulationGammaCurrentAlpha", 1f);//colorProgress);
        Shader.SetGlobalFloat("_UltraTimeManipulationGammaAdjust", (baseGamma + 1f * difference) / baseGamma);

        Shader.SetGlobalFloat("_UltraTimeManipulationColorChangeAmount", colorProgress * colorChangeDepth);
        Shader.SetGlobalColor("_UltraTimeManipulationTargetTint", colorHandsColor);

        Shader.SetGlobalFloat("_UltraTimeManipulationNoiseIntensity", visualNoiseIntensity);
        Shader.SetGlobalFloat("_UltraTimeManipulationNoiseFrequency", visualNoiseFrequency);
        Shader.SetGlobalFloat("_UltraTimeManipulationGrainIntensity", grainIntensity);
        Shader.SetGlobalFloat("_UltraTimeManipulationColorGrainIntensity", colorGrainIntensity);

        Shader.SetGlobalInt("_UltraTimeManipulationUseOpacity", 1);
    }

    [HarmonyPatch(typeof(Enemy), "Start")] //husk stray schism soldier
    public class EnemyStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Enemy __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                if(name.Contains("Puppet"))
                {
                    //dont work
                    /*SkinnedMeshRenderer renderer = null;

                    GameObject rotTrans = __instance.transform.GetChild(0).gameObject;
                    GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;
                    GameObject body = bodyCenterRot.transform.GetChild(0).gameObject;
                    GameObject sph = body.transform.GetChild(1).gameObject;

                    renderer = sph.GetComponent<SkinnedMeshRenderer>();

                    renderer.material.shader = targetShader; */
                }
                else if(name.Contains("Malicious Face"))
                {
                    GameObject rig = __instance.transform.GetChild(0).gameObject;
                    GameObject face = rig.transform.GetChild(5).gameObject;

                    SkinnedMeshRenderer renderer1 = face.GetComponent<SkinnedMeshRenderer>();
                    renderer1.material.shader = targetShader; 
                }
                else if(name.Contains("Providence"))
                {
                    GameObject prov = __instance.transform.GetChild(4).gameObject;

                    GameObject go1 = prov.transform.GetChild(20).gameObject;
                    GameObject go2 = prov.transform.GetChild(21).gameObject;
                    GameObject go3 = prov.transform.GetChild(24).gameObject;

                    SkinnedMeshRenderer renderer1 = go1.GetComponent<SkinnedMeshRenderer>();
                    SkinnedMeshRenderer renderer2 = go2.GetComponent<SkinnedMeshRenderer>();
                    SkinnedMeshRenderer renderer3 = go3.GetComponent<SkinnedMeshRenderer>();

                    renderer1.material.shader = targetShader; 
                    renderer2.material.shader = targetShader; 
                    renderer3.material.shader = targetShader;
                }
                else if(name.Contains("MinosBoss"))
                {
                    //doesnt work
                    /*GameObject minos = __instance.transform.GetChild(0).gameObject;
                    GameObject minosReal = minos.transform.GetChild(2).gameObject;

                    SkinnedMeshRenderer renderer1 = minosReal.GetComponent<SkinnedMeshRenderer>();
                    renderer1.material.shader = targetShader; */
                }
                else if(name.Contains("Leviathan"))
                {
                    //boss already has special shader, oh well.
                    /*GameObject head = __instance.transform.GetChild(0).gameObject;
                    GameObject spine = head.transform.GetChild(0).gameObject;
                    GameObject arm = spine.transform.GetChild(1).gameObject;

                    SkinnedMeshRenderer renderer1 = arm.GetComponent<SkinnedMeshRenderer>();
                    renderer1.material.shader = targetShader; 

                    GameObject tail = __instance.transform.GetChild(2).gameObject;
                    GameObject spine2 = tail.transform.GetChild(0).gameObject;
                    GameObject arm2 = spine2.transform.GetChild(1).gameObject;

                    SkinnedMeshRenderer renderer2 = arm2.GetComponent<SkinnedMeshRenderer>();
                    renderer2.material.shader = targetShader; */
                }
                else if(name.Contains("Flesh Prison"))
                {
                    /*GameObject rig = __instance.transform.GetChild(1).gameObject;
                    GameObject ob = rig.transform.GetChild(1).gameObject;

                    SkinnedMeshRenderer renderer1 = ob.GetComponent<SkinnedMeshRenderer>();
                    renderer1.material.shader = targetShader; */
                }
                else if(name.Contains("Flesh Panopticon"))
                {
                    /*GameObject rig = __instance.transform.GetChild(1).gameObject;
                    GameObject ob1 = rig.transform.GetChild(1).gameObject;
                    GameObject ob2 = rig.transform.GetChild(2).gameObject;

                    SkinnedMeshRenderer renderer1 = ob1.GetComponent<SkinnedMeshRenderer>();
                    renderer1.material.shader = targetShader; 
                    SkinnedMeshRenderer renderer2 = ob2.GetComponent<SkinnedMeshRenderer>();
                    renderer2.material.shader = targetShader; */
                }
                else if(name.Contains("CentaurMortar")) //works
                {
                    GameObject mortar = __instance.transform.GetChild(1).gameObject;

                    SkinnedMeshRenderer renderer1 = mortar.GetComponent<SkinnedMeshRenderer>();
                    renderer1.material.shader = targetShader; 
                }
                else if(name.Contains("CentaurTower")) //works
                {
                    GameObject tower = __instance.transform.GetChild(1).gameObject;

                    SkinnedMeshRenderer renderer1 = tower.GetComponent<SkinnedMeshRenderer>();
                    renderer1.material.shader = targetShader; 
                }
                else if(name.Contains("CentaurRocketLauncher")) //works
                {
                    GameObject launcher = __instance.transform.GetChild(2).gameObject;

                    SkinnedMeshRenderer renderer1 = launcher.GetComponent<SkinnedMeshRenderer>();
                    renderer1.material.shader = targetShader; 
                }
                else if(__instance.IsZombie() == true)
                {
                    SkinnedMeshRenderer renderer = null;
                    Zombie zom = __instance.GetComponent<Zombie>();

                    if(name.Contains("Soldier")) //soldier
                    {
                        GameObject rotTrans = zom.transform.GetChild(0).gameObject;
                        GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;
                        GameObject model = bodyCenterRot.transform.GetChild(0).gameObject;
                        GameObject husk = model.transform.GetChild(0).gameObject;
                        renderer = husk.GetComponent<SkinnedMeshRenderer>();
                    }
                    else if(name.Contains("Filth")) // filth
                    {
                        GameObject rotTrans = zom.transform.GetChild(2).gameObject;
                        GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;
                        GameObject filth = bodyCenterRot.transform.GetChild(0).gameObject;
                        GameObject husk = filth.transform.GetChild(1).gameObject;
                        renderer = husk.GetComponent<SkinnedMeshRenderer>();
                    }
                    else //stray / schism
                    {          
                        GameObject rotTrans = zom.transform.GetChild(2).gameObject;
                        GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;
                        GameObject rig = bodyCenterRot.transform.GetChild(1).gameObject;
                        GameObject zombie = rig.transform.GetChild(1).gameObject;
                        renderer = zombie.GetComponent<SkinnedMeshRenderer>();
                        //AddShaderToSkinnedMeshRenderer(renderer, targetShader);
                    }

                    //end
                    renderer.material.shader = targetShader; 
                } 
                else if(__instance.IsStatue())
                {
                    Statue stat = __instance.GetComponent<Statue>();

                    GameObject rotTrans = stat.transform.GetChild(0).gameObject;
                    GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;

                    GameObject cerb = bodyCenterRot.transform.GetChild(0).gameObject;
                    GameObject apple = cerb.transform.GetChild(1).gameObject;
                    GameObject body = cerb.transform.GetChild(2).gameObject;

                    SkinnedMeshRenderer renderer1 = apple.GetComponent<SkinnedMeshRenderer>(); 
                    SkinnedMeshRenderer renderer2 = body.GetComponent<SkinnedMeshRenderer>(); 
                    //missing apple glow or wtv

                    renderer1.material.shader = targetShader; 
                    renderer2.material.shader = targetShader; 
                }
                else if(__instance.name.Contains("Power"))
                {
                    GameObject power = __instance.transform.GetChild(0).gameObject;

                    GameObject go1 = power.transform.GetChild(1).gameObject;
                    GameObject go2 = power.transform.GetChild(2).gameObject;

                    SkinnedMeshRenderer renderer1 = go1.GetComponent<SkinnedMeshRenderer>(); 
                    SkinnedMeshRenderer renderer2 = go2.GetComponent<SkinnedMeshRenderer>(); 

                    renderer1.material.shader = targetShader; 
                    renderer2.material.shader = targetShader; 
                }
                //panopticon
            }
        }
    }

    [HarmonyPatch(typeof(Stalker), "Start")] //stalker
    public class StalkerStartPatch // works
    {
        [HarmonyPostfix]
        private static void Postfix(Stalker __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                //SkinnedMeshRenderer renderer = null;

                GameObject rotTrans = __instance.transform.GetChild(0).gameObject;
                GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;

                GameObject stalker = bodyCenterRot.transform.GetChild(1).gameObject;

                GameObject go1 = stalker.transform.GetChild(1).gameObject;
                GameObject go2 = stalker.transform.GetChild(2).gameObject;
                GameObject go3 = stalker.transform.GetChild(3).gameObject;
                GameObject go4 = stalker.transform.GetChild(4).gameObject;

                SkinnedMeshRenderer renderer1 = go1.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer renderer2 = go2.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer renderer3 = go3.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer renderer4 = go4.GetComponent<SkinnedMeshRenderer>();

                renderer1.material.shader = targetShader; 
                renderer2.material.shader = targetShader; 
                renderer3.material.shader = targetShader; 
                renderer4.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(Sisyphus), "Start")] //insurrectionist
    public class SisyphusStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Sisyphus __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;

                GameObject armStretchRig = __instance.transform.GetChild(0).gameObject;
                GameObject body = armStretchRig.transform.GetChild(0).gameObject;
                SkinnedMeshRenderer renderer1 = body.GetComponent<SkinnedMeshRenderer>();
                renderer1.material.shader = targetShader; 

                GameObject armStretchRig2 = __instance.transform.GetChild(2).gameObject;
                GameObject body2 = armStretchRig2.transform.GetChild(0).gameObject;
                SkinnedMeshRenderer renderer2 = body2.GetComponent<SkinnedMeshRenderer>();
                renderer2.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(Ferryman), "Start")] //ferryman
    public class FerrymanStartPatch //fix cloth -- works
    {
        [HarmonyPostfix]
        private static void Postfix(Ferryman __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;

                GameObject rotTrans = __instance.transform.GetChild(0).gameObject;
                GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;

                GameObject ferryman2 = bodyCenterRot.transform.GetChild(0).gameObject;

                GameObject ferrymanReal = ferryman2.transform.GetChild(1).gameObject;
                GameObject oar = ferryman2.transform.GetChild(2).gameObject;

                SkinnedMeshRenderer renderer1 = ferrymanReal.GetComponent<SkinnedMeshRenderer>();
                SkinnedMeshRenderer renderer2 = oar.GetComponent<SkinnedMeshRenderer>();

                renderer1.material.shader = targetShader; 
                renderer1.materials[0].shader = targetShader; 
                renderer1.materials[1].shader = targetShader; //fixes cloth

                renderer2.material.shader = targetShader; 
            }
        }
    }
    
    [HarmonyPatch(typeof(SwordsMachine), "Start")] //swordsmachine
    public class SwordsMachineStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(SwordsMachine __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;

                GameObject rotTrans = __instance.transform.GetChild(0).gameObject;
                GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;

                GameObject swordsmachineReal = bodyCenterRot.transform.GetChild(0).gameObject;

                GameObject armature = swordsmachineReal.transform.GetChild(0).gameObject;
                GameObject control = armature.transform.GetChild(0).gameObject;
                GameObject waist = control.transform.GetChild(2).gameObject; //are we there yet?
                GameObject chest = waist.transform.GetChild(0).gameObject;
                GameObject armRight = chest.transform.GetChild(2).gameObject; //are we there yet?
                GameObject armRight2 = armRight.transform.GetChild(0).gameObject;
                GameObject shotgun = armRight2.transform.GetChild(1).gameObject; //are we there yet?
                GameObject shotgunReal = shotgun.transform.GetChild(2).gameObject;
                SkinnedMeshRenderer renderer1 = shotgunReal.GetComponent<SkinnedMeshRenderer>(); //omg

                GameObject sword = swordsmachineReal.transform.GetChild(1).gameObject;
                SkinnedMeshRenderer renderer2 = sword.GetComponent<SkinnedMeshRenderer>(); 
                GameObject rig = swordsmachineReal.transform.GetChild(2).gameObject;
                SkinnedMeshRenderer renderer3 = rig.GetComponent<SkinnedMeshRenderer>(); 

                renderer1.material.shader = targetShader; 
                renderer2.material.shader = targetShader; 
                renderer3.material.shader = targetShader; 
            }
        }
    }
    
    [HarmonyPatch(typeof(Drone), "Start")] //drone virtue
    public class DroneStartPatch //virute broken -- fix
    {
        [HarmonyPostfix]
        private static void Postfix(Drone __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                if(name.Contains("Drone"))
                {
                    GameObject drone = __instance.transform.GetChild(2).gameObject;
                    GameObject body = drone.transform.GetChild(0).gameObject;
                    GameObject visor = drone.transform.GetChild(1).gameObject;

                    MeshRenderer renderer1 = body.GetComponent<MeshRenderer>(); 
                    MeshRenderer renderer2 = visor.GetComponent<MeshRenderer>(); 

                    renderer1.material.shader = targetShader; 
                    renderer2.material.shader = targetShader; 
                }
                else if(name.Contains("Virtue"))
                {
                    //the shaders used here are funky, cant just be replaced
                    /*
                    GameObject virtue = __instance.transform.GetChild(0).gameObject;
                    GameObject main = virtue.transform.GetChild(1).gameObject;

                    for(int i = 0; i < 8; i++)
                    {
                        GameObject obj = main.transform.GetChild(i).gameObject;
                        MeshRenderer renderer = obj.GetComponent<MeshRenderer>(); 

                        renderer.material.shader = targetShader; 
                    }

                    GameObject wing = virtue.transform.GetChild(2).gameObject;
                    SkinnedMeshRenderer renderer2 = wing.GetComponent<SkinnedMeshRenderer>(); 
                    renderer2.material.shader = targetShader; 
                    */
                }
            }
        }
    }

    [HarmonyPatch(typeof(Streetcleaner), "Start")] //streetcleaner
    public class StreetcleanerStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Streetcleaner __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;

                GameObject rotTrans = __instance.transform.GetChild(0).gameObject;
                GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;

                GameObject flameboi = bodyCenterRot.transform.GetChild(0).gameObject;
                GameObject m_streetrenderer = flameboi.transform.GetChild(1).gameObject;
                renderer = m_streetrenderer.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(V2), "Start")] //v2
    public class V2StartPatch // fix
    {
        [HarmonyPostfix]
        private static void Postfix(V2 __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;

                GameObject v2Comb = __instance.transform.GetChild(0).gameObject;
                GameObject mdl = v2Comb.transform.GetChild(0).gameObject;

                renderer = mdl.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(Mindflayer), "Start")] //mindflayer
    public class MindflayerStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Mindflayer __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;

                GameObject center = __instance.transform.GetChild(0).gameObject;
                GameObject rig = center.transform.GetChild(0).gameObject;
                GameObject model = rig.transform.GetChild(1).gameObject;
                renderer = model.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(Turret), "Start")] //sentry
    public class TurretStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Turret __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;

                GameObject rotTrans = __instance.transform.GetChild(0).gameObject;
                GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;

                GameObject turretBot = bodyCenterRot.transform.GetChild(0).gameObject;
                GameObject model = turretBot.transform.GetChild(1).gameObject;
                renderer = model.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(Guttertank), "Start")] //guttertank
    public class GuttertankStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Guttertank __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;

                GameObject rotTrans = __instance.transform.GetChild(0).gameObject;
                GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;

                GameObject gutterTank = bodyCenterRot.transform.GetChild(0).gameObject;
                GameObject model = gutterTank.transform.GetChild(1).gameObject;

                SkinnedMeshRenderer renderer = model.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(Gutterman), "Start")] //gutterman
    public class GuttermanStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Gutterman __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;

                GameObject rotTrans = __instance.transform.GetChild(0).gameObject;
                GameObject bodyCenterRot = rotTrans.transform.GetChild(0).gameObject;

                GameObject gutterTank = bodyCenterRot.transform.GetChild(0).gameObject;
                GameObject door = gutterTank.transform.GetChild(1).gameObject;
                GameObject mainMdl = gutterTank.transform.GetChild(2).gameObject;
                GameObject shield = gutterTank.transform.GetChild(3).gameObject;

                SkinnedMeshRenderer renderer1 = door.GetComponent<SkinnedMeshRenderer>(); 
                SkinnedMeshRenderer renderer2 = mainMdl.GetComponent<SkinnedMeshRenderer>(); 
                SkinnedMeshRenderer renderer3 = shield.GetComponent<SkinnedMeshRenderer>(); 

                renderer1.material.shader = targetShader; 
                renderer2.material.shader = targetShader; 
                renderer3.material.shader = targetShader; 
            }
        }
    }

    //defense system guys
    /*
    [HarmonyPatch(typeof(SpiderBody), "Start")] //malface
    public class SpiderBodyStartPatch //broken
    {
        [HarmonyPostfix]
        private static void Postfix(SpiderBody __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;

                GameObject body = __instance.transform.GetChild(0).gameObject;

                GameObject malFace = body.transform.GetChild(0).gameObject;
                GameObject model = malFace.transform.GetChild(5).gameObject;
                renderer = model.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 

                for(int i = 0; i < 4; i++)
                {
                    GameObject leg = __instance.transform.GetChild(2 + i).gameObject;
                    MeshRenderer mr = leg.GetComponent<MeshRenderer>();
                    mr.material.shader = targetShader; 
                }
            }
        }
    }
    */
    /*[HarmonyPatch(typeof(SpiderLegLines), "Start")] //malface
    public class SpiderLegLinesStartPatch //broken
    {
        [HarmonyPostfix]
        private static void Postfix(SpiderLegLines __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                MeshRenderer renderer = null;
                renderer = __instance.GetComponent<MeshRenderer>(); 

                renderer.material.shader = targetShader; 
            }
        }
    }
    */

    [HarmonyPatch(typeof(Mass), "Start")] //hid mass
    public class MassStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Mass __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;

                GameObject mass = __instance.transform.GetChild(0).gameObject;

                GameObject body = mass.transform.GetChild(1).gameObject;
                GameObject spear = mass.transform.GetChild(2).gameObject;

                SkinnedMeshRenderer renderer1 = body.GetComponent<SkinnedMeshRenderer>(); 
                SkinnedMeshRenderer renderer2 = spear.GetComponent<SkinnedMeshRenderer>(); 

                renderer1.material.shader = targetShader; 
                renderer2.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(Idol), "Awake")] 
    public class IdolAwakePatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Idol __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                MeshRenderer renderer = null;

                GameObject idol = __instance.transform.GetChild(0).gameObject;
                GameObject model = idol.transform.GetChild(0).gameObject;
                renderer = model.GetComponent<MeshRenderer>(); 

                renderer.material.shader = targetShader; 

                GameObject halo = __instance.transform.GetChild(2).gameObject;
                GameObject spriteObj = halo.transform.GetChild(0).gameObject;
                GameObject particleEffects = halo.transform.GetChild(1).gameObject;
                GameObject mainParticleEffect = particleEffects.transform.GetChild(0).gameObject;
                //now do stuff with these
            }
        }
    }

    [HarmonyPatch(typeof(Mannequin), "Start")] //mannequin
    public class MannequinStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Mannequin __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;

                GameObject man = __instance.transform.GetChild(0).gameObject;
                GameObject model = man.transform.GetChild(1).gameObject;
                renderer = model.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(Minotaur), "Start")] //minotaur
    public class MinotaurStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Minotaur __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;

                GameObject minotaur = __instance.transform.GetChild(0).gameObject;

                GameObject obj1 = minotaur.transform.GetChild(1).gameObject;
                GameObject obj2 = minotaur.transform.GetChild(2).gameObject;

                SkinnedMeshRenderer renderer1 = obj1.GetComponent<SkinnedMeshRenderer>(); 
                SkinnedMeshRenderer renderer2 = obj2.GetComponent<SkinnedMeshRenderer>(); 
                renderer1.material.shader = targetShader; 
                renderer2.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(Gabriel), "Start")] //Gabriel
    public class GabrielStartPatch //fix gabe enraged
    {
        [HarmonyPostfix]
        private static void Postfix(Gabriel __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;

                GameObject gabeRig = __instance.transform.GetChild(0).gameObject;
                GameObject mdl = gabeRig.transform.GetChild(0).gameObject;
                GameObject mdl2 = gabeRig.transform.GetChild(1).gameObject;
                GameObject mdl3 = gabeRig.transform.GetChild(2).gameObject;

                renderer = mdl.GetComponent<SkinnedMeshRenderer>(); 
                SkinnedMeshRenderer renderer1 = mdl2.GetComponent<SkinnedMeshRenderer>(); 
                SkinnedMeshRenderer renderer2 = mdl3.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 
                renderer1.material.shader = targetShader; 
                renderer2.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(MinosBoss), "Start")] //minos 
    public class MinosBossStartPatch //idk
    {
        [HarmonyPostfix]
        private static void Postfix(MinosBoss __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;


                renderer.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(MinosArm), "Start")] //minos 
    public class MinosArmStartPatch //idk
    {
        [HarmonyPostfix]
        private static void Postfix(MinosArm __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;


                renderer.material.shader = targetShader; 
            }
        }
    }

    //minos prime cube

    //sisy cube

    [HarmonyPatch(typeof(MinosPrime), "Start")] //minos prime
    public class MinosPrimeStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(MinosPrime __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;
                SkinnedMeshRenderer renderer1 = null;
                SkinnedMeshRenderer renderer2 = null;

                GameObject model = __instance.transform.GetChild(0).gameObject;

                GameObject part1 = model.transform.GetChild(1).gameObject;
                GameObject part2 = model.transform.GetChild(1).gameObject;
                GameObject part3 = model.transform.GetChild(1).gameObject;

                renderer = part1.GetComponent<SkinnedMeshRenderer>(); 
                renderer1 = part2.GetComponent<SkinnedMeshRenderer>(); 
                renderer2 = part3.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 
                renderer1.material.shader = targetShader; 
                renderer2.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(SisyphusPrime), "Start")] //sisy prime
    public class SisyphusPrimeStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(SisyphusPrime __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;
                SkinnedMeshRenderer renderer1 = null;
                SkinnedMeshRenderer renderer2 = null;

                GameObject model = __instance.transform.GetChild(0).gameObject;

                GameObject part1 = model.transform.GetChild(2).gameObject;
                GameObject part2 = model.transform.GetChild(6).gameObject;
                GameObject part3 = model.transform.GetChild(7).gameObject;

                renderer = part1.GetComponent<SkinnedMeshRenderer>(); 
                renderer1 = part2.GetComponent<SkinnedMeshRenderer>(); 
                renderer2 = part3.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 
                renderer1.material.shader = targetShader; 
                renderer2.material.shader = targetShader; 
            }
        }
    }

    [HarmonyPatch(typeof(Projectile), "Start")] 
    public class ProjectileStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Projectile __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                MeshRenderer renderer1 = __instance.GetComponent<MeshRenderer>(); 
                renderer1.material.shader = targetShader; 
            }
        }
    }

    //providence

    //geryon

    //deathcatcher

    [HarmonyPatch(typeof(Deathcatcher), "Start")] //sisy prime
    public class DeathcatcherStartPatch //works
    {
        [HarmonyPostfix]
        private static void Postfix(Deathcatcher __instance)
        {
            if(slowdownColorsTargets)
            {
                string name = __instance.name;
                SkinnedMeshRenderer renderer = null;

                GameObject model = __instance.transform.GetChild(0).gameObject;
                GameObject go = model.transform.GetChild(0).gameObject;
                GameObject final = go.transform.GetChild(3).gameObject;
                GameObject final2 = go.transform.GetChild(2).gameObject;
                GameObject final3 = go.transform.GetChild(1).gameObject;

                renderer = final.GetComponent<SkinnedMeshRenderer>(); 
                SkinnedMeshRenderer renderer2 = final2.GetComponent<SkinnedMeshRenderer>(); 
                SkinnedMeshRenderer renderer3 = final3.GetComponent<SkinnedMeshRenderer>(); 

                renderer.material.shader = targetShader; 
                renderer2.material.shader = targetShader; 
                renderer3.material.shader = targetShader; 

            }
        }
    }
    

    //leviathan

    //skulls

}
