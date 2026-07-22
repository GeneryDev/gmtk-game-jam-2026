using Game.Entities;
using GDF.Composition;
using GDF.Data;
using GDF.Input;
using GDF.Util;
using Godot;

namespace Game.Player;

public partial class PlayerAttackController : Node
{
    [Export] public GdfInputAction AttackAction;
    [Export] public float AttackInterval = 0.2f;
    
    private ComponentCache<GdfPlayerInput> _playerInput;
    private ComponentCache<PlayerController> _playerController;
    private ComponentCache<MotionComponent> _motionComponent;

    private bool _attacking = false;

    private float _fireStartCooldown;
    private Accumulator _fireIntervalTimer;

    private Vector2 _targetPosGlobal;

    [Export] public PackedScene AttackScene;
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        var playerInput = _playerInput.Get(this);
        _attacking = _playerController.Get(this).HasControl() && playerInput.GetBool(AttackAction);
        
        if (_fireStartCooldown <= 0)
        {
            if (playerInput.ConsumeActionEvent(AttackAction))
            {
                _fireIntervalTimer.Reset();
                _fireIntervalTimer.Add(AttackInterval);
                _fireStartCooldown = AttackInterval;
            }
        }

        var camera = GetViewport().GetCamera2D();
        if (camera != null)
        {
            var globalPos = camera.GetGlobalMousePosition();
            _targetPosGlobal = globalPos;
            // _targetDir = (_targetPosGlobal - _motionComponent.Get(this).Body.GlobalPosition).Normalized();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_fireStartCooldown > 0)
        {
            _fireStartCooldown -= (float)delta;
        }

        if (_attacking)
        {
            _fireIntervalTimer.Add((float)delta);
            while (_fireIntervalTimer.Consume(AttackInterval))
            {
                Attack();
            }
        }
    }

    public void Attack()
    {
        var body = _motionComponent.Get(this).Body;
        GD.Print("Attack!");
        if (AttackScene == null) return;
        var attackContext = new AttackInstanceContext(Owner, body.GlobalPosition, _targetPosGlobal);

        var attackInstance = AttackScene.GdfInstantiate();
        
        attackInstance.InjectContext(attackContext);
        Owner.AddChild(attackInstance);
        attackInstance.Owner = Owner;
    }
}