using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TapUImanager : MonoBehaviour
{
    public static TapUImanager mg;
    [SerializeField]
    GameObject circleTapPrefab;
    bool enabled = true;

    private void Awake()
    {
        if (mg != null) Destroy(gameObject);
        else mg = this;

        DontDestroyOnLoad(this);
    }

    void Update()
    {
        if (!enabled) return;
            if(Input.GetMouseButtonDown(0))
            {
                Vector2 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
                Instantiate(mg.circleTapPrefab, new Vector3(worldPos.x, worldPos.y, Camera.main.transform.position.z+1), mg.circleTapPrefab.transform.rotation, null);
            }
    }

    private void OnLevelWasLoaded(int level)
    {
        enabled = !(SceneManager.GetSceneByBuildIndex(level).name == "Match" ||
            SceneManager.GetSceneByBuildIndex(level).name == "Match-PVE_test");
    }
}
