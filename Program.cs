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
        // Remove spaces from the filename (not the path)
        var directory = Path.GetDirectoryName(originalFilename) ?? "";
        var filenameOnly = Path.GetFileNameWithoutExtension(originalFilename).Replace(" ", "");
        var extension = Path.GetExtension(originalFilename);
        var sanitizedFilename = Path.Combine(directory, filenameOnly + extension);

        // If the sanitized filename is different, rename the file
        if (!string.Equals(originalFilename, sanitizedFilename, StringComparison.Ordinal))
        {
            File.Move(originalFilename, sanitizedFilename);
            originalFilename = sanitizedFilename;
        }

        string jpgfilename = Path.Combine(directory, filenameOnly + ".jpg");
        string mdfilename = Path.Combine(directory, filenameOnly + ".md");

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

                File.WriteAllText(mdfilename, fullfile);
                var bytearray = File.ReadAllBytes(originalFilename);
                var base64pdf = Convert.ToBase64String(bytearray);

                PDFtoImage.Conversion.SavePng(jpgfilename, base64pdf);
            }
        }
    }
}