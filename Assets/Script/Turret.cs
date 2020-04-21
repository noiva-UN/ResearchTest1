using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Enemy
{

    [SerializeField] private float coolDown = 2f;
    private float mathTime = 0;
    
    // Update is called once per frame
    void Update()
    {
        if (coolDown * (101 - difficulty)/50 <= mathTime)
        {
            Shot((int) pow, _player.transform.position - transform.position);
            mathTime = 0;
        }
        else
        {
            mathTime += Time.deltaTime;
        }
        
        LifeCheck();
    }

    public override void Initialized(int diff, Vector3 pos)
    {
        base.Initialized(diff, pos);

        rb.velocity = Vector3.down * (float)speed;
        mathTime = 0;
    }

    private void Shot(int pow, Vector3 vec)
    {

        _bulletManager.Shot(transform.position, (float) speed * difficulty / 50, vec, "Enemy");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(transform.tag))
        {
            other.gameObject.SetActive(false);
            hp = 0;
        }
    }
}
