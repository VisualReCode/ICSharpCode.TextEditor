// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;

namespace ICSharpCode.TextEditor
{
	public delegate void ToolTipRequestEventHandler(object sender, ToolTipRequestEventArgs e);
	
	public class ToolTipRequestEventArgs
	{
	    public Point MousePosition { get; }

	    public TextLocation LogicalPosition { get; }

	    public bool InDocument { get; }

	    /// <summary>
		/// Gets if some client handling the event has already shown a tool tip.
		/// </summary>
		public bool ToolTipShown => toolTipText != null;

	    internal string toolTipText;
		
		public void ShowToolTip(string text)
		{
			toolTipText = text;
		}
		
		public ToolTipRequestEventArgs(Point mousePosition, TextLocation logicalPosition, bool inDocument)
		{
			this.MousePosition = mousePosition;
			this.LogicalPosition = logicalPosition;
			this.InDocument = inDocument;
		}
	}
}
