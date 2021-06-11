using Topten.RichTextKit.Utils;

namespace Topten.RichTextKit.Editor.UndoUnits
{
    class UndoDeleteParagraph : UndoUnit<TextDocument>
    {
        public UndoDeleteParagraph(int index)
        {
            _index = index;
        }

        public override void Do(TextDocument context)
        {
            _paragraph = context._paragraphs[_index];
            context.Paragraphs.RemoveAt(_index);
            if (_paragraph.NumberedListIndex >= 0)
            {
                _numberedListIndex = _paragraph.NumberedListIndex;
                _paragraph.NumberedList.Remove(_paragraph);
            }
        }

        public override void Undo(TextDocument context)
        {
            context._paragraphs.Insert(_index, _paragraph);
            _paragraph.NumberedList?.Insert(_numberedListIndex, _paragraph);
        }

        int _index;
        Paragraph _paragraph;
        int _numberedListIndex;
    }
}
