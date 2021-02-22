using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Parallax : MonoBehaviour
{
    private float length, startPos;
    [SerializeField] private GameObject cam;
    [SerializeField] private float parralxEffect;

    private void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void FixedUpdate()
    {
        float temp = (Camera.main.transform.position.x * (1-parralxEffect));
        float dist = (Camera.main.transform.position.x * parralxEffect);

        transform.position = new Vector3(startPos + dist, transform.position.y);

        if (temp > startPos + length) startPos += length;
        else if (temp < startPos - length) startPos -= length;

    }
}
