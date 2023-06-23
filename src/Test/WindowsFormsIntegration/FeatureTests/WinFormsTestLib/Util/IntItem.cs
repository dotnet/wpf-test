using System;

namespace WFCTestLib.Util
{
    // <doc>
    // <desc>
    //  Small data object that holds an integer value and a string value.
    //  Used for stuffing a combo box so that you can retrieve the current
    //  item in the combo box and get it's associated int value.
    // </desc>
    // </doc>
    public class IntItem
    {
        // <doc>
        // <desc>
        //  The integer value to attach to the string
        // </desc>
        // </doc>
        private int    nValue;

        // <doc>
        // <desc>
        //  The string we are attaching to the integer value.
        // </desc>
        // <seealso member="ToString"/>
        // </doc>
        private String s;

        // <doc>
        // <desc>
        //  Constructs a new IntItem object associating an integer
        //  with a given String.
        // </desc>
        // <param term="nValue">
        //  The integer associated with the given string
        // </param>
        // <param term="s">
        //  The string
        // </param>
        // </doc>
        public IntItem(int nValue, String s)
        {
            this.nValue = nValue;
            this.s      = s;
        }

        // <doc>
        // <desc>
        //  Constructs a new IntItem object associating an integer
        //  with a given String.
        // </desc>
        // <param term="s">
        //  The string
        // </param>
        // <param term="nValue">
        //  The integer associated with the given string
        // </param>
        // </doc>
        public IntItem(String s, int nValue) : this (nValue, s)
        {
        }

        // <doc>
        // <desc>
        //  Returns the string passed in in the constructor
        // </desc>
        // <retvalue>
        //  The string passed in in the object's constructor
        // </retvalue>
        // </doc>
        public override String ToString()
        {
            return s;
        }

        // <doc>
        // <desc>
        //  The integer value passed in in the constructor.
        // </desc>
        // </doc>
        public virtual int Value
        {
            get
            {
                return nValue;
            }
        }
    }
}
