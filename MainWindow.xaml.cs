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
        bool copyMake=false , ctrlIsPressed = false, shiftIsPressed = false;
        uint sortType = (int)SortType.FirstWord;
        IInputElement lastFocusedTextBox;
        IInputElement[] lastFocusedTextBoxes;
        List<string> words;
     

        public MainWindow()
        {
            lastFocusedTextBoxes = new IInputElement[0];
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

                //if (copyMake)
                //{
                //    path = path.Substring(0, path.Length - ext.Length) + " — copy.txt";
                //}


                using (StreamWriter writer = new StreamWriter(path, false))
                {
                    foreach (var item in words)
                    {
                        writer.WriteLine(item);

                    }

                }
                WordsShow();
                PathLabel.Content = "File is Saved";
            }
            catch (Exception ex)
            {

                PathLabel.Content = ex.Message;
            }
        }
        private void SaveFileAs(object sender, RoutedEventArgs e)
        {

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
            
                tx.PreviewMouseUp += MouseUpTextBox;
             

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
        private void MouseUpTextBox(object sender, RoutedEventArgs e)
        {
            if (ctrlIsPressed)
            {
               
                    lastFocusedTextBox = FocusManager.GetFocusedElement(this);
                    TextBox tb = lastFocusedTextBox as TextBox;
                    if(RemoveWordSelect(lastFocusedTextBox))
                    {
                        Array.Resize(ref lastFocusedTextBoxes, lastFocusedTextBoxes.Length + 1);
                        lastFocusedTextBoxes[lastFocusedTextBoxes.Length - 1] = FocusManager.GetFocusedElement(this);
                        tb.Background = (Brush)new BrushConverter().ConvertFrom("#FF99C9EF");

                  
                    }
                PathLabel.Content = $"Selected: {lastFocusedTextBoxes.Length} words";



            }
            else
            {
                DeleteSelections();
                lastFocusedTextBoxes = new IInputElement[0];
                if (!ShiftSelection(sender,e))
                {
                  
                    lastFocusedTextBox = FocusManager.GetFocusedElement(this);
                    if (lastFocusedTextBox != null)
                    {
                        TextBox tb = lastFocusedTextBox as TextBox;
                        if (tb != null)
                            PathLabel.Content = $"Selected: {tb?.Text}";

                    }
                }
                else
                      PathLabel.Content = $"Selected: {lastFocusedTextBoxes.Length} words";
            
                  
                
                
              
            }
           
        }
        private bool ShiftSelection(object sender, RoutedEventArgs e)
        {
            if (shiftIsPressed&&lastFocusedTextBox !=null)
            {
                TextBox lastFocusedElement = lastFocusedTextBox as TextBox;
               
             
        
                bool check = false, check1 = false;
                foreach (var item in (StackPanelWords).Children)
                {
                    if (item is TextBox)
                    {
                        TextBox tb = (TextBox)item;
                        if (tb == lastFocusedElement || tb == FocusManager.GetFocusedElement(this) as TextBox)
                        {
                            check = true;
                            check1 = lastFocusedTextBox == FocusManager.GetFocusedElement(this)?false: !check1; 
                        }
                                       
                    
                        if (check)
                        {
                            
                                Array.Resize(ref lastFocusedTextBoxes, lastFocusedTextBoxes.Length + 1);
                                lastFocusedTextBoxes[lastFocusedTextBoxes.Length - 1] = (IInputElement)tb;
                                tb.Background = (Brush)new BrushConverter().ConvertFrom("#FF99C9EF");
                            
                        }
                        if ((tb == FocusManager.GetFocusedElement(this) as TextBox || tb == lastFocusedElement) && check&&!check1) break;
                       
                    }
                    
                }
              
                return lastFocusedTextBoxes.Length > 0;
            }
            else
            {
                return false;
            }
            
        }
        private void KeyDownTextBox(object sender, KeyEventArgs e) {
            switch (e.Key)
            {
                case Key.LeftCtrl:
                    ctrlIsPressed = true;
                    break;
                case Key.LeftShift:
                    shiftIsPressed = true;
                    break;
                default:
                    break;
            }
        }
        private void KeyUpTextBox(object sender, KeyEventArgs e) {
            switch (e.Key)
            {
                case Key.LeftCtrl:
                    ctrlIsPressed = false;
                    break;
                case Key.LeftShift: 
                    shiftIsPressed = false;
                    break;
                default:
                    break;
            }      

        }
        private bool RemoveWordSelect(IInputElement el)
        {
            if (lastFocusedTextBoxes.Any(x=>x==el))
            {
                TextBox tb = el as TextBox;
                tb.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                lastFocusedTextBoxes = lastFocusedTextBoxes.Where(x => x != el).ToArray();
                return false;
            }
            return true;
        }
        private void DeleteSelections()
        {
            foreach (var item in StackPanelWords.Children)
            {
                if (item is TextBox)
                {
                    TextBox tb = (TextBox)item;  
                    tb.Background = (Brush)new BrushConverter().ConvertFrom("#FFFFFFFF");
                }
            }
        }
        private void RemoveWordButton(object sender, RoutedEventArgs e)
        {
            if (lastFocusedTextBoxes.Length >0)
            {
                TextBox tb;
                foreach (var word in lastFocusedTextBoxes)
                {
                    tb = word as TextBox;
                    words.Remove(tb.Text);
                    StackPanelWords.Children.Remove(tb);
                   
                }
                LabelsFill();
               
            }
            else if (lastFocusedTextBox !=null)
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
            lastFocusedTextBoxes = new IInputElement[0];
            LabelsFill();
            DeleteSelections();
        }
    }
}
