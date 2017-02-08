using DatabaseLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexEngine
{
    public class Indexer : Observable
    {
        public bool HasNext { get; set; }
        public int DirectoryIndex { get; set; }
        public List<string> SearchDirectories { get; set; }
        private SearchEngineDatabaseManager DatabaseManager;
        private Dictionary<string, int> termsInDatabase = new Dictionary<string, int>();
        public List<Observer> Observers { get; set; }
        public string StatusText { get; set; }
        public DateTime FileStartTime { get; set; }
        public DateTime FileEndTime { get; set; }
        public DateTime IndexerStartTime { get; set; }
        public DateTime IndexerEndTime { get; set; }
        public DateTime DirectoryStartTime { get; set; }
        public DateTime DirectoryEndTime { get; set; }

        public Indexer(List<string> searchDirectories, bool dropDatabase)
        {
            StatusText = "Indexer initialized...";
            DatabaseManager = new SearchEngineDatabaseManager(dropDatabase);

            SearchDirectories = searchDirectories;
            Observers = new List<Observer>();
            DirectoryIndex = 0;
            if (SearchDirectories.Count > 0)
            {
                HasNext = true;
            }
            else
            {
                HasNext = false;
            }
            IndexerStartTime = DateTime.Now;
        }

        public void AddObserver(Observer observer)
        {
            Observers.Add(observer);
        }

        public void RemoveObserver(Observer observer)
        {
            Observers.Remove(observer);
        }

        public void NotifyObservers(object message)
        {
            foreach (Observer observer in Observers)
            {
                observer.Notify(message);
            }
        }

        /// <summary>
        /// Indexes the next element in the list.
        /// </summary>
        public void Next()
        {
            // Index the current directory
            IndexDirectory(SearchDirectories.ElementAt(DirectoryIndex));
            DirectoryIndex++;
            if (DirectoryIndex < SearchDirectories.Count - 1)
            {
                HasNext = true;
            }
            else
            {
                HasNext = false;
                IndexerEndTime = DateTime.Now;
                TimeSpan indexerTime = IndexerEndTime - IndexerStartTime;
                StatusText = "Indexer took: " + indexerTime.Milliseconds + " millisecond(s).";
                NotifyObservers(StatusText);
                Console.WriteLine(StatusText);
            }
        }

        private void IndexDirectory(string directory)
        {
            DirectoryStartTime = DateTime.Now;
            StatusText = "Indexing directory: " + directory;
            NotifyObservers(StatusText);
            Console.WriteLine(StatusText);
            string[] elementsInDirectory = System.IO.Directory.GetFiles(directory, "*.*", System.IO.SearchOption.AllDirectories);
            
            //Index the files one by one from the elementsInDirecotry array.
            foreach (var file in elementsInDirectory)
            {
                //Console.WriteLine("Indexing file: " + file);
                IndexFile(directory, file);
            }
            DirectoryEndTime = DateTime.Now;
            TimeSpan directoryTime = DirectoryEndTime - DirectoryStartTime;
            StatusText = "Directory took: " + directoryTime.Milliseconds + " millisecond(s).";
            NotifyObservers(StatusText);
            //Console.WriteLine(StatusText);
        }

        private void IndexFile(string directory, string file)
        {
            FileStartTime = DateTime.Now;
            StatusText = "Indexing file: " + file;
            NotifyObservers(StatusText);
            Console.WriteLine(StatusText);
            int positionInFile = 0;
            FileInfo info = new FileInfo(file);
            if (info.Exists)
            {
                using (StreamReader streamReader = new StreamReader(info.FullName))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string readLine = streamReader.ReadLine();
                        //Console.WriteLine("Indexing line: " + readLine);
                        positionInFile = IndexLine(readLine, info.FullName, positionInFile);
                    }
                }
            }
            FileEndTime = DateTime.Now;
            TimeSpan fileIndexTime = FileEndTime - FileStartTime;
            StatusText = "Took: " + fileIndexTime.Milliseconds + " millisecond(s).";
            NotifyObservers(StatusText);
            //Console.WriteLine(StatusText);
        }

        private int IndexLine(string line, string fileNameIncludingPath, int positionInFile)
        {
            line = line.Replace('\t', ' ');
            string[] terms = line.Split(' ');
            int lastPositionUsed = positionInFile;

            foreach(string term in terms)
            {
                //Console.WriteLine("Indexing term: " + term);
                DatabaseManager.IndexTerm(lastPositionUsed, term, fileNameIncludingPath);
                lastPositionUsed += term.Length;
                lastPositionUsed++;
            }

            return lastPositionUsed;
        }
    }
}
