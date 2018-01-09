using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour {

    public float projectileSpeed;
    public float lifeTime;//maybe change to distance
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0)
            Destroy(gameObject);
        transform.Translate(Vector3.right * projectileSpeed * Time.deltaTime);
    }
}
