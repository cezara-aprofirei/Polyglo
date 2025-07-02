using SQLite;
using UnityEngine;

public class UserLanguages 
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public int UserID { get; set; }
    public int LanguageID { get; set; }
}
