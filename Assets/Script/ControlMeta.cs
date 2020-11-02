using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ControlMeta : MonoBehaviour
{
    //難易度調整用　初期値50
    [SerializeField, Range(1,100)] private int difficulty = 50;

    [SerializeField] private float coolDown = 0.5f, timeLimit = 60f, CountDown = 3f;
    private float mathTime = 0, time = 0;
    private int math = 0;
    
    [SerializeField] private Player _player;
    
    [SerializeField] private List<Controls> controls = new List<Controls>();

    [SerializeField] private Text diffUI, GameOver, lastDiff, timeUI, countUI;
    [SerializeField] private Image setImage;

    private int adCount = 0, diCount = 0, unCount = 0;
    
    private bool setup = false;

    [HideInInspector] public bool change = false;

    private bool inGame = false;

    public bool InGame => inGame;

    // Start is called before the first frame update
    void Awake()
    {
        diffUI.text = difficulty.ToString();
        if (!setup)
        {
            SetUp();
            //setup = true;
        }
        Initialized();
    }

    // Update is called once per frame
    void Update()
    {
        if (!setup)
        {
            if (math == controls.Count)
            {
                countUI.gameObject.SetActive(true);
                countUI.text = CountDown.ToString();
                if (CountDown <= mathTime)
                {
                    setup= true;
                    inGame = true;
                    setImage.gameObject.SetActive(false);
                    countUI.gameObject.SetActive(false);
                }
                else
                {
                    foreach (var monos in controls)
                    {
                        monos.CountDown();
                    }

                    mathTime += Time.deltaTime;
                    countUI.text = (CountDown - (int) mathTime).ToString();
                }
            }

            return;
        }
        
        if (inGame)
        {
            foreach (var monos in controls)
            {
                monos.MyUpdate();
            }
            
            if (_player.Hp <= 0)
            {
                inGame = false;
                GameOver.gameObject.SetActive(true);
                GameOver.text = "GameOver";
                GameOver.color = Color.red;
                
                lastDiff.gameObject.SetActive(true);
                lastDiff.text = "難易度変更："+(adCount+diCount).ToString() + "  加算："+adCount+"回  減算："+diCount+"回";
                
                setImage.gameObject.SetActive(true);

                var str = "GameOver \n 変更回数:" + (adCount + diCount + unCount) + "  加算:" + adCount + "  減算:" + diCount+"\n 最終難易度" + difficulty;

                WriteLog(Application.dataPath + "/LogData.txt", str);
            }
            
            if (time >= timeLimit)
            {
                inGame = false;
                GameOver.gameObject.SetActive(true);
                GameOver.text = "Success";
                GameOver.color=Color.green;
            
                lastDiff.gameObject.SetActive(true);
                lastDiff.text = "難易度変更："+(adCount+diCount).ToString() + "  加算："+adCount+"回  減算："+diCount+"回";
            
                setImage.gameObject.SetActive(true);
            
                var str = "Success \n 変更回数:" + (adCount + diCount + unCount) + "  加算:" + adCount + "  減算:" + diCount;

                WriteLog(Application.dataPath+ "Assets/LogData.txt", str);
            }
            else
            {
                timeUI.text = "残り　" + (int) (timeLimit - time) + "秒";
                time += Time.deltaTime;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("Scenes/SampleScene");
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                UnityEngine.Application.Quit();   
            } 
        }
  /* テスト用      
        if (Input.GetKeyUp(KeyCode.Z))
        {
            difficulty++;
            if (difficulty > 100) difficulty = 100;
            change = true;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            difficulty--;
            if (difficulty < 0) difficulty = 0;

            change = true;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            difficulty = 0;

            change = true;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            difficulty = 100;

            change = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            difficulty = 50;

            change = true;
        }
*/
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
        math = 0;
        mathTime = 0;
        _player.SetControlMeta(this);
        
        controls.Clear();
        transform.SendMessage("SetControl", (this));
    }

    private void Initialized()
    {
        StreamWriter sw;
        if (!File.Exists(Application.dataPath+"/LogData.txt"))
        {
            sw = File.CreateText(Application.dataPath+"/LogData.txt");
        }
        else
        {
            sw = new StreamWriter(Application.dataPath+"/LogData.txt", true);
        }

        sw.WriteLine("開始");
        sw.Close();
        
        setImage.gameObject.SetActive(true);
        countUI.gameObject.SetActive(false);
        foreach (var monos in controls)
        {
            monos.Initialized(difficulty);
        }
        
        lastDiff.gameObject.SetActive(false);
        GameOver.gameObject.SetActive(false);
        time = 0;
        timeUI.text = "残り　" + (int)(timeLimit - time) + "秒";
        adCount = 0;
        diCount = 0;
        unCount = 0;
    }

    public void GetReady()
    {
        math++;
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

    public void UnChangedDiff()
    {
        unCount++;
    }
    
    public void UpDifficulty(int arg)
    {
        difficulty += arg;
        change = true;
        adCount++;
    }
    
    public void DownDifficulty(int arg)
    {
        difficulty -= arg;
        change = true;
        diCount++;
    }

    private void WriteLog(string path, string text)
    {
        StreamWriter sw = new StreamWriter(path,true);
        sw.WriteLine(text);     
        sw.Flush();
        sw.Close();
    }
}
