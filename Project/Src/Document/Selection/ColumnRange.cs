﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.TextEditor.Document
{
    public class ColumnRange
    {
        public static readonly ColumnRange NoColumn    = new ColumnRange(-2, -2);
        public static readonly ColumnRange WholeColumn = new ColumnRange(-1, -1);

        public int StartColumn { get; set; }

        public int EndColumn { get; set; }

        public ColumnRange(int startColumn, int endColumn)
        {
            StartColumn = startColumn;
            EndColumn = endColumn;

        }

        public override int GetHashCode()
        {
            return StartColumn + (EndColumn << 16);
        }

        public override bool Equals(object obj)
        {
            if (obj is ColumnRange range) {
                return range.StartColumn == StartColumn &&
                       range.EndColumn == EndColumn;

            }
            return false;
        }

        public override string ToString()
        {
            return String.Format("[ColumnRange: StartColumn={0}, EndColumn={1}]", StartColumn, EndColumn);
        }

    }
}
