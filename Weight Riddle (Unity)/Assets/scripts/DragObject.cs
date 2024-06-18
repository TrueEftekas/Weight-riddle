using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour
{
    Vector3 first, second;
    public float speed = 0.4f;

    private void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
        this.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
        this.GetComponent<Rigidbody>().isKinematic = true;

        float planeY = 3;
        Transform draggingObject = transform;

        Plane plane = new Plane(Vector3.up, Vector3.up * planeY); // ground plane

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float distance; // the distance from the ray origin to the ray intersection of the plane
        if (plane.Raycast(ray, out distance))
        {
            draggingObject.position = ray.GetPoint(distance); // distance along the ray
        }
    }
    private void OnMouseUp()
    {
        this.GetComponent<Rigidbody>().isKinematic = false;
        this.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
}
