using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected EnemyControl _enemyControl;
    protected Player _player;
    protected BulletManager _bulletManager;
    
    [SerializeField] protected double default_hp = 1, default_pow = 1;

    protected int difficulty = 50;
    protected double hp = 1, pow = 1, speed = 2;

    public bool inGame = false;
    
    protected Rigidbody rb;
    public void SetUp(EnemyControl control,Player player, BulletManager bulletManager)
    {
        _enemyControl = control;
        _player = player;
        _bulletManager = bulletManager;
        rb = GetComponent<Rigidbody>();
    }

    public virtual void  Initialized(int diff, Vector3 pos, bool ingame)
    {
        gameObject.SetActive(true);
        hp = 2;
        //hp = Mathf.Max((int)default_hp, (int)(default_hp * (diff / 25f)-1));
        pow = default_pow;
        transform.position = pos;
        difficulty = diff;
        inGame = ingame;
    }

    protected void LifeCheck()
    {
        if (hp <= 0)
        {
            _enemyControl.DeadEnemy(this);
            gameObject.SetActive(false);
        }
    }

    public virtual void ChangeDifficulty(int diff)
    {
        difficulty = diff;
    }
}
