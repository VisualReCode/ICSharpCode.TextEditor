// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.TextEditor
{
    public class TextAreaDragDropHandler
    {
        public static Action<Exception> OnDragDropException = ex => MessageBox.Show(ex.ToString());

        private TextArea textArea;

        public void Attach(TextArea textArea)
        {
            this.textArea = textArea;
            textArea.AllowDrop = true;

            textArea.DragEnter += MakeDragEventHandler(OnDragEnter);
            textArea.DragDrop += MakeDragEventHandler(OnDragDrop);
            textArea.DragOver += MakeDragEventHandler(OnDragOver);
        }

        /// <summary>
        ///     Create a drag'n'drop event handler.
        ///     Windows Forms swallows unhandled exceptions during drag'n'drop, so we report them here.
        /// </summary>
        private static DragEventHandler MakeDragEventHandler(DragEventHandler h)
        {
            return (sender, e) =>
            {
                try
                {
                    h(sender, e);
                }
                catch (Exception ex)
                {
                    OnDragDropException(ex);
                }
            };
        }

        private static DragDropEffects GetDragDropEffect(DragEventArgs e)
        {
            if ((e.AllowedEffect & DragDropEffects.Move) > 0 &&
                (e.AllowedEffect & DragDropEffects.Copy) > 0)
                return (e.KeyState & 8) > 0 ? DragDropEffects.Copy : DragDropEffects.Move;

            if ((e.AllowedEffect & DragDropEffects.Move) > 0)
                return DragDropEffects.Move;

            if ((e.AllowedEffect & DragDropEffects.Copy) > 0)
                return DragDropEffects.Copy;
            return DragDropEffects.None;
        }

        protected void OnDragEnter(object sender, DragEventArgs e)
        {
            if (IsSupportedData(e.Data))
                e.Effect = GetDragDropEffect(e);
        }

        private void InsertString(int offset, string str)
        {
            if (str == null)
                return;

            textArea.Document.Insert(offset, str);

            textArea.SelectionManager.SetSelection(
                new DefaultSelection(
                    textArea.Document,
                    textArea.Document.OffsetToPosition(offset),
                    textArea.Document.OffsetToPosition(offset + str.Length)));
            textArea.Caret.Position = textArea.Document.OffsetToPosition(offset + str.Length);
            textArea.Refresh();
        }

        protected void OnDragDrop(object sender, DragEventArgs e)
        {
            if (!IsSupportedData(e.Data))
            {
                return;
            }

            textArea.BeginUpdate();
            textArea.Document.UndoStack.StartUndoGroup();
            try
            {
                var offset = textArea.Caret.Offset;
                if (textArea.IsReadOnly(offset))
                    return;

                try
                {
                    if (e.Data.GetDataPresent(typeof(DefaultSelection)))
                    {
                        var sel = (ISelection)e.Data.GetData(typeof(DefaultSelection));
                        if (sel.ContainsPosition(textArea.Caret.Position))
                            return;
                        if (GetDragDropEffect(e) == DragDropEffects.Move)
                        {
                            if (SelectionManager.SelectionIsReadOnly(textArea.Document, sel))
                                return;
                            var len = sel.Length;
                            textArea.Document.Remove(sel.Offset, len);
                            if (sel.Offset < offset)
                                offset -= len;
                        }
                    }
                }
                catch (System.InvalidCastException)
                {
                    /*
                        If GetDataPresent(typeof(DefaultSelection)) threw this
                        exception, then it's an interprocess DefaultSelection
                        COM object that is not serializable! In general,
                        GetDataPresent(typeof(...)) throws InvalidCastException
                        for drags and drops from other GitExt processes (maybe
                        we need to make the data objects [Serializable]?). We
                        can get around this exception by doing
                        GetDataPresent(String s) [using the string of the type
                        name seems to work fine!] Since it is interprocess
                        data, just get the string data from it - special
                        handling logic in try {} is only valid for selections
                        within the current process's text editor!
                    */
                }
                    
                textArea.SelectionManager.ClearSelection();
                InsertString(offset, (string)e.Data.GetData("System.String"));
                textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
            }
            finally
            {
                textArea.Document.UndoStack.EndUndoGroup();
                textArea.EndUpdate();
            }
        }

        protected void OnDragOver(object sender, DragEventArgs e)
        {
            if (!textArea.Focused)
                textArea.Focus();

            var p = textArea.PointToClient(new Point(e.X, e.Y));

            if (textArea.TextView.DrawingPosition.Contains(p.X, p.Y))
            {
                var realmousepos = textArea.TextView.GetLogicalPosition(
                    p.X - textArea.TextView.DrawingPosition.X,
                    p.Y - textArea.TextView.DrawingPosition.Y);
                var lineNr = Math.Min(textArea.Document.TotalNumberOfLines - 1, Math.Max(val1: 0, realmousepos.Y));

                textArea.Caret.Position = new TextLocation(realmousepos.X, lineNr);
                textArea.SetDesiredColumn();
                if (IsSupportedData(e.Data) && !textArea.IsReadOnly(textArea.Caret.Offset))
                    e.Effect = GetDragDropEffect(e);
                else
                    e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private static bool IsSupportedData(IDataObject data)
        {
            return data.GetDataPresent(DataFormats.StringFormat)
                   || data.GetDataPresent(DataFormats.Text)
                   || data.GetDataPresent(DataFormats.UnicodeText);
        }
    }
}