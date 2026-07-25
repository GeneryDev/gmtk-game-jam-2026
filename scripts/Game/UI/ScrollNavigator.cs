using GDF.Data;
using GDF.Input;
using GDF.UI;
using GDF.Util;
using Godot;
using Godot.Collections;

namespace Game.UI;

public partial class ScrollNavigator : Node, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();
    [Signal]
    public delegate void ReachedEndEventHandler();

    [Export] public ScrollContainer ScrollContainer;
    [Export] public float ScrollDecay = 5;
    [Export] public bool AutoScroll = false;
    [Export] public float AutoScrollSpeed = 0;
    [Export] public bool ManualScroll = false;
    [Export] public float ManualScrollSpeed = 300;
    [Export] public bool ManualScrollRequireFocus = true;
    [Export] public bool ManualScrollRequireGroupFocus = true;
    [Export] public GdfInputAction ManualScrollAction;
    [Export] public string ScrollContextActionName = "action.scroll";
    
    private UserInterfaceComponent _uiComponent;

    private Vector2 _scrollVelocity;
    private Vector2 _scrollSubPixelDebt;

    private bool _reachedEnd = false;
    
    
    private Array<int> _applicablePlayerIds = new();
    private Array<int> _tempPlayerIdArray = new();
    private bool _scrollBarVisible = false;

    public override void _Ready()
    {
        _uiComponent = GetParentOrNull<UserInterfaceComponent>();
        EmitSignalUpdated();
    }

    private bool CanManualScroll(int playerId, out GdfPlayerInput input)
    {
        input = null;
        if (!ManualScroll) return false;
        if (_uiComponent?.GetUserInterface() is not { } ui || !ui.HasControl()) return false;
        if (ManualScrollRequireFocus && ui.GetPlayerFocus(playerId) != _uiComponent) return false;
        if (ManualScrollRequireGroupFocus && !_uiComponent.IsGroupFocused()) return false;
        input = ui.GetPlayerInput(playerId);
        return true;
    }

    public override void _Process(double delta)
    {
        if (UpdateApplicablePlayerIds())
        {
            EmitSignalUpdated();
        }

        if (ScrollContainer != null)
        {
            bool scrollbarVisible = ScrollContainer?.GetVScrollBar()?.IsVisibleInTree() ?? false;
            if (scrollbarVisible != _scrollBarVisible)
            {
                _scrollBarVisible = scrollbarVisible;
                EmitSignalUpdated();
            }
        }
        
        ProcessScrolling(delta);
    }

    private void ProcessScrolling(double delta)
    {
        float speed = AutoScroll ? AutoScrollSpeed : 0;
        var scrollDir = Vector2.Down;
        var manualScrolling = false;
        if (ManualScroll && _uiComponent?.GetUserInterface() is {} focusInterface)
        {
            foreach (int playerId in focusInterface.GetAllFocusedPlayerIds())
            {
                if (CanManualScroll(playerId, out var input) && input != null)
                {
                    var inputVec = input.GetVec2(ManualScrollAction ?? _uiComponent?.GetUserInterface()?.NavigateAction);
                    if (!inputVec.IsZeroApprox())
                    {
                        float scrollAmount = inputVec.Dot(scrollDir);
                        speed += ManualScrollSpeed * scrollAmount;
                        manualScrolling = true;
                    }
                }
            }
        }
        if(speed != 0)
            _scrollVelocity = scrollDir * (float)(speed * delta);

        var intendedDiff = _scrollVelocity + _scrollSubPixelDebt;
        if (intendedDiff.IsZeroApprox()) return;
        _scrollSubPixelDebt = Vector2.Zero;

        if (ScrollContainer != null)
        {
            var integerDiff = new Vector2((int)intendedDiff.X, (int)intendedDiff.Y);
            _scrollSubPixelDebt = new Vector2(intendedDiff.X % 1, intendedDiff.Y % 1);
            var preScrollAmount = new Vector2(ScrollContainer.ScrollHorizontal, ScrollContainer.ScrollVertical);
            ScrollContainer.ScrollHorizontal += (int)integerDiff.X;
            ScrollContainer.ScrollVertical += (int)integerDiff.Y;
            var postScrollAmount = new Vector2(ScrollContainer.ScrollHorizontal, ScrollContainer.ScrollVertical);
            var actualDiff = postScrollAmount - preScrollAmount;
            if (_reachedEnd && !actualDiff.IsZeroApprox()) _reachedEnd = false;
            if (!_reachedEnd && !integerDiff.IsZeroApprox() && actualDiff.Length() < integerDiff.Length() &&
                (integerDiff.X > 0 || integerDiff.Y > 0))
            {
                // GD.Print("Reached end");
                _reachedEnd = true;
                EmitSignalReachedEnd();
                EmitSignalUpdated();
            }
        }

        if(!manualScrolling)
            _scrollVelocity = ExpDecay.LerpOverTime(_scrollVelocity, Vector2.Zero, (float)(delta * ScrollDecay));
    }

    public void Reset()
    {
        ResetVelocity();
        if (ScrollContainer != null)
            ScrollContainer.ScrollHorizontal = ScrollContainer.ScrollVertical = 0;
        EmitSignalUpdated();
    }

    public void ResetVelocity()
    {
        _scrollVelocity = Vector2.Zero;
        _reachedEnd = false;
    }

    private bool UpdateApplicablePlayerIds()
    {
        _tempPlayerIdArray.Clear();
        _tempPlayerIdArray.AddRange(_applicablePlayerIds);
        // _tempPlayerIdArray now contains IDs of players who *could* scroll manually last frame.
        _applicablePlayerIds.Clear();
        
        bool anyNewAdded = false;
        bool anyRemoved = false;
        if (ManualScroll && _uiComponent?.GetUserInterface() is { } focusInterface)
        {
            foreach (int playerId in focusInterface.GetAllFocusedPlayerIds())
            {
                if (CanManualScroll(playerId, out _))
                {
                    if (!_tempPlayerIdArray.Remove(playerId))
                    {
                        anyNewAdded = true;
                    }
                    _applicablePlayerIds.Add(playerId);
                }
            }
        }
        // by now, _tempPlayerIdArray contains players who *could* scroll manually last frame, but now cannot.
        anyRemoved = _tempPlayerIdArray.Count > 0;
        
        _tempPlayerIdArray.Clear();

        return anyNewAdded || anyRemoved;
    }

    public StringName UpdatedSignalName => SignalName.Updated;

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "can_manual_scroll":
            {
                return this.OutputBooleanVariable(_applicablePlayerIds.Count > 0, ref output, input);
            }
            case "scrollbar_visible":
            case "is_scrollbar_visible":
            {
                return this.OutputBooleanVariable(_scrollBarVisible, ref output, input);
            }
            case "player_ids":
            {
                output = _applicablePlayerIds;
                return true;
            }
            case "manual_scroll_action":
            {
                output = ManualScrollAction;
                return true;
            }
        }

        return false;
    }
}