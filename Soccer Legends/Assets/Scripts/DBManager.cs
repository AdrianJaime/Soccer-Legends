using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using Firebase;
using Firebase.Storage;
using System.Threading.Tasks;
using UnityEngine.Networking;


public class DBManager : MonoBehaviour
{
    public Text text;
    Uri url = null;
    public Image image;

    // Update is called once per frame
    void Update()
    {
        if(url != null) StartCoroutine(GetImage());
    }

    public void GetDB()
    {
        RestClient.Get<User>("https://soccer-legends-d86c7.firebaseio.com/-LvaWZe8QhmQSlQJ8Njp.json").Then(response =>
        {
            text.text = response.userName;
        });
    }
    public void PostDB()
    {
        Test test=new Test("Test", 999);
        User user = new User("Player1", 10, new Status(11,12,13), test);
        RestClient.Put("https://soccer-legends-d86c7.firebaseio.com/"+user.userName+".json", user);
        
    }
    public void DownloadDB()
    {

        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference gs_reference = storage.GetReferenceFromUrl("gs://soccer-legends-d86c7.appspot.com/Captura.PNG");
        gs_reference.GetDownloadUrlAsync().ContinueWith((Task<Uri> task) =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Download URL: " + task.Result);
                url = task.Result;
            }
        });
    }

    IEnumerator GetImage()
    {
        Debug.Log("Hi");
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        url = null;
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite spr = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero, 1);
            image.sprite = spr;

        }
    }

    
}
