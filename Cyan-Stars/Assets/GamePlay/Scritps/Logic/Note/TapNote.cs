using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapNote : BaseNote
{
    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (timer < EvaluateHelper.CheckInputEndTime)
        {
            //没接住 miss
            DestorySelf();
            Debug.LogError($"Tap音符miss：{data}");
        }
    }

    public override void OnInput(InputType inputType)
    {
        base.OnInput(inputType);

        if (inputType == InputType.Down)
        {
            DestorySelf(false);
            Debug.LogError($"Tap音符命中,评价:{EvaluateHelper.GetTapEvaluate(timer)},{data}");
        }
    }
}
