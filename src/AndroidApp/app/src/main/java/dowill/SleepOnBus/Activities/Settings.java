package dowill.SleepOnBus.Activities;

import java.io.File;

import android.app.Activity;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.ToggleButton;
import dowill.SleepOnBus.Constants;
import dowill.SleepOnBus.R;
import dowill.SleepOnBus.SoundManager;

public class Settings extends Activity {
	private SoundManager _soundManager = null;
	private TextView _currentSoundFile = null;
	private EditText _etAwareDistance = null;
	private EditText _etAwareLine = null;
	private Button _btnPlaySound = null;
	private Button _btnStopPlaySound = null;
	private Button _btnChooseSoundFile = null;
	private String _soundEffectName = null;
	private boolean _enableShock = false;
	private boolean _enableAutoVol = true;
	private boolean _useSysBuiltInSound = false;
	private int _choosedSysBuiltInSoundIdx = 0;
	private String[] _sysSounds = null;

	@Override
	protected void onActivityResult(int requestCode, int resultCode, Intent data) {
		switch (resultCode) {
		case RESULT_OK:
			String choosedSoundName = data
					.getStringExtra(Constants.SETTING_CHOOSE_FILENAME);
			_useSysBuiltInSound = (null == choosedSoundName || choosedSoundName
					.equals(""));
			if (_useSysBuiltInSound) {
				_choosedSysBuiltInSoundIdx = data.getIntExtra(
						Constants.SETTING_CHOOSE_FILEPATH, 0);
				choosedSoundName = _sysSounds[_choosedSysBuiltInSoundIdx];
				if (-1 == Constants.SYSTEM_SOUND_RES_ID[_choosedSysBuiltInSoundIdx]) {
					_btnPlaySound.setEnabled(false);
				}
			} else {
				_currentSoundFile.setText(choosedSoundName);
				_soundEffectName = data
						.getStringExtra(Constants.SETTING_CHOOSE_FILEPATH);
				Log.d(Constants.TAG, "fullFileName=" + _soundEffectName);
			}
			_currentSoundFile.setText(choosedSoundName);
			break;
		default:
			break;
		}
		super.onActivityResult(requestCode, resultCode, data);
	}

	/** Called when the activity is first created. */
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.settings);
		_soundManager = new SoundManager(this);
		_currentSoundFile = (TextView) findViewById(R.id.tvCurrentSounfFile);

		_sysSounds = getResources().getStringArray(R.array.soundList);

		SharedPreferences settings = getSharedPreferences(Constants.TAG,
				MODE_PRIVATE);
		_soundEffectName = settings.getString(
				Constants.SETTING_AUDIO_SELECT_USER, null);
		_useSysBuiltInSound = (null == _soundEffectName || _soundEffectName
				.equals(""));
		if (_useSysBuiltInSound) {
			_choosedSysBuiltInSoundIdx = settings.getInt(
					Constants.SETTING_AUDIO_SELECT_SYS, 0);
			_currentSoundFile.setText(_sysSounds[_choosedSysBuiltInSoundIdx]);
		} else {
			File soundFile = new File(_soundEffectName);
			_currentSoundFile.setText(soundFile.getName());
		}
		_etAwareDistance = (EditText) findViewById(R.id.etAwareDistance);
		_etAwareDistance.setText(String.valueOf(settings.getInt(
				Constants.SETTING_AWARE_DISTANCE, 1000)));

		_etAwareLine = (EditText) findViewById(R.id.etAwareLine);
		_etAwareLine.setText(String.valueOf(settings.getInt(
				Constants.SETTING_AWARE_LINE, 10000)));

		_btnPlaySound = (Button) findViewById(R.id.btnPlaySound);
		_btnStopPlaySound = (Button) findViewById(R.id.btnStopPlaySound);
		_btnChooseSoundFile = (Button) findViewById(R.id.btnChooseSoundFile);

		ToggleButton tbtnEnableShock = (ToggleButton) findViewById(R.id.tbtnEnableShock);
		_enableShock = settings.getBoolean(Constants.SETTING_ENABLE_SHOCK,
				false);
		tbtnEnableShock.setChecked(_enableShock);
		tbtnEnableShock
				.setOnCheckedChangeListener(new ToggleButton.OnCheckedChangeListener() {
					public void onCheckedChanged(CompoundButton buttonView,
							boolean isChecked) {
						_enableShock = isChecked;
					}
				});
		ToggleButton tbtnEnableAutoVol = (ToggleButton) findViewById(R.id.tbtnEnableAutoVol);
		_enableAutoVol = settings.getBoolean(Constants.SETTING_ENABLE_AUTO_VOL,
				true);
		tbtnEnableAutoVol.setChecked(_enableAutoVol);
		tbtnEnableAutoVol
				.setOnCheckedChangeListener(new ToggleButton.OnCheckedChangeListener() {
					public void onCheckedChanged(CompoundButton buttonView,
							boolean isChecked) {
						_enableAutoVol = isChecked;
					}
				});	}

	public void btnPlaySoundOnClick(View v) {
		SoundManager.SoundCompleteListener listener = new SoundManager.SoundCompleteListener() {
			@Override
			public void OnComplete() {
				_btnPlaySound.setEnabled(true);
				_btnStopPlaySound.setEnabled(false);
				_btnChooseSoundFile.setEnabled(true);
			}
		};
		if (null == _soundEffectName || _soundEffectName.equals("")) {
			_soundManager.PlaySound(
					Constants.SYSTEM_SOUND_RES_ID[_choosedSysBuiltInSoundIdx],
					1, listener);
		} else {
			_soundManager.PlaySound(_soundEffectName, 1, listener);
		}
		_btnPlaySound.setEnabled(false);
		_btnChooseSoundFile.setEnabled(false);
		_btnStopPlaySound.setEnabled(true);
	}

	public void btnStopPlaySoundOnClick(View v) {
		_soundManager.StopSound();
		_btnPlaySound.setEnabled(true);
		_btnStopPlaySound.setEnabled(false);
		_btnChooseSoundFile.setEnabled(true);
	}

	public void btnSaveOnClick(View v) {
		SharedPreferences settings = getSharedPreferences(Constants.TAG, 0);
		SharedPreferences.Editor editor = null;
		if (_useSysBuiltInSound) {
			editor = settings.edit().putInt(Constants.SETTING_AUDIO_SELECT_SYS,
					_choosedSysBuiltInSoundIdx);
			editor = editor
					.putString(Constants.SETTING_AUDIO_SELECT_USER, null);
		} else {
			editor = settings.edit().putString(
					Constants.SETTING_AUDIO_SELECT_USER, _soundEffectName);
		}
		editor.putInt(Constants.SETTING_AWARE_DISTANCE,
				Integer.parseInt(_etAwareDistance.getText().toString()))
				.putInt(Constants.SETTING_AWARE_LINE,
						Integer.parseInt(_etAwareLine.getText().toString()))
				.putBoolean(Constants.SETTING_ENABLE_SHOCK, _enableShock)
				.putBoolean(Constants.SETTING_ENABLE_AUTO_VOL, _enableAutoVol)
				.commit();
		Toast.makeText(this, R.string.effected, Toast.LENGTH_LONG).show();
		finish();
	}

	public void btnCancelOnClick(View v) {
		finish();
	}

	public void btnChooseSoundFile(View v) {
		Intent intent = new Intent(this, AlertSoundPicker.class);
		startActivityForResult(intent, 0);
	}
}
