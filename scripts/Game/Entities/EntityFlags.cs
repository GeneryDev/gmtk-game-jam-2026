using GDF.Data;
using Godot;

namespace Game.Entities;

public partial class EntityFlags : Node, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();
    
    private bool _mirrored = false;

    public bool Mirrored
    {
        get => _mirrored;
        set
        {
            if (_mirrored == value) return;
            _mirrored = value;
            EmitSignalUpdated();
        }
    }

    public StringName UpdatedSignalName => SignalName.Updated;

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "mirrored":
            {
                output = _mirrored;
                return true;
            }
        }

        return false;
    }
}