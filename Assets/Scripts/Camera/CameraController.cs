using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Player _player;

    private Vector3 _offset;

	void Start () {
        //_offset = _camera.transform.position - _player.transform.position;
        if(_player == null) {
            _player = FindObjectOfType<Player>();
        }
	}

    private void LateUpdate() {
        if (_player.IsActive) {
            _camera.transform.Translate(new Vector3(_player.AutoMoveX * Time.deltaTime, 0, 0));
        }
        //Vector3 screenPoint = _camera.WorldToViewportPoint(_player.transform.position);
        //bool onScreen = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

        //if (!onScreen) {
        //    Debug.Log("Player not visible: " + screenPoint);
        //}

        //_camera.transform.position = _player.transform.position + _offset;
    }

}
