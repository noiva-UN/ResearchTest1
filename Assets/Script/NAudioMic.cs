using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NAudio;
using NAudio.Wave;
using System;

public class NAudioMic :  Controls
{



    [SerializeField] private float VolumeDeadLine, followingDownline, aboveUpLine;

    private int count = 0;

    private float volume_Max = 0, mathTime = 0, math = 0;



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





        StartCoroutine(SetUp());
    }

    private IEnumerator SetUp()
    {

        var length = Microphone.devices.Length;
        if (!( length > 0)) // オーディオソースとマイクがある
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

        setText.gameObject.SetActive(false);
        setField.gameObject.SetActive(false);
        GetReady();
    }




    public override void MyUpdate()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartRec(path + "/demo" + count + ".wav");
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            StopRec(path + "/demo" + count + ".wav");
            count++;
        }
        
    }


    private void StartRec(string filepath)
    {
        inEvent = new WaveInEvent();
        inEvent.DeviceNumber = DevNum;
        inEvent.WaveFormat = new WaveFormat(44100, WaveInEvent.GetCapabilities(DevNum).Channels);

        fileWriter = new WaveFileWriter(filepath, inEvent.WaveFormat);

        inEvent.DataAvailable += (_, ee) =>
        {
            fileWriter.Write(ee.Buffer, 0, ee.BytesRecorded);
            fileWriter.Flush();
        };
        inEvent.RecordingStopped += (_, __) =>
        {
            fileWriter.Flush();
        };

        inEvent.StartRecording();

    }

    private void StopRec(string filepath)
    {
        inEvent?.StopRecording();
        inEvent?.Dispose();
        inEvent = null;

        fileWriter?.Close();
        fileWriter = null;

    }

}
