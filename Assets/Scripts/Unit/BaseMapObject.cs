using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseMapObject : MonoBehaviour
{

    public Material defaultMaterial;
    public Material highlightedMaterial;
    public SpriteRenderer spriteRenderer;

    public bool isHighlighted = false;
    
    public Vector3Int tile {
        get; private set;
    } = new Vector3Int(0,0,1);

    void Awake() {
        
    }

    void Start()
    {
       
    }

    void Update()
    {
        
    }

    public virtual void SetTile(Vector3Int tile, bool rightDirection = false) {
        if(this.tile!=new Vector3Int(0,0,1)) BattleMap.instance.DeoccupyTile(this);
        this.tile = tile;
        this.SetPosition(TileManager.instance.TileToWorld(tile));
 
        spriteRenderer.flipX = rightDirection;
        BattleMap.instance.OccupyTile(this);
    }

    private void SetPosition(Vector3 pos) {
        gameObject.transform.position = pos;
    }

    public virtual void Highlight(bool highlight) {
        this.isHighlighted = highlight;
        this.spriteRenderer.material = isHighlighted ? highlightedMaterial : defaultMaterial;
    }

    public virtual void UpdateNeighbourTile(Vector3Int tile) {

    }

     public virtual void Destroy() {
        Destroy(this.gameObject);
    }
}
