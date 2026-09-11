using UnityEngine;

public class CopyCamera : MonoBehaviour
{
    public Transform target;
    public float altura;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = target.rotation;
        transform.position = new Vector3(target.position.x, target.position.y + altura, target.position.z);    
    }
}
