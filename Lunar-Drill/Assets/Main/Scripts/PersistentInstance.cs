using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentInstance : MonoBehaviour
{
    static PersistentInstance instance;

    private void Awake()
    {
        if (instance == null)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }
}
