using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour {
	// Use this for initialization
    private Camera _camera;
    private Renderer _rend;
    private Image _curImage;
    public Color normColor, activeColor;
    public float holdTime;
    private float curTimer;
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
                RaycastUIObjects();
            }
            else
            {
                _rend.material.color = activeColor;
                curTimer = 0;
                if (_curImage != null)
                {
                    _curImage.fillAmount = 0;
                    _curImage = null;
                }
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
        if (raycastResults.Count > 1)
            Debug.LogWarning("More than one target hit by raycast");
   
        foreach (RaycastResult result in raycastResults)
        {
            Debug.Log("Hit " + raycastResults.Count + result.gameObject.name);

            
            Button b = result.gameObject.GetComponent<Button>();
            Image i  = result.gameObject.GetComponent<Image>();
            if(i != _curImage)
            {
                if(_curImage != null)
                    _curImage.fillAmount = 0;
                _curImage = i;
                curTimer = 0;
            }
            if(b == null){
                b = result.gameObject.GetComponentInParent<Button>();
            }
            
            if (b != null)
            {
                curTimer += Time.deltaTime;
                if(_curImage != null)
                {
                    _curImage.fillAmount = _curImage.fillAmount + Time.deltaTime * (holdTime+0.2f);
                    Debug.Log("fill");
                    Debug.Log(_curImage.fillAmount);
                }
                if (curTimer > holdTime) {
                    b.onClick.Invoke();
                }
            }
        }
        if (raycastResults.Count == 0)
        {
            curTimer = 0;
            if (_curImage != null)
            {
                _curImage.fillAmount = 0;
                _curImage = null;
            }
                
        }


    }

}
