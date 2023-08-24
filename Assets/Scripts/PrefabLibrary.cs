using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabLibrary : MonoBehaviour
{
    public static PrefabLibrary instance {get; private set;}

    [Header("units")]
    public GameObject knight;
    public GameObject archer;
    public GameObject skeleton;
    public GameObject skeletonDeriveEmpty;
    public GameObject skeletonDeriveHalf;
    public GameObject skeletonDeriveFull;
    public GameObject zombie;
    public GameObject cannon;

    [Header("ui")]
    public GameObject damageRange;
    public GameObject abilityUI;

    private void Awake() {
        instance = this;
    }

}
