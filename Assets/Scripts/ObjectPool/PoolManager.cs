using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    public List<GameObject> poolPrefabs;
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();
    private Queue<GameObject> soundQueue = new Queue<GameObject>();

    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent    += OnParticleEffectEvent;
        EventHandler.InitSoundEffect        += InitSoundEffect;
    }

    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent    -= OnParticleEffectEvent;
        EventHandler.InitSoundEffect        -= InitSoundEffect;
    }

    private void Start()
    {
        CreatePool();
    }

    /// <summary>
    /// 生成对象池
    /// </summary>
    private void CreatePool()
    {
        foreach(GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, parent),
                e => { e.SetActive(true); },
                e => { e.SetActive(false); },
                e => { Destroy(e); }
                );

            poolEffectList.Add(newPool);
        }
    }

    private void OnParticleEffectEvent(ParticleEffectType effectType, Vector3 pos)
    {
        // WORKFLOW:根据特效补全
        ObjectPool<GameObject> objPool = effectType switch
        {
            ParticleEffectType.LeaceFalling01  => poolEffectList[0],
            ParticleEffectType.LeaceFalling02  => poolEffectList[1],
            ParticleEffectType.Rock            => poolEffectList[2],
            ParticleEffectType.ReapableScenery => poolEffectList[3],
            _                                  => null,
        };

        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool, obj));
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool,GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        pool.Release(obj);
    }

    //private void InitSoundEffect(SoundDetails soundDatails)
    //{
    //    Debug.Log("InitSoundEffect");
    //    ObjectPool<GameObject> objPool = poolEffectList[4];
    //    var obj = objPool.Get();

    //    obj.GetComponent<Sound>().SetSound(soundDatails);
    //    StartCoroutine(DisableSound(objPool, obj, soundDatails));
    //}

    //private IEnumerator DisableSound(ObjectPool<GameObject> pool, GameObject obj, SoundDetails soundDatails)
    //{
    //    //yield return new WaitForSeconds(1.5f);
    //    yield return new WaitForSeconds(soundDatails.soundClip.length);
    //    pool.Release(obj);
    //}

    /// <summary>
    /// 创建声音对象池
    /// </summary>
    private void CreateSoundPool()
    {
        var parent = new GameObject(poolPrefabs[4].name).transform;
        parent.SetParent(transform);

        for (int i = 0; i < 20; i++)
        {
            GameObject newObj = Instantiate(poolPrefabs[4], parent);
            newObj.SetActive(false);
            soundQueue.Enqueue(newObj);         // 在末尾添加元素
        }
    }

    /// <summary>
    /// 获取对象池
    /// </summary>
    /// <returns>从队列里拿出一个</returns>
    private GameObject GetPoolObject()
    {
        if (soundQueue.Count < 2)
            CreateSoundPool();
        return soundQueue.Dequeue();        // 读取头部元素并删除
    }

    /// <summary>
    /// 播放声音
    /// </summary>
    /// <param name="soundDetails"></param>
    private void InitSoundEffect(SoundDetails soundDetails)
    {
        var obj = GetPoolObject();
        obj.GetComponent<Sound>().SetSound(soundDetails);
        obj.SetActive(true);
        StartCoroutine(DisableSound(obj, soundDetails.soundClip.length));
    }

    /// <summary>
    /// 直到当前声音播放完，放回队列
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator DisableSound(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
        soundQueue.Enqueue(obj);        // 在末尾添加元素
    }
}
