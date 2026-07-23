using System.Collections.Generic;
using GDF.Data;
using GDF.Logical.Signals;
using GDF.Scenes;
using GDF.UI;
using GDF.Util;
using Godot;

namespace Game.Util;

[GlobalClass]
public partial class ScreenSpawner : Node
{
    [Export] public PackedScene Template;

    [Export] public Node Context;
    [Export] public Godot.Collections.Dictionary<StringName, NodePath> DataContextsBySlot
#if !GODOT_WEB
            = new()
#endif
        ;
    [Export] public SignalStation ConnectSignalStation;
    [Export]
    public NodePath RelativeToNode = ".";

    [Export(PropertyHint.Enum, "As Child,As Sibling")]
    public ISceneToggler.RelativeModeEnum RelativeMode = ISceneToggler.RelativeModeEnum.AsChild;

    private readonly List<Screen> _created = new();

    public void Trigger()
    {
        var relativeNode = GetRelativeNode(out var relativeMode);
        
        var screen = Template.GdfInstantiate<Screen>();
        var nodeToEnterTree = screen.ToPlaceholder();
        screen.InjectContext(GetContext());

        if (DataContextsBySlot != null)
        {
            foreach (var (slotId, contextNode) in DataContextsBySlot)
            {
                screen.InjectContext(slotId, this.GetNodeOrNull(contextNode) as IDataContext);
            }
        }

        if (ConnectSignalStation != null)
        {
            screen.ConnectSignalStation(ConnectSignalStation);
        }

        if (!IsInstanceValid(screen) || screen.IsQueuedForDeletion()) return;
        _created.Add(screen);
        
        switch(RelativeMode)
        {
            case ISceneToggler.RelativeModeEnum.AsChild:
                relativeNode.AddChild(nodeToEnterTree);
                break;
            case ISceneToggler.RelativeModeEnum.AsSibling:
                relativeNode.AddSibling(nodeToEnterTree);
                break;
        };
        screen.ShowScreen();
    }

    private IDataContext GetContext()
    {
        if (Context is not IDataContext nodeContext) return null;
        return nodeContext;
    }
    
    private Node GetRelativeNode(out ISceneToggler.RelativeModeEnum relativeMode)
    {
        relativeMode = RelativeMode;
        return GetNode(RelativeToNode) ?? this;
    }

    public void CloseAll()
    {
        if (_created.Count == 0) return;
        foreach (var screen in _created)
        {
            if(IsInstanceValid(screen)) screen.ForceFadeOutScreen();
        }
        _created.Clear();
    }
}