using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class handController : MonoBehaviour
{
    [SerializeField] private Sprite flashLight, normalRight, normalLeft;
    [SerializeField] private GameObject leftHand, rightHand, flashLightHand, garotte;
    [SerializeField] private GameObject flashLightBulb;
    [SerializeField] private Transform followCamera;
    [SerializeField] private Animator handAnimator;
    Vector3 garotteSpherePosition;
    float garotteRadius = 0.5f;
    bool flashLightOn = false;
    bool garotteOut = false;



    private void Start()
    {
        garotteSpherePosition = transform.position + transform.forward;
    }
    void OnLight(InputValue something)
    {
        if (flashLightOn && something.isPressed)
        {
            flashLightOn= false;
        }
        else if(something.isPressed) 
        {
            flashLightOn = true;
            garotteOut = false;
        }
    }
    void OnGarotte(InputValue value)
    {
        if (garotteOut && value.isPressed)
        {
            garotteOut = false;
        }
        else if(value.isPressed)
        {
            garotteOut = true;
            flashLightOn = false;

        }
    }
    void OnUseItem(InputValue value)
    {
        if (value.isPressed)
        {
            handAnimator.SetTrigger("use");    
        }
        if(garotteOut)
        {
            Collider[] garotteHits = Physics.OverlapSphere(garotteSpherePosition, garotteRadius);
            foreach(Collider hit in garotteHits)
            {
                if (hit.CompareTag("guard"))
                {

                    if (!hit.gameObject.GetComponent<guardPatrolController>().dead)
                    {
                        handAnimator.SetBool("hit", true);
                        hit.gameObject.GetComponent<guardPatrolController>().getStrangled();
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        garotteSpherePosition = transform.position + transform.forward;
        if (flashLightOn)
        {
            flashLightHand.SetActive(true);
            rightHand.SetActive(false);
            flashLightBulb.SetActive(true);
            leftHand.SetActive(true);
        }
        else if (garotteOut)
        {
            flashLightHand.SetActive(false);
            rightHand.SetActive(false);
            leftHand.SetActive(false);
            flashLightBulb.SetActive(false);
            garotte.SetActive(true);


        }
        else
        {
            flashLightHand.SetActive(false);
            rightHand.SetActive(true);
            flashLightBulb.SetActive(false);
            leftHand.SetActive(true);
            garotte.SetActive(false);
        }


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + transform.forward, garotteRadius);
        
    }
}
