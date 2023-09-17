using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    private LightControl[]  sceneLights;
    private LightShift      currentLightShift;
    private Season          currentSeason;
    private float           timeOfChangeLight;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
        EventHandler.LightShiftChangeEvent  += OnLightShiftChangeEvent;
        EventHandler.StartNewGameEvent      += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
        EventHandler.LightShiftChangeEvent  -= OnLightShiftChangeEvent;
        EventHandler.StartNewGameEvent      -= OnStartNewGameEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        sceneLights = FindObjectsOfType<LightControl>();

        foreach (LightControl light in sceneLights)
        {
            // ÇÐ»»µÆ¹â
            light.ChangeLightShift(currentSeason, currentLightShift, timeOfChangeLight);
        }
    }

    private void OnLightShiftChangeEvent(Season season, LightShift lightShift, float timeOfChangeLight)
    {
        currentSeason = season;
        this.timeOfChangeLight = timeOfChangeLight;
        if (currentLightShift != lightShift)
        {
            currentLightShift = lightShift;

            foreach (LightControl light in sceneLights)
            {
                // ÇÐ»»µÆ¹â
                light.ChangeLightShift(currentSeason, currentLightShift, timeOfChangeLight);
            }
        }
    }
    private void OnStartNewGameEvent(int index)
    {
        currentLightShift = LightShift.Morning;
    }
}
