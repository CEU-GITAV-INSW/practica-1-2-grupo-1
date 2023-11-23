using IrrKlang;

public class MusicManager {

    private ISoundEngine _soundEngine = new ISoundEngine();
    private string _music_path;
    private bool _loop;

    public MusicManager(string path_to_music, bool loop ) {
        _music_path = path_to_music; //initialize MusicManager with file path
        _loop = loop;
    }

    public void PlayMusic(){
        _soundEngine.Play2D(_music_path, _loop); // the second parameter set to true makes it play in a loop
    }

    public void StopMusic(){
        _soundEngine.StopAllSounds();
    }
}