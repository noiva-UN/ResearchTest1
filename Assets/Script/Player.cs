using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private int sideLimit, hightLimit;
    [SerializeField] private int hp, pow;
    [SerializeField] private float speed, coolDown;
    private float mathTime = 0;
    
    [SerializeField] private BulletManager _bulletManager;
    private Vector3 newPos = new Vector3();

    [SerializeField] private Text hpUI;

    private ControlMeta _controlMeta;
    
    // Start is called before the first frame update
    void Awake()
    {
        hpUI.text = hp.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(!_controlMeta.InGame) return;
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
            Shot();
        }


    }

    private void Shot()
    {
        if (mathTime >= coolDown)
        {
            mathTime = 0;
            _bulletManager.Shot(transform.position, speed, gameObject.tag);
        }
    }

    public void SetBulletManager(BulletManager bulletManager)
    {
        _bulletManager = bulletManager;
    }

    public void SetControlMeta(ControlMeta meta)
    {
        _controlMeta = meta;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(transform.tag))
        {
            other.gameObject.SetActive(false);
            hp -= 1;
            hpUI.text = hp.ToString();
        }
    }
}
