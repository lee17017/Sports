using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class menuBowser : MonoBehaviour {

    private SkinnedMeshRenderer _skinRend;
    private int _animation = 100;
    // Use this for initialization
    void Start ()
    {
        _skinRend = GetComponentInChildren<SkinnedMeshRenderer>();

    }
	
	// Update is called once per frame
	void Update () {
		transform.Translate(new Vector3(Time.deltaTime*3, 0, 0));
        if (transform.position.x > 50)
            transform.position = new Vector3(-25, transform.position.y, transform.position.z);
        if (_animation == 100)
            StartCoroutine("flapAnimation");
    }


    private IEnumerator flapAnimation()
    {
        for (int i = 0; i < 20; i++)
        {
            _animation = _animation - 6;
            _skinRend.SetBlendShapeWeight(0, _animation);
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < 20; i++)
        {
            _animation = _animation + 6;
            _skinRend.SetBlendShapeWeight(0, _animation);
            yield return new WaitForEndOfFrame();
        }
    }
}
