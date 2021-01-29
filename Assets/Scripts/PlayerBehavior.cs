using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerBehavior : MonoBehaviour
{
    public float Speed = 5.0f;

    public InteractableBehavior NorthItem;
    public InteractableBehavior SouthItem;
    public InteractableBehavior HeldItem;

    private CharacterController _controller;

    private bool _canInteract = false;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate movement
        float motion = Input.GetAxis("Horizontal") * Speed;
        _controller.Move(new Vector3(motion, 0) * Time.deltaTime);

        //Calculate interation
        float interact = Input.GetAxis("Vertical");
        if (_canInteract)
            if (HeldItem && interact != 0)
                HeldItem.Drop(this, interact);
            else if (NorthItem && interact > 0)
                NorthItem.Lift(this);
            else if (SouthItem && interact < 0)
                SouthItem.Lift(this);

        if (interact != 0)
            _canInteract = false;
        else
            _canInteract = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractableBehavior item = other.GetComponentInParent<InteractableBehavior>();
        if (item)
            if (item.transform.position.z > transform.position.z)
                NorthItem = item;
            else if (item.transform.position.z < transform.position.z)
                SouthItem = item;
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableBehavior item = other.GetComponentInParent<InteractableBehavior>();
        if (item)
            if (item.transform.position.z > transform.position.z)
                NorthItem = null;
            else if (item.transform.position.z < transform.position.z)
                SouthItem = null;
    }
}
