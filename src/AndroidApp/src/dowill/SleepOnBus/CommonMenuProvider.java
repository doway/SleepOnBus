package dowill.SleepOnBus;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager.NameNotFoundException;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.EditText;
import android.widget.TextView;
import dowill.SleepOnBus.Activities.Settings;

public final class CommonMenuProvider {
	private CommonMenuProvider() {
	}

	public static void onCreateOptionsMenu(Menu menu, Activity host) {
		menu.add(Menu.NONE, 0, 1, R.string.about);
		menu.add(Menu.NONE, 1, 2, R.string.alertSetting);
		menu.add(Menu.NONE, 2, 3, R.string.feedback);
		menu.getItem(0).setIcon(android.R.drawable.ic_dialog_info);
		menu.getItem(1).setIcon(android.R.drawable.ic_menu_preferences);
		menu.getItem(2).setIcon(android.R.drawable.ic_dialog_email);
	}

	public static void onOptionsItemSelected(MenuItem item,
			final Activity host, final SoundManager soundManager) {
		LayoutInflater inflater = LayoutInflater.from(host);
		switch (item.getItemId()) {
		case 0: {
			final View textEntryView = inflater.inflate(R.layout.about, null);
			TextView tvVersion = (TextView) textEntryView
					.findViewById(R.id.tvVersion);
			try {
				PackageInfo pkgInfo = host.getPackageManager().getPackageInfo(
						host.getPackageName(), 0);
				tvVersion.setText("Code : " + pkgInfo.versionCode
						+ "\nName : " + pkgInfo.versionName);
			} catch (NameNotFoundException e) {
				e.printStackTrace();
			}
			new AlertDialog.Builder(host).setTitle(R.string.about)
					.setView(textEntryView)
					.setPositiveButton(R.string.ok, null).show();
			break;
		}
		case 1: {
			Intent intent = new Intent();
			intent.setClass(host, Settings.class);
			host.startActivity(intent);
			break;
		}
		case 2: {
			final View textEntryView = inflater
					.inflate(R.layout.feedback, null);
			new AlertDialog.Builder(host)
					.setTitle(R.string.feedback)
					.setView(textEntryView)
					.setPositiveButton(R.string.submit,
							new DialogInterface.OnClickListener() {
								public void onClick(DialogInterface dialog,
										int which) {
									EditText ed = (EditText) textEntryView
											.findViewById(R.id.edFeedback);
									Intent emailIntent = new Intent(
											android.content.Intent.ACTION_SEND);
									emailIntent.setType("plain/text");
									emailIntent
											.putExtra(
													android.content.Intent.EXTRA_EMAIL,
													new String[] { "dowill.service@gmail.com" });
									emailIntent
											.putExtra(
													android.content.Intent.EXTRA_SUBJECT,
													host.getText(R.string.app_name));
									emailIntent.putExtra(
											android.content.Intent.EXTRA_TEXT,
											ed.getText().toString().trim());
									host.startActivity(Intent.createChooser(
											emailIntent,
											host.getText(R.string.sending)));
								}
							}).setNegativeButton(R.string.cancel, null).show();
			break;
		}
		}
	}
}
