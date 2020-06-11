using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using LitJson;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class JsonTest : MonoBehaviour
{

    public string url = "https://unity-api-falcon.herokuapp.com/api/users";
    public string apiKey = "YvhIy2Chob7kDWujOQKot6_0JYX9iQWxZEtrgRyQBOg";

    public string audioPath;

    public AudioClip audio;
    
    private List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

    private byte[] sample;
    
    // Use this for initialization

    void Start()
    {
        //StartCoroutine(API());
    }
    
    public IEnumerator API()
    {
        formData.Add(new MultipartFormDataSection("apikey", apiKey));
        //formData.Add(new MultipartFormDataSection("wav", audioPath));
        formData.Add( new MultipartFormDataSection("wav", File.ReadAllBytes(audioPath)));
        //formData.Add(new MultipartFormFileSection("wav",); 

        var www = UnityWebRequest.Post(url, formData);
        //www.SetRequestHeader("Content-type", "multipart/form-data");

        yield return www.SendWebRequest();

        print(www.downloadHandler.text);
    }
}