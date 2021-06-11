using System;
using System.Collections.Generic;
using Topten.RichTextKit.Utils;

namespace Topten.RichTextKit.Editor.UndoUnits
{
    public class UndoCancelList  : UndoUnit<TextDocument>
    {
        public UndoCancelList(Tuple<int, int> paragraphRange)
        {
            _paragraphRange = paragraphRange;
        }
        public override void Do(TextDocument context)
        {
             var firstParagraph = context.Paragraphs[_paragraphRange.Item1];
             _savedListKind = firstParagraph.ListKind;
            if (_savedListKind == ListKind.NumberingList)
            {
                var paragraphIndex = firstParagraph.NumberedListIndex;
                _savedList = firstParagraph.NumberedList;
                _first = paragraphIndex == 0;

                var lastParagraph = context.Paragraphs[_paragraphRange.Item1];
                var lastIndex = lastParagraph.NumberedListIndex;
                _last = lastIndex == lastParagraph.NumberedList.Count - 1;
                
                if (!_first && !_last)
                {
                    _newList = new List<Paragraph>();
                    for (int i = lastIndex + 1; i < _savedList.Count;)
                    {
                        _newList.Add(_savedList[i]);
                        _savedList[i].NumberedList = _newList;
                        _savedList.RemoveAt(i);
                    }
                }
            }
            
            //clear range
            for (int i = _paragraphRange.Item1; i <= _paragraphRange.Item2; i++)
            {
                context.ClearNumberedListInfo(context.Paragraphs[i]);                        
            }
        }

        public override void Undo(TextDocument context)
        {
            if (_savedListKind == ListKind.NumberingList)
            {
                if (_first)
                {
                    for (int i = _paragraphRange.Item2; i >= _paragraphRange.Item1; i--)
                    {
                        var paragraph = context.Paragraphs[i];
                        paragraph.NumberedList = _savedList;
                        paragraph.ListKind = ListKind.NumberingList;
                        _savedList.Insert(0, paragraph);
                    }
                }
                else if(_last)
                {
                    for (int i = _paragraphRange.Item1; i <= _paragraphRange.Item2; i++)
                    {
                        var paragraph = context.Paragraphs[i];
                        paragraph.NumberedList = _savedList;
                        paragraph.ListKind = ListKind.NumberingList;
                        _savedList.Insert(paragraph.NumberedList.Count, paragraph);
                    }
                }
                else
                {
                    for (int i = _paragraphRange.Item1; i <= _paragraphRange.Item2; i++)
                    {
                        var paragraph = context.Paragraphs[i];
                        paragraph.ListKind = ListKind.NumberingList;
                        paragraph.NumberedList = _savedList;
                        _savedList.Add(paragraph);
                    }
                    foreach (var p in _newList)
                    {
                        _savedList.Add(p);
                        p.NumberedList = _savedList;
                    }
                }
            }
            else
            {
                for (int i = _paragraphRange.Item2; i >= _paragraphRange.Item1; i--)
                {
                    var paragraph = context.Paragraphs[i];
                    paragraph.ListKind = ListKind.BulletList;
                }
            }
        }

        private readonly Tuple<int, int> _paragraphRange;

        private ListKind _savedListKind;
        private List<Paragraph> _savedList;
        private List<Paragraph> _newList;
        private bool _first;
        private bool _last;
    }
}