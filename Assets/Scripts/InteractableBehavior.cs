using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBehavior : MonoBehaviour {
    public bool Lost = false;
    public bool Found = false;

    public Transform Home;

    // Start is called before the first frame update
    void Start() {
        Home = transform;
    }

    // Update is called once per frame
    void Update() {

    }

    public void Lift(PlayerBehavior player) {
        transform.SetParent(player.transform);
        transform.localPosition = new Vector3(0.0f, 1.25f, 0.0f);

        player.HeldItem = this;
        player.NorthItems.Remove(this);
        player.SouthItems.Remove(this);
    }

    public void Drop(PlayerBehavior player, float direction) {
        transform.parent = null;
        transform.localPosition = new Vector3(player.transform.position.x, 0, direction);

        player.HeldItem = null;
        if (direction > 0 && !player.NorthItems.Contains(this))
            player.NorthItems.Add(this);
        else if (direction < 0 && !player.SouthItems.Contains(this))
            player.SouthItems.Add(this);

        if (!Found) {
            Lost = !Lost;
            Found = true;
        }
    }

    public void Scatter() {
        Lost = true;
        float randX = Random.Range(5.0f, 35.0f);
        float randZ = Random.Range(-1.5f, 1.5f);
        transform.localPosition = new Vector3(randX, 0.0f, randZ);
    }
}
