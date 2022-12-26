using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
    
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();
            string line;
            try
            {
                string ext = System.IO.Path.GetExtension(path);
                if (ext!= ".txt")
                {
                    path =  "Invalid Type";
                    throw new InvalidOperationException("Invalid Type");
                }
                using (StreamReader reader = new StreamReader(path))
                {
                    line = reader.ReadLine();
                    list.Add(line);
                    while (line != null)
                    {


                        line = reader.ReadLine();
                        list.Add(line);


                    }

                }

                Sorting.Sort(ref list, DefineType());


                if (copyMake)
                {
                    path = path.Substring(0, path.Length - ext.Length)+ " — copy.txt";
                }
                   
               
                using (StreamWriter writer = new StreamWriter(path, false))
                {     
                    foreach (var item in list)
                    {
                        writer.WriteLine(item);

                    }

                }
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
            Label1.Content = path;
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

        private void typeBoxes(object sender, RoutedEventArgs e)
        {
            UncheckedAllExpectOne(sender,GroupBoxType);
          

        }

        private void CopyCheckBox(object sender, RoutedEventArgs e)
        {
            copyMake = !copyMake;
            Label1.Content = copyMake && path != null ? Path.GetFileNameWithoutExtension(path) + " — copy" : Path.GetFileNameWithoutExtension(path);
        }
    }
}
