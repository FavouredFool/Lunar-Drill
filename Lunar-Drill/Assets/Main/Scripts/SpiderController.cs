using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : MonoBehaviour
{
    //--- Exposed Fields ------------------------

    [SerializeField][Range(2, 5)] float _innerOrbitRange = 3;
    [SerializeField][Range(0.1f, 10f)] float _rotationSpeed = 5f;


    //--- Private Fields ------------------------

    Rigidbody2D _rigidbody;


    //--- Unity Methods ------------------------

    public void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        
    }


    //--- Public Methods ------------------------
}
