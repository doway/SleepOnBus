package dowill.SleepOnBus;

import java.util.ArrayList;

import android.content.Context;
import android.database.Cursor;
import android.widget.Adapter;
import android.widget.ArrayAdapter;
import dowill.SleepOnBus.Model.LineInfo;
import dowill.SleepOnBus.Model.StopInfo;
import dowill.SleepOnBus.backend.BusWebservice;

public final class DataAdapterFactory {
	private DataAdapterFactory() {
	}

	public static Adapter GetLineListAdapter(double longitude, double latitude,
			double radius, Context context) throws Exception {
		new BusWebservice(context);
		ArrayList<LineInfo> list = BusWebservice.getLineList(
				longitude, latitude, radius);
		list.add(0, new LineInfo(context.getString(R.string.myPersonal), 0));
		list.add(0, new LineInfo(context.getString(R.string.noWhatINeed), 0));
		list.add(0, new LineInfo(context.getString(R.string.pleaseChoose), 0));
		return new ArrayAdapter<LineInfo>(context,
				android.R.layout.simple_dropdown_item_1line, list);
	}

	public static Adapter GetStopAdapter(int lineID, Context context)
			throws Exception {
		ArrayList<StopInfo> list = new BusWebservice(context)
				.getStopByLineID(lineID);
		list.add(0, new StopInfo(0, 0, context.getString(R.string.noWhatINeed),
				0));
		list.add(0, new StopInfo(0, 0,
				context.getString(R.string.pleaseChoose), 0));
		return new ArrayAdapter<StopInfo>(context,
				android.R.layout.simple_dropdown_item_1line, list);
	}

	public static Adapter GetPersonalStops(Context context) {
		ArrayList<StopInfo> list = new ArrayList<StopInfo>();
		list.add(new StopInfo(0, 0, context.getString(R.string.pleaseChoose), 0));
		list.add(new StopInfo(0, 0, context.getString(R.string.noWhatINeed), 0));
		BusDAO dao = new BusDAO(context);
		dao.open();
		Cursor cursor = dao.selectPersonalStops();
		while (cursor.moveToNext())
			list.add(new StopInfo(cursor.getInt(0), cursor.getInt(3), cursor
					.getInt(4), cursor.getString(2), cursor.getInt(1)));
		cursor.close();
		dao.close();

		return new ArrayAdapter<StopInfo>(context,
				android.R.layout.simple_dropdown_item_1line, list);
	}
}
