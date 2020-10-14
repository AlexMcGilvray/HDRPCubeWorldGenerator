using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Camera viewCamera;

    public Transform cameraTarget;

    public float rotateSpeed = 10f;

    void Start()
    {
        _initialCameraPosition = viewCamera.transform.position;
        
        _distanceToTarget = Vector2.Distance(
            new Vector2(_initialCameraPosition.x,_initialCameraPosition.z), 
            new Vector2(cameraTarget.position.x,cameraTarget.position.z));
        _cameraHeight = Mathf.Abs(cameraTarget.position.y - _initialCameraPosition.y);
    }

    
    void Update()
    {
        _currentCamearRotation += Time.deltaTime * rotateSpeed;
        if (_currentCamearRotation > Mathf.PI * 2)
        {
            _currentCamearRotation = 0f;
        }

        float cameraX = Mathf.Cos(_currentCamearRotation) * _distanceToTarget;
        float cameraZ = Mathf.Sin(_currentCamearRotation) * _distanceToTarget;
        float cameraY = _cameraHeight;

        viewCamera.transform.position = new Vector3(cameraX,cameraY,cameraZ);
        viewCamera.transform.LookAt(cameraTarget);
    }
float _distanceToTarget;
 
    private Vector3 _initialCameraPosition;
    private float _cameraHeight;
    private float _initialDistanceFromCamera;

    private float _currentCamearRotation = 0f;

}
