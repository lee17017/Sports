using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    [SerializeField]
    private Player _player;
	
    

    
    void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _player.OnFlapWings();
            Debug.Log("Up Arrow Pressed");
        }
    }

}
