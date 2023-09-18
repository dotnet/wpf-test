// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a Stream implementation with a string backing store.

namespace Test.Uis.IO
{
    #region Namespaces.

    using System;
    using System.IO;
    using System.Text;
    using InteropServices = System.Runtime.InteropServices;

    #endregion Namespaces.

    /// <summary>
    /// Provides a Stream implementation with a System.String backing
    /// store.
    /// </summary>
    public class StringStream: Stream
    {
        #region Constructors.

        /// <summary>
        /// Creates a new StringStream with no text and a
        /// default Unicode encoding.
        /// </summary>
        public StringStream(): this(String.Empty, new UnicodeEncoding(), false)
        {
        }

        /// <summary>
        /// Creates a new StringStream with the specified text and a
        /// default Unicode encoding.
        /// </summary>
        /// <param name='text'>Text to initialize stream with.</param>
        public StringStream(string text): this(text, new UnicodeEncoding(), false)
        {
        }

        /// <summary>
        /// Creates a new StringStream with the specified text and encoding,
        /// optionally including the encoding prefix in operations.
        /// </summary>
        /// <param name='text'>Text to initialize stream with.</param>
        /// <param name='encoding'>Encoding to and from bytes.</param>
        /// <param name='ignorePreamble'>Whether to omit the encoding preamble in streams.</param>
        public StringStream(string text, Encoding encoding, bool ignorePreamble)
        {
            this._encoding = encoding;
            this._includePreamble = !ignorePreamble;
            Text = text;
        }

        #endregion Constructors.

        #region Stream implementation.

        /// <summary>Clears all buffers for this stream.</summary>
        public override void Flush()
        {
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances
        /// the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name='buffer'>
        /// An array of bytes. When this method returns, the buffer contains
        /// the specified byte array with the values between offset and
        /// (offset + count- 1) replaced by the bytes read from the current
        /// source.
        /// </param>
        /// <param name='offset'>
        /// The zero-based byte offset in buffer at which to begin storing
        /// bytes read from the current stream.
        /// </param>
        /// <param name='count'>
        /// The maximum number of bytes to be read from the current stream.
        /// </param>
        public override int Read(
            [InteropServices.In, InteropServices.Out] byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset + count > buffer.Length)
                throw new ArgumentException("The sum of offset and count is greater than the buffer length.");

            long bytesLeft = _contents.Length - _position;
            long bytesToRead = (count > bytesLeft)? bytesLeft : count;
            if (bytesToRead == 0) return 0;
            Array.Copy(_contents, _position, buffer, offset, bytesToRead);
            _position += bytesToRead;
            return checked((int) bytesToRead);
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position
        /// within the stream by one byte, or returns -1 if at the
        /// end of the stream.
        /// </summary>
        /// <returns>
        /// The unsigned byte cast to an Int32, or -1 if at the end of the stream.
        /// </returns>
        public override int ReadByte()
        {
            if (_contents.Length == _position)
            {
                return -1;
            }
            return _contents[_position++];
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name='offset'>
        /// A byte offset relative to the origin parameter.
        /// </param>
        /// <param name='origin'>
        /// A value of type SeekOrigin indicating the reference point used to
        /// obtain the new position.
        /// </param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    break;
                case SeekOrigin.Current:
                    _position += offset;
                    break;
                case SeekOrigin.End:
                    _position = _contents.Length + offset;
                    break;
                default:
                    throw new ArgumentException("origin");
            }
            if (_position < 0) _position = 0;
            if (_position > _contents.Length) _position = _contents.Length;
            return _position;
        }

        /// <summary>Sets the length of the current stream.</summary>
        /// <param name='value'>
        /// The desired length of the current stream in bytes.
        /// </param>
        public override void SetLength(long value)
        {
            if (value == 0)
            {
                _contents = new byte[0];
                return;
            }

            byte[] newArray = new byte[value];
            long copyCount = (value < _contents.Length)? value : _contents.Length;
            Array.Copy(_contents, newArray, copyCount);
            _contents = newArray;
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the
        /// current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name='buffer'>
        /// An array of bytes. This method copies count bytes from buffer to
        /// the current stream.
        /// </param>
        /// <param name='offset'>
        /// The zero-based byte offset in buffer at which to begin copying
        /// bytes to the current stream.
        /// </param>
        /// <param name='count'>
        /// The number of bytes to be written to the current stream.
        /// </param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (offset + count > buffer.Length)
                throw new ArgumentException("The sum of offset and count is greater than the buffer length.");

            // Grow if required.
            if (_contents.Length < (_position + count))
            {
                SetLength(_position + count);
            }

            Array.Copy(buffer, offset, _contents, _position, count);
            _position += count;
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances
        /// the position within the stream by one byte
        /// </summary>
        /// <param name='value'>
        /// The byte to write to the stream.
        /// </param>
        public override void WriteByte(byte value)
        {
            // Grow if required.
            if (_position == _contents.Length)
            {
                SetLength(_position + 1);
            }
            _contents[_position++] = value;
        }

        #endregion Stream implementation.


        #region Public properties.

        /// <summary>
        /// Encoding used when getting or setting text directly.
        /// </summary>
        /// <remarks>
        /// Modifying this property on a stream with content may
        /// corrupt its data.
        /// </remarks>
        public Encoding Encoding
        {
            get { return this._encoding; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this._encoding = value;
            }
        }
        
        /// <summary>
        /// Whether to include the preamble in generated streams.
        /// </summary>
        public bool IncludePreamble
        {
            get { return this._includePreamble; }
        }

        /// <summary>
        /// Gets or sets the text in the stream. Setting the text will
        /// automatically reset the position to 0.
        /// </summary>
        public string Text
        {
            get
            {
                int charCount = _encoding.GetCharCount(
                    _contents, 0, _contents.Length);
                char[] chars = new char[charCount];
                _encoding.GetChars(_contents, 0, _contents.Length,
                    chars, 0);
                return new string(chars);
            }
            set
            {
                if (value == null) value = String.Empty;

                // Get the encoded bytes.
                byte[] encodedBytes = _encoding.GetBytes(value);
                
                // Prepend preamble bytes if requested, to form the
                // stream bytes.
                byte[] streamBytes;
                if (this._includePreamble)
                {
                    byte[] preamble = _encoding.GetPreamble();
                    streamBytes = new byte[preamble.Length + encodedBytes.Length];
                    preamble.CopyTo(streamBytes, 0);
                    encodedBytes.CopyTo(streamBytes, preamble.Length);
                }
                else
                {
                    streamBytes = encodedBytes;
                }

                // Store results in object fields.
                _contents = streamBytes;
                _position = 0;
            }
        }

        #region Stream implementation.

        /// <summary>
        /// Returns a value of true to indicate that reading is supported.
        /// </summary>
        public override bool CanRead { get { return true; } }

        /// <summary>
        /// Returns a value of true to indicate that seeking is supported.
        /// </summary>
        public override bool CanSeek { get { return true; } }

        /// <summary>
        /// Returns a value of true to indicate that writing is supported.
        /// </summary>
        public override bool CanWrite { get { return true; } }

        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get { return _contents.Length; }
        }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        public override long Position
        {
            get { return this._position; }
            set
            {
                if (value < 0) value = 0;
                if (value > _contents.Length) value = _contents.Length;
                _position = value;
            }
        }

        #endregion Stream implementation.

        #endregion Public properties.

        #region Private fields.

        private long _position;
        private byte[] _contents;
        private Encoding _encoding;
        private bool _includePreamble;

        #endregion Private fields.
    }
}
