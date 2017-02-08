using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLib.Models;

namespace DatabaseLib
{
    public class SearchEngineDatabaseManager
    {
        public SearchEngineDatabaseContext DatabaseContext { get; set; }
        public SearchEngineDatabaseManager(bool dropDatabase)
        {
            DatabaseContext = new SearchEngineDatabaseContext(dropDatabase);
        }

        public int InsertDocument(string title)
        {
            DatabaseContext.Documents.Add(new Document { Title = title , DateCreated = DateTime.Now});
            DatabaseContext.SaveChanges();
            return DatabaseContext.Documents.Where(d => d.Title == title).First().DocumentId;
        }

        public int InsertTerm(string value)
        {
            DatabaseContext.Terms.Add(new Models.Term { Value = value });
            DatabaseContext.SaveChanges();
            return DatabaseContext.Terms.Where(t => t.Value == value).First().TermId;
        }

        public void InsertIndex(int position, int documentId, int termId)
        {
            DatabaseContext.Indexes.Add(new Index { DocumentId = documentId, TermId = termId, Position = position });
            DatabaseContext.SaveChanges();
        }

        public bool IsTermInDatabase(string term, out int id)
        {
            bool result = false;

            result = DatabaseContext.Terms.Any(t => t.Value == term);
            if (result)
            {
                id = DatabaseContext.Terms.Where(t => t.Value == term).First().TermId;
            }
            else
            {
                id = -1;
            }

            return result;
        }

        public bool IsDocumentInDatabase(string document, out int id)
        {
            bool result = false;

            result = DatabaseContext.Documents.Any(d => d.Title == document);
            if (result)
            {
                id = DatabaseContext.Documents.Where(d => d.Title == document).First().DocumentId;
            }
            else
            {
                id = -1;
            }

            return result;
        }

        public List<Index> GetIndexFromTermValue(string value)
        {
            int termId = DatabaseContext.Terms.Where(t => t.Value == value).First().TermId;
            List<Index> indexes = DatabaseContext.Indexes.Where(i => i.TermId == termId).ToList();
            return indexes;
        }

        public List<Document> GetDocumentFromId(int id)
        {
            return DatabaseContext.Documents.Where(d => d.DocumentId == id).ToList();
        }

        public List<int> GetPositionsFromIdexes(int documentId, string term)
        {
            int termId = -1;
            if (IsTermInDatabase(term, out termId))
            {
                return DatabaseContext.Indexes.Where(i => i.DocumentId == documentId && i.TermId == termId).Select(i => i.Position).ToList();
            }
            return null;
        }

        public List<string> GetAllTerms()
        {
            List<string> result = DatabaseContext.Terms.Select(t => t.Value).ToList();

            return result;
        }

        public void IndexTermFromDocument(int position, string term, string document)
        {
            int termId;
            int documentId;
            if (!IsTermInDatabase(term, out termId))
            {
                //Console.WriteLine("Term NOT in database with name: " + term);
                termId = InsertTerm(term);
                //Console.WriteLine("Inserted term: " + term + " into database with id: " + termId);
            }

            if (!IsDocumentInDatabase(document, out documentId))
            {
                //Console.WriteLine("Document NOT in database with name: " + document);
                documentId = InsertDocument(document);
                //Console.WriteLine("Inserted document: " + document + " into database with id: " + documentId);
            }
            InsertIndex(position, documentId, termId);
            //Console.WriteLine("Inserted index into database for: " + term + " (" + termId + ") with position: " + position + " in document: " + document + " (" + documentId + ")");
        }
    }
}
