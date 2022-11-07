using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool dungeonComplete = false;

    private void Start()
    {
        dungeonComplete = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyUp(KeyCode.Return) && dungeonComplete == true)
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
