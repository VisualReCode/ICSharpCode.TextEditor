// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;

namespace ICSharpCode.TextEditor.Tests
{
    [TestFixture]
    public class BlockCommentTests
    {
        [SetUp]
        public void Init()
        {
            document = new DocumentFactory().CreateDocument();
            document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("XML");
        }

        private IDocument document;
        private readonly string commentStart = "<!--";
        private readonly string commentEnd = "-->";

        [Test]
        public void CaretInsideCommentButNoSelectedText()
        {
            document.TextContent = "<!---->";
            var selectionStartOffset = 4;
            var selectionEndOffset = 4;
            var expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, startOffset: 0, endOffset: 4);

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.AreEqual(expectedCommentRegion, commentRegion);
        }

        [Test]
        public void CursorJustOutsideCommentEnd()
        {
            document.TextContent = "<!-- -->";
            var selectionStartOffset = 8;
            var selectionEndOffset = 8;

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.IsNull(commentRegion);
        }

        [Test]
        public void CursorJustOutsideCommentStart()
        {
            document.TextContent = "<!-- -->";
            var selectionStartOffset = 0;
            var selectionEndOffset = 0;

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.IsNull(commentRegion);
        }

        [Test]
        public void EntireCommentAndExtraTextSelected()
        {
            document.TextContent = "a<!-- -->";
            var selectionStartOffset = 0;
            var selectionEndOffset = 9;
            var expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, startOffset: 1, endOffset: 6);

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.AreEqual(expectedCommentRegion, commentRegion);
        }

        [Test]
        public void EntireCommentSelected()
        {
            document.TextContent = "<!---->";
            var selectionStartOffset = 0;
            var selectionEndOffset = 7;
            var expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, startOffset: 0, endOffset: 4);

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.AreEqual(expectedCommentRegion, commentRegion);
        }

        [Test]
        public void FirstCharacterOfCommentStartSelected()
        {
            document.TextContent = "<!-- -->";
            var selectionStartOffset = 0;
            var selectionEndOffset = 1;
            var expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, startOffset: 0, endOffset: 5);

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.AreEqual(expectedCommentRegion, commentRegion);
        }

        [Test]
        public void LastCharacterOfCommentEndSelected()
        {
            document.TextContent = "<!-- -->";
            var selectionStartOffset = 7;
            var selectionEndOffset = 8;
            var expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, startOffset: 0, endOffset: 5);

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.AreEqual(expectedCommentRegion, commentRegion);
        }

        [Test]
        public void NoTextSelected()
        {
            document.TextContent = string.Empty;
            var selectionStartOffset = 0;
            var selectionEndOffset = 0;

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.IsNull(commentRegion, "Should not be a comment region for an empty document");
        }

        [Test]
        public void OnlyCommentEndSelected()
        {
            document.TextContent = "<!-- -->";
            var selectionStartOffset = 5;
            var selectionEndOffset = 8;
            var expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, startOffset: 0, endOffset: 5);

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.AreEqual(expectedCommentRegion, commentRegion);
        }

        [Test]
        public void OnlyCommentStartSelected()
        {
            document.TextContent = "<!-- -->";
            var selectionStartOffset = 0;
            var selectionEndOffset = 4;
            var expectedCommentRegion = new BlockCommentRegion(commentStart, commentEnd, startOffset: 0, endOffset: 5);

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.AreEqual(expectedCommentRegion, commentRegion);
        }

        [Test]
        public void TwoExistingBlockComments()
        {
            document.TextContent = "<a>\r\n" +
                                   "<!--<b></b>-->\r\n" +
                                   "\t<c></c>\r\n" +
                                   "<!--<d></d>-->\r\n" +
                                   "</a>";

            var selectedText = "<c></c>";
            var selectionStartOffset = document.TextContent.IndexOf(selectedText);
            var selectionEndOffset = selectionStartOffset + selectedText.Length;

            var commentRegion = ToggleBlockComment.FindSelectedCommentRegion(document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);
            Assert.IsNull(commentRegion);
        }
    }
}