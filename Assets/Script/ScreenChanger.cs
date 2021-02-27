using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenChanger : MonoBehaviour
{


    private Vector3 fp;
    private Vector3 lp;
    public Vector3 swipe;
    private float dragDistance;
        
    // Start is called before the first frame update
    void Start()
    {
        dragDistance = Screen.height * 15 / 100;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount==1)
        {
            Touch touch1 = Input.GetTouch(0);
            if(touch1.phase == TouchPhase.Began)
            {
                fp = touch1.position;
            }
            else if(touch1.phase == TouchPhase.Ended)
            {
                lp = touch1.position;
                swipe = lp - fp;

                if(swipe.x>= dragDistance)
                {
                    SceneManager.LoadScene(0);
                }

                if(swipe.x <= dragDistance)

                {
                    SceneManager.LoadScene(1);
                }

                if (swipe.x < -dragDistance) { 
                
                    SceneManager.LoadScene(2);

                }
            }
        }
    }
}
