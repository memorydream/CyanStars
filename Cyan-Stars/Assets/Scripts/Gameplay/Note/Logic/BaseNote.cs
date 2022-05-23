using CyanStars.Framework;
using CyanStars.Gameplay.Data;
using CyanStars.Gameplay.Input;
using CyanStars.Gameplay.Evaluate;

namespace CyanStars.Gameplay.Note
{
    /// <summary>
    /// 音符基类
    /// </summary>
    public abstract class BaseNote
    {
        /// <summary>
        /// 音符数据
        /// </summary>
        protected NoteData Data;

        /// <summary>
        /// 此音符所属图层
        /// </summary>
        private NoteLayer layer;

        /// <summary>
        /// 剩余时间的倒计时（主要用于逻辑层）
        /// </summary>
        public float LogicTimer { get; private set; }

        /// <summary>
        /// 音符位置值
        /// </summary>
        public float Pos => Data.Pos;

        /// <summary>
        /// 受速率缩放影响的剩余时间的倒计时（主要用于视图层）
        /// </summary>
        public float ViewTimer;

        /// <summary>
        /// 视图层物体
        /// </summary>
        protected IView ViewObject;

        /// <summary>
        /// 是否创建过视图层物体
        /// </summary>
        private bool createdViewObject = false;

        protected MusicGameModule DataModule;

        /// <summary>
        /// 设置数据
        /// </summary>
        public virtual void Init(NoteData data, NoteLayer layer)
        {
            this.Data = data;
            this.layer = layer;
            LogicTimer = data.StartTime / 1000f;
            ViewTimer = ViewHelper.GetViewStartTime(data);

            DataModule = GameRoot.GetDataModule<MusicGameModule>();

            //考虑性能问题 不再会一开始就创建出所有Note的游戏物体
            //而是需要在viewTimer运行到一个特定时间时再创建
            //viewObject = ViewHelper.CreateViewObject(data);
        }

        /// <summary>
        /// 是否可接收输入
        /// </summary>
        public virtual bool CanReceiveInput()
        {
            return LogicTimer <= EvaluateHelper.CheckInputStartTime && LogicTimer >= EvaluateHelper.CheckInputEndTime;
        }

        /// <summary>
        /// 是否与指定范围有重合
        /// </summary>
        public virtual bool IsInRange(float min, float max)
        {
            //3种情况可能重合 1.最左侧在范围内 2.最右侧在范围内 3.中间部分在范围内
            bool result = (Data.Pos >= min && Data.Pos <= max)
                          || ((Data.Pos + NoteData.NoteWidth) >= min && (Data.Pos + NoteData.NoteWidth) <= max)
                          || (Data.Pos <= min && (Data.Pos + NoteData.NoteWidth) >= max);

            return result;
        }

        public virtual void OnUpdate(float deltaTime, float noteSpeedRate)
        {
            LogicTimer -= deltaTime;
            ViewTimer -= deltaTime * noteSpeedRate;

            TryCreateViewObject();

            ViewObject?.OnUpdate(deltaTime * noteSpeedRate);
        }

        public virtual void OnUpdateInAutoMode(float deltaTime, float noteSpeedRate)
        {
            LogicTimer -= deltaTime;
            ViewTimer -= deltaTime * noteSpeedRate;

            TryCreateViewObject();

            ViewObject?.OnUpdate(deltaTime * noteSpeedRate);
        }

        /// <summary>
        /// 尝试创建视图层物体
        /// </summary>
        private async void TryCreateViewObject()
        {
            if (ViewObject == null && ViewTimer <= ViewHelper.ViewObjectCreateTime && !createdViewObject)
            {
                //到创建视图层物体的时间点了
                createdViewObject = true;
                ViewObject = await ViewHelper.CreateViewObject(Data, this);
            }
        }

        /// <summary>
        /// 此音符有对应输入时
        /// </summary>
        public virtual void OnInput(InputType inputType)
        {
            //Debug.Log($"音符接收输入:输入类型{inputType},倒计时器:{timer},数据{data}");
        }

        /// <summary>
        /// 销毁自身
        /// </summary>
        protected void DestroySelf(bool autoMove = true)
        {
            layer.RemoveNote(this);
            ViewObject.DestroySelf(autoMove);
            ViewObject = null;
        }
    }
}
