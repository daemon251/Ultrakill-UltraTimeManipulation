using System.IO;
using UnityEngine;

using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using PluginConfig.API.Functionals;
using ULTRAKILL.Cheats;

namespace UltraTimeManipulation;

public class PluginConfig
{
    public enum KeyEnum
    {
        Backspace, Tab, Escape, Space, UpArrow, DownArrow, RightArrow, LeftArrow, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, 
        Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9, Alpha0, CapsLock,
        RightShift, LeftShift, RightControl, LeftControl, RightAlt, LeftAlt, Mouse1, Mouse2, Mouse3, Mouse4, Mouse5, Mouse6, Mouse7
    }
    public static KeyCode convertKeyEnumToKeyCode(KeyEnum value)
    {
        KeyCode code = KeyCode.None;
        if (value.Equals(KeyEnum.Mouse4)) {code = KeyCode.Mouse3;} //default
        else if (value.Equals(KeyEnum.Backspace)) {code = KeyCode.Backspace;}
        else if (value.Equals(KeyEnum.Tab)) {code = KeyCode.Tab;}
        else if (value.Equals(KeyEnum.Escape)) {code = KeyCode.Escape;}
        else if (value.Equals(KeyEnum.Space)) {code = KeyCode.Space;}
        else if (value.Equals(KeyEnum.UpArrow)) {code = KeyCode.UpArrow;}
        else if (value.Equals(KeyEnum.DownArrow)) {code = KeyCode.DownArrow;}
        else if (value.Equals(KeyEnum.RightArrow)) {code = KeyCode.RightArrow;}
        else if (value.Equals(KeyEnum.LeftArrow)) {code = KeyCode.LeftArrow;}
        else if (value.Equals(KeyEnum.A)) {code = KeyCode.A;}
        else if (value.Equals(KeyEnum.B)) {code = KeyCode.B;}
        else if (value.Equals(KeyEnum.C)) {code = KeyCode.C;}
        else if (value.Equals(KeyEnum.D)) {code = KeyCode.D;}
        else if (value.Equals(KeyEnum.E)) {code = KeyCode.E;}
        else if (value.Equals(KeyEnum.F)) {code = KeyCode.F;}
        else if (value.Equals(KeyEnum.G)) {code = KeyCode.G;}
        else if (value.Equals(KeyEnum.H)) {code = KeyCode.H;}
        else if (value.Equals(KeyEnum.I)) {code = KeyCode.I;}
        else if (value.Equals(KeyEnum.J)) {code = KeyCode.J;}
        else if (value.Equals(KeyEnum.K)) {code = KeyCode.K;}
        else if (value.Equals(KeyEnum.L)) {code = KeyCode.L;}
        else if (value.Equals(KeyEnum.M)) {code = KeyCode.M;}
        else if (value.Equals(KeyEnum.N)) {code = KeyCode.N;}
        else if (value.Equals(KeyEnum.O)) {code = KeyCode.O;}
        else if (value.Equals(KeyEnum.P)) {code = KeyCode.P;}
        else if (value.Equals(KeyEnum.Q)) {code = KeyCode.Q;}
        else if (value.Equals(KeyEnum.R)) {code = KeyCode.R;}
        else if (value.Equals(KeyEnum.S)) {code = KeyCode.S;}
        else if (value.Equals(KeyEnum.T)) {code = KeyCode.T;}
        else if (value.Equals(KeyEnum.U)) {code = KeyCode.U;}
        else if (value.Equals(KeyEnum.V)) {code = KeyCode.V;}
        else if (value.Equals(KeyEnum.W)) {code = KeyCode.W;}
        else if (value.Equals(KeyEnum.X)) {code = KeyCode.X;}
        else if (value.Equals(KeyEnum.Y)) {code = KeyCode.Y;}
        else if (value.Equals(KeyEnum.Z)) {code = KeyCode.Z;}
        else if (value.Equals(KeyEnum.Alpha1)) {code = KeyCode.Alpha1;}
        else if (value.Equals(KeyEnum.Alpha2)) {code = KeyCode.Alpha2;}
        else if (value.Equals(KeyEnum.Alpha3)) {code = KeyCode.Alpha3;}
        else if (value.Equals(KeyEnum.Alpha4)) {code = KeyCode.Alpha4;}
        else if (value.Equals(KeyEnum.Alpha5)) {code = KeyCode.Alpha5;}
        else if (value.Equals(KeyEnum.Alpha6)) {code = KeyCode.Alpha6;}
        else if (value.Equals(KeyEnum.Alpha7)) {code = KeyCode.Alpha7;}
        else if (value.Equals(KeyEnum.Alpha8)) {code = KeyCode.Alpha8;}
        else if (value.Equals(KeyEnum.Alpha9)) {code = KeyCode.Alpha9;}
        else if (value.Equals(KeyEnum.Alpha0)) {code = KeyCode.Alpha0;}
        else if (value.Equals(KeyEnum.CapsLock)) {code = KeyCode.CapsLock;}
        else if (value.Equals(KeyEnum.RightShift)) {code = KeyCode.RightShift;}
        else if (value.Equals(KeyEnum.LeftShift)) {code = KeyCode.LeftShift;}
        else if (value.Equals(KeyEnum.RightControl)) {code = KeyCode.RightControl;}
        else if (value.Equals(KeyEnum.LeftControl)) {code = KeyCode.LeftControl;}
        else if (value.Equals(KeyEnum.RightAlt)) {code = KeyCode.RightAlt;}
        else if (value.Equals(KeyEnum.LeftAlt)) {code = KeyCode.LeftAlt;}
        else if (value.Equals(KeyEnum.Mouse1)) {code = KeyCode.Mouse0;} //these dont line up
        else if (value.Equals(KeyEnum.Mouse2)) {code = KeyCode.Mouse1;}
        else if (value.Equals(KeyEnum.Mouse3)) {code = KeyCode.Mouse2;}
        else if (value.Equals(KeyEnum.Mouse4)) {code = KeyCode.Mouse3;}
        else if (value.Equals(KeyEnum.Mouse6)) {code = KeyCode.Mouse5;}
        else if (value.Equals(KeyEnum.Mouse7)) {code = KeyCode.Mouse6;}
        return code;
    }

    public enum QuadrantEnum {One, Two, Three, Four}
    public static int convertQuadrantEnumToInt(QuadrantEnum value)
    {
        int code = 0;
        if (value.Equals(QuadrantEnum.One)) {code = 1;} //default
        else if (value.Equals(QuadrantEnum.Two)) {code = 2;}
        else if (value.Equals(QuadrantEnum.Three)) {code = 3;}
        else if (value.Equals(QuadrantEnum.Four)) {code = 4;}
        return code;
    }

    public enum IntensityEnum {None, VeryLight, Light, Medium, MediumHeavy, Heavy, VeryHeavy, Nightmare}
    public static float convertIntensityEnumToColorCompressionFloat(IntensityEnum value)
    {
        float code = 0.0f;
        if(value.Equals(IntensityEnum.None)){code = -20f;}
        if(value.Equals(IntensityEnum.VeryLight)){code = -0.5f;}
        if(value.Equals(IntensityEnum.Light)){code = 0f;}
        if(value.Equals(IntensityEnum.Medium)){code = 0.2f;}
        if(value.Equals(IntensityEnum.MediumHeavy)){code = 0.4f;}
        if(value.Equals(IntensityEnum.Heavy)){code = 0.6f;}
        if(value.Equals(IntensityEnum.VeryHeavy)){code = 1.0f;}
        if(value.Equals(IntensityEnum.Nightmare)){code = 1.5f;}

        return code;
    }

    public static float convertIntensityEnumToGammaFloat(IntensityEnum value)
    {
        float code = 0.0f;
        if(value.Equals(IntensityEnum.None)){code = 0f;}
        if(value.Equals(IntensityEnum.VeryLight)){code = 0.1f;}
        if(value.Equals(IntensityEnum.Light)){code = 0.25f;}
        if(value.Equals(IntensityEnum.Medium)){code = 0.4f;}
        if(value.Equals(IntensityEnum.MediumHeavy)){code = 0.6f;}
        if(value.Equals(IntensityEnum.Heavy)){code = 0.8f;}
        if(value.Equals(IntensityEnum.VeryHeavy)){code = 1.0f;}
        if(value.Equals(IntensityEnum.Nightmare)){code = 1.5f;}

        return code;
    }

    public static void OpenSoundFolder() {Application.OpenURL(Plugin.DefaultSoundFolder);}

    public static void OpenImageFolder() {Application.OpenURL(Plugin.DefaultImageFolder);}

    public static void UltraTimeManipulationConfig()
    {
        var config = PluginConfigurator.Create("UltraTimeManipulation", "UltraTimeManipulation");
        config.SetIconWithURL($"{Path.Combine(Plugin.DefaultParentFolder!, "icon.png")}");

        ConfigHeader warningHeader = new ConfigHeader(config.rootPanel, "This mod disables score submissions when enabled.");
        warningHeader.textSize = 20;
        warningHeader.textColor = Color.red;
        BoolField enabledField = new BoolField(config.rootPanel, "Mod Enabled", "modEnabled", true);

        ConfigDivision division = new ConfigDivision(config.rootPanel, "division");
        enabledField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.modEnabled = e.value; division.interactable = e.value; Time.timeScale = 1.0f;};
        Plugin.modEnabled = enabledField.value; division.interactable = enabledField.value; Time.timeScale = 1.0f;

        //-----\\
        //SOUND\\
        //-----\\
        ConfigPanel sfxPanel = new ConfigPanel(division, "Sounds", "sfxPanel");

        FloatField volumeField = new FloatField(sfxPanel, "Volume Multiplier", "volumeMultiplier", 0.7f, 0.00f, 1.00f);
        volumeField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.volumeMult = e.value;};
        Plugin.volumeMult = volumeField.value;

        BoolField soundsField = new BoolField(sfxPanel, "Sounds Enabled", "soundsEnabled", true);
        soundsField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.soundEnabled = e.value;};
        Plugin.soundEnabled = soundsField.value;

        ButtonField openSoundsFolderField = new ButtonField(sfxPanel, "Open Sounds Folder", "button.openfolder");
        openSoundsFolderField.onClick += new ButtonField.OnClick(OpenSoundFolder);

        BoolField soundSlowdownField = new BoolField(sfxPanel, "Sound Slowdown Enabled", "soundSlowdownEnabled", true);
        soundSlowdownField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.soundSlowdownEnabled = e.value;};
        Plugin.soundSlowdownEnabled = soundSlowdownField.value;

        FloatField soundSlowdownAmountField = new FloatField(sfxPanel, "Sound Slowdown Amount", "soundSlowdownAmount", 0.4f, 0.01f, 1.00f);
        soundSlowdownAmountField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.soundSlowDown = e.value;};
        Plugin.soundSlowDown = soundSlowdownAmountField.value;

        FloatField soundSlowdownRelativePitchField = new FloatField(sfxPanel, "Sound Slowdown Relative Pitch", "soundSlowdownRelativePitch", 1.0f, 0.01f, 5.00f);
        soundSlowdownRelativePitchField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.soundSlowDownRelativePitch = e.value;};
        Plugin.soundSlowDownRelativePitch = soundSlowdownRelativePitchField.value;

        //-------\\
        //VISUALS\\
        //-------\\
        ConfigPanel visualsPanel = new ConfigPanel(division, "Visuals", "visualsPanel");

        BoolField visualsField = new BoolField(visualsPanel, "Visuals Enabled", "visualsEnabled", true);
        visualsField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.visualsEnabled = e.value;};
        Plugin.visualsEnabled = visualsField.value;

        //this is an enum because the floatField version was very unintuitive
        EnumField<IntensityEnum> slowdownColorCompressionField = new EnumField<IntensityEnum>(visualsPanel, "Slowdown Color Compression", "slowdownColorCompression", IntensityEnum.Medium);
        slowdownColorCompressionField.SetEnumDisplayName(IntensityEnum.VeryLight, "Very Light");
        slowdownColorCompressionField.SetEnumDisplayName(IntensityEnum.MediumHeavy, "Medium-Heavy");
        slowdownColorCompressionField.SetEnumDisplayName(IntensityEnum.VeryHeavy, "Very Heavy");
        slowdownColorCompressionField.onValueChange += (EnumField<IntensityEnum>.EnumValueChangeEvent e) => {Plugin.slowdownColorCompression = convertIntensityEnumToColorCompressionFloat(e.value);};
        Plugin.slowdownColorCompression = convertIntensityEnumToColorCompressionFloat(slowdownColorCompressionField.value);

        EnumField<IntensityEnum> slowdownDarknessField = new EnumField<IntensityEnum>(visualsPanel, "Slowdown Gamma Reduction", "slowdownDarkness", IntensityEnum.Medium);
        slowdownDarknessField.SetEnumDisplayName(IntensityEnum.VeryLight, "Very Light");
        slowdownDarknessField.SetEnumDisplayName(IntensityEnum.MediumHeavy, "Medium-Heavy");
        slowdownDarknessField.SetEnumDisplayName(IntensityEnum.VeryHeavy, "Very Heavy");
        slowdownDarknessField.onValueChange += (EnumField<IntensityEnum>.EnumValueChangeEvent e) => {Plugin.slowdownDarkness = convertIntensityEnumToGammaFloat(e.value);};
        Plugin.slowdownDarkness = convertIntensityEnumToGammaFloat(slowdownDarknessField.value);

        //-----\\
        //INPUT\\
        //-----\\
        ConfigPanel inputPanel = new ConfigPanel(division, "Input", "inputPanel");

        EnumField<KeyEnum> keyIDField = new EnumField<KeyEnum>(inputPanel, "Slowdown Key", "keyID", KeyEnum.Mouse4);
        keyIDField.onValueChange += (EnumField<KeyEnum>.EnumValueChangeEvent e) => {Plugin.slowdownCode = convertKeyEnumToKeyCode(e.value);};
        Plugin.slowdownCode = convertKeyEnumToKeyCode(keyIDField.value);

        BoolField keyToggleFunctionalityField = new BoolField(inputPanel, "Toggle Instead Of Hold", "keyToggleFunctionality", false);
        keyToggleFunctionalityField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.keyToggleFunctionality = e.value; Debug.Log(e.value);};
        Plugin.keyToggleFunctionality = keyToggleFunctionalityField.value;

        //---\\
        //HUD\\
        //---\\
        ConfigPanel HUDPanel = new ConfigPanel(division, "HUD", "HUDPanel");

        FloatField HUDScaleField = new FloatField(HUDPanel, "HUD Scale", "HUDScale", 1.0f, 0.01f, 20.0f);
        HUDScaleField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.HUDScale = e.value / 4;}; //scaled down 4x. Done to make scaling better.
        Plugin.HUDScale = HUDScaleField.value / 4;

        EnumField<QuadrantEnum> HUDQuadrantField = new EnumField<QuadrantEnum>(HUDPanel, "HUD Quadrant", "HUDQuadrant", QuadrantEnum.Two);
        HUDQuadrantField.onValueChange += (EnumField<QuadrantEnum>.EnumValueChangeEvent e) => {Plugin.HUDQuadrant = convertQuadrantEnumToInt(e.value);};
        Plugin.HUDQuadrant = convertQuadrantEnumToInt(HUDQuadrantField.value);

        BoolField minimalHUDField = new BoolField(HUDPanel, "Minimal HUD", "minimalHUD", false);
        minimalHUDField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.minimalHUD = e.value;};
        Plugin.minimalHUD = minimalHUDField.value;

        BoolField flashingHUDField = new BoolField(HUDPanel, "Slowdown HUD Flickering", "flashingHUD", true);
        flashingHUDField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.HUDFlashing = e.value;};
        Plugin.HUDFlashing = flashingHUDField.value;

        BoolField HUDSparksField = new BoolField(HUDPanel, "Slowdown HUD Sparks", "sparkingHUD", true);
        HUDSparksField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.HUDSparks = e.value;};
        Plugin.HUDSparks = HUDSparksField.value;

        FloatField HUDSparksLifetimeField = new FloatField(HUDPanel, "Slowdown HUD Sparks Lifetime", "sparkingLifetimeHUD", 0.8f, 0.01f, 10.0f);
        HUDSparksLifetimeField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.HUDSparksFadeOutTime = e.value;}; 
        Plugin.HUDSparksFadeOutTime = HUDSparksLifetimeField.value;

        new ConfigSpace(HUDPanel, 10f);
        new ConfigHeader(HUDPanel, "Legacy HUD should never be used, and doesn't work with any of the above options. It also looks bad.");

        BoolField legacyHUDField = new BoolField(HUDPanel, "Legacy HUD (looks bad)", "legacyUI", false);
        legacyHUDField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.legacyDisplay = e.value;};
        Plugin.legacyDisplay = legacyHUDField.value;

        ButtonField openVisualsFolderField = new ButtonField(HUDPanel, "Open HUD Folder", "button.openfolder");
        openVisualsFolderField.onClick += new ButtonField.OnClick(OpenImageFolder);

        //---------------\\
        //SLOWDOWN EFFECT\\
        //---------------\\
        ConfigPanel slowdownEffectPanel = new ConfigPanel(division, "Slowdown Effect", "slowdownEffectPanel");

        FloatField slowdownMultiplierField = new FloatField(slowdownEffectPanel, "Slowdown Speed Multiplier", "slowdownMultiplier", 0.4f, 0.01f, 1.00f); //should the max really be 1.00?
        slowdownMultiplierField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.slowdownMult = e.value;};
        Plugin.slowdownMult = slowdownMultiplierField.value;

        FloatField maxTimeField = new FloatField(slowdownEffectPanel, "Slowdown Max Duration (s)", "maxTime", 6.0f, 0.05f, 1000000000f); //max value of a billion seconds... enough.
        maxTimeField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.maxTimeAccumulated = e.value;};
        Plugin.maxTimeAccumulated = maxTimeField.value;

        FloatField rampUpTimeField = new FloatField(slowdownEffectPanel, "Slowdown Rampup Time (s)", "rampUpTime", 0.2f, 0.001f, 1000000000f); //max value of a billion seconds... enough. Min value isnt zero cause that causes crashes (the timescale will go to zero).
        rampUpTimeField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.rampUpTime = e.value;};
        Plugin.rampUpTime = rampUpTimeField.value;

        FloatField rechargeMultField = new FloatField(slowdownEffectPanel, "Recharge Rate (seconds use / s)", "rechargeMultiplier", 0.75f, 0.0f, 1000f); 
        rechargeMultField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.rechargeMultiplier = e.value;};
        Plugin.rechargeMultiplier = rechargeMultField.value;
    }
}