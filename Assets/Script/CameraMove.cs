using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraMove : MonoBehaviour
{

    public static CameraMove instance;
    [Header("model and camera initial position, automatic initialization")]
    public Vector3 cameraInitialPos;
    public Vector3 cameraInitialRot;
    public Vector3 modelInitialRot;

    [Header("object to be controlled")]
    [Tooltip("Camera to be controlled")]
    public Camera myCamera;
    [Tooltip("Camera zoom focus")]
    public Transform zoomTarget;
    [Tooltip("Model to observe")]
    public Transform model;

    [Header("set parameters")]
    [Tooltip("Zoom Rate")]
    public int zoomRate = 40;
    [Tooltip("the farthest observation distance")]
    public float maxObservationDis;
    [Tooltip("Recent Viewing Distance")]
    public float minObservationDis;
    [Tooltip("horizontal maximum moving distance")]
    public float maxDisH = 2f;
    [Tooltip("Maximum moving distance in the vertical direction")]
    public float maxDisV = 5f;
    [Tooltip("horizontal vertical plane moving speed")]
    public float deltaMoveSpeed = 2f;
    [Tooltip("speed when turning")]
    public float rotSpeed = 2f;
    [Tooltip("Maximum rotation angle in the vertical direction")]
    public float maxRotV = 90f;

    [Header("automatically obtained variables, no need to assign")]
    public float currentObservationDis;
    public float mouseX;
    public float mouseY;
    public Vector3 zoomDir;
    Vector3 mouseScreenPos;
    Vector3 mouseWorldPos;
    float rotV = 0f;

    /// <summary>
    /// Each variable when the finger is controlled
    /// </summary>
    Touch firstFinger;
    Touch secondFinger;
    TouchType touchType;
    Vector2 deltaPos;
    bool isStartIJudgeLongPress;
    bool isStartIZoomByTwoFinger;
    float twoFingerOldDis;
    float twoFingerNewDis;
    void Awake()
    {
        Init();
    }
    // Update is called once per frame
    void Update()
    {
        //The PC calls the Move() method. Function: The left mouse button controls the camera plane movement, hold down the right mouse button and drag the mouse to rotate around the object. The wheel camera zooms toward the mouse position.
        //Move();


        //Android calls this method function: one finger moves the screen model to rotate, one finger presses the screen and then slides the model plane to move, two fingers realize the model zooms in and out between the two fingers
        //MoveByFinger();
    }
    /// <summary>
    /// Initialize
    /// </summary>
    public void Init()
    {
        instance = this;
        cameraInitialPos = myCamera.transform.position;
        cameraInitialRot = myCamera.transform.localEulerAngles;
        modelInitialRot = model.localEulerAngles;
    }
    /// <summary>
    /// PC-side mouse control movement method
    /// </summary>
    private void Move()
    {
        MoveByMouse0();
        ZoomByScrollWheel();
        RotByMouse1();
    }
    /// <summary>
    /// Android is controlled by your finger
    /// </summary>
    private void MoveByFinger()
    {
        if (Input.touchCount == 0)
        {
            touchType = TouchType.NoneFinger;
            StopAllCoroutines();
            isStartIJudgeLongPress = false;
            isStartIZoomByTwoFinger = false;
        }
        else if (Input.touchCount == 1)
        {

            if (touchType == TouchType.NoneFinger && !isStartIJudgeLongPress)
            {
                isStartIJudgeLongPress = true;
                StartCoroutine(IJudgeLongPress());
                firstFinger = Input.GetTouch(0);
            }
        }
        else if (Input.touchCount == 2)
        {
            touchType = TouchType.TwoFingerZoom;
        }
        switch (touchType)
        {
            case TouchType.NoneFinger:
                break;
            case TouchType.OneFingerMove:
                MoveByOneFinger();
                break;
            case TouchType.OneFingerRot:
                RotByOneFinger();
                break;
            case TouchType.TwoFingerZoom:
                if (!isStartIZoomByTwoFinger)
                {
                    isStartIZoomByTwoFinger = true;
                    StartCoroutine("IZoomByTwoFinger");
                }
                break;
            default:
                break;
        }

    }
    /// <summary>
    /// Determine if you press and hold
    /// </summary>
    /// <returns></returns>
    IEnumerator IJudgeLongPress()
    {

        if (Input.touchCount != 0)
        {
            Vector2 firstPos = Input.GetTouch(0).position;
            yield return new WaitForSeconds(0.2f);
            if (Input.touchCount != 0)
            {
                Vector2 secondPos = Input.GetTouch(0).position;
                if ((secondPos - firstPos).magnitude < 2f)
                {
                    touchType = TouchType.OneFingerMove;
                }
                else
                {
                    touchType = TouchType.OneFingerRot;
                }
            }

        }

    }
    /// <summary>
    /// Hold down the left mouse button and drag
    /// </summary>
    public void MoveByMouse0()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse X") < 0)
            {
                //SetMoveSpeed(2.0f);
                MoveByDeltaDistance(true, true);
            }
            else if (Input.GetAxis("Mouse X") > 0)
            {
                //SetMoveSpeed(2.0f);
                MoveByDeltaDistance(true, false);

            }

            if (Input.GetAxis("Mouse Y") < 0)
            {
                //SetMoveSpeed(2.0f);
                MoveByDeltaDistance(false, true);

            }
            else if (Input.GetAxis("Mouse Y") > 0)
            {
                //SetMoveSpeed(2.0f);
                MoveByDeltaDistance(false, false);

            }
        }
    }
    /// <summary>
    /// Move the camera by keyboard or arrow keys or by clicking the button
    /// </summary>
    /// <param name="Hmoving" true means horizontal movement ></param>
    /// <param name="positiveDir" true means forward movement></param>
    public void MoveByDeltaDistance(bool Hmoving, bool positiveDir)
    {
        //target.rotation = transform.rotation;
        if (Hmoving)
        {


            if (positiveDir)
            {
                if (myCamera.transform.position.x < maxDisH)
                {
                    myCamera.transform.Translate(Vector3.right * deltaMoveSpeed * Time.deltaTime, Space.Self);
                    //target.Translate(Vector3.right * deltaMoveSpeed * Time.deltaTime);
                }
            }
            else
            {
                if (myCamera.transform.position.x > -maxDisH)
                {
                    myCamera.transform.Translate(Vector3.right * -deltaMoveSpeed * Time.deltaTime, Space.Self);
                    //target.Translate(Vector3.right * -deltaMoveSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            if (positiveDir)
            {

                if (myCamera.transform.position.y < maxDisH)
                {
                    myCamera.transform.Translate(transform.up * deltaMoveSpeed * Time.deltaTime, Space.World);
                    //target.Translate(transform.up * deltaMoveSpeed * Time.deltaTime, Space.World); 
                }
            }
            else
            {
                if (myCamera.transform.position.y > -maxDisH)
                {
                    myCamera.transform.Translate(transform.up * -deltaMoveSpeed * Time.deltaTime, Space.World);
                    //target.Translate(transform.up * -deltaMoveSpeed * Time.deltaTime, Space.World); 
                }

            }
        }


    }
    /// <summary>
    /// Pulley key zoom
    /// </summary>
    public void ZoomByScrollWheel()
    {
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheel != 0)
        {

            Vector3 zoomTargetScreenPos = myCamera.WorldToScreenPoint(zoomTarget.position);
            mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, zoomTargetScreenPos.z);
            zoomTarget.position = myCamera.ScreenToWorldPoint(mouseScreenPos);
            Ray ray = myCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                zoomTarget.position = hitInfo.point;
            }
        }
        currentObservationDis = (zoomTarget.position - myCamera.transform.position).magnitude;
        zoomDir = (zoomTarget.position - myCamera.transform.position).normalized;
        Zoom(zoomDir, scrollWheel * Time.deltaTime * zoomRate);
    }

    /// <summary>
    /// Camera moves to the object
    /// </summary>
    /// <param name="zoomDir">moving direction</param>
    /// <param name="zoomDis">moving distance</param>
    public void Zoom(Vector3 zoomDir, float zoomDis)
    {
        //target.Translate(zoomDir * zoomDis, Space.World);
        if (zoomDis < 0 && currentObservationDis > maxObservationDis)
        {
            return;
        }
        else if (zoomDis > 0 && currentObservationDis < minObservationDis)
        {
            return;
        }
        myCamera.transform.Translate(zoomDir * zoomDis, Space.World);
    }
    /// <summary>
    /// Right mouse button and hold the camera
    /// </summary>
    public void RotByMouse1()
    {
        if (Input.GetMouseButton(1))
        {

            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                if (mouseX < 0)
                {
                    model.Rotate(Vector3.up, rotSpeed);
                    //myCamera.transform.RotateAround(model.transform.position, Vector3.up, -rotSpeed);
                }
                else if (mouseX > 0)
                {
                    model.Rotate(Vector3.up, -rotSpeed);
                    //myCamera.transform.RotateAround(model.transform.position, Vector3.up, rotSpeed);

                }
            }
            else if (Mathf.Abs(mouseX) < Mathf.Abs(mouseY))
            {
                if (mouseY < 0)
                {
                    if (rotV < maxRotV)
                    {
                        myCamera.transform.RotateAround(model.transform.position, Vector3.right, rotSpeed);
                        rotV += rotSpeed;
                    }
                }
                else if (mouseY > 0)
                {
                    if (rotV > -maxRotV)
                    {
                        myCamera.transform.RotateAround(model.transform.position, Vector3.right, -rotSpeed);
                        rotV -= rotSpeed;
                    }
                    //if (myCamera.transform.localEulerAngles.x >=90)
                    //{
                    //    myCamera.transform.RotateAround(model.transform.position, Vector3.right, -rotSpeed);
                    //}
                }
            }
        }
    }
    /// <summary>
    /// Drag with one finger
    /// </summary>
    public void MoveByOneFinger()
    {

        firstFinger = Input.GetTouch(0);

        if (firstFinger.phase == TouchPhase.Moved)
        {
            deltaPos = firstFinger.deltaPosition;
            if (deltaPos.x < 0)
            {
                //SetMoveSpeed(2.0f);
                MoveByDeltaDistance(true, true);
            }
            else if (deltaPos.x > 0)
            {
                //SetMoveSpeed(2.0f);
                MoveByDeltaDistance(true, false);

            }

            if (deltaPos.y < 0)
            {
                //SetMoveSpeed(2.0f);
                MoveByDeltaDistance(false, true);

            }
            else if (deltaPos.y > 0)
            {
                //SetMoveSpeed(2.0f);
                MoveByDeltaDistance(false, false);

            }
        }
    }
    /// <summary>
    /// One finger long press
    /// </summary>
    public void RotByOneFinger()
    {
        firstFinger = Input.GetTouch(0);
        if (firstFinger.phase == TouchPhase.Moved)
        {
            deltaPos = firstFinger.deltaPosition;
            mouseX = deltaPos.x;
            mouseY = deltaPos.y;
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                if (mouseX < 0)
                {
                    model.Rotate(Vector3.up, rotSpeed);
                }
                else if (mouseX > 0)
                {
                    model.Rotate(Vector3.up, -rotSpeed);

                }
            }
            else if (Mathf.Abs(mouseX) < Mathf.Abs(mouseY))
            {
                if (mouseY < 0)
                {
                    if (rotV < maxRotV)
                    {
                        myCamera.transform.RotateAround(model.transform.position, Vector3.right, rotSpeed);
                        rotV += rotSpeed;
                    }
                }
                else if (mouseY > 0)
                {
                    if (rotV > -maxRotV)
                    {
                        myCamera.transform.RotateAround(model.transform.position, Vector3.right, -rotSpeed);
                        rotV -= rotSpeed;
                    }
                    //if (myCamera.transform.localEulerAngles.x >=90)
                    //{
                    //    myCamera.transform.RotateAround(model.transform.position, Vector3.right, -rotSpeed);
                    //}
                }
            }
        }
    }
    /// <summary>
    /// Two finger zoom coroutines
    /// </summary>
    public IEnumerator IZoomByTwoFinger()
    {
        Vector2 centerPos;
        while (true)
        {
            firstFinger = Input.GetTouch(0);
            secondFinger = Input.GetTouch(1);
            twoFingerOldDis = Vector2.Distance(firstFinger.position, secondFinger.position);
            centerPos = (firstFinger.position + secondFinger.position) / 2;
            yield return 0;
            firstFinger = Input.GetTouch(0);
            secondFinger = Input.GetTouch(1);
            twoFingerNewDis = Vector2.Distance(firstFinger.position, secondFinger.position);
            float scrollWheel = twoFingerNewDis - twoFingerOldDis;
            if (scrollWheel != 0)
            {

                Vector3 zoomTargetScreenPos = myCamera.WorldToScreenPoint(zoomTarget.position);
                mouseScreenPos = new Vector3(centerPos.x, centerPos.y, zoomTargetScreenPos.z);
                zoomTarget.position = myCamera.ScreenToWorldPoint(mouseScreenPos);
                Ray ray = myCamera.ScreenPointToRay(centerPos);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    zoomTarget.position = hitInfo.point;
                }
            }
            currentObservationDis = (zoomTarget.position - myCamera.transform.position).magnitude;
            zoomDir = (zoomTarget.position - myCamera.transform.position).normalized;
            Zoom(zoomDir, scrollWheel * Time.deltaTime * zoomRate * 0.1f);
            if (Input.touchCount != 2)
            {
                isStartIZoomByTwoFinger = false;
                break;
            }
        }


    }
    /// <summary>
    /// Reset method
    /// </summary>
    public void Reset()
    {
        myCamera.transform.position = cameraInitialPos;
        myCamera.transform.localEulerAngles = cameraInitialRot;
        model.localEulerAngles = modelInitialRot;
        rotV = 0;
    }
}
public enum TouchType
{
    NoneFinger,
    OneFingerMove,
    OneFingerRot,
    TwoFingerZoom
}