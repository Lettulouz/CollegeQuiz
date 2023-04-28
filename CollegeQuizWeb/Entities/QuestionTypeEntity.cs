using System.ComponentModel.DataAnnotations.Schema;
using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("question_types")]
public class QuestionTypeEntity : AbstractAuditableEntity
{
    [Column("site_id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; }
}