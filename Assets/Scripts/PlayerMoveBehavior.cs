using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMoveBehavior : MonoBehaviour
{
    public float Speed = 5.0f;

    private CharacterController _controller;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float motion = Input.GetAxis("Horizontal") * Speed;

        _controller.Move(new Vector3(motion, 0) * Time.deltaTime);
    }
}
