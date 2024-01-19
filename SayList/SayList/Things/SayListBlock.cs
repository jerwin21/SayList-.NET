namespace SayList.Things;

public class SayListBlock
{

    public SayListBlock(int index, int wordIndex, int maxSize)
    {
        Index = index;
        WordIndex = wordIndex;
        SizesNotTried = CreateSizeList(maxSize);
    }

    public int Index { get; set; }

    public int WordIndex { get; set; }

    public List<int> SizesNotTried { get; set; }

    public int SizeLastTried { get; set; }

    public SayListTrack Track { get; set; }

    public string Phrase { get; set; }

    public static List<int> CreateSizeList(int maxSize)
    {
        List<int> list = new List<int>();
        Random rng = new Random();

        for (int i = 1; i <= maxSize; i++)
        {
            list.Add(i);
        }

        List<int> shuffled = list.OrderBy(a => rng.Next()).ToList();

        return shuffled;
    }

}