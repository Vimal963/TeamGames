using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentScaleUpDown : MonoBehaviour
{
    float x, poDiff, maxScaleDiff, maxPoDiff;

    // Start is called before the first frame update
    void Start()
    {
        maxScaleDiff = 0.3f;
        maxPoDiff = 4.2f;
    }


    // Update is called once per frame
    void Update()
    {
        ScaleUpDown();
    }

    private void ScaleUpDown()
    {
        poDiff = Mathf.Abs(transform.position.x - 0);

        x = Mathf.Clamp(poDiff * maxScaleDiff / maxPoDiff, 0, maxScaleDiff);
        transform.localScale = new Vector2(1 - x, 1 - x);
    }
    //900*1200
}
