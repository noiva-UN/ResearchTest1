using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    List<Bullet> _bullets = new List<Bullet>();

    [SerializeField] private int initialInstance = 5;
    
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < initialInstance; i++)
        {
            CreateNew();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Bullet SearchPool()
    {
        for (int i = 0; i < _bullets.Count; i++)
        {
            if (_bullets[i].gameObject.active != true)
            {
                _bullets[i].gameObject.SetActive(true);
                return _bullets[i];
            }
        }

        return null;
    }

    private Bullet CreateNew()
    {
        var bul = Instantiate(bullet).GetComponent<Bullet>();
        bul.transform.SetParent(transform);
        bul.SetUp();
        bul.gameObject.SetActive(false); 
        
        _bullets.Add(bul);
        return bul;
    }
    
    public void Shot(Vector3 homePos, float speed)
    {
        Bullet bul = SearchPool();
        if (bul == null)
        {
            bul = CreateNew();
            bul.gameObject.SetActive(true);
        }

        bul.Initialize(homePos,speed);
        
    }
}
