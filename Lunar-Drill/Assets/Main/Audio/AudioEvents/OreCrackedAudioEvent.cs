
// Event that signals that an ore was cracked by drillian


public class OreCrackedAudioEvent : IAudioEvent
{
    public int PitchNumber; // int from 0 to 5 that encodes the pitch of the sound.
    public OreCrackedAudioEvent(int PitchNumber)
    {
        this.PitchNumber = PitchNumber;
    }
}
