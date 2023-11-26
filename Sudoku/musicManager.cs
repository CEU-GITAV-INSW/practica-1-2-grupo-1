using IrrKlang;

public interface IMusicManager{

    public void PlayMusic();
    public void StopMusic();

}
public class MusicManager: IMusicManager {

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

public class FakeMusicManager: IMusicManager{


    public FakeMusicManager(){
    }
    public void PlayMusic(){
        System.Console.WriteLine("Sorry, playing music is not supported on this architecture");
    }
    public void StopMusic(){
        System.Console.WriteLine("Sorry, stopping music is not supported on this architecture");
    }

}