using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegressionBehavior : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        PlayerBehavior player = other.gameObject.GetComponentInParent<PlayerBehavior>();
        if (player) {
            player.SetPosition(0, 0, 0);
        }
    }

}
