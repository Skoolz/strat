using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "TitleData")]
public class TileData : ScriptableObject
{
    public TileBase tile;
    [SerializeField] public bool walkable;
    [NonSerialized] public bool occupied;

    public bool moveable {
        get {
            return walkable && occupied;
        }
    }
}
