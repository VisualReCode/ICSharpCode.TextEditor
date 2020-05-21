// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.TextEditor.Document;
using NUnit.Framework;

namespace ICSharpCode.TextEditor.Tests
{
    [TestFixture]
    public class TextMarkerTests
    {
        [SetUp]
        public void SetUp()
        {
            document = new DocumentFactory().CreateDocument();
            document.TextContent = "0123456789";
            marker = new TextMarker(offset: 3, length: 3, textMarkerType: TextMarkerType.Underlined);
            document.MarkerStrategy.AddMarker(marker);
        }

        private IDocument document;
        private TextMarker marker;

        [Test]
        public void InsertTextAfterMarker()
        {
            document.Insert(offset: 7, text: "ab");
            Assert.AreEqual("345", document.GetText(marker));
        }

        [Test]
        public void InsertTextBeforeMarker()
        {
            document.Insert(offset: 1, text: "ab");
            Assert.AreEqual("345", document.GetText(marker));
        }

        [Test]
        public void InsertTextImmediatelyAfterMarker()
        {
            document.Insert(offset: 6, text: "ab");
            Assert.AreEqual("345", document.GetText(marker));
        }

        [Test]
        public void InsertTextImmediatelyBeforeMarker()
        {
            document.Insert(offset: 3, text: "ab");
            Assert.AreEqual("345", document.GetText(marker));
        }

        [Test]
        public void InsertTextInsideMarker()
        {
            document.Insert(offset: 4, text: "ab");
            Assert.AreEqual("3ab45", document.GetText(marker));
        }

        [Test]
        public void RemoveTextAfterMarker()
        {
            document.Remove(offset: 7, length: 1);
            Assert.AreEqual(expected: 1, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
            Assert.AreEqual("345", document.GetText(marker));
        }

        [Test]
        public void RemoveTextBeforeMarker()
        {
            document.Remove(offset: 1, length: 1);
            Assert.AreEqual(expected: 1, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
            Assert.AreEqual("345", document.GetText(marker));
        }

        [Test]
        public void RemoveTextBeforeMarkerIntoMarker()
        {
            document.Remove(offset: 2, length: 2);
            Assert.AreEqual(expected: 1, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
            Assert.AreEqual("45", document.GetText(marker));
        }

        [Test]
        public void RemoveTextBeforeMarkerOverMarkerEnd()
        {
            document.Remove(offset: 2, length: 5);
            Assert.AreEqual(expected: 0, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
        }

        [Test]
        public void RemoveTextBeforeMarkerUntilMarkerEnd()
        {
            document.Remove(offset: 2, length: 4);
            Assert.AreEqual(expected: 0, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
        }

        [Test]
        public void RemoveTextFromMarkerStartIntoMarker()
        {
            document.Remove(offset: 3, length: 1);
            Assert.AreEqual(expected: 1, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
            Assert.AreEqual("45", document.GetText(marker));
        }

        [Test]
        public void RemoveTextFromMarkerStartOverMarkerEnd()
        {
            document.Remove(offset: 3, length: 4);
            Assert.AreEqual(expected: 0, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
        }

        [Test]
        public void RemoveTextFromMarkerStartUntilMarkerEnd()
        {
            document.Remove(offset: 3, length: 3);
            Assert.AreEqual(expected: 0, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
        }

        [Test]
        public void RemoveTextImmediatelyAfterMarker()
        {
            document.Remove(offset: 6, length: 1);
            Assert.AreEqual(expected: 1, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
            Assert.AreEqual("345", document.GetText(marker));
        }

        [Test]
        public void RemoveTextImmediatelyBeforeMarker()
        {
            document.Remove(offset: 2, length: 1);
            Assert.AreEqual(expected: 1, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
            Assert.AreEqual("345", document.GetText(marker));
        }

        [Test]
        public void RemoveTextInsideMarker()
        {
            document.Remove(offset: 4, length: 1);
            Assert.AreEqual(expected: 1, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
            Assert.AreEqual("35", document.GetText(marker));
        }

        [Test]
        public void RemoveTextInsideMarkerOverMarkerEnd()
        {
            document.Remove(offset: 4, length: 3);
            Assert.AreEqual(expected: 1, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
            Assert.AreEqual("3", document.GetText(marker));
        }

        [Test]
        public void RemoveTextInsideMarkerUntilMarkerEnd()
        {
            document.Remove(offset: 4, length: 2);
            Assert.AreEqual(expected: 1, actual: document.MarkerStrategy.GetMarkers(offset: 0, length: document.TextLength).Count);
            Assert.AreEqual("3", document.GetText(marker));
        }
    }
}