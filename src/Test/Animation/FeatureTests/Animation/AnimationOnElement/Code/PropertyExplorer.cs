// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
//  Desc: Integration Utilities to explore PMEs form objects
//
// $Id:$ $Change:$
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;

using MS.Internal;
using System.Windows.Test.CommonSource;


namespace Microsoft.Test.Animation
{

    /// <summary>
    /// Explores Dynamic properties and their associated types
    /// </summary>
    public class PropertyExplorer
    {
        /// <summary>
        /// Dynamic property list
        /// </summary>
        protected DependencyProperty[]   properties;
        /// <summary>
        /// Internal object reference to the thing being analyzed
        /// </summary>
        protected System.Object       theObject;
        /// <summary>
        /// Internal type reference
        /// </summary>
        protected System.Type         theType;
        /// <summary>
        /// Internal random reference
        /// </summary>
        protected Random              rnd = new Random();
        /// <summary>
        /// Final list of integration properties
        /// </summary>
        protected ArrayList           integrationProperties;
        /// <summary>
        /// Accesor to the internal integrationporperties
        /// </summary>
        public ArrayList              IntegrationProperties
        {
            get { return this.integrationProperties; }
        }



        #region External Logging

        /// <summary>
        /// Static logging function interface
        /// </summary>
        public static ILog Log
        {
            set { s_log = value; }
        }
        static ILog s_log = new LogToConsole();

        #endregion External Logging



        #region AppDomain handlers

        protected static Hashtable loadedAssemblies;
        protected static bool hasAppDomainHandlers = false;
        static PropertyExplorer()
        {
            loadedAssemblies = new Hashtable();
            //### 
            // AddAppDomainEventHandlers();

        }
        #endregion AppDomain handlers



        /// <summary>
        /// Constructor, takes an object to ananlyse
        /// </summary>
        /// <param name="obj">Object to analyze</param>
        public PropertyExplorer( System.Object obj )
        {
            // Get the object
            theObject = obj ;
            // Get the object's type
            theType = theObject.GetType() ;
            // cache casting
            DependencyObject theDO = (DependencyObject)theObject;
            // reset list
            integrationProperties = new ArrayList();

            // Use reflection to find all instances of DependencyProperties in loaded assemblies
            Hashtable registeredProperties = GetRegisteredAttachedProperties();
            foreach( string propertyFullName in registeredProperties.Keys )
            {
                // get declaring type
                Type ownerType = (Type)registeredProperties[ propertyFullName ] ;

                // ignore abstract types since they cause exceptions
                if ( ownerType.IsAbstract )
                {
                    Type concretType = GetConcreteImplementingType( ownerType );
                    if ( concretType != null )
                    {
                        // promote owner to concrete version
                        ownerType = concretType;
                    }
                    else
                    {
                        s_log.Log( "### Ignoring property in abstract class: " + propertyFullName );
                        continue;// IGNORE PROPERTY
                    }
                }

                // get data
                string ownerName = propertyFullName.Split(new char[] {'.'} )[0];
                string propertyName = propertyFullName.Split(new char[] {'.'} )[1];
          
                // get property refernce
                DependencyProperty dp = null;
                try
                {
                    FieldInfo fi = ownerType.GetField( propertyName + "Property", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static );
                    if ( fi == null )
                    {
                        // get public properties
                        foreach ( FieldInfo uncommonFieldInfo in ownerType.GetFields( BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static ) )
                        {
                            if ( uncommonFieldInfo.FieldType == typeof(DependencyProperty) )
                            {
                                if( uncommonFieldInfo.Name.IndexOf(propertyName) != -1 )
                                {
                                    fi = uncommonFieldInfo;
                                    s_log.Log( String.Format(
                                        "####### WARINING DependencyProperty {2}.{1} does not follow naming convention for property {0}.",
                                        propertyName, uncommonFieldInfo.Name, ownerType.FullName ));
                                    break;
                                }
                            }
                        }
                    }

                    if ( fi != null )
                    {
                            dp = (DependencyProperty)fi.GetValue( null );
                    }
                    else
                    {
                        s_log.Log( String.Format(
                            "### WARNING DependencyProperty on type {1} could not get access for property {0}.",
                            propertyName, ownerType.FullName ));
                        continue;
                    }
                }
                catch( System.Exception ex1 )
                {
                    s_log.Log(
                        String.Format("### Could not get DP {0} in Type {2}\n### Exception:\n{1} ", propertyName, ex1.ToString(), ownerType.FullName ));
                    continue; // IGNORE PROPERTY
                };
                // Create New property
                IntegrationProperty ip = new IntegrationProperty();

                // Set Values
                ip.OriginalProperty = dp;
                ip.Name = dp.Name;
                ip.Type = dp.PropertyType.Name;
                ip.OwnerType = ownerType;
                ip.Owner = ip.OwnerType.Name;

                try
                {
                    object theValue = theDO.GetValue(dp);
                    ip.CurrentValue = NormalizeValue( theValue );
                }
                catch (Exception e)
                {
                    s_log.Log(
                        String.Format("FAIL: DepedencyObject.GetValue(\"{0}.{1}\") threw an exception {2}: \"{3}\". See inner exception for more details.", ownerName, propertyName, e, e.Message)
                        );
                    // ignore broken DP's
                    continue;
                }

                // Get Metadata for this one
                PropertyMetadata metadata = dp.GetMetadata(theDO);
                if ( metadata != null )
                {
                    ip.ReadOnly = dp.ReadOnly;
                    ip.DefaultValue = NormalizeValue( metadata.DefaultValue );
                    // read-enabled properties are databindable by default
                    if ( !ip.ReadOnly ) ip.DataBindable = true;
                }
                if ( metadata is FrameworkPropertyMetadata)
                {
                    ip.DataBindable = !((FrameworkPropertyMetadata)metadata).IsNotDataBindable;
                    ip.AffectsMeasure = ((FrameworkPropertyMetadata)metadata).AffectsMeasure;
                    ip.AffectsArrange = ((FrameworkPropertyMetadata)metadata).AffectsArrange;
                    ip.AffectsParentMeasure = ((FrameworkPropertyMetadata)metadata).AffectsParentMeasure;
                    ip.AffectsParentArrange = ((FrameworkPropertyMetadata)metadata).AffectsParentArrange;
                    ip.AffectsRender = ((FrameworkPropertyMetadata)metadata).AffectsRender;
                    ip.Inherits = ((FrameworkPropertyMetadata)metadata).Inherits;
                    ip.SpanSeparatedTrees = ((FrameworkPropertyMetadata)metadata).OverridesInheritanceBehavior;
                }

                //8-15-06: HACK-HACK:  FontSize animation fails for several DO's, which causes
                //test cases to fail 
                if (ip.Name != "FontSize")
                {
                    // Add to local list
                    integrationProperties.Add(ip);
                }
            }

            // Sort By Owner.Name by default.
            IntegrationProperties.Sort(
                new IntegrationProperty.CompareMultiple(
                    new IComparer[] {
                        new IntegrationProperty.CompareOwner() ,
                        new IntegrationProperty.CompareName()
                        } )
                );

        }


        /// <summary>
        /// Gets a list of fully quallified properties in the form of "Owner.Property"
        /// and their corresponding declaring types.
        /// </summary>
        /// <returns>Hashtable of the form key:string -> value:System.Type </returns>
        public static Hashtable GetRegisteredAttachedProperties()
        {
            // once only brute force search for DependencyProperties ...
            Hashtable registeredProperties = new Hashtable();

            // get loaded assemblies
            foreach ( Assembly asm in AppDomain.CurrentDomain.GetAssemblies() )
            {


                if ( loadedAssemblies.Contains(asm.FullName) ) continue;
                
                // GetExportedTypes is not implemented in .Net Core dynamic assemblies,
                // so we're skipping it.
                if (asm.IsDynamic) continue;

                // get exported types
                foreach ( Type tp in asm.GetExportedTypes() )
                {
                    // get public properties
                    foreach ( FieldInfo fi in tp.GetFields() )
                    {
                        if ( fi.FieldType == typeof(DependencyProperty) )
                        {
                            // new one
                            int idx = fi.Name.LastIndexOf("Property");
                            string propertyRemoved = fi.Name.Remove(idx); //Remove (the last) 'Property' from the Name.
                            string propertyName = String.Format( "{0}.{1}", tp.Name, propertyRemoved);
                            registeredProperties.Add( propertyName, tp );
                        }
                    }
                }
            }
            return registeredProperties;
        }


        /// <summary>
        /// Gets a list of fully quallified properties in the form of "Owner.Property"
        /// and their corresponding declaring types.
        /// </summary>
        /// <returns>Hashtable of the form key:string -> value:System.Type </returns>
        public static ArrayList GetSubClassesOf( Type baseType  )
        {
            // once only brute force search for DependencyProperties ...
            ArrayList subClasses = new ArrayList();

            // get loaded assemblies
            foreach ( Assembly asm in AppDomain.CurrentDomain.GetAssemblies() )
            {
                // get exported types
                foreach ( Type tp in asm.GetExportedTypes() )
                {
                    if ( tp.IsAbstract ) continue;
                    if ( tp.IsSubclassOf( baseType ) )
                    {
                        subClasses.Add( tp );
                    }
                }
            }
            return subClasses;
        }


        /// <summary>
        /// Gets the string value from the given object, defaults to ToString() except in known cases
        /// </summary>
        /// <param name="curValue">current value of many types</param>
        /// <returns>normalized string value</returns>
        protected string NormalizeValue( object curValue )
        {
            string currentValue = null ;
            if( curValue != null )
            {
                try
                {
                    // Use a Data Translator for this
                    DataTranslator translator = DataFactory.GetTranslator( "PropertyExplorer" );
                    currentValue = translator.GetMarkupRepresentation(curValue.GetType().FullName, curValue);
                }
                catch ( System.Exception ex )
                {
                    // ignore exceptions for bad translation
                    s_log.Log( String.Format("### Bad default value for {0} : {1} ", curValue.GetType().FullName, ex.ToString() ));
                }
            }
            return currentValue;
        }

        /// <summary>
        /// Gets the first concrete types that implements an abstract type
        /// </summary>
        /// <param name="abstractType">Abstract type to search for</param>
        /// <returns>First type that implements the abstract type or null.</returns>
        public static Type GetConcreteImplementingType( Type abstractType )
        {
            // get loaded assemblies
            foreach ( Assembly asm in AppDomain.CurrentDomain.GetAssemblies() )
            {


                if ( loadedAssemblies.Contains(asm.FullName) ) continue;
                
                // GetExportedTypes is not implemented in .Net Core dynamic assemblies,
                // so we're skipping it.
                if (asm.IsDynamic) continue;

                // get exported types
                foreach ( Type tp in asm.GetExportedTypes() )
                {
                    if ( tp.IsAbstract ) continue;
                    if ( abstractType.IsAssignableFrom( tp ) ) return tp;
                }
            }
            return null;
        }

    }
}
