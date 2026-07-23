using GDF.Data;
using Godot;

namespace Game.Player;

public class AttackInstanceContext : IDataContext
{
    public Node Source;
    public Vector2 OriginPos;
    public Vector2 TargetPos;
    public Vector2 TargetDir;

    public AttackInstanceContext(Node source, Vector2 originPos, Vector2 targetPos)
    {
        Source = source;
        OriginPos = originPos;
        TargetPos = targetPos;
        TargetDir = (targetPos - originPos).Normalized();
    }

    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "source":
            {
                output = Source;
                return true;
            }
            case "origin_pos":
            case "origin_position":
            {
                output = OriginPos;
                return true;
            }
            case "target_pos":
            case "target_position":
            {
                output = TargetPos;
                return true;
            }
            case "target_dir":
            case "target_direction":
            {
                output = TargetDir;
                return true;
            }
            case "rotation":
            {
                output = TargetDir.Angle();
                return true;
            }
        }

        return false;
    }
}