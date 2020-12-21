using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NAudio;
using NAudio.Wave;
using System;

public class NAudioMic :  Controls
{
    [SerializeField] private float dataPauseSec = 0.1f, recSec = 1f;
    [SerializeField] private int sampleRate = 11025;

    AudioSource m_source;
    private float[] currentValues;

    [SerializeField] private float VolumeDeadLine, followingDownline, aboveUpLine;

    private int count = 0;

    private float mathTime = 0, math = 0;


    [SerializeField] private LineRenderer m_lineRenderer;
    Vector3 m_sttPos;
    Vector3 m_endPos;
    [SerializeField, Range(0f, 1000f)] float m_gain = 200f; // 倍率

    [SerializeField] private string path;

    [SerializeField] private Text setText, resultText;
    [SerializeField] private InputField setField;


    private WaveInEvent inEvent;
    private WaveFileWriter fileWriter;

    private string DevName;
    private int DevNum;


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
        currentValues = new float[2<<9];

        m_sttPos = m_lineRenderer.GetPosition(0);
        m_endPos = m_lineRenderer.GetPosition(m_lineRenderer.positionCount - 1);

        mathTime = -1f;
        math = 0f;

        StartCoroutine(SetUp());
    }

    private IEnumerator SetUp()
    {

        var length = Microphone.devices.Length;
        if (!(m_source != null && length > 0)) // オーディオソースとマイクがある
        {
            Debug.LogError("マイクが見つからない");
            yield break;
        }

        var text = "Microphone List";
        for (int i = 0; i < length; i++)
        {
            text += "\n" + (i + 1).ToString() + "." + Microphone.devices[i];
        }

        setText.text = text;


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
                        DevNum = num - 1;
                        DevName = Microphone.devices[DevNum];
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
        m_source.clip = Microphone.Start(DevName, true, 1, sampleRate); // clipをマイクに設定
        while (!(Microphone.GetPosition(DevName) > 0))
        {
        } // きちんと値をとるために待つ

        Microphone.GetPosition(null);
        m_source.Play();

        setField.gameObject.SetActive(false);

        mathTime = -1;
        setText.text = "1秒以上適当に発言してください";

        while (true)
        {
            if (SpeakCheck())
            {
                if (mathTime < 0)
                {
                    StartRec(this, path + "/demo" + count + ".wav");
                    mathTime = 0;
                }
                else
                {
                    mathTime += Time.deltaTime;
                }
            }
            else
            {
                if (0 < mathTime)
                {
                    StopRec(this, path + "/demo" + count + ".wav");
                    break;
                }
            }

            yield return null;
        }
        count++;
        mathTime = -1;
        setText.gameObject.SetActive(false);

        GetReady();
    }




    public override void MyUpdate()
    {
        if(recSec <= mathTime)
        {
            StopRec(this, path + "/demo" + count + ".wav");
            count++;
            mathTime = -1;
        }else if(mathTime < 0)
        {
            if (SpeakCheck())
            {
                StartRec(this, path + "/demo" + count + ".wav");
                mathTime = 0;
            }
        }
        else
        {
            mathTime += Time.deltaTime;
        }
    }

    private void StartRec(object sender, string filepath)
    {
        inEvent = new WaveInEvent();
        inEvent.DeviceNumber = DevNum;
        inEvent.WaveFormat = new WaveFormat(sampleRate, WaveInEvent.GetCapabilities(DevNum).Channels);

        fileWriter = new WaveFileWriter(filepath, inEvent.WaveFormat);

        inEvent.DataAvailable += (_, ee) =>
        {
            fileWriter.Write(ee.Buffer, 0, ee.BytesRecorded);
            fileWriter.Flush();
        };
        inEvent.RecordingStopped += (_, __) =>
        {
            fileWriter.Flush();

            inEvent.Dispose();
            inEvent = null;

            fileWriter.Close();
            fileWriter = null;
            Debug.Log("RecEnd : " + filepath);

            //GetWave(filepath);


        };
        inEvent.StartRecording();
        Debug.Log("RecStart : " + filepath);
    }

    private void StopRec(object sender, string filepath)
    {

        inEvent.StopRecording();

    }

    private bool SpeakCheck()
    {
        float volume_Max = 0f;
        m_source.GetSpectrumData(currentValues, 0, FFTWindow.Hamming);

        for (int i = 0; i < currentValues.Length; i++)
        {
            if (volume_Max <= currentValues[i])
            {
                volume_Max = currentValues[i];
            }
        }

        if (volume_Max*100f <= VolumeDeadLine)
        {
            return false;
        }
        else
        {
            Debug.Log(volume_Max*100f);
            return true;
        }
    }

    private void GetWave(string filepath)
    {
        WaveFileReader reader = new WaveFileReader(filepath);

        float[] samples = new float[reader.Length / reader.BlockAlign];


        var level = samples.Length;
        Vector3[] positions = new Vector3[level];

        for (int i = 0; i < level; i++)
        {
            float[] sample = reader.ReadNextSampleFrame();

            samples[i] = sample[0];

            positions[i] = m_sttPos + (m_endPos - m_sttPos) * (float)i / (float)(level - 1);
            positions[i].y += samples[i] * m_gain;
        }
        m_lineRenderer.positionCount = level;
        m_lineRenderer.SetPositions(positions);
        Debug.Log("ViewEnd : " + filepath);
    }
}