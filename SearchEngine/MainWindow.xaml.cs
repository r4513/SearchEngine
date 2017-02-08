using DatabaseLib;
using DatabaseLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SearchEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string SearchString { get; set; }
        public SearchEngineDatabaseManager DatabaseManager { get; set; }
        public List<Document> Documents { get; set; }
        public List<Index> Indexes { get; set; }
        public List<string> SearchList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DatabaseManager = new SearchEngineDatabaseManager(false);
            SearchList = new List<string>();
            articleView.ItemsSource = SearchList;
        }

        private void onSearchButtonClicked(object sender, RoutedEventArgs e)
        {
            SearchList.Clear();
            SearchString = searchBox.Text;
            string[] terms = SearchString.Split(' ');
            Documents = new List<Document>();
            Indexes = new List<Index>();

            //Search in database for terms
            foreach (string term in terms)
            {
                Indexes = DatabaseManager.GetIndexFromTermValue(term);
                foreach (Index i in Indexes)
                {
                    Documents.AddRange(DatabaseManager.GetDocumentFromId(i.DocumentId));
                }

                List<Document> documentsAllreadyLookedAt = new List<Document>();
                foreach (Document doc in Documents)
                {
                    if (!documentsAllreadyLookedAt.Contains(doc))
                    {
                        string searchResult = doc.Title;
                        List<int> positions = DatabaseManager.GetPositionsFromDocumentIdAndTerm(doc.DocumentId, term);
                        searchResult += "\nAt positions[";
                        foreach (int pos in positions)
                        {
                            searchResult += pos + ",";
                        }
                        searchResult += "]\n\n";
                        SearchList.Add(searchResult);
                        documentsAllreadyLookedAt.Add(doc);
                    }
                }
            }
            
            articleView.ItemsSource = SearchList;
            articleView.Items.Refresh();
        }

        private void onReadButtonClicked(object sender, RoutedEventArgs e)
        {
            //Get selected article from articleView and open it
        }
    }
}
