using System.Collections;
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

    private VoiceTest _voiceTest;
    
    [SerializeField] private Text diffUI;
    
    private bool setup = false;

    private bool change = false;
    
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
            diffUI.text = difficulty.ToString();
            change = true;
        }else if (Input.GetKeyDown(KeyCode.X))
        {
            difficulty--;
            if (difficulty < 0) difficulty = 0;
            diffUI.text = difficulty.ToString();
            change = true;
        }else if (Input.GetKeyDown(KeyCode.A))
        {
            difficulty = 0;
            diffUI.text = difficulty.ToString();
            change = true;
        }else if (Input.GetKeyDown(KeyCode.S))
        {
            difficulty = 100;
            diffUI.text = difficulty.ToString();
            change = true;
        }else if (Input.GetKeyDown(KeyCode.S))
        {
            difficulty = 50;
            diffUI.text = difficulty.ToString();
            change = true;
        }

        if (!change) return;

        if (mathTime == 0)
        {
            foreach (var t in controls)
            {
                t.ControlDifficulty(difficulty);
            }
        }

        if (mathTime >= coolDown)
        {
            change = false;
            mathTime = 0;
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

    public void UpDifficulty(int arg)
    {
        difficulty += arg;
        change = true;
    }
    
    public void DownDifficulty(int arg)
    {
        difficulty -= arg;
        change = true;
    }

    public void SetVoiceTest(VoiceTest voiceTest)
    {
        _voiceTest = voiceTest;
    }
}
