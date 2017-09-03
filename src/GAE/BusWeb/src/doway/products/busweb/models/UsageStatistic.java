package doway.products.busweb.models;

import javax.jdo.annotations.Persistent;

public class UsageStatistic extends GAEBaseModel<UsageStatistic> {
	public UsageStatistic(byte device, double latitude, double longitude,
			double radius, String userCode) {
		super();
		Device = device;
		Latitude = latitude;
		Longitude = longitude;
		Radius = radius;
		UserCode = userCode;
	}

	@Persistent
	public byte Device;
	@Persistent
	public double Latitude;
	@Persistent
	public double Longitude;
	@Persistent
	public double Radius;
	@Persistent
	public String UserCode;

	@SuppressWarnings("unchecked")
	public static UsageStatistic Load(long stopId) {
		return GAEBaseModel.Load(stopId);
	}
}
