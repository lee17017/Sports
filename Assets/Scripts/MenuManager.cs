using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private bool handState;
    public float interactionTimer;
    private float timer = 0;

    // Use this for initialization
    void Start()
    {
        // Init
    }

    // Update is called once per frame
    void Update()
    {
        handState = GestureHandler.getRightHandState();

        //Überprüfen, ob man die rechte Hand zuhält
        if (handState == true)
        {
            //Start Timer:
            timer += Time.deltaTime;

            if (timer >= interactionTimer)
            {
                // Interaction complete:
                timer = 0;
                startGame();
            }
        }
        else if (handState == false && timer > 0)
        {
            //Stopping Timer:
            timer = 0;
        }
    }

    public void startGame()
    {
        // More Init and Setup here!
        loadLevel(1);
    }

    void loadLevel(int level)
    {
        // Reset lives, etc...
        SceneManager.LoadScene(level);
    }
}
