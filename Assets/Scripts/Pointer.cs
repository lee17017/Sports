using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour {

	// Use this for initialization
    private Camera _camera;
    private Renderer _rend;
    public Color normColor, activeColor;
	void Start () {
        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _rend = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (GestureHandler.Instance.detectPlayer())
        {
            Vector2 handPos = GestureHandler.Instance.getMappedRightHandPosition();
            transform.position = _camera.ViewportToWorldPoint(new Vector3(handPos.x, handPos.y, 1));

            if (GestureHandler.Instance.getRightHandState())
            {
                _rend.material.color = normColor;
            }
            else
            {
                _rend.material.color = activeColor;
            }
        }
        else
        {
           // transform.position = new Vector3(0, -100, 0);
        }
	}
}
