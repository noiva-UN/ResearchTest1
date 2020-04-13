using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private Rigidbody _rigidbody;
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    public void Initialize(Vector3 homePos, float speed)
    {
        transform.position = homePos;
        _rigidbody.velocity = Vector3.up * speed;
    }
}
