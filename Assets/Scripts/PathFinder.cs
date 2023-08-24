using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public static PathTile Find(Vector3Int start, Vector3Int end,List<Vector3Int>range,out bool completed) {
        Queue<PathTile>tiles = new Queue<PathTile>();
        List<Vector3Int>visited = new List<Vector3Int>();

        tiles.Enqueue(new PathTile(start,null));
        visited.Add(start);
        completed = false;

        while(tiles.Count > 0) {
            var currentTile = tiles.Dequeue();
            
            var neighbours = TileManager.instance.GetNeighbours(currentTile.tile);

            foreach (var neighbour in neighbours) {
                if(neighbour == end) {
                    completed = true;
                    return new PathTile(neighbour,currentTile);
                }
                if(BattleMap.instance.IsInBounds(neighbour) &&
                    visited.Contains(neighbour) == false &&
                    range.Contains(neighbour) ) 
                    {
                        tiles.Enqueue(new PathTile(neighbour,currentTile));
                    }
                visited.Add(neighbour);
            }
        }
        return null;
    }
}

public class PathTile {
    public PathTile prev;
    public Vector3Int tile;
    public int counter = 0;

    public PathTile(Vector3Int tile, PathTile prev) {
        this.prev = prev;
        this.tile = tile;

        if(prev!=null) {
            this.counter = prev.counter+1;
        }
    }
}
