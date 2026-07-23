using Game.Entities;
using GDF;
using GDF.Composition;
using GDF.Input;
using GDF.Multiplayer;
using Godot;

namespace Game.Player;

public partial class PlayerController : Node
{
    [Export] public GdfInputAction MoveAction;
    [Export] public GdfInputAction DashAction;

    [Export] public float MovementSpeed = 100;
    [Export] public float DashSpeedMultiplier = 1.5f;
    
    public Vector2 MoveVector = new Vector2();
    public bool Dashing = false;

    private int PlayerId => 0;
    private ComponentCache<GdfPlayerInput> _playerInput;
    private ComponentCache<MotionComponent> _motionComponent;
    private ComponentCache<EntityFlags> _entityFlags;

    public bool HasControl()
    {
        return PerPlayerPropertyStacks.HasBaseControl(PlayerId, InputGroups.Default);
    }

    public override void _Process(double delta)
    {
        if (HasControl())
        {
            MoveVector = _playerInput.Get(this).GetVec2(MoveAction);
            Dashing = _playerInput.Get(this).GetBool(DashAction);
        }
        else
        {
            MoveVector = Vector2.Zero;
            Dashing = false;
        }
        if (Mathf.Abs(MoveVector.X) > 0.1f && _entityFlags.Get(this) is { } entityFlags)
        {
            entityFlags.Mirrored = MoveVector.X < 0;
        }
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        var motion = _motionComponent.Get(this);

        var speed = MovementSpeed;
        if (Dashing) speed *= DashSpeedMultiplier;
        motion.ApplyTranslation(MoveVector * speed * (float)delta);
        base._PhysicsProcess(delta);
    }
}