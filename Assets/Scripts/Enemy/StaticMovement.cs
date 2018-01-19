using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMovement : MonoBehaviour {

	public float movementSpeed;
	public Vector3 staticDirection;

	private bool _isActive;

	// Use this for initialization
	void Start () {
		_isActive = false;
		staticDirection = staticDirection.normalized;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (_isActive) {
			float timeFactor = movementSpeed * Time.deltaTime;
			Vector3 nextStep = new Vector3 (transform.position.x + (staticDirection.x * timeFactor), transform.position.y + (staticDirection.y * timeFactor), transform.position.z + (staticDirection.z * timeFactor));
			transform.position = nextStep;
		}
	}

	void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.tag == "CameraBox" && !_isActive)
		{
			_isActive = true;
		}
	}
}
