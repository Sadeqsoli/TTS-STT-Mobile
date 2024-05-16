using UnityEngine;

namespace SleeplessDev.TTS
{
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
			public OnInitListener() : base("android.speech.tts.TextToSpeech$OnInitListener") { }

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
}
