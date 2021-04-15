using UnityEngine;

public class ZombieController : ZombieContext
{
    // Start is called before the first frame update
    void Start()
    {
        InitializeContext();
    }

    private void Update()
    {
        ManageState(this);
    }

    private void OnDrawGizmos()
    {
        Debug.Log("Drawing Gizmo");
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.currentTarget, this.currentTarget + new Vector3(0, 10, 0));
    }
}