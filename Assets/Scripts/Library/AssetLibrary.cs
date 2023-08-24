using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AssetLibrary<T> where T:Asset
{
    private Dictionary<string,T>dictionary = new Dictionary<string, T>();
    private List<T>list = new List<T>();
    public abstract void Init();

    public void Add(T _object) {
        if(dictionary.ContainsKey(_object.id) == true) {
            Debug.LogError($"{_object.id} is already in library");
            return;
        }
        dictionary.Add(_object.id,_object);
        list.Add(_object);
    } 

    public T Get(string id) {
        return dictionary[id];
    }

    public T Get(int index) {
        return list[index];
    }
}
