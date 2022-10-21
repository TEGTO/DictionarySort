using System;
using System.Collections.Generic;
using System.IO;
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

namespace DictionarySort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string path = "Invalid Type";
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ImagePanel_Drop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                   
                    path = files[0];
                   
             
                
            }
        
            layout2_DragLeave(sender, e);
            Label1.Content = path;
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

                list.Sort();
                list = list.Where(x => x != null).ToList();
               
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
            Label1.Content = "";
            layout2.Opacity =1;
        }

    }
}
