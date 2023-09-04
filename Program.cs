using System.Collections.Generic;
using System.Linq;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

public static class Program
{
    public static void Main(string[] args)
    {
        using (PdfDocument document = PdfDocument.Open(args[0]))
        {
            foreach (Page page in document.GetPages())
            {
                IReadOnlyList<Letter> letters = page.Letters;
                string example = string.Join(string.Empty, letters.Select(x => x.Value));

                IEnumerable<Word> words = page.GetWords();

                StringBuilder builder = new StringBuilder();
                builder.AppendFormat("# {0}\n\n", args[0]);
                builder.AppendFormat("![[{0}]]\n\n", args[0]);
                builder.Append("## Words\n");
                foreach (var word in words)
                {
                    builder.AppendFormat("{0} ", word);
                }
                builder.Append("\n");
                var fullfile = builder.ToString();
                // Console.Write(fullfile);

                string newfilename = args[0].Replace(".pdf", ".md");

                File.WriteAllText(newfilename, fullfile);

                // IEnumerable<IPdfImage> images = page.GetImages();
            }
        }
    }
}