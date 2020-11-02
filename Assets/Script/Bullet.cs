using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private Rigidbody _rigidbody;

    [SerializeField] private Vector2 deleteLine;

    private BulletManager _bulletManager;

    private int pow = 10;
    public int Pow => pow;
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.y) >= deleteLine.y || Mathf.Abs(transform.position.x) >= deleteLine.x)
        {
            _bulletManager.Delete(this);
        }
    }

    public void SetUp(BulletManager bulletManager)
    {
        _rigidbody = GetComponent<Rigidbody>();
        _bulletManager = bulletManager;
    }
    
    public void Initialize(Vector3 homePos, float speed, string tag)
    {
        transform.position = homePos;
        _rigidbody.velocity = Vector3.up * speed;
        
        //if(gameObject.CompareTag(tag)) return;
        
        gameObject.tag = tag;
        gameObject.GetComponent<Renderer>().material.color = tag == "Player" ? Color.blue : Color.red;

    }
    
    public void Initialize(Vector3 homePos, float speed, Vector3 vec, string tag)
    {
        transform.position = homePos;
        _rigidbody.velocity = vec * speed;
        
        //if(gameObject.CompareTag(tag)) return;
        gameObject.tag = tag;
        
        gameObject.GetComponent<Renderer>().material.color = tag == "Player" ? Color.blue : Color.red;
    }
    
}
