package dowill.SleepOnBus;

import java.util.List;

import android.content.Context;
import android.graphics.drawable.Drawable;
import android.location.Criteria;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;
import android.util.Log;
import android.widget.Toast;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapActivity;
import com.google.android.maps.MapController;
import com.google.android.maps.MapView;
import com.google.android.maps.Overlay;

public class MapManager {
	private MapActivity _host = null;
	private MapView _mv = null;
	private MapController _mp = null;
	private int _zoomLevel = 0;
	private LocationManager _lmgr = null;
	private String _locProvider = null;
	private Location _loc = null;
	private LocationListener _subLocationListener = null;
	private StopMarkOverlay _currentLoc = null;
	private final LocationListener _locationListener = new LocationListener() {
		public void onLocationChanged(Location loc) {
			if (null != _currentLoc)
				removeStopOverlay(_currentLoc);
			_loc = loc;
			_currentLoc = refreshMapView(_loc);
			if (null != _subLocationListener)
				_subLocationListener.onLocationChanged(loc);
		}

		public void onProviderDisabled(String provider) {
		}

		public void onProviderEnabled(String provider) {
		}

		public void onStatusChanged(String provider, int status, Bundle extras) {
		}
	};

	public MapManager(MapActivity host, MapView mv) {
		this(host, mv, null);
	}

	public MapManager(MapActivity host, MapView mv,
			LocationListener locationListener) {
		_mv = mv;
		_host = host;
		_mv.setBuiltInZoomControls(false);
		_mp = _mv.getController();
		_zoomLevel = 17;
		_mp.setZoom(_zoomLevel);

		_lmgr = (LocationManager) host
				.getSystemService(Context.LOCATION_SERVICE);
		getLocationProvider();
		Log.d(Constants.TAG, "_locProvider=" + _locProvider);

		if (null != _loc)
			_currentLoc = refreshMapView(_loc);
		if (null != locationListener) {
			_subLocationListener = locationListener;
			if (null != _locProvider)
				_lmgr.requestLocationUpdates(_locProvider, 2000, 10,
						_locationListener);
		}
	}

	public void enableCurrentLocationRefresh() {
		_lmgr.requestLocationUpdates(_locProvider, 2000, 10, _locationListener);
	}

	public LocationManager getLocationManager() {
		return _lmgr;
	}

	public Location getCurrentLocation() {
		return _loc;
	}

	public StopMarkOverlay refreshMapView(Location loc) {
		StopMarkOverlay rtn = null;
		if (null != loc) {
			Log.d(Constants.TAG, "refreshMapView(Latitude=" + loc.getLatitude()
					+ ", Longitude=" + loc.getLongitude() + ")");
			GeoPoint p = new GeoPoint((int) (loc.getLatitude() * 1E6),
					(int) (loc.getLongitude() * 1E6));
			rtn = refreshMapView(p, R.drawable.man, 0, null, null, false);
		}
		return rtn;
	}

	public StopMarkOverlay refreshMapView(GeoPoint gp, int drawableResId,
			int _lineId, String lineName, String locationName, boolean isPersonal) {
		StopMarkOverlay rtn = null;
		if (null != gp) {
			_mp.animateTo(gp);

			Drawable dr = _host.getResources().getDrawable(drawableResId);
			dr.setBounds(-15, -20, 15, 20);
			rtn = new StopMarkOverlay(dr, gp, _host, (_lineId != 0), _lineId,
					lineName, locationName, isPersonal);
			if (null != _loc)
				rtn.setCurrentLocation(_loc);
			List<Overlay> overlays = _mv.getOverlays();
			overlays.add(rtn);
		}
		return rtn;
	}

	public void removeStopOverlay(StopMarkOverlay so) {
		_mv.getOverlays().remove(so);
	}

	public void zoomIn() {
		_mp.zoomIn();
	}

	public void zoomOut() {
		_mp.zoomOut();
	}

	private void getLocationProvider() {
		Criteria criteria = new Criteria();
		criteria.setAccuracy(Criteria.ACCURACY_FINE);
		criteria.setAltitudeRequired(false);
		criteria.setBearingRequired(false);
		criteria.setCostAllowed(true);
		criteria.setPowerRequirement(Criteria.POWER_LOW);
		_locProvider = _lmgr.getBestProvider(criteria, true);
		if (null != _locProvider)
			_loc = _lmgr.getLastKnownLocation(_locProvider);
		else
			Toast.makeText(_host, R.string.currentLocNotAvailable,
					Toast.LENGTH_LONG).show();
	}
}
