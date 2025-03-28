using System.IO;
using UnityEngine;

using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using PluginConfig.API.Functionals;

namespace UltraTimeManipulation;

public class PluginConfig
{
    public enum KeyEnum
    {
        Backspace, Tab, Escape, Space, UpArrow, DownArrow, RightArrow, LeftArrow, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z, 
        Alpha1, Alpha2, Alpha3, Alpha4, Alpha5, Alpha6, Alpha7, Alpha8, Alpha9, Alpha0, CapsLock,
        RightShift, LeftShift, RightControl, LeftControl, RightAlt, LeftAlt, Mouse1, Mouse2, Mouse3, Mouse4, Mouse5, Mouse6, Mouse7,
        BackQuote, EqualsSign, Minus, LeftBracket, RightBracket, Semicolon, Quote, Comma, Period, Slash, Backslash, 
		Numlock, KeypadDivide, KeypadMultiply, KeypadMinus, KeypadPlus, KeypadEnter, KeypadPeriod, 
		Keypad0, Keypad1, Keypad2, Keypad3, Keypad4, Keypad5, Keypad6, Keypad7, Keypad8, Keypad9, 
		Home, End, PageUp, PageDown, Enter, 
		F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15
    }
    public static KeyCode convertKeyEnumToKeyCode(KeyEnum value)
    {
        KeyCode code = KeyCode.None;
        if (value.Equals(KeyEnum.Backspace)) {code = KeyCode.Backspace;}
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
        else if (value.Equals(KeyEnum.Mouse5)) {code = KeyCode.Mouse4;}
        else if (value.Equals(KeyEnum.Mouse6)) {code = KeyCode.Mouse5;}
        else if (value.Equals(KeyEnum.Mouse7)) {code = KeyCode.Mouse6;}
        
        else if(value.Equals(KeyEnum.BackQuote)) {return KeyCode.BackQuote;} 
        else if(value.Equals(KeyEnum.EqualsSign)) {return KeyCode.Equals;} 
        else if(value.Equals(KeyEnum.Minus)) {return KeyCode.Minus;} 
        else if(value.Equals(KeyEnum.LeftBracket)) {return KeyCode.LeftBracket;} 
        else if(value.Equals(KeyEnum.RightBracket)) {return KeyCode.RightBracket;} 
        else if(value.Equals(KeyEnum.Semicolon)) {return KeyCode.Semicolon;} 
        else if(value.Equals(KeyEnum.Quote)) {return KeyCode.Quote;} 
        else if(value.Equals(KeyEnum.Comma)) {return KeyCode.Comma;} 
        else if(value.Equals(KeyEnum.Period)) {return KeyCode.Period;} 
        else if(value.Equals(KeyEnum.Slash)) {return KeyCode.Slash;} 
        else if(value.Equals(KeyEnum.Backslash)) {return KeyCode.Backslash;} 
        else if(value.Equals(KeyEnum.Numlock)) {return KeyCode.Numlock;} 
        else if(value.Equals(KeyEnum.KeypadDivide)) {return KeyCode.KeypadDivide;} 
        else if(value.Equals(KeyEnum.KeypadMultiply)) {return KeyCode.KeypadMultiply;} 
        else if(value.Equals(KeyEnum.KeypadMinus)) {return KeyCode.KeypadMinus;} 
        else if(value.Equals(KeyEnum.KeypadPlus)) {return KeyCode.KeypadPlus;} 
        else if(value.Equals(KeyEnum.KeypadEnter)) {return KeyCode.KeypadEnter;} 
        else if(value.Equals(KeyEnum.KeypadPeriod)) {return KeyCode.KeypadPeriod;} 
        else if(value.Equals(KeyEnum.Keypad0)) {return KeyCode.Keypad0;} 
        else if(value.Equals(KeyEnum.Keypad1)) {return KeyCode.Keypad1;} 
        else if(value.Equals(KeyEnum.Keypad2)) {return KeyCode.Keypad2;} 
        else if(value.Equals(KeyEnum.Keypad3)) {return KeyCode.Keypad3;} 
        else if(value.Equals(KeyEnum.Keypad4)) {return KeyCode.Keypad4;} 
        else if(value.Equals(KeyEnum.Keypad5)) {return KeyCode.Keypad5;} 
        else if(value.Equals(KeyEnum.Keypad6)) {return KeyCode.Keypad6;} 
        else if(value.Equals(KeyEnum.Keypad7)) {return KeyCode.Keypad7;} 
        else if(value.Equals(KeyEnum.Keypad8)) {return KeyCode.Keypad8;} 
        else if(value.Equals(KeyEnum.Keypad9)) {return KeyCode.Keypad9;} 
        else if(value.Equals(KeyEnum.Home)) {return KeyCode.Home;} 
        else if(value.Equals(KeyEnum.End)) {return KeyCode.End;} 
        else if(value.Equals(KeyEnum.PageUp)) {return KeyCode.PageUp;} 
        else if(value.Equals(KeyEnum.PageDown)) {return KeyCode.PageDown;} 
        else if(value.Equals(KeyEnum.Enter)) {return KeyCode.Return;} 
        else if(value.Equals(KeyEnum.F1)) {return KeyCode.F1;} 
        else if(value.Equals(KeyEnum.F2)) {return KeyCode.F2;}
        else if(value.Equals(KeyEnum.F3)) {return KeyCode.F3;} 
        else if(value.Equals(KeyEnum.F4)) {return KeyCode.F4;} 
        else if(value.Equals(KeyEnum.F5)) {return KeyCode.F5;} 
        else if(value.Equals(KeyEnum.F6)) {return KeyCode.F6;} 
        else if(value.Equals(KeyEnum.F7)) {return KeyCode.F7;} 
        else if(value.Equals(KeyEnum.F8)) {return KeyCode.F8;} 
        else if(value.Equals(KeyEnum.F9)) {return KeyCode.F9;} 
        else if(value.Equals(KeyEnum.F10)) {return KeyCode.F10;} 
        else if(value.Equals(KeyEnum.F11)) {return KeyCode.F11;} 
        else if(value.Equals(KeyEnum.F12)) {return KeyCode.F12;} 
        else if(value.Equals(KeyEnum.F13)) {return KeyCode.F13;} 
        else if(value.Equals(KeyEnum.F14)) {return KeyCode.F14;}
        else if(value.Equals(KeyEnum.F15)) {return KeyCode.F15;}
        return code;
    }

    private static void SetDisplayNames(EnumField<KeyEnum> field) 
    {
		field.SetEnumDisplayName(KeyEnum.Minus, "-");
		field.SetEnumDisplayName(KeyEnum.EqualsSign, "=");
		field.SetEnumDisplayName(KeyEnum.LeftBracket, "[");
		field.SetEnumDisplayName(KeyEnum.RightBracket, "]");
		field.SetEnumDisplayName(KeyEnum.CapsLock, "Caps Lock");
		field.SetEnumDisplayName(KeyEnum.LeftShift, "Left Shift");
		field.SetEnumDisplayName(KeyEnum.RightShift, "Right Shift");
		field.SetEnumDisplayName(KeyEnum.LeftControl, "Left Control");
		field.SetEnumDisplayName(KeyEnum.RightControl, "Right Control");
		field.SetEnumDisplayName(KeyEnum.LeftAlt, "Left Alt");
		field.SetEnumDisplayName(KeyEnum.RightAlt, "Right Alt");
		field.SetEnumDisplayName(KeyEnum.BackQuote, "`");
		field.SetEnumDisplayName(KeyEnum.Quote, "'");
		field.SetEnumDisplayName(KeyEnum.Semicolon, ";");
		field.SetEnumDisplayName(KeyEnum.Slash, "/");
		field.SetEnumDisplayName(KeyEnum.Backslash, "\\");
		field.SetEnumDisplayName(KeyEnum.Keypad0, "Keypad 0");
		field.SetEnumDisplayName(KeyEnum.Keypad1, "Keypad 1");
		field.SetEnumDisplayName(KeyEnum.Keypad2, "Keypad 2");
		field.SetEnumDisplayName(KeyEnum.Keypad3, "Keypad 3");
		field.SetEnumDisplayName(KeyEnum.Keypad4, "Keypad 4");
		field.SetEnumDisplayName(KeyEnum.Keypad5, "Keypad 5");
		field.SetEnumDisplayName(KeyEnum.Keypad6, "Keypad 6");
		field.SetEnumDisplayName(KeyEnum.Keypad7, "Keypad 7");
		field.SetEnumDisplayName(KeyEnum.Keypad8, "Keypad 8");
		field.SetEnumDisplayName(KeyEnum.Keypad9, "Keypad 9");
		field.SetEnumDisplayName(KeyEnum.KeypadDivide, "Keypad /");
		field.SetEnumDisplayName(KeyEnum.KeypadMultiply, "Keypad *");
		field.SetEnumDisplayName(KeyEnum.KeypadPlus, "Keypad Plus");
		field.SetEnumDisplayName(KeyEnum.KeypadMinus, "Keypad Minus");
		field.SetEnumDisplayName(KeyEnum.KeypadEnter, "Keypad Enter");
		field.SetEnumDisplayName(KeyEnum.KeypadPeriod, "Keypad .");
		field.SetEnumDisplayName(KeyEnum.Period, ".");
		field.SetEnumDisplayName(KeyEnum.Comma, ",");
		field.SetEnumDisplayName(KeyEnum.PageUp, "Page Up");
		field.SetEnumDisplayName(KeyEnum.PageDown, "Page Down");
		field.SetEnumDisplayName(KeyEnum.UpArrow, "Up Arrow");
		field.SetEnumDisplayName(KeyEnum.DownArrow, "Down Arrow");
		field.SetEnumDisplayName(KeyEnum.LeftArrow, "Left Arrow");
		field.SetEnumDisplayName(KeyEnum.RightArrow, "Right Arrow");
		field.SetEnumDisplayName(KeyEnum.Alpha0, "0");
		field.SetEnumDisplayName(KeyEnum.Alpha1, "1");
		field.SetEnumDisplayName(KeyEnum.Alpha2, "2");
		field.SetEnumDisplayName(KeyEnum.Alpha3, "3");
		field.SetEnumDisplayName(KeyEnum.Alpha4, "4");
		field.SetEnumDisplayName(KeyEnum.Alpha5, "5");
		field.SetEnumDisplayName(KeyEnum.Alpha6, "6");
		field.SetEnumDisplayName(KeyEnum.Alpha7, "7");
		field.SetEnumDisplayName(KeyEnum.Alpha8, "8");
		field.SetEnumDisplayName(KeyEnum.Alpha9, "9");
		field.SetEnumDisplayName(KeyEnum.Mouse1, "Mouse 1");
		field.SetEnumDisplayName(KeyEnum.Mouse2, "Mouse 2");
		field.SetEnumDisplayName(KeyEnum.Mouse3, "Mouse 3");
		field.SetEnumDisplayName(KeyEnum.Mouse4, "Mouse 4");
		field.SetEnumDisplayName(KeyEnum.Mouse5, "Mouse 5");
		field.SetEnumDisplayName(KeyEnum.Mouse6, "Mouse 6");
		field.SetEnumDisplayName(KeyEnum.Mouse7, "Mouse 7");
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

    public static void OpenSoundFolder() {Application.OpenURL(Plugin.DefaultParentFolder);}

    //public static void OpenImageFolder() {Application.OpenURL(Plugin.DefaultImageFolder);}

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

        BoolField soundsField = new BoolField(sfxPanel, "Sounds Enabled", "soundsEnabled", true);
        ConfigDivision soundDivision = new ConfigDivision(sfxPanel, "soundDivision");
        soundsField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.soundEnabled = e.value; soundDivision.interactable = e.value;};
        Plugin.soundEnabled = soundsField.value; soundDivision.interactable = soundsField.value;

        FloatField volumeField = new FloatField(soundDivision, "Slowdown Enter/Leave Volume Mult.", "volumeMultiplier", 0.7f, 0.00f, 1.00f);
        volumeField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.volumeMult = e.value;};
        Plugin.volumeMult = volumeField.value;

        ConfigHeader soundDistortionWarningHeader = new ConfigHeader(soundDivision, "Sound distortion is experimental. May need to restart game to reset some sounds.");
        soundDistortionWarningHeader.textSize = 16;
        soundDistortionWarningHeader.textColor = Color.red;

        BoolField soundDistortionField = new BoolField(soundDivision, "Sound Distortion Enabled", "soundDistortionEnabled", true);
        ConfigDivision soundDistortionDivision = new ConfigDivision(soundDivision, "soundDistortiongDivision");
        soundDistortionField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.soundDistortionEnabled = e.value; soundDistortionDivision.interactable = e.value;};
        Plugin.soundDistortionEnabled = soundDistortionField.value; soundDistortionDivision.interactable = soundDistortionField.value;

        FloatField soundSlowdownAmountField = new FloatField(soundDistortionDivision, "General Sound Slowdown Multiplier", "soundSlowdownAmount", 1.00f, 0.00f, 1.00f);
        soundSlowdownAmountField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.soundSlowdownFactor = e.value;};
        Plugin.soundSlowdownFactor = soundSlowdownAmountField.value;

        FloatField musicSlowdownAmountField = new FloatField(soundDistortionDivision, "Music Slowdown Multiplier", "musicSlowdownAmount", 0.66f, 0.00f, 1.00f);
        musicSlowdownAmountField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.musicSlowdownFactor = e.value;};
        Plugin.musicSlowdownFactor = musicSlowdownAmountField.value;

        FloatField parrySoundSlowdownAmountField = new FloatField(soundDistortionDivision, "Parry Sound Slowdown Multiplier", "parrySoundSlowdownAmount", 0.00f, 0.00f, 1.00f);
        parrySoundSlowdownAmountField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.parrySoundSlowdownFactor = e.value;};
        Plugin.parrySoundSlowdownFactor = parrySoundSlowdownAmountField.value;

        BoolField muffleMusicField = new BoolField(soundDistortionDivision, "Muffle Music in Slowdown", "muffleMusicSlowdown", true);
        muffleMusicField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.muffleMusicSlowDown = e.value;};
        Plugin.muffleMusicSlowDown = muffleMusicField.value;

        /*BoolField allSoundPitchField = new BoolField(sfxPanel, "General Sound Pitch Changes", "allSoundPitch", true);
        allSoundPitchField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.allSoundPitchEnabled = e.value;};
        Plugin.allSoundPitchEnabled = allSoundPitchField.value;

        BoolField doorSoundPitchField = new BoolField(sfxPanel, "Door Sound Pitch Changes", "doorSoundPitch", true);
        doorSoundPitchField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.doorSoundPitchEnabled = e.value;};
        Plugin.doorSoundPitchEnabled = doorSoundPitchField.value;

        BoolField goreSoundPitchField = new BoolField(sfxPanel, "Gore Sound Pitch Changes", "goreSoundPitch", true);
        goreSoundPitchField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.goreSoundPitchEnabled = e.value;};
        Plugin.goreSoundPitchEnabled = goreSoundPitchField.value;

        BoolField unfreezeableSoundPitchField = new BoolField(sfxPanel, "'Unfreezeable' Sound Pitch Changes", "unfreezeableSoundPitch", false);
        unfreezeableSoundPitchField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.unfreezeableSoundPitchEnabled = e.value;};
        Plugin.unfreezeableSoundPitchEnabled = unfreezeableSoundPitchField.value;

        //FloatField soundSlowdownRelativePitchField = new FloatField(sfxPanel, "Sound Slowdown Relative Pitch", "soundSlowdownRelativePitch", 1.0f, 0.01f, 5.00f);
        //soundSlowdownRelativePitchField.onValueChange += (FloatField.FloatValueChangeEvent e) => {Plugin.soundSlowDownRelativePitch = e.value;};
        //Plugin.soundSlowDownRelativePitch = soundSlowdownRelativePitchField.value;*/

        ButtonField openSoundsFolderField = new ButtonField(sfxPanel, "Open Sounds Folder", "button.openfolder");
        openSoundsFolderField.onClick += new ButtonField.OnClick(OpenSoundFolder);

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
        SetDisplayNames(keyIDField);

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

        //new ConfigSpace(HUDPanel, 10f);
        //new ConfigHeader(HUDPanel, "Legacy HUD should never be used, and doesn't work with any of the above options. It also looks bad.");

        //BoolField legacyHUDField = new BoolField(HUDPanel, "Legacy HUD (looks bad)", "legacyUI", false);
        //legacyHUDField.onValueChange += (BoolField.BoolValueChangeEvent e) => {Plugin.legacyDisplay = e.value;};
        //Plugin.legacyDisplay = legacyHUDField.value;

        //ButtonField openVisualsFolderField = new ButtonField(HUDPanel, "Open HUD Folder", "button.openfolder");
        //openVisualsFolderField.onClick += new ButtonField.OnClick(OpenImageFolder);

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