using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image progressBar;

    public void Start()
    {
        progressBar.type = Image.Type.Filled;
        progressBar.fillMethod = Image.FillMethod.Horizontal;
    }

    public void UpdateProgressBar(float progress)
    {
        float clampedProgress = Mathf.Clamp01(progress);
        progressBar.fillAmount = clampedProgress;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hidden()
    {
        gameObject.SetActive(false);
    }
}
