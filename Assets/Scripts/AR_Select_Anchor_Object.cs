using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AR_Select_Anchor_Object : MonoBehaviour
{
    [SerializeField]
    private GameObject _prefabToPlace;

    private ARRaycastManager _raycastManager;
    private ARAnchorManager _anchorManager;

    private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    private bool originPlaced = false;
    private ARAnchor currentOrigin = null;

    public float scaleFactor = 0.01f;

    private int clickNumber = 0;
    private ARAnchor lastCreated = null;

    private void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
        _anchorManager = GetComponent<ARAnchorManager>();
    }

    private void Update()
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began) { return; }

        if (_raycastManager.Raycast(touch.position, Hits, UnityEngine.XR.ARSubsystems.TrackableType.AllTypes))
        {
            ++clickNumber;

            var hitPose = Hits[0].pose;

            currentOrigin = CreateAnchor(Hits[0]);

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
                if (originPlaced && currentOrigin != null)
                {
                    // GameObject placedObject = currentOrigin.gameObject;
                    // placedObject.transform.LookAt(hit.trackable.transform);

                    currentOrigin.transform.LookAt(Hits[0].pose.position);

                    // currentOrigin.transform.LookAt(hit.trackable.transform);
                    originPlaced = false;
                    Debug.Log($"{clickNumber} Should point now");
                    return currentOrigin;
                }
                else
                {
                    if (lastCreated) {
                        Destroy(lastCreated);
                        Debug.Log("Destroyed");
                    }
                    var oldPrefab = _anchorManager.anchorPrefab;
                    _anchorManager.anchorPrefab = _prefabToPlace;
                    anchor = _anchorManager.AttachAnchor(plane, hit.pose);
                    _anchorManager.anchorPrefab = oldPrefab;
                    anchor.gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

                    Debug.Log($"{clickNumber} Created anchor attachment for plane (id: {anchor.nativePtr}).");
                    originPlaced = true;

                    lastCreated = anchor;

                    return anchor;
                }
            }
        }
        //else
        //{
        //    var instantiatedObject = Instantiate(_prefabToPlace, hit.pose.position, hit.pose.rotation);

        //    anchor = instantiatedObject.GetComponent<ARAnchor>();
        //    if (anchor == null)
        //    {
        //        anchor = instantiatedObject.AddComponent<ARAnchor>();
        //    }
        //    Debug.Log($"Created regular anchor (id: {anchor.nativePtr}).");

        //    originPlaced = true;

        //    return anchor;
        //}

        Debug.Log("Didn't hit a plane");

        currentOrigin.transform.LookAt(Hits[0].pose.position);

        return currentOrigin;
    }
}
