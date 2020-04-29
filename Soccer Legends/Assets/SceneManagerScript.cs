using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    public void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void LoadMatch()
    {
        if(GetComponent<FSOnlineManager>() == null) SceneManager.LoadScene("Match-PVE_test");
        else SceneManager.LoadScene("Match");
    }
}
