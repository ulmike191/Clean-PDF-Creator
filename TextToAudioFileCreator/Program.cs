using Microsoft.Office.Interop.Word;
using System;
using System.IO;
using System.Text.RegularExpressions;
using Word = Microsoft.Office.Interop.Word;

namespace TextToAudioFileCreator
{
    public class CleanPDF_CreatorLib
    {
        public static string CleanPDF_Creator(string rawContent, string newPDFName, bool isText = true)
        {
            string newFileName = newPDFName;
            string destinationPath = String.Empty;
            string text = String.Empty; 

            if (isText)
            {
                destinationPath = $@"C:\Users\ulmik\Documents\Teaching\PDFs";
                text = CleanFileContents(rawContent, newFileName, destinationPath);
            }
            else
            {
                destinationPath = $@"C:\Users\ulmik\Documents\BetSheets";
                text = rawContent;
            }

            string wordDocFilePath = CreateWordDocument(destinationPath, newFileName, text);

            string pdfDocFilePath = CreatePDFDocument(wordDocFilePath);

            File.Delete(wordDocFilePath);

            return pdfDocFilePath;
        }

        public static string CleanFileContents(string text, string filename, string pathname)
        {
            
            //string path = Path.Combine(pathname, filename + ".txt");
            //string text = File.ReadAllText(path);

            //File.Delete(path);

            // Clean the text by removing all content within brackets.
            text = Regex.Replace(text, @"\[.*?\]", "");
            text = Regex.Replace(text, @"\s{2,}", " ");

            // Could add improvement that removes the spaces before paragraphs left by removing the brackets.

            return text;
        }
        public static string CreateWordDocument(string pathName, string fileName, string text)
        {
            // Write the cleaned text back to a Word document.
            Word.Application word = new Word.Application();
            word.Visible = true;
            word.WindowState = Word.WdWindowState.wdWindowStateNormal;
            Word.Document document = new Word.Document();
            Word.Paragraph paragraph;
            paragraph = document.Paragraphs.Add();
            paragraph.Range.Text = text;
            string docFileName = fileName + "Audio.doc";
            string docFilePath = Path.Combine(pathName, docFileName);
            document.SaveAs(docFilePath);
            document.Close();
            word.Quit();

            return docFilePath;
        }

        public static string CreatePDFDocument(string path)
        {
            Word.Application app = new Word.Application();
            app.DisplayAlerts = WdAlertLevel.wdAlertsNone;
            app.Visible = true;

            var objPresSet = app.Documents;
            var objPres = objPresSet.Open(path, true, true, false);

            var pdfFileName = Path.ChangeExtension(path, ".pdf"); 
            
            try
            {
                objPres.ExportAsFixedFormat(
                    pdfFileName,
                    WdExportFormat.wdExportFormatPDF,
                    false,
                    WdExportOptimizeFor.wdExportOptimizeForPrint,
                    WdExportRange.wdExportAllDocument
                );

                //bool flag = CloseWordDocuments(path);

                Console.WriteLine("Your file is located at " + pdfFileName);
            }
            catch
            {
                pdfFileName = null;

                Console.WriteLine("Unable to create PDF version of your Word doc");

            }
            finally
            {
                objPres.Close();

            }

            return pdfFileName;
        }

        public static bool CloseWordDocuments(string osPath)
        {
            Word.Application app = (Word.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application");
            if (app == null)
                return true;

            foreach (Word.Document d in app.Documents)
            {
                if (d.FullName.ToLower() == osPath.ToLower())
                {
                    object saveOption = Word.WdSaveOptions.wdDoNotSaveChanges;
                    object originalFormat = Word.WdOriginalFormat.wdOriginalDocumentFormat;
                    object routeDocument = false;
                    d.Close(ref saveOption, ref originalFormat, ref routeDocument);
                    return true;
                }
            }
            return true;
        }
    }
}
