using SQLite;

namespace Assets.Scripts.DBClasses
{
    public class ScoreHistory
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int gameID { get; set; }
        public float score { get; set; }
    }
}
