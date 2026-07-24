using System.Collections.Generic;
using Game.Timers;
using GDF.Data;
using GDF.Data.Static;
using Godot;

namespace Game.Player;

[StaticDataContext("player_position_context")]
public struct PlayerPositionContext : IDataContext, ICacheableDataContext<PlayerPositionContext>
{
    private static readonly List<Vector2> TempPlayerCharacterPositions = new();
    public static Vector2? FindNearbyOpenPosition()
    {
        TempPlayerCharacterPositions.Clear();
        var playerCharacters = GameTimer.Instance.GetTree().GetNodesInGroup("player_character");
        if(playerCharacters.Count == 0) return null;
        if (playerCharacters.Count >= 9) return null;
        foreach (var node in playerCharacters)
        {
            if(node is Node2D node2D) TempPlayerCharacterPositions.Add(node2D.GlobalPosition);
        }

        if (TempPlayerCharacterPositions.Count == 0) return null;

        var centerPos = TempPlayerCharacterPositions[0];

        Vector2? result = null;

        foreach (var radius in new float[] { 60f })
        {
            for (float angle = 0; angle < Mathf.Tau; angle += Mathf.Tau / 8)
            {
                var candidatePos = centerPos + (Vector2.Right.Rotated(angle) * radius);
                if (IsOpenPosition(candidatePos, 40))
                {
                    result = candidatePos;
                    break;
                }
            }

            if (result.HasValue) break;
        }

        TempPlayerCharacterPositions.Clear();
        return result;
    }

    private static bool IsOpenPosition(Vector2 candidatePos, float minDistance)
    {
        foreach (var occupiedPos in TempPlayerCharacterPositions)
        {
            if (occupiedPos.DistanceSquaredTo(candidatePos) < minDistance * minDistance) return false;
        }

        return true;
    }
    
    public bool GetContextVariable(string key, string input, ref Variant output, IDataQueryOptions options)
    {
        switch (key)
        {
            case "nearby_open_position_for_copy":
            {
                if (FindNearbyOpenPosition() is { } pos)
                {
                    output = pos;
                    return true;
                }
                return false;
            }
        }

        return false;
    }

    public bool EqualsContext(PlayerPositionContext otherCtx) => true;

    public bool CanCache() => true;
}