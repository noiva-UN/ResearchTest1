using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AcousticAnalysis : MonoBehaviour
{
    private readonly int SampleNum = (2 << 9); // サンプリング数は2のN乗(N=5-12)

    AudioSource m_source;
    private float[] currentValues;
    
    [SerializeField, Range(0f, 1000f)] float gain = 200f;
    [SerializeField] private LineRenderer mLineRenderer;
    private Vector3 m_sttPos;
    private Vector3 m_endPos;

    private Vector3[] points;
    
    private void Awake()
    {
        //m_lineRenderer = gameObject.GetComponent<LineRenderer>();
        m_sttPos = mLineRenderer.GetPosition(0);
        m_endPos = mLineRenderer.GetPosition(mLineRenderer.positionCount - 1);
        currentValues = new float[SampleNum];
    }

    public void Analysis(float[] values,float frequency)
    {
        currentValues = values;    
        
        //メル変換 (https://ja.wikipedia.org/wiki/%E3%83%A1%E3%83%AB%E5%B0%BA%E5%BA%A6)
        //周波数パラメータが何のことかわからんからとりあえずサンプリング周波数を入れる

        frequency = 700;


        
        for (int i = 0; i < currentValues.Length; i++)
        {
            currentValues[i] = (1000 / Mathf.Log(1000 / frequency + 1)) * Mathf.Log(Mathf.Abs(currentValues[i]) / frequency + 1);
        }

        
        int length = currentValues.Length / 8;
        //MicSpectrumSampleに合わせる
        
        points=new Vector3[length];
        for (int i = 0; i < length; i++)
        {
            points[i] = m_sttPos + (m_endPos - m_sttPos) * (float) i / (float) (length - 1);
            points[i].y += currentValues[i] * gain;

        }

        mLineRenderer.positionCount = length;
        mLineRenderer.SetPositions(points);

    }
}
