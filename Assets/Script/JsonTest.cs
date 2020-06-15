using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows;

public class JsonTest : MonoBehaviour
{

    public string url = "https://unity-api-falcon.herokuapp.com/api/users";
    public string apiKey = "YvhIy2Chob7kDWujOQKot6_0JYX9iQWxZEtrgRyQBOg";

    public string audioPath;

    private List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

    private byte[] sample;
    
    public  IEnumerator API(Action<string> callback)
    {
        formData.Add(new MultipartFormDataSection("apikey", apiKey));
        formData.Add( new MultipartFormDataSection("wav", File.ReadAllBytes(audioPath)));

        var www = UnityWebRequest.Post(url, formData);
        //www.SetRequestHeader("Content-type", "multipart/form-data");

        yield return www.SendWebRequest();

        //print(www.downloadHandler.text);

        callback(www.downloadHandler.text);
    }
}