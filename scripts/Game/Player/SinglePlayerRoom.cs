using System.Collections.Generic;
using GDF.Multiplayer;
using Godot;

namespace Game.Player;

[GlobalClass]
public partial class SinglePlayerRoom : Room
{
    public List<PlayerInfo> AllPlayers = new()
    {
        new PlayerInfo()
        {
            IndexInClient = 0,
            PeerId = 1,
            PlayerId = 0
        }
    };
    public List<int> AllPeers = new()
    {
        1
    };
    
    public override List<int> GetAllPeerIds()
    {
        return AllPeers;
    }

    public override List<PlayerInfo> GetAllPlayerInfo()
    {
        return AllPlayers;
    }

    public override int PeerId => 1;
}