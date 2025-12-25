
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform _camera;
        private void Start()
    {
        _camera = Camera.main.transform;
    }
    void Update()
    {
        transform.LookAt( _camera );
    }
}
