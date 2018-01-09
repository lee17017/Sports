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
        if (Input.GetKey(KeyCode.LeftArrow)) {
            _player.OnLean(-1f);
        }else if (Input.GetKey(KeyCode.RightArrow)) {
            _player.OnLean(1f);
        } else {
            _player.OnLean(0f);
        }
    }

}
