// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

using System;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Xml;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using System.Windows.Documents;
using System.Collections;
using System.Collections.Generic;

using Annotation = System.Windows.Annotations.Annotation;
using ContentLocatorBase = System.Windows.Annotations.ContentLocatorBase;
using ContentLocatorPart = System.Windows.Annotations.ContentLocatorPart;
using ContentLocator = System.Windows.Annotations.ContentLocator;
using ContentLocatorGroup = System.Windows.Annotations.ContentLocatorGroup;
using AnnotationResource = System.Windows.Annotations.AnnotationResource;
using AnnotationResourceChangedEventArgs = System.Windows.Annotations.AnnotationResourceChangedEventArgs;
using AnnotationAuthorChangedEventArgs = System.Windows.Annotations.AnnotationAuthorChangedEventArgs;
using AnnotationResourceChangedEventHandler = System.Windows.Annotations.AnnotationResourceChangedEventHandler;
using AnnotationAuthorChangedEventHandler = System.Windows.Annotations.AnnotationAuthorChangedEventHandler;
using AnnotationAction = System.Windows.Annotations.AnnotationAction;

using Proxies.System.Windows.Annotations;
using Proxies.MS.Internal.Annotations.Anchoring;
using System.Windows.Annotations.Storage;

using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations
{
	/// <summary>
	/// Static helper methods.
	/// This class contains the general, dll-candidate methods that can be used
	/// across all Annotations test cases
	/// </summary>
	public abstract class AnchoringAPIHelpers
	{
		#region globals
		// Hardcoded strings to use in AnnotationResource, Annotation constructors
		private const string ANN_ANCHOR_NAME = "annotation anchor";
		private const string ANN_TYPE = "testAnnotation";
		private const string ANN_NAMESPACE = "http://schemas.microsoft.com/caf2";
		#endregion


		/// <summary>
		/// Checks if the start and end TextPointers for the twoTextRanges are equal
		/// </summary>
		/// <param name="origSelection">TextRange used to generate locator</param>
		/// <param name="resolvedAnchor">TextRange produced by resolving locator</param>
		public static bool AreTextPointersEqual(TextRange origSelection, TextRange resolvedAnchor)
		{
			if (origSelection == null || resolvedAnchor == null)
				return false;

			if (origSelection.Start.Equals(resolvedAnchor.Start) && origSelection.End.Equals(resolvedAnchor.End))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Compares all the atomic properties (Strings, DateTimes) of two annotations
		/// and based on the equality of these, returns true or false
		/// </summary>
		/// <param name="ann1">First annotation to compare</param>
		/// <param name="ann2">Second annotation to compare</param>
		/// <returns>returns true if properties are equal, otherwise, returns false</returns>
		public static bool AreAnnotationsEqual(Annotation ann1, Annotation ann2)
		{
			if (ann1.CreationTime.Equals(ann2.CreationTime) &&
				ann1.Id.Equals(ann2.Id) &&
				ann1.AnnotationType.Equals(ann2.AnnotationType) &&
				ann1.LastModificationTime.Equals(ann2.LastModificationTime))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Checks the ContentLocatorPart count and compares each ContentLocatorPart within
		/// two LocatorPartLists.  Based on the equality of these, returns true or false
		/// </summary>
		/// <param name="loc1">first ContentLocator to compare</param>
		/// <param name="loc2">second ContentLocator to compare</param>
		/// <returns>true if ContentLocatorPart counts and each ContentLocatorPart (sequence within ContentLocatorBase and value)
		/// are equal, otherwise returns false</returns>
		public static bool AreLocatorPartListsEqual(ContentLocator loc1, ContentLocator loc2)
		{
			if (loc1.Parts.Count != loc2.Parts.Count)
				return false;

			for (int i = 0; i < loc1.Parts.Count; i++)
			{
				if (!loc1.Parts[i].Equals(loc2.Parts[i]))
					return false;
			}

			return true;
		}

		/// <summary>
		/// Creates an annotation that is anchored off of o (can be a TextRange or FrameworkElement)
		/// and adds it to the store.  Returns the annotation created and stored, so it should have
		/// all the Name, TypeName, CreationTime, LastModificationTime, etc. information
		/// </summary>
		/// <param name="o">TextRange/FrameworkElement to use as an anchor</param>
		/// <returns>Annotation created and stored</returns>
		public static Annotation CreateBasicAnnotation(Object o, LocatorManager manager, AnnotationStore store)
		{
			Annotation annotation = null;

			AnnotationResource anchor = new AnnotationResource(ANN_ANCHOR_NAME);
			IList<ContentLocatorBase> locators = manager.GenerateLocators(o);

			foreach (ContentLocatorBase locator in locators)
				anchor.ContentLocators.Add(locator);

			annotation = new Annotation(new XmlQualifiedName(ANN_TYPE, ANN_NAMESPACE));
			annotation.Anchors.Add(anchor);
			store.AddAnnotation(annotation);

			return annotation;
		}

		/// <summary>
		/// Creates a new TextRange that is a substring of TextRange trSource.
		/// </summary>
		/// <param name="trSource">TextRange to generate substring off of</param>
		/// <param name="startPosition"># chars from start of trSource to mark start of substring</param>
		/// <param name="endPosition"># chars from start of trSource to mark end of substring</param>
		/// <returns>TextRange substring of the TextRange passed in</returns>
		public static TextRange SubstringTextRange(TextBlock text, int startPosition, int endPosition)
		{
			if (text == null)
				throw new Exception("SubstringTextRange:  Cannot substring off a null TextRange");

			if (startPosition > endPosition)
				throw new Exception("SubstringTextRange:  startPosition must be smaller than endPosition");

			// Create a TextRange that spans some text within a TextBlock element
			TextPointer startTN = text.ContentStart;
			TextPointer endTN = text.ContentStart;

            startTN = startTN.GetPositionAtOffset(startPosition);
            endTN = endTN.GetPositionAtOffset(endPosition);

			return new TextRange(startTN, endTN);
		}


		/// <summary>
		/// Creates a new TextRange that is a substring of TextRange trSource.
		/// </summary>
		/// <param name="trSource">TextRange to generate substring off of</param>
		/// <param name="startPosition"># chars from start of trSource to mark start of substring</param>
		/// <param name="endPosition"># chars from start of trSource to mark end of substring</param>
		/// <returns>TextRange substring of the TextRange passed in</returns>
		public static TextRange SubstringTextBox(TextBox text, int startPosition, int endPosition)
		{
			if (text == null)
				throw new Exception("SubstringTextBox:  Cannot substring off a null TextBox");

			if (startPosition > endPosition)
				throw new Exception("SubstringTextBox:  startPosition must be smaller than endPosition");

			// Create a TextRange that spans some text within a TextBlock element
			TextPointer startTN = InternalGetTextBoxPosition(text, "Start");
			TextPointer endTN = InternalGetTextBoxPosition(text, "Start");

            startTN = startTN.GetPositionAtOffset(startPosition);
            endTN = endTN.GetPositionAtOffset(endPosition);

			return new TextRange(startTN, endTN);
		}


		/// <summary>
		/// Creates a new TextRange substring from possibly two separate (but adjacent) text elements
		/// such as TextRanges, Text, Paragraphs, FlowDocumentScrollViewer etc.
		/// </summary>
		/// <param name="startPos">TextPointer marking the starting position of selection</param>
		/// <param name="endPos">TextPointer marking the ending position of the selection</param>
		/// <param name="moveStart"># chars to adjust startPos by</param>
		/// <param name="moveEnd"># chars to adjust endPos by</param>
		/// <returns></returns>
		public static TextRange SubstringTextRange(TextPointer startPos, TextPointer endPos, int moveStart, int moveEnd)
		{
			if (startPos == null || endPos == null)
				throw new Exception("SubstringTextRange:  Cannot substring if either TextPointer is null\n");

			TextPointer adjustedStart = startPos;
            adjustedStart = adjustedStart.GetPositionAtOffset(moveStart);
			TextPointer adjustedEnd = endPos;
            adjustedEnd = adjustedEnd.GetPositionAtOffset(moveEnd);

			return new TextRange(adjustedStart, adjustedEnd);
		}

		/// <summary>
		/// Invokes an internal method (usually only accessible to other classes in the same assembly).
		/// BindingFlags are set to look for methods that are one of private, internal, and static
		/// </summary>
		/// <param name="methodName">Case-sensitive string with methodName as it appears in method header</param>
		/// <param name="caller">Type/value that is used to </param>
		/// <param name="methodParams">Array containing parameters as they appear in method header</param>
		/// <returns>Return value of the internal method invoked</returns>
		public static object CallInternalMethod(String methodName, object caller, object[] methodParams)
		{
			if (methodName.Equals(String.Empty))
				throw new Exception("CallInternalMethod:  methodName cannot be an empty string");

			if (caller == null)
				throw new Exception("CallInternalMethod:  Cannot invoke methods off a null caller");

			// Prepare Reflection permissions to access API at run-time
			ReflectionPermission permission = new ReflectionPermission(PermissionState.Unrestricted);
			permission.Demand();

			bool isStatic = caller is Type;
			Type callerType = isStatic ? (Type)caller : caller.GetType();

			if (callerType.IsSubclassOf(typeof(AReflectiveProxy)))
			{
				if (isStatic)
					return AReflectiveProxy.InvokeStaticDelegateMethod(methodName, callerType, methodParams);
				else
					return ((AReflectiveProxy)caller).InvokeDelegateMethod(methodName, methodParams);
			}
			else
			{
				MethodInfo runTimeMethod = callerType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
				return runTimeMethod.Invoke((caller is Type ? null : caller), methodParams);
			}
		}

		/// <summary>
		/// Debug routine to export a ContentLocatorPart to an XML file.
		/// For verification purposes only.
		/// </summary>
		/// <param name="locatorParts">IEnumerator listing all ContentLocatorParts of a ContentLocator</param>
		public static void ExportLocatorParts(IEnumerator<ContentLocatorPart> locatorParts)
		{
			StreamWriter writer = new StreamWriter("c:\\locatorParts.txt", true);

			// Iterate through all ContentLocatorParts until MoveNext returns false (end of IEnumerator is reached)
			while (locatorParts.MoveNext())
			{
				writer.WriteLine("\nLocatorPart\n" + locatorParts.Current.PartType.Name);

				// Get the list of keys then print out key, value pairs in locatorParts
				ICollection<string> locatorPartKeys = locatorParts.Current.NameValuePairs.Keys;

				foreach (string key in locatorPartKeys)
					writer.WriteLine("\t" + key + "\t" + locatorParts.Current.NameValuePairs[key]);
			}

			writer.Flush();
			writer.Close();
		}

        private static TextPointer InternalGetTextBoxPosition(TextBox textBox, string containerPropertyName)
        {
            object textContainer;

            if (textBox == null)
            {
                throw new ArgumentNullException("textBox");
            }

            PropertyInfo TextContainerInfo = typeof(TextBoxBase).GetProperty("TextContainer", BindingFlags.NonPublic | BindingFlags.Instance);
            if (TextContainerInfo == null)
            {
                throw new Exception("TextBoxBase.TextContainer property is not accessible");
            }
            textContainer = TextContainerInfo.GetValue(textBox, null);
            PropertyInfo property = textContainer.GetType().GetProperty(containerPropertyName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (property == null)
            {
                throw new Exception("TextContainer.Start property is not accessible");
            }
            return (TextPointer)property.GetValue(textContainer, null);
        }

    }		// end of AnchoringAPIHelpers class

}			// end of namespace

