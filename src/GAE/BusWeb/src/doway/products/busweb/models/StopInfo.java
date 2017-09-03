package doway.products.busweb.models;

import javax.jdo.annotations.IdGeneratorStrategy;
import javax.jdo.annotations.IdentityType;
import javax.jdo.annotations.PersistenceCapable;
import javax.jdo.annotations.Persistent;
import javax.jdo.annotations.PrimaryKey;

@PersistenceCapable(identityType = IdentityType.APPLICATION)
public class StopInfo extends GAEBaseModel<StopInfo> {
	@PrimaryKey
	@Persistent(valueStrategy = IdGeneratorStrategy.IDENTITY)
	public Long StopID;
	@Persistent
	public String StopName;
	@Persistent
	public double Longitude;
	@Persistent
	public double Latitude;
	@Persistent
	public int Good;
	@Persistent
	public int Bad;
	@Persistent
	public double CreatorLongitude;
	@Persistent
	public double CreatorLatitude;
	@Persistent
	public String Culture;
	@Persistent
	public String Owner;

	public StopInfo(String stopName, double longitude, double latitude,
			int good, int bad, double creatorLongitude, double creatorLatitude,
			String culture, String owner) {
		super();
		StopName = stopName;
		Longitude = longitude;
		Latitude = latitude;
		Good = good;
		Bad = bad;
		CreatorLongitude = creatorLongitude;
		CreatorLatitude = creatorLatitude;
		Culture = culture;
		Owner = owner;
	}

	@SuppressWarnings("unchecked")
	public static StopInfo Load(long stopId) {
		return GAEBaseModel.Load(stopId);
	}

	public static StopInfo[] GetStopsByLineID(double lineID, String owner) {
		return null; // TODO: uncompleted
	}
}
