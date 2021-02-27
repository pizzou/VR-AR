using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public static Vector3 currentScale;

    private Touch oldTouch1;
    private Touch oldTouch2;

    private float minFov = 0.5f;
    private float maxFov = 1.5f;

    float field;
    private Camera main;
    private Transform pai;
    private void Start()
    {
        main = Camera.main;
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            // rotate
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 deltaPos = touch.deltaPosition;
                transform.Rotate(Vector3.down * deltaPos.x, Space.Self);
                pai.Rotate(Vector3.up * deltaPos.x, Space.World);
            }
            if (Input.touchCount == 2)
            {
                // Zoom
                Touch newTouch1 = Input.GetTouch(0);

                Touch newTouch2 = Input.GetTouch(1);

                if (newTouch2.phase == TouchPhase.Began)
                {
                    oldTouch2 = newTouch2;
                    oldTouch1 = newTouch1;

                    return;
                }
                float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
                float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);

                float offset = newDistance - oldDistance;


                float scaleFactor = offset / 500f;

                Vector3 localScale = transform.localScale;

                Vector3 scale = new Vector3(localScale.x + scaleFactor, localScale.y + scaleFactor, localScale.z + scaleFactor);

                if ((scale.x >= minFov && scale.x <= maxFov) && (scale.y >= minFov && scale.y <= maxFov) && (scale.z >= minFov && scale.z <= maxFov))
                {
                    transform.localScale = scale;
                    currentScale = scale;
                }

                field = main.fieldOfView;
                field -= offset / 100f;
                field = Mathf.Clamp(field, minFov, maxFov);
                main.fieldOfView = field;

                oldTouch1 = newTouch1;
                oldTouch2 = newTouch2;
            }
        }
    }
}