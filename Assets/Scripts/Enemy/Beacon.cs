using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour {

	private GameObject parent;

	void Start() {
		//parent = this.transform.parent.gameObject;
	}

    // Damit man während man einen Gegner markiert direkt sieht, in welche Richtung er sich bewegt
	void OnDrawGizmosSelected()
    {
        //TBD: besseres Gizmo nehmen
        Gizmos.DrawIcon(transform.position, "Light Gizmo.tiff", true);
		parent = this.transform.parent.gameObject;

		if (parent != null) {
			Gizmos.color = Color.blue;
			Gizmos.DrawLine (transform.position, parent.transform.position);
		}
    }
}
