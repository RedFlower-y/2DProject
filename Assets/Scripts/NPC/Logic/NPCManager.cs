using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singloten<NPCManager>
{
    public SceneRouteDataList_SO sceneRouteDate;
    public List<NPCPosition> npcPositionList;
    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();

    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
    }

    /// <summary>
    /// ��ʼ��·���ֵ�
    /// </summary>
    private void InitSceneRouteDict()
    {
        if (sceneRouteDate.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteDate.sceneRouteList)
            {
                var key = route.fromSceneName + route.goToSceneName;

                if (sceneRouteDict.ContainsKey(key))
                    continue;
                else
                    sceneRouteDict.Add(key, route);
            }
        }
    }

    /// <summary>
    /// ��������������·��
    /// </summary>
    /// <param name="fromSceneName">��ʼ����</param>
    /// <param name="goToSceneName">Ŀ�곡��</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName,string goToSceneName)
    {
        return sceneRouteDict[fromSceneName + goToSceneName];
    }
}
