using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(CharacterController))]
public class PlayerBehavior : MonoBehaviour {
    public float Speed = 5.0f;

    public InteractableBehavior HeldItem;
    public InteractableBehavior NearestItem;
    public ItemSpaceBehavior NearestSpace;
    public List<InteractableBehavior> NearbyItems;
    public List<ItemSpaceBehavior> NearbySpaces;

    public Mesh Walk0;
    public Mesh Walk1;
    public Mesh Walk2;
    public Mesh Walk3;
    public Mesh Hold0;
    public Mesh Hold1;
    public Mesh Hold2;
    public Mesh Hold3;

    public Transform Arrow;

    public StudioEventEmitter Footsteps;
    public StudioEventEmitter Music;

    public List<ItemSpaceBehavior> HomeSpaces;

    private CharacterController _controller;
    private MeshFilter _mesh;

    private bool _canInteract = false;
    private float _walkFrame = 0;

    // Start is called before the first frame update
    void Start() {
        _controller = GetComponent<CharacterController>();
        _mesh = GetComponentInChildren<MeshFilter>();
        //Music.Play();
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

        //Find nearest item or space
        if (!HeldItem)
            NearestItem = FindNearest(NearbyItems);
        else
            NearestSpace = FindNearest(NearbySpaces);

        //Calculate interation
        float interact = Input.GetAxis("Vertical");
        if (_canInteract) {
            if (interact < 0 && HeldItem && NearbySpaces.Count > 0) {
                if (NearestSpace) {
                    HeldItem.Drop(this, NearestSpace);
                    //NearbySpaces.Remove(NearestSpace);
                }
            } else if (interact > 0 && !HeldItem && NearbyItems.Count > 0) {
                if (NearestItem) {
                    NearestItem.Lift(this);
                    //NearbyItems.Remove(NearestItem);
                }
            }
        }
        if (interact != 0)
            _canInteract = false;
        else
            _canInteract = true;

        //Update arrow
        Quaternion quaternion = new Quaternion();
        if (HeldItem) {
            MoveArrow(NearestSpace);
            quaternion.eulerAngles = new Vector3(-90.0f, 0.0f, 0.0f);
        } else {
            MoveArrow(NearestItem);
            quaternion.eulerAngles = new Vector3(90.0f, 0.0f, 0.0f);
        }
        Arrow.rotation = quaternion;
    }

    //Enter the range of an interactable item
    private void OnTriggerEnter(Collider other) {
        InteractableBehavior item = other.GetComponent<InteractableBehavior>();
        if (item) {
            NearbyItems.Add(item);
        }
        ItemSpaceBehavior space = other.GetComponent<ItemSpaceBehavior>();
        if (space) {
            NearbySpaces.Add(space);
        }
    }

    //Exit the range of an interactable item
    private void OnTriggerExit(Collider other) {
        InteractableBehavior item = other.GetComponent<InteractableBehavior>();
        if (item) {
            NearbyItems.Remove(item);
        }
        ItemSpaceBehavior space = other.GetComponent<ItemSpaceBehavior>();
        if (space) {
            NearbySpaces.Remove(space);
        }
    }
    
    //Find the item closest to the player in the given list of items
    private InteractableBehavior FindNearest(List<InteractableBehavior> list) {
        InteractableBehavior nearest = null;
        float shortestDistance = Mathf.Infinity;
        //Find the nearest
        foreach (InteractableBehavior item in list) {
            //Skip held items
            if (item == HeldItem)
                continue;
            //Find the distance
            float distance = Mathf.Abs(item.transform.position.x - transform.position.x);
            //Store the item if it's the new nearest
            if (distance < shortestDistance) {
                nearest = item;
                shortestDistance = distance;
            }
        }
        return nearest;
    }

    //Find the space closest to the player in the given list of spaces
    private ItemSpaceBehavior FindNearest(List<ItemSpaceBehavior> list) {
        ItemSpaceBehavior nearest = null;
        float shortestDistance = Mathf.Infinity;
        //Find the nearest
        foreach (ItemSpaceBehavior space in list) {
            //Skip occupied spaces
            if (space.item)
                continue;
            //Find the distance
            float distance = Mathf.Abs(space.transform.position.x - transform.position.x);
            //Store the item if it's the new nearest
            if (distance < shortestDistance) {
                nearest = space;
                shortestDistance = distance;
            }
        }
        return nearest;
    }

    public void MoveArrow(MonoBehaviour target) {
        //Position arrow over nearest
        if (target) {
            if (Arrow.parent != target.transform) {
                Arrow.SetParent(target.transform);
                Arrow.position = new Vector3(
                    target.transform.position.x,
                    target.transform.position.y + 0.4f,
                    target.transform.position.z - 0.25f);
            } else {
                BobArrow(Arrow);
            }
        } else {
            Arrow.parent = null;
            Arrow.position = new Vector3(0, -5, 0);
        }
    }

    public void BobArrow(Transform arrow) {
        //Calculate arrow bob
        Vector3 pos = arrow.position;
        float deltaY = Mathf.Sin(Time.timeSinceLevelLoad + Time.deltaTime);
        deltaY -= Mathf.Sin(Time.timeSinceLevelLoad);
        deltaY /= 20;
        arrow.position = new Vector3(pos.x, pos.y + deltaY, pos.z);
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
