package dowill.SleepOnBus.Model;

public class LineInfo {
	private String _lineName = null;
	private int _lineID = 0;

	public LineInfo(String lineName, int lineID) {
		_lineName = lineName;
		_lineID = lineID;
	}

	public String getLineName() {
		return _lineName;
	}

	public int getLineID() {
		return _lineID;
	}

	@Override
	public String toString() {
		return _lineName;
	}
}
