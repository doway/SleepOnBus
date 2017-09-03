package doway.products.busweb.models;

import javax.jdo.PersistenceManager;

public abstract class GAEBaseModel<T> {
	public void Save() {
		PersistenceManager pm = PMF.get().getPersistenceManager();
		try {
			pm.makePersistent(this);
		} finally {
			pm.close();
		}
	}
	
	public void Delete(){
		PersistenceManager pm = PMF.get().getPersistenceManager();
		try {
			pm.deletePersistent(this);
		} finally {
			pm.close();
		}		
	}

	@SuppressWarnings("unchecked")
	protected static <T> T Load(long id) {
		PersistenceManager pm = PMF.get().getPersistenceManager();
		T model = null;
		try {
			model = (T) pm.getObjectById(id);
		} finally {
			pm.close();
		}
		return model;
	}
}
