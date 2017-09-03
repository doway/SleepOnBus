package dowill.SleepOnBus.Activities;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.net.Uri;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.maps.MapActivity;
import com.google.android.maps.MapView;

import dowill.SleepOnBus.BusDAO;
import dowill.SleepOnBus.CommonMenuProvider;
import dowill.SleepOnBus.Constants;
import dowill.SleepOnBus.MapManager;
import dowill.SleepOnBus.R;
import dowill.SleepOnBus.SoundManager;
import dowill.SleepOnBus.Model.StopInfo;
import dowill.SleepOnBus.backend.BusWebservice;
import dowill.SleepOnBus.service.DetectingService;

/**
 * @author Tom Tang
 * 
 */
public class DetectingMap extends MapActivity {
	private Button _btnStopDetect = null;
	private Button _btnAddToPersonal = null;
	private Button _btnRatingBad = null;
	private TextView _txtLayoutTitle = null;
	private View _stopReportLayout = null;
	private MapView _mv = null;
	private StopInfo _destination = null;
	private MapManager _mpmgr = null;
	private SoundManager _soundManager = null;
	private SharedPreferences _settings = null;
	public static Activity _parentActivity = null;

	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.detecting);
		_txtLayoutTitle = (TextView) findViewById(R.id.txtLayoutTitle);
		_btnStopDetect = (Button) findViewById(R.id.btnStopDetect);
		_btnAddToPersonal = (Button) findViewById(R.id.btnAddToPersonal);
		_btnRatingBad = (Button) findViewById(R.id.btnRatingBad);
		_mv = (MapView) findViewById(R.id.map);
		_mpmgr = new MapManager(this, _mv, null);
		_stopReportLayout = findViewById(R.id.stopReportLayout);
		_soundManager = new SoundManager(this);
		String lineName = null;

		if (null != DetectingService.getInstance()
				&& DetectingService.getInstance().getIsRunning()) {
			_destination = DetectingService.getInstance().getDestination();
			_txtLayoutTitle.setVisibility(View.VISIBLE);
			_stopReportLayout.setVisibility(View.GONE);
			_mpmgr.enableCurrentLocationRefresh();
		} else {
			_destination = new StopInfo(getIntent().getIntExtra("DbID", 0),
					getIntent().getIntExtra("x", 0), getIntent().getIntExtra(
							"y", 0), getIntent().getStringExtra("stopName"),
					getIntent().getIntExtra("stopID", 0));

			TextView lblLine = (TextView) findViewById(R.id.lblLine);
			TextView lblStopName = (TextView) findViewById(R.id.lblStopName);
			lineName = getIntent().getStringExtra("lineName");
			String stopName = getIntent().getStringExtra("stopName");
			lblLine.setText(getString(R.string.ratingLine) + lineName);
			lblStopName.setText(getString(R.string.ratingStop) + stopName);
			if (_destination.getDbID() != 0) {
				_btnAddToPersonal.setVisibility(View.GONE);
				_btnRatingBad.setVisibility(View.GONE);
			}
		}

		_mpmgr.refreshMapView(_destination, R.drawable.bus, 0, lineName, null,
				false);
		if (null == _settings)
			_settings = getSharedPreferences(Constants.TAG, 0);
	}

	@Override
	protected boolean isRouteDisplayed() {
		// TODO Auto-generated method stub
		return false;
	}

	public void btnStopDetectOnClick(View v) {
		_btnStopDetect.setVisibility(Button.GONE);
		_txtLayoutTitle.setText(R.string.detectingDisableTitle);
		Intent i = new Intent(this, DetectingService.class);
		stopService(i);
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

	public void btnBackOnClick(View v) {
		finish();
	}

	public void btnRatingBadOnClick(View v) {
		try {
			BusWebservice.rateStop(_destination.getStopID(), false);
		} catch (Exception e) {
			e.printStackTrace();
		}
		Toast.makeText(this, R.string.ifLocationWrong, Toast.LENGTH_LONG)
				.show();
		finish();
	}

	public void btnRatingGoodOnClick(View v) {
		if (_destination.getDbID() == 0)
			try {
				BusWebservice.rateStop(_destination.getStopID(), true);
			} catch (Exception e) {
				e.printStackTrace();
			}
		Intent intent = new Intent(this, DetectingService.class);
		intent.putExtra("stopID", _destination.getStopID());
		intent.putExtra("stopName", _destination.getStopName());
		intent.putExtra("x", _destination.getLatitudeE6());
		intent.putExtra("y", _destination.getLongitudeE6());
		intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
		startService(intent);

		_mpmgr.enableCurrentLocationRefresh(); // issue #8

		_stopReportLayout.setVisibility(View.GONE);
		_txtLayoutTitle.setVisibility(View.VISIBLE);

		if (null != _parentActivity)
			_parentActivity.finish();
		if (!_settings
				.getBoolean(Constants.SETTING_DETECT_TIP_NO_REMIND, false))
			promptTip();
	}

	public void imgDowillLogoOnClick(View v) {
		Uri uri = Uri.parse(getString(R.string.dowillUrl));
		Intent intent = new Intent(Intent.ACTION_VIEW, uri);
		startActivity(intent);
	}

	public void btnZoomInOnClick(View v) {
		_mpmgr.zoomIn();
	}

	public void btnZoomOutOnClick(View v) {
		_mpmgr.zoomOut();
	}

	public void btnSetupNowOnClick(View v) {
		Intent intent = new Intent();
		intent.setClass(this, Settings.class);
		startActivity(intent);
	}

	public void btnAddToPersonalOnClick(View v) {
		BusDAO dao = new BusDAO(this);
		dao.open();
		dao.addNewStop(_destination.getStopID(), _destination.getStopName(),
				_destination.getLatitudeE6(), _destination.getLongitudeE6());
		dao.close();
		_btnAddToPersonal.setEnabled(false);
		Toast.makeText(this, R.string.doneInAddingFavorite, Toast.LENGTH_SHORT)
				.show();
	}

	private void promptTip() {
		LayoutInflater inflater = LayoutInflater.from(this);
		final View textEntryView = inflater.inflate(R.layout.detect_tip, null);
		new AlertDialog.Builder(this)
				.setTitle(R.string.coverTitle)
				.setView(textEntryView)
				.setPositiveButton(R.string.ok,
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog,
									int which) {
								CheckBox chkNoRemind = (CheckBox) textEntryView
										.findViewById(R.id.chkNoRemind);
								_settings
										.edit()
										.putBoolean(
												Constants.SETTING_DETECT_TIP_NO_REMIND,
												chkNoRemind.isChecked())
										.commit();
								dialog.dismiss();
							}
						}).show();
	}
}
