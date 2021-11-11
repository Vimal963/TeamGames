using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{

    /// <summary>
    /// this usedd to move cloud animation in game scene(stop clock)
    /// </summary>
    [SerializeField] private float speed;
    float min_x, max_x;

    // Start is called before the first frame update
    void Start()
    {
        min_x = -5;
        max_x = 5;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.left * Time.deltaTime * speed;
        if (transform.position.x <= min_x) transform.position = new Vector3(max_x, transform.position.y, transform.position.z);
    }
}
