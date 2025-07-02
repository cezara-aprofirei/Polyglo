using SQLite;

namespace Assets.Scripts.DBClasses
{
    public class GameWordMapping
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int gameID { get; set; }
        public int wordID { get; set; }
    }
}
