﻿// RichTextKit
// Copyright © 2019-2020 Topten Software. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may 
// not use this product except in compliance with the License. You may obtain 
// a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
// License for the specific language governing permissions and limitations 
// under the License.

using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using Topten.RichTextKit.Utils;

namespace Topten.RichTextKit.Editor
{
    /// <summary>
    /// Implements a text paragraph
    /// </summary>
    public class TextParagraph : Paragraph
    {
        /// <summary>
        /// Constructs a new TextParagraph
        /// </summary>
        public TextParagraph(IStyle style)
        {
            _textBlock = new TextBlock();
            _textBlock.AddText("\u2029", style);
        }

        // Create a new textblock by copying the content of another
        public TextParagraph(TextParagraph source, int from, int length)
        {
            // Copy the text block
            _textBlock = source.TextBlock.Copy(from, length);

            // Copy styles
            CopyStyleFrom(source);
        }

        /// <inheritdoc />
        public override void Layout(TextDocument owner)
        {
            // For layout just need to set the appropriate layout width on the text block
            if (owner.LineWrap)
            {
                _textBlock.MaxWidth = owner.PageWidth - ListContentXCoord;
                // - owner.MarginLeft - owner.MarginRight
                // - this.MarginLeft - this.MarginRight;
            }
            else
                _textBlock.MaxWidth = null;
        }

        /// <inheritdoc />
        public override void Paint(SKCanvas canvas, TextPaintOptions options)
        {
            if (ListKind == ListKind.BulletList && FirstParaInList)
            {
                using (var paint = new SKPaint())
                {
                    paint.Color = BulletColor;
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawCircle(new SKPoint(BulletCenterXCoord, BulletCenterYCoord), BulletRadius, paint);
                }
            }
            else if(ListKind == ListKind.NumberingList && FirstParaInList)
            {
                canvas.Save();
                canvas.Translate(NumberingListContentXCoord, ContentYCoord);
                
                var ctx = new PaintTextContext
                {
                    Canvas = canvas,
                    Options = TextPaintOptions.Default,
                };
                NumberedListRun.Paint(ctx);
                
                canvas.Restore();
            }

            _textBlock.Paint(canvas, new SKPoint(ContentXCoord, ContentYCoord), options);
        }

        /// <inheritdoc />
        public override CaretInfo GetCaretInfo(CaretPosition position) => _textBlock.GetCaretInfo(position);

        /// <inheritdoc />
        public override HitTestResult HitTest(float x, float y) => _textBlock.HitTest(x, y);

        /// <inheritdoc />
        public override HitTestResult HitTestLine(int lineIndex, float x) => _textBlock.HitTestLine(lineIndex, x);

        /// <inheritdoc />
        public override IReadOnlyList<int> CaretIndicies => _textBlock.CaretIndicies;

        /// <inheritdoc />
        public override IReadOnlyList<int> WordBoundaryIndicies => _textBlock.WordBoundaryIndicies;

        /// <inheritdoc />
        public override IReadOnlyList<int> LineIndicies => _textBlock.LineIndicies;

        /// <inheritdoc />
        public override int Length => _textBlock.Length;

        /// <inheritdoc />
        public override float ContentWidth => _textBlock.MeasuredWidth + (ListKind != ListKind.None ? ListContentXCoord : 0);

        /// <inheritdoc />
        public override float ContentHeight => _textBlock.MeasuredHeight;

        /// <inheritdoc />
        public override TextBlock TextBlock => _textBlock;

        /// <inheritdoc />
        public override void CopyStyleFrom(Paragraph other)
        {
            base.CopyStyleFrom(other);
            _textBlock.Alignment = other.TextBlock.Alignment;
            _textBlock.BaseDirection = other.TextBlock.BaseDirection;
        }
        
        public override bool IsEmpty {
            get
            {
                if (Length <= 0)
                {
                    return true;
                }

                return Length == 1 && TextBlock.FontRuns.First().RunKind == FontRunKind.TrailingWhitespace;
            }
        }

        // Private attributes
        TextBlock _textBlock;
    }
}
