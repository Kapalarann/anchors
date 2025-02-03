using UnityEngine;

public abstract class Spell : MonoBehaviour
{
    public GameObject spellPrefab;
    public abstract void Cast(Vector3 target);
}
