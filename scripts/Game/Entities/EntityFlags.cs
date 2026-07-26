using Game.Player;
using GDF;
using GDF.Composition;
using GDF.Data;
using Godot;

namespace Game.Entities;

[Icon($"{GdfConstants.IconRoot}/data_context.png")]
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

    public bool Moving => !(_mobController.Get(this)?.MoveVector.IsZeroApprox() ?? true) ||
                          !(_playerController.Get(this)?.MoveVector.IsZeroApprox() ?? true);

    public StringName UpdatedSignalName => SignalName.Updated;

    private ComponentCache<MobController> _mobController;
    private ComponentCache<PlayerController> _playerController;

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