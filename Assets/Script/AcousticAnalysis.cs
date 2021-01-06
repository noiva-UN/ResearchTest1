using System;
using MathNet.Numerics.IntegralTransforms;
using UnityEngine;
using NAudio.Wave;
using NAudio.Dsp;
using MathNet.Numerics.Statistics;
using System.Collections;
using System.IO;

public class AcousticAnalysis : MonoBehaviour
{
    private readonly int SampleNum = (2 << 9); // サンプリング数は2のN乗(N=5-12)

    AudioSource m_source;
    private float[] currentValues;
    
    [SerializeField, Range(0f, 1000f)] float gain = 200f;
    [SerializeField] private LineRenderer mLineRenderer;
    private UnityEngine.Vector3 m_sttPos;
    private UnityEngine.Vector3 m_endPos;

    private UnityEngine.Vector3[] points;
 

    private float fundamentalFrequency = 0f, AveragePower = 0f;

    //この値のベクトル方向にこの値よりも大きな差があった場合加算する　逆逆方向に差がある場合減算
    [SerializeField] private float addDiffFrequency = 0.1f, addDiffPower = 0.1f;
    [SerializeField] private int divisions = 10;

    private int wavSampleRate;

    private void Awake()
    {
        //m_lineRenderer = gameObject.GetComponent<LineRenderer>();
        m_sttPos = mLineRenderer.GetPosition(0);
        m_endPos = mLineRenderer.GetPosition(mLineRenderer.positionCount - 1);
        currentValues = new float[SampleNum];
    }

    public IEnumerator CheckAdjustment(string filepath, int numOfDivixions, int sampleRate, string path, Action<int> callback)
    {
        divisions = numOfDivixions;
        var frequency = 0f;
        var power = 0f;

        wavSampleRate = sampleRate;

        yield return StartCoroutine(Analysis(filepath, divisions, (r, s) => (frequency, power) = (r, s)));

        frequency = Mathf.Log(frequency);

        Debug.Log("基本周波数:" + frequency + ", 短時間平均パワー:" + power);

        if (fundamentalFrequency == 0f && AveragePower == 0f)
        {
            fundamentalFrequency = frequency;
            AveragePower = power;
            wavSampleRate = sampleRate;

            StreamWriter s = new StreamWriter(path + "/LogData.txt", true);
            s.WriteLine("各基準値 基本周波数の標準偏差:" + frequency + ", 短時間平均パワーの最大値:" + power);
            s.Close();
            yield break;
        }

        var adjustmentDiff = 0;     //難易度調整の値(-2～2))
        var frequencyDiff = frequency - fundamentalFrequency;
        var powerDiff = power - AveragePower;

        //基本周波数の標準偏差の変動による調整
        if (addDiffFrequency > 0)   //調整の基準値のベクトルがプラス
        {
            if (frequencyDiff > addDiffFrequency)   //かつプラス方向に基準値より変化が大きい
            {
                adjustmentDiff++;
            }
            else if (-addDiffFrequency > frequencyDiff)　//かつマイナス方向に基準値より変化が大きい
            {
                adjustmentDiff--;
            }

        }
        else// 調整の基準値のベクトルがマイナス
        {
            if (addDiffFrequency > frequencyDiff)   //かつマイナス方向に基準値より変化が大きい
            {
                adjustmentDiff++;
            }
            else if (frequencyDiff > -addDiffFrequency)   //かつプラス方向に基準値より変化が大きい
            {
                adjustmentDiff--;
            }

        }

        //短時間平均パワーの録音時間最大値による調整
        if (addDiffPower > 0)
        {
            if (powerDiff > addDiffPower)
            {
                adjustmentDiff++;
            }
            else if (-addDiffPower > addDiffPower)
            {
                adjustmentDiff--;
            }

        }
        else
        {
            if (addDiffPower > powerDiff)
            {
                adjustmentDiff++;
            }
            else if (powerDiff > -addDiffPower)
            {
                adjustmentDiff--;
            }
        }

        StreamWriter sw = new StreamWriter(path + "/LogData.txt", true);
        sw.WriteLine("基本周波数の標準偏差:" + frequency + ", 短時間平均パワーの最大値:" + power + ", 難易度変化:" + adjustmentDiff);
        sw.Close();

        callback(adjustmentDiff);
    }

    /// <summary>
    /// 分析するヤツ
    /// </summary>
    /// <param name="filepath">分析する音声ファイルのパス(.wave)</param>
    /// <param name="numOfDivisions">分析の際の分割数</param>
    /// <returns></returns>
    private IEnumerator Analysis(string filepath, int numOfDivisions, Action<float, float> callback)
    {

        WaveFileReader reader = new WaveFileReader(filepath);
        reader.Position = 0;

        float[] funFrequency = new float[divisions];
        float[] power = new float[divisions];

        var exc = (reader.Length / reader.BlockAlign) % divisions;
        var lenght = ((reader.Length / reader.BlockAlign) - exc) / divisions;

        System.Numerics.Complex[] complex = new System.Numerics.Complex[lenght];
        float[] samples = new float[lenght];

        for (int i = 0; i < divisions; i++)
        {
            for (int j = 0; j < lenght; j++)
            {
                samples[j] = reader.ReadNextSampleFrame()[0];
                complex[j] = new System.Numerics.Complex(Mathf.Abs(samples[j]), 0.0f);
            }
            funFrequency[i] = MathFundamentalFrequency(complex);
            power[i] = MathAveragePower(samples);
            yield return null;
            //計算が大量にあるため分割
        }

        var fundiv = MathStandardDeviation(funFrequency);
        var powMax = 0f;
        for (int i = 0; i < numOfDivisions; i++)
        {
            if (powMax < power[i])
            {
                powMax = power[i];
            }
        }

        powMax *= (float)wavSampleRate;

        callback(fundiv, powMax);

       yield return null;
    }


    public float MathFundamentalFrequency(System.Numerics.Complex[] complices)
    {
        Fourier.Forward(complices, FourierOptions.Matlab);

        string debug = "";


        float result = 0;
        var x = complices[0].Magnitude;
        var rising = false;

        for (int i = 1; i < complices.Length; i++)
        {
            var m = complices[i].Magnitude;

            if (rising)
            {
                if (m < x)
                {
                    result = i;
                    break;
                }
            }
            else
            {
                if (x < m)
                {
                    rising = true;
                    x = m;
                }
                else
                {
                    x = m;
                }
            }
        }
        return result;
    }

    public float MathAveragePower(float[] samples)
    {
        var ave = 0f;
        float length = samples.Length;

        for(int i = 0; i < length; i++)
        {
            ave += samples[i];
        }
        ave = ave / length;

        return ave;
    }

    public float MathStandardDeviation(float[] data)
    {
        var ave = 0f;
        float length = data.Length;
        for(int i = 0; i <length; i++)
        {
            ave += data[i];
        }
        ave = ave / length;

        var dev = 0f;

        for(int i = 0; i < length; i++)
        {
            dev += Mathf.Pow(data[i] - ave,2);
        }
        dev = Mathf.Sqrt(dev / length);

        return dev;
    }


    public void MelAnalysis(float[] values,float frequency)
    {
        currentValues = values;

        //メル変換 (https://ja.wikipedia.org/wiki/%E3%83%A1%E3%83%AB%E5%B0%BA%E5%BA%A6)
        //周波数パラメータが何のことかわからんからとりあえずサンプリング周波数を入れる

        //frequency = 700;
        /* for (int i = 0; i < currentValues.Length; i++)
         {
             currentValues[i] = (1000 / Mathf.Log(1000 / frequency + 1)) * Mathf.Log(currentValues[i] / frequency + 1);
         }*/


        System.Numerics.Complex[] complex = new System.Numerics.Complex[currentValues.Length];
        for (int i = 0; i < complex.Length; i++)
        {
            complex[i] = new System.Numerics.Complex(Mathf.Abs(currentValues[i]), 0.0);
        }
        Fourier.Inverse(complex);    //iFFT
        //Fourier.Forward(complex);
        
       // int size = (int)(Mathf.Log10(frequency) / Mathf.Log10(2));
    }

   
}
