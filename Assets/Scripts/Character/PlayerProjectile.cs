using UnityEngine;

[RequireComponent(typeof(Collider))]

public class PlayerProjectile : MonoBehaviour {

    [SerializeField]
    private float _speed;

    [SerializeField]
    private int _damage = 1;

    private Collider _collider;

	// Use this for initialization
	void Start () {
        _collider = this.GetComponent<Collider>();

    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<Enemy>()) {
            collision.gameObject.GetComponent<Enemy>().LooseHealth(_damage);
        }
    }

    // Update is called once per frame
    void Update () {
        transform.Translate(new Vector3(0, -(_speed * Time.deltaTime), 0));

        if (!GetComponent<Renderer>().isVisible) {
            Destroy(gameObject);
        }
    }
}
