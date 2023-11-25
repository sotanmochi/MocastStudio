// The original source code is available on GitHub.
// https://github.com/sotanmochi/OscJack/blob/2.1.1/src/OscJack/Runtime/Base/Internal/OscDataTypes.cs
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
using System.Text;

namespace MocapSignalTransmission.Infrastructure.Transmitter.Serialization
{
    internal static class OscDataTypes
    {
        static Byte[] _temp4 = new Byte[4]; // only used to reverse byte order

        public static bool IsSupportedTag(char tag)
        {
            return tag == 'i' || tag == 'f' || tag == 's' || tag == 'b';
        }

        public static int Align4(int length)
        {
            return (length + 3) & ~3;
        }

        public static int ReadInt(Byte[] buffer, int offset)
        {
            return (buffer[offset + 0] << 24) +
                   (buffer[offset + 1] << 16) +
                   (buffer[offset + 2] <<  8) +
                   (buffer[offset + 3]);
        }

        public static float ReadFloat(Byte[] buffer, int offset)
        {
            _temp4[0] = buffer[offset + 3];
            _temp4[1] = buffer[offset + 2];
            _temp4[2] = buffer[offset + 1];
            _temp4[3] = buffer[offset    ];
            return BitConverter.ToSingle(_temp4, 0);
        }

        public static int GetStringSize(Byte[] buffer, int offset)
        {
            var length = 0;
            while (buffer[offset + length] != 0) length++;
            return Align4(offset + length + 1) - offset;
        }

        public static string ReadString(Byte[] buffer, int offset)
        {
            var length = 0;
            while (buffer[offset + length] != 0) length++;
            return Encoding.UTF8.GetString(buffer, offset, length);
        }
    }
}
