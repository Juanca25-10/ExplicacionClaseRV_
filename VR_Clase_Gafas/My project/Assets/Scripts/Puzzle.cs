using UnityEngine;

public class Puzzle : MonoBehaviour
{

    
    public Transform doorTransform;
    public float openHeight = 3f;
    public float openSpeed = 2f;
    public int requiredObjects = 3;

    private int currentObjectsInSockets = 0;

    private bool isOpening = false;
    private Vector3 closedPosition;
    private Vector3 targetOpenPosition;

    void Start()
    {
        if (doorTransform != null)
        {
            closedPosition = doorTransform.position;
            targetOpenPosition = closedPosition + new Vector3(0, openHeight, 0);
        }
    }

    void Update()
    {
        if (isOpening && doorTransform != null)
        {
            doorTransform.position = Vector3.MoveTowards(
                doorTransform.position,
                targetOpenPosition,
                openSpeed * Time.deltaTime
            );
        }
    }
    public void OnObjectSocketed()
    {
        currentObjectsInSockets++;
    }

    public void OnObjectRemoved()
    {
        currentObjectsInSockets--;
    }

    public void TryOpenDoor()
    {
        if (currentObjectsInSockets >= requiredObjects)
        {
            isOpening = true;
        }
   
    }


}
