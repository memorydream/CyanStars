﻿using CyanStars.Framework;
using CyanStars.Framework.Dialogue;
using CyanStars.Framework.Event;
using Newtonsoft.Json;

namespace CyanStars.Gameplay.Dialogue
{
    [DialogueActionUnit("PlaySound", AllowMultiple = true)]
    public class PlaySoundAction : BaseActionUnit
    {
        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        public override void OnInit()
        {
            GameRoot.Event.Dispatch(EventConst.PlaySoundEvent, this, SingleEventArgs<string>.Create(FilePath));
            IsCompleted = true;
        }

        public override void OnUpdate(float deltaTime)
        {

        }

        public override void OnComplete()
        {
            IsCompleted = true;
        }
    }
}
