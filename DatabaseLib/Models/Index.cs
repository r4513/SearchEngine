using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib.Models
{
    public class Index
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Document")]
        public int DocumentId { get; set; }
        public virtual Document Document { get; set; }
        [Index("IX_Term", IsUnique = false)]
        [ForeignKey("Term")]
        public int TermId { get; set; }
        public virtual Term Term { get; set; }
        public int Position { get; set; }
    }
}
