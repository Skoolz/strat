using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveAbilityUI : MonoBehaviour
{
    public ActiveAbility activeAbility;
    public Button button;
    public Text text;

    public void Init(ActiveAbility activeAbility) {
        this.activeAbility = activeAbility;
        button.interactable = activeAbility.CanActivate();
        activeAbility.OnCast+=UpdateInfo;
        text.text = activeAbility.id;
    }

    public void SwitchAbility() {
        if(activeAbility.IsActivated) activeAbility.unit.DeselectActiveAbility();
        else activeAbility.unit.SelectActiveAbility(activeAbility.id);
    }

    public void UpdateInfo(ActiveAbility _) {
        button.interactable = activeAbility.CanActivate();
    }

    public void Clear() {
        activeAbility.OnCast-=UpdateInfo;
    }
}
