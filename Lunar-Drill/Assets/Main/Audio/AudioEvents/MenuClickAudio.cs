public class MenuClickAudio : IAudioEvent
{
    public string Info;

    public MenuClickAudio(string info)
    {
        this.Info = info;
    }
}
