using System.Windows.Forms;
using TextToAudioFileCreator;
using ImageToTextCreator;
using IronOcr;

namespace CleanPDFCreator
{
    public partial class Form1 : Form
    {
        List<string> list;

        public Form1()
        {
            InitializeComponent();
            list = new List<string>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (!text AND (!name OR !image)) OR (text AND !name)
            if(((richTextBox1.Text == "") && (textBox1.Text == "" || list.Count == 0)) || (textBox1.Text == "")){
               
                MessageBox.Show("Please Enter Valid Data.");

                return;
            }

            if (list.Count != 0)
            {
                var listSize = list.Count;

                //HandleImageDrop();

                for(int i = 0; i < listSize; i++)
                {
                    var ocr = new IronTesseract();
                    using (var input = new OcrInput(list[i]))
                    {
                        var result = ocr.Read(input);
                        var savePath = Path.Combine(@"C:\Users\ulmik\Documents\BetSheets\", "imageText.txt");
                        result.SaveAsTextFile(savePath);
                        MessageBox.Show("Your file is saved at " + savePath);
                    }

                    list.Remove(list[i]);
                }

                resetForm();
                return;
            }

            HandleTextInput();

            return;
        }

        private void resetForm()
        {
            progressBar1.Value = 0;
            richTextBox1.Text = String.Empty;
            textBox1.Text = String.Empty;
            label4.Text = "Progress";
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            
            if (files != null)
            {
                foreach(string file in files)
                {
                    list.Add(file);
                }
            }
        }

        private void HandleTextInput()
        {
            label4.Text = "Loading...";

            for (int i = 0; i < 50; i++)
            {
                progressBar1.Value += 1;
            }

            string pdfDocFilePath = CleanPDF_CreatorLib.CleanPDF_Creator(richTextBox1.Text, textBox1.Text);

            for (int j = 0; j < 50; j++)
            {
                progressBar1.Value += 1;
            }

            MessageBox.Show("Your PDF is located at " + pdfDocFilePath);

            resetForm();

        }
        private void HandleImageDrop()
        {
            string text = String.Empty;

            if (list != null)
            {
                foreach (string item in list)
                {
                    MyImageToTextCreator creator = new MyImageToTextCreator();
                    string imageText = creator.TranslateImageToText(item);
                    text = CleanPDF_CreatorLib.CleanPDF_Creator(imageText, textBox1.Text, false);

                    MessageBox.Show("Your PDF is located at " + text);
                }
            }
        }
    }
}