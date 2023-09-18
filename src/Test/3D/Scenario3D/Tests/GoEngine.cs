// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Microsoft.Test.Graphics
{
    public class GoEngine
    {
        public GoEngine( int xSize, int ySize )
        {
            this._xSize = xSize;
            this._ySize = ySize;
            _board = new BoardContent[ xSize, ySize ];
            _moves = new ArrayList();
            Clear();
        }

        public GoEngine( int size )
            : this( size, size )
        {
        }

        public GoEngine()
            : this( 19 )
        {
        }

        public void Parse( string moves )
        {
            Clear();
            moves = moves.Replace( 'W', ' ' );
            moves = moves.Replace( 'B', ' ' );
            moves = moves.Replace( '[', ' ' );
            moves = moves.Replace( ']', ',' );
            foreach ( string move in moves.Split( new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries ) )
            {
                Move m = new Move( move );
                if ( m.IsPass )
                {
                    Pass();
                }
                else
                {
                    Play( m.X, m.Y );
                }
            }
        }

        public enum BoardContent
        {
            Empty,
            BlackPiece,
            WhitePiece
        }

        public struct Move
        {
            public Move( int x, int y )
            {
                this._x = (char)( 'a' + (char)x );
                this._y = (char)( 'a' + (char)y );
            }

            public Move( string moveCode )
            {
                this._x = moveCode.ToLower()[ 0 ];
                this._y = moveCode.ToLower()[ 1 ];
            }

            private Move( char x, char y )
            {
                this._x = x;
                this._y = y;
            }

            char _x;
            char _y;

            public bool IsPass { get { return ( _x == 'z' && _y == 'z' ); } }
            public int X { get { return (int)( _x - 'a' ); } }
            public int Y { get { return (int)( _y - 'a' ); } }

            public static Move Pass = new Move( 'z', 'z' );
        }

        public bool Play( int x, int y )
        {
            // check bounds
            if ( ( x >= 0 && x < _xSize ) && ( y >= 0 && y < _ySize ) )
            {
                // check if the space is available
                if ( _board[ x, y ] == BoardContent.Empty )
                {
                    // check if move is valid
                    if ( IsMoveValid( x, y ) )
                    {
                        // save move
                        Move m = new Move( x, y );
                        UpdateBoard( m );
                        return true;
                    }
                }
            }

            // invalid move
            return false;
        }

        public void Pass()
        {
            _moves.Add( Move.Pass );
        }

        public void Clear()
        {
            _moves.Clear();
            for ( int x=0; x<_xSize; x++ )
            {
                for ( int y=0; y<_ySize; y++ )
                {
                    _board[ x, y ] = BoardContent.Empty;
                }
            }
        }

        bool IsMoveValid( int x, int y )
        {
            // check atari

            // check ko

            // todo: check for atari, ko, etc ...
            return true;
        }

        void UpdateBoard( Move m )
        {
            bool blackPlay = ( _moves.Count % 2 == 0 );

            // place stone
            if ( blackPlay )
            {
                _board[ m.X, m.Y ] = BoardContent.BlackPiece;
            }
            else
            {
                _board[ m.X, m.Y ] = BoardContent.WhitePiece;
            }

            // remove stones, if necessary

            // add this to the play store
            _moves.Add( m );
        }

        public BoardContent[ , ] Board { get { return _board; } }
        public int XSize { get { return _xSize; } }
        public int YSize { get { return _ySize; } }

        BoardContent[ , ] _board;
        ArrayList _moves;
        int _xSize;
        int _ySize;

    }
}
