using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class LevelSettings : MonoBehaviour {

    [SerializeField]
    private int _life;
    public int Life { get { return _life; } }

    [SerializeField]
    private float _levelEndX;
    public float LevelEndX { get { return _levelEndX; } }

    [SerializeField]
    private float[] _checkpoints;
    public float[] Checkpoints { get { return _checkpoints; } }
#if UNITY_EDITOR
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(_levelEndX, -10, 0), new Vector3(_levelEndX, 10, 0));
        Handles.Label(new Vector3(_levelEndX, 1, 0), "End of Level");

        Gizmos.color = Color.green;
        for (int i=0; i<_checkpoints.Length; i++) {
            Gizmos.DrawLine(new Vector3(_checkpoints[i], -10, 0), new Vector3(_checkpoints[i], 10, 0));
            Handles.Label(new Vector3(_checkpoints[i], 1, 0), "Checkpoint " + i);
        }
        
    }
#endif

    public GameObject flagPolePrefab;
    private void Start()
    {
        flagPolePrefab = Instantiate(flagPolePrefab, new Vector3(_levelEndX - 0.9f, 1.1f, 0), Quaternion.identity);
    }

    public void WinLevel()
    {
        flagPolePrefab.GetComponent<Flagpole>().changePole();
    }
}
