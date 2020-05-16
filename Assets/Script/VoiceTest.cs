using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class VoiceTest : MonoBehaviour
{

    public string[] UpKeyWords,DownKeyWords;

    private string[] _AllWords;

    private int diff;

    private bool setUp = false;
    
    public ControlMeta _controlMeta;
    
    private KeywordRecognizer mKeywordRecognizer;
    
    // Start is called before the first frame update
    void Start()
    {
        _controlMeta.SetVoiceTest(this);
        
        _AllWords = new string[UpKeyWords.Length + DownKeyWords.Length];
        for (int i = 0; i < _AllWords.Length; i++)
        {
            if (i < UpKeyWords.Length)
            {
                _AllWords[i] = UpKeyWords[i];
            }
            else
            {
                _AllWords[i] = DownKeyWords[i - UpKeyWords.Length];
            }
        }

        mKeywordRecognizer = new KeywordRecognizer(_AllWords);
        mKeywordRecognizer.OnPhraseRecognized += OnPhraseRecognized;
        mKeywordRecognizer.Start();
        setUp = true;
    }

    private void OnEnable()
    {
        if(setUp) mKeywordRecognizer.Start();
    }

    private void OnDisable()
    {
        mKeywordRecognizer.Stop();
        mKeywordRecognizer.Dispose();
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (FindInLibrary(args, UpKeyWords))
        {
            _controlMeta.UpDifficulty(1);
        }
        else if(FindInLibrary(args,DownKeyWords))
        {
            _controlMeta.DownDifficulty(1);
        }
    }

    private bool FindInLibrary(PhraseRecognizedEventArgs str,string[] lib)
    {
        foreach (var t in lib)
        {
            if (str.text.StartsWith(t, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
