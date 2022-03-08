using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARTouchPlaceHologram : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefabToPlace;

    private ARRaycastManager _raycastManager;
    private ARAnchorManager _anchorManager;

    private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        _anchorManager = GetComponent<ARAnchorManager>();
    }

    void Update()
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) { return; }

        if (_raycastManager.Raycast(touch.position, Hits, UnityEngine.XR.ARSubsystems.TrackableType.AllTypes))
        {
            var hitPose = Hits[0].pose;

            CreateAnchor(Hits[0]);

            Debug.Log($"Instantiated on: {Hits[0].hitType}");
        }
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor;

        if (hit.trackable is ARPlane plane)
        {
            var planeManager = GetComponent<ARPlaneManager>();
            if (planeManager)
            {
                var oldPrefab = _anchorManager.anchorPrefab;
                _anchorManager.anchorPrefab = _prefabToPlace;
                anchor = _anchorManager.AttachAnchor(plane, hit.pose);
                _anchorManager.anchorPrefab = oldPrefab;

                Debug.Log($"Created anchor attachment for plane (id: {anchor.nativePtr}).");
                return anchor;
            }
        }



        var instantiatedObject = Instantiate(_prefabToPlace, hit.pose.position, hit.pose.rotation);

        anchor = instantiatedObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = instantiatedObject.AddComponent<ARAnchor>();
        }
        Debug.Log($"Created regular anchor (id: {anchor.nativePtr}).");

        return anchor;
    }
}
