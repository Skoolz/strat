using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SelectFormType {
    tile,
    three,
    twohex,
}
public class SelectForm
{
    SelectFormType formType;
    public List<Vector3Int>oddOffsets;
    public List<Vector3Int>evenOffsets;

    public List<Vector3Int> GetOffsets(Vector3Int tile) {
        return tile.y%2 == 0 ? evenOffsets : oddOffsets;
    }


    public static Dictionary<SelectFormType,SelectForm>forms;

    public static void Init() {
        forms = new Dictionary<SelectFormType, SelectForm>();

        var form = new SelectForm() {
            formType = SelectFormType.tile,
            oddOffsets = new List<Vector3Int>() {new Vector3Int(0,0,0)},
            evenOffsets = new List<Vector3Int>() {new Vector3Int(0,0,0)},
        };
        forms.Add(form.formType,form);

        form = new SelectForm() {
            formType = SelectFormType.three,
            evenOffsets = new List<Vector3Int>() {
                new Vector3Int(0,0,0),
                new Vector3Int(0,1,0),
                new Vector3Int(1,0,0)
            },
            oddOffsets = new List<Vector3Int>() {
                new Vector3Int(0,0,0),
                new Vector3Int(1,1,0),
                new Vector3Int(1,0,0)
            },
        };
        forms.Add(form.formType,form);

        form = new SelectForm() {
            formType = SelectFormType.twohex,
            evenOffsets = new List<Vector3Int>() {
                new Vector3Int(0,0,0),
                new Vector3Int(-1,1,0),
                new Vector3Int(0,1,0),
                new Vector3Int(1,0,0),
                new Vector3Int(1,1,0),
                new Vector3Int(-1,-1,0),
                new Vector3Int(0,-1,0),
                new Vector3Int(1,-1,0),
                new Vector3Int(0,2,0),
                new Vector3Int(1,2,0),
                new Vector3Int(0,-2,0),
                new Vector3Int(1,-2,0)
            },
            oddOffsets = new List<Vector3Int>() {
                new Vector3Int(0,0,0),
                new Vector3Int(0,1,0),
                new Vector3Int(1,1,0),
                new Vector3Int(1,0,0),
                new Vector3Int(2,1,0),
                new Vector3Int(0,2,0),
                new Vector3Int(1,2,0),
                new Vector3Int(0,-1,0),
                new Vector3Int(1,-1,0),
                new Vector3Int(2,-1,0),
                new Vector3Int(0,-2,0),
                new Vector3Int(1,-2,0),
            },
        };
        forms.Add(form.formType, form);
    }
}
