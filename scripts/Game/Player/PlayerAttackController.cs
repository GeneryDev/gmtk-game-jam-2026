using Game.Entities;
using GDF.Composition;
using GDF.Data;
using GDF.Input;
using GDF.Util;
using Godot;

namespace Game.Player;

public partial class PlayerAttackController : Node
{
    [Signal]
    public delegate void NoAmmoEventHandler();
    
    [Export] public GdfInputAction AttackAction;
    
    private ComponentCache<GdfPlayerInput> _playerInput;
    private ComponentCache<PlayerController> _playerController;
    private ComponentCache<MotionComponent> _motionComponent;

    private bool _attacking = false;

    private float _fireStartCooldown;
    private Accumulator _fireIntervalTimer;

    private Vector2 _targetPosGlobal;

    public PlayerAttacks.Descriptor EquippedAttack => WeaponSystem.Instance.EquippedAttack;

    private float GetAttackInterval()
    {
        return EquippedAttack.Reference?.AttackInterval ?? 1;
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        var playerInput = _playerInput.Get(this);
        _attacking = _playerController.Get(this).HasControl() && playerInput.GetBool(AttackAction);
        
        if (_fireStartCooldown <= 0)
        {
            if (playerInput.ConsumeActionEvent(AttackAction))
            {
                if (WeaponSystem.Instance.HasAmmo(EquippedAttack))
                {
                    _fireIntervalTimer.Reset();
                    float attackInterval = GetAttackInterval();
                    _fireIntervalTimer.Add(attackInterval);
                    _fireStartCooldown = attackInterval;
                }
                else
                {
                    EmitSignalNoAmmo();
                }
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
            float attackInterval = GetAttackInterval();
            while (_fireIntervalTimer.Consume(attackInterval))
            {
                Attack();
            }
        }
    }

    public void Attack()
    {
        var body = _motionComponent.Get(this).Body;
        if (EquippedAttack.IsEmpty) return;
        if (!WeaponSystem.Instance.ConsumeAmmo(EquippedAttack)) return;
        
        var attackContext = new AttackInstanceContext(Owner, body.GlobalPosition, _targetPosGlobal);

        var attackInstance = EquippedAttack.New();
        
        attackInstance.InjectContext(attackContext);
        Owner.AddChild(attackInstance);
        attackInstance.Owner = Owner;
    }
}