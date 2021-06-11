using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using Topten.RichTextKit.Utils;

namespace Topten.RichTextKit.Editor.UndoUnits
{
    public class UndoCreateList : UndoUnit<TextDocument>
    {
        public UndoCreateList(Tuple<int, int> paragraphRange, ListKind listKind)
        {
            _paragraphRange = paragraphRange;
            _listKind = listKind;
        }
        public override void Do(TextDocument context)
        {
            _savedListKind = context.Paragraphs[_paragraphRange.Item1].ListKind;
            
            if (_listKind == ListKind.BulletList)
            {
                for (var index = _paragraphRange.Item1; index <= _paragraphRange.Item2; index++)
                {
                    var paragraph = context.Paragraphs[index];
                    context.ClearNumberedListInfo(paragraph);
                    paragraph.ListKind = ListKind.BulletList;
                }    
            }
            else
            {
                List<Paragraph> list = new List<Paragraph>();
                    
                for (var index = _paragraphRange.Item1; index <= _paragraphRange.Item2; index++)
                {
                    list.Add(context.Paragraphs[index]);
                    context.Paragraphs[index].ListKind = ListKind.NumberingList;
                    context.Paragraphs[index].NumberedList = list;
                }   
            }
        }

        public override void Undo(TextDocument context)
        {
            for (var index = _paragraphRange.Item1; index <= _paragraphRange.Item2; index++)
            {
                context.ClearNumberedListInfo(context.Paragraphs[index]);
            }
            
            if (_savedListKind == ListKind.BulletList)
            {
                for (var index = _paragraphRange.Item1; index <= _paragraphRange.Item2; index++)
                {
                    context.Paragraphs[index].ListKind = ListKind.BulletList;
                }               
            }
            else if(_savedListKind == ListKind.NumberingList)
            {
                List<Paragraph> list = new List<Paragraph>();
                    
                for (var index = _paragraphRange.Item1; index <= _paragraphRange.Item2; index++)
                {
                    list.Add(context.Paragraphs[index]);
                    context.Paragraphs[index].ListKind = ListKind.NumberingList;
                    context.Paragraphs[index].NumberedList = list;
                }                   
            }
        }

        private ListKind _savedListKind;
        private readonly Tuple<int, int> _paragraphRange;
        private readonly ListKind _listKind;
    }
}