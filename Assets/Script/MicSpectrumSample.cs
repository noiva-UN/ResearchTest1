using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MicSpectrumSample : MonoBehaviour {
    private readonly int SampleNum = (2 << 9); // サンプリング数は2のN乗(N=5-12)
    [SerializeField, Range(0f, 1000f)] float m_gain = 200f; // 倍率
    AudioSource m_source;
    LineRenderer m_lineRenderer;
    Vector3 m_sttPos;
    Vector3 m_endPos;
    float[] currentValues;

    [SerializeField] private float VolumeDeadLine, followingDownline, aboveUpLine;

    private int count;
    
    [SerializeField] private ControlMeta _controlMeta;
    
    private float volume_Max=0;

    private string[] noteNames = {"ド", "ド♯", "レ", "レ♯", "ミ", "ファ", "ファ♯", "ソ", "ソ♯", "ラ", "ラ♯", "シ"};

    [SerializeField] private Text _text;
    
    // Use this for initialization
    void Start () {
        m_source = GetComponent<AudioSource>();
        m_lineRenderer = GetComponent<LineRenderer>();
        m_sttPos = m_lineRenderer.GetPosition(0);
        m_endPos = m_lineRenderer.GetPosition(m_lineRenderer.positionCount-1);
        currentValues = new float[SampleNum];
        if ((m_source != null) && (Microphone.devices.Length > 0)) // オーディオソースとマイクがある
        {
            if (m_source.clip == null) // クリップがなければマイクにする
            {
                string devName = Microphone.devices[0]; // 複数見つかってもとりあえず0番目のマイクを使用
                Debug.Log(devName);
                int minFreq, maxFreq;
                Microphone.GetDeviceCaps(devName, out minFreq, out maxFreq); // 最大最小サンプリング数を得る
                int ms = minFreq / SampleNum; // サンプリング時間を適切に取る
                m_source.loop = true; // ループにする
                m_source.clip = Microphone.Start(devName, true, ms, minFreq); // clipをマイクに設定
                while (!(Microphone.GetPosition(devName) > 0)) { } // きちんと値をとるために待つ
                Microphone.GetPosition(null);
                m_source.Play();
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        volume_Max = 0;
        m_source.GetSpectrumData(currentValues, 0, FFTWindow.Hamming);
        int levelCount = currentValues.Length/8; // 高周波数帯は取らない
        Vector3[] positions = new Vector3[levelCount];
        for (int i=0;i< levelCount; i++)
        {

            positions[i] = m_sttPos + (m_endPos - m_sttPos) * (float)i / (float)(levelCount - 1);
            positions[i].y += currentValues[i] * m_gain;
            if (volume_Max <= positions[i].y)
            {
                volume_Max = positions[i].y;
            }
        }
        
        //Debug.Log("MAX ="+volume_Max);
        
        m_lineRenderer.positionCount = levelCount;
        m_lineRenderer.SetPositions(positions);

        if(volume_Max <= VolumeDeadLine) return;
        
        if(_controlMeta.change) return;
        
        if (volume_Max >= aboveUpLine)
        {
            count++;
        }else if (volume_Max <= followingDownline)
        {
            count--;
        }
        Debug.Log(count);

        if (Mathf.Abs(count) >= 5)
        {
            _controlMeta.adjustmentDifficulty(-count);
            count = 0;
        }
        

        
        float[] spectrum = new float[256];
        AudioListener.GetSpectrumData(spectrum, 1, FFTWindow.Rectangular);
        
        var maxIndex = 0;
        var maxValue = 0.0f;
        for (int i = 0; i < spectrum.Length; i++)
        {
            var val = spectrum[i];
            if (val > maxValue)
            {
                maxValue = val;
                maxIndex = i;
            }
        }
        
        var freq = maxIndex * AudioSettings.outputSampleRate / 2 / spectrum.Length;

        _text.text = GetNoteName(freq);
    }
    
    public string GetNoteName(float freq)
    {
        // 周波数からMIDIノートナンバーを計算
        var noteNumber = calculateNoteNumberFromFrequency(freq);
        // 0:C - 11:B に収める
        var note = noteNumber % 12;
        // 0:C～11:Bに該当する音名を返す
        if (note < 0) return null;
        
        Debug.Log(note);
        return noteNames[note];
    }

    // See https://en.wikipedia.org/wiki/MIDI_tuning_standard
    private int calculateNoteNumberFromFrequency(float freq)
    {
        return Mathf.FloorToInt(69 + 12 * Mathf.Log(freq / 440, 2));
    }

}