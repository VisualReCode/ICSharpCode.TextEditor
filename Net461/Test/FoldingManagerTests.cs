/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 10/28/2006
 * Time: 8:42 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Collections.Generic;
using System.Text;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;

namespace ICSharpCode.TextEditor.Tests
{
    [TestFixture]
    public class FoldingManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            var doc = new DocumentFactory().CreateDocument();
            var b = new StringBuilder();
            for (var i = 0; i < 50; i++)
                b.AppendLine(new string(c: 'a', count: 50));
            doc.TextContent = b.ToString();
            list = new List<FoldMarker>();
            list.Add(new FoldMarker(doc, startLine: 1, startColumn: 6, endLine: 5, endColumn: 2));
            list.Add(new FoldMarker(doc, startLine: 2, startColumn: 1, endLine: 2, endColumn: 3));
            list.Add(new FoldMarker(doc, startLine: 3, startColumn: 7, endLine: 4, endColumn: 1));
            list.Add(new FoldMarker(doc, startLine: 10, startColumn: 1, endLine: 14, endColumn: 1));
            list.Add(new FoldMarker(doc, startLine: 10, startColumn: 3, endLine: 10, endColumn: 3));
            list.Add(new FoldMarker(doc, startLine: 11, startColumn: 1, endLine: 15, endColumn: 1));
            list.Add(new FoldMarker(doc, startLine: 12, startColumn: 1, endLine: 16, endColumn: 1));
            foreach (var fm in list)
                fm.IsFolded = true;
            doc.FoldingManager.UpdateFoldings(new List<FoldMarker>(list));
            manager = doc.FoldingManager;
        }

        private FoldingManager manager;
        private List<FoldMarker> list;

        private void AssertPosition(int line, int column, params int[] markers)
        {
            AssertList(manager.GetFoldingsFromPosition(line, column), markers);
        }

        private void AssertList(List<FoldMarker> l, params int[] markers)
        {
            Assert.AreEqual(markers.Length, l.Count);
            foreach (var m in markers)
                Assert.Contains(list[m], l);
        }

        [Test]
        public void GetFoldingsContainsLineNumber()
        {
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 1));
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 2), 0);
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 3), 0);
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 4), 0);
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 5));
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 10));
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 11), 3);
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 12), 3, 5);
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 13), 3, 5, 6);
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 14), 5, 6);
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 15), 6);
            AssertList(manager.GetFoldingsContainsLineNumber(lineNumber: 16));
        }

        [Test]
        public void GetFoldingsWithStart()
        {
            AssertList(manager.GetFoldingsWithStart(lineNumber: 1), 0);
            AssertList(manager.GetFoldingsWithStart(lineNumber: 2), 1);
            AssertList(manager.GetFoldingsWithStart(lineNumber: 3), 2);
            AssertList(manager.GetFoldingsWithStart(lineNumber: 4));
            AssertList(manager.GetFoldingsWithStart(lineNumber: 10), 3, 4);
            AssertList(manager.GetFoldingsWithStart(lineNumber: 11), 5);
            AssertList(manager.GetFoldingsWithStart(lineNumber: 12), 6);
            AssertList(manager.GetFoldingsWithStart(lineNumber: 13));
            AssertList(manager.GetFoldingsWithStart(lineNumber: 14));
            AssertList(manager.GetFoldedFoldingsWithStartAfterColumn(lineNumber: 10, column: 0), 3, 4);
            AssertList(manager.GetFoldedFoldingsWithStartAfterColumn(lineNumber: 10, column: 1), 4);
            AssertList(manager.GetFoldedFoldingsWithStartAfterColumn(lineNumber: 10, column: 2), 4);
            AssertList(manager.GetFoldedFoldingsWithStartAfterColumn(lineNumber: 10, column: 3));
            AssertList(manager.GetFoldedFoldingsWithStartAfterColumn(lineNumber: 10, column: 4));
        }

        [Test]
        public void GetFromPositionOverlapping()
        {
            AssertPosition(line: 10, column: 1);
            AssertPosition(10, 2, 3);
            AssertPosition(10, 3, 3);
            AssertPosition(10, 4, 3);
            AssertPosition(11, 1, 3);
            AssertPosition(11, 2, 3, 5);
            AssertPosition(12, 1, 3, 5);
            AssertPosition(12, 2, 3, 5, 6);
            AssertPosition(14, 0, 3, 5, 6);
            AssertPosition(14, 1, 5, 6);
            AssertPosition(15, 0, 5, 6);
            AssertPosition(15, 1, 6);
            AssertPosition(16, 0, 6);
            AssertPosition(line: 16, column: 1);
        }

        [Test]
        public void GetFromPositionTest()
        {
            AssertPosition(line: 1, column: 5);
            //AssertPosition(1, 6,  0);
            AssertPosition(1, 7, 0);
            AssertPosition(5, 0, 0);
            AssertPosition(5, 1, 0);
            AssertPosition(line: 5, column: 2);
            AssertPosition(line: 5, column: 3);
            AssertPosition(3, 8, 0, 2);
            AssertPosition(3, 30, 0, 2);
            AssertPosition(4, 0, 0, 2);
            AssertPosition(4, 1, 0);
            AssertPosition(2, 1, 0);
            AssertPosition(2, 2, 0, 1);
            AssertPosition(2, 3, 0);
        }

        [Test]
        public void GetTopLevelFoldedFoldings()
        {
            AssertList(manager.GetTopLevelFoldedFoldings(), 0, 3);
        }
    }
}