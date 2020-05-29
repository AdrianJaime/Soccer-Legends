using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using Firebase.Auth;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SignInScript : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        // Initialize Play Games Configuration and Activate it.
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().RequestServerAuthCode(false /*forceRefresh*/).Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        //FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://soccer-legends-db.firebaseio.com/");

        //DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }


    public void SignInWithPlayGames()
    {

        //txt.text = "Pressed";
        // Initialize Firebase Auth
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        //txt.text = "He pasado";
        // Sign In and Get a server auth code.
        UnityEngine.Social.localUser.Authenticate((bool success) => {
            if (!success)
            {
                //txt.text = "SignInOnClick: Failed to Sign into Play Games Services.";
                Debug.LogError("SignInOnClick: Failed to Sign into Play Games Services.");
                return;
            }

            string authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
            if (string.IsNullOrEmpty(authCode))
            {
                // txt.text = "SignInOnClick: Signed into Play Games Services but failed to get the server auth code.";
                Debug.LogError("SignInOnClick: Signed into Play Games Services but failed to get the server auth code.");
                return;
            }
            Debug.LogFormat("SignInOnClick: Auth code is: {0}", authCode);

            // Use Server Auth Code to make a credential
            Firebase.Auth.Credential credential = Firebase.Auth.PlayGamesAuthProvider.GetCredential(authCode);

            // Sign In to Firebase with the credential
            auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    //   txt.text = "SignInOnClick was canceled.";
                    Debug.LogError("SignInOnClick was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    //txt.text = "SignInOnClick encountered an error: " + task.Exception;
                    Debug.LogError("SignInOnClick encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("SignInOnClick: User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
                PlayerPrefs.SetString("username", newUser.DisplayName);
                SceneManager.LoadScene("MainMenu_scene");
            });
        });
    }

    //private void SearchUser(string UID)
    //{
    //    FirebaseDatabase.DefaultInstance.GetReference("player").GetValueAsync().ContinueWith(task =>
    //    {
    //        if (task.IsFaulted) Debug.Log("F in the chat");
    //        else if (task.IsCompleted)
    //        {
    //            DataSnapshot snapshot = task.Result;
    //            for(int i = 0; i < snapshot.ChildrenCount; i++)
    //            {
    //                snapshot.
    //            }
    //        }
    //    });

    //}
}