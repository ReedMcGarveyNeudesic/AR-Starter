using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Switch_AR_Mode : MonoBehaviour
{
    private string[] buttonText;

    private Behaviour[] simpleARComponents;
    private Behaviour advancedARComponent;
    public TextMeshProUGUI buttonTextMeshPro;

    private GameObject[] prefabsToDestroy;

    public bool simpleMode = true;

    void Start()
    {
        prefabsToDestroy = GameObject.FindGameObjectsWithTag("PrefabAsset");
        simpleARComponents = new Behaviour[] {
                this.gameObject.GetComponent<ARTouchPlaceHologram>(),
                this.gameObject.GetComponent<ARPointCloudManager>(),
                this.gameObject.GetComponent<ARPlaceTrackedImages>(),
                this.gameObject.GetComponent<ARTrackedImageManager>()
            };
        advancedARComponent = this.gameObject.GetComponent<AR_Select_Anchor_Object>();
        buttonText = new string[] { "Change to simple placement", "Change to advanced placement" };
        buttonTextMeshPro.GetComponent<TextMeshProUGUI>().text = buttonText[1];
    }
    public void SwitchMode()
    {
        prefabsToDestroy = GameObject.FindGameObjectsWithTag("PrefabAsset");
        if (simpleMode)
        {
            Debug.Log("simple to advanced");
            simpleMode = false;

            foreach(Behaviour component in simpleARComponents)
            {
                component.enabled = false;
            }
            advancedARComponent.enabled = true;
            buttonTextMeshPro.GetComponent<TextMeshProUGUI>().text = buttonText[0];
        } else
        {
            Debug.Log("advanced to simple");
            simpleMode = true;

            foreach (Behaviour component in simpleARComponents)
            {
                component.enabled = true;
            }
            advancedARComponent.enabled = false;
            buttonTextMeshPro.GetComponent<TextMeshProUGUI>().text = buttonText[1];
        }

        foreach(GameObject asset in prefabsToDestroy)
        {
            Destroy(asset);
        }
    }
}
