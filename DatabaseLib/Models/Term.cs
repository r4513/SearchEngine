using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib.Models
{
    public class Term
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TermId { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(450)]
        [Index("IX_Value", IsUnique = true)]
        public string Value { get; set; }
    }
}
