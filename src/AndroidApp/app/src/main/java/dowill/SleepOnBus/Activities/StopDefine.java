package dowill.SleepOnBus.Activities;

import java.io.IOException;
import java.util.List;
import java.util.Locale;

import android.content.Intent;
import android.location.Address;
import android.location.Geocoder;
import android.location.Location;
import android.net.Uri;
import android.os.Bundle;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.MapActivity;
import com.google.android.maps.MapView;
import com.google.android.maps.Projection;

import dowill.SleepOnBus.CommonMenuProvider;
import dowill.SleepOnBus.Constants;
import dowill.SleepOnBus.MapManager;
import dowill.SleepOnBus.R;
import dowill.SleepOnBus.SoundManager;
import dowill.SleepOnBus.StopMarkOverlay;

/**
 * @author Tom Tang
 * 
 */
public class StopDefine extends MapActivity {
	private MapView _mv = null;
	private TextView _lblChosenLine = null;
	private TextView _txtCurrentLocation = null;
	private TextView _txtTargetLocation = null;
	private EditText _txtAddress = null;
	private MapManager _mpmgr = null;
	private int _lineId = 0;
	private boolean _isPersonalLine = false;
	private String _lineName = null;
	private SoundManager _soundManager = null;
	private StopMarkOverlay _stopOverlay = null;

	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.stop_define);

		_soundManager = new SoundManager(this);
		_txtCurrentLocation = (TextView) findViewById(R.id.txtCurrentLocation);
		_txtTargetLocation = (TextView) findViewById(R.id.txtTargetLocation);
		_lblChosenLine = (TextView) findViewById(R.id.txtChosenLine);
		_lineName = getIntent().getStringExtra("lineName");
		_lineId = getIntent().getIntExtra("lineId", 0);
		_isPersonalLine = getIntent().getBooleanExtra("isPersonal", false);
		String lineNameStatement = getResources()
				.getString(R.string.chosenLine);
		_lblChosenLine.setText(lineNameStatement + _lineName);
		_txtAddress = (EditText) findViewById(R.id.edSearch);

		_mv = (MapView) findViewById(R.id.map);
		_mpmgr = new MapManager(this, _mv);
		Location loc = _mpmgr.getCurrentLocation();
		if (null != loc) {
			_txtCurrentLocation.setText(getResources().getText(
					R.string.currentLocation).toString()
					+ loc.getLatitude() + ", " + loc.getLongitude());
		}

		_mv.setOnTouchListener(new View.OnTouchListener() {
			public boolean onTouch(View v, MotionEvent event) {
				Log.d(Constants.TAG, "_mv.onTouch");
				int x = (int) event.getX();
				int y = (int) event.getY();
				Projection pj = _mv.getProjection();
				GeoPoint gp = pj.fromPixels(x, y);
				if (null != gp) {
					_txtTargetLocation.setText(getText(R.string.targetLocation)
							.toString()
							+ (gp.getLatitudeE6() / 1E6)
							+ ", "
							+ (gp.getLongitudeE6() / 1E6));
					if (null != _stopOverlay)
						_mpmgr.removeStopOverlay(_stopOverlay);
					_stopOverlay = _mpmgr.refreshMapView(gp, R.drawable.bus,
							_lineId, _lineName, _txtAddress.getText()
									.toString(), _isPersonalLine);
				}
				return false;
			}
		});
	}

	public void btnSearchOnClick(View v) {
		Log.d(Constants.TAG, "_btnSearch.onClick");
		String address = _txtAddress.getText().toString();
		Log.d(Constants.TAG, "address=" + address);

		GeoPoint gp = getGeoByAddress(address);
		if (null != gp) {
			_txtTargetLocation.setText(getText(R.string.targetLocation)
					.toString()
					+ (gp.getLatitudeE6() / 1E6)
					+ ", "
					+ (gp.getLongitudeE6() / 1E6));
			if (null != _stopOverlay)
				_mpmgr.removeStopOverlay(_stopOverlay);
			_stopOverlay = _mpmgr.refreshMapView(gp, R.drawable.bus, _lineId,
					_lineName, _txtAddress.getText().toString(),
					_isPersonalLine);
			Toast.makeText(this, R.string.confirmNewStopLoc, Toast.LENGTH_LONG)
					.show();
		} else {
			Toast.makeText(this, R.string.locationNameNotFound,
					Toast.LENGTH_LONG).show();
		}
	}

	public void edSearchOnClick(View view) {
		Log.d(Constants.TAG, "edSearchOnClick");
		if (_txtAddress
				.getText()
				.toString()
				.equals(StopDefine.this.getResources().getString(
						R.string.txtSearchTip)))
			_txtAddress.setText("");
	}

	@Override
	protected boolean isRouteDisplayed() {
		// TODO Auto-generated method stub
		return false;
	}

	private GeoPoint getGeoByAddress(String strSearchAddress) {
		GeoPoint gp = null;
		if (!strSearchAddress.equals("")) {
			Geocoder geocoder01 = new Geocoder(StopDefine.this,
					Locale.getDefault());

			List<Address> lstAddress;
			try {
				lstAddress = geocoder01
						.getFromLocationName(strSearchAddress, 1);
				if (!lstAddress.isEmpty()) {
					Address adsLocation = lstAddress.get(0);
					/* 1E6 = 1000000 */
					double geoLatitude = adsLocation.getLatitude() * 1E6;
					double geoLongitude = adsLocation.getLongitude() * 1E6;
					gp = new GeoPoint((int) geoLatitude, (int) geoLongitude);
				}
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
		return gp;
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		CommonMenuProvider.onCreateOptionsMenu(menu, this);
		return super.onCreateOptionsMenu(menu);
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		CommonMenuProvider.onOptionsItemSelected(item, this, _soundManager);
		return super.onOptionsItemSelected(item);
	}

	public void imgDowillLogoOnClick(View v) {
		Uri uri = Uri.parse(getString(R.string.dowillUrl));
		Intent intent = new Intent(Intent.ACTION_VIEW, uri);
		startActivity(intent);
	}

	public void btnOKOnClick(View v) {
		if (null != _stopOverlay)
			_stopOverlay.Setup();
		else
			Toast.makeText(this, R.string.pleaseTellusWhereTheStopIs,
					Toast.LENGTH_LONG).show();
	}

	public void btnZoomInOnClick(View v) {
		_mpmgr.zoomIn();
	}

	public void btnZoomOutOnClick(View v) {
		_mpmgr.zoomOut();
	}
}
