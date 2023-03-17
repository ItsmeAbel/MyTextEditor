using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;


//Några antaganden som har gjorts under detta laboration är att det endast används filer av ".txt" formatet och att
//proggammet körs endast på datorer med windows OS. Programmet har ej testas på andra OS och därmed kan ej garanteras att funka.
//Programmet byggdes mha .NET framework version 5.0.4
//Referens till kod kan hittas på: https://learn.microsoft.com/en-us/dotnet/
//Gör ett försök på Version 3(Full poäng)

namespace MyTextEditor
{

    public partial class underleaf : Form
    {
        string filePath = String.Empty; //filepath is empty from the start
        int isfileSaved; //variable used to check if the file has been saved or not
        int isopen; //used for char count in an opened or dragged and dropped file
        int allletterscount; //Keeps count of all characters
        int letterswithoutspace;
        int newfile = 0;
        int newsaved = 0;
        int dragndrop; //used for window title in drag and drop
 
   

        //initilaizes the program
        public underleaf()
        {
            InitializeComponent();
            this.Text = "Nameless"; //sets windows title to Nameless
            texteditor.AllowDrop = true;
            //texteditor.EnableAutoDragDrop = true; //can be used to move text within the editor
            texteditor.DragDrop += Texteditor_DragDrop; 
        }

        //drag and drop function
        private void Texteditor_DragDrop(object sender, DragEventArgs e)
        {
            //throw new NotImplementedException();
            var data = e.Data.GetData(DataFormats.FileDrop);
            if(data != null)
            {
                string[] dropfiles = (string[])e.Data.GetData(DataFormats.FileDrop, false); //used to get file path
                if (dropfiles != null && dropfiles.Length != 0) 
                {
                    var dropreader = new StreamReader(dropfiles[0]);
                    string dropfileTemp = dropreader.ReadToEnd(); //saves the contents of file in a temporary file
                    dropreader.Close();
                    //if Ctrl is pressed
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {   
                        underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath) + '*'; //window title gets updated
                        texteditor.SelectionColor = Color.DarkGreen; //new text has another color
                        texteditor.AppendText(dropfileTemp); //appends the text to the end of the already existing text
                    }
                    else if (ModifierKeys.HasFlag(Keys.Shift)) //If Shift is pressed
                    {
                        //underleaf.ActiveForm.Text = underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath) + '*';
                        texteditor.SelectedText = dropfileTemp; //puts the text where the curser is
                    }
                    else
                    {
                        //if user hasn't pressed any key on the keyboard
                        DialogResult result = MessageBox.Show("You have not saved the current file. Save the file first?", "Warning!", MessageBoxButtons.YesNoCancel);
                        if (result == DialogResult.Yes)
                        {
                            texteditor.Text = dropfileTemp;
                            filePath = dropfiles[0];
                            Console.WriteLine(dropfiles[0]);
                            savefile();
                            underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
                            dragndrop = 1; //file has been dropped
                            isfileSaved = 2; //file has been saved
                            isopen = 1; //files has been opened

                        }
                        else if(result == DialogResult.No)
                        {
                            filePath = String.Empty;
                            texteditor.Text = dropfileTemp;
                            isfileSaved = 1;
                            isopen = 1;
                        }
                        else
                        {

                        }
                    }
                }
                //underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
            //underleaf.ActiveForm.Text = "Nameless";
            //dragndropNo = 1;
        }

        //creates a new text file
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            if (isfileSaved == 1)
            {
                //saves the already existing text if it hasn't been saved yet
                result = MessageBox.Show("You have not saved the current file. Save the file first?", "Warning!", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    newfile = 1;
                    savefile();
                    if (newsaved == 1)
                    {
                        texteditor.Clear();
                        filePath = "Nameless"; //sets file title to Nameless
                        underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        isfileSaved = 2;
                        //isopen = 2;

                    }

                }
                else if (result == DialogResult.No) //opens new file without saving the already existing text
                {
                    texteditor.Clear(); //clears the text
                    filePath = "Nameless"; //sets file title to Nameless
                    underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    //isopen = 2;
                    isfileSaved = 2;
                }
            }
            else
            {
                texteditor.Clear();
                isfileSaved = 2;
                filePath = "Nameless"; //sets file title to Nameless
                underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //DialogResult result;
            if (isfileSaved == 1)
            {
                    Application.Exit();
                    isfileSaved = 2; 
            }
            else
            {
                Application.Exit();
            }
        }
        //opens a file using the open button
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            //if the already existing text hasn't been saved
            if (isfileSaved == 1)
            {
                result = MessageBox.Show("You have not saved the current file. Save the file first?", "Warning!", MessageBoxButtons.YesNoCancel);

                if (result == System.Windows.Forms.DialogResult.Yes) //saves the file first
                {
                    savefile();
                    underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath); //title without asterisk

                } else if (result == System.Windows.Forms.DialogResult.No) //if user doesn't want to save text
                {
                    OpenFileDialog fileopener = new OpenFileDialog();
                    fileopener.Filter = "Text Files (.txt)|*.txt"; //allows only files with .txt extension
                    fileopener.Title = "Select file to open ...";

                    if (fileopener.ShowDialog() == DialogResult.OK)
                    {
                        filePath = fileopener.FileName; //comment out????????
                        //Read the contents of the file into a stream
                        var fileStream = fileopener.OpenFile();
                        System.IO.StreamReader reader = new System.IO.StreamReader(fileStream);
                        texteditor.Text = reader.ReadToEnd(); //reads from the stream into the richtextbox

                        //updates counters
                        allletterscount = texteditor.TextLength;
                        allletters.Text = "All Letters: " + allletterscount;

                        String temstring = texteditor.Text;
                        letterswithoutspace = temstring.Count(char.IsLetterOrDigit);
                        Lettersspace.Text = "Letters without space: " + letterswithoutspace;

                        rowcount.Text = "Rows:" + texteditor.Lines.Count();

                        int words = wordCounter(texteditor.Text);
                        wordcount.Text = "Words: " + words;
                        reader.Close();

                        isopen = 1; //marks file has been opened. Used for live update of the counters in texteditor_TextChanged()
                        isfileSaved = 2; //marks that the file has already been saved
                    }
                    underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath); //updates window's title
                }
            }
            else
            {
                //if field is empty or text is already saved
                OpenFileDialog fileopener = new OpenFileDialog();
                fileopener.Filter = "Text Files (.txt)|*.txt"; //only .txt files
                fileopener.Title = "Select file to open ...";

                if (fileopener.ShowDialog() == DialogResult.OK)
                {
                    filePath = fileopener.FileName;
                    //Read the contents of the file into a stream
                    var fileStream = fileopener.OpenFile();
                    System.IO.StreamReader reader = new System.IO.StreamReader(fileStream);
                    texteditor.Text = reader.ReadToEnd();
                    allletterscount = texteditor.TextLength;
                    allletters.Text = "All Letters: " + allletterscount;

                    String temstring = texteditor.Text;
                    letterswithoutspace = temstring.Count(char.IsLetterOrDigit);
                    Lettersspace.Text = "Letters without space: " + letterswithoutspace;

                    rowcount.Text = "Rows:" + texteditor.Lines.Count();

                    int words = wordCounter(texteditor.Text);
                    wordcount.Text = "Words: " + words;
                    reader.Close();
                    isfileSaved = 2;
                    isopen = 1;
                }
                underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }

        }
        //save button. simply saves text
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savefile();
        }

        //save as button. Saves the text to a file
        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog filesaver = new SaveFileDialog();
            filesaver.Filter = "Text Files (.txt)|*.txt";
            filesaver.Title = "Save file ...";
            filesaver.FileName = "Nameless";


            if (filesaver.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter saver = new System.IO.StreamWriter(filesaver.FileName);
                filePath = filesaver.FileName;
                underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
                saver.Write(texteditor.Text);
                saver.Close();
            }

            isfileSaved = 2;
        }

        //when the contents of the text field has been changed
        private void texteditor_TextChanged(object sender, EventArgs e)
        {
            String temstring = texteditor.Text;
            char excludechar = '\n';

            //updates counter
            string textwithoutbreak = temstring.Replace(excludechar.ToString(), ""); //solves the issue of newline counting as a letter
            allletterscount = textwithoutbreak.Length;
            allletters.Text = "All Letters: " + allletterscount;

            letterswithoutspace = temstring.Count(char.IsLetterOrDigit);
            Lettersspace.Text = "Letters without space: " + letterswithoutspace;

            rowcount.Text = "Rows:" + texteditor.Lines.Count();

            //working solution
            string[] words = Regex.Split(temstring, @"\s+");
            int wordcountnum = words.Length;
            wordcount.Text = "Words: " + (wordcountnum - 1);

            if (filePath == String.Empty || isopen == 1 && newfile != 0) //if path is empty or new file has been opened
            {
                if (filePath == String.Empty )
                {
                    //underleaf.ActiveForm.Text = "Nameless" + "*"; throws an error while using drag and drop. caused due to the windows losing focus. Solution: this.text
                    this.Text = "Nameless" + "*"; //if there is no filepath for the file, set window title to "Nameless*"
                    newfile = 1;
                }
                else
                {
                    underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath) + "*"; //if there is a filepath, set title to file name + *
                    newfile = 1;
                }
                
                isfileSaved = 1; //marks file as unsaved
               
            }
            else
            {
                //used to update windows title if file has been dragged and dropped
                if (dragndrop == 1)
                {
                    underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    isfileSaved = 2;
                    dragndrop = 2;
                }
                else
                {
                    underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath) + "*";
                    isfileSaved = 1;
                }

            }

        }

        //save function
        private void savefile()
        {
            //if file path is empty. i.e text has never been saved yet, save the text as a file
            if (filePath==string.Empty|| newfile == 1)
            {
                SaveFileDialog filesaver = new SaveFileDialog();
                filesaver.Filter = "Text Files (.txt)|*.txt";
                filesaver.Title = "Save file ...";
                filesaver.FileName = "Nameless";

                if (filesaver.ShowDialog() == DialogResult.OK)
                {
                    System.IO.StreamWriter saver0 = new System.IO.StreamWriter(filesaver.FileName);
                    saver0.Write(texteditor.Text);
                    saver0.Close();
                    filePath = filesaver.FileName; //set path to the file
                    underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
                    isfileSaved = 2;
                    newfile = 0;
                    newsaved = 1;
                }
                else if (filesaver.ShowDialog() == DialogResult.Cancel)
                {
                    newfile = 1;
                    isfileSaved = 1;
                    newsaved = 0;
                }


            }
            else
            {
                //if path already exists, save text into the file at the path
                underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
                isfileSaved = 2; //marks the text as saved
                System.IO.StreamWriter saver = new System.IO.StreamWriter(filePath);
                saver.Write(texteditor.Text);
                saver.Close();
            }
        }
        //used to count words, identified by space
        private static int wordCounter(string s)
        {
            return s.Split(new char[] {' '},StringSplitOptions.RemoveEmptyEntries).Length;
        }

        //Triggers upon exiting the program. Any attempt to close the proggramm will trigger this function. 'Exit' button, 'X' button or 'Alt+F4' for example
        //There isn't a built in function in .NET to handle 'X' so i use this overriding function instead
        //Function works properly even it says 0 references
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            DialogResult result;
            if (isfileSaved == 1) //if text not saved
            {
                //save text first
                result = MessageBox.Show("You have not saved the current file. Save the file first?", "Warning!", MessageBoxButtons.YesNoCancel);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    savefile();
                    isfileSaved = 2;
                }
                else if (result == System.Windows.Forms.DialogResult.No) //if user doesn't want to save text
                {
                    isfileSaved = 2; //mark as saved and move on
                }
                else if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    e.Cancel = true;
                    isfileSaved = 1; //text hasn't been saved yet
                }
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
