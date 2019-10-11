using GoogleMobileAds.Api;
using System;

public class AdMobManager
{
	#region Consts

	private const string PlayStoreAppId = "#APPID#";
	private const string PlayStoreAdId = "#ADID#";

	#endregion

	#region Variables

	private AdRequest _adRequest;
	private RewardedAd _rewardedAd;

	private bool _clickedAd;
	private bool _completedAd;

	#endregion

	#region Properties

	public bool IsLoaded => _rewardedAd != null && _rewardedAd.IsLoaded();

	#endregion

	#region Events

	public delegate void AdCompletedWithSuccessHandler(bool clicked, bool completed);
	public event AdCompletedWithSuccessHandler AdCompletedWithSuccessEvent;
	private void FireAdCompletedWithSuccesEvent(bool clicked, bool completed)
	{
		AdCompletedWithSuccessEvent?.Invoke(clicked, completed);
	}

	public delegate void AdCompletedWithErrorHandler(Error error);
	public event AdCompletedWithErrorHandler AdCompletedWithErrorEvent;
	private void FireAdCompletedWithErrorEvent(Error error)
	{
		AdCompletedWithErrorEvent?.Invoke(error);
	}


	#endregion

	public AdMobManager()
	{
		MobileAds.Initialize(PlayStoreAppId);
		_adRequest = new AdRequest.Builder().Build();
		RewardBasedVideoAd.Instance.OnAdLeavingApplication += OnAdLeavingApplication;
	}

	#region Public Methods

	public void LoadAd()
	{
		if(_rewardedAd == null)
		{
			_rewardedAd = new RewardedAd(PlayStoreAdId);
			_rewardedAd.LoadAd(_adRequest);
			_rewardedAd.OnAdClosed += OnAdClosed;
			_rewardedAd.OnAdFailedToLoad += OnAdFailedToLoad;
			_rewardedAd.OnAdFailedToShow += OnAdFailedToShow;
			_rewardedAd.OnUserEarnedReward += OnUserEarnedReward;
		}
	}

	public void PlayAd()
	{
		if(IsLoaded)
		{
			_clickedAd = false;
			_completedAd = false;
			_rewardedAd.Show();
		}
	}

	#endregion

	#region Private Methods

	private void TryDestroyAd(object sender)
	{
		if(sender is RewardedAd rewardedAd)
		{
			rewardedAd.OnAdClosed -= OnAdClosed;
			rewardedAd.OnAdFailedToLoad -= OnAdFailedToLoad;
			rewardedAd.OnAdFailedToShow -= OnAdFailedToShow;
			rewardedAd.OnUserEarnedReward -= OnUserEarnedReward;

			if(rewardedAd == _rewardedAd)
			{
				_rewardedAd = null;
			}
		}
	}

	private void OnAdLeavingApplication(object sender, EventArgs e)
	{
		_clickedAd = true;
	}

	private void OnUserEarnedReward(object sender, Reward e)
	{
		_completedAd = true;
	}

	private void OnAdFailedToShow(object sender, AdErrorEventArgs e)
	{
		TryDestroyAd(sender);
		FireAdCompletedWithErrorEvent(Error.FailedToShow);
	}

	private void OnAdFailedToLoad(object sender, AdErrorEventArgs e)
	{
		TryDestroyAd(sender);
		FireAdCompletedWithErrorEvent(Error.FailedToLoad);
	}

	private void OnAdClosed(object sender, EventArgs e)
	{
		TryDestroyAd(sender);
		FireAdCompletedWithSuccesEvent(_clickedAd, _completedAd);
	}

	#endregion

	#region Nested Classes

	public enum Error
	{
		None,
		FailedToShow,
		FailedToLoad
	}

	#endregion
}