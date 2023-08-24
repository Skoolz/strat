using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAbilityManager : MonoBehaviour
{
    public List<ActiveAbilityUI>abilities = new List<ActiveAbilityUI>();

    public Unit selectedUnit;

    public void Init(Unit unit) {
        Clear();
        this.selectedUnit = unit;

        foreach(ActiveAbility activeAbility in unit.GetActiveAbilities()) {
            AddAbility(activeAbility);
        }
    }

    public void AddAbility(ActiveAbility activeAbility) {
        GameObject abilityUI_gm = GameObject.Instantiate(PrefabLibrary.instance.abilityUI);
        ActiveAbilityUI activeAbilityUI = abilityUI_gm.GetComponent<ActiveAbilityUI>();

        activeAbilityUI.Init(activeAbility);
        abilityUI_gm.transform.SetParent(this.transform);

        abilities.Add(activeAbilityUI);
    }

    public void Clear() {
        foreach(ActiveAbilityUI activeAbilityUI in abilities) {
            activeAbilityUI.Clear();
            GameObject.Destroy(activeAbilityUI.gameObject);
        }
        abilities.Clear();
    }
}
