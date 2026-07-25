using System.Collections.Generic;
using Godot;

namespace Game.Util;

[Tool]
[GlobalClass]
public partial class Area2DExtension : Area2D
{
    [Signal]
    public delegate void EnteredEventHandler();

    [Signal]
    public delegate void NodeEnteredEventHandler(Node2D node);

    [Signal]
    public delegate void ExitedEventHandler();

    [Signal]
    public delegate void NodeExitedEventHandler(Node2D node);

    private readonly List<Node2D> _overlappingNodes = new();
    private readonly List<bool> _nodeMetCondition = new();
    private bool _anyMetCondition = false;

    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;
        BodyEntered += OnEntered;
        BodyExited += OnExited;
        AreaEntered += OnEntered;
        AreaExited += OnExited;
    }

    private void OnEntered(Node2D body)
    {
        HandleEntered(body);
    }

    private void HandleEntered(Node2D body)
    {
        if (body == null) return;
        _overlappingNodes.Add(body);
        bool meetsCondition = MeetsCondition(body);
        _nodeMetCondition.Add(meetsCondition);
        if (meetsCondition)
        {
            EmitSignal(SignalName.NodeEntered, body);

            if (!_anyMetCondition)
            {
                _anyMetCondition = true;
                EmitSignal(SignalName.Entered);
            }
        }
    }

    private void OnExited(Node2D body)
    {
        HandleExited(body);
    }

    private void HandleExited(Node2D body)
    {
        if (body == null) return;

        int existingIndex = _overlappingNodes.IndexOf(body);
        if (existingIndex == -1) return;
        bool hadMetCondition = _nodeMetCondition[existingIndex];
        _overlappingNodes.RemoveAt(existingIndex);
        _nodeMetCondition.RemoveAt(existingIndex);
        if (hadMetCondition)
        {
            EmitSignal(SignalName.NodeExited, body);
        }
    }

    public bool MeetsCondition(Node2D node)
    {
        return true;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint()) return;
        var anyMeetsCondition = false;
        for (var i = 0; i < _overlappingNodes.Count; i++)
        {
            var node = _overlappingNodes[i];
            bool hadMetCondition = _nodeMetCondition[i];
            if (!IsInstanceValid(node))
            {
                _overlappingNodes.RemoveAt(i);
                _nodeMetCondition.RemoveAt(i);
                i--;
                continue;
            }

            bool meetsCondition = MeetsCondition(node);
            if (meetsCondition != hadMetCondition)
            {
                EmitSignal(meetsCondition ? SignalName.NodeEntered : SignalName.NodeExited, node);

                _nodeMetCondition[i] = meetsCondition;
            }

            if (meetsCondition) anyMeetsCondition = true;
        }

        if (anyMeetsCondition != _anyMetCondition)
        {
            _anyMetCondition = anyMeetsCondition;
            EmitSignal(anyMeetsCondition ? SignalName.Entered : SignalName.Exited);
        }
    }

    public bool HasMatchingOverlappingNodes()
    {
        for (var i = 0; i < _overlappingNodes.Count; i++)
            if (_nodeMetCondition[i])
                return true;
        return false;
    }
}