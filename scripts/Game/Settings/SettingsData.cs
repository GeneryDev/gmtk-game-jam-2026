using GDF.Data;
using GDF.Data.Static;
using GDF.Serialization;
using GDF.UI;
using GDF.Util;
using Godot;

namespace Game.Settings;

public class SettingsData : IJsonSerializable
{
    public float MasterVolume = 1.0f;
    public float SfxVolume = 1.0f;
    public float MusicVolume = 1.0f;

    public GdfViewportUserSettings ViewportSettings = new();

    public void Deserialize(Variant v)
    {
        var json = JsonSerializer.Default with
        {
            PropertyOmissionHandlingMode = JsonSerializer.PropertyOmissionHandlingModeEnum.KeepDefault
        };
        var dict = v.AsGodotDictionary();
        json.Deserialize(dict, nameof(MasterVolume), ref MasterVolume);
        json.Deserialize(dict, nameof(SfxVolume), ref SfxVolume);
        json.Deserialize(dict, nameof(MusicVolume), ref MusicVolume);
        json.Deserialize(dict, nameof(ViewportSettings), ref ViewportSettings);
    }

    public Variant Serialize()
    {
        var json = JsonSerializer.Default;
        var dict = new Godot.Collections.Dictionary();
        json.Serialize(dict, nameof(MasterVolume), ref MasterVolume);
        json.Serialize(dict, nameof(SfxVolume), ref SfxVolume);
        json.Serialize(dict, nameof(MusicVolume), ref MusicVolume);
        json.Serialize(dict, nameof(ViewportSettings), ref ViewportSettings);
        return dict;
    }
}

[StaticDataContext("settings")]
public struct SettingsContext : IDataContext
{
    public SettingsData Settings => SettingsManager.Instance.Settings;
    
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        if (!SettingsManager.InstanceExists) return false;
        switch (key)
        {
            case "master_volume":
            {
                output = Settings.MasterVolume;
                return true;
            }
            case "sfx_volume":
            {
                output = Settings.SfxVolume;
                return true;
            }
            case "music_volume":
            {
                output = Settings.MusicVolume;
                return true;
            }
        }

        return false;
    }

    public bool WriteBack(string key, Variant value)
    {
        switch (key)
        {
            case "master_volume":
            {
                Settings.MasterVolume = value.AsSingle();
                SettingsManager.Instance.EmitChanged(nameof(Settings.MasterVolume), Settings.MasterVolume);
                return true;
            }
            case "sfx_volume":
            {
                Settings.SfxVolume = value.AsSingle();
                SettingsManager.Instance.EmitChanged(nameof(Settings.SfxVolume), Settings.SfxVolume);
                return true;
            }
            case "music_volume":
            {
                Settings.MusicVolume = value.AsSingle();
                SettingsManager.Instance.EmitChanged(nameof(Settings.MusicVolume), Settings.MusicVolume);
                return true;
            }
        }

        return false;
    }
    
    public bool EqualsContext(IDataContext other)
    {
        return other is SettingsContext;
    }

    public void ConnectUpdateSignal(Callable callable)
    {
        if (!SettingsManager.InstanceExists) return;
        SettingsManager.Instance.TryConnect(SettingsManager.SignalName.Updated, callable);
    }

    public void DisconnectUpdateSignal(Callable callable)
    {
        if (!SettingsManager.InstanceExists) return;
        SettingsManager.Instance.TryDisconnect(SettingsManager.SignalName.Updated, callable);
    }
}