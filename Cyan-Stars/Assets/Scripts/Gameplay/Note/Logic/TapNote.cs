using CyanStars.Framework.Utils;
using CyanStars.Gameplay.Data;
using CyanStars.Gameplay.Input;
using CyanStars.Gameplay.Loggers;
using CyanStars.Gameplay.Evaluate;

namespace CyanStars.Gameplay.Note
{
    public class TapNote : BaseNote
    {
        public override void OnUpdate(float deltaTime, float noteSpeedRate)
        {
            base.OnUpdate(deltaTime, noteSpeedRate);

            if (LogicTimer < EvaluateHelper.CheckInputEndTime)//没接住Miss
            {
                DestroySelf();//延迟销毁

                LogHelper.NoteLogger.Log(new DefaultNoteJudgeLogArgs(data, EvaluateType.Miss));//Log

                GameManager.Instance.maxScore += data.GetFullScore();//更新最理论高分
                GameManager.Instance.RefreshData(-1, -1, EvaluateType.Miss, float.MaxValue);//更新数据
            }
        }

        public override void OnUpdateInAutoMode(float deltaTime, float noteSpeedRate)//AutoMode下的更新
        {
            base.OnUpdateInAutoMode(deltaTime, noteSpeedRate);

            if (EvaluateHelper.GetTapEvaluate(LogicTimer) == EvaluateType.Exact)
            {
                viewObject.CreateEffectObj(NoteData.NoteWidth);//生成特效
                DestroySelf(false);//销毁

                LogHelper.NoteLogger.Log(new DefaultNoteJudgeLogArgs(data, EvaluateType.Exact));//Log
                
                GameManager.Instance.maxScore += data.GetFullScore();//更新理论最高分
                GameManager.Instance.RefreshData(addCombo: 1, 
                addScore: data.GetFullScore(),
                grade: EvaluateType.Exact, currentDeviation: float.MaxValue);//更新数据
            }
        }

        public override void OnInput(InputType inputType)
        {
            base.OnInput(inputType);

            if (inputType != InputType.Down) return;                               //只处理按下的情况

            viewObject.CreateEffectObj(NoteData.NoteWidth);                               //生成特效
            DestroySelf(false);                                                   //销毁

            EvaluateType evaluateType = EvaluateHelper.GetTapEvaluate(LogicTimer);//获取评价类型

            LogHelper.NoteLogger.Log(new DefaultNoteJudgeLogArgs(data, evaluateType));//Log

            GameManager.Instance.maxScore += data.GetFullScore();                 //更新理论最高分
            GameManager.Instance.RefreshData(addCombo: 1, 
            addScore: EvaluateHelper.GetScoreWithEvaluate(evaluateType) * data.GetMagnification(),
            grade: evaluateType, currentDeviation: LogicTimer);//更新数据

        }
    }
}
