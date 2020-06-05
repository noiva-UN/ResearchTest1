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

public class JsonTest : MonoBehaviour
{

    public string url = "https://unity-api-falcon.herokuapp.com/api/users";
    public string apiKey = "YvhIy2Chob7kDWujOQKot6_0JYX9iQWxZEtrgRyQBOg";

    public string audioPath;

    // Use this for initialization
    IEnumerator Start()
    {
        WWWForm form = new WWWForm();
        form.AddField("apikey", apiKey);
        form.AddField("wav", audioPath);

        var www = UnityWebRequest.Post(url, form);
        www.SetRequestHeader("Content-type", "multipart/form-data");

        yield return www.SendWebRequest();

        print(www.downloadHandler.text);

        var xxx = new WWW(url, form);

        yield return xxx;

        print(xxx.text);


        Hashtable header = new Hashtable();
        header.Add("Content-Type", "multipart/form-data");
        
        byte[] postBytes = form.data;
        
        var yyy = new WWW(url, postBytes, header);

        yield return yyy;

        print(yyy.text);
        
    }
}