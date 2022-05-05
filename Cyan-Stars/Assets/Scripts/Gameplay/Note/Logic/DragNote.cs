using CyanStars.Framework;
using CyanStars.Framework.Utils;
using CyanStars.Gameplay.Data;
using CyanStars.Gameplay.Input;
using CyanStars.Gameplay.Loggers;
using CyanStars.Gameplay.Evaluate;

namespace CyanStars.Gameplay.Note
{
    /// <summary>
    /// Drag音符
    /// </summary>
    public class DragNote : BaseNote
    {
        private bool isHit;

        public override bool CanReceiveInput()
        {
            return LogicTimer <= EvaluateHelper.DragTimeRange && LogicTimer >= -EvaluateHelper.DragTimeRange;
        }

        public override void OnUpdate(float deltaTime, float noteSpeedRate)
        {
            base.OnUpdate(deltaTime, noteSpeedRate);

            if (isHit && LogicTimer <= 0)//接住并过线
            {
                DestroySelf(false);//立即销毁
                return;
            }

            if (LogicTimer < -EvaluateHelper.DragTimeRange)//没接住Miss
            {
                DestroySelf();//延迟销毁

                LogHelper.NoteLogger.Log(new DefaultNoteJudgeLogArgs(data, EvaluateType.Miss));//Log

                dataModule.MaxScore += data.GetFullScore();//更新最理论高分
                dataModule.RefreshPlayingData(-1, -1, EvaluateType.Miss, float.MaxValue);//更新数据
            }
        }

        public override void OnUpdateInAutoMode(float deltaTime, float noteSpeedRate)
        {
            base.OnUpdateInAutoMode(deltaTime, noteSpeedRate);

            if (CanReceiveInput() && !isHit)
            {
                viewObject.CreateEffectObj(NoteData.NoteWidth);//生成特效
                DestroySelf(false);//立即销毁
                
                LogHelper.NoteLogger.Log(new DefaultNoteJudgeLogArgs(data, EvaluateType.Exact));//Log
                
                dataModule.MaxScore += data.GetFullScore();//更新理论最高分
                dataModule.RefreshPlayingData(addCombo: 1,
                addScore: data.GetFullScore(),
                grade: EvaluateType.Exact, currentDeviation: float.MaxValue);//更新数据

                isHit = true;
            }
        }

        public override void OnInput(InputType inputType)
        {
            base.OnInput(inputType);

            if (inputType != InputType.Press) return;//只处理按下的情况

            if (isHit) return;//已经接住了

            viewObject.CreateEffectObj(NoteData.NoteWidth);//生成特效
            LogHelper.NoteLogger.Log(new DefaultNoteJudgeLogArgs(data, EvaluateType.Exact));//Log

            dataModule.MaxScore += data.GetFullScore();//更新理论最高分

            dataModule.RefreshPlayingData(addCombo: 1, 
            addScore: EvaluateHelper.GetScoreWithEvaluate(EvaluateType.Exact) * data.GetMagnification(),
            grade: EvaluateType.Exact, currentDeviation: float.MaxValue);//更新数据


            if (LogicTimer > 0)
            {
                //早按准点放
                isHit = true;
            }
            else
            {
                //晚按即刻放
                DestroySelf(false);
            }
        }
    }
}
