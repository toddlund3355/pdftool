using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

public static class Program
{
    public static void Main(string[] args)
    {
        var originalFilename = args[0];
        string jpgfilename = originalFilename.Replace(".pdf", ".jpg");
        using (PdfDocument document = PdfDocument.Open(originalFilename))
        {
            foreach (Page page in document.GetPages())
            {
                IReadOnlyList<Letter> letters = page.Letters;
                string example = string.Join(string.Empty, letters.Select(x => x.Value));

                IEnumerable<Word> words = page.GetWords();

                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("# {0}\n\n", originalFilename);
                builder.AppendFormat("[{0}]({0})\n\n", originalFilename);
                builder.AppendFormat("![{0}]({0})\n\n", jpgfilename);
                builder.Append("## Words\n");
                foreach (var word in words)
                {
                    builder.AppendFormat("{0} ", word);
                }
                builder.Append("\n");
                var fullfile = builder.ToString();
                // Console.Write(fullfile);

                string mdfilename = originalFilename.Replace(".pdf", ".md");


                File.WriteAllText(mdfilename, fullfile);
                var bytearray = File.ReadAllBytes(originalFilename);
                var base64pdf = Convert.ToBase64String(bytearray);

                PDFtoImage.Conversion.SavePng(jpgfilename, base64pdf);
                // IEnumerable<IPdfImage> images = page.GetImages();
            }
        }
    }
}