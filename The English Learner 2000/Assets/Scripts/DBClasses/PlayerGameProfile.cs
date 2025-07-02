using SQLite;

namespace Assets.Scripts.DBClasses
{
    public class PlayerGameProfile
    {
        [PrimaryKey, AutoIncrement] 
        public int ID { get; set; }
        public int gameID { get; set; }
        public int currentLevel { get; set; }
        
    }
}
