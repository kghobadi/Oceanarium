using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObject : MonoBehaviour {

    [HideInInspector] public ObjectPooler m_ObjectPooler;
    [HideInInspector] public Vector3 originalScale;
    
    public void ReturnToPool() {
        m_ObjectPooler.ReturnObject(gameObject);
    }
}
