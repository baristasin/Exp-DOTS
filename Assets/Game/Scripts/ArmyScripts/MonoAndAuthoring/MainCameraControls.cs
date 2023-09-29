using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraControls : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;

    private float _positionUpdateMultiplier;

    private void Update()
    {
        if (Input.GetKey("a"))
        {
            var rightWithoutY = new Vector3(_cameraTransform.right.x, 0, _cameraTransform.right.z);
            _cameraTransform.position += -1 * rightWithoutY * Time.deltaTime * 12f * _positionUpdateMultiplier;
            _positionUpdateMultiplier += Time.deltaTime * 10f;
        }

        if (Input.GetKey("s"))
        {
            var forwardWithoutY = new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z);
            _cameraTransform.position += -1 * forwardWithoutY * Time.deltaTime * 12f * _positionUpdateMultiplier;
            _positionUpdateMultiplier += Time.deltaTime * 3f;
        }

        if (Input.GetKey("d"))
        {
            var rightWithoutY = new Vector3(_cameraTransform.right.x, 0, _cameraTransform.right.z);
            _cameraTransform.position += rightWithoutY * Time.deltaTime * 12f * _positionUpdateMultiplier;
            _positionUpdateMultiplier += Time.deltaTime * 3f;
        }

        if (Input.GetKey("w"))
        {
            var forwardWithoutY = new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z);
            _cameraTransform.position += forwardWithoutY * Time.deltaTime * 12f * _positionUpdateMultiplier;
            _positionUpdateMultiplier += Time.deltaTime * 3f;
        }

        if (Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d") || Input.GetKey("w"))
        {

        }
        else
        {
            _positionUpdateMultiplier = 1;
        }
    }
}
