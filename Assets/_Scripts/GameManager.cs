using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static bool dungeonComplete = false;

    [SerializeField]
    private DungeonGenerator generator;

    public static int collectedKeys = 0;
    public static int totalKeys = 0;

    private void Start()
    {
        dungeonComplete = false;

        this.generator.GenerateDungeon();
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

        if (Input.GetKeyUp(KeyCode.T))
        {
            SceneManager.LoadScene(1);
        }
    }
}
