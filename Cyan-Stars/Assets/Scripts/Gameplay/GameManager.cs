using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using CatLrcParser;
using CatTimeline;

/// <summary>
/// 游戏管理器
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public MusicTimelineSO TimelineSo;
    public InputMapSO InputMapSO;
    public CameraControllerSo CameraControllerSo;
    public EffectControllerSo EffectControllerSo;
    
    public Camera MainCamera;
    public AudioSource AudioSource;
    public Transform viewRoot;
    public GameObject TapPrefab;
    public GameObject HoldPrefab;
    public GameObject DragPrefab;
    public GameObject ClickPrefab;
    public GameObject BreakPrefab;
    
    public Button BtnStart;
    
    
    public TextAsset LrcAsset;
    public AudioClip Music;
    private InputMapData inputMapData;
    private Timeline timeline;
    private NoteTrack noteTrack;

    public KeyViewController keyViewController;

    private HashSet<KeyCode> pressedKeySet;


    private float deltaTime;
    private float lastTime = -float.Epsilon;

    public float TimelineTime
    {
        get
        {
            if(timeline == null)
            {
                return -0.1f;
            }
            return timeline.CurrentTime;
        }
    }
    
    [Header("-----UI----------")]
    public List<PlayingUI> playingUIList = new List<PlayingUI>();//游戏UI

    [Header("-----游戏数据-----")]
    [Header("1.Combo数量")]
    public int combo = 0;//Combo数量
    [Header("2.分数")]
    public float score = 0;//分数
    [Header("3.评分")]
    public EvaluateType grade;//评分
    [Header("4.当前精准度")]
    public float currentDeviation = 0;//当前精准度
    [Header("5.各个音符的偏移")]
    public List<float> deviationList = new List<float>();//各个音符的偏移
    [Header("6.理论最高分")]
    public float maxScore = 0;//理论最高分
    [Header("7.各个评分数量")]
    public int excatNum = 0;
    public int greatNum = 0;
    public int rightNum = 0;
    public int badNum = 0;
    public int missNum = 0;
    [Header("8.当前歌词")] 
    public string curLrcText;
    [Header("9.全谱总分")]
    public float fullScore = 0;
    
    [Header("-----游戏模式-----")]
    [Header("AutoMode")]
    public bool isAutoMode = false;//是否为自动模式
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        
        Instance = this;
        inputMapData = InputMapSO.InputMapData;
        pressedKeySet = new HashSet<KeyCode>();
        
        CameraControllerSo.keyFrames.Sort((x, y) => x.time.CompareTo(y.time));

        fullScore = GetFullScore();
    }
    

    private void Start()
    {
        BtnStart.onClick.AddListener(OnBtnStartClick);
    }
    
    private void Update()
    {
        if (timeline != null)
        {
            CheckKeyboardInput();

            deltaTime = AudioSource.time - lastTime;
            lastTime = AudioSource.time;
            
            timeline.OnUpdate(deltaTime);
        }
    }
    
    /// <summary>
    /// 点击开始按钮
    /// </summary>
    private void OnBtnStartClick()
    {
        MusicTimelineData data = TimelineSo.musicTimelineData;
        ViewHelper.CalViewTime(data);
        Lyric lrc = LrcParser.Parse(LrcAsset.text);
        
        timeline = new Timeline(data.Time / 1000f);
        timeline.OnStop += () =>
        {
            timeline = null;
        };
        
        //添加音符轨道
        int index = timeline.AddTrack<NoteTrack>(1, data,NoteTrack.CreateClip);
        noteTrack = timeline.GetTrack<NoteTrack>(index);
        
        //添加歌词轨道
        timeline.AddTrack<LrcTrack>(lrc.TimeTagList.Count, lrc.TimeTagList,LrcTrack.CreateClip);

        //添加相机轨道
        index = timeline.AddTrack<CameraTrack>(CameraControllerSo.keyFrames.Count, CameraControllerSo.keyFrames,CameraTrack.CreateClip);
        CameraTrack cameraTrack = timeline.GetTrack<CameraTrack>(index);
        cameraTrack.DefaultCameraPos = CameraControllerSo.defaultPosition;
        cameraTrack.CameraTrans = MainCamera.transform;
        
        //添加音乐轨道
        index = timeline.AddTrack<MusicTrack>(1, Music,MusicTrack.CreateClip);
        timeline.GetTrack<MusicTrack>(index).audioSource = AudioSource;
        
        //添加特效轨道
        index = timeline.AddTrack<EffectTrack>(EffectControllerSo.keyFrames.Count, EffectControllerSo.keyFrames, EffectTrack.CreateClip);
        EffectTrack effectTrack = timeline.GetTrack<EffectTrack>(index);
        effectTrack.Bpm = EffectControllerSo.bpm;
        effectTrack.EffectGOs = EffectControllerSo.effectList;
        effectTrack.EffectParent = EffectControllerSo.transform;
        effectTrack.Frame = EffectControllerSo.frame;
        
        Debug.Log("时间轴创建完毕");
    }

    private int GetFullScore()
    {
        int fullScore = 0;
        foreach(var layer in TimelineSo.musicTimelineData.LayerDatas)
        {
            foreach(var clip in layer.ClipDatas)
            {
                foreach(var note in clip.NoteDatas)
                {
                    fullScore += note.GetFullScore();
                }
            }
        }
        return fullScore;
    }
    

    public float TimeSchedule()
    {
        if(timeline == null)return 1;
        return timeline.CurrentTime/timeline.Length;
    }
    
    
    /// <summary>
    /// 检查键盘输入
    /// </summary>
    private void CheckKeyboardInput()
    {
        if(isAutoMode)return;
        
        for (int i = 0; i < inputMapData.Items.Count; i++)
        {
            InputMapData.Item item = inputMapData.Items[i];
            if (Input.GetKeyDown(item.key))
            {
                pressedKeySet.Add(item.key);
                ReceiveInput(InputType.Down,item);
                keyViewController.KeyDown(item);
                continue;
            }
            
            if (Input.GetKey(item.key))
            {
                ReceiveInput(InputType.Press,item);
                continue;
            }

            if (pressedKeySet.Remove(item.key))
            {
                ReceiveInput(InputType.Up,item);
                keyViewController.KeyUp(item);
            }
        }
    }

    /// <summary>
    /// 接收输入
    /// </summary>
    public void ReceiveInput(InputType inputType, InputMapData.Item item)
    {
        noteTrack.OnInput(inputType,item);
    }

    
    public void RefreshPlayingUI(int combo,float score,string grade)
    {
        foreach(var item in playingUIList)
        {
            item.Refresh(combo,score,grade,-1);
        }
    }
    public void RefreshData(int addCombo,float addScore,EvaluateType grade,float currentDeviation)
    {
        if(addCombo < 0)
        {
            combo = 0;
        }
        else
        {
            this.combo += addCombo;
            this.score += addScore;
        }
        
        this.grade = grade;

        _ = grade switch
        {
            EvaluateType.Exact => excatNum++,
            EvaluateType.Great => greatNum++,
            EvaluateType.Right => rightNum++,
            EvaluateType.Out => rightNum++,
            EvaluateType.Bad   => badNum++,
            EvaluateType.Miss  => missNum++,
            _ => throw new System.NotImplementedException()
        };

        
        if(currentDeviation < 10000)
        {
            this.currentDeviation = currentDeviation;
            deviationList.Add(currentDeviation);
        }    
        RefreshPlayingUI(combo,score,grade.ToString());
    }
}
