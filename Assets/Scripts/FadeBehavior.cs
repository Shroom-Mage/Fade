using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBehavior : MonoBehaviour
{
    public SpriteRenderer FadeScreen;
    public ProgressionBehavior Progression;

    private Material _material;

    private float _countdown = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //Countdown until game closes
        if (_countdown <= 0.0f || Input.GetKeyDown(KeyCode.Escape)) {
            Debug.Log("Closing");
            Application.Quit();
        } else if (_countdown < 10.0f) {
            _countdown -= Time.deltaTime;
            Debug.Log("Countdown: " + _countdown);
            return;
        }

        //Set screen alpha according to score
        Color screen = FadeScreen.color;
        screen.a = -Progression.Score * 0.025f;
        if (screen.a >= 1.0f)
            _countdown -= Time.deltaTime;
        FadeScreen.color = screen;

        //Fade the title text
        Color title = _material.color;
        if (title.a > 0) {
            title.a -= Time.deltaTime * 0.05f;
            _material.color = title;
        } else {
            transform.position = new Vector3(0, -20, 0);
        }
    }
}
