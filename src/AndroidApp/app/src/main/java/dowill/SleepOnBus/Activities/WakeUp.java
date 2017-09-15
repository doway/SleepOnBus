package dowill.SleepOnBus.Activities;

import android.app.Activity;
import android.os.Bundle;
import android.view.View;
import dowill.SleepOnBus.R;
import dowill.SleepOnBus.service.DetectingService;

public class WakeUp extends Activity {
	/** Called when the activity is first created. */
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.wake_up);
	}
	
	public void btnStopSoundOnClick(View v){
		DetectingService ds = DetectingService.getInstance();
		if (null != ds){
			ds.StopSound();
		}
		finish();
	}
}
