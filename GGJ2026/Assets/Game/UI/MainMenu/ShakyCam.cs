using System.Collections.Generic;
using UnityEngine;

public class ShakyCam : MonoBehaviour
{
    private Vector3 mainPosition;

    [SerializeField]
    private List<Vector2> speeds;

    [SerializeField] private float offsetStrength;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 finalOffset = Vector2.zero;
        foreach (Vector2 speed in speeds)
        {
            finalOffset.x += Mathf.Sin(Time.time * speed.x);
            finalOffset.y += Mathf.Sin(Time.time * speed.y);
        }

        finalOffset /= speeds.Count;
        transform.localPosition = mainPosition + new Vector3(finalOffset.x, finalOffset.y, 0) * offsetStrength;
    }
}
