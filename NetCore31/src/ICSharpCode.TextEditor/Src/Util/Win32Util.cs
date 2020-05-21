using System;
using System.Drawing;

namespace ICSharpCode.TextEditor.Util
{
    public static class Win32Util
    {
        public static Point ToPoint(this IntPtr lparam) =>
            new Point(unchecked((int)lparam.ToInt64()));
    }
}