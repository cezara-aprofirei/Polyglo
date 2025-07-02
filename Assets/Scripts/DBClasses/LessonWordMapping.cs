using SQLite;

namespace Assets.Scripts.DBClasses
{
    public class LessonWordMapping
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int lessonID { get; set; }
        public int wordID { get; set; }
    }
}
