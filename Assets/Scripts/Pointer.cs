using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
            GestureHandler.Instance.calcPositions();
            Vector2 handPos = GestureHandler.Instance.getMappedRightHandPosition();
            transform.position = _camera.ViewportToWorldPoint(new Vector3(handPos.x, handPos.y, 89));

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

    void RaycastUIObjects()
    {
        var pointer = new PointerEventData(EventSystem.current);
        // convert to screen space
        pointer.position = Camera.main.WorldToScreenPoint(transform.position);
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);
        foreach (RaycastResult result in raycastResults)
        {
            Debug.Log("Hit2 " + raycastResults.Count + result.gameObject.name);

            /*
            Button b = result.gameObject.GetComponent<Button>();
            if (b != null)
            {
                b.onClick.Invoke();
            }
             * */
        }
    }

}
