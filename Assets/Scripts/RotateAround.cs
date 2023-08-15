using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private Transform _rotatingTransform;
    [SerializeField] private Transform _rotateAround;
    [SerializeField] private float _anglePerSecond = 30;
    
    private void Update()
    {
        _rotatingTransform.RotateAround(_rotateAround.position, Vector3.up, _anglePerSecond * Time.deltaTime);
    }
}
