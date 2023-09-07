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
    /// ʵ���л��ƹ�
    /// </summary>
    /// <param name="season">����</param>
    /// <param name="lightShift">ʱ�����ڶ�Ӧ�ƹ�</param>
    /// <param name="timeOfChangeLight">�ƹ��л�ʱ���</param>
    public void ChangeLightShift(Season season, LightShift lightShift, float timeOfChangeLight)
    {
        targetLightDetails = lightData.GetLightDetails(season, lightShift);

        if (timeOfChangeLight < Settings.lightChangeDuration)
        {
            // �����ü��л��ƹ� չʾ����
            var colorOffset = (targetLightDetails.lightColor - currentLight.color) / Settings.lightChangeDuration * timeOfChangeLight;
            currentLight.color += colorOffset;
            DOTween.To(() => currentLight.color,     c => currentLight.color     = c, targetLightDetails.lightColor,  Settings.lightChangeDuration - timeOfChangeLight);
            DOTween.To(() => currentLight.intensity, i => currentLight.intensity = i, targetLightDetails.lightAmount, Settings.lightChangeDuration - timeOfChangeLight);
        }

        if (timeOfChangeLight >= Settings.lightChangeDuration)
        {
            // �������л� ��������
            currentLight.color = targetLightDetails.lightColor;
            currentLight.intensity = targetLightDetails.lightAmount;
        }
    }
}
