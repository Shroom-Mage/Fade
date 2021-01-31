using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(CharacterController))]
public class PlayerBehavior : MonoBehaviour {
    public float Speed = 5.0f;

    public List<InteractableBehavior> NorthItems;
    public List<InteractableBehavior> SouthItems;
    public InteractableBehavior HeldItem;
    public InteractableBehavior NearestNorthItem;
    public InteractableBehavior NearestSouthItem;

    public Mesh Walk0;
    public Mesh Walk1;
    public Mesh Walk2;
    public Mesh Walk3;
    public Mesh Hold0;
    public Mesh Hold1;
    public Mesh Hold2;
    public Mesh Hold3;

    public Transform ArrowUp;
    public Transform ArrowDown;
    public Transform ArrowGroup;

    public StudioEventEmitter Footsteps;
    public StudioEventEmitter Music;

    private CharacterController _controller;
    private MeshFilter _mesh;

    private bool _canInteract = false;
    private float _walkFrame = 0;

    // Start is called before the first frame update
    void Start() {
        _controller = GetComponent<CharacterController>();
        _mesh = GetComponentInChildren<MeshFilter>();
        Music.Play();
    }

    // Update is called once per frame
    void Update() {
        //Store initial position
        Vector3 iPosition = transform.position;

        //Calculate movement
        float motion = Input.GetAxis("Horizontal") * Speed;
        _controller.Move(new Vector3(motion, 0) * Time.deltaTime);

        //Find change in position
        Vector3 deltaPosition = transform.position - iPosition;

        //Calculate animation frame
        int previousFrame = (int)_walkFrame;
        _walkFrame += deltaPosition.x;
        if (_walkFrame >= 4) _walkFrame -= 4;
        else if (_walkFrame < 0) _walkFrame += 4;
        int currentFrame = (int)_walkFrame;

        //Animate
        if (!HeldItem) {
            if (_walkFrame >= 3) _mesh.mesh = Walk3;
            else if (_walkFrame >= 2) _mesh.mesh = Walk2;
            else if (_walkFrame >= 1) _mesh.mesh = Walk1;
            else if (_walkFrame >= 0) _mesh.mesh = Walk0;
        } else {
            if (_walkFrame >= 3) _mesh.mesh = Hold3;
            else if (_walkFrame >= 2) _mesh.mesh = Hold2;
            else if (_walkFrame >= 1) _mesh.mesh = Hold1;
            else if (_walkFrame >= 0) _mesh.mesh = Hold0;
        }

        //Play footsteps
        if (previousFrame != currentFrame && (currentFrame == 1 || currentFrame == 3))
            Footsteps.Play();

        //Face
        Quaternion newRotation = new Quaternion();
        if (motion < 0) newRotation.eulerAngles = new Vector3(0, 80, 0);
        else if (motion > 0) newRotation.eulerAngles = new Vector3(0, -80, 0);
        else if (!HeldItem) _mesh.mesh = Walk0;
        else _mesh.mesh = Hold0;
        _mesh.transform.rotation = newRotation;

        //Calculate interation
        float interact = Input.GetAxis("Vertical");
        if (_canInteract)
            if (HeldItem && interact != 0) {
                HeldItem.Drop(this, interact);
                CalculateNearestItems();
            } else if (NorthItems.Count > 0 && interact > 0) {
                if (NearestNorthItem) NearestNorthItem.Lift(this);
                CalculateNearestItems();
            } else if (SouthItems.Count > 0 && interact < 0) {
                if (NearestSouthItem) NearestSouthItem.Lift(this);
                CalculateNearestItems();
            }

        if (interact != 0)
            _canInteract = false;
        else
            _canInteract = true;
    }

    //Enter the range of an interactable item
    private void OnTriggerEnter(Collider other) {
        InteractableBehavior item = other.GetComponentInParent<InteractableBehavior>();
        if (item)
            if (item.transform.position.z > transform.position.z)
                NorthItems.Add(item);
            else if (item.transform.position.z < transform.position.z)
                SouthItems.Add(item);

        CalculateNearestItems();
    }

    //Exit the range of an interactable item
    private void OnTriggerExit(Collider other) {
        InteractableBehavior item = other.GetComponentInParent<InteractableBehavior>();
        if (item)
            if (item.transform.position.z > transform.position.z)
                NorthItems.Remove(item);
            else if (item.transform.position.z < transform.position.z)
                SouthItems.Remove(item);

        CalculateNearestItems();
    }
    
    //Find the item closest to the player in the given list of items
    private InteractableBehavior FindNearestItem(List<InteractableBehavior> items) {
        InteractableBehavior nearestItem = null;
        float nearestItemDistance = Mathf.Infinity;
        //Find the nearest item
        foreach (InteractableBehavior item in items) {
            //Skip held items
            if (item == HeldItem)
                continue;
            //Find the distance
            float itemDistance = item.transform.position.x - transform.position.x;
            //Store the item if it's the new nearest
            if (itemDistance < nearestItemDistance) {
                nearestItem = item;
                nearestItemDistance = itemDistance;
            }
        }
        //Return the nearest item
        return nearestItem;
    }

    private void CalculateNearestItems() {
        //Position arrow over nearest north item
        NearestNorthItem = FindNearestItem(NorthItems);
        if (NearestNorthItem) {
            ArrowUp.SetParent(NearestNorthItem.transform);
            ArrowUp.position = new Vector3(
                NearestNorthItem.transform.position.x,
                1,
                NearestNorthItem.transform.position.z);
        } else {
            ArrowUp.SetParent(ArrowGroup);
            ArrowUp.localPosition = new Vector3(0, 0, 0);
        }
        //Position arrow over nearest south item
        NearestSouthItem = FindNearestItem(SouthItems);
        if (NearestSouthItem) {
            ArrowDown.SetParent(NearestSouthItem.transform);
            ArrowDown.position = new Vector3(
                NearestSouthItem.transform.position.x,
                1,
                NearestSouthItem.transform.position.z);
        } else {
            ArrowDown.SetParent(ArrowGroup);
            ArrowDown.localPosition = new Vector3(0, 0, 0);
        }
    }

    public void SetPosition(Vector3 position) {
        _controller.enabled = false;
        transform.position = position;
        _controller.enabled = true;
    }

    public void SetPosition(float x, float y, float z) {
        SetPosition(new Vector3(x, y, z));
    }

}
