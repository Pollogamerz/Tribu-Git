using UnityEngine;

public class AnimalAccessoryDrag : MonoBehaviour
{
    private Vector3 _offset;
    private bool _isDragging = false;

    private void OnMouseDown()
    {
        _offset = transform.position - GetMouseWorldPosition();
        _isDragging = true;
    }

    private void OnMouseUp()
    {
        _isDragging = false;
    }

    private void Update()
    {
        if (_isDragging) 
        {
            transform.position = GetMouseWorldPosition() + _offset;
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}