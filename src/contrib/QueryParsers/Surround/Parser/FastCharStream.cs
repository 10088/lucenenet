﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lucene.Net.QueryParsers.Surround.Parser
{
    public sealed class FastCharStream : ICharStream
    {
        char[] buffer = null;

        int bufferLength = 0;          // end of valid chars
        int bufferPosition = 0;        // next char to read

        int tokenStart = 0;          // offset in buffer
        int bufferStart = 0;          // position in file of buffer

        TextReader input;            // source of chars

        public FastCharStream(TextReader r)
        {
            input = r;
        }

        public char ReadChar()
        {
            if (bufferPosition >= bufferLength)
                Refill();
            return buffer[bufferPosition++];
        }

        private void Refill()
        {
            int newPosition = bufferLength - tokenStart;

            if (tokenStart == 0)
            {        // token won't fit in buffer
                if (buffer == null)
                {        // first time: alloc buffer
                    buffer = new char[2048];
                }
                else if (bufferLength == buffer.Length)
                { // grow buffer
                    char[] newBuffer = new char[buffer.Length * 2];
                    Array.Copy(buffer, 0, newBuffer, 0, bufferLength);
                    buffer = newBuffer;
                }
            }
            else
            {            // shift token to front
                Array.Copy(buffer, tokenStart, buffer, 0, newPosition);
            }

            bufferLength = newPosition;        // update state
            bufferPosition = newPosition;
            bufferStart += tokenStart;
            tokenStart = 0;

            int charsRead =          // fill space in buffer
              input.Read(buffer, newPosition, buffer.Length - newPosition);
            if (charsRead == -1)
                throw new IOException("read past eof");
            else
                bufferLength += charsRead;
        }

        public char BeginToken()
        {
            tokenStart = bufferPosition;
            return ReadChar();
        }

        public void Backup(int amount)
        {
            bufferPosition -= amount;
        }

        public string Image
        {
            get { return new String(buffer, tokenStart, bufferPosition - tokenStart); }
        }

        public char[] GetSuffix(int len)
        {
            char[] value = new char[len];
            Array.Copy(buffer, bufferPosition - len, value, 0, len);
            return value;
        }

        public void Done()
        {
            try
            {
                input.Dispose();
            }
            catch (IOException)
            {
            }
        }

        public int Column
        {
            get { return bufferStart + bufferPosition; }
        }

        public int Line
        {
            get { return 1; }
        }

        public int EndColumn
        {
            get { return bufferStart + bufferPosition; }
        }

        public int EndLine
        {
            get { return 1; }
        }

        public int BeginColumn
        {
            get { return bufferStart + tokenStart; }
        }

        public int BeginLine
        {
            get { return 1; }
        }
    }
}
