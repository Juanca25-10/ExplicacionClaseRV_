using UnityEngine;
using UnityEngine.XR.Content.Interaction;

public class Palanca : MonoBehaviour
{
    public XRLever palanca;

    public float upSpeed;

    private void Update()
    {
        float speedArriba = upSpeed * (palanca.value ? 1 : 0);

        Vector3 velocity = new Vector3(0, speedArriba, 0);
        transform.position += velocity * Time.deltaTime;
    }

}
