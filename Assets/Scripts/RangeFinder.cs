using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeFinder
{
    public static Dictionary<Vector3Int,ReachTile> GetReachableTiles(Vector3Int tile, int range, bool disableCheckMoveable = false) {
        var map = BattleMap.instance;

        List<Vector3Int> visited = new List<Vector3Int>();

        Queue<ReachTile> check = new Queue<ReachTile>();

        Dictionary<Vector3Int,ReachTile>reachable = new Dictionary<Vector3Int,ReachTile>();

        ReachTile firstTile = new ReachTile(tile,null,range);

        check.Enqueue(firstTile);
        visited.Add(tile);
        reachable.Add(firstTile.tile,firstTile);

        while(check.Count > 0) {
            var currentTile = check.Dequeue();
            if(currentTile.counter >= range) continue;
            var neighbours = TileManager.instance.GetNeighbours(currentTile.tile);
            foreach (Vector3Int movePos in neighbours) {
                if(map.IsInBounds(movePos) &&
                    movePos != tile) 
                    {
                        if(BattleMap.instance.IsMoveable(movePos) || disableCheckMoveable) {
                            ReachTile newTile = new ReachTile(movePos,currentTile,range);
                            if(newTile.counter < range) check.Enqueue(newTile);
                            if(reachable.ContainsKey(movePos) == false) {
                                reachable.Add(movePos,newTile);
                            }
                        }
                    }
            }
        }
        return reachable;
    }
}


public class ReachTile {
    public int counter {get; private set;}
    public Vector3Int tile;
    public bool action = false;

    public ReachTile(Vector3Int tile,ReachTile prev, int range) {
        this.tile = tile;
        if(prev != null)
            this.counter = prev.counter+1;
        else
            this.counter = 0;
        if(this.counter == range) {
            action = false;
        }
    }
}