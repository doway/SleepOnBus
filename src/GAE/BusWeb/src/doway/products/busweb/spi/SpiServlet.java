package doway.products.busweb.spi;

import java.io.IOException;
import java.util.HashSet;
import java.util.logging.Logger;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.google.gson.Gson;

import doway.products.busweb.models.LineInfo;
import doway.products.busweb.models.Stop2Line;
import doway.products.busweb.models.StopInfo;
import doway.products.busweb.models.UsageStatistic;

@SuppressWarnings("serial")
public class SpiServlet extends HttpServlet {
	private static final Logger _logger = Logger.getLogger(SpiServlet.class
			.getName());
	private static HashSet<String> _preventDupSet = new HashSet<String>();

	@Override
	protected void doPost(HttpServletRequest req, HttpServletResponse resp)
			throws ServletException, IOException {
		super.doPost(req, resp);
	}

	public void doGet(HttpServletRequest req, HttpServletResponse resp)
			throws IOException, ServletException {
		Object rtn = null;
		resp.reset();
		switch (req.getParameter("spi")) {
		case "gnv": // get new version
			rtn = getNewVersion();
			break;
		case "inl": // insert new line
			rtn = insertNewLine(
					req.getParameter("linename"),
					String.format("%1$2s-%2$2s", req.getParameter("lang"),
							req.getParameter("country")));
			break;
		case "ins": // insert new stop
			insertNewStop(
					Integer.parseInt(req.getParameter("lineid")),
					req.getParameter("stopname"),
					Double.parseDouble(req.getParameter("longitude")),
					Double.parseDouble(req.getParameter("latitude")),
					Double.parseDouble(req.getParameter("curLongitude")),
					Double.parseDouble(req.getParameter("curLatitude")),
					req.getParameter("owner"),
					String.format("%1$2s-%2$2s", req.getParameter("lang"),
							req.getParameter("country")));
			break;
		case "gll": // get line list
			rtn = getLineList(
					Double.parseDouble(req.getParameter("longitude")),
					Double.parseDouble(req.getParameter("latitude")),
					Double.parseDouble(req.getParameter("radius")),
					req.getParameter("owner"), (byte) 1);
			break;
		case "gsbl": // get stop by line
			rtn = getStopsByLineID(
					Integer.parseInt(req.getParameter("lineid")),
					req.getParameter("owner"));
			break;
		case "rs": // rate stop
			rateStop(Integer.parseInt(req.getParameter("stopid")),
					req.getParameter("gb"));
			break;
		case "ds": // delete stop by owner
			rtn = deleteStop(Integer.parseInt(req.getParameter("stopid")),
					req.getParameter("owner"));
			break;
		}
		Gson gson = new Gson();
		if (null != rtn)
			resp.getWriter().println(gson.toJson(rtn));
	}

	private String getNewVersion() {
		return "1.2011092700"; // ConfigurationManager.AppSettings["LatestVersion"];
	}

	private long insertNewLine(String lineName, String culture) {
		_logger.info("insertNewLine");
		LineInfo line_info = new LineInfo(lineName, culture);
		line_info.Save();
		return line_info.LineID;
	}

	/**
	 * @param lineID
	 * @param stopName
	 * @param longitude
	 * @param latitude
	 * @param curLongitude
	 * @param curLatitude
	 * @param owner
	 * @param culture
	 */
	private void insertNewStop(long lineID, String stopName, double longitude,
			double latitude, double curLongitude, double curLatitude,
			String owner, String culture) {
		_logger.info("insertNewStop");
		// issue #38 http://doway-svr/btnet/edit_bug.aspx?id=38
		String hashData = String.format("%1$S%2$S%3$S%4$S%5$S", lineID,
				stopName, longitude, latitude, owner);
		synchronized (_preventDupSet) {
			if (_preventDupSet.contains(hashData)) {
				_preventDupSet.clear();
				return;
			}
			_preventDupSet.add(hashData);
		}

		StopInfo stop_info = new StopInfo(stopName, longitude, latitude, 0, 0,
				curLongitude, curLatitude, culture,
				(null == owner || "" == owner) ? "SYSTEM" : owner);
		stop_info.Save();

		Stop2Line stop_to_line = new Stop2Line(lineID, stop_info.StopID);
		stop_to_line.Save();

		// region Do remove bad rating stops and data merging
		// synchronized (SpiServlet.class)
		// {
		// if (null == _mergeDataThread ||
		// _mergeDataThread.ThreadState != ThreadState.Running)
		// {
		// _mergeDataThread = new Thread(new
		// ParameterizedThreadStart(removeBadRatingAndMergeLines));
		// _mergeDataThread.Start(db);
		// }
		// }
	}

	private void removeBadRatingAndMergeLines(Object obj) {
		_logger.info("removeBadRatingAndMergeLines");
		// db.RemoveBadRatingStops();
		// db.MergeLines();
	}

	private LineInfo[] getLineList(double Longitude, double Latitude,
			double radius, String owner, byte device) {
		_logger.info("getLineList");
		UsageStatistic usage_statistic = new UsageStatistic(device, Latitude,
				Longitude, radius, owner);
		usage_statistic.Save();
		LineInfo[] line_list = LineInfo.GetLinesList(Longitude, Latitude,
				radius, owner);

		return line_list;
	}

	private StopInfo[] getStopsByLineID(long lineID, String owner) {
		_logger.info("getStopsByLineID");
		return StopInfo.GetStopsByLineID(lineID, owner);
	}

	private void rateStop(long stopID, String goodOrBad) {
		_logger.info("rateStop");
		StopInfo stop_info = StopInfo.Load(stopID);
		switch (goodOrBad) {
		case "g":
			stop_info.Good++;
			break;
		case "b":
			stop_info.Bad++;
			break;
		}
		stop_info.Save();
	}

	private boolean deleteStop(long stopID, String owner) {
		_logger.info("deleteStop");
		StopInfo stop_info = StopInfo.Load(stopID);
		stop_info.Delete();
		return true;
	}
}
