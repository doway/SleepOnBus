package dowill.SleepOnBus;

import java.io.File;

import android.content.Context;
import android.media.MediaPlayer;
import android.net.Uri;

public class SoundManager {
	private Context _context = null;
	private boolean _inPlaying = false;
	private int _repeat = 0;
	private int _innerRepeat = 0;
	private MediaPlayer _mp = null;

	public SoundManager(Context context) {
		_context = context;
	}

	public void PlaySound(final int soundID, final int repeat) {
		PlaySound(soundID, repeat, null);
	}

	public void PlaySound(final String soundFile, final int repeat) {
		PlaySound(soundFile, repeat, null);
	}

	public void PlaySound(final String soundFile, final int repeat,
			final SoundCompleteListener listener) {
		File file = new File(soundFile);
		_mp = MediaPlayer.create(_context, Uri.fromFile(file));
		_mp.setOnCompletionListener(new MediaPlayer.OnCompletionListener() {
			public void onCompletion(MediaPlayer mp) {
				_mp.release();
				if (_innerRepeat < _repeat) {
					PlaySound(soundFile, repeat, listener);
				} else {
					_innerRepeat = 0;
					_inPlaying = false;
					if (null != listener)
						listener.OnComplete();
				}
			}
		});
		_mp.setOnErrorListener(new MediaPlayer.OnErrorListener() {
			public boolean onError(MediaPlayer mp, int what, int extra) {
				_mp.release();
				_inPlaying = false;
				return _inPlaying;
			}
		});
		if (null != _mp) {
			try {
				_repeat = repeat;
				_mp.stop();
				_mp.prepare();
				_mp.start();
				_inPlaying = true;
				_innerRepeat++;
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
	}

	public void PlaySound(final int soundID, final int repeat,
			final SoundCompleteListener listener) {
		_mp = MediaPlayer.create(_context, soundID);
		_mp.setOnCompletionListener(new MediaPlayer.OnCompletionListener() {
			public void onCompletion(MediaPlayer mp) {
				_mp.release();
				if (_innerRepeat < _repeat) {
					PlaySound(soundID, repeat, listener);
				} else {
					_innerRepeat = 0;
					_inPlaying = false;
					if (null != listener)
						listener.OnComplete();
				}
			}
		});
		_mp.setOnErrorListener(new MediaPlayer.OnErrorListener() {
			public boolean onError(MediaPlayer mp, int what, int extra) {
				_mp.release();
				_inPlaying = false;
				return _inPlaying;
			}
		});
		if (null != _mp) {
			try {
				_repeat = repeat;
				_mp.stop();
				_mp.prepare();
				_mp.start();
				_inPlaying = true;
				_innerRepeat++;
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
	}

	public void StopSound() {
		_repeat = 0;
		if (_inPlaying) {
			if (null != _mp) {
				try {
					_mp.stop();
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		}
	}

	public boolean getInPlaying() {
		return _inPlaying;
	}

	public static abstract class SoundCompleteListener {
		public abstract void OnComplete();
	}
}
