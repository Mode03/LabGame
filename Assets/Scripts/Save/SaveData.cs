[System.Serializable]
public class SaveData
{
    public int currency;

    public int level;
    public int totalExperience;

    public bool[] isBought = new bool[12];
    public float[] revealTimers = new float[12];
}
