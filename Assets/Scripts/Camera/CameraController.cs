using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Transform _player;

    private Vector3 _offset;

	void Start () {
        _offset = _camera.transform.position - _player.position;
	}

    private void LateUpdate() {
        _camera.transform.position = _player.position + _offset;
    }

}
