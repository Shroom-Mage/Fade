using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBehavior : MonoBehaviour
{
    public bool InReach = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Lift(Transform playerTransform)
    {
        if (InReach)
        {
            transform.SetParent(playerTransform);
            transform.localPosition = new Vector3(0, 10, 0);

            PlayerInteractBehavior player = playerTransform.GetComponentInParent<PlayerInteractBehavior>();
            player.ItemHeld = this;
        }
    }
}
