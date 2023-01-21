using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        bool copyMake=false , ctrlIsPressed = false, shiftIsPressed = false, isFileChanged =false;
        uint sortType = (int)SortType.FirstWord;
        IInputElement lastFocusedTextBox;
        IInputElement[] lastFocusedTextBoxes;
        List<string> words;
        

        public MainWindow()
        {
            words = new List<string>();
            lastFocusedTextBoxes = new IInputElement[0];
            InitializeComponent();
            PathSaveRead();


        }

        //File Read Functions 
        void PathSaveRead()
        {
            if (File.Exists("filePath.txt"))
            {
                using (StreamReader reader = new StreamReader("filePath.txt"))
                {
                    bool r = bool.TryParse(reader.ReadLine(), out r)? r : true;
                    PathSave.IsChecked = r;
                    if (PathSave.IsChecked)
                    {
                        path = reader.ReadLine();
                        if (path?.Length > 0)
                        {
                            fileRead();
                            wordsShow();
                        }

                    }
                }
            }
        }
        private void dragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    path = files[0];



                }
              
                layout2_DragLeave(sender, e);
                fileRead();
                wordsShow();
               
            }
            catch (Exception ex)
            {

                PathLabel.Content = ex.Message;
            }
            
        }
        private void openFileDialog(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (openFileDialog.ShowDialog() == true)
                    path = openFileDialog.FileName;
                fileRead();
                wordsShow();
            }
            catch (Exception ex)
            {
                PathLabel.Content = ex.Message;
               
            }
          
        }
        void fileRead()
        {
            words = new List<string>();
            string line;
            try
            {
                string ext = System.IO.Path.GetExtension(path);
                if (ext != ".txt")
                {
                    path = "";
                    throw new InvalidOperationException("");
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

        //File Save Functions
        private void saveFile(object sender, RoutedEventArgs e)
        {
            try
            {
                listUpdateByTextBox();
                if (path == null || path?.Length <= 0)
                {
                    saveFileDialog(sender, e);
                    return;
                }
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
                wordsShow();
                isFileChanged = false;
                PathLabel.Content = "File is Saved";
            }
            catch (Exception ex)
            {

                PathLabel.Content = ex.Message;
            }
        }
        private void saveFileDialog(object sender, RoutedEventArgs e)
        {
            try
            {
                listUpdateByTextBox();
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Text file (*.txt)|*.txt";
                saveFileDialog.InitialDirectory=path?.Length>0&&path!=null ?path : Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (saveFileDialog.ShowDialog() == true)
                {
                    path = saveFileDialog.FileName;
                    string ext = System.IO.Path.GetExtension(saveFileDialog.FileName);

                    if (ext != ".txt")
                    {
                        path = "Invalid Type";
                        throw new InvalidOperationException("Invalid Type");
                    }

                    using (StreamWriter writer = new StreamWriter(path, false))
                    {
                        foreach (var item in words)
                        {
                            writer.WriteLine(item);

                        }

                    }
                    wordsShow();
                    PathLabel.Content = "File is Saved";
                    isFileChanged = false;




                }
            }
            catch (Exception ex)
            {

                PathLabel.Content = ex.Message;
            }
          
        }

        //Sorting & Showing Functions 
        private void startSorting()
        {
           
            try
            {
              
                if (words.Count<=0)
                {
                    throw new InvalidOperationException("Error");
                }
                isFileChanged = true;
                listUpdateByTextBox();
                Sorting.sort(ref words, sortType);
                wordsShow();
                
            }
            catch (Exception ex)
            {
           
                PathLabel.Content = path == null || words.Count <= 0? "File is empty. Enter the file.": ex.Message;
            }

        }
        private void wordsShow()
        {
            int it = 0, start = 0, count = 0;
            labelsFill();
            foreach (var item in (StackPanelWords).Children)
            {
                if (item is TextBox && it <words.Count)
                {
                    TextBox tb = (TextBox)item;        
                    tb.Text = words[it];
                    it++;
                    start++;
                }
                else if (item is TextBox)
                {
                    count++;
    
                }
            }
            if (count!=0)
                StackPanelWords.Children.RemoveRange(start, count);
       
           
            for (; it < words?.Count(); it++)
            {
                TextBox tx = addNewTextBox(words[it]);       
                StackPanelWords.Children.Add(tx);
            }

        }
        TextBox addNewTextBox(string text)
        {
            TextBox tx = new TextBox();
            tx.Text = text;
            tx.Width = 220; tx.Height = 20;
            tx.Margin = new Thickness(-90, 5, 0, 0);
            tx.PreviewMouseUp += mouseUpTextBox;
            tx.TextChanged += textChangedEventHandler;
            return tx;
        }
        private void listUpdateByTextBox()
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
        void labelsFill()
        {
            PathLabel.Content = (copyMake && path != null ? System.IO.Path.GetFileNameWithoutExtension(path) + " — copy" : System.IO.Path.GetFileNameWithoutExtension(path));
            WordsCountLabel.Content = "Word Count: " + (words?.Count??0);
        }
        private void sortByWordButton(object sender, RoutedEventArgs e)
        {
         
            if (sender == SortByFirstWord)
            {
                sortType = (int)SortType.FirstWord;
                SortByFirstWord.Content = Sorting.reverseSortFirst ? "↑" : "↓";
            }
            else
            {
                sortType = (int)SortType.SecondWord;
                SortBySecondWord.Content = Sorting.reverseSortSecond ? "↑" : "↓";
            }
            startSorting();
        }


        private void addWordButton(object sender, RoutedEventArgs e)
        {
            isFileChanged = true;
                TextBox tx = addNewTextBox("");        
                words.Add(tx.Text);
                StackPanelWords.Children.Add(tx);
                labelsFill();
           

        }
        private void removeWordButton(object sender, RoutedEventArgs e)
        {

            if (lastFocusedTextBoxes.Length > 0)
            {
                isFileChanged = true;
                TextBox tb;
                foreach (var word in lastFocusedTextBoxes)
                {
                    tb = word as TextBox;
                    words.Remove(tb.Text);
                    StackPanelWords.Children.Remove(tb);

                }
                labelsFill();

            }
            else if (lastFocusedTextBox != null)
            {
                isFileChanged = true;
                TextBox tb = lastFocusedTextBox as TextBox;
                words.Remove(tb.Text);
                StackPanelWords.Children.Remove(tb);
                labelsFill();
            }

        }

        private void newFileButton(object sender, RoutedEventArgs e)
        {
            path = String.Empty;
            words = new List<string>();
            lastFocusedTextBoxes = new IInputElement[0];
            lastFocusedTextBox = null;
            StackPanelWords.Children.Clear();
            PathLabel.Content = path;
            WordsCountLabel.Content = "Words Count: "+words.Count;
            wordsShow();
        }

        //Visuals Functions
       private void imagePanel_GiveFeedback(object sender, GiveFeedbackEventArgs e)
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
            foreach (var item in (StackPanelWords).Children)
            {
                if (item is TextBox)
                {
                    TextBox tx = item as TextBox;
                    tx.IsHitTestVisible = false;
                }
            }
            
           
        }

        private void layout2_DragLeave(object sender, DragEventArgs e)
        {
            
            labelsFill();
            layout2.Opacity =1;
            foreach (var item in (StackPanelWords).Children)
            {
                if (item is TextBox)
                {
                    TextBox tx = item as TextBox;
                    tx.IsHitTestVisible = true;
                }
            }
        }
        private void uncheckedAllExpectOne(object sender,GroupBox gB)
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
          //  UncheckedAllExpectOne(sender,GroupBoxType);
          

        }

        private void copyCheckBox(object sender, RoutedEventArgs e)
        {
            copyMake = !copyMake;
            labelsFill();
        }

        private void textChangedEventHandler(object sender, TextChangedEventArgs args) => isFileChanged = true;
        
        private void mouseUpTextBox(object sender, RoutedEventArgs e)
        {
            if (ctrlIsPressed)
            {
               
                    lastFocusedTextBox = FocusManager.GetFocusedElement(this);
                    TextBox tb = lastFocusedTextBox as TextBox;
                    if(removeWordSelect(lastFocusedTextBox))
                    {
                        Array.Resize(ref lastFocusedTextBoxes, lastFocusedTextBoxes.Length + 1);
                        lastFocusedTextBoxes[lastFocusedTextBoxes.Length - 1] = FocusManager.GetFocusedElement(this);
                        tb.Background = (Brush)new BrushConverter().ConvertFrom("#FF99C9EF");

                  
                    }
                PathLabel.Content = $"Selected: {lastFocusedTextBoxes.Length} words";



            }
            else
            {
                deleteSelections();
                lastFocusedTextBoxes = new IInputElement[0];
                if (!shiftSelection(sender,e))
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
        private bool shiftSelection(object sender, RoutedEventArgs e)
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
        private void keyDownTextBox(object sender, KeyEventArgs e) {
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
        private void keyUpTextBox(object sender, KeyEventArgs e) {
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
        private bool removeWordSelect(IInputElement el)
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
        private void deleteSelections()
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
   
        private void onMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastFocusedTextBox = null;
            lastFocusedTextBoxes = new IInputElement[0];
            labelsFill();
            deleteSelections();
        }
        //Exit Function
        bool closingApp(object sender)
        {
            if (isFileChanged)
            {
                RoutedEventArgs e = new RoutedEventArgs();
                MessageBoxResult saveFileMessage = MessageBox.Show("Do you want to save the file before exiting?", Title, MessageBoxButton.YesNoCancel);
                switch (saveFileMessage)
                {
                    case MessageBoxResult.Yes:
                        saveFile(sender,e);
                        break;
                    case MessageBoxResult.No: 
                        break;
                    case MessageBoxResult.Cancel:
                        return false;
                        break;
                }
            }
           
                using (StreamWriter writer = new StreamWriter("filePath.txt", false))
                {
                writer.WriteLine(PathSave.IsChecked.ToString());
                if (PathSave.IsChecked)        
                    writer.WriteLine(path);
                 
                }
           
            System.Windows.Application.Current.Shutdown();
            return true;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!closingApp(e))
            {
                e.Cancel = true;
            }
          
        
        }
        private void exitFromApplication(object sender, RoutedEventArgs e) => closingApp(sender);
    }
  
}
