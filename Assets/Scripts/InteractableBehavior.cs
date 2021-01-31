using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBehavior : MonoBehaviour {
    public bool Lost = false;
    public bool Found = false;

    public int Value = 0;

    public ItemSpaceBehavior HomeSpace;

    private ItemSpaceBehavior _occupiedSpace;

    public void Lift(PlayerBehavior player) {
        OccupySpace(null);
        transform.SetParent(player.transform);
        transform.localPosition = new Vector3(0.0f, 1.25f, 0.0f);
        //transform.localRotation = new Quaternion();
        //_occupiedSpace.item = null;
        //_occupiedSpace = null;

        player.HeldItem = this;
        player.NearbyItems.Remove(this);

        Found = true;
    }

    public void Drop(PlayerBehavior player, ItemSpaceBehavior space) {
        //Place the item in the location of the item space
        OccupySpace(space);

        //Clear the player's inventory
        player.HeldItem = null;
        if (!player.NearbyItems.Contains(this))
            player.NearbyItems.Add(this);

        //Flag as not Lost if in the correct space
        if (_occupiedSpace == HomeSpace) {
            if (Value == -1) Value = 0;
            Lost = false;
            Found = true;
        }

    }

    public void Scatter(List<ItemSpaceBehavior> lostSpaces) {
        Value = -1;
        Lost = true;
        //Build the list of spaces available
        List<ItemSpaceBehavior> freeSpaces = new List<ItemSpaceBehavior>();
        foreach (ItemSpaceBehavior space in lostSpaces) {
            if (!space.item) {
                freeSpaces.Add(space);
            }
        }
        //Stop if there are no free spaces
        if (freeSpaces.Count <= 0) return;
        //Reposition at a random free space
        int index = Random.Range(0, freeSpaces.Count);
        OccupySpace(freeSpaces[index]);
    }

    public void OccupySpace(ItemSpaceBehavior space) {
        //Evacuate the previous space
        if (_occupiedSpace) {
            _occupiedSpace.item = null;
        }
        //Occupy the new space
        _occupiedSpace = space;
        if (space) {
            space.item = this;
            transform.parent = space.transform;
        }
        else {
            transform.parent = null;
        }
        transform.localPosition = new Vector3();
        transform.localRotation = new Quaternion();
    }

}
