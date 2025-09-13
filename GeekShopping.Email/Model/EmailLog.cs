using System.ComponentModel.DataAnnotations.Schema;
using GeekShopping.Email.Model.Base;

namespace GeekShopping.Email.Model;

[Table("email_logs")]
public class EmailLog : BaseEntity
{
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    [Column("log")]
    public string Log { get; set; } = string.Empty;
    [Column("sent_date")]
    public DateTime SentDate { get; set; }
}
