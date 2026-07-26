using GDF.Composition;
using GDF.Logical.Values;
using GDF.PropertyStacks;
using GDF.PropertyStacks.Definitions;
using Godot;
using Godot.Collections;

namespace Game.Util;

[Tool]
public partial class AnimatableProperties : Node
{
    [Signal]
    public delegate void BecameActiveEventHandler();

    [Signal]
    public delegate void BecameInactiveEventHandler();
    
    private const string GeneratedPropertyPrefix = "properties/";
    
    private Godot.Collections.Dictionary _properties = new();
    
    private Godot.Collections.Dictionary _propertyOperations = new();

    [Export(PropertyHint.Enum,"Global,Own,Camera")] public int StackSource = 0;
    
    [Export]
    public bool Active
    {
        get => _active;
        set
        {
            if (_active == value) return;
            _active = value;
            if (value)
            {
                if(IsInsideTree() && _frame == null) CreateFrame();
            }
            else
            {
                RemoveFrame();
            }
        }
    }
    
    [Export]
    public float Weight
    {
        get => _weight;
        set
        {
            _weight = value;
            _frame?.SetWeight(value);
        }
    }

    [Export]
    public Godot.Collections.Dictionary Properties
    {
        get => _properties;
        set
        {
            ClearCache();
            if (_properties == value) return;
            _properties = value?.Duplicate(); //Godot please stop sharing exported collections across instances of a scene thx
        }
    }
    
    [Export]
    public Godot.Collections.Dictionary PropertyModificationOperations
    {
        get => _propertyOperations;
        set
        {
            ClearCache();
            if (_propertyOperations == value) return;
            _propertyOperations = value?.Duplicate(); //Godot please stop sharing exported collections across instances of a scene thx
        }
    }
    
    [Export] public int Order = 0;

    private PropertyFrame _frame;
    private float _weight = 1.0f;
    private bool _active = false;

    private Array<Dictionary> _cachedPropertyList;
    private System.Collections.Generic.Dictionary<StringName, StringName> _generatedPropertyToKeyMap;

    private PropertyStack GetStack()
    {
        switch (StackSource)
        {
            case 0:
                return GlobalPropertyStack.Instance;
            case 1:
                return this.GetComponent<PropertyStack>();
            case 2:
                return GetViewport()?.GetCamera2D()?.GetComponent<PropertyStack>();
            default:
                return null;
        }
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        if (Engine.IsEditorHint()) return;
        if (Active && _frame == null)
        {
            CreateFrame();
        }
    }
    
    private void CreateFrame()
    {
        if (_frame != null)
        {
            return;
        }
        if (!IsInsideTree())
        {
            GD.PrintErr("Cannot apply AnimatableProperties from outside the tree");
        }

        if (GetStack() is not { } stack) return;

        _frame = stack.NewFrame("Modification", order: Order).BindToNode(this);
        
        _frame.SetWeight(Weight);

        UpdateFrame(_frame);
        EmitSignalBecameActive();
    }

    private void UpdateFrame(PropertyFrame frame)
    {
        if (frame == null) return;
        if (Properties != null)
            foreach ((var vKey, var value) in Properties)
            {
                string key = vKey.AsString();
                Variant computedValue;
                if (value.VariantType == Variant.Type.Object && value.AsGodotObject() is ValueSource valueSource)
                    computedValue = valueSource.GetValue(this);
                else
                    computedValue = value;

                if (_propertyOperations.TryGetValue(key, out var operation))
                {
                    frame.Set(key, new VectorModification<Variant>()
                    {
                        Operation = operation.As<ModificationOperation>(),
                        Value = computedValue
                    });
                }
                else
                {
                    frame.Set(key, computedValue);
                }
            }
    }

    private void RemoveFrame()
    {
        if (_frame == null) return;
        _frame = _frame.Remove();
        EmitSignalBecameInactive();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (Engine.IsEditorHint()) return;
        
        if (Active && _frame == null) CreateFrame();
        if (Active && _frame != null) UpdateFrame(_frame);
        if (!Active && _frame != null) RemoveFrame();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (Engine.IsEditorHint()) return;
        RemoveFrame();
    }

    private void ClearCache()
    {
        _cachedPropertyList = null;
        _generatedPropertyToKeyMap = null;
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        EnsurePropertyListReady();
        return _cachedPropertyList;
    }

    private void EnsurePropertyListReady()
    {
        if (_cachedPropertyList != null) return;

        _cachedPropertyList = new();
        _generatedPropertyToKeyMap ??= new();
        if(_properties != null)
            foreach ((var key, var value) in _properties)
            {
                string generatedName = GeneratedPropertyPrefix + key;
                _cachedPropertyList.Add(new Dictionary()
                {
                    { "name", generatedName },
                    { "type", Variant.From(Variant.Type.Nil) },
                    { "hint", Variant.From(PropertyHint.None) },
                    { "hint_string", "" },
                    { "usage", Variant.From(PropertyUsageFlags.Editor | PropertyUsageFlags.NilIsVariant) }
                });
                _generatedPropertyToKeyMap[generatedName] = key.AsString();
            }
    }

    public override Variant _Get(StringName property)
    {
        EnsurePropertyListReady();
        if (_generatedPropertyToKeyMap.TryGetValue(property, out var key) && _properties != null)
        {
            return _properties[key];
        }
        return base._Get(property);
    }

    public override bool _Set(StringName property, Variant value)
    {
        EnsurePropertyListReady();
        if (_generatedPropertyToKeyMap.TryGetValue(property, out var key) && _properties != null)
        {
            _properties[key] = value;
            return true;
        }
        return base._Set(property, value);
    }

    public override bool _PropertyCanRevert(StringName property)
    {
        EnsurePropertyListReady();
        return _generatedPropertyToKeyMap.ContainsKey(property);
    }

    public override Variant _PropertyGetRevert(StringName property)
    {
        return default;
    }
}