using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    private Vector3 _direction;
    private float _movementSpeed;
    [SerializeField]
    static float _timeToLive; //Sekunden anzahl, bevor das Projektil verschwindet

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float timeFactor = _movementSpeed * Time.deltaTime;
        Vector3 nextStep = new Vector3(transform.position.x + (_direction.x * timeFactor), transform.position.y + (_direction.y * timeFactor), transform.position.z + (_direction.z * timeFactor));
        transform.position = nextStep;

        if (!GetComponent<Renderer>().isVisible) // Wenn nicht sichtbar, verschwinden sie nach ein paar Sekunden
        {
            StartCoroutine(WaitBeforeDie());
        }
    }

    public EnemyProjectile(Vector3 direction, float speed)
    {
        _direction = direction.normalized;
        _movementSpeed = speed;
    }

    IEnumerator WaitBeforeDie()
    {
        yield return new WaitForSeconds(_timeToLive);
        Destroy(this.gameObject);
    }
}
