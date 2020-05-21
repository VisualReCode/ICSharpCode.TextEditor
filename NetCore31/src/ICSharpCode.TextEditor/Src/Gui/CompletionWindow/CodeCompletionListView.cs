// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.TextEditor.Gui.CompletionWindow
{
    /// <summary>
    ///     Description of CodeCompletionListView.
    /// </summary>
    public class CodeCompletionListView : UserControl
    {
        private readonly ICompletionData[] completionData;
        private int firstItem;
        private int selectedItem = -1;

        public CodeCompletionListView(ICompletionData[] completionData)
        {
            Array.Sort(completionData, DefaultCompletionData.Compare);
            this.completionData = completionData;

//            this.KeyDown += new System.Windows.Forms.KeyEventHandler(OnKey);
//            SetStyle(ControlStyles.Selectable, false);
//            SetStyle(ControlStyles.UserPaint, true);
//            SetStyle(ControlStyles.DoubleBuffer, false);
        }

        public ImageList ImageList { get; set; }

        public int FirstItem
        {
            get => firstItem;
            set
            {
                if (firstItem != value)
                {
                    firstItem = value;
                    OnFirstItemChanged(EventArgs.Empty);
                }
            }
        }

        public ICompletionData SelectedCompletionData
        {
            get
            {
                if (selectedItem < 0)
                    return null;
                return completionData[selectedItem];
            }
        }

        public int ItemHeight => Math.Max(ImageList.ImageSize.Height, (int)(Font.Height*1.25));

        public int MaxVisibleItem => Height/ItemHeight;

        public void Close()
        {
            if (completionData != null)
                Array.Clear(completionData, index: 0, completionData.Length);
            base.Dispose();
        }

        public void SelectIndex(int index)
        {
            var oldSelectedItem = selectedItem;
            var oldFirstItem = firstItem;

            index = Math.Max(val1: 0, index);
            selectedItem = Math.Max(val1: 0, Math.Min(completionData.Length - 1, index));
            if (selectedItem < firstItem)
                FirstItem = selectedItem;
            if (firstItem + MaxVisibleItem <= selectedItem)
                FirstItem = selectedItem - MaxVisibleItem + 1;
            if (oldSelectedItem != selectedItem)
            {
                if (firstItem != oldFirstItem)
                {
                    Invalidate();
                }
                else
                {
                    var min = Math.Min(selectedItem, oldSelectedItem) - firstItem;
                    var max = Math.Max(selectedItem, oldSelectedItem) - firstItem;
                    Invalidate(new Rectangle(x: 0, 1 + min*ItemHeight, Width, (max - min + 1)*ItemHeight));
                }

                OnSelectedItemChanged(EventArgs.Empty);
            }
        }

        public void CenterViewOn(int index)
        {
            var oldFirstItem = FirstItem;
            var firstItem = index - MaxVisibleItem/2;
            if (firstItem < 0)
                FirstItem = 0;
            else if (firstItem >= completionData.Length - MaxVisibleItem)
                FirstItem = completionData.Length - MaxVisibleItem;
            else
                FirstItem = firstItem;
            if (FirstItem != oldFirstItem)
                Invalidate();
        }

        public void ClearSelection()
        {
            if (selectedItem < 0)
                return;
            var itemNum = selectedItem - firstItem;
            selectedItem = -1;
            Invalidate(new Rectangle(x: 0, itemNum*ItemHeight, Width, (itemNum + 1)*ItemHeight + 1));
            Update();
            OnSelectedItemChanged(EventArgs.Empty);
        }

        public void PageDown()
        {
            SelectIndex(selectedItem + MaxVisibleItem);
        }

        public void PageUp()
        {
            SelectIndex(selectedItem - MaxVisibleItem);
        }

        public void SelectNextItem()
        {
            SelectIndex(selectedItem + 1);
        }

        public void SelectPrevItem()
        {
            SelectIndex(selectedItem - 1);
        }

        public void SelectItemWithStart(string startText)
        {
            if (string.IsNullOrEmpty(startText)) return;
            var originalStartText = startText;
            startText = startText.ToLower();
            var bestIndex = -1;
            var bestQuality = -1;
            // Qualities: 0 = match start
            //            1 = match start case sensitive
            //            2 = full match
            //            3 = full match case sensitive
            double bestPriority = 0;
            for (var i = 0; i < completionData.Length; ++i)
            {
                var itemText = completionData[i].Text;
                var lowerText = itemText.ToLower();
                if (lowerText.StartsWith(startText))
                {
                    var priority = completionData[i].Priority;
                    int quality;
                    if (lowerText == startText)
                    {
                        if (itemText == originalStartText)
                            quality = 3;
                        else
                            quality = 2;
                    }
                    else if (itemText.StartsWith(originalStartText))
                    {
                        quality = 1;
                    }
                    else
                    {
                        quality = 0;
                    }

                    bool useThisItem;
                    if (bestQuality < quality)
                    {
                        useThisItem = true;
                    }
                    else
                    {
                        if (bestIndex == selectedItem)
                            useThisItem = false;
                        else if (i == selectedItem)
                            useThisItem = bestQuality == quality;
                        else
                            useThisItem = bestQuality == quality && bestPriority < priority;
                    }

                    if (useThisItem)
                    {
                        bestIndex = i;
                        bestPriority = priority;
                        bestQuality = quality;
                    }
                }
            }

            if (bestIndex < 0)
            {
                ClearSelection();
            }
            else
            {
                if (bestIndex < firstItem || firstItem + MaxVisibleItem <= bestIndex)
                {
                    SelectIndex(bestIndex);
                    CenterViewOn(bestIndex);
                }
                else
                {
                    SelectIndex(bestIndex);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            float yPos = 1;
            float itemHeight = ItemHeight;
            // Maintain aspect ratio
            var imageWidth = (int)(itemHeight*ImageList.ImageSize.Width/ImageList.ImageSize.Height);

            var curItem = firstItem;
            var g = pe.Graphics;
            while (curItem < completionData.Length && yPos < Height)
            {
                var drawingBackground = new RectangleF(x: 1, yPos, Width - 2, itemHeight);
                if (drawingBackground.IntersectsWith(pe.ClipRectangle))
                {
                    // draw Background
                    if (curItem == selectedItem)
                        g.FillRectangle(SystemBrushes.Highlight, drawingBackground);
                    else
                        g.FillRectangle(SystemBrushes.Window, drawingBackground);

                    // draw Icon
                    var xPos = 0;
                    if (completionData[curItem].ImageIndex < ImageList.Images.Count)
                    {
                        g.DrawImage(ImageList.Images[completionData[curItem].ImageIndex], new RectangleF(x: 1, yPos, imageWidth, itemHeight));
                        xPos = imageWidth;
                    }

                    // draw text
                    if (curItem == selectedItem)
                        g.DrawString(completionData[curItem].Text, Font, SystemBrushes.HighlightText, xPos, yPos);
                    else
                        g.DrawString(completionData[curItem].Text, Font, SystemBrushes.WindowText, xPos, yPos);
                }

                yPos += itemHeight;
                ++curItem;
            }

            g.DrawRectangle(SystemPens.Control, new Rectangle(x: 0, y: 0, Width - 1, Height - 1));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            float yPos = 1;
            var curItem = firstItem;
            float itemHeight = ItemHeight;

            while (curItem < completionData.Length && yPos < Height)
            {
                var drawingBackground = new RectangleF(x: 1, yPos, Width - 2, itemHeight);
                if (drawingBackground.Contains(e.X, e.Y))
                {
                    SelectIndex(curItem);
                    break;
                }

                yPos += itemHeight;
                ++curItem;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pe)
        {
        }

        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            SelectedItemChanged?.Invoke(this, e);
        }

        protected virtual void OnFirstItemChanged(EventArgs e)
        {
            FirstItemChanged?.Invoke(this, e);
        }

        public event EventHandler SelectedItemChanged;
        public event EventHandler FirstItemChanged;
    }
}