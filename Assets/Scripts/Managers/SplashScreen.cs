using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SplashScreen : MonoBehaviour {

    public float autoLoadNextLevelAfter;
    // Use this for initialization
    void Start()
    {

        Invoke("LoadNextLevel", autoLoadNextLevelAfter);

    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

}



