using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using DatabaseLib.Models;

namespace DatabaseLib
{
    public class SearchEngineDatabaseContext : DbContext
    {
        public SearchEngineDatabaseContext(bool dropDatabase)
        {
            if (dropDatabase)
            {
                Database.SetInitializer(new DropCreateDatabaseAlways<SearchEngineDatabaseContext>());
            }
        }
        public DbSet<Index> Indexes { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<Document> Documents { get; set; }
    }
}
