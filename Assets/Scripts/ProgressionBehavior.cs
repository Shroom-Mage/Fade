using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionBehavior : MonoBehaviour {
    public int Cycle = 1;
    public int Score = 0;

    public List<InteractableBehavior> Items = new List<InteractableBehavior>();
    public List<Transform> Homes = new List<Transform>();
    public List<Transform> Losts = new List<Transform>();
    public List<Transform> Furniture = new List<Transform>();

    public GameObject HomePrefab;

    private void Start() {
        //Set up all interactables and their homes
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject interactable in interactables) {
            InteractableBehavior item = interactable.GetComponent<InteractableBehavior>();
            Items.Add(item);
            Transform home = Instantiate(HomePrefab).transform;
            home.position = item.transform.position;
            home.rotation = item.transform.rotation;
            home.name = item.name + " Home";
            Homes.Add(home);
        }
        //Set up all lost points
        GameObject[] losts = GameObject.FindGameObjectsWithTag("Lost");
        foreach (GameObject lost in losts) {
            Furniture.Add(lost.transform);
        }
        //Set up all furniture
        GameObject[] furnitures = GameObject.FindGameObjectsWithTag("Furniture");
        foreach (GameObject furniture in furnitures) {
            Furniture.Add(furniture.transform);
        }
    }

    private void OnTriggerEnter(Collider other) {
        PlayerBehavior player = other.gameObject.GetComponentInParent<PlayerBehavior>();
        if (player) {
            player.SetPosition(0, 0, 0);
            //Shuffle the lists
            Shuffle(Items);
            Shuffle(Furniture);
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

    //Shuffle method based on the Fisher-Yates shuffle
    public void Shuffle<T>(List<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

}
