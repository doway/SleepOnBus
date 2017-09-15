package dowill.SleepOnBus.backend;

import java.net.URLEncoder;
import java.util.ArrayList;
import java.util.Locale;

import org.apache.http.HttpResponse;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.util.EntityUtils;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import android.accounts.Account;
import android.accounts.AccountManager;
import android.content.Context;
import android.util.Log;
import dowill.SleepOnBus.Constants;
import dowill.SleepOnBus.R;
import dowill.SleepOnBus.Model.LineInfo;
import dowill.SleepOnBus.Model.StopInfo;

public class BusWebservice {
	private static final String _backUrl = "http://bus.comeondata.com/spi/spi.aspx?";
	private Context _context = null;
	private static final int _multiInt = (int) 1E6;
	private static String _owner = null;
	public static Locale _locale = null;

	public static void setLocale(Locale locale) {
		_locale = locale;
		if (null != _locale) {
			Log.d(Constants.TAG, "_locale.language=" + _locale.getLanguage());
			Log.d(Constants.TAG,
					"_locale.displayLanguage=" + _locale.getDisplayLanguage());
			Log.d(Constants.TAG, "_locale.country=" + _locale.getCountry());
			Log.d(Constants.TAG,
					"_locale.displayCountry=" + _locale.getDisplayCountry());
		}
	}

	public static Locale getLocale() {
		return _locale;
	}

	public BusWebservice(Context context) {
		_context = context;
		synchronized (_backUrl) {
			if (null == _owner) {
				Account[] acct = AccountManager.get(context).getAccountsByType(
						"com.google");
				Log.d(Constants.TAG, "acct.length=" + acct.length);
				if (acct.length > 0) {
					Log.d(Constants.TAG, "acct[0].name=" + acct[0].name);
					_owner = acct[0].name;
				} else {
					_owner = "";
				}
			}
		}
	}

	public static String getNewVersion() throws Exception {
		return getHttpResponse(_backUrl + "spi=gnv").replace("\"", "");
	}

	public static int insertNewLine(String linename) throws Exception {
		linename = replaceFullShapeChar(linename);
		return Integer.parseInt(getHttpResponse(_backUrl + "spi=inl&linename="
				+ URLEncoder.encode(linename, "utf-8")));
	}

	public static void insertNewStop(int lineid, String stopname,
			double longitude, double latitude, double curLongitude,
			double curLatitude) throws Exception {
		stopname = replaceFullShapeChar(stopname);
		getHttpResponse(_backUrl
				+ "spi=ins&lineid="
				+ URLEncoder.encode(String.valueOf(lineid), "utf-8")
				+ "&lang="
				+ URLEncoder.encode((null != _locale) ? _locale.getLanguage()
						: "", "utf-8")
				+ "&country="
				+ URLEncoder.encode((null != _locale) ? _locale.getCountry()
						: "", "utf-8") + "&stopname="
				+ URLEncoder.encode(stopname, "utf-8") + "&longitude="
				+ URLEncoder.encode(String.valueOf(longitude), "utf-8")
				+ "&latitude="
				+ URLEncoder.encode(String.valueOf(latitude), "utf-8")
				+ "&curLongitude="
				+ URLEncoder.encode(String.valueOf(curLongitude), "utf-8")
				+ "&curLatitude="
				+ URLEncoder.encode(String.valueOf(curLatitude), "utf-8")
				+ "&owner=" + URLEncoder.encode(_owner, "utf-8"));
	}

	public static ArrayList<LineInfo> getLineList(double longitude,
			double latitude, double radius) throws Exception {
		JSONArray jary = jsonToObj(getHttpResponse(_backUrl
				+ "spi=gll&&longitude="
				+ URLEncoder.encode(String.valueOf(longitude), "utf-8")
				+ "&latitude="
				+ URLEncoder.encode(String.valueOf(latitude), "utf-8")
				+ "&radius="
				+ URLEncoder.encode(String.valueOf(radius), "utf-8")
				+ "&owner=" + URLEncoder.encode(_owner, "utf-8")));
		ArrayList<LineInfo> ary = new ArrayList<LineInfo>();
		for (int i = 0; i < jary.length(); i++) {
			JSONObject jObj = jary.getJSONObject(i);
			ary.add(new LineInfo(jObj.getString("LineName"), jObj
					.getInt("LineID")));
		}
		return ary;
	}

	public ArrayList<StopInfo> getStopByLineID(int lineid) throws Exception {
		JSONArray jary = jsonToObj(getHttpResponse(_backUrl
				+ "spi=gsbl&lineid="
				+ URLEncoder.encode(String.valueOf(lineid), "utf-8")
				+ "&owner=" + URLEncoder.encode(_owner, "utf-8")));
		ArrayList<StopInfo> ary = new ArrayList<StopInfo>();
		for (int i = 0; i < jary.length(); i++) {
			JSONObject jObj = jary.getJSONObject(i);
			ary.add(new StopInfo(
					(int) (_multiInt * jObj.getDouble("Latitude")),
					(int) (_multiInt * jObj.getDouble("Longtitude")), jObj
							.getString("StopName").replace("(*)",
									_context.getString(R.string.mine)), jObj
							.getInt("StopID")));
		}
		return ary;
	}

	public static void rateStop(int stopid, boolean good) throws Exception {
		getHttpResponse(_backUrl + "spi=rs&stopid="
				+ URLEncoder.encode(String.valueOf(stopid), "utf-8") + "&gb="
				+ ((good) ? "g" : "b"));
	}

	public static boolean deleteStop(int stopid) {
		boolean success = false;
		try {
			success = Boolean.parseBoolean(getHttpResponse(_backUrl
					+ "spi=ds&stopid="
					+ URLEncoder.encode(String.valueOf(stopid), "utf-8")
					+ "&owner=" + URLEncoder.encode(_owner, "utf-8")));
			Log.d(Constants.TAG, "success = " + success);
		} catch (Exception e) {
			e.printStackTrace();
		}
		return success;
	}

	private static JSONArray jsonToObj(String jsonString) throws JSONException {
		return new JSONArray(jsonString);
	}

	private static String getHttpResponse(String rqUrl) throws Exception {
		HttpGet rq = new HttpGet(rqUrl);
		String rtn = null;
		try {
			HttpResponse rs = new DefaultHttpClient().execute(rq);
			if (rs.getStatusLine().getStatusCode() == 200) {
				rtn = EntityUtils.toString(rs.getEntity());
				Log.d(Constants.TAG, "rqUrl=" + rqUrl + "\trtn=" + rtn);
			} else {
				throw new Exception("statusCode="
						+ rs.getStatusLine().getStatusCode());
			}
		} catch (Exception e) {
			e.printStackTrace();
			throw e;
		}
		return rtn;
	}

	private static String replaceFullShapeChar(String inputString) {
		char[] fullChar = { '１', '２', '３', '４', '５', '６', '７', '８', '９', '０' };
		char[] ansiChar = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
		for (int i = 0; i < fullChar.length; i++)
			inputString = inputString.replace(fullChar[i], ansiChar[i]);
		return inputString;
	}
}