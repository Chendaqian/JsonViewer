using System;
using System.IO;

namespace Json.Viewer
{
    [Serializable]
    public class UnbufferedStringReader : TextReader
    {
        // Fields
        private int _length;

        private string _s;

        public int Position { get; private set; }

        // Methods
        public UnbufferedStringReader(string s)
        {
            _s = s ?? throw new ArgumentNullException(nameof(s));
            _length = s.Length;
        }

        public override void Close()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            _s = null;
            Position = 0;
            _length = 0;
            base.Dispose(disposing);
        }

        public override int Peek()
        {
            if (_s == null)
                throw new Exception("object closed");

            if (Position == _length)
                return -1;

            return _s[Position];
        }

        public override int Read()
        {
            if (_s == null)
                throw new Exception("object closed");

            if (Position == _length)
                return -1;

            return _s[Position++];
        }

        public override int Read(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (buffer.Length - index < count)
            {
                throw new ArgumentException("invalid offset length");
            }
            if (_s == null)
            {
                throw new Exception("object closed");
            }
            int num = _length - Position;
            if (num > 0)
            {
                if (num > count)
                {
                    num = count;
                }
                _s.CopyTo(Position, buffer, index, num);
                Position += num;
            }
            return num;
        }

        public override string ReadLine()
        {
            if (_s == null)
                throw new Exception("object closed");

            int num = Position;
            while (num < _length)
            {
                char ch = _s[num];
                switch (ch)
                {
                    case '\r':
                    case '\n':
                    {
                        string text = _s.Substring(Position, num - Position);
                        Position = num + 1;
                        if (ch == '\r' && Position < _length && _s[Position] == '\n')
                            Position++;

                        return text;
                    }
                }
                num++;
            }

            if (num <= Position)
                return null;

            string text2 = _s.Substring(Position, num - Position);
            Position = num;
            return text2;
        }

        public override string ReadToEnd()
        {
            if (_s == null)
                throw new Exception("object closed");

            string text = Position == 0 ? _s : _s.Substring(Position, _length - Position);

            Position = _length;
            return text;
        }
    }
}