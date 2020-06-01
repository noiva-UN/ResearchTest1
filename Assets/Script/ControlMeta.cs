﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.UI;

public class ControlMeta : MonoBehaviour
{
    //難易度調整用　初期値50
    [SerializeField, Range(1,100)] private int difficulty = 50;

    [SerializeField] private float coolDown = 0.5f;
    private float mathTime = 0;
    
    [SerializeField] private Player _player;
    
    private List<Controls> controls = new List<Controls>();

    private VoiceTest1 _voiceTest1;
    
    [SerializeField] private Text diffUI;
    
    private bool setup = false;

    public bool change = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        diffUI.text = difficulty.ToString();
        if (!setup)
        {
            SetUp();
            setup = true;
        }
        Initialized();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            difficulty++;
            if (difficulty > 100) difficulty = 100;
            change = true;
        }else if (Input.GetKeyDown(KeyCode.X))
        {
            difficulty--;
            if (difficulty < 0) difficulty = 0;

            change = true;
        }else if (Input.GetKeyDown(KeyCode.A))
        {
            difficulty = 0;

            change = true;
        }else if (Input.GetKeyDown(KeyCode.S))
        {
            difficulty = 100;

            change = true;
        }else if (Input.GetKeyDown(KeyCode.S))
        {
            difficulty = 50;

            change = true;
        }

        if (!change) return;

        if (mathTime == 0)
        {
            foreach (var t in controls)
            {
                t.ControlDifficulty(difficulty);
            }
            diffUI.text = difficulty.ToString();
        }

        if (mathTime >= coolDown)
        {
            change = false;
            //_voiceTest1.gameObject.SetActive(true);
            mathTime = 0;
            return;
        }
        
        mathTime += Time.deltaTime;

        
    }

    private void SetUp()
    {
        controls.Clear();
        transform.SendMessage("SetControl", (this));
    }

    private void Initialized()
    {
        foreach (var monos in controls)
        {
            monos.Initialized(difficulty);
        }
    }

    public void AddControls(Controls mono)
    {
        controls.Add(mono);
    }

    public Controls SearchControl(string com)
    {
        foreach (var t in controls)
        {
            Debug.Log(t.name);
            if (t.name == com)
            {
               
                return t;
            }
        }

        return null;
    }

    public Player GetPlayer()
    {
        return _player;
    }

    public void adjustmentDifficulty(int arg)
    {
        if (arg > 0)
        {
            UpDifficulty(arg);
        }
        else
        {
            DownDifficulty(-arg);
        }
    }
    
    public void UpDifficulty(int arg)
    {
        difficulty += arg;
        change = true;
        Debug.Log("Difficulty UP");
        //_voiceTest1.gameObject.SetActive(false);
    }
    
    public void DownDifficulty(int arg)
    {
        difficulty -= arg;
        change = true;
        Debug.Log("Difficulty Down");
        //_voiceTest1.gameObject.SetActive(false);
    }

    public void SetVoiceTest(VoiceTest1 voiceTest1)
    {
        _voiceTest1 = voiceTest1;
    }
}
