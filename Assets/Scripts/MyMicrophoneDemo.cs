using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;
using TMPro;
/* *** SUMMARY *** 

*/

namespace Whisper.Samples
{
    /// <summary>
    /// Record audio clip from microphone and make a transcription.
    /// </summary>
    public class MyMicrophoneDemo : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;

        [Header("UI")] 
        public Button button;
        public TextMeshProUGUI buttonText;
        public TMP_Text outputText;
        public UnityAndGeminiV3 gemini;
        
        private string _buffer;

        public ToggleIcon toggleIcon;



        private void Awake()
        {
            if (gemini == null)
                gemini = FindObjectOfType<UnityAndGeminiV3>();
            whisper.OnNewSegment += OnNewSegment;

            microphoneRecord.OnRecordStop += OnRecordStop;

            button.onClick.AddListener(OnButtonPressed);

            if (button.GetComponent<RecordButton>() == null)
            {
                RecordButton recordButton = button.gameObject.AddComponent<RecordButton>();
                recordButton.microphoneDemo = this;
            }

            whisper.language = "eng";

        }

        private void OnVadChanged(bool vadStop)
        {
            microphoneRecord.vadStop = vadStop;
        }

        public void OnButtonPressed()
        {
            if (gemini == null)
                gemini = FindObjectOfType<UnityAndGeminiV3>();

            if (gemini != null && !gemini.useMicrophoneInput)
            {
                gemini.SubmitPrompt(gemini.sampleTestPrompt);
                return;
            }

            if (!microphoneRecord.IsRecording)
            {
                microphoneRecord.StartRecord();
                toggleIcon.ToggleSprite();
            }
            else
            {
                microphoneRecord.StopRecord();
                toggleIcon.ToggleSprite();
            }
        }
        
        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            _buffer = "";

            var sw = new Stopwatch();
            sw.Start();
            
            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null) 
                return;


            var text = res.Result;

            if (outputText) {
            outputText.text = text;
            }
            if (gemini != null)
                gemini.SubmitPrompt(text);
        }
        

        
        private void OnNewSegment(WhisperSegment segment)
        {
            if (!streamSegments || !outputText)
                return;

            _buffer += segment.Text;
            if (outputText) {
                outputText.text = _buffer + "...";
            }
        }
    }
}