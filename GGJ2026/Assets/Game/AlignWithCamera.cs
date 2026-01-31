using UnityEngine;

public class AlignWithCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
