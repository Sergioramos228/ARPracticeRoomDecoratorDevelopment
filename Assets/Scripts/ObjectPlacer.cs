using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

[RequireComponent(typeof(ARRaycastManager))]
public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private Transform _objectPlace;
    [SerializeField] private Camera _camera;
    [SerializeField] private GameObject _container;

    private ARRaycastManager _aRRaycastManager;
    private GameObject _installedObject;
    private List<ARRaycastHit> _hits = new();

    private void Start()
    {
        _aRRaycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        UpdatePlacementPose();

        if (Input.touchCount == 2)
            SetObject();
    }

    private void UpdatePlacementPose()
    {
        Vector2 screenCenter = _camera.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        Ray ray = _camera.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit raycastHit))
            SetObjectPosition(raycastHit.point);
        else if (_aRRaycastManager.Raycast(screenCenter, _hits, TrackableType.PlaneWithinPolygon))
            SetObjectPosition(_hits[0].pose.position);
    }

    private void SetObjectPosition(Vector3 position)
    {
        _objectPlace.position = position;

        Vector3 cameraForward = _camera.transform.forward;
        Vector3 cameraRotation = new(cameraForward.x, 0, cameraForward.z);
        _objectPlace.rotation = Quaternion.Euler(cameraRotation);
    }

    private void SetObject()
    {
        _installedObject.GetComponent<Collider>().enabled = true;
        _installedObject.transform.parent = _container.transform;
        _installedObject = null;
    }

    public void SetInstalledObject(ItemData itemData)
    {
        if (_installedObject != null)
            Destroy(_installedObject);

        _installedObject = Instantiate(itemData.Prefab, _objectPlace);
        _installedObject.GetComponent<Collider>().enabled = false;
    }
}
