using System.ComponentModel.DataAnnotations.Schema;
using SQLite;

namespace PersonalTask;

[SQLite.Table("duty")]

// public class  SetRecord{

//     [SQLite.]]
//     public int count { get; set; }
//     public DateTime? CompletedDate { get; set; }
// }
public class Duty
{

    [PrimaryKey, AutoIncrement]
    [SQLite.Column("id")]
    public int Id { get; set; }

    [SQLite.Column("name")]
    public string Name { get; set; }

    [SQLite.Column("is_done")]
    public bool IsDone { get; set; }

    [SQLite.Column("duration")]
    public TimeSpan Duration { get; set; }

    [SQLite.Column("sets")]
    public int Sets { get; set; }
    
    // [SQLite.Column("set_records")]
    // public SetRecord[] SetRecords { get; set; }

    public DateTime? CompletedDate { get; set; }

    [SQLite.Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
