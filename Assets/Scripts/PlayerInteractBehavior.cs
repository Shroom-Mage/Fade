using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractBehavior : MonoBehaviour
{
    public Collider NorthCollider;
    public Collider SouthCollider;

    public InteractableBehavior ItemInReach;
    public InteractableBehavior ItemHeld;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float interact = Input.GetAxis("Vertical");

        if (ItemInReach && interact != 0)
        {
            ItemInReach.Lift(transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Touch!");
        ItemInReach = other.GetComponentInParent<InteractableBehavior>();

        if (ItemInReach)
            ItemInReach.InReach = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Away");
        ItemInReach = other.GetComponentInParent<InteractableBehavior>();

        if (ItemInReach)
            ItemInReach.InReach = false;
    }
}
