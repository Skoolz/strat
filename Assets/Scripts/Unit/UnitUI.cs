using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class UnitUI : MonoBehaviour
{
    public TextMeshProUGUI size;
    public TextMeshProUGUI change;
    public Unit unit;

    private void Start() {
        unit.OnChange+=UpdateInfo;
        unit.OnHighlight+=Highlighted;
        size.text = unit.size.ToString();
    }


    public void UpdateInfo(Unit unit) {
        int currentSize = Convert.ToInt32(size.text);
        if(currentSize != unit.size) {
            size.text = unit.size.ToString();

            int diff = unit.size-currentSize;
            change.transform.localScale = new Vector3(0,0);
            change.transform.localPosition = size.transform.localPosition;
            change.text = diff.ToString();
            change.gameObject.SetActive(true);
            DOTween.Sequence().Join(change.transform.DOScale(1,0.5f)).
            Join(change.transform.DOLocalMoveY(-0.6f,0.5f)).AppendCallback(HideChange);
        }
    }

    public void Highlighted() {
        if(unit.isHighlighted && UnitController.instance.selectedUnit!=null && UnitController.instance.selectedUnit.IsEnemy(unit)) {
            var attack = UnitController.instance.selectedUnit;

            var range = BattleManager.instance.CalculateDamageRange(attack,unit);
            int min = range.min;
            int max = range.max;

            GameObject damageRange_gm = GameObject.Instantiate(PrefabLibrary.instance.damageRange);
            damageRange_gm.name = "DamageRange";

            DamageRangeUI damageRangeUI = damageRange_gm.GetComponent<DamageRangeUI>();
            damageRangeUI.SetInfo(min,max);

            damageRange_gm.transform.SetParent(this.transform);
            damageRange_gm.transform.localPosition = new Vector3(0,1.6f,0);
        }
        else if (unit.isHighlighted == false) {
            if(this.transform.Find("DamageRange")!=null) Destroy(this.transform.Find("DamageRange").gameObject);
        }
    }

    public void HideChange() {
        change.gameObject.SetActive(false);
    }
}
