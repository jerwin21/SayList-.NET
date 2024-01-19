namespace SayList.Things;

public class SayListState
{
    public List<SayListBlock> Playlist { get; set; }

    public int WordsRemaining { get; set; }

    public int WordIndex { get; set; }

    public int BlockIndex { get; set; }

    public int SetLength { get; set; }
}