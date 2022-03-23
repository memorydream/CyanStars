using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// 视图层物体脚本
/// </summary>
public class ViewObject : MonoBehaviour, IView
{
    private float deltaTime;
    public GameObject effectPrefab;
    private GameObject effectObj;
    
    public void OnUpdate(float deltaTime)
    {
        this.deltaTime = deltaTime;
        
        Vector3 pos = transform.position;
        pos.z -= deltaTime;
        transform.position = pos;
    }
    public void CreateEffectObj(float w)
    {
        effectObj = GameObject.Instantiate(effectPrefab,transform.position + new Vector3(Endpoint.Instance.GetLenth()*w/2,0,0),Quaternion.identity);
    }
    public void DestroyEffectObj()
    {
        foreach(var particle in effectObj.GetComponent<NoteClickEffect>().particleSystemList)
        {
            particle.Stop();
        }
        Destroy(effectObj,5);
    }

    public void DestroySelf(bool autoMove = true)
    {
        if (!autoMove)
        {
            Destroy(gameObject);
            return;
        }
        
        StartCoroutine(AutoMove());
    }
    
    /// <summary>
    /// 自动移动一段时间然后销毁自己
    /// </summary>
    public IEnumerator AutoMove()
    {
        float timer = 0;
        while (true)
        {
            timer += deltaTime;
            
            Vector3 pos = transform.position;
            pos.z -= deltaTime;
            transform.position = pos;

            if (timer >= 2f)
            {
                Destroy(gameObject);
                yield break;
            }
            
            yield return null;
        }
    }
}
