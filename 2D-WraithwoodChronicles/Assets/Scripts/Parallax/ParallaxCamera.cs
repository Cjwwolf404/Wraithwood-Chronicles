using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(float deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;
 
    private float oldPosition;

    // Start is called before the first frame update
    void Start()
    {
        oldPosition = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x != oldPosition)
        {
            if (onCameraTranslate != null)
            {
                float delta = oldPosition - transform.position.x;
                onCameraTranslate(delta);
            }
 
            oldPosition = transform.position.x;
        }
    }
}
