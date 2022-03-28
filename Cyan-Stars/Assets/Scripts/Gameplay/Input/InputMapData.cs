using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 输入映射数据
/// </summary>
[System.Serializable]
public class InputMapData
{
    /// <summary>
    /// 数据项
    /// </summary>
    [System.Serializable]
    public class Item
    {
        /// <summary>
        /// 按键
        /// </summary>
        public KeyCode key;
        
        /// <summary>
        /// 映射范围的最小值
        /// 对应NoteData中的Pos
        /// </summary>
        [Range(-1,2)]
        public float RangeMin;

        /// <summary>
        /// 映射范围的宽度
        /// 对应NoteData中的Width
        /// </summary>
        [Range(-1,2)]
        public float RangeWidth;
    }

    /// <summary>
    /// 数据项列表
    /// </summary>
    public List<Item> Items;
}