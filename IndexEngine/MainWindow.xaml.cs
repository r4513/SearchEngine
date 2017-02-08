using DatabaseLib;
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
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;
using static IndexEngine.Indexer;
using System.Windows.Threading;
using System.ComponentModel;

namespace IndexEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Observer
    {
        public List<string> SearchDirectories { get; set; }
        private string IndexDirectoriesFile = Environment.CurrentDirectory + @"\SavedDirectories.dat";
        public string TitleString { get; set; }
        public bool DropDatabaseChecked { get; set; }
        public Indexer Indexer { get; set; }
        public string StatusText { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            SearchDirectories = new List<string>();
            TitleString = "IndexEngine";
            StatusText = "Not running...";
            

            if (initializeSearchDirectories() > 0){
                indexButton.IsEnabled = true;
                directoryView.ItemsSource = SearchDirectories;
            }
            else
            {
                indexButton.IsEnabled = false;
                directoryView.ItemsSource = null;
            }
        }

        /// <summary>
        /// Initializes the SearchDirectories List with directories saved in the save file IndexDirectoriesFile.
        /// </summary>
        /// <returns>Number of directories added.</returns>
        private int initializeSearchDirectories()
        {
            if (!File.Exists(IndexDirectoriesFile))
            {
                StreamWriter dummy = File.CreateText(IndexDirectoriesFile);
                dummy.Close();
            }
            StreamReader streamReader = new StreamReader(IndexDirectoriesFile);
            while (!streamReader.EndOfStream)
            {
                SearchDirectories.Add(streamReader.ReadLine());
            }

            return SearchDirectories.Count;
        }

        private void onIndexButtonClicked(object sender, RoutedEventArgs e)
        {
            DropDatabaseChecked = dropDatabaseCheckBox.IsChecked.Value;
            Indexer = new Indexer(SearchDirectories, DropDatabaseChecked);
            Indexer.AddObserver(this);
            while (Indexer.HasNext)
            {
                Indexer.Next();
            }
        }

        public void Notify(object message)
        {
            //Console.WriteLine("Updating statusBox with: " + (string)message);
            StatusText = (string)message;
            statusBox.Text = StatusText;
        }

        private void onAddButtonClicked(object sender, RoutedEventArgs e)
        {
            var directoryToAddToIndexingList = new System.Windows.Forms.FolderBrowserDialog();
            directoryToAddToIndexingList.ShowNewFolderButton = false;
            directoryToAddToIndexingList.Description = "Add directory to be indexed.";
            DialogResult resultOfDialog = directoryToAddToIndexingList.ShowDialog();

            if (resultOfDialog == System.Windows.Forms.DialogResult.OK)
            {
                //Handle file chosen
                SearchDirectories.Add(directoryToAddToIndexingList.SelectedPath);
            }else
            {
                //Handle no file chosen
            }

            if (SearchDirectories.Count > 0)
            {
                //Handle one or directories to show in directoriesView
                indexButton.IsEnabled = true;
                directoryView.ItemsSource = SearchDirectories;
            }
            else
            {
                //Handle no more directories to show in direcotiresView
                indexButton.IsEnabled = false;
                directoryView.ItemsSource = null;
            }
            directoryView.Items.Refresh();
        }

        private void onCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            CloseApplication();
        }

        private void CloseApplication()
        {
            writeDirectoriesToIndexToFile();
            Environment.Exit(0);
        }

        private void writeDirectoriesToIndexToFile()
        {
            StreamWriter streamWriter = new StreamWriter(IndexDirectoriesFile, false);
            foreach(string directory in SearchDirectories)
            {
                streamWriter.WriteLine(directory);
                //streamWriter.Flush();
            }
            streamWriter.Close();
        }

        private void onCloseButtonClicked(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void onRemoveButtonClicked(object sender, RoutedEventArgs e)
        {
            string selectedDirectory = (string) directoryView.SelectedValue;
            SearchDirectories.Remove(selectedDirectory);
            directoryView.ItemsSource = SearchDirectories;
            directoryView.Items.Refresh();
        }
    }
}
