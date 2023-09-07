using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LightControl : MonoBehaviour
{
    public LightPatternList_SO lightData;
    private Light2D currentLight;
    private LightDetails targetLightDetails;

    private void Awake()
    {
        currentLight = GetComponent<Light2D>();
    }

    /// <summary>
    /// 实际切换灯光
    /// </summary>
    /// <param name="season">季节</param>
    /// <param name="lightShift">时间周期对应灯光</param>
    /// <param name="timeOfChangeLight">灯光切换时间差</param>
    public void ChangeLightShift(Season season, LightShift lightShift, float timeOfChangeLight)
    {
        targetLightDetails = lightData.GetLightDetails(season, lightShift);

        if (timeOfChangeLight < Settings.lightChangeDuration)
        {
            // 还来得及切换灯光 展示过程
            var colorOffset = (targetLightDetails.lightColor - currentLight.color) / Settings.lightChangeDuration * timeOfChangeLight;
            currentLight.color += colorOffset;
            DOTween.To(() => currentLight.color,     c => currentLight.color     = c, targetLightDetails.lightColor,  Settings.lightChangeDuration - timeOfChangeLight);
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, targetLightDetails.lightAmount, Settings.lightChangeDuration - timeOfChangeLight);
        }

        if (timeOfChangeLight >= Settings.lightChangeDuration)
        {
            // 来不及切换 跳过过程
            currentLight.color = targetLightDetails.lightColor;
            currentLight.intensity = targetLightDetails.lightAmount;
        }
    }
}
