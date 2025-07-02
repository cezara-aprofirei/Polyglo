using SQLite;

namespace Assets.Scripts.DBClasses
{
    public class PlayerLessonProgress
    {
        [PrimaryKey, AutoIncrement]
        public int playerID { get; set; }
        public int lessonID { get; set; }
        public int status { get; set; }//-1=locked, 0=unlocked, 1=completed
    }
}
