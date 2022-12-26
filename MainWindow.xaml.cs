using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Input;
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
                Label1.Content = copyMake && path != null ? Path.GetFileNameWithoutExtension(path) + " — copy" : Path.GetFileNameWithoutExtension(path);
            }
            catch (Exception ex)
            {

                Label1.Content = ex.Message;
            }
            
        }
        public int DefineType() 
        {
            if ((bool)SecondType.IsChecked) 
                return (int)SortType.SecondWord;
            else
                return (int)SortType.FirstWord;  
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

                Label1.Content = ex.Message;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            try
            {
                string ext = System.IO.Path.GetExtension(path);
                ListUpdateByTextBox();
                Sorting.Sort(ref words, DefineType());
                if (ext != ".txt")
                {
                    path = "Invalid Type";
                    throw new InvalidOperationException("Invalid Type");
                }

                if (copyMake)
                {
                    path = path.Substring(0, path.Length - ext.Length)+ " — copy.txt";
                }
                   
               
                using (StreamWriter writer = new StreamWriter(path, false))
                {     
                    foreach (var item in words)
                    {
                        writer.WriteLine(item);

                    }

                }
                WordsShow();
                Label1.Content = "Words sort is successful";
            }
            catch (Exception ex)
            {
           
                Label1.Content = ex.Message;
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
            Label1.Content = "DROP IT";
        }

        private void layout2_DragLeave(object sender, DragEventArgs e)
        {
            Label1.Content = copyMake && path != null ? Path.GetFileNameWithoutExtension(path) + " — copy" : Path.GetFileNameWithoutExtension(path);
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
            UncheckedAllExpectOne(sender,GroupBoxType);
          

        }

        private void CopyCheckBox(object sender, RoutedEventArgs e)
        {
            copyMake = !copyMake;
            Label1.Content = copyMake && path != null ? Path.GetFileNameWithoutExtension(path) + " — copy" : Path.GetFileNameWithoutExtension(path);
        }

        private void WordsShow()
        {
            int it = 0;
            foreach (var item in (StackPanelWords).Children)
            {
                if (item is TextBox)
                {
                    TextBox tb = (TextBox)item;
                    tb.Text = words[it];
                    it++;
                }
            }
            for (; it < words.Count(); it++)
            {
                TextBox tx = new TextBox();
               
                tx.Width = 220; tx.Height = 20;
                tx.Margin = new Thickness(-90, 5, 0, 0);
                tx.Text = words[it];
               
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
    }
}
