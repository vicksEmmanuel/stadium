using System;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace Middleware {
    public class Manager {

        private SpeechConfig _speech;

        public Manager()
        {
            DotNetEnv.Env.Load("../.env");
            _speech = SpeechConfig.FromSubscription(DotNetEnv.Env.GetString("SPEECH_TO_TEXT_KEY"), DotNetEnv.Env.GetString("SPEECH_TO_TEXT_REGION_KEY"));
        }

        public SpeechConfig GetSpeech() {
            return _speech;
        }
    }
}