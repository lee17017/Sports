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

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag == "Enemy") {
            if (collision.gameObject.GetComponent<Enemy>() != null)
            {
                if (collision.gameObject.GetComponent<Enemy>().getActive())
                collision.gameObject.GetComponent<Enemy>().LooseHealth(_damage);
            }
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.GetComponent<EnemyProjectile>())
        {
            Destroy(collision.gameObject);
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
