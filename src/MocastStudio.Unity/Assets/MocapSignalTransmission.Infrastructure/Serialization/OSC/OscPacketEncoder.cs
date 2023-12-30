// The original source code is available on GitHub.
// https://github.com/sotanmochi/OscJack/blob/2.1.1/src/OscJack/Runtime/Base/Internal/OscPacketEncoder.cs
//
// -----
//
// This is free and unencumbered software released into the public domain.
//
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
//
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// For more information, please refer to <http://unlicense.org/>
//
// -----

using System;

namespace MocapSignalTransmission.Infrastructure.Transmitter.Serialization
{
    public sealed class OscPacketEncoder
    {
        public Byte[] Buffer { get { return _buffer; } }
        public int Length { get { return _length; } }

        public void Clear()
        {
            _length = 0;
        }

        public void Append(string data)
        {
            var len = data.Length;
            for (var i = 0; i < len; i++)
                _buffer[_length++] = (Byte)data[i];

            var len4 = OscDataTypes.Align4(len + 1);
            for (var i = len; i < len4; i++)
                _buffer[_length++] = 0;
        }

        public void Append(int data)
        {
            _buffer[_length++] = (Byte)(data >> 24);
            _buffer[_length++] = (Byte)(data >> 16);
            _buffer[_length++] = (Byte)(data >>  8);
            _buffer[_length++] = (Byte)(data);
        }

        public void Append(float data)
        {
            _tempFloat[0] = data;
            System.Buffer.BlockCopy(_tempFloat, 0, _tempByte, 0, 4);
            _buffer[_length++] = _tempByte[3];
            _buffer[_length++] = _tempByte[2];
            _buffer[_length++] = _tempByte[1];
            _buffer[_length++] = _tempByte[0];
        }

        Byte[] _buffer = new Byte[4096];
        int _length;

        // Used to change byte order
        static float[] _tempFloat = new float[1];
        static Byte[] _tempByte = new Byte[4];
    }
}
