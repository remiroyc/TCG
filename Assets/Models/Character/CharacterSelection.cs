using System;
using UnityEngine;

[Serializable]
public class CharacterSelection
{
    [HideInInspector]
    public GameObject InstantiatedCharacter;
    public GameObject PrefabCharacter;
    public bool IsAvailable;
}
