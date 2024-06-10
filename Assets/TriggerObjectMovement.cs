using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObjectMovement : MonoBehaviour, IInteractable
{

    [SerializeField] moveStuff objectToTrigger;
    [SerializeField] Vector3 Destination;
    [SerializeField] float duration;
    [SerializeField] float waitTime;


    
    enum modes
    {
        backAndForth,
        oneWay,
        twoInteraction,
    }

    [SerializeField] modes mode;

    Vector3 totalMovement;
    Vector3 initialPos;
    float timing = 0;
    bool triggered = false;
    bool inMovement = false;
    bool waitForSecondInteraction=false;

    // Start is called before the first frame update
    void Start()
    {
        totalMovement = Destination - objectToTrigger.transform.position;
        initialPos = objectToTrigger.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void goingOn()
    {
        inMovement = true;
        objectToTrigger.movement = totalMovement * (duration);
        timing += Time.deltaTime;

        StartCoroutine(timingGoingOn());
    }

    IEnumerator timingGoingOn()
    {
        yield return new WaitForSeconds(duration);
        objectToTrigger.movement = Vector3.zero;
        objectToTrigger.transform.position = Destination;
        switch (mode)
        {
            case modes.backAndForth:StartCoroutine(movedWait());break;
            case modes.oneWay: break;
            case modes.twoInteraction: waitForSecondInteraction = true; inMovement = false; break;
        }
    }

    IEnumerator movedWait()
    {
        yield return new WaitForSeconds(waitTime);
        goingBack();
    }



    void goingBack()
    {
        inMovement = true;
        objectToTrigger.movement = -totalMovement * (duration);
        timing -= Time.deltaTime;

        StartCoroutine(timingGoingBack());
    }

    IEnumerator timingGoingBack()
    {
        yield return new WaitForSeconds(duration);
        objectToTrigger.movement = Vector3.zero;
        objectToTrigger.transform.position = initialPos;
        inMovement = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(GetComponent<BoxCollider>().isTrigger && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            triggered = true;
            if (!inMovement)
            {
                if (waitForSecondInteraction)
                {
                    waitForSecondInteraction = false;
                    goingBack();
                }
                else
                    goingOn();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (GetComponent<BoxCollider>().isTrigger && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            triggered = false;
        }
    }

    public void Interact()
    {
        if (!GetComponent<BoxCollider>().isTrigger && !inMovement)
        {
            if (waitForSecondInteraction)
            {
                waitForSecondInteraction = false;
                goingBack();
            }
            else
                goingOn();
        }
    }
}
