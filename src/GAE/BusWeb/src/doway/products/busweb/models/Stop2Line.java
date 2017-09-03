package doway.products.busweb.models;

import javax.jdo.annotations.Persistent;

public class Stop2Line extends GAEBaseModel<Stop2Line> {
	@Persistent
	public long LineID;
	@Persistent
	public long StopID;
	public Stop2Line(long lineID, long stopID) {
		super();
		LineID = lineID;
		StopID = stopID;
	}
	@SuppressWarnings("unchecked")
	public static Stop2Line Load(long lineId) {
		return GAEBaseModel.Load(lineId);
	}
}
