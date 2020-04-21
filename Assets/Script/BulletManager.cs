using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : Controls
{
    [SerializeField] private GameObject bullet;
    List<Bullet> _bullets = new List<Bullet>();

    [SerializeField] private GameObject bulletParent;
    
    [SerializeField] private int initialInstance = 5;
    
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void SetControl(ControlMeta meta)
    {
        base.SetControl(meta);
    }
    
    public override void Initialized(int diff)
    {
        base.Initialized(diff);
        _bullets.Clear();
        for (int i = 0; i < initialInstance; i++)
        {
            CreateNew();
        }
        
    }
    
    private Bullet SearchPool()
    {
        foreach (var t in _bullets)
        {
            if (t.gameObject.active != true)
            {
                t.gameObject.SetActive(true);
                return t;
            }
        }

        return null;
    }

    private Bullet CreateNew()
    {
        var bul = Instantiate(bullet).GetComponent<Bullet>();
        bul.transform.SetParent(transform);
        bul.transform.position=Vector3.zero;
        bul.SetUp(this);
        bul.gameObject.SetActive(false);
        bul.transform.SetParent(bulletParent.transform);
        
        _bullets.Add(bul);
        return bul;
    }
    
    public void Shot(Vector3 homePos, float speed, string tag)
    {
        Bullet bul = SearchPool();
        if (bul == null)
        {
            bul = CreateNew();
            bul.gameObject.SetActive(true);
        }

        bul.Initialize(homePos, speed, tag);

    }
    
    public void Shot(Vector3 homePos, float speed, Vector3 vec, string tag)
    {
   
        Bullet bul = SearchPool();
        if (bul == null)
        {
            bul = CreateNew();
            bul.gameObject.SetActive(true);
        }

        bul.Initialize(homePos, speed, vec, tag);

    }

    public void Delete(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }
}
