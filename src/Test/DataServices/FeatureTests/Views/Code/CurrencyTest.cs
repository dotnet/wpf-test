// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Changes currency and checks the CurrentChanged and CurrentChanging event handlers and the
    /// CurrentItem of CollectionView.
    /// </description>
    /// </summary>
    [Test(2, "Views", "CurrencyTest")]
    public class CurrencyTest : AvalonTest
    {
        object _beforeMoveBook,_afterMoveBook;
        CLRBook _item;
        ObservableCollection<CLRBook> _books;
        ICollectionView _cv;
        CurrentChangingVerifier _ccgv;
        CurrentChangedVerifier _ccdv;
        public CurrencyTest()
        {
            InitializeSteps +=new TestStep(Init);
            RunSteps +=new TestStep(MoveNextStep);
            RunSteps +=new TestStep(MovePreviousStep);
            RunSteps +=new TestStep(MoveLastStep);
            RunSteps +=new TestStep(CheckEOFStep);
            RunSteps +=new TestStep(MoveFirstStep);
            RunSteps +=new TestStep(CheckBOFStep);
            RunSteps +=new TestStep(MoveToStep);
			RunSteps += new TestStep(MoveToPosition);
			RunSteps += new TestStep(CancelCurrentChange);
		}
        private TestResult Init()
        {
            Status("Init");
            _books = new ObservableCollection<CLRBook>();
            _item = new CLRBook("Homo Faber", "Max Frisch", 1957, 14.99, BookType.Novel);
            _books.Add(_item);
            _item = new CLRBook("The Fourth Hand", "John Irving", 2001, 14.95, BookType.Novel);
            _books.Add(_item);
            _item = new CLRBook("Inside C#", "Tom Archer e.a.", 2002, 49.99, BookType.Reference);
            _books.Add(_item);
            _item = new CLRBook("A Man in Full", "Tom Wolfe", 1998, 8.95, BookType.Novel);
            _books.Add(_item);
            _cv = CollectionViewSource.GetDefaultView(_books);
            _ccgv = new CurrentChangingVerifier(_cv);
            _ccdv = new CurrentChangedVerifier(_cv);

            return TestResult.Pass;
        }
        private TestResult MoveNextStep()
        {
            Status("MoveNextStep");
            _beforeMoveBook = _books[0];
            _afterMoveBook = _books[1];
            _cv.MoveCurrentToNext();

            IVerifyResult ivr1 = _ccgv.Verify(_cv, 1, _beforeMoveBook);
            if (ivr1.Result != TestResult.Pass)
            {
                LogComment(ivr1.Message);
                return ivr1.Result;
            }
            IVerifyResult ivr2 = _ccdv.Verify(_cv, 1, _afterMoveBook);
            if (ivr2.Result != TestResult.Pass)
            {
                LogComment(ivr2.Message);
                return ivr2.Result;
            }

            return TestResult.Pass;
        }

        private TestResult MovePreviousStep()
        {
            Status("MovePreviousStep");
            _beforeMoveBook = _afterMoveBook;
            _afterMoveBook = _books[0];
            _cv.MoveCurrentToPrevious();

            IVerifyResult ivr1 = _ccgv.Verify(_cv, 1, _beforeMoveBook);
            if (ivr1.Result != TestResult.Pass)
            {
                LogComment(ivr1.Message);
                return ivr1.Result;
            }
            IVerifyResult ivr2 = _ccdv.Verify(_cv, 1, _afterMoveBook);
            if (ivr2.Result != TestResult.Pass)
            {
                LogComment(ivr2.Message);
                return ivr2.Result;
            }

            return TestResult.Pass;
        }
        private TestResult MoveLastStep()
        {
            Status("MoveLastStep");
            _beforeMoveBook = _afterMoveBook;
            _afterMoveBook = _books[3];
            _cv.MoveCurrentToLast();

            IVerifyResult ivr1 = _ccgv.Verify(_cv, 1, _beforeMoveBook);
            if (ivr1.Result != TestResult.Pass)
            {
                LogComment(ivr1.Message);
                return ivr1.Result;
            }
            IVerifyResult ivr2 = _ccdv.Verify(_cv, 1, _afterMoveBook);
            if (ivr2.Result != TestResult.Pass)
            {
                LogComment(ivr2.Message);
                return ivr2.Result;
            }

            return TestResult.Pass;
        }
        private TestResult CheckEOFStep()
        {
            Status("CheckEOFStep");
            _beforeMoveBook = _afterMoveBook;
            _afterMoveBook = null;
            _cv.MoveCurrentToNext();

            IVerifyResult ivr1 = _ccgv.Verify(_cv, 1, _beforeMoveBook);
            if (ivr1.Result != TestResult.Pass)
            {
                LogComment(ivr1.Message);
                return ivr1.Result;
            }
            IVerifyResult ivr2 = _ccdv.Verify(_cv, 1, _afterMoveBook);
            if (ivr2.Result != TestResult.Pass)
            {
                LogComment(ivr2.Message);
                return ivr2.Result;
            }

			if (!_cv.IsCurrentAfterLast)
			{
				LogComment("Fail - Position should be after last");
				return TestResult.Fail;
			}

			return TestResult.Pass;
        }
        private TestResult MoveFirstStep()
        {
            Status("MoveFirstStep");
            _beforeMoveBook = _afterMoveBook;
            _afterMoveBook = _books[0];
            _cv.MoveCurrentToFirst();

            IVerifyResult ivr1 = _ccgv.Verify(_cv, 1, _beforeMoveBook);
            if (ivr1.Result != TestResult.Pass)
            {
                LogComment(ivr1.Message);
                return ivr1.Result;
            }
            IVerifyResult ivr2 = _ccdv.Verify(_cv, 1, _afterMoveBook);
            if (ivr2.Result != TestResult.Pass)
            {
                LogComment(ivr2.Message);
                return ivr2.Result;
            }

            return TestResult.Pass;
        }
        private TestResult CheckBOFStep()
        {
            Status("CheckBOFStep");
            _beforeMoveBook = _afterMoveBook;
            _afterMoveBook = null;
            _cv.MoveCurrentToPrevious();

            IVerifyResult ivr1 = _ccgv.Verify(_cv, 1, _beforeMoveBook);
            if (ivr1.Result != TestResult.Pass)
            {
                LogComment(ivr1.Message);
                return ivr1.Result;
            }
            IVerifyResult ivr2 = _ccdv.Verify(_cv, 1, _afterMoveBook);
            if (ivr2.Result != TestResult.Pass)
            {
                LogComment(ivr2.Message);
                return ivr2.Result;
            }

			if (!_cv.IsCurrentBeforeFirst)
			{
				LogComment("Fail - Position should be before first");
				return TestResult.Fail;
			}

			return TestResult.Pass;
        }
		private TestResult MoveToStep()
        {
            Status("MoveToStep");
            _beforeMoveBook = _afterMoveBook;
            _afterMoveBook = _books[3];
            _cv.MoveCurrentTo(_books[3]);

            IVerifyResult ivr1 = _ccgv.Verify(_cv, 1, _beforeMoveBook);
            if (ivr1.Result != TestResult.Pass)
            {
                LogComment(ivr1.Message);
                return ivr1.Result;
            }
            IVerifyResult ivr2 = _ccdv.Verify(_cv, 1, _afterMoveBook);
            if (ivr2.Result != TestResult.Pass)
            {
                LogComment(ivr2.Message);
                return ivr2.Result;
            }

            return TestResult.Pass;
        }
		private TestResult MoveToPosition()
		{
			Status("MoveToPosition");
			_beforeMoveBook = _afterMoveBook;
			int positionBefore = 2;
			_afterMoveBook = _books[positionBefore];
			_cv.MoveCurrentToPosition(positionBefore);

			IVerifyResult ivr1 = _ccgv.Verify(_cv, 1, _beforeMoveBook);
			if (ivr1.Result != TestResult.Pass)
			{
				LogComment(ivr1.Message);
				return ivr1.Result;
			}
			IVerifyResult ivr2 = _ccdv.Verify(_cv, 1, _afterMoveBook);
			if (ivr2.Result != TestResult.Pass)
			{
				LogComment(ivr2.Message);
				return ivr2.Result;
			}

			int positionAfter = _cv.CurrentPosition;
			if (positionBefore != positionAfter)
			{
				LogComment("Fail - Position before:" + positionBefore + " Position afte:" + positionAfter);
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}
		private TestResult CancelCurrentChange()
        {
            Status("CancelCurrentChange");
            _beforeMoveBook = _afterMoveBook;
            _ccgv.CancelEvent = true;
            _cv.MoveCurrentTo(_books[0]);
            CancelEventArgs cet = new CancelEventArgs(false);

            IVerifyResult ivr1 = _ccgv.Verify(_cv, 1, _beforeMoveBook);
            if (ivr1.Result != TestResult.Pass)
            {
                LogComment(ivr1.Message);
                return ivr1.Result;
            }
            IVerifyResult ivr2 = _ccdv.Verify(_cv, 0, _beforeMoveBook);
			if (ivr2.Result != TestResult.Pass)
            {
                LogComment(ivr2.Message);
                return ivr2.Result;
            }
            return TestResult.Pass;
        }
    }
}




