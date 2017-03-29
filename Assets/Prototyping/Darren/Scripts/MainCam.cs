using UnityEngine;

/// <summary>
/// MainCam
/// Moves the camera from room to room via nodes above each room.
/// </summary>
public class MainCam : MonoBehaviour
{

    public float speed = 10.0f;
    public Vector3 startPos;

    private Vector3 moveTo;

    /// <summary>
    /// Use this for initialization
    /// Set the moveTo position to be the starting position.
    /// </summary>
    void Start()
    {
        moveTo = startPos;
    }


    /// <summary>
    /// Update is called once per frame
    /// If the camera needs to move it will move towards each frame
    /// </summary>
    void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, moveTo, step);
    }

    /// <summary>
    /// Set the position the camera should move to
    /// </summary>
    /// <param name="target">New camera position</param>
    public void MoveCamera(Vector3 target)
    {
        moveTo = target;
    }
}