using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;

public class NetworkManager : MonoBehaviour
{
    [SerializeField]
    private Text placeholderText;

    string baseURL = "https://test.iamdave.ai/conversation/exhibit_aldo/74710c52-42a5-3e65-b1f0-2dc39ebe42c2";

    private AudioSource audio;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    public void NetworkRequestWrapper(string cust_state_text)
    {
        StartCoroutine(Post(cust_state_text));
    }

    public IEnumerator Post(string cust_state)
    {
        var body = new BodyData();

        body.system_response = "sr_init";
        body.engagement_id = "NzQ3MTBjNTItNDJhNS0zZTY1LWIxZjAtMmRjMzllYmU0MmMyZXhoaWJpdF9hbGRv";
        body.customer_state = cust_state;

        string json = JsonUtility.ToJson(body);

        var req = new UnityWebRequest(baseURL, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("X-I2CE-ENTERPRISE-ID", "dave_expo");
        req.SetRequestHeader("X-I2CE-USER-ID", "74710c52-42a5-3e65-b1f0-2dc39ebe42c2");
        req.SetRequestHeader("X-I2CE-API-KEY", "NzQ3MTBjNTItNDJhNS0zZTY1LWIxZjAtMmRjMzllYmU0MmMyMTYwNzIyMDY2NiAzNw__");

        //Send the request then wait here until it returns
        yield return req.SendWebRequest();

        if (req.isNetworkError)
        {
            Debug.Log("Error While Sending: " + req.error);
        }
        else
        {
            JObject response = JObject.Parse(req.downloadHandler.text);
            placeholderText.text = "Placeholder text: \n" +response["placeholder"].ToString();
            StartCoroutine(PlaySoundFromURL(response["response_channels"]["voice"].ToString()));
        }
    }

    bool play = false;
    private IEnumerator PlaySoundFromURL(string url)
    {
        // url = System.Uri.EscapeUriString (url);
        Debug.Log(url);
        WWW www = new WWW(url);
        yield return www;
        audio.clip = (AudioClip)www.GetAudioClip();
        play = true;
        Debug.Log("Audio played");
    }

    void Update()
    {
        if (play)
        {
            if (!audio.isPlaying && audio.clip.isReadyToPlay)
            {
                audio.Play();
                play = false;
            }
        }
    }
}

public class BodyData
{
    public string system_response;
    public string engagement_id;
    public string customer_state;
}
