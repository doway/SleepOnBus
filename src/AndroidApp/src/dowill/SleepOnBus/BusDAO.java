package dowill.SleepOnBus;

import java.util.Date;

import android.content.ContentValues;
import android.content.Context;
import android.database.Cursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteOpenHelper;

public class BusDAO extends SQLiteOpenHelper {
	private final static String DATABASE_NAME = "Bus";
	private final static int DATABASE_VERSION = 1;
	private final static String TABLE_MY_STOP = "myStop";
	private final static String FIELD_DB_ID = "DbID";
	private final static String FIELD_STOPID = "StopID";
	private final static String FIELD_STOPNAME = "StopName";
	private final static String FIELD_LATITUDE = "Latitude";
	private final static String FIELD_LONGITUDE = "Longitude";
	private final static String MY_TABLE = "FunStatis";
	private final static String FIELD_REC_DATE = "DATE";
	private final static String FIELD_FUNCTION_ID = "FID";
	private final static String FIELD_COUNT = "CNT";
	private SQLiteDatabase _db = null;

	public BusDAO(Context context) {
		super(context, DATABASE_NAME, null, DATABASE_VERSION);
	}
	
	public void open(){
		_db = getWritableDatabase();		
	}
	@Override
	public void onCreate(SQLiteDatabase db) {
		String sql = "CREATE TABLE " + TABLE_MY_STOP + "(" + FIELD_DB_ID
				+ " INTEGER PRIMARY KEY AUTOINCREMENT, " + FIELD_STOPID
				+ " INTEGER, " + FIELD_STOPNAME + " TEXT, " + FIELD_LATITUDE
				+ " INTEGER, " + FIELD_LONGITUDE + " INTEGER)";
		db.execSQL(sql);
		sql = "CREATE TABLE " + MY_TABLE + "(" + FIELD_DB_ID
				+ " INTEGER PRIMARY KEY AUTOINCREMENT, " + FIELD_REC_DATE
				+ " INTEGER, " + FIELD_FUNCTION_ID + " TEXT, " + FIELD_COUNT
				+ " INTEGER)";
		db.execSQL(sql);
	}

	@Override
	public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
		// TODO Auto-generated method stub
	}

	public long addNewStop(int stopID, String stopName, int latitude,
			int longitude) {
		ContentValues cv = new ContentValues();
		cv.put(FIELD_STOPID, stopID);
		cv.put(FIELD_STOPNAME, stopName);
		cv.put(FIELD_LATITUDE, latitude);
		cv.put(FIELD_LONGITUDE, longitude);
		return  _db.insert(TABLE_MY_STOP, null, cv);
	}

	public Cursor selectPersonalStops() {
		Cursor cursor = _db.query(TABLE_MY_STOP, null, null,
				null, null, null, FIELD_STOPNAME);
		return cursor;
	}

	public void deleteStop(int dbID) {
		String where = FIELD_DB_ID + " = ?";
		String[] whereValues = { Integer.toString(dbID) };
		_db.delete(TABLE_MY_STOP, where, whereValues);
	}
	
	public void AddOneCount(String funID) {
		Date dd = new Date();
		int date = dd.getYear() * 10000 + dd.getMonth() * 100 + dd.getDay();
		_db.execSQL("UPDATE " + MY_TABLE + " SET " + FIELD_COUNT + " = "
				+ FIELD_COUNT + "+1 WHERE DATE = " + date + " AND "
				+ FIELD_FUNCTION_ID + " = " + funID);
	}

	public Cursor selectStatistic() {
		Cursor cursor = _db.query(MY_TABLE, null, null, null, null, null,
				FIELD_FUNCTION_ID);
		return cursor;
	}

	public void cleanUp() {
		_db.delete(MY_TABLE, null, null);
	}	
	public void close(){
		_db.close();
	}
}
