using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private bool _handState;
    public float interactionTimer;
    private float _timer = 0;

    // Update is called once per frame
    void Update()
    {
        _handState = GestureHandler.getRightHandState();

        //Überprüfen, ob man die rechte Hand zuhält
        if (_handState == true)
        {
            //Start Timer:
            _timer += Time.deltaTime;

            if (_timer >= interactionTimer)
            {
                // Interaction complete:
                _timer = 0;
                StartGame();
            }
        }
        else if (_handState == false && _timer > 0)
        {
            //Stopping Timer:
            _timer = 0;
        }
    }

    public void StartGame()
    {
        GameManager.Instance.LoadLevel(1);
    }
}
