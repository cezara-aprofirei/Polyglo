using SQLite;
using System;
using System.Collections.Generic;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public string Username { get; set; }
    public DateTime LastLogin { get; set; }
    public int Streak { get; set; }
    //public string NativeLanguage { get; set; }

    [Ignore]
    public static List<string> EnrolledLanguages { get; set; } = new List<string>();
    [Ignore]
    public static List<string> AvailableLanguages { get; set; } = new List<string>();
}
