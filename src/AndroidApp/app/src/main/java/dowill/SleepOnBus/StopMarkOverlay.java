package dowill.SleepOnBus;

import java.util.ArrayList;
import java.util.List;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.drawable.Drawable;
import android.location.Location;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.EditText;
import android.widget.Toast;

import com.google.android.maps.GeoPoint;
import com.google.android.maps.ItemizedOverlay;
import com.google.android.maps.OverlayItem;

import dowill.SleepOnBus.Activities.DetectingMap;
import dowill.SleepOnBus.backend.BusWebservice;

public class StopMarkOverlay extends ItemizedOverlay<OverlayItem> {
	private List<OverlayItem> _items = new ArrayList<OverlayItem>();
	private Activity _activity = null;
	private int _lineId = 0;
	private String _lineName = null;
	private double _longitude = 0;
	private double _latitude = 0;
	private double _curLongitude = 0;
	private double _curLatitude = 0;
	private String _locationName = null;
	private boolean _isPersonal = false;

	public StopMarkOverlay(Drawable defaultMarker, GeoPoint gp,
			Activity context, boolean clickable, int lineId, String lineName,
			String locationName, boolean isPersonal) {
		super(defaultMarker);
		_longitude = gp.getLongitudeE6() / 1E6;
		_latitude = gp.getLatitudeE6() / 1E6;
		_activity = context;
		_lineId = lineId;
		_lineName = lineName;
		_locationName = locationName;
		_isPersonal = isPersonal;
		_items.add(new OverlayItem(gp, locationName, null));
		populate();
	}

	void setCurrentLocation(GeoPoint gp) {
		_curLongitude = gp.getLongitudeE6() / 1E6;
		_curLatitude = gp.getLatitudeE6() / 1E6;
	}

	void setCurrentLocation(Location loc) {
		_curLongitude = loc.getLongitude();
		_curLatitude = loc.getLatitude();
	}

	@Override
	protected OverlayItem createItem(int i) {
		return _items.get(i);
	}

	@Override
	public int size() {
		return _items.size();
	}

	public boolean OnClick() {
		return onTap(0);
	}

	public void Setup() {
		LayoutInflater inflater = LayoutInflater.from(_activity);
		final View textEntryView = inflater.inflate(R.layout.stop_info_editor,
				null);
		final EditText ed = (EditText) textEntryView
				.findViewById(R.id.txtStopName);
		ed.setText(_locationName);
		ed.setOnClickListener(new View.OnClickListener() {
			public void onClick(View v) {
				if (ed.getText()
						.toString()
						.equals(_activity.getResources().getString(
								R.string.txtSearchTip)))
					ed.setText("");
			}
		});
		new AlertDialog.Builder(_activity)
				.setTitle(R.string.lblCreateStop)
				.setView(textEntryView)
				.setPositiveButton(R.string.save,
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog,
									int which) {
								if (ed.getText().toString().equals("")
										|| ed.getText()
												.toString()
												.equals(_activity
														.getResources()
														.getString(
																R.string.txtSearchTip))) {
									Toast.makeText(_activity,
											R.string.pleaseInputStopName,
											Toast.LENGTH_LONG).show();
								} else {
									if (_isPersonal) {
										BusDAO dao = new BusDAO(_activity);
										dao.open();
										dao.addNewStop(0, ed.getText()
												.toString(),
												(int) (_latitude * 1E6),
												(int) (_longitude * 1E6));
										dao.close();
									} else {
										try {
											if (0 == _lineId)
												_lineId = BusWebservice
														.insertNewLine(_lineName);
											BusWebservice
													.insertNewStop(_lineId, ed
															.getText()
															.toString(),
															_longitude,
															_latitude,
															_curLongitude,
															_curLatitude);
										} catch (Exception e) {
											e.printStackTrace();
										}

										new AlertDialog.Builder(_activity)
												.setTitle(
														R.string.beginDetectingDirectly)
												.setPositiveButton(
														R.string.ok,
														new DialogInterface.OnClickListener() {
															public void onClick(
																	DialogInterface dialog,
																	int which) {
																dialog.dismiss();
																Intent intent = new Intent();
																intent.setClass(
																		_activity,
																		DetectingMap.class);
																ed.getText()
																		.toString();
																intent.putExtra(
																		"stopID",
																		0);
																intent.putExtra(
																		"stopName",
																		ed.getText()
																				.toString());
																intent.putExtra(
																		"lineName",
																		_lineName);
																intent.putExtra(
																		"x",
																		(int) (_latitude * 1E6));
																intent.putExtra(
																		"y",
																		(int) (_longitude * 1E6));
																_activity
																		.startActivity(intent);
																_activity
																		.finish();
															}
														})
												.setNegativeButton(
														R.string.cancel,
														new DialogInterface.OnClickListener() {
															public void onClick(
																	DialogInterface dialog,
																	int whichButton) {
																dialog.dismiss();
																Toast.makeText(
																		_activity,
																		_activity
																				.getResources()
																				.getText(
																						R.string.stopHasBeenEstablished),
																		Toast.LENGTH_LONG)
																		.show();
															}
														}).show();

										dialog.dismiss();
									}
								}
							}
						})
				.setNegativeButton(R.string.cancel,
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog,
									int whichButton) {
								dialog.dismiss();
							}
						}).show();
	}
}
