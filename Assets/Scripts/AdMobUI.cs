using UnityEngine;

public class AdMobUI : MonoBehaviour
{
	#region Variables

	private AdMobManager _adMobManager;
	private bool _lastAdClicked = false;
	private bool _lastAdCompleted = false;
	private AdMobManager.Error _lastError = AdMobManager.Error.None;

	#endregion

	#region Lifecycle

	private void Start()
	{
		_adMobManager = new AdMobManager();
		_adMobManager.AdCompletedWithSuccessEvent += OnAdCompletedWithSucces;
		_adMobManager.AdCompletedWithErrorEvent += OnAdCompletedWithError;
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical("Box");
		{
			GUILayout.Label("Is ad loaded: " + _adMobManager.IsLoaded);
			if(GUILayout.Button("Load ad"))
			{
				_adMobManager.LoadAd();
			}
			if(GUILayout.Button("Play ad"))
			{
				_adMobManager.PlayAd();
			}
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical("Box");
		{
			GUILayout.Label("Last ad result:");
			GUILayout.Label("Clicked: " + _lastAdClicked);
			GUILayout.Label("Completed: " + _lastAdCompleted);
			GUILayout.Label("Error: " + _lastError);
		}
		GUILayout.EndVertical();
	}

	#endregion

	#region Private Methods

	private void OnAdCompletedWithSucces(bool clicked, bool completed)
	{
		_lastAdClicked = clicked;
		_lastAdCompleted = completed;
		_lastError = AdMobManager.Error.None;
	}

	private void OnAdCompletedWithError(AdMobManager.Error error)
	{
		_lastAdClicked = false;
		_lastAdCompleted = false;
		_lastError = error;
	}

	#endregion
}