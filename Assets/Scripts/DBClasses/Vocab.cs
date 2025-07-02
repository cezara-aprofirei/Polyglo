using SQLite;

public class Vocab
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public int LanguageID { get; set; }
    public string Word { get; set; }
    public int LessonID { get; set; }
    public int GameID { get; set; }
    public int Level { get; set; }
}
