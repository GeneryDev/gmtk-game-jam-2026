using GDF.Data;
using Godot;

namespace Game.UI;

public partial class HSliderExtension : HSlider, IDataContext
{
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "value":
            {
                output = Value;
                return true;
            }
        }

        return false;
    }

    public void Increment()
    {
        Value += Step;
    }

    public void Decrement()
    {
        Value -= Step;
    }
}