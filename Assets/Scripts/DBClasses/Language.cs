using UnityEngine;
using SQLite;

public class Language
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string LanguageName { get; set; }
}
