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
    /// A buffer that can be read from.
    /// </summary>
    internal class ReadBuffer
    {
        /// <summary>
        /// The buffer used. When replaced, the read position is reset and the available
        /// data size is set to the full buffer.
        /// </summary>
        public byte[] buffer
        {
            get
            {
                return m_buffer;
            }
            set
            {
                m_buffer = value;
                m_size = m_buffer.Length;
                m_carret = 0;
            }
        }

        /// <summary>
        /// Number of bytes available to read.
        /// </summary>
        public int availableBytes
        {
            get
            {
                return m_size - m_carret;
            }
        }

        private byte[] m_buffer = new byte[0];
        private int m_carret = 0;
        private int m_size = 0;

        /// <summary>
        /// Creates a ReadBuffer.
        /// </summary>
        public ReadBuffer()
        {

        }

        /// <summary>
        /// Resize the internal buffer. The read position is reset.
        /// </summary>
        public void Resize(int newSize)
        {
            m_size = newSize;
            if (m_buffer.Length < newSize)
            {
                m_buffer = new byte[newSize];
            }
            m_carret = 0;
        }

        /// <summary>
        /// Read a byte from the buffer.
        /// </summary>
        /// <param name="b"></param>
        public void Read(ref byte b)
        {
            if (m_carret + 1 > m_size)
            {
                throwException(1);
            }
            b = m_buffer[m_carret++];
        }

        /// <summary>
        /// Read a byte array from the buffer.
        /// </summary>
        /// <param name="b"></param>
        public void Read(ref byte[] bytes)
        {
            int count = 0;
            Read(ref count);
            bytes = new byte[count];
            if (m_carret + count > m_size)
            {
                throwException(count);
            }
            Array.Copy(m_buffer, m_carret, bytes, 0, count);
            m_carret += count;
        }

        /// <summary>
        /// Reset the read position.
        /// </summary>
        public void Reset()
        {
            m_carret = 0;
        }

        /// <summary>
        /// Read a bool from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public void Read(ref bool v)
        {
            if (m_carret + 1 > m_size)
            {
                throwException(1);
            }
            v = m_buffer[m_carret++] != 0;
        }

        /// <summary>
        /// Read a char from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref char v)
        {
            if (m_carret + 2 > m_size)
            {
                throwException(2);
            }
            fixed (char* p = &v)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read a float from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref float v)
        {
            if (m_carret + 4 > m_size)
            {
                throwException(4);
            }
            fixed (float* p = &v)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read a double from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref double v)
        {
            if (m_carret + 8 > m_size)
            {
                throwException(8);
            }
            fixed (double* p = &v)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
                asByte[4] = m_buffer[m_carret++];
                asByte[5] = m_buffer[m_carret++];
                asByte[6] = m_buffer[m_carret++];
                asByte[7] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read an integer from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref int v)
        {
            if (m_carret + 4 > m_size)
            {
                throwException(4);
            }
            fixed (int* p = &v)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read an unsigned integer from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref uint v)
        {
            if (m_carret + 4 > m_size)
            {
                throwException(4);
            }
            fixed (uint* p = &v)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read a long from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref long v)
        {
            if (m_carret + 8 > m_size)
            {
                throwException(8);
            }
            fixed (long* p = &v)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
                asByte[4] = m_buffer[m_carret++];
                asByte[5] = m_buffer[m_carret++];
                asByte[6] = m_buffer[m_carret++];
                asByte[7] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read an unsigned long from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref ulong v)
        {
            if (m_carret + 8 > m_size)
            {
                throwException(8);
            }
            fixed (ulong* p = &v)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
                asByte[4] = m_buffer[m_carret++];
                asByte[5] = m_buffer[m_carret++];
                asByte[6] = m_buffer[m_carret++];
                asByte[7] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read a short from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref short v)
        {
            if (m_carret + 2 > m_size)
            {
                throwException(2);
            }
            fixed (short* p = &v)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read an unsigned short from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref ushort v)
        {
            if (m_carret + 2 > m_size)
            {
                throwException(2);
            }
            fixed (ushort* p = &v)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read a Vector3 from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref Vector3 v)
        {
            if (m_carret + 12 > m_size)
            {
                throwException(12);
            }
            fixed (float* p = &v.x)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
            fixed (float* p = &v.y)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
            fixed (float* p = &v.z)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read a Vector2 from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref Vector2 v)
        {
            if (m_carret + 8 > m_size)
            {
                throwException(8);
            }
            fixed (float* p = &v.x)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
            fixed (float* p = &v.y)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read a Quaternion from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref Quaternion v)
        {
            if (m_carret + 16 > m_size)
            {
                throwException(16);
            }
            fixed (float* p = &v.x)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
            fixed (float* p = &v.y)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
            fixed (float* p = &v.z)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
            fixed (float* p = &v.w)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read a Color from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public unsafe void Read(ref Color v)
        {
            if (m_carret + 16 > m_size)
            {
                throwException(16);
            }
            fixed (float* p = &v.r)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
            fixed (float* p = &v.g)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
            fixed (float* p = &v.b)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
            fixed (float* p = &v.a)
            {
                byte* asByte = (byte*)p;
                asByte[0] = m_buffer[m_carret++];
                asByte[1] = m_buffer[m_carret++];
                asByte[2] = m_buffer[m_carret++];
                asByte[3] = m_buffer[m_carret++];
            }
        }

        /// <summary>
        /// Read a Color32 from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public void Read(ref Color32 v)
        {
            if (m_carret + 4 > m_size)
            {
                throwException(4);
            }
            v.r = m_buffer[m_carret++];
            v.g = m_buffer[m_carret++];
            v.b = m_buffer[m_carret++];
            v.a = m_buffer[m_carret++];
        }

        /// <summary>
        /// Read a string from the buffer.
        /// </summary>
        /// <param name="v"></param>
        public void Read(ref string v)
        {
            int count = 0;
            Read(ref count);
            if (m_carret + count > m_size)
            {
                throwException(count);
            }
            v = System.Text.Encoding.Default.GetString(m_buffer, m_carret, count);
            m_carret += count;
        }

        private void throwException(int sizeToRead)
        {
            throw new Exception("Reading out bound in ReadBuffer class.");
        }

    }
}
