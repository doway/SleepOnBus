package dowill.SleepOnBus.Model;

import com.google.android.maps.GeoPoint;

public class StopInfo extends GeoPoint {

	private String _stopName = null;
	private int _stopID = 0;
	private int _dbID = 0;

	public StopInfo(int latitudeE6, int longitudeE6, String stopName, int stopID) {
		super(latitudeE6, longitudeE6);
		_stopName = stopName;
		_stopID = stopID;
	}
	
	public StopInfo(int dbID, int latitudeE6, int longitudeE6, String stopName, int stopID){
		this(latitudeE6, longitudeE6, stopName, stopID);
		_dbID = dbID;
	}

	public String getStopName() {
		return _stopName;
	}
	
	public int getStopID(){
		return _stopID;
	}
	
	public int getDbID(){
		return _dbID;
	}

	@Override
	public String toString() {
		return _stopName;
	}
}
