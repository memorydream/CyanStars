using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CatAsset.Runtime
{
    /// <summary>
    /// 资源运行时信息
    /// </summary>
    public class AssetRuntimeInfo : IComparable<AssetRuntimeInfo>, IEquatable<AssetRuntimeInfo>
    {
        /// <summary>
        /// 所属资源包的清单信息
        /// </summary>
        public BundleManifestInfo BundleManifest;

        /// <summary>
        /// 资源清单信息
        /// </summary>
        public AssetManifestInfo AssetManifest;

        /// <summary>
        /// 已加载的资源实例
        /// </summary>
        public object Asset;
        
        /// <summary>
        /// 引用计数
        /// </summary>
        public int RefCount { get; private set; }

        /// <summary>
        /// 下游资源集合（依赖此资源的资源）
        /// </summary>
        public readonly HashSet<AssetRuntimeInfo> DownStream = new HashSet<AssetRuntimeInfo>();

        /// <summary>
        /// 增加引用计数
        /// </summary>
        public void AddRefCount(int count = 1)
        {
            RefCount += count;
            CheckLifeCycle();
        }

        /// <summary>
        /// 减少引用计数
        /// </summary>
        public void SubRefCount(int count = 1)
        {
            if (RefCount == 0)
            {
                Debug.LogError($"尝试减少引用计数为0的资源的引用计数:{this}");
                return;
            }

            RefCount -= count;
            CheckLifeCycle();
        }
        
        /// <summary>
        /// 是否未被使用
        /// </summary>
        public bool IsUnused()
        {
            return RefCount == 0;
        }

        /// <summary>
        /// 检查资源生命周期
        /// </summary>
        public void CheckLifeCycle()
        {
            BundleRuntimeInfo bundleRuntimeInfo =
                CatAssetDatabase.GetBundleRuntimeInfo(BundleManifest.RelativePath);
            
            if (IsUnused())
            {
                //从资源包的使用中资源集合删除
                bundleRuntimeInfo.RemoveUsingAsset(this);
            }
            else
            {
                bundleRuntimeInfo.AddUsingAsset(this);
            }
        }

        /// <summary>
        /// 添加上游资源（依赖此资源的资源）
        /// </summary>
        public void AddDownStream(AssetRuntimeInfo assetRuntimeInfo)
        {
            if (Asset == null)
            {
                return;
            }
            DownStream.Add(assetRuntimeInfo);
        }

        /// <summary>
        /// 移除上游资源（依赖此资源的资源）
        /// </summary>
        public void RemoveUpStream(AssetRuntimeInfo assetRuntimeInfo)
        {
            if (Asset == null)
            {
                return;
            }
            DownStream.Remove(assetRuntimeInfo);
        }
        
        public int CompareTo(AssetRuntimeInfo other)
        {
            return AssetManifest.CompareTo(other.AssetManifest);
        }

        public bool Equals(AssetRuntimeInfo other)
        {
            return BundleManifest.Equals(other.BundleManifest) && AssetManifest.Equals(other.AssetManifest);
        }

        public override string ToString()
        {
            return AssetManifest.ToString();
        }
    }
}