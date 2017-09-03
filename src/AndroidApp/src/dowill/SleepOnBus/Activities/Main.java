package dowill.SleepOnBus.Activities;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.ProgressDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.res.Configuration;
import android.content.res.Resources;
import android.location.Criteria;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.net.Uri;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.AdapterView;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.SpinnerAdapter;
import android.widget.TextView;
import android.widget.Toast;
import dowill.SleepOnBus.BusDAO;
import dowill.SleepOnBus.CommonMenuProvider;
import dowill.SleepOnBus.Constants;
import dowill.SleepOnBus.DataAdapterFactory;
import dowill.SleepOnBus.R;
import dowill.SleepOnBus.SoundManager;
import dowill.SleepOnBus.Model.LineInfo;
import dowill.SleepOnBus.Model.StopInfo;
import dowill.SleepOnBus.backend.BusWebservice;
import dowill.SleepOnBus.service.DetectingService;

public class Main extends Activity {

	private static final int UPDATELINESPIN = 1;
	private static final int UPDATESTOPSPIN = 2;
	private static final int INIT = 0;
	private static final int CREATE_NEW_ONE = 1;
	private static final int MY_PERSONAL = 2;
	private Spinner _spLine = null;
	private Spinner _spDestination = null;
	private TextView _tvMode = null;
	private Button _btnBeginDetect = null;
	private Button _btnCreateOne = null;
	private String _lineName = null;
	private TextView _tvDestination = null;
	private Location _currentLoc = null;
	private LocationManager _lmgr = null;
	private SoundManager _soundManager = null;
	private static double _delta = GetDeltaDegree(0.5);
	private ProgressDialog _progDial = null;
	private SharedPreferences _settings = null;
	private boolean _isEditingMode = false;
	private MenuItem _editingModeMenuItem = null;
	private final Handler _messageHandler = new Handler() {
		public void handleMessage(Message msg) {
			switch (msg.what) {
			case Main.UPDATELINESPIN:
				_spLine.setAdapter((SpinnerAdapter) msg.obj);
				break;
			case Main.UPDATESTOPSPIN:
				_spDestination.setAdapter((SpinnerAdapter) msg.obj);
				break;
			}
			if (null != _progDial)
				try {
					_progDial.dismiss();
					_progDial = null;
				} catch (Exception e) {
					e.printStackTrace();
				}
			super.handleMessage(msg);
		}
	};
	private final LocationListener _locationListener = new LocationListener() {
		public void onLocationChanged(Location loc) {
			if (null != loc) {
				_currentLoc = loc;
				_lmgr.removeUpdates(_locationListener);
				initSpLineControl();
			}
		}

		public void onProviderDisabled(String provider) {
		}

		public void onProviderEnabled(String provider) {
		}

		public void onStatusChanged(String provider, int status, Bundle extras) {
		}
	};

	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.main);
		try {
			if (checkVersion()) {
				if (null != DetectingService.getInstance()
						&& DetectingService.getInstance().getIsRunning()) {
					Intent intent = new Intent();
					intent.setClass(Main.this, DetectingMap.class);
					startActivity(intent);
					finish();
				} else {
					initialLocaleInfo();
					promptCover();
					initCurrentLocation();
					initControls();
					_soundManager = new SoundManager(this);
				}
			} else {
				Toast.makeText(this, R.string.newVersion, Toast.LENGTH_LONG)
						.show();
				finish();
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	private void initialLocaleInfo() {
		Resources res = getResources();
		Configuration conf = res.getConfiguration();
		BusWebservice.setLocale(conf.locale);
	}

	private void promptCover() {
		LayoutInflater inflater = LayoutInflater.from(this);
		final View textEntryView = inflater.inflate(R.layout.cover, null);
		new AlertDialog.Builder(this)
				.setTitle(R.string.coverTitle)
				.setView(textEntryView)
				.setPositiveButton(R.string.ok,
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog,
									int which) {
								dialog.dismiss();
							}
						}).show();
	}

	private boolean checkVersion() throws Exception {
		String currentVer = getPackageManager().getPackageInfo(
				getPackageName(), 0).versionName;
		Log.d(Constants.TAG, "current version=" + currentVer);
		return currentVer.equals(BusWebservice.getNewVersion());
	}

	private void destinationControlVisible(int visibility) {
		_tvDestination.setVisibility(visibility);
		_spDestination.setVisibility(visibility);
		if (View.INVISIBLE == visibility) {
			_btnBeginDetect.setVisibility(visibility);
			_btnCreateOne.setVisibility(View.VISIBLE);
		}
	}

	private void initCurrentLocation() {
		_lmgr = (LocationManager) getSystemService(Context.LOCATION_SERVICE);
		Criteria criteria = new Criteria();
		criteria.setAccuracy(Criteria.ACCURACY_FINE);
		criteria.setAltitudeRequired(false);
		criteria.setBearingRequired(false);
		criteria.setCostAllowed(true);
		criteria.setPowerRequirement(Criteria.POWER_LOW);
		String locProvider = _lmgr.getBestProvider(criteria, true);
		Log.d(Constants.TAG, "locProvider=" + locProvider);
		_lmgr.requestLocationUpdates(locProvider, 0, 10, _locationListener);
		_currentLoc = _lmgr.getLastKnownLocation(locProvider);
		if (null == _currentLoc) {
			_currentLoc = new Location(locProvider);
			_currentLoc.setLatitude(25.0310315);
			_currentLoc.setLongitude(121.5574003);
		}
		Log.d(Constants.TAG, "1_currentLoc == null is " + (_currentLoc == null));
	}

	private void initSpLineControl() {
		if (null == _progDial)
			try {
				_progDial = ProgressDialog.show(Main.this,
						Main.this.getText(R.string.waiting),
						Main.this.getText(R.string.loading), true);
			} catch (Exception e) {
				e.printStackTrace();
			}
		new Thread() {
			public void run() {
				try {
					Message msg = new Message();
					msg.what = Main.UPDATELINESPIN;
					msg.obj = DataAdapterFactory.GetLineListAdapter(
							_currentLoc.getLongitude(),
							_currentLoc.getLatitude(), _delta, Main.this);
					_messageHandler.sendMessage(msg);
				} catch (Exception e) {
					e.printStackTrace();
				}
				if (null != _progDial)
					try {
						_progDial.dismiss();
						_progDial = null;
					} catch (Exception e) {
						e.printStackTrace();
					}
			}
		}.start();
	}

	private void initControls() throws Exception {
		_tvDestination = (TextView) findViewById(R.id.lblDestination);
		_tvMode = (TextView) findViewById(R.id.tvMode);
		_btnBeginDetect = (Button) findViewById(R.id.btnBeginDetect);
		_btnCreateOne = (Button) findViewById(R.id.btnCreateOne);
		_spLine = (Spinner) findViewById(R.id.spLine);
		_spDestination = (Spinner) findViewById(R.id.spDestination);
		Log.d(Constants.TAG, "2_currentLoc == null is " + (_currentLoc == null));

		if (null == _settings)
			_settings = getSharedPreferences(Constants.TAG, 0);
		int deltaMeter = _settings.getInt(Constants.SETTING_AWARE_LINE, 10000);
		double delta = (double) deltaMeter / 1000.0;
		Log.d(Constants.TAG, "delta=" + delta);
		_delta = GetDeltaDegree(delta);

		if (null != _currentLoc) {
			initSpLineControl();
		} else {
			Toast.makeText(Main.this, R.string.currentLocNotAvailable,
					Toast.LENGTH_LONG).show();
			if (null == _progDial)
				try {
					_progDial = ProgressDialog.show(Main.this,
							Main.this.getText(R.string.waiting),
							Main.this.getText(R.string.loading), true);
				} catch (Exception e) {
					e.printStackTrace();
				}
			new Thread() {
				public void run() {
					try {
						Message msg = new Message();
						msg.what = Main.UPDATELINESPIN;
						msg.obj = DataAdapterFactory.GetLineListAdapter(
								_currentLoc.getLongitude(),
								_currentLoc.getLatitude(), _delta, Main.this);
						_messageHandler.sendMessage(msg);
					} catch (Exception e) {
						e.printStackTrace();
					}
				}
			}.start();
		}
		_spLine.setOnItemSelectedListener(new Spinner.OnItemSelectedListener() {
			public void onItemSelected(AdapterView<?> arg0, View arg1,
					int arg2, long arg3) {
				onSpLineItemSelected(arg0, arg1, arg2, arg3);
			}

			public void onNothingSelected(AdapterView<?> arg0) {
				// TODO Auto-generated method stub
			}
		});

		_spDestination
				.setOnItemSelectedListener(new Spinner.OnItemSelectedListener() {
					public void onItemSelected(AdapterView<?> arg0, View arg1,
							int arg2, long arg3) {
						onSpDestinationItemSelected(arg0, arg1, arg2, arg3);
					}

					public void onNothingSelected(AdapterView<?> arg0) {
						// TODO Auto-generated method stub
					}
				});
	}

	private void onSpLineItemSelected(AdapterView<?> arg0, View arg1, int arg2,
			long arg3) {
		try {
			switch (_spLine.getSelectedItemPosition()) {
			case INIT:
				destinationControlVisible(View.INVISIBLE);
				break;
			case CREATE_NEW_ONE:
				btnCreateOneOnClick(_btnCreateOne);
				break;
			case MY_PERSONAL:
				destinationControlVisible(View.VISIBLE);
				_spDestination.setAdapter((SpinnerAdapter) DataAdapterFactory
						.GetPersonalStops(Main.this));
				break;
			default:
				destinationControlVisible(View.VISIBLE);
				if (null == _progDial)
					_progDial = ProgressDialog.show(Main.this,
							Main.this.getText(R.string.waiting),
							Main.this.getText(R.string.loading), true);
				new Thread() {
					public void run() {
						try {
							Message msg = new Message();
							msg.what = Main.UPDATESTOPSPIN;
							msg.obj = DataAdapterFactory.GetStopAdapter(
									((LineInfo) _spLine.getSelectedItem())
											.getLineID(), Main.this);
							_messageHandler.sendMessage(msg);
						} catch (Exception e) {
							e.printStackTrace();
						}
						if (null != _progDial)
							try {
								_progDial.dismiss();
								_progDial = null;
							} catch (Exception e) {
								e.printStackTrace();
							}
					}
				}.start();
				break;
			}
		} catch (Exception ex) {
			ex.printStackTrace();
		}
	}

	private void onSpDestinationItemSelected(AdapterView<?> arg0, View arg1,
			int arg2, long arg3) {
		try {
			switch (_spDestination.getSelectedItemPosition()) {
			case INIT:
				_btnBeginDetect.setVisibility(View.INVISIBLE);
				_btnCreateOne.setVisibility(View.VISIBLE);
				break;
			case CREATE_NEW_ONE:
				btnCreateOneOnClick(_btnCreateOne);
				break;
			default:
				if (_isEditingMode) {
					new AlertDialog.Builder(Main.this)
							.setTitle(R.string.coverTitle)
							.setMessage(R.string.removeStop)
							.setPositiveButton(R.string.ok,
									new DialogInterface.OnClickListener() {
										public void onClick(
												DialogInterface dialog,
												int which) {
											onStopDeletingAlertDialogPostiveClick(
													dialog, which);
										}
									}).setNegativeButton(R.string.cancel, null)
							.show();

				} else {
					_btnBeginDetect.setVisibility(View.VISIBLE);
					_btnCreateOne.setVisibility(View.INVISIBLE);
				}
				break;
			}
		} catch (Exception ex) {
			ex.printStackTrace();
		}
	}

	private void onStopDeletingAlertDialogPostiveClick(DialogInterface dialog,
			int which) {
		if (!isPersonalLine()) {
			if (BusWebservice.deleteStop(((StopInfo) _spDestination
					.getSelectedItem()).getStopID())) {
				Toast.makeText(Main.this, R.string.stopRemoved,
						Toast.LENGTH_SHORT).show();
				initSpLineControl();
			} else {
				Toast.makeText(Main.this, R.string.errorNotRemove,
						Toast.LENGTH_LONG).show();
				return; // Exit function
			}
		} else {
			BusDAO dao = new BusDAO(Main.this);
			dao.open();
			dao.deleteStop(((StopInfo) _spDestination.getSelectedItem())
					.getDbID());
			dao.close();
		}
		_isEditingMode = !_isEditingMode;
		Log.d(Constants.TAG, (_isEditingMode) ? "Entering editing mode"
				: "Exiting editing mode");
		_editingModeMenuItem
				.setTitle((_isEditingMode) ? R.string.exitEditPersonal
						: R.string.editPersonal);
		_tvMode.setVisibility((_isEditingMode) ? View.VISIBLE : View.GONE);

		Toast.makeText(Main.this, R.string.stopRemoved, Toast.LENGTH_SHORT)
				.show();
		initSpLineControl();
	}

	public void btnCreateOneOnClick(View v) {
		// See if user had ever agreed to publish their setup
		if (_spLine.getSelectedItemPosition() != MY_PERSONAL
				&& !_settings
						.getBoolean(Constants.SETTING_PUBLISH_AGREE, false)) {
			// Confirm user's agreement
			LayoutInflater inflater = LayoutInflater.from(this);
			final View textEntryView = inflater.inflate(R.layout.public_tip,
					null);
			new AlertDialog.Builder(this)
					.setTitle(R.string.coverTitle)
					.setView(textEntryView)
					.setPositiveButton(R.string.agree,
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int which) {
									_settings
											.edit()
											.putBoolean(
													Constants.SETTING_PUBLISH_AGREE,
													true).commit();
									btnCreateOneOnClick(null);
									dialog.dismiss();
								}
							}).setNegativeButton(R.string.disagree, null)
					.show();
			return;
		}

		Log.d(Constants.TAG,
				"getSelectedItemPosition = "
						+ _spLine.getSelectedItemPosition());
		Log.d(Constants.TAG,
				"getSelectedItemId = " + _spLine.getSelectedItemId());
		_lineName = null;
		int lineId = 0;
		switch (_spLine.getSelectedItemPosition()) {
		case INIT:
		case CREATE_NEW_ONE:
			LayoutInflater inflater = LayoutInflater.from(this);
			final View textEntryView = inflater
					.inflate(R.layout.new_line, null);
			new AlertDialog.Builder(this)
					.setTitle(R.string.txtNewLine)
					.setView(textEntryView)
					.setPositiveButton(R.string.next,
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int which) {
									EditText ed = (EditText) textEntryView
											.findViewById(R.id.edNewLine);
									_lineName = ed.getText().toString().trim();
									Log.d(Constants.TAG,
											"btnCreateOne:_lineName="
													+ _lineName);
									if (null != _lineName
											&& !_lineName.equals("")) {
										int lineId = 0;
										Intent intent = new Intent();
										intent.setClass(Main.this,
												StopDefine.class);
										intent.putExtra("lineName", _lineName);
										intent.putExtra("lineId", lineId);
										startActivity(intent);
									}
								}
							})
					.setNegativeButton(R.string.cancel,
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int whichButton) {
									_spLine.setSelection(0);
									dialog.dismiss();
								}
							}).show();
			break;
		default:
			LineInfo info = (LineInfo) _spLine.getSelectedItem();
			lineId = info.getLineID();
			_lineName = info.getLineName();
			break;
		}

		if (null != _lineName && !_lineName.trim().equals("")) {
			Intent intent = new Intent();
			intent.setClass(Main.this, StopDefine.class);
			intent.putExtra("lineName", _lineName);
			intent.putExtra("lineId", lineId);
			intent.putExtra("isPersonal", isPersonalLine());
			startActivity(intent);
		}
	}

	public void imgDowillLogoOnClick(View v) {
		Uri uri = Uri.parse(getString(R.string.dowillUrl));
		Intent intent = new Intent(Intent.ACTION_VIEW, uri);
		startActivity(intent);
	}

	public void btnBeginDetectOnClick(View view) {
		Intent intent = new Intent();
		intent.setClass(Main.this, DetectingMap.class);
		DetectingMap._parentActivity = this;
		_lineName = _spLine.getSelectedItem().toString();
		_spDestination.getSelectedItem().toString();
		StopInfo si = (StopInfo) _spDestination.getSelectedItem();
		intent.putExtra("DbID", si.getDbID());
		intent.putExtra("stopID", si.getStopID());
		intent.putExtra("stopName", si.getStopName());
		intent.putExtra("lineName", _lineName);
		intent.putExtra("x", si.getLatitudeE6());
		intent.putExtra("y", si.getLongitudeE6());
		startActivity(intent);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		CommonMenuProvider.onCreateOptionsMenu(menu, this);
		menu.add(Menu.NONE, 3, 0, R.string.editPersonal);
		_editingModeMenuItem = menu.getItem(0).setIcon(
				android.R.drawable.ic_delete);
		return super.onCreateOptionsMenu(menu);
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		if (_editingModeMenuItem == item) {
			_isEditingMode = !_isEditingMode;
			Log.d(Constants.TAG, (_isEditingMode) ? "Entering editing mode"
					: "Exiting editing mode");
			item.setTitle((_isEditingMode) ? R.string.exitEditPersonal
					: R.string.editPersonal);
			_tvMode.setVisibility((_isEditingMode) ? View.VISIBLE : View.GONE);
			Toast.makeText(
					Main.this,
					(_isEditingMode) ? R.string.enterEditingMode
							: R.string.exitEditingMode, Toast.LENGTH_SHORT)
					.show();
		} else {
			CommonMenuProvider.onOptionsItemSelected(item, this, _soundManager);
		}
		return super.onOptionsItemSelected(item);
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see com.google.android.maps.MapActivity#onResume()
	 */
	@Override
	protected void onResume() {
		Log.d(Constants.TAG, "onResume()");
		try {
			initControls();
		} catch (Exception e) {
			e.printStackTrace();
		}
		super.onResume();
	}

	private static double GetDeltaDegree(double y) {
		final double R = 6371;
		return y * 180 / (R * Math.PI);
	}

	private boolean isPersonalLine() {
		return (_spLine.getSelectedItemPosition() == MY_PERSONAL);
	}
}