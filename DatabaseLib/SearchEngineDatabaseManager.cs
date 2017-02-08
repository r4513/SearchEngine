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
        private SearchEngineDatabaseContext databaseContext;
        private Dictionary<string, Document> documentsInDatabase;
        private Dictionary<string, Term> termsInDatabase;
        public SearchEngineDatabaseManager(bool dropDatabase)
        {
            if (dropDatabase)
            {
                databaseContext = new SearchEngineDatabaseContext(dropDatabase);
                documentsInDatabase = new Dictionary<String, Document>(StringComparer.InvariantCultureIgnoreCase);
                termsInDatabase = new Dictionary<String, Term>(StringComparer.InvariantCultureIgnoreCase);
            }
            else
            {
                //Don't drop database and populate the Dictionaries with the data in the database
                databaseContext = new SearchEngineDatabaseContext(false);
                documentsInDatabase = GetDocumentsTitleAndId();
                termsInDatabase = GetTermsValueAndId();
            }
        }

        /// <summary>
        /// Gets a Dictionary (HashMap) with Term Value as key and the Term as Value
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Term> GetTermsValueAndId()
        {
            Dictionary<string, Term> result = new Dictionary<String, Term>(StringComparer.InvariantCultureIgnoreCase);
            List<Term> terms = databaseContext.Terms.ToList();
            foreach(Term term in terms)
            {
                result.Add(term.Value, term);
            }

            return result;
        }

        /// <summary>
        /// Gets a Dictionary (HashMap) with Document Title as key and the Document as Value
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, Document> GetDocumentsTitleAndId()
        {
            Dictionary<string, Document> result = new Dictionary<String, Document>(StringComparer.InvariantCultureIgnoreCase);
            List<Document> documents = databaseContext.Documents.ToList();
            foreach (Document doc in documents)
            {
                result.Add(doc.Title, doc);
            }

            return result;
        }

        /// <summary>
        /// Inserts new Document in database
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private Document InsertDocument(string title)
        {
            databaseContext.Documents.Add(new Document { Title = title , DateCreated = DateTime.Now});
            databaseContext.SaveChanges();
            return databaseContext.Documents.Where(d => d.Title == title).First();
        }

        /// <summary>
        /// Inserts new Term in database
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private Term InsertTerm(string value)
        {
            databaseContext.Terms.Add(new Models.Term { Value = value });
            databaseContext.SaveChanges();
            return databaseContext.Terms.Where(t => t.Value == value).First();
        }

        /// <summary>
        /// Inserts new Index in database
        /// </summary>
        /// <param name="position"></param>
        /// <param name="documentId"></param>
        /// <param name="termId"></param>
        private void InsertIndex(int position, int documentId, int termId)
        {
            databaseContext.Indexes.Add(new Index { DocumentId = documentId, TermId = termId, Position = position });
            databaseContext.SaveChanges();
        }

        /// <summary>
        /// Creates a new Index entry in the database. It checks for already existing entries for both document and term (in memory checks).
        /// </summary>
        /// <param name="position">The position of the term in the document</param>
        /// <param name="term">The term</param>
        /// <param name="document">The document as a url</param>
        public void IndexTerm(int position, string term, string document)
        {
            if (term == null || term.Equals("") || document == null || document.Equals("") || position < 0)
            {
                return;
            }
            int termId;
            int documentId;

            //Check if term is in database, if not then save it and get the id, else just get the id
            if (!termsInDatabase.ContainsKey(term))
            {
                Term termModel = InsertTerm(term);
                termId = termModel.TermId;
                termsInDatabase.Add(term, termModel);
            }
            else
            {
                termId = termsInDatabase.Where(t => t.Key.Equals(term, StringComparison.InvariantCultureIgnoreCase)).First().Value.TermId;
            }

            //Check if the document is in the database, if not save it and get the id, else just get the id
            if (!documentsInDatabase.ContainsKey(document))
            {
                Document documentModel = InsertDocument(document);
                documentId = documentModel.DocumentId;
                documentsInDatabase.Add(document, documentModel);
            }
            else
            {
                documentId = documentsInDatabase.Where(d => d.Key.Equals(document, StringComparison.InvariantCultureIgnoreCase)).First().Value.DocumentId;
            }

            InsertIndex(position, documentId, termId);
        }

        //Used for Search Engine User End
        public bool IsTermInDatabase(string term, out int id)
        {
            bool result = false;

            result = databaseContext.Terms.Any(t => t.Value == term);
            if (result)
            {
                id = databaseContext.Terms.Where(t => t.Value == term).First().TermId;
            }
            else
            {
                id = -1;
            }

            return result;
        }

        /// <summary>
        /// Get all indices with a given term
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<Index> GetIndexFromTermValue(string value)
        {
            int termId = databaseContext.Terms.Where(t => t.Value == value).First().TermId;
            List<Index> indexes = databaseContext.Indexes.Where(i => i.TermId == termId).ToList();
            return indexes;
        }

        /// <summary>
        /// Get all documents with a given documentId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Document> GetDocumentFromId(int id)
        {
            return databaseContext.Documents.Where(d => d.DocumentId == id).ToList();
        }

        /// <summary>
        /// Gets the position given a document id and term (This may not be used later on).
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public List<int> GetPositionsFromDocumentIdAndTerm(int documentId, string term)
        {
            int termId = -1;
            if (IsTermInDatabase(term, out termId))
            {
                return databaseContext.Indexes.Where(i => i.DocumentId == documentId && i.TermId == termId).Select(i => i.Position).ToList();
            }
            return null;
        }
    }
}
