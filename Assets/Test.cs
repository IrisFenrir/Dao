using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject go;

    private void Start()
    {
        go = GameObject.Find("Canvas/Puzzle1_Box");
        
    }
}
