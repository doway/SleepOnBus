package dowill.SleepOnBus.Activities;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import android.app.ListActivity;
import android.app.ProgressDialog;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import dowill.SleepOnBus.Constants;
import dowill.SleepOnBus.R;

public class AlertSoundPicker extends ListActivity {
	private static List<String> _items = null;
	private static List<String> _paths = null;
	private static final String ROOT_PATH = "/mnt/sdcard";
	private ProgressDialog _progDial = null;
	private final Handler _messageHandler = new Handler() {
		public void handleMessage(Message msg) {
			ArrayAdapter<String> fileList = new ArrayAdapter<String>(
					AlertSoundPicker.this, R.layout.file_row, _items);
			setListAdapter(fileList);
			if (null != _progDial)
				try {
					_progDial.dismiss();
					_progDial = null;
				} catch (Exception e) {
					e.printStackTrace();
				}
			super.handleMessage(msg);
		}
	};

	private String[] _sysSounds = null;

	/** Called when the activity is first created. */
	@Override
	protected void onCreate(Bundle icicle) {
		super.onCreate(icicle);

		setContentView(R.layout.file_list);
		_sysSounds = getResources().getStringArray(R.array.soundList);
		getFileDir();
	}

	private void getSubFile(String filePath) {
		File f = new File(filePath);
		File[] files = f.listFiles();
		if (null != files)
			for (int i = 0; i < files.length; i++) {
				File file = files[i];
				String filename = file.getName().toLowerCase();
				if (file.isDirectory()) {
					getSubFile(file.getPath());
				} else if (filename.endsWith(".mp3")
						|| filename.endsWith(".amr")) {
					_items.add(file.getName());
					_paths.add(file.getPath());
				}
			}
	}

	private void getFileDir() {
		if (null == _items) {
			if (null == _progDial)
				try {
					_progDial = ProgressDialog.show(this,
							getText(R.string.waiting),
							getText(R.string.searching), true);
				} catch (Exception e) {
					e.printStackTrace();
				}

			new Thread() {
				public void run() {
					try {
						_items = new ArrayList<String>();
						_paths = new ArrayList<String>();

						// Indicate default built-in sounds
						_items.add(getResources()
								.getString(R.string.sysBuiltIn));
						_paths.add(null);
						for (int i = 0; i < _sysSounds.length; i++) {
							_items.add(_sysSounds[i]);
							_paths.add(String.valueOf(i));
						}
						_items.add(getResources()
								.getString(R.string.userChoose));
						_paths.add(null);

						getSubFile(ROOT_PATH);
						Message msg = new Message();
						_messageHandler.sendMessage(msg);
					} catch (Exception e) {
						e.printStackTrace();
					}
				}
			}.start();
		} else {
			ArrayAdapter<String> fileList = new ArrayAdapter<String>(this,
					R.layout.file_row, _items);
			setListAdapter(fileList);
		}
	}

	@Override
	protected void onListItemClick(ListView l, View v, int position, long id) {
		String clickedOne = _paths.get(position);
		if (null != clickedOne && !clickedOne.equals("")) {
			Intent intent = getIntent();
			// Detect if user was choosing system built-in sounds
			if (position <= _sysSounds.length) {
				// Use system built-in sound
				intent.putExtra(Constants.SETTING_CHOOSE_FILENAME, "");
				intent.putExtra(Constants.SETTING_CHOOSE_FILEPATH, position - 1);
			} else {
				File file = new File(clickedOne);
				intent.putExtra(Constants.SETTING_CHOOSE_FILENAME,
						file.getName());
				intent.putExtra(Constants.SETTING_CHOOSE_FILEPATH,
						file.getPath());
			}
			setResult(RESULT_OK, intent);
			finish();
		}
	}
}