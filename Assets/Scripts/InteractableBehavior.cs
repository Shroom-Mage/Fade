using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBehavior : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Lift(PlayerBehavior player)
    {
        if (player.NorthItem == this || player.SouthItem == this)
        {
            transform.SetParent(player.transform);
            transform.localPosition = new Vector3(0, 2, 0);

            player.HeldItem = this;
            player.NorthItem = null;
            player.SouthItem = null;
        }
    }

    public void Drop(PlayerBehavior player, float direction)
    {
        transform.parent = null;
        transform.localPosition = new Vector3(player.transform.position.x, 0, direction);

        player.HeldItem = null;
        if (direction > 0)
            player.NorthItem = this;
        else if (direction < 0)
            player.SouthItem = this;
    }
}
