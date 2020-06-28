using Microsoft.MixedReality.Toolkit.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
#if WINDOWS_UWP
using Windows.Media.Capture;
using Windows.Storage;
#endif

public class StateManager : MonoBehaviour
{
    private int currentStep;
    private int numSteps;
    private ModelKit modelKit;

    [SerializeField]
    private GameObject[] progressUI; // Buttons, Step indicator
    [SerializeField]
    private GameObject partsList; // Billboard Object that shows the runner
    [SerializeField]
    private GameObject animationObj; // Animating Objects
    [SerializeField]
    private TextMesh introText;
    [SerializeField]
    private GameObject tutorialUI;
    [SerializeField]
    private TextMesh progressText;

    private ModelKit retrieveModelKit()
    {
        return new ModelKit("Haro", 23, animationObj.transform.childCount);
    }
    public void Start()
    {
        modelKit = retrieveModelKit();
        numSteps = modelKit.steps;
        currentStep = 0;

        for (int i = 0; i < numSteps; i++)
        {
            // set inactive if it is not inactive already
            animationObj.transform.GetChild(i).gameObject.SetActive(false);
            partsList.transform.GetChild(i).gameObject.SetActive(false);
            for (int j = 0; j < progressUI.Length; j++) progressUI[j].SetActive(false);
        }

    }
    public void exitTutorial()
    {
        Debug.Log("exit tutorial");
        tutorialUI.SetActive(false);
        currentStep = 1;
        updateProgressUI(0);
    }

    public void updateProgressStep(string type)
    {
        int prevStep = currentStep;
        Debug.Log("Updating step with " + type);
        if (type == "next") currentStep += 1;
        else if (type == "back") currentStep -= 1;
        else if (type == "reset") currentStep = 0;
        updateProgressUI(prevStep);

    }
    public void updateProgressUI(int prevStep)
    {
        Debug.Log("Updating Progress UI. currentStep: " + currentStep);

        if (currentStep <= numSteps)
        {
            progressText.text = currentStep + " / " + numSteps;
            if (prevStep == 0) // if user just finished tutorial and starts the building procedure
            {
                for (int i = 0; i < progressUI.Length; i++) progressUI[i].SetActive(true); // show back and next buttons
                progressText.gameObject.SetActive(true);
                animationObj.transform.GetChild(currentStep - 1).gameObject.SetActive(true);
                partsList.transform.GetChild(currentStep - 1).gameObject.SetActive(true);

            }
            else // show animation for new step, and deactivate previous step objects
            {
                animationObj.transform.GetChild(prevStep - 1).gameObject.SetActive(false);
                partsList.transform.GetChild(prevStep - 1).gameObject.SetActive(false);
                animationObj.transform.GetChild(currentStep - 1).gameObject.SetActive(true);
                partsList.transform.GetChild(currentStep - 1).gameObject.SetActive(true);
            }

        }

        else
        {
            animationObj.transform.GetChild(prevStep - 1).gameObject.SetActive(false);
            partsList.transform.GetChild(prevStep - 1).gameObject.SetActive(false);
            for (int i = 0; i < progressUI.Length; i++) progressUI[i].SetActive(false);
            progressText.text = "You're done :D";
            progressText.gameObject.SetActive(true);
            Debug.Log("You're done :D, User should be prompted to take a screenie");
            
            #if WINDOWS_UWP
            takeScreenie();
            #endif
        }
    }
#if WINDOWS_UWP
    public async void takeScreenie()
    {
        Debug.Log("Taking screenie...");

        CameraCaptureUI captureUI = new CameraCaptureUI();
        captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
        StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

        if (photo == null)
        {
            // User cancelled photo capture
            return;
        }

        StorageFolder destinationFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("ModelKitPictures",
            CreationCollisionOption.OpenIfExists);

        await photo.CopyAsync(destinationFolder, modelKit.name + ".jpg", NameCollisionOption.ReplaceExisting);
        await photo.DeleteAsync();
    }
#endif
    /*
     * Debug Handlers and unused functions
     */
    public void debugManipulationHandler()
    {
        Debug.Log("Manipulation event detected");
    }
    public void debugManipulationHandler2()
    {
        Debug.Log("Manipulation event ended");
    }
    private void changePartsListColor(int index)
    {
        var part = partsList.transform.GetChild(index).gameObject;
        var partRenderer = part.GetComponent<Renderer>();
        Debug.Log(part.name + "prev color is " + partRenderer.material.color);
        float H, S, V;

        Color.RGBToHSV(partRenderer.material.color, out H, out S, out V);
        Debug.Log("H: " + H + " S: " + S + " V: " + V);

        V -= 0.1f;
        Color newColor = Color.HSVToRGB(H, S, V);
        partRenderer.material.SetColor("_Color", newColor);
        Debug.Log(part.name + "after color is " + partRenderer.material.color);
    }

}