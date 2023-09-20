using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRaycaster : MonoBehaviour
{
    public Vector3 OrderPositionValue => _orderPositionValue;

    [SerializeField] private Camera _cam;

    private Vector3 _orderPositionValue;

    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                _orderPositionValue = hit.point;
            }
        }
    }
}