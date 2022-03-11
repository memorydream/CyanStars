using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音乐时间轴
/// </summary>
public partial class MusicTimeline
{
    /// <summary>
    /// 音乐时间轴数据
    /// </summary>
    public MusicTimelineData Data
    {
        get;
    }

    /// <summary>
    /// 计时器
    /// </summary>
    public float Timer
    {
        get;
        private set;
    }

    /// <summary>
    /// 时间轴结束时间
    /// </summary>
    private float endTime;
    
    /// <summary>
    /// 图层列表
    /// </summary>
    private List<Layer> layers = new List<Layer>();
    
    public MusicTimeline(MusicTimelineData data)
    {
        Data = data;
        endTime = data.Time / 1000f;
        CreateLayers();
    }

    /// <summary>
    /// 创建图层
    /// </summary>
    private void CreateLayers()
    {
        for (int i = 0; i < Data.LayerDatas.Count; i++)
        {
            Layer layer = new Layer(Data.LayerDatas[i]);
            layers.Add(layer);
        }
    }

    public void OnUpdate(float deltaTime)
    {
        Timer += deltaTime;
        
        //计算timeline速率
        float timelineSpeedRate = Data.BaseSpeed * Data.SpeedRate;

        if (Timer >= endTime)
        {
            //运行结束
            GameMgr.Instance.TimelineEnd();
            return;;
        }
        
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].OnUpdate(Timer,deltaTime,timelineSpeedRate);
        }
    }
    
    public void OnInput(InputType inputType, InputMapData.Item item)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            layers[i].OnInput(inputType,item);
        }
    }
}
