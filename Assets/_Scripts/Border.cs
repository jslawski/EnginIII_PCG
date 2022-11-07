using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    [SerializeField]
    private GameObject wallObject;
    [SerializeField]
    private GameObject doorwayObject;

    public void EnableWall()
    {
        this.wallObject.SetActive(true);
        this.doorwayObject.SetActive(false);
    }

    public void EnableDoorway()
    {
        this.doorwayObject.SetActive(true);
        this.wallObject.SetActive(false);
    }
}
