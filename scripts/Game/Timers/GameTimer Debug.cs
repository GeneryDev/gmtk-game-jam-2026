using GDF.Debug;
using Godot;

namespace Game.Timers;

[HasDebugCommands]
public partial class GameTimer
{
    [DebugCommand("timer", DebugCommandType.TriggerWithArguments)]
    public static void DebugSpeed(DebugCommandArgumentParser args)
    {
        if (args.ReadWord(out var subcommand0))
        {
            switch (subcommand0)
            {
                case "set":
                {
                    if (args.ReadInt(out var newTime))
                    {
                        Instance.RemainingTime = newTime;
                        GD.Print($"Set timer time: {Instance.RemainingTime}");
                    }
                    else
                    {
                        args.PrintError();
                    }

                    break;
                }
                case "add":
                {
                    if (args.ReadInt(out var offset))
                    {
                        Instance.RemainingTime += offset;
                        GD.Print($"Set timer time: {Instance.RemainingTime}");
                    }
                    else
                    {
                        args.PrintError();
                    }

                    break;
                }
                case "speed":
                {
                    if (args.ReadWord(out var subcommand1))
                    {
                        switch (subcommand1)
                        {
                            case "set":
                            {
                                if (args.ReadFloat(out var newSpeed))
                                {
                                    Instance.TickRate = newSpeed;
                                    GD.Print($"Set timer tick rate: {Instance.TickRate}");
                                }
                                else
                                {
                                    args.PrintError();
                                }
                                break;
                            }
                            case "multiply":
                            {
                                if (args.ReadFloat(out var multiplier))
                                {
                                    Instance.TickRate *= multiplier;
                                    GD.Print($"Set timer tick rate: {Instance.TickRate}");
                                }
                                else
                                {
                                    args.PrintError();
                                }
                                break;
                            }
                            default:
                            {
                                args.PrintCustomError($"No such subcommand '{subcommand1}'");
                                break;
                            }
                        }
                    }
                    else
                    {
                        args.PrintError();
                    }

                    break;
                }
            }
        }
        else
        {
            args.PrintError();
        }
        
    }
}