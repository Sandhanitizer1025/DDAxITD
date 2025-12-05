using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    public GameObject CurrentActiveObject;

    [SerializeField]
    private ARTrackedImageManager trackedImageManager;

    [SerializeField]
    private GameObject[] placeablePrefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    public event Action<GameObject> OnPlantActivated;
    public event Action<GameObject> OnPlantDeactivated;

    void OnEnable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.AddListener(OnImageChanged);   // NEW API
    }

    void OnDisable()
    {
        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnImageChanged);
    }

    void Start()
    {
        SetupPrefabs();
    }

    void SetupPrefabs()
    {
        spawnedPrefabs.Clear();

        foreach (GameObject prefab in placeablePrefabs)
        {
            GameObject newObj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            newObj.name = prefab.name;
            newObj.SetActive(false);
            spawnedPrefabs.Add(prefab.name, newObj);
        }
    }

    void OnImageChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var added in args.added)
            UpdateImage(added);

        foreach (var updated in args.updated)
            UpdateImage(updated);

        foreach (var removed in args.removed)
            UpdateImage(removed.Value);
    }

    void UpdateImage(ARTrackedImage trackedImage)
    {
        if (trackedImage == null || trackedImage.referenceImage == null) return;

        string name = trackedImage.referenceImage.name;

        if (!spawnedPrefabs.TryGetValue(name, out GameObject obj))
            return;

        // Lost tracking
        if (trackedImage.trackingState == TrackingState.None ||
            trackedImage.trackingState == TrackingState.Limited)
        {
            if (obj.activeSelf)
            {
                obj.SetActive(false);

                if (CurrentActiveObject == obj)
                {
                    CurrentActiveObject = null;
                    OnPlantDeactivated?.Invoke(obj);
                }
            }

            return;
        }

        // Good tracking
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            obj.transform.position = trackedImage.transform.position;
            obj.transform.rotation = trackedImage.transform.rotation;

            if (!obj.activeSelf)
                obj.SetActive(true);

            if (CurrentActiveObject != obj)
            {
                CurrentActiveObject = obj;
                OnPlantActivated?.Invoke(obj);
            }
        }
    }
}

