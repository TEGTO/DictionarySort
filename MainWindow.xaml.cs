using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xaml;
using static System.Net.Mime.MediaTypeNames;

public enum SortType
{
    FirstWord, SecondWord
}
namespace DictionarySort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path;
        bool copyMake=false;
        uint sortType = (int)SortType.FirstWord;
        IInputElement lastFocusedTextBox;
        List<string> words;
     

        public MainWindow()
        {
            InitializeComponent();
        }
        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    path = files[0];



                }
              
                layout2_DragLeave(sender, e);
                FileRead();
                WordsShow();
               
            }
            catch (Exception ex)
            {

                PathLabel.Content = ex.Message;
            }
            
        }
      
        
        void FileRead()
        {
            words = new List<string>();
            string line;
            try
            {
                string ext = System.IO.Path.GetExtension(path);
                if (ext != ".txt")
                {
                    path = "Invalid Type";
                    throw new InvalidOperationException("Invalid Type");
                }
                using (StreamReader reader = new StreamReader(path))
                {
                    line = reader.ReadLine();
                    words.Add(line);
                    while (line != null)
                    {


                        line = reader.ReadLine();
                        words.Add(line);
                    }

                }
                words = words.Where(x => x != "" && x != null).ToList();
               
            }
            catch (Exception ex)
            {

                PathLabel.Content = ex.Message;
            }
        }
        private void StartSorting()
        {
           
            try
            {
               
                ListUpdateByTextBox();
                Sorting.Sort(ref words, sortType);
                WordsShow();
                
            }
            catch (Exception ex)
            {
           
                PathLabel.Content = path == null? "File is empty. Enter the file.": ex.Message;
            }

        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            try
            {
                string ext = System.IO.Path.GetExtension(path);
               
                
                if (ext != ".txt")
                {
                    path = "Invalid Type";
                    throw new InvalidOperationException("Invalid Type");
                }

                if (copyMake)
                {
                    path = path.Substring(0, path.Length - ext.Length) + " — copy.txt";
                }


                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    foreach (var item in words)
                    {
                        writer.WriteLine(item);

                    }

                }
                WordsShow();
                PathLabel.Content = "Words sort is successful";
            }
            catch (Exception ex)
            {

                PathLabel.Content = ex.Message;
            }
        }




       private void ImagePanel_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (e.Effects == DragDropEffects.Copy)
            {
                e.UseDefaultCursors = false;
                Mouse.SetCursor(Cursors.Hand);
            }
            else
                e.UseDefaultCursors = true;

            e.Handled = true;
        }

     

        private void layout2_DragEnter(object sender, DragEventArgs e)
        {

            layout2.Opacity = 0.5;      
            e.Effects = DragDropEffects.None; 
            PathLabel.Content = "DROP IT";
        }

        private void layout2_DragLeave(object sender, DragEventArgs e)
        {
            LabelsFill();
            layout2.Opacity =1;
        }
        private void UncheckedAllExpectOne(object sender,GroupBox gB)
        {
            CheckBox checkBox = (CheckBox)sender; //check box that need to hold checked status
            foreach (var item in ((StackPanel)gB.Content).Children)
            {
                if (item is CheckBox && item != checkBox)
                {
                    CheckBox cb = (CheckBox)item;
                    cb.IsChecked = false;
                }
            }
        }
      
        private void TypeBoxes(object sender, RoutedEventArgs e)
        {
          //  UncheckedAllExpectOne(sender,GroupBoxType);
          

        }

        private void CopyCheckBox(object sender, RoutedEventArgs e)
        {
            copyMake = !copyMake;
            LabelsFill();
        }

        private void WordsShow()
        {
            int it = 0;
            LabelsFill();
            foreach (var item in (StackPanelWords).Children)
            {
                if (item is TextBox)
                {
                    TextBox tb = (TextBox)item;
                    tb.Text = words[it];
                    it++;
                }
            }
            for (; it < words?.Count(); it++)
            {
                TextBox tx = new TextBox();
        
                tx.Width = 220; tx.Height = 20;
                tx.Margin = new Thickness(-90, 5, 0, 0);
                tx.Text = words[it];
                tx.PreviewMouseDown += MouseDownTextBox;
                tx.PreviewMouseUp += MouseDownTextBox;
                StackPanelWords.Children.Add(tx);
            }
         
        }

        private void ListUpdateByTextBox()
        {
            int it = 0;
            foreach (var item in (StackPanelWords).Children)
            {
                if (item is TextBox)
                {
                    TextBox tb = (TextBox)item;
                    words[it] = tb.Text;
                    it++;
                }
            }
        }
        void LabelsFill()
        {
            PathLabel.Content = (copyMake && path != null ? System.IO.Path.GetFileNameWithoutExtension(path) + " — copy" : System.IO.Path.GetFileNameWithoutExtension(path));
            WordsCountLabel.Content = "Word Count: " + (words?.Count??0);
        }
        private void SortByFirstWordButton(object sender, RoutedEventArgs e)
        {
            sortType = (int)SortType.FirstWord;
            SortByFirstWord.Content = Sorting.reverseSortFirst ? "↑" : "↓";
            StartSorting();
        


        }

        private void SortBySecondWordButton(object sender, RoutedEventArgs e)
        {
            sortType = (int)SortType.SecondWord;
            SortBySecondWord.Content = Sorting.reverseSortSecond ? "↑" : "↓";
            StartSorting();
           
        }

        private void AddWordButton(object sender, RoutedEventArgs e)
        {
            if (path!=null)
            {
                TextBox tx = new TextBox();
                tx.Width = 220; tx.Height = 20;
                tx.Margin = new Thickness(-90, 5, 0, 0);
                tx.Text = "";
                words.Add(tx.Text);
                StackPanelWords.Children.Add(tx);
                LabelsFill();
            }
            else
                PathLabel.Content =  "File is empty. Enter the file.";

        }
        private void MouseDownTextBox(object sender, RoutedEventArgs e)
        {
            lastFocusedTextBox = FocusManager.GetFocusedElement(this);
            if (lastFocusedTextBox != null)
            {
                TextBox tb = lastFocusedTextBox as TextBox;
                if (tb !=null)
                    PathLabel.Content = $"Selected: {tb?.Text}";

            }
        }
      
        private void RemoveWordButton(object sender, RoutedEventArgs e)
        {
            if (lastFocusedTextBox !=null)
            {
                TextBox tb = lastFocusedTextBox as TextBox;
                words.Remove(tb.Text);
                StackPanelWords.Children.Remove(tb);
                LabelsFill();
            }
            
        }
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastFocusedTextBox = null;
            LabelsFill();
        }
    }
}
