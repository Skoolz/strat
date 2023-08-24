using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageRangeUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void SetInfo(int min, int max) {
        text.text = $"{min}-{max}";
    }
}
