using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectSelector : MonoBehaviour
{
    BaseMapObject selectedObject;
    public bool disable {get; private set;} = false;
    public static MapObjectSelector instance {get; private set;}

    void Awake() {
        instance = this;
    }

    void Update() {
        if(BattleMap.instance.IsInBounds(SelectTile.instance.saved_tile) == false || disable) {
            return;
        }
        BaseMapObject _object = BattleMap.instance.GetOccupiedTile(SelectTile.instance.saved_tile);
        if(_object != selectedObject) {
            Highlight(selectedObject);
        }
    }

    public void Highlight(BaseMapObject baseMapObject) {
        Unhighlight();
        selectedObject = baseMapObject;
        selectedObject?.Highlight(true);
    }

    public void Unhighlight() {
        selectedObject?.Highlight(false);
        selectedObject = null;
    }

    public void Disable(bool disable) {
        if(this.disable == disable) return;
        this.disable = disable;

        if(disable) {
            Unhighlight();
        }
    }
}
