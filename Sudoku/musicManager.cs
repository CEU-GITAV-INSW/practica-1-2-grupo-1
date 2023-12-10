using IrrKlang;

public class MusicManager
{

    private ISoundEngine _soundEngine = new ISoundEngine();
    private ISound _currentSound;
    private string _music_path;
    private bool _loop;

    public MusicManager(string path_to_music, bool loop)
    {
        _music_path = path_to_music; //initialize MusicManager with file path
        _loop = loop;
    }

    public void PlayMusic()
    {
        _currentSound = _soundEngine.Play2D(_music_path, _loop); // the second parameter set to true makes it play in a loop
    }

    public void StopMusic()
    {
        _soundEngine.StopAllSounds();
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
