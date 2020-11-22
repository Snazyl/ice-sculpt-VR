using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    private InputDevice targetDevice;
    private GameObject spawnedController;
    private GameObject spawnedHandModel;
    private Animator handAnims;
    public List<GameObject> controllerPrefabs;
    public InputDeviceCharacteristics controllerCharacteristics;
    public GameObject handModelPrefab;
    public bool showController = false;

    // Start is called before the first frame update
    void Start()
    {
        tryInitialize();
    }

    // Check for controller on start of the game until one is found
    void tryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }

        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            if (prefab)
            {
                spawnedController = Instantiate(prefab, transform);
            }
            else
            {
                Debug.LogError("Could not find corresponding controller model.");
                spawnedController = Instantiate(controllerPrefabs[0], transform);
            }

            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnims = spawnedHandModel.GetComponent<Animator>();
        }
    }

    // Update hand animations based on device input
    void updateHandAnimations()
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnims.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnims.SetFloat("Trigger", 0);
        }
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnims.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnims.SetFloat("Grip", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!targetDevice.isValid)
        {
            tryInitialize();
        }
        else
        {
            if (showController)
            {
                spawnedHandModel.SetActive(false);
                spawnedController.SetActive(true);
            }
            else
            {
                spawnedHandModel.SetActive(true);
                spawnedController.SetActive(false);
                updateHandAnimations();
            }
        }
    }
}
