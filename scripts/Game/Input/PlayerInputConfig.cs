using System.Collections.Generic;
using System.Linq;
using GDF.Input;
using GDF.Multiplayer;
using Godot;
using Godot.Collections;

namespace Game.Input;

[Tool]
[GlobalClass]
public partial class PlayerInputConfig : GdfInputAutoConfiguration
{
    [Export(PropertyHint.Enum, "All Connected Devices,All Players,Unique Per Player")]
    public InputConfigMode Mode = InputConfigMode.AllConnectedDevices;
    [Export] public PackedScene JoypadInputContext;
    [Export] public PackedScene[] KeyboardInputContexts;
    
    public override void Configure(GdfPlayerInput input)
    {
        if (!Room.InstanceExists)
        {
            Configure(input, new Room.PlayerInfo()
            {
                IndexInClient = 0,
                PeerId = 1,
                PlayerId = 0
            });
            return;
        }
        if (Room.Instance.TryGetPlayerInfo(input.PlayerId, out var playerInfo)) // Connected player
            Configure(input, playerInfo);
        else if (input.PlayerId != -1) // Disconnected player
            Configure(input, new Room.PlayerInfo()
            {
                PeerId = -1,
                IndexInClient = -1,
                PlayerId = input.PlayerId
            });
        else // Playerless
            Configure(input, new Room.PlayerInfo()
            {
                PeerId = Room.Instance.PeerId,
                IndexInClient = 0,
                PlayerId = -1
            });
    }

    public void Configure(GdfPlayerInput input, Room.PlayerInfo playerInfo)
    {
        input.PlayerId = playerInfo.PlayerId;
        if (playerInfo.IsLocal)
        {
            int indexInClient = playerInfo.IndexInClient;
            // SaveSystem.Settings.InputOrderConfig.SanitizeInputOrder();

            input.InputContexts = new Array<PackedScene>();
            input.UsesKeyboard = false; // overwrite below

            switch (Mode)
            {
                case InputConfigMode.UniquePerPlayer:
                {
                    var config = GetInputConfigForLocalIndex(indexInClient);

                    if (config.IsEmpty) break;

                    input.DeviceIndices = config.DeviceIndex != -1
                        ? new[] { config.DeviceIndex }
                        : Godot.Input.GetConnectedJoypads().ToArray();

                    foreach (var scene in EnumerateInputContexts())
                    {
                        var context = GdfInputSystem.Instance.GetContextInstance(scene);

                        var include = false;
                        foreach (string tag in config.InputContextTags)
                            if (context.HasTag(tag))
                            {
                                include = true;
                                if (tag.Contains("keyboard"))
                                {
                                    input.UsesKeyboard = true;
                                    input.UsesMouse = true;
                                }
                                break;
                            }

                        if (include) input.InputContexts.Add(scene);
                    }

                    break;
                }
                case InputConfigMode.AllPlayers:
                {
                    input.DeviceIndices = Godot.Input.GetConnectedJoypads().ToArray();

                    foreach (var scene in EnumerateInputContexts())
                    {
                        var context = GdfInputSystem.Instance.GetContextInstance(scene);

                        var include = false;
                        for (var localIndex = 0; localIndex < Room.Instance.LocalPlayerCount && !include; localIndex++)
                        {
                            var config = GetInputConfigForLocalIndex(localIndex);
                            if (config.IsEmpty) continue;

                            foreach (string tag in config.InputContextTags)
                                if (context.HasTag(tag))
                                {
                                    include = true;
                                    if (tag.Contains("keyboard"))
                                    {
                                        input.UsesKeyboard = true;
                                        input.UsesMouse = true;
                                    }
                                    break;
                                }
                        }

                        if (include) input.InputContexts.Add(scene);
                    }

                    break;
                }
                case InputConfigMode.AllConnectedDevices:
                {
                    input.DeviceIndices = Godot.Input.GetConnectedJoypads().ToArray();
                    if (KeyboardInputContexts is { Length: > 0 })
                        input.UsesKeyboard = true;
                    foreach (var scene in EnumerateInputContexts()) input.InputContexts.Add(scene);
                    break;
                }
            }
        }
        else
        {
            input.DeviceIndices = System.Array.Empty<int>();
            input.InputContexts = new Array<PackedScene>();
        }
    }

    private InputOrderEntry GetInputConfigForLocalIndex(int indexInClient)
    {
        // TODO do we want local multiplayer? splitscreen?
        return new InputOrderEntry()
        {
            DeviceIndex = -1,
            InputContextTags = new List<string> { "keyboard" }
        };
    }

    private IEnumerable<PackedScene> EnumerateInputContexts()
    {
        if (JoypadInputContext != null) yield return JoypadInputContext;
        if (KeyboardInputContexts != null)
            foreach (var context in KeyboardInputContexts)
                yield return context;
    }

    public override void ConnectUpdateSignal(Callable callable)
    {
    }

    public override void DisconnectUpdateSignal(Callable callable)
    {
    }

    public enum InputConfigMode
    {
        AllConnectedDevices,
        AllPlayers,
        UniquePerPlayer,
    }
}