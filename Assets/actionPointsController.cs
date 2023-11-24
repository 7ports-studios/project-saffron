using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class actionPointsController : MonoBehaviour
{

    public int actionPoints, maxActionPoints = 100;
    public bool caught = false;
    float timer = 0;
    Vector3 currentPosition;
    public static actionPointsController instance;
    [SerializeField] private Image actionPointsBar;
    [SerializeField] private TextMeshProUGUI actionPointsCounter;
    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        //currentPosition = transform.position;
        //refill action points once every 5 seconds if not being caught
        if((actionPoints < maxActionPoints) && !caught) {

            timer += Time.deltaTime;
            if(timer >= 10)
            {
                actionPoints++;
                timer = 0;
            }
        
        }
        actionPointsBar.fillAmount = (float) actionPoints/maxActionPoints;
        actionPointsCounter.text = "" + actionPoints;
    }


    void OnResume(InputValue value)
    {
        if(value.isPressed)
        {
            caught = false;
        }
    }


    public void removeActionPoints(int pointsToRemove)
    {
        actionPoints -= pointsToRemove;
    }

    IEnumerator beingCaught()
    {
        float walkedDistance = 0;

        while(actionPoints > 0 && caught) {
            Debug.Log(walkedDistance); 
            if(transform.position != currentPosition) { 
                walkedDistance += Vector3.Distance(transform.position, currentPosition);
                currentPosition = transform.position;
            }
            while(walkedDistance >= 1)
            {
                walkedDistance -= 1;
                actionPoints--;
            }


        
            yield return new WaitForEndOfFrame();
        
        }
        yield return null;
    }
}
