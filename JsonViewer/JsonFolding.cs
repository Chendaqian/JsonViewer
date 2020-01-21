using ICSharpCode.TextEditor.Document;

using System.Collections.Generic;

namespace Json.Viewer
{
    /// <summary>
    /// The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
    /// </summary>
    public class JsonFolding : IFoldingStrategy
    {
        /// <summary>
        /// Generates the foldings for our document.
        /// </summary>
        /// <param name="document">The current document.</param>
        /// <param name="fileName">The filename of the document.</param>
        /// <param name="parseInformation">Extra parse information, not used in this sample.</param>
        /// <returns>A list of FoldMarkers.</returns>
        public List<FoldMarker> GenerateFoldMarkers(IDocument document, string fileName, object parseInformation)
        {
            var list = new List<FoldMarker>();
            var startLines = new Stack<int>();

            // Create foldmarkers for the whole document, enumerate through every line.
            for (int i = 0; i < document.TotalNumberOfLines; i++)
            {
                var seg = document.GetLineSegment(i);
                int offs, end = document.TextLength;
                char c;
                //string text = document.GetText(document.GetLineSegment(i));
                for (offs = seg.Offset; offs < end && ((c = document.GetCharAt(offs)) == ' ' || c == '\t'); offs++)
                {
                }
                if (offs == end)
                    break;
                int spaceCount = offs - seg.Offset;

                // now offs points to the first non-whitespace char on the line
                if (document.GetCharAt(offs) == '{' || document.GetCharAt(offs) == '}')
                {
                    string text = document.GetText(offs, seg.Length - spaceCount);
                    if (text.Contains("{"))
                        startLines.Push(i);
                    if (text.Contains("}") && startLines.Count > 0)
                    {
                        // Add a new FoldMarker to the list.
                        int start = startLines.Pop();
                        list.Add(new FoldMarker(document, start,
                            document.GetLineSegment(start).Length,
                            i, spaceCount + "#endregion".Length, FoldType.Region, "{...}"));
                    }
                }

                // { }
                //if (document.GetCharAt(offs) == '{')
                //{
                //    int offsetOfClosingBracket = document.FormattingStrategy.SearchBracketForward(document, offs + 1, '{', '}');
                //    if (offsetOfClosingBracket > 0)
                //    {
                //        int length = offsetOfClosingBracket - offs + 1;
                //        list.Add(new FoldMarker(document, offs, length, "{...}", false));
                //    }
                //}

                //if (document.GetCharAt(offs) == '/')
                //{
                //    string text = document.GetText(offs, seg.Length - spaceCount);
                //    if (text.StartsWith("/// <summary>"))
                //        startLines.Push(i);
                //    if ((text.StartsWith("/// <param") || text.StartsWith("/// <returns>") || text.StartsWith("/// </summary>"))
                //        && startLines.Count > 0)
                //    {
                //        // Add a new FoldMarker to the list.
                //        int start = startLines.Pop();
                //        list.Add(new FoldMarker(document, start,
                //            document.GetLineSegment(start).Length,
                //            i, spaceCount + "/// </summary>".Length, FoldType.TypeBody, "/// <summary>..."));
                //    }

                //}
            }

            return list;
        }
    }
}