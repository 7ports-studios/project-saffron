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
    [SerializeField] private GameObject leftHand, rightHand, flashLightHand, garotte, oilHand;
    [SerializeField] private GameObject flashLightBulb;
    [SerializeField] private Transform followCamera;
    [SerializeField] private Animator handAnimator;
    [SerializeField] private int oilPointsCost = 15, garottePointsCost = 20;
    Vector3 garotteSpherePosition;
    float garotteRadius = 0.5f;
    bool flashLightOn = false;
    bool garotteOut = false;
    bool oilOut = false;
    [SerializeField] private GameObject oilSpill;



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
            oilOut = false;
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
            oilOut = false;

        }
    }
    void OnOil(InputValue value)
    {
        if (oilOut && value.isPressed)
        {
            oilOut = false;
        }
        else if(value.isPressed)
        {
            oilOut = true;
            flashLightOn = false;
            garotteOut = false;

        }
    }
    void OnUseItem(InputValue value)
    {
        if (value.isPressed)
        {
            if (garotteOut)
            {
                handAnimator.SetTrigger("useGarotte");
                //remove action points if using while caught, even if missing
                if (actionPointsController.instance.caught)
                {
                    actionPointsController.instance.removeActionPoints(garottePointsCost);
                }
                Collider[] garotteHits = Physics.OverlapSphere(garotteSpherePosition, garotteRadius);
                foreach (Collider hit in garotteHits)
                {
                    if (hit.CompareTag("guard"))
                    {

                        if (!hit.gameObject.GetComponent<guardPatrolController>().dead)
                        {
                            if (hit.gameObject.GetComponent<guardPatrolController>().playerIsBehind)
                            {
                                handAnimator.SetBool("hit", true);
                                hit.gameObject.GetComponent<guardPatrolController>().getStrangled();
                            }
                            //add logic to reduce garotte durability here
                        }
                    }
                }
            }
            else if (oilOut)
            {
                if (isCaught())
                {
                    actionPointsController.instance.removeActionPoints(oilPointsCost);
                }
                //add logic to remove oil from inv here
                handAnimator.SetTrigger("useOil");
                GameObject.Instantiate(oilSpill, transform.position + transform.forward, Quaternion.identity);
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
            oilHand.SetActive(false);
            garotte.SetActive(false);
        }
        else if (garotteOut)
        {
            flashLightHand.SetActive(false);
            rightHand.SetActive(false);
            leftHand.SetActive(false);
            flashLightBulb.SetActive(false);
            garotte.SetActive(true);
            oilHand.SetActive(false);
        }
        else if (oilOut)
        {
            flashLightHand.SetActive(false);
            rightHand.SetActive(false);
            leftHand.SetActive(true);
            flashLightBulb.SetActive(false);
            garotte.SetActive(false);
            oilHand.SetActive(true);
        }
        else
        {
            flashLightHand.SetActive(false);
            rightHand.SetActive(true);
            flashLightBulb.SetActive(false);
            leftHand.SetActive(true);
            garotte.SetActive(false);
            oilHand.SetActive(false);
        }


    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + transform.forward, garotteRadius);
        
    }

    bool isCaught()
    {
        return actionPointsController.instance.caught;
    }
}
