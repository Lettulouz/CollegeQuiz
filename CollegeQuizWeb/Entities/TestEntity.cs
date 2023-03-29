using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using CollegeQuizWeb.DbConfig;

namespace CollegeQuizWeb.Entities;

[Table("test_entity")]
public class TestEntity : AbstractAuditableEntity
{
    [Required]
    [Column("name")]
    public string Name { get; set; }
}