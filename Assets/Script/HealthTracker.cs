using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthTracker : MonoBehaviour
{
    public Slider HealthBarSlider;
    public Image sliderFill;

    public Material greenEmission;
    public Material yellowEmission;
    public Material redEmission;
    public Material shockEmission;


    private Coroutine smoothHealthChangeCoroutine;
    private Coroutine healthBarShock;

    // Call this method to update the health bar and color
    public void UpdateSliderValue(float currentHealth, float maxHealth)
    {
        // Calculate the health percentage
        float healthPercentage = Mathf.Clamp01(currentHealth / maxHealth);

        // Update the slider value and size
       // HealthBarSlider.value = healthPercentage;



        // If there is an ongoing smooth health change coroutine, stop it
        if (smoothHealthChangeCoroutine != null)
        {
            StopCoroutine(smoothHealthChangeCoroutine);
        }
        if (healthBarShock != null) 
        {
            StopCoroutine(healthBarShock);
        }

        // Start a new coroutine for smooth health change
        smoothHealthChangeCoroutine = StartCoroutine(SmoothHealthChange(HealthBarSlider.value, healthPercentage, 0.1f));
        healthBarShock = StartCoroutine(HearthBarShock(0.1f));


        // Update the color based on health percentage
        UpdateColor(healthPercentage);
    }

    // Coroutine for smooth health change
    private IEnumerator SmoothHealthChange(float startValue, float targetValue, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            HealthBarSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        HealthBarSlider.value = targetValue;

        // Clear the coroutine reference after it's finished
        smoothHealthChangeCoroutine = null;
    }


    private IEnumerator HearthBarShock(float duration)
    {
        Material orgnMat = sliderFill.material;

        sliderFill.material = shockEmission;
        yield return new WaitForSeconds(duration / 3);

        if(sliderFill.material != shockEmission)
        {
            sliderFill.material = sliderFill.material;
        }
        else
        {
            sliderFill.material = orgnMat;
        }
        yield return new WaitForSeconds(duration / 3);

        sliderFill.material = shockEmission;
        yield return new WaitForSeconds(duration / 3);

        if (sliderFill.material != shockEmission)
        {
            sliderFill.material = sliderFill.material;
        }
        else
        {
            sliderFill.material = orgnMat;
        }
    }

    // Set the color based on the health percentage
    private void UpdateColor(float healthPercentage)
    {
        if (healthPercentage >= 0.6f)
        {
            sliderFill.material = greenEmission;
        }
        else if (healthPercentage >= 0.3f)
        {
            sliderFill.material = yellowEmission;
        }
        else
        {
            sliderFill.material = redEmission;
        }
    }

}
