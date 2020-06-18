using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class JsonTest : MonoBehaviour
{

    public string url = "https://unity-api-falcon.herokuapp.com/api/users";
    public string apiKey = "YvhIy2Chob7kDWujOQKot6_0JYX9iQWxZEtrgRyQBOg";

    private List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

    public  IEnumerator API(string wavPath,Action<string> callback)
    {
        Debug.Log(wavPath);
        formData.Clear();
        formData.Add(new MultipartFormDataSection("apikey", apiKey));
        formData.Add( new MultipartFormDataSection("wav", File.ReadAllBytes(wavPath)));

        var www = UnityWebRequest.Post(url, formData);
        //www.SetRequestHeader("Content-type", "multipart/form-data");

        yield return www.SendWebRequest();

        //print(www.downloadHandler.text);

        callback(www.downloadHandler.text);
    }
}