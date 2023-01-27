using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MyTextEditor
{

    public partial class underleaf : Form
    {
        string filePath = string.Empty;
        int isfileSaved; //variable used to check if the file has been saved or not
        int isopen;
        int allletterscount; //Keeps count of all characters
        int letterswithoutspace; 
        //int words;
        int rows;

        //initilaizes the program
        public underleaf()
        {
            InitializeComponent();
            this.Text = "Nameless.txt";
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
                String[] dropfiles = (string[])e.Data.GetData(DataFormats.FileDrop, false); //used to get file path
                if (dropfiles != null && dropfiles.Length != 0)
                {
                    System.IO.StreamReader dropreader = new System.IO.StreamReader(dropfiles[0]);
                    string dropfileTemp = dropreader.ReadToEnd(); //saves the contents of file in a temporary file
                    dropreader.Close();
                    //if Ctrl is pressed
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {   
                        underleaf.ActiveForm.Text = underleaf.ActiveForm.Text + '*';
                        texteditor.SelectionColor = Color.DarkGreen;
                        texteditor.AppendText(dropfileTemp);
                    }else if (ModifierKeys.HasFlag(Keys.Shift)) //If Shift is pressed
                    {
                        //underleaf.ActiveForm.Text = underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath) + '*';
                        texteditor.SelectedText = dropfileTemp;
                    }
                    else
                    {
                        
                        DialogResult result = MessageBox.Show("You have not saved the current file. Save the file first?", "Warning!", MessageBoxButtons.YesNoCancel);
                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            texteditor.Text = dropfileTemp;
                            savefile();
                            underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
                            isfileSaved = 2;

                        }
                        else
                        { 
                        
                        }
                    }
                }
                underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }
            underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
        }
        //creates a new text file
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            if (isfileSaved == 1)
            {
                result = MessageBox.Show("You have not saved the current file. Save the file first?", "Warning!", MessageBoxButtons.YesNoCancel);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    savefile();

                    underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
                }
                else if (result == System.Windows.Forms.DialogResult.No)
                {
                    texteditor.Clear();
                    filePath = "Nameless";
                    underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
                }
            }
            else
            {
                texteditor.Clear();

            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
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
        //Used to open a file
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result;
            if (isfileSaved == 1)
            {
                result = MessageBox.Show("You have not saved the current file. Save the file first?", "Warning!", MessageBoxButtons.YesNoCancel);

                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    savefile();

                } else if (result == System.Windows.Forms.DialogResult.No)
                {
                    OpenFileDialog fileopener = new OpenFileDialog();
                    fileopener.Filter = "Text Files (.txt)|*.txt";
                    fileopener.Title = "Select file to open ...";

                    if (fileopener.ShowDialog() == DialogResult.OK)
                    {
                        //filePath = fileopener.FileName;
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

                        int word = wordCounter(texteditor.Text);
                        wordcount.Text = "Words: " + word;
                        reader.Close();

                        isfileSaved = 2;
                    }
                    underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);

                }
            }
            else
            {
                OpenFileDialog fileopener = new OpenFileDialog();
                fileopener.Filter = "Text Files (.txt)|*.txt";
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

                    int word = wordCounter(texteditor.Text);
                    wordcount.Text = "Words: " + word;
                    reader.Close();
                    isfileSaved = 2;
                }
                underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath);
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            savefile();
        }

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
            if (filePath == String.Empty)
            {
                underleaf.ActiveForm.Text = "Nameless" + "*";
                isfileSaved = 1;
                allletterscount = texteditor.TextLength;
                allletters.Text = "All Letters: " + allletterscount;

                String temstring = texteditor.Text;
                letterswithoutspace = temstring.Count(char.IsLetterOrDigit);
                Lettersspace.Text = "Letters without space: " + letterswithoutspace;

                rowcount.Text = "Rows:" + texteditor.Lines.Count();

                int words = wordCounter(texteditor.Text);
                wordcount.Text = "Words: " + words;
            }
            else
            {
                underleaf.ActiveForm.Text = System.IO.Path.GetFileNameWithoutExtension(filePath) + "*";
                isfileSaved = 1;
            }

        }

        //Saves as file
        private void savefile()
        {
            if (filePath == String.Empty)
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
                }
                filePath = filesaver.FileName;
                isfileSaved = 2;
            }
            else
            {
                isfileSaved = 2;
                System.IO.StreamWriter saver = new System.IO.StreamWriter(filePath);
                saver.Write(texteditor.Text);
                saver.Close();
            }
        }
        //counts characters excluding space
        private static int wordCounter(string s)
        {
            return s.Split(new char[] {' '},StringSplitOptions.RemoveEmptyEntries).Length;
        }

        //triggers upon exiting the program
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            DialogResult result;
            if (isfileSaved == 1)
            {
                result = MessageBox.Show("You have not saved the current file. Save the file first?", "Warning!", MessageBoxButtons.YesNoCancel);
                if (result == System.Windows.Forms.DialogResult.Yes)
                {
                    savefile();
                    isfileSaved = 2;
                }
                else if (result == System.Windows.Forms.DialogResult.No)
                {
                    
                    isfileSaved = 2;
                }
                else if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    e.Cancel = true;
                    isfileSaved = 1;
                }
            }
            else
            {
                Application.Exit();

            }
        }
    }
}
