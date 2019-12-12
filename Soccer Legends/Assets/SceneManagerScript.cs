using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{

    public void ChangeScene(int _sceneBuildID)
    {
        if( SceneManager.sceneCountInBuildSettings >= _sceneBuildID)
            SceneManager.LoadScene(_sceneBuildID);
    }
}
