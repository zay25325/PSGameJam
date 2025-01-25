using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectClearer : MonoBehaviour
{
    public void ClearEffect()
    {
        GameObject.Destroy(gameObject);
    }
}
