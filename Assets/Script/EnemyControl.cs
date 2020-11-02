using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : Controls
{
    private List<Enemy> enemys = new List<Enemy>();
    
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform enemyParent;
    [SerializeField] private BulletManager _bulletManager;
    
    [SerializeField] private int popLimit = 10, coolDown = 2;
    private int pops = 0;
    private float mathTime = 0;

    public bool countDown = false;

    [SerializeField] private int sideLimit;

    public override void CountDown()
    {
        base.CountDown();
        if (pops < popLimit / 50 && (coolDown - 1) * (101f - (float) difficulty) / 50f <= mathTime)
        {
            PopEnemy();
            mathTime = 0;
        }
        else
        {
            mathTime += Time.deltaTime;
        }
    }

    // Update is called once per frame
    public override void MyUpdate()
    {
        if (countDown) countDown = false;
        if (pops < popLimit*difficulty/50 && coolDown * (101f - (float)difficulty)/50f <= mathTime)
        {
            PopEnemy();
            mathTime = 0;
        }
        else
        {
            mathTime += Time.deltaTime;    
        }
    }

    protected override void SetControl(ControlMeta meta)
    {
        base.SetControl(meta);
        
    }


    public override void Initialized(int diff)
    {
        base.Initialized(diff);
        
        enemys.Clear();

        for (int i = 0; i < Mathf.Floor(diff/5); i++)
        {
            SetNewEnemy();
        }

        countDown = true;
        GetReady();
    }

    public override void ControlDifficulty(int diff)
    {
        base.ControlDifficulty(diff);
        foreach (var e in enemys)
        {
            e.ChangeDifficulty(diff);
        }
    }

    private void PopEnemy()
    {
        for (int i = 0; i < enemys.Count; i++)
        {
            if (!enemys[i].gameObject.active)
            {
                enemys[i].Initialized(difficulty, new Vector3(Random.Range(-sideLimit, sideLimit), 25, 0),GetCommanderInGame());
                return;
            }
            
        }
        SetNewEnemy().Initialized(difficulty, new Vector3(Random.Range(-sideLimit, sideLimit), 25, 0),GetCommanderInGame());
    }
    
    private Enemy SetNewEnemy()
    {
        var enemy = Instantiate(enemyPrefab,enemyParent).GetComponent<Enemy>();
        enemys.Add(enemy);
        enemy.SetUp(this, _player,_bulletManager);
        enemy.gameObject.SetActive(false);
        return enemy;
    }

    public void DeadEnemy(Enemy enemy)
    {
       
    }

    public bool GetCommanderInGame()
    {
        return commander.InGame;
    }
}
