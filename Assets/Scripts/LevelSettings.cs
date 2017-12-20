using UnityEngine;

public class LevelSettings : MonoBehaviour {

    [SerializeField]
    private int _life;
    public int Life { get { return _life; } }

    [SerializeField]
    private float _levelEndX;
    public float LevelEndX { get { return _levelEndX; } }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(_levelEndX, -10, 0), new Vector3(_levelEndX, 10, 0));
    }
}
