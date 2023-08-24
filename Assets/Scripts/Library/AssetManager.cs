using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager
{
    public StatsLibrary statsLibrary {get; private set;}
    public AbilityLibrary abilityLibrary {get; private set;}
    public ActiveAbilityLibrary activeAbilityLibrary {get; private set;}

    public void Init() {
        statsLibrary = new StatsLibrary();
        statsLibrary.Init();
        abilityLibrary = new AbilityLibrary();
        abilityLibrary.Init();
        activeAbilityLibrary = new ActiveAbilityLibrary();
        activeAbilityLibrary.Init();

        SelectForm.Init();
    }
}
