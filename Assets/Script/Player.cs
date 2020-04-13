using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int sideLimit, hightLimit;
    [SerializeField] private int hp, pow;
    [SerializeField] private float speed, coolDown;
    private float mathTime = 0;
    
    [SerializeField] private BulletManager _bulletManager;
    private Vector3 newPos = new Vector3();
    
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
        mathTime += Time.deltaTime;
    }

    private void PlayerMove()
    {
        newPos = transform.position;
        var horizon = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizon) >= 0.1)
        {
            newPos.x += horizon;
            if (Mathf.Abs(newPos.x) >= sideLimit)
            {
                newPos.x = Mathf.Sign(newPos.x) * sideLimit;
            }
        }

        var vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs((vertical)) >= 0.1)
        {
            newPos.y += vertical;
            if (Mathf.Abs(newPos.y) >= hightLimit)
            {
                newPos.y = Mathf.Sign(newPos.y) * hightLimit;
            }
        }
        
        transform.position = newPos;

        if (Input.GetAxis("Jump") != 0)
        {
            Debug.Log("Shot");
            Shot();
        }


    }

    private void Shot()
    {
        if (mathTime >= coolDown)
        {
            mathTime = 0;
            _bulletManager.Shot(transform.position, speed);
        }
    }
}
