using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionBehavior : MonoBehaviour {
    public int Cycle = 1;
    public int Score = 0;

    public List<InteractableBehavior> Items = new List<InteractableBehavior>();

    private void Start() {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject interactable in interactables) {
            Items.Add(interactable.GetComponent<InteractableBehavior>());
        }
    }

    private void OnTriggerEnter(Collider other) {
        PlayerBehavior player = other.gameObject.GetComponentInParent<PlayerBehavior>();
        if (player) {
            player.SetPosition(0, 0, 0);
            //Iterate through every interactable
            foreach (InteractableBehavior item in Items) {
                //Increase the score for items correctly found
                if (item.Found && !item.Lost)
                    Score++;
                //Decrease the score for any items still lost
                else if (item.Lost)
                    Score--;
                item.Found = false;
            }
            Debug.Log("Score: " + Score);
            //Scatter an item for each cycle that has passed
            //Already lost items don't count towards the total
            int scatterCount = Cycle;
            for (int i = 0; i < scatterCount && i < Items.Count; i++) {
                InteractableBehavior item = Items[i];
                if (item.Lost)
                    scatterCount++;
                if (item.transform.parent != player.transform)
                    item.Scatter();
            }
            Cycle++;
        }
    }

}
