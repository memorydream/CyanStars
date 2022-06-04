using UnityEngine;
using System.Collections.Generic;
using System.Data.SqlTypes;
using CyanStars.Framework.Timeline;
using CyanStars.Gameplay.Data;

namespace CyanStars.Gameplay.Camera
{
    /// <summary>
    /// 相机轨道
    /// </summary>
    public class CameraTrack : BaseTrack
    {
        public Vector3 DefaultCameraPos;
        public Transform CameraTrans;
        public Vector3 oldRot;

        /// <summary>
        /// 片段创建方法
        /// </summary>
        public static readonly CreateClipFunc<CameraTrack, CameraTrackData, CameraTrackData.KeyFrame> CreateClipFunc = CreateClip;

        private static BaseClip<CameraTrack> CreateClip(CameraTrack track, CameraTrackData trackData, int curIndex, CameraTrackData.KeyFrame keyFrame)
        {
            float startTime = 0;
            if (curIndex > 0)
            {
                startTime = trackData.KeyFrames[curIndex - 1].Time;
            }

            CameraClip clip = new CameraClip(startTime / 1000f, keyFrame.Time / 1000f, track, keyFrame.Position,
                keyFrame.Rotation, keyFrame.EasingType);

            return clip;
        }
    }
}
