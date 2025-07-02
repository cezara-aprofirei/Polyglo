using SQLite;
using UnityEngine;

public class Lesson 
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Theory { get; set; }
    public string ExtraButtonText { get; set; }
    public string ExtraButtonTitle { get; set; }
    public int Level { get; set; }

}
