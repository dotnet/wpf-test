// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 2 $
 
********************************************************************/

using Avalon.Test.CoreUI.Trusted;
using System;
using System.IO;
using System.Threading;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// This class is a stream that limits reads past a specified position.  
    /// If you try to read beyond this point, the read call will block.  This can be
    /// used to simulate someone writing slowly to a stream by slowly adding to ReadLimit
    /// </summary>
    internal class ThrottledReadStream : Stream
    {
        #region Constructors
		
        /// <summary>
        /// Wraps a stream to limit how far it can be read.
        /// </summary>
        /// <param name="source">the stream to wrap</param>
        public ThrottledReadStream(Stream source)
        {
            _source = source;
        }
		
        #endregion Constructors

        #region Public Methods	
        public override void Flush()
        {
            _source.Flush();
        }

        //
        public override System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
        {
            //are we at the end?
            if (this.Length == this.Position)
                return 0;

            //block until the read limit is beyond our position
            while (_readLimit <= this.Position)
            {
                Thread.Sleep(10);
            }

            //limit the number of bytes read
            int gap = (int)(_readLimit - this.Position);
            if (count > gap) 
                count = gap;

            return _source.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _source.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _source.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _source.Write(buffer, offset, count);
        }
        #endregion Public Methods	

        #region Public Properties	
		
        public override bool CanRead 
        { 
            get
            {
                return _source.CanRead;
            } 
        }

        public override bool CanSeek
        {
            get
            {
                return _source.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _source.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _source.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _source.Position;
            }
            set
            {
                _source.Position = value;
            }
        }

        /// <summary>
        /// Limits how far into the stream you are allowed to read.
        /// if you atttempt to read past the limit, the read operation will block.
        /// </summary>
        public Int64 ReadLimit
        {
            get { return _readLimit; }
            set { _readLimit = value; }
        }
		
        #endregion Private Fields	


        #region Public Properties
		
        private Stream _source = null;
        private Int64 _readLimit = 0;

        #endregion Private Fields	
    }
}
