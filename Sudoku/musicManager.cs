using IrrKlang;

public class MusicManager
{

    private ISoundEngine _soundEngine = new ISoundEngine();
    private ISound _currentSound;
    private string _music_path;
    private bool _loop;
    private bool _isPaused;

    public MusicManager(string path_to_music, bool loop)
    {
        _music_path = path_to_music; //initialize MusicManager with file path
        _loop = loop;
    }

    public void PlayMusic()
    {
        _currentSound = _soundEngine.Play2D(_music_path, _loop); // the second parameter set to true makes it play in a loop
    }

    public void PauseResumeMusic()
    {
        if (_currentSound != null)
        {
            if (_isPaused)
            {
                _currentSound.Paused = false; // Reanudar la música
                _isPaused = false;
            }
            else
            {
                _currentSound.Paused = true; // Pausar la música
                _isPaused = true;
            }
        }
    }

    public void StopMusic()
    {
        if (_currentSound != null)
        {
            _currentSound.Paused = false; 
            _soundEngine.StopAllSounds();
            _currentSound = null;
            _isPaused = false;
        }
    }
    public void AdjustVolume(float volume)
    {
        if (_currentSound != null)
        {
            // Asegúrate de que el volumen esté en el rango [0, 1]
            volume = System.Math.Max(0f, System.Math.Min(1f, volume));
            _currentSound.Volume = volume;
        }
    }
}
