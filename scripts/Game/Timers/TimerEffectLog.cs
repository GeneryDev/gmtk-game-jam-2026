using System.Collections.Generic;
using GDF.Data;
using GDF.Data.Static;
using GDF.Util;
using Godot;

namespace Game.Timers;

[SingletonUsage(SingletonUsage.Scene)]
public partial class TimerEffectLog : SingletonNode<TimerEffectLog>, IDataContext
{
    [Signal]
    public delegate void UpdatedEventHandler();
    
    [Export] public int MaxItems = 10;
    private readonly List<Item> _items = new();

    public void Log(string message)
    {
        GD.Print(message);
        var item = new Item()
        {
            Message = message
        };
        if (_items.Count >= MaxItems)
        {
            _items.RemoveAt(0);
        }
        _items.Add(item);
        EmitSignalUpdated();
    }

    public StringName UpdatedSignalName => SignalName.Updated;

    public bool GetContextCollection(string key, string input, List<IDataContext> output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "messages":
            {
                output.AddRange(_items);
                return true;
            }
        }

        return false;
    }


    public class Item : IDataContext
    {
        public string Message = "";

        public bool GetContextString(string key, string input, ref string replacement, IDataQueryOptions options)
        {
            switch (key)
            {
                case "message":
                {
                    replacement = Message;
                    return true;
                }
            }

            return false;
        }
    }
}

[StaticDataContext("timer_effect_log_context")]
public struct TimerEffectLogContext : ISingletonContext<TimerEffectLog>, ICacheableDataContext<TimerEffectLogContext>
{
    public bool EqualsContext(TimerEffectLogContext otherCtx) => true;

    public bool CanCache() => true;
}