package dowill.SleepOnBus.service;

import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.location.Criteria;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.media.AudioManager;
import android.os.Bundle;
import android.os.IBinder;
import android.os.Vibrator;
import android.util.Log;

import com.google.android.maps.GeoPoint;

import dowill.SleepOnBus.Constants;
import dowill.SleepOnBus.R;
import dowill.SleepOnBus.SoundManager;
import dowill.SleepOnBus.Activities.Main;
import dowill.SleepOnBus.Activities.WakeUp;
import dowill.SleepOnBus.Model.StopInfo;

public class DetectingService extends Service {
	private LocationManager _lmgr = null;
	private StopInfo _destination = null;
	private SoundManager _soundManager = null;
	private Location _currentLoc = null;
	private Criteria _criteria = new Criteria();
	private String _locProvider = null;
	private boolean _isRunning = false;
	private double _awareDistance = 1000; // 500 meter is the default.
	private int _awareTimespan = 10 * 1000; // 10 sec is the default.
	private SharedPreferences _settings = null;
	private static DetectingService _selfInstance = null;
	private NotificationManager _notiMgr = null;
	private Notification _enableDetectingNoti = null;
	private Vibrator _vibrator = null;
	private final LocationListener _locationListener = new LocationListener() {
		public void onLocationChanged(Location loc) {
			_currentLoc = loc;
			if (null == _currentLoc) {
				_currentLoc = new Location(_locProvider);
				_currentLoc.setLatitude(25.0310315);
				_currentLoc.setLongitude(121.5574003);
			}
			double distance = GetDistance(getGeoByLocation(_currentLoc),
					_destination);
			Log.d(Constants.TAG, "_awareDistance=" + _awareDistance);
			Log.d(Constants.TAG, "distance=" + distance);
			if (distance < _awareDistance) {
				boolean needShock = _settings.getBoolean(
						Constants.SETTING_ENABLE_SHOCK, false);

				if (needShock) {
					_vibrator = (Vibrator) getApplication().getSystemService(
							Service.VIBRATOR_SERVICE);
					_vibrator.vibrate(new long[] { 1000, 10000 }, 0);
				}

				SoundManager.SoundCompleteListener listener = new SoundManager.SoundCompleteListener() {
					@Override
					public void OnComplete() {
						if (null != _vibrator)
							_vibrator.cancel();
					}
				};
				String soundFileName = _settings.getString(
						Constants.SETTING_AUDIO_SELECT_USER, null);
				boolean useSysBuiltInSound = (null == soundFileName || soundFileName
						.equals(""));

				boolean enableAutoVol = _settings.getBoolean(
						Constants.SETTING_ENABLE_AUTO_VOL, true);

				if (useSysBuiltInSound) {
					int choosedSysBuiltInSoundIdx = _settings.getInt(
							Constants.SETTING_AUDIO_SELECT_SYS, 1);
					if (Constants.SYSTEM_SOUND_RES_ID[choosedSysBuiltInSoundIdx] != -1) {
						if (enableAutoVol) {
							AudioManager am = (AudioManager) getSystemService(Context.AUDIO_SERVICE);
							if (null != am)
								am.setStreamVolume(
										AudioManager.STREAM_MUSIC,
										am.getStreamMaxVolume(AudioManager.STREAM_MUSIC),
										AudioManager.FLAG_SHOW_UI);
						}
						_soundManager
								.PlaySound(
										Constants.SYSTEM_SOUND_RES_ID[choosedSysBuiltInSoundIdx],
										10, listener);
					}
				} else {
					if (enableAutoVol) {
						AudioManager am = (AudioManager) getSystemService(Context.AUDIO_SERVICE);
						if (null != am)
							am.setStreamVolume(
									AudioManager.STREAM_MUSIC,
									am.getStreamMaxVolume(AudioManager.STREAM_MUSIC),
									AudioManager.FLAG_SHOW_UI);
					}
					_soundManager.PlaySound(soundFileName, 10, listener);
				}

				DetectingService.this.stopSelf();
				Intent intent = new Intent(DetectingService.this, WakeUp.class);
				intent.addFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
				DetectingService.this.startActivity(intent);
			} else {
				_awareDistance = _settings.getInt(
						Constants.SETTING_AWARE_DISTANCE, 1000);
				double times = distance / _awareDistance;
				if (times > 100) {
					_awareTimespan = 10 * 60 * 1000;
				} else if (times > 30) {
					_awareTimespan = 60 * 1000;
				} else if (times > 5) {
					_awareTimespan = 10 * 1000;
				} else {
					_awareTimespan = 1000;
				}
				resetLocationListener();
			}
		}

		public void onProviderDisabled(String provider) {
		}

		public void onProviderEnabled(String provider) {
		}

		public void onStatusChanged(String provider, int status, Bundle extras) {
		}
	};

	public static DetectingService getInstance() {
		return _selfInstance;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see android.app.Service#onDestroy()
	 */
	@Override
	public void onDestroy() {
		Log.d(Constants.TAG, "service onDestroy()");
		_lmgr.removeUpdates(_locationListener);
		_isRunning = false;

		if (null == _notiMgr)
			_notiMgr = (NotificationManager) getSystemService(NOTIFICATION_SERVICE);
		_notiMgr.cancelAll();

		super.onDestroy();
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see android.app.Service#onStart(android.content.Intent, int)
	 */
	@Override
	public void onStart(Intent intent, int startId) {
		Log.d(Constants.TAG, "service onstart()");
		getResources().getStringArray(R.array.soundList);
		_settings = getSharedPreferences(Constants.TAG, 0);
		_awareDistance = _settings.getInt(Constants.SETTING_AWARE_DISTANCE,
				1000);
		Log.d(Constants.TAG, "_awareDistance=" + _awareDistance);
		_destination = new StopInfo(intent.getIntExtra("x", 0),
				intent.getIntExtra("y", 0), intent.getStringExtra("stopName"),
				intent.getIntExtra("stopID", 0));
		_lmgr = (LocationManager) getSystemService(Context.LOCATION_SERVICE);
		_locProvider = _lmgr.getBestProvider(_criteria, true);
		Log.d(Constants.TAG, "_locProvider=" + _locProvider);
		_currentLoc = _lmgr.getLastKnownLocation(_locProvider);
		resetLocationListener();
		_criteria.setAccuracy(Criteria.ACCURACY_FINE);
		_criteria.setAltitudeRequired(false);
		_criteria.setBearingRequired(false);
		_criteria.setCostAllowed(true);
		_criteria.setPowerRequirement(Criteria.POWER_LOW);
		_soundManager = new SoundManager(this);
		_isRunning = true;
		_selfInstance = this;
		if (null == _notiMgr)
			_notiMgr = (NotificationManager) getSystemService(NOTIFICATION_SERVICE);
		if (null == _enableDetectingNoti) {
			_enableDetectingNoti = new Notification(
					R.drawable.bus_notification, "", System.currentTimeMillis());
			PendingIntent t = PendingIntent.getActivity(this, 0, new Intent(
					this, Main.class), PendingIntent.FLAG_UPDATE_CURRENT);
			_enableDetectingNoti.setLatestEventInfo(this,
					this.getText(R.string.detecting), "", t);
		}
		_notiMgr.notify(0, _enableDetectingNoti);

		super.onStart(intent, startId);
	}

	private void resetLocationListener() {
		_lmgr.requestLocationUpdates(_locProvider, _awareTimespan, 10,
				_locationListener);
	}

	private static GeoPoint getGeoByLocation(Location location) {
		GeoPoint gp = null;
		try {
			if (location != null) {
				double geoLatitude = location.getLatitude() * 1E6;
				double geoLongitude = location.getLongitude() * 1E6;
				gp = new GeoPoint((int) geoLatitude, (int) geoLongitude);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		return gp;
	}

	private static double GetDistance(GeoPoint gp1, GeoPoint gp2) {
		double Lat1r = ConvertDegreeToRadians(gp1.getLatitudeE6() / 1E6);
		double Lat2r = ConvertDegreeToRadians(gp2.getLatitudeE6() / 1E6);
		double Long1r = ConvertDegreeToRadians(gp1.getLongitudeE6() / 1E6);
		double Long2r = ConvertDegreeToRadians(gp2.getLongitudeE6() / 1E6);
		/* ¦a²y¥b®|(KM) */
		double R = 6371;
		double d = Math
				.acos(Math.sin(Lat1r) * Math.sin(Lat2r) + Math.cos(Lat1r)
						* Math.cos(Lat2r) * Math.cos(Long2r - Long1r))
				* R;
		return d * 1000;
	}

	private static double ConvertDegreeToRadians(double degrees) {
		return (Math.PI / 180) * degrees;
	}

	@Override
	public IBinder onBind(Intent arg0) {
		// TODO Auto-generated method stub
		return null;
	}

	public boolean getIsRunning() {
		return _isRunning;
	}

	public StopInfo getDestination() {
		return _destination;
	}

	public void StopSound() {
		_soundManager.StopSound();
		if (null != _vibrator)
			_vibrator.cancel();
	}
}
