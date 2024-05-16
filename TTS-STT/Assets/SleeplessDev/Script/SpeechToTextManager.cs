using UnityEngine;

namespace SleeplessDev.STT
{
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
			public RecognitionListener() : base("android.speech.RecognitionListener") { }

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
				var partialMatches = partialResults.Call<AndroidJavaObject>("getStringArrayList", "android.speech.SpeechRecognizer.RESULTS_RECOGNITION");
				if (partialMatches != null)
				{
					string partialText = partialMatches.Call<string>("get", 0);
					Debug.Log("Partial recognized text: " + partialText);
				}
			}

			public void onError(int error)
			{
				Debug.LogError("Speech recognition error: " + error);
			}

			public void onReadyForSpeech(AndroidJavaObject @params)
			{
				Debug.Log("Ready for speech");
			}

			public void onBeginningOfSpeech()
			{
				Debug.Log("Beginning of speech");
			}

			public void onRmsChanged(float rmsdB)
			{
				Debug.Log("RMS changed: " + rmsdB);
			}

			public void onBufferReceived(byte[] buffer)
			{
				Debug.Log("Buffer received");
			}

			public void onEndOfSpeech()
			{
				Debug.Log("End of speech");
			}

			public void onEvent(int eventType, AndroidJavaObject @params)
			{
				Debug.Log("Event occurred: " + eventType);
			}
		}
	}
}
