using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using Firebase;
using Firebase.Storage;
using System.Threading.Tasks;
using UnityEngine.Networking;


public static class DBManager 
{
    public Text text;
    static Uri url = null;
    public Image image;


    // Update is called once per frame
    void Update()
    {
        if (url != null)
        {
            Debug.Log("YYYYY");
            StartCoroutine(GetImage());
        }
    }

    public void GetDB()
    {
        RestClient.Get<User>("https://soccer-legends-db.firebaseio.com/-LvaWZe8QhmQSlQJ8Njp.json").Then(response =>
        {
            text.text = response.userName;
        });
    }
    public void PostDB()
    {
        Test test=new Test("Test", 999);
        User user = new User("Player1", 10, new Status(11,12,13), test);
        RestClient.Put("https://soccer-legends-db.firebaseio.com/"+user.userName+".json", user);
        
    }
    public void DownloadDB()
    {
        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        StorageReference gs_reference = storage.GetReferenceFromUrl("gs://soccer-legends-db.appspot.com/Image.jpg");
        gs_reference.GetDownloadUrlAsync().ContinueWith((Task<Uri> task) =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
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
            Texture2D myTexture = new Texture2D(8,8);
            if (!File.Exists(Application.persistentDataPath + "/Image.JPG"))
            {
                Debug.Log("Don't Have it");
             myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
             byte[] bytes = myTexture.EncodeToJPG();
              File.WriteAllBytes(Application.persistentDataPath + "/Image.JPG", bytes);
            }
           else {
                Debug.Log(Application.persistentDataPath);
                byte[] byteArr = File.ReadAllBytes(Application.persistentDataPath + "/Image.JPG");
                myTexture.LoadImage(byteArr);
            }
            Sprite spr = Sprite.Create(myTexture, new Rect(0, 0, myTexture.width, myTexture.height), Vector2.zero, 1);
            image.sprite = spr;
        }
    }    
}
