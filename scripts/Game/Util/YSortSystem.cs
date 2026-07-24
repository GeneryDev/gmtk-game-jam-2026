using System.Collections.Generic;
using GDF.Util;
using Godot;

namespace Game.Util;

[SingletonUsage(SingletonUsage.Scene)]
public partial class YSortSystem : SingletonNode<YSortSystem>
{
    private readonly List<YSorted> _all = new();

    public void Add(YSorted item)
    {
        _all.Add(item);
    }
    public void Remove(YSorted item)
    {
        _all.Remove(item);
    }

    public void Sort()
    {
        float? keyMin = null;
        float? keyMax = null;
        
        // Grab and cache global position once per item
        foreach (var item in _all)
        {
            item.UpdateSortKey();
            float key = item.SortKey;
            keyMin = Mathf.Min(key, keyMin ?? key);
            keyMax = Mathf.Max(key, keyMax ?? key);
        }

        if (keyMin == null) return;

        foreach (var item in _all)
        {
            float key = item.SortKey;
            float t = (key - keyMin.Value) / (keyMax.Value - keyMin.Value);
            int zIndex = Mathf.RoundToInt(Mathf.Lerp(RenderingServer.CanvasItemZMin, RenderingServer.CanvasItemZMax, t));
            item.SetZIndex(zIndex);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Sort();
    }
}