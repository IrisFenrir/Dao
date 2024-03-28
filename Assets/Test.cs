using Dao;
using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject go;

    private void Start()
    {
        go = FindUtility.Find("CameraSetting", FindUtility.Find("Room1").transform);
        

    }
}
