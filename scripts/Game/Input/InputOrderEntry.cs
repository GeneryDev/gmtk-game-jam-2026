using System.Collections.Generic;
using GDF.Serialization;
using Godot;

namespace Game.Input;

public struct InputOrderEntry : IJsonSerializable
{
    public List<string> InputContextTags = new();
    public int DeviceIndex = -1;

    public InputOrderEntry()
    {
    }

    public bool IsEmpty => InputContextTags == null;

    public InputOrderEntry Duplicate()
    {
        return new InputOrderEntry()
        {
            InputContextTags = new List<string>(InputContextTags),
            DeviceIndex = DeviceIndex
        };
    }
    
    public void Deserialize(Variant v)
    {
        var dict = v.AsGodotDictionary();
        var json = JsonSerializer.Default;
        json.DeserializeVariants(dict, nameof(InputContextTags), ref InputContextTags);
        json.Deserialize(dict, nameof(DeviceIndex), ref DeviceIndex);
    }

    public Variant Serialize()
    {
        var dict = new Godot.Collections.Dictionary();
        var json = JsonSerializer.Default;
        json.SerializeVariants(dict, nameof(InputContextTags), ref InputContextTags);
        json.Serialize(dict, nameof(DeviceIndex), ref DeviceIndex);
        return dict;
    }
}