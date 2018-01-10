using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {

    [SerializeField]
    private float _speed;

    private Collider _collider;

	// Use this for initialization
	void Start () {
        _collider = this.GetComponent<Collider>();

    }
	
	// Update is called once per frame
	void Update () {
        transform.Translate(new Vector3(_speed*Time.deltaTime, 0, 0));
    }
}
