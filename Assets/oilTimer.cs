using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oilTimer : MonoBehaviour
{
    [SerializeField] private float timerValue, timerMax;
    
    // Start is called before the first frame update
    void Start()
    {
        timerValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(!actionPointsController.instance.caught)
            timerValue += Time.deltaTime;

        if(timerValue > timerMax)
        {
            Destroy(gameObject);
        }
    }
}
