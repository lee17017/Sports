using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beacon : MonoBehaviour {

    // Damit man während man einen Gegner markiert direkt sieht, in welche Richtung er sich bewegt
	void OnDrawGizmosSelected()
    {
        //TBD: besseres Gizmo nehmen
        Gizmos.DrawIcon(transform.position, "Light Gizmo.tiff", true);
    }
}
