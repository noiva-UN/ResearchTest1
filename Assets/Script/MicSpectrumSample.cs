using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

//https://qiita.com/ELIXIR/items/595579a9372ef181e0bc

public class MicSpectrumSample : Controls
{
    private readonly int SampleNum = (2 << 9); // サンプリング数は2のN乗(N=5-12)
    [SerializeField, Range(0f, 1000f)] float m_gain = 200f; // 倍率
    AudioSource m_source;
    LineRenderer m_lineRenderer;
    Vector3 m_sttPos;
    Vector3 m_endPos;
    float[] currentValues;

    [SerializeField] private int samplingFrequency = 11025;

    [SerializeField, Range(1, 4)] private int recordingSec = 1;

    [SerializeField] private float VolumeDeadLine, followingDownline, aboveUpLine;

    private int count = 0;

    private float volume_Max = 0, mathTime = 0,math=0;

    private JsonTest API;
    
    
    [SerializeField] private bool checkAPI = false;
    
    private string result;
    private string[] dest;

    public int[] feelings = new int[5];

    [SerializeField] private string path;

    [SerializeField] private Text setText, resultText;
    [SerializeField] private InputField setField;

    protected override void SetControl(ControlMeta meta)
    {
        base.SetControl(meta);
    }

    public override void Initialized(int diff)
    {
        base.Initialized(diff);
        setField.text = "";

        path = SavWav.Initialized(path);
        
        m_source = GetComponent<AudioSource>();
        m_lineRenderer = GetComponent<LineRenderer>();
        m_sttPos = m_lineRenderer.GetPosition(0);
        m_endPos = m_lineRenderer.GetPosition(m_lineRenderer.positionCount - 1);
        currentValues = new float[SampleNum];


        
        StartCoroutine(SetUp());
    }

    private IEnumerator SetUp()
    {
        
        var length = Microphone.devices.Length;
        if (!(m_source != null && length > 0)) // オーディオソースとマイクがある
        {
            Debug.LogError("AudioSourceかマイクが見つからない");
            yield break;
        }

        var text = "Microphone List";
        for (int i = 0; i < length; i++)
        {
            text += "\n" + (i + 1).ToString() + "." + Microphone.devices[i];
        }

        setText.text = text;

        var devName = "";
        var num = -1;

        setField.ActivateInputField();

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (int.TryParse(setField.text, out num))
                {
                    if (0 <= num && num <= length)
                    {
                        devName = Microphone.devices[num - 1];
                        break;
                    }
                    else
                    {
                        Debug.LogError("リストの範囲外の数字が入力されている");
                    }
                }
                else
                {
                    Debug.LogError("整数以外が入力されている");
                }
            }

            yield return null;
        }

        m_source.loop = true; // ループにする
        m_source.clip = Microphone.Start(devName, true, recordingSec, samplingFrequency); // clipをマイクに設定
        while (!(Microphone.GetPosition(devName) > 0))
        {
        } // きちんと値をとるために待つ

        Microphone.GetPosition(null);
        m_source.Play();

        API = GetComponent<JsonTest>();
        
        setText.gameObject.SetActive(false);
        setField.gameObject.SetActive(false);
        GetReady();
    }

    public override void MyUpdate()
    {
        volume_Max = 0;
        m_source.GetSpectrumData(currentValues, 0, FFTWindow.Hamming);
        int levelCount = currentValues.Length / 8; // 高周波数帯は取らない
        Vector3[] positions = new Vector3[levelCount];
        for (int i = 0; i < levelCount; i++)
        {

            positions[i] = m_sttPos + (m_endPos - m_sttPos) * (float) i / (float) (levelCount - 1);
            positions[i].y += currentValues[i] * m_gain;
            if (volume_Max <= positions[i].y)
            {
                volume_Max = positions[i].y;
            }
        }

        m_lineRenderer.positionCount = levelCount;
        m_lineRenderer.SetPositions(positions);

        if(!checkAPI) return;
        
        if (mathTime >= recordingSec)
        {
            if (math >= 1)
            {
                
                StartCoroutine(Empath());

            }
            else
            {
                resultText.text = "十分な音量の音が \n 検知されませんでした";
                
                StreamWriter sw = new StreamWriter("Assets/LogData.txt", true);
                sw.WriteLine("音量不足");
                sw.Close();
            }
        }
        else
        {
            mathTime += Time.deltaTime;
        }

        if (volume_Max <= VolumeDeadLine) return;

        math++;
/*
        if (commander.change) return;

        if (volume_Max >= aboveUpLine)
        {
            count++;
        }
        else if (volume_Max <= followingDownline)
        {
            count--;
        }

        //Debug.Log(count);

        if (Mathf.Abs(count) >= 5)
        {
            commander.adjustmentDifficulty(-count);
            count = 0;
        }*/
    
    }


    private IEnumerator Empath()
    {
        var name = "demo" + count + ".wav";
        SavWav.Save(name, m_source.clip);
        count++;
        mathTime = 0;
        math = 0;
        yield return StartCoroutine(API.API(path + "/" + name ,r => result = r));
        print(result);
        
        StreamWriter sw = new StreamWriter("Assets/LogData.txt", true);
        sw.WriteLine(result);
        sw.Close();
        
        result = result.Remove(result.Length - 2);
        dest = result.Split(',');
        resultText.text = "";

        for (int i = 1; i < dest.Length; i++)
        {
            resultText.text += dest[i] + "\n";
            feelings[i - 1] = int.Parse(dest[i].Split(':')[1]);
        }
        
        foreach (var text in dest)
        {
            resultText.text += text + "\n";
        }

        if (feelings[2] >= 25 || feelings[4] >= 25)
        {
            //喜びか元気度が25以上　適性値かな？ってことで変化なし
        }else if (feelings[0] >= 25)
        {
            //平常度が25以上だとEZってことで難易度上げる
            commander.adjustmentDifficulty(2);
        }else if (feelings[1] >= 25 || feelings[3] >= 25)
        {
            //怒りか悲しみが25以上だと難易度下げる
            commander.adjustmentDifficulty(-2);
        }
    }
}