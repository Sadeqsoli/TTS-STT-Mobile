# Unity TTS and STT Integration README

This guide provides instructions to integrate Text-to-Speech (TTS) and Speech-to-Text (STT) functionalities in a Unity project using `AndroidJavaObject` to leverage Android's native capabilities.

## Prerequisites

- Unity 2020 LTS or later (Unity 2021 LTS and Unity 2022 LTS recommended)
- Android build support installed in Unity
- Basic understanding of Unity and C#

## Setup

### 1. Permissions

Ensure you have the necessary permissions in your `AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
```

### 2. Text-to-Speech (TTS) Integration
Step 1: Create a TTS Manager Script

Create a C# script named TextToSpeechManager.cs and add the following code:

```csharp
using UnityEngine;

public class TextToSpeechManager : MonoBehaviour
{
    private AndroidJavaObject tts;

    void Start()
    {
        InitializeTTS();
    }

    private void InitializeTTS()
    {
        // Initialize the TextToSpeech object
        tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", 
            new AndroidJavaObject("android.content.ContextWrapper", 
            new AndroidJavaObject("android.app.Activity")));

        // Set the OnInitListener
        tts.Call("setOnInitListener", new OnInitListener());
    }

    private class OnInitListener : AndroidJavaProxy
    {
        public OnInitListener() : base("android.speech.tts.TextToSpeech$OnInitListener") {}

        public void onInit(int status)
        {
            if (status == 0) // TextToSpeech.SUCCESS
            {
                var tts = new AndroidJavaObject("android.speech.tts.TextToSpeech", new AndroidJavaObject("android.app.Activity"));
                tts.Call<int>("setLanguage", new AndroidJavaObject("java.util.Locale", "US"));
                tts.Call("speak", "Hello, this is a TTS test.", 0, null, null);
            }
            else
            {
                Debug.LogError("Initialization of TTS failed.");
            }
        }
    }
}
```
Step 2: Attach the Script to a GameObject

Create an empty GameObject in your Unity scene.
Attach the TextToSpeechManager script to the GameObject.
### 3. Speech-to-Text (STT) Integration
Step 1: Create an STT Manager Script

Create a C# script named SpeechToTextManager.cs and add the following code:
```csharp
using UnityEngine;

public class SpeechToTextManager : MonoBehaviour
{
    private AndroidJavaObject speechRecognizer;

    void Start()
    {
        InitializeSTT();
    }

    private void InitializeSTT()
    {
        speechRecognizer = new AndroidJavaObject("android.speech.SpeechRecognizer", 
            new AndroidJavaObject("android.content.ContextWrapper", 
            new AndroidJavaObject("android.app.Activity")));

        var listener = new RecognitionListener();
        speechRecognizer.Call("setRecognitionListener", listener);

        StartListening();
    }

    private void StartListening()
    {
        var intent = new AndroidJavaObject("android.content.Intent", "android.speech.RecognizerIntent.ACTION_RECOGNIZE_SPEECH");
        intent.Call<AndroidJavaObject>("putExtra", "android.speech.RecognizerIntent.EXTRA_LANGUAGE_MODEL", "android.speech.RecognizerIntent.LANGUAGE_MODEL_FREE_FORM");
        intent.Call<AndroidJavaObject>("putExtra", "android.speech.RecognizerIntent.EXTRA_LANGUAGE", "en-US");
        intent.Call<AndroidJavaObject>("putExtra", "android.speech.RecognizerIntent.EXTRA_PARTIAL_RESULTS", true);
        speechRecognizer.Call("startListening", intent);
    }

    private class RecognitionListener : AndroidJavaProxy
    {
        public RecognitionListener() : base("android.speech.RecognitionListener") {}

        public void onResults(AndroidJavaObject results)
        {
            var matches = results.Call<AndroidJavaObject>("getStringArrayList", "android.speech.SpeechRecognizer.RESULTS_RECOGNITION");
            if (matches != null)
            {
                string text = matches.Call<string>("get", 0);
                Debug.Log("Recognized text: " + text);
            }
        }

        public void onPartialResults(AndroidJavaObject partialResults)
        {
            // Handle partial results if needed
        }

        public void onError(int error)
        {
            Debug.LogError("Speech recognition error: " + error);
        }

        // Implement other RecognitionListener methods as needed
        public void onReadyForSpeech(AndroidJavaObject @params) {}
        public void onBeginningOfSpeech() {}
        public void onRmsChanged(float rmsdB) {}
        public void onBufferReceived(byte[] buffer) {}
        public void onEndOfSpeech() {}
        public void onEvent(int eventType, AndroidJavaObject @params) {}
    }
}
```
Step 2: Attach the Script to a GameObject

Create an empty GameObject in your Unity scene.
Attach the SpeechToTextManager script to the GameObject.
### 4. Build and Run
Open File > Build Settings.
Select Android as the target platform.
Ensure your scene is added to the build.
Click on Player Settings and ensure all necessary settings (like package name and minimum API level) are configured.
Click Build and Run to deploy the application to your Android device.
## Notes
Ensure your Android device has Google services and the necessary language packs installed for offline TTS and STT.
Handle runtime permissions for audio recording in Android 6.0 and above.
Test the functionalities thoroughly on different devices to ensure compatibility.
## Troubleshooting
TTS Initialization Failed: Ensure the language pack for the specified locale is installed on the device.
STT Errors: Check for proper microphone permissions and internet connection (if required for language model downloads).
Logs: Use adb logcat to view detailed logs from the Android device for debugging.
By following this guide, you should be able to integrate and utilize TTS and STT functionalities within your Unity project effectively.
