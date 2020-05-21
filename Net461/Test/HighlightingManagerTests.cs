using ICSharpCode.TextEditor.Document;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ICSharpCode.TextEditor.Tests
{
    [TestFixture]
    public class HighlightingManagerTests
    {
        [TestCase("test.xml", "XML")]
        [TestCase("test.vcxproj.filters", "XML")] //Extension with a '.' inside
        [TestCase("test.cs", "C#")] //lowercase
        [TestCase("test.CS", "C#")] //Upper case
        [TestCase("test.htm", "HTML")]
        public void FindHighlighterForFile_Should_find_Highlight_strategy(string filename, string expectedStrategy)
        {
            IHighlightingStrategy highlightingStrategy = HighlightingManager.Manager.FindHighlighterForFile(filename);
            Assert.AreEqual(expectedStrategy, highlightingStrategy.Name);
        }

        [Test]
        public void FindHighlighterForFile_Should_not_find_Highlight_strategy()
        {
            IHighlightingStrategy highlightingStrategy = HighlightingManager.Manager.FindHighlighterForFile("test.unkown");
            Assert.AreEqual("Default", highlightingStrategy.Name);
        }

        [Test]
        public void ResourceStreamNamesMatchSyntaxNodes()
        {
            Assembly assembly = typeof(ICSharpCode.TextEditor.Document.ResourceSyntaxModeProvider).Assembly;
            var resources = assembly.GetManifestResourceNames();
            var syntaxNodesResource = resources.First(r => r.Contains("SyntaxModes"));
            var xmlModes = XElement.Load(assembly.GetManifestResourceStream(syntaxNodesResource)).Elements("Mode");
            var resourcesToCheck = resources.Where(r => r.EndsWith("xshd"));

            var matched = from xml in xmlModes
                          join res in resourcesToCheck on xml.Attribute("file").Value equals
                          res.Replace("ICSharpCode.TextEditor.Resources.", "")
                          select new { ResourceName = res, XMLModeFile = xml.Attribute("file").Value };

            var missingInXML = resourcesToCheck.Except(matched.Select(m => m.ResourceName));

            var missingInResources = from nd in xmlModes
                                     where !matched.Select(m => m.XMLModeFile).Contains(nd.Attribute("file").Value)
                                     select nd.Attribute("file").Value;

            Assert.That(!missingInXML.Any(), "The SyntaxNodes.xml file is out of sync with the actual resources. Check the following resource names that don't exist in xml file. {0}", string.Join(",", missingInXML));
            Assert.That(!missingInResources.Any(), "The SyntaxNodes.xml file is out of sync with the actual resources. Check the following resource names that don't exist in embedded resources. {0}", string.Join(",", missingInResources));
        }
    }
}
