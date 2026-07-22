using GDF.Input.Icons;
using Godot;

namespace Game.Input;

[Tool]
[GlobalClass]
public partial class SelfUpdatingInputIcon : GdfInputIconAction
{
    public static readonly StringName Group = "input_icon";
    
    [Export] public bool ChangeColorOnPress = false;
    [Export] public Color PressedColor = Colors.Gold;
    [Export] public bool RestrictToAncestorUI = true;
    
    // TODO

    // public override void _EnterTree()
    // {
    //     base._EnterTree();
    //     if (Engine.IsEditorHint()) return;
    //     this.AddToGroup(Group);
    //     GdfInputSystem.Instance.PlayerChangedInputDevice += OnPlayerChangedInputDevice;
    //     GdfInputSystem.Instance.PlayerInputNodesUpdated += OnPlayerInputNodesUpdated;
    //
    //     UpdateIconStyle();
    //     ExecuteSelfUpdate();
    //     //CommonSignalListeners.Connect(CommonSignalListeners.SignalName.SettingsDataUpdated, new Callable(this, MethodName.OnSettingsUpdated));
    // }
    //
    // public override void _ExitTree()
    // {
    //     base._EnterTree();
    //     if (Engine.IsEditorHint()) return;
    //     GdfInputSystem.Instance.PlayerChangedInputDevice -= OnPlayerChangedInputDevice;
    //     GdfInputSystem.Instance.PlayerInputNodesUpdated -= OnPlayerInputNodesUpdated;
    //     //CommonSignalListeners.Disconnect(CommonSignalListeners.SignalName.SettingsDataUpdated, new Callable(this, MethodName.OnSettingsUpdated));
    // }
    //
    // private void UpdateIconStyle()
    // {
    //     var preferredStyle = SaveSystem.Settings.PreferredInputIconStyle;
    //     if (preferredStyle != this.IconStyle)
    //     {
    //         this.IconStyle = preferredStyle;
    //         this.Update();
    //     }
    // }
    //
    // private void OnSettingsUpdated()
    // {
    //     UpdateIconStyle();
    //     this.Update();
    // }
    //
    // public override void _Process(double delta)
    // {
    //     base._Process(delta);
    //     if (ChangeColorOnPress) Modulate = IsActionPressed() ? PressedColor : Colors.White;
    // }
    //
    // public bool IsActionPressed()
    // {
    //     if (Engine.IsEditorHint()) return false;
    //     foreach (var player in GdfInputSystem.Instance.GetPlayerNodes())
    //         if (player.GetActionState(Action).Strength > 0 && AcceptsPlayerId(player.PlayerId))
    //             return true;
    //
    //     return false;
    // }
    //
    // private void OnPlayerInputNodesUpdated()
    // {
    //     ExecuteSelfUpdate();
    // }
    //
    // public void OnPlayerChangedInputDevice(int playerId)
    // {
    //     if (!AcceptsPlayerId(playerId)) return;
    //
    //     ExecuteSelfUpdate();
    // }
    //
    // private void ExecuteSelfUpdate()
    // {
    //     if (Engine.IsEditorHint()) return;
    //     if (OS.GetThreadCallerId() != OS.GetMainThreadId())
    //     {
    //         CallDeferred(GDF.Input.Icons.SelfUpdatingInputIcon.MethodName.ExecuteSelfUpdate);
    //         return;
    //     }
    //     PrepareToModify();
    //     LimitToContextTags ??= new Array<string>();
    //     LimitToContextTags.Clear();
    //     LimitToPlayerInputNodes ??= new Array<GdfPlayerInput>();
    //     LimitToPlayerInputNodes.Clear();
    //     JoypadType = KenneyIconConstants.JoypadType.Xbox;
    //
    //     if (LimitToPlayerIds is {Count: > 0})
    //     {
    //         foreach (int playerId in LimitToPlayerIds)
    //             AddConstraintsForPlayer(playerId);
    //     }
    //     else if(Room.InstanceExists)
    //     {
    //         foreach (var playerInfo in Room.Instance.GetAllPlayerInfo())
    //         {
    //             if(playerInfo.IsLocal) AddConstraintsForPlayer(playerInfo.PlayerId);
    //         }
    //     }
    //     UpdateDescriptor();
    // }
    //
    // private void AddConstraintsForPlayer(int playerId)
    // {
    //     GetInputTagAndType(playerId, out string contextTag, out var joypadType, out var playerInputNode);
    //     if(contextTag != null)
    //         LimitToContextTags.Add(contextTag);
    //     if(joypadType.HasValue)
    //         JoypadType = joypadType.Value;
    //     if (RestrictToAncestorUI && playerInputNode != null)
    //         LimitToPlayerInputNodes.Add(playerInputNode);
    // }
    //
    // private void GetInputTagAndType(int playerId, out string contextTag, out KenneyIconConstants.JoypadType? joypadType, out GdfPlayerInput playerInputNode)
    // {
    //     var lastUsed = GdfInputSystem.Instance.GetLastPlayerInput(playerId);
    //
    //     if (lastUsed.Context?.Tags is { Count: >= 1 } tags)
    //         contextTag = tags[0];
    //     else
    //         contextTag = null;
    //
    //     if (lastUsed.Device != default)
    //         joypadType = lastUsed.Device.GetIconJoypadType();
    //     else
    //         joypadType = InputSettingsMapper.Instance.GetLastUsedJoypadDeviceForPlayer(playerId)?.GetIconJoypadType();
    //     if (!joypadType.HasValue)
    //         joypadType = InputSettingsMapper.Instance.LastUsedJoypadDevice.GetIconJoypadType() ??
    //                      GdfInputIconJoypadType.Xbox;
    //
    //     playerInputNode = null;
    //
    //     var ui = UserInterfaceRoot.FindAncestorUI(this);
    //     if (ui?.FocusInterface is { } focusInterface)
    //     {
    //         playerInputNode = focusInterface.GetPlayerInput(playerId);
    //     }
    // }
    //
    // protected override void OnQueryChanged()
    // {
    //     ExecuteSelfUpdate();
    // }
}