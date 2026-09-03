/*************************************************************************
 *
 * Copyright 2026, Mechdyne Corporation
 * ALL RIGHTS RESERVED
 *
 * UNPUBLISHED -- Rights reserved under the copyright laws of the United
 * States. Use of a copyright notice is precautionary only and does not
 * imply publication or disclosure.
 *
 * THE CONTENT OF THIS WORK CONTAINS CONFIDENTIAL AND PROPRIETARY
 * INFORMATION OF MECHDYNE CORPORATION. ANY DUPLICATION, MODIFICATION,
 * DISTRIBUTION, OR DISCLOSURE IN ANY FORM, IN WHOLE, OR IN PART, IS
 * STRICTLY PROHIBITED WITHOUT THE PRIOR EXPRESS WRITTEN PERMISSION OF
 * MECHDYNE CORPORATION.
 *
 * Version 4.5.0.4237
 *
 ************************************************************************/

using System;
using UnityEngine;

namespace getReal3D
{
    /// <summary>
    /// A buffer that can be written into.
    /// </summary>
    internal class WriteBuffer
    {
        /// <summary>
        /// Return the size of the written data.
        /// </summary>
        public int size
        {
            get
            {
                return m_carret;
            }
        }

        /// <summary>
        /// Return the data. Do not attempt to modify!
        /// </summary>
        public byte[] buffer
        {
            get
            {
                return m_buffer;
            }
        }

        private byte[] m_buffer = new byte[0];
        private int m_carret = 0;

        /// <summary>
        /// Create a WriteBuffer.
        /// </summary>
        public WriteBuffer()
        {

        }

        /// <summary>
        /// Reset the WriteBuffer.
        /// </summary>
        public void Reset()
        {
            m_carret = 0;
        }

        /// <summary>
        /// Write a byte to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public void WriteByte(byte v)
        {
            if (1 + m_carret > m_buffer.Length)
            {
                Grow(1);
            }
            m_buffer[m_carret++] = v;
        }

        /// <summary>
        /// Write a byte array to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public void WriteBytes(byte[] v)
        {
            WriteInteger(v.Length);
            if (v.Length + m_carret > m_buffer.Length)
            {
                Grow(v.Length);
            }
            v.CopyTo(m_buffer, m_carret);
            m_carret += v.Length;
        }

        /// <summary>
        /// Write a bool to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteBool(bool v)
        {
            if (1 + m_carret > m_buffer.Length)
            {
                Grow(1);
            }
            m_buffer[m_carret++] = (byte)(v ? 1 : 0);
        }

        /// <summary>
        /// Write an integer to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteInteger(int v)
        {
            if (4 + m_carret > m_buffer.Length)
            {
                Grow(4);
            }
            byte* asByte = (byte*)&v;
            m_buffer[m_carret++] = asByte[0];
            m_buffer[m_carret++] = asByte[1];
            m_buffer[m_carret++] = asByte[2];
            m_buffer[m_carret++] = asByte[3];
        }

        /// <summary>
        /// Write a float to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteFloat(float v)
        {
            if (4 + m_carret > m_buffer.Length)
            {
                Grow(4);
            }
            byte* asByte = (byte*)&v;
            m_buffer[m_carret++] = asByte[0];
            m_buffer[m_carret++] = asByte[1];
            m_buffer[m_carret++] = asByte[2];
            m_buffer[m_carret++] = asByte[3];
        }

        /// <summary>
        /// Write a char to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteChar(char v)
        {
            if (2 + m_carret > m_buffer.Length)
            {
                Grow(2);
            }
            byte* asByte = (byte*)&v;
            m_buffer[m_carret++] = asByte[0];
            m_buffer[m_carret++] = asByte[1];
        }

        /// <summary>
        /// Write a double to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteDouble(double v)
        {
            if (8 + m_carret > m_buffer.Length)
            {
                Grow(8);
            }
            byte* asByte = (byte*)&v;
            m_buffer[m_carret++] = asByte[0];
            m_buffer[m_carret++] = asByte[1];
            m_buffer[m_carret++] = asByte[2];
            m_buffer[m_carret++] = asByte[3];
            m_buffer[m_carret++] = asByte[4];
            m_buffer[m_carret++] = asByte[5];
            m_buffer[m_carret++] = asByte[6];
            m_buffer[m_carret++] = asByte[7];
        }

        /// <summary>
        /// Write a short to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteShort(short v)
        {
            if (2 + m_carret > m_buffer.Length)
            {
                Grow(2);
            }
            byte* asByte = (byte*)&v;
            m_buffer[m_carret++] = asByte[0];
            m_buffer[m_carret++] = asByte[1];
        }

        /// <summary>
        /// Write an unsigned integer to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteUInteger(uint v)
        {
            if (4 + m_carret > m_buffer.Length)
            {
                Grow(4);
            }
            byte* asByte = (byte*)&v;
            m_buffer[m_carret++] = asByte[0];
            m_buffer[m_carret++] = asByte[1];
            m_buffer[m_carret++] = asByte[2];
            m_buffer[m_carret++] = asByte[3];
        }

        /// <summary>
        /// Write a long to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteLong(long v)
        {
            if (8 + m_carret > m_buffer.Length)
            {
                Grow(8);
            }
            byte* asByte = (byte*)&v;
            m_buffer[m_carret++] = asByte[0];
            m_buffer[m_carret++] = asByte[1];
            m_buffer[m_carret++] = asByte[2];
            m_buffer[m_carret++] = asByte[3];
            m_buffer[m_carret++] = asByte[4];
            m_buffer[m_carret++] = asByte[5];
            m_buffer[m_carret++] = asByte[6];
            m_buffer[m_carret++] = asByte[7];
        }

        /// <summary>
        /// Write an unsigned long to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteULong(ulong v)
        {
            if (8 + m_carret > m_buffer.Length)
            {
                Grow(8);
            }
            byte* asByte = (byte*)&v;
            m_buffer[m_carret++] = asByte[0];
            m_buffer[m_carret++] = asByte[1];
            m_buffer[m_carret++] = asByte[2];
            m_buffer[m_carret++] = asByte[3];
            m_buffer[m_carret++] = asByte[4];
            m_buffer[m_carret++] = asByte[5];
            m_buffer[m_carret++] = asByte[6];
            m_buffer[m_carret++] = asByte[7];
        }

        /// <summary>
        /// Write an unsigned short  to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteUShort(ushort v)
        {
            if (2 + m_carret > m_buffer.Length)
            {
                Grow(2);
            }
            byte* asByte = (byte*)&v;
            m_buffer[m_carret++] = asByte[0];
            m_buffer[m_carret++] = asByte[1];
        }

        /// <summary>
        /// Write a string to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public void WriteString(string v)
        {
            byte[] bytes = System.Text.Encoding.Default.GetBytes(v);
            WriteInteger(bytes.Length);
            if (bytes.Length + m_carret > m_buffer.Length)
            {
                Grow(bytes.Length);
            }
            bytes.CopyTo(m_buffer, m_carret);
            m_carret += bytes.Length;
        }

        /// <summary>
        /// Write a Vector2 to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteVector2(Vector2 v)
        {
            if (8 + m_carret > m_buffer.Length)
            {
                Grow(8);
            }
            byte* x = (byte*)&v.x;
            m_buffer[m_carret++] = x[0];
            m_buffer[m_carret++] = x[1];
            m_buffer[m_carret++] = x[2];
            m_buffer[m_carret++] = x[3];
            byte* y = (byte*)&v.y;
            m_buffer[m_carret++] = y[0];
            m_buffer[m_carret++] = y[1];
            m_buffer[m_carret++] = y[2];
            m_buffer[m_carret++] = y[3];
        }

        /// <summary>
        /// Write a Vector3 to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteVector3(Vector3 v)
        {
            if (12 + m_carret > m_buffer.Length)
            {
                Grow(12);
            }
            byte* x = (byte*)&v.x;
            m_buffer[m_carret++] = x[0];
            m_buffer[m_carret++] = x[1];
            m_buffer[m_carret++] = x[2];
            m_buffer[m_carret++] = x[3];
            byte* y = (byte*)&v.y;
            m_buffer[m_carret++] = y[0];
            m_buffer[m_carret++] = y[1];
            m_buffer[m_carret++] = y[2];
            m_buffer[m_carret++] = y[3];
            byte* z = (byte*)&v.z;
            m_buffer[m_carret++] = z[0];
            m_buffer[m_carret++] = z[1];
            m_buffer[m_carret++] = z[2];
            m_buffer[m_carret++] = z[3];
        }

        /// <summary>
        /// Write a quaternion to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteQuaternion(Quaternion v)
        {
            if (16 + m_carret > m_buffer.Length)
            {
                Grow(16);
            }
            byte* x = (byte*)&v.x;
            m_buffer[m_carret++] = x[0];
            m_buffer[m_carret++] = x[1];
            m_buffer[m_carret++] = x[2];
            m_buffer[m_carret++] = x[3];
            byte* y = (byte*)&v.y;
            m_buffer[m_carret++] = y[0];
            m_buffer[m_carret++] = y[1];
            m_buffer[m_carret++] = y[2];
            m_buffer[m_carret++] = y[3];
            byte* z = (byte*)&v.z;
            m_buffer[m_carret++] = z[0];
            m_buffer[m_carret++] = z[1];
            m_buffer[m_carret++] = z[2];
            m_buffer[m_carret++] = z[3];
            byte* w = (byte*)&v.w;
            m_buffer[m_carret++] = w[0];
            m_buffer[m_carret++] = w[1];
            m_buffer[m_carret++] = w[2];
            m_buffer[m_carret++] = w[3];
        }

        /// <summary>
        /// Write a Color32 to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public void WriteColor32(Color32 v)
        {
            if (4 + m_carret > m_buffer.Length)
            {
                Grow(4);
            }
            m_buffer[m_carret++] = v.r;
            m_buffer[m_carret++] = v.g;
            m_buffer[m_carret++] = v.b;
            m_buffer[m_carret++] = v.a;
        }

        /// <summary>
        /// Write a Color to the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void WriteColor(Color v)
        {
            if (16 + m_carret > m_buffer.Length)
            {
                Grow(16);
            }
            byte* r = (byte*)&v.r;
            m_buffer[m_carret++] = r[0];
            m_buffer[m_carret++] = r[1];
            m_buffer[m_carret++] = r[2];
            m_buffer[m_carret++] = r[3];
            byte* g = (byte*)&v.g;
            m_buffer[m_carret++] = g[0];
            m_buffer[m_carret++] = g[1];
            m_buffer[m_carret++] = g[2];
            m_buffer[m_carret++] = g[3];
            byte* b = (byte*)&v.b;
            m_buffer[m_carret++] = b[0];
            m_buffer[m_carret++] = b[1];
            m_buffer[m_carret++] = b[2];
            m_buffer[m_carret++] = b[3];
            byte* a = (byte*)&v.a;
            m_buffer[m_carret++] = a[0];
            m_buffer[m_carret++] = a[1];
            m_buffer[m_carret++] = a[2];
            m_buffer[m_carret++] = a[3];
        }

        private void Grow(int s)
        {
            Resize(Math.Max(m_buffer.Length + s, m_buffer.Length * 2));
        }

        private void Resize(int newSize)
        {
            var newBuffer = new byte[newSize];
            int dataToCopy = Math.Min(newSize, m_buffer.Length);
            Array.Copy(m_buffer, newBuffer, dataToCopy);
            m_buffer = newBuffer;
        }
    }

}
