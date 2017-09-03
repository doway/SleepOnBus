package doway.products.busweb.models;

import javax.jdo.annotations.IdGeneratorStrategy;
import javax.jdo.annotations.IdentityType;
import javax.jdo.annotations.PersistenceCapable;
import javax.jdo.annotations.Persistent;
import javax.jdo.annotations.PrimaryKey;

@PersistenceCapable(identityType = IdentityType.APPLICATION)
public class LineInfo extends GAEBaseModel<LineInfo> {
	public LineInfo(String lineName, String culture) {
		super();
		LineName = lineName;
		Culture = culture;
	}

	@PrimaryKey
	@Persistent(valueStrategy = IdGeneratorStrategy.IDENTITY)
	public Long LineID;
	@Persistent
	public String LineName;
	@Persistent
	public String Culture;

	@SuppressWarnings("unchecked")
	public static LineInfo Load(long lineId) {
		return GAEBaseModel.Load(lineId);
	}

	public static LineInfo[] GetLinesList(double ongitude, double Latitude,
			double radius, String owner) {
		return null;	// TODO: Uncompleted
	}
}
