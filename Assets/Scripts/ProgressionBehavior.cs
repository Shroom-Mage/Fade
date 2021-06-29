using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class ItemSpace {
//    public Transform transform = null;
//    public InteractableBehavior item = null;
//}

public class ProgressionBehavior : MonoBehaviour {
    public int Cycle = 1;
    public int Score = 0;

    public List<InteractableBehavior> Items = new List<InteractableBehavior>();
    public List<ItemSpaceBehavior> HomeSpaces = new List<ItemSpaceBehavior>();
    public List<ItemSpaceBehavior> LostSpaces = new List<ItemSpaceBehavior>();
    public List<Transform> Furniture = new List<Transform>();

    public GameObject HomePrefab;

    public PlayerBehavior Player;
    public Transform Title;

    //Only sets parameters in this script
    public MusicManager musicManager;   

    private void Start() {
        //Set up all interactables and their homes
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        foreach (GameObject interactable in interactables) {
            //Store the item
            InteractableBehavior item = interactable.GetComponent<InteractableBehavior>();
            Items.Add(item);
            //Create the home space
            GameObject spaceObject = Instantiate(HomePrefab);
            ItemSpaceBehavior space = spaceObject.GetComponent<ItemSpaceBehavior>();
            //ItemSpace space = new ItemSpace();
            //space.transform = Instantiate(HomePrefab).transform;
            spaceObject.transform.position = item.transform.position;
            spaceObject.transform.rotation = item.transform.rotation;
            spaceObject.transform.name = item.name + " Home";
            //Occupy the space with the item
            item.OccupySpace(space);
            //Store the home space
            HomeSpaces.Add(space);
            item.HomeSpace = space;
        }
        Player.HomeSpaces = HomeSpaces;
        //Set up all lost points
        GameObject[] losts = GameObject.FindGameObjectsWithTag("Lost");
        foreach (GameObject lost in losts) {
            //Create the lost space
            ItemSpaceBehavior space = lost.GetComponent<ItemSpaceBehavior>();
            LostSpaces.Add(space);
        }
        //Set up all furniture
        GameObject[] furnitures = GameObject.FindGameObjectsWithTag("Furniture");
        foreach (GameObject furniture in furnitures) {
            Furniture.Add(furniture.transform);
        }
    }

    private void OnTriggerEnter(Collider other) {
        Title.transform.position = new Vector3(0, -20, 0);
        PlayerBehavior player = other.gameObject.GetComponentInParent<PlayerBehavior>();
        if (player) {
            player.SetPosition(0, 0, 0);
            //Shuffle the lists
            Shuffle(Items);
            Shuffle(Furniture);
            //Iterate through every interactable
            foreach (InteractableBehavior item in Items) {
                //Ignore held items
                if (item == player.HeldItem) { }
                else { Score += item.Value; }
                ////Increase the score for items correctly found
                //else if (item.Found && !item.Lost)
                //    Score++;
                ////Decrease the score for any items still lost
                //else if (item.Lost)
                //    Score--;
                item.Value = 0;
                item.Found = false;
            }
            Debug.Log("Score: " + Score);
            if (Score <= -20) musicManager.SetVariationsParameter(2f);
            else if (Score <= -10) musicManager.SetVariationsParameter(1f);
            //Scatter an item for each cycle that has passed
            //Already lost items don't count towards the total
            int scatterCount = Cycle;
            for (int i = 0; i < scatterCount && i < Items.Count; i++) {
                InteractableBehavior item = Items[i];
                if (item.Lost)
                    scatterCount++;
                if (item.transform.parent != player.transform)
                    item.Scatter(LostSpaces);
            }
            //Rotate furniture according to the score
            int rotateCount = -Score;
            for (int i = 0; i < rotateCount && i < Items.Count; i++) {
                Transform furniture = Furniture[i];
                if (furniture.transform.parent != player.transform)
                    furniture.Rotate(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
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
