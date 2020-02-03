public class CharacterDB 
{
    [System.Serializable]
    public struct Stats
    {
        public int shot;
        public int technique;
        public int defense;
    }

    //info user
    public Stats stats;
    public int ID;
    public int level;
    public int power;
    public bool owned;
}
