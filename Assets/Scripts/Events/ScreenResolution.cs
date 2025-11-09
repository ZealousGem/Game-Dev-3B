using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ScreenResolution : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Resolution[] sizes;
    // public Dropdown dropdown;
    public TMP_Dropdown dropdown;
    void Start()
    {
        sizes = Screen.resolutions;

        dropdown.ClearOptions();
        List<string> option = new List<string>();
        int sizesIndex = 0;

        for (int i = 0; i < sizes.Length; i++)
        {
            string choice = sizes[i].width + "x" + sizes[i].height;
            option.Add(choice);

            if (sizes[i].width == Screen.currentResolution.width && sizes[i].height == Screen.currentResolution.height)
            {
                sizesIndex = i;
            }
        }

        dropdown.AddOptions(option);
        dropdown.value = sizesIndex;
        dropdown.RefreshShownValue();
    }

    public void FullScreen(bool toggle)
    {
        Screen.fullScreen = toggle;
    }

    public void setSize(int resIndex)
    {
        Resolution resolution = sizes[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}