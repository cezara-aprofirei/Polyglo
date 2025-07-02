using SQLite;

namespace Assets.Scripts.DBClasses
{
    public class Games
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string GameName { get; set; }
    }
}
