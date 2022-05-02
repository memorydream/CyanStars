using System;
using System.Collections.Generic;
using UnityEngine;

namespace CyanStars.Framework.FSM
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    public class FSM
    {
        /// <summary>
        /// 状态字典
        /// </summary>
        private Dictionary<Type, BaseState> stateDict = new Dictionary<Type, BaseState>();

        /// <summary>
        /// 当前状态
        /// </summary>
        private BaseState currentState;
        
        public FSM(List<BaseState> states)
        {
            for (int i = 0; i < states.Count; i++)
            {
                BaseState state = states[i];
                state.SetOwner(this);
                stateDict.Add(state.GetType(),state);
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState<T>() where T : BaseState
        {
            Type type = typeof(T);
            ChangeState(type);
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void ChangeState(Type stateType)
        {
            if (!stateDict.TryGetValue(stateType,out BaseState state))
            {
                throw new Exception($"状态切换失败，FSM的状态字典中没有此状态：{stateType}");
            }
            
            Debug.Log($"状态切换：{currentState?.GetType().Name}->{stateType.Name}");
            
            currentState?.OnExit();
            currentState = state;
            currentState.OnEnter();
            
            
        }
        
        /// <summary>
        /// 轮询有限状态机
        /// </summary>
        public void OnUpdate(float deltaTime)
        {
            currentState.OnUpdate(deltaTime);
        }
    }

}
