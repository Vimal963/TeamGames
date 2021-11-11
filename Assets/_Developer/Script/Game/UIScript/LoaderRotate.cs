using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderRotate : MonoBehaviour
{
    /// <summary>
    /// it's just rotate loading screen loading image
    /// </summary>
    [SerializeField] private float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * Time.deltaTime * speed);
    }
}
