package dowill.SleepOnBus;

public final class Constants {
	private Constants() {
	}

	public final static String TAG = "dowill.SleepOnBus";
	public final static String SETTING_AUDIO_SELECT_SYS = "AUDIO_SELECT";
	public final static String SETTING_AUDIO_SELECT_USER = "AUDIO_SELECT_USER_PREFER";
	public final static String SETTING_AWARE_DISTANCE = "AWARE_DISTANCE";
	public final static String SETTING_AWARE_LINE = "AWARE_LINE";
	public final static String SETTING_PUBLISH_AGREE = "PUBLISH_AGREE";
	public final static String SETTING_DETECT_TIP_NO_REMIND = "DETECT_NO_REMIND";
	public final static String SETTING_CHOOSE_FILENAME = "CHOOSE_FILENAME";
	public final static String SETTING_CHOOSE_FILEPATH = "CHOOSE_FILEPATH";
	public final static String SETTING_ENABLE_SHOCK = "ENABLE_SHOCK";
	public final static String SETTING_ENABLE_AUTO_VOL = "ENABLE_AUTO_VOL";
	public final static int[] SYSTEM_SOUND_RES_ID = { R.raw.bus_normal,
			R.raw.bus_wakeup, R.raw.bus_runaway, R.raw.bus_getout, -1 };
}
