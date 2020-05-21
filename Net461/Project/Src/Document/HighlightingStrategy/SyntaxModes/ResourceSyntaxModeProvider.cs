// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System.Collections.Generic;
using System.Xml;

namespace ICSharpCode.TextEditor.Document
{
    public class ResourceSyntaxModeProvider : ISyntaxModeFileProvider
    {
        private readonly List<SyntaxMode> syntaxModes;

        public ResourceSyntaxModeProvider()
        {
            var assembly = typeof(SyntaxMode).Assembly;
            var syntaxModeStream = assembly.GetManifestResourceStream("ICSharpCode.TextEditor.Resources.SyntaxModes.xml");
            if (syntaxModeStream != null)
                syntaxModes = SyntaxMode.GetSyntaxModes(syntaxModeStream);
            else
                syntaxModes = new List<SyntaxMode>();
        }

        public ICollection<SyntaxMode> SyntaxModes => syntaxModes;

        public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
        {
            var assembly = typeof(SyntaxMode).Assembly;
            var stream = assembly.GetManifestResourceStream($"ICSharpCode.TextEditor.Resources.{syntaxMode.FileName}");
            return new XmlTextReader(stream);
        }

        public void UpdateSyntaxModeList()
        {
            // resources don't change during runtime
        }
    }
}