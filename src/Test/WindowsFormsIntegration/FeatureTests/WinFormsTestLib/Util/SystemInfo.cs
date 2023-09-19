// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Security;
using System.Security.Permissions;

namespace WFCTestLib.Util
{
	/// <summary>
	/// Wraps Win32 struct SYSTEM_INFO
	/// </summary>
	public class SystemInfo
	{
		/// <summary>
		/// Enum representing the list of valid processors.
		/// Used in ProcessorArchitecture.
		/// </summary>
		public enum Architecture
		{
			Unknown,
			Intel,
			Mips,
			Alpha,
			Ppc,
			Shx,
			Arm,
			IA64,
			Alpha64,
			MsIl,
			Amd64,
			Ia32OnWin64,
		}

		/// <summary>
		/// obsolete. Enum representing the list of valid processors.
		/// Used in ProcessorType
		/// </summary>
		public enum CpuType
		{
			cpu386,
			cpu486,
			cpuPentium,
		}

		/// <summary>
		/// System's processor architecture.
		/// </summary>
		public Architecture ProcessorArchitecture { get { return _processorArchitecture; } }

		/// <summary>
		/// Reserved for future use. 
		/// </summary>
		public int Reserved { get { return _reserved; } }

		/// <summary>
		/// Page size and the granularity of page protection and commitment. 
		/// This is the page size used by the VirtualAlloc function. 
		/// </summary>
		public long PageSize { get { return _pageSize; } }

		/// <summary>
		/// Pointer to the lowest memory address accessible to applications and dynamic-link libraries (DLLs). 
		/// </summary>
		public long MinimumApplicationAddress { get { return _minimumApplicationAddress; } }

		/// <summary>
		/// Pointer to the highest memory address accessible to applications and DLLs. 
		/// </summary>
		public long MaximumApplicationAddress { get { return _maximumApplicationAddress; } }

		/// <summary>
		/// Mask representing the set of processors configured into the system. 
		/// Bit 0 is processor 0; bit 31 is processor 31. 
		/// </summary>
		public long ActiveProcessorMask { get { return _activeProcessorMask; } }
		
		/// <summary>
		/// Number of processors in the system. 
		/// </summary>
		public long NumberOfProcessors { get { return _numberOfProcessors; } }

		/// <summary>
		/// An obsolete member that is retained for compatibility with Windows NT 3.5 and earlier. 
		/// Use the ProcessorArchitecture, ProcessorLevel, and ProcessorRevision members to determine 
		/// the type of processor.
		/// </summary>
		public CpuType ProcessorType { get { return _processorType; } }

		/// <summary>
		/// Granularity with which virtual memory is allocated. For example, a VirtualAlloc request to 
		/// allocate 1 byte will reserve an address space of dwAllocationGranularity bytes. This value 
		/// was hard coded as 64K in the past, but other hardware architectures may require different values.
		/// </summary>
		public long AllocationGranularity { get { return _allocationGranularity; } }

		/// <summary>
		/// System's architecture-dependent processor level. It should be used only for display purposes. 
		/// To determine the feature set of a processor, use the IsProcessorFeaturePresent function.
		/// </summary>
		public int ProcessorLevel { get { return _processorLevel; } }

		/// <summary>
		/// Architecture-dependent processor revision.
		/// </summary>
		public int ProcessorRevision { get { return _processorRevision; } }

		public SystemInfo()
		{
			SYSTEM_INFO info = new SYSTEM_INFO();

			LibSecurity.UnmanagedCode.Assert();
			try { GetSystemInfo(info); }
			finally { SecurityPermission.RevertAssert(); }
			Parse(info);
		}

		/// <summary>
		/// Quick access to the system's processor architecture
		/// </summary>
		public static Architecture GetArchitecture()
		{
			SystemInfo info = new SystemInfo();
			return info.ProcessorArchitecture;
		}

		public override string ToString()
		{
			return String.Format("{0} {1} ({2} {3}:{4})", NumberOfProcessors.ToString(), ProcessorArchitecture.ToString(), ProcessorType, ProcessorLevel, ProcessorRevision);		
		}

		private void Parse(SYSTEM_INFO info)
		{
			switch(info.wProcessorArchitecture)
			{
				case PROCESSOR_ARCHITECTURE_INTEL:			_processorArchitecture = Architecture.Intel; break;
				case PROCESSOR_ARCHITECTURE_MIPS:			_processorArchitecture = Architecture.Mips; break;
				case PROCESSOR_ARCHITECTURE_ALPHA:			_processorArchitecture = Architecture.Alpha; break;
				case PROCESSOR_ARCHITECTURE_PPC:			_processorArchitecture = Architecture.Ppc; break;
				case PROCESSOR_ARCHITECTURE_SHX:			_processorArchitecture = Architecture.Shx; break;
				case PROCESSOR_ARCHITECTURE_ARM:			_processorArchitecture = Architecture.Arm; break;
				case PROCESSOR_ARCHITECTURE_IA64:			_processorArchitecture = Architecture.IA64; break;
				case PROCESSOR_ARCHITECTURE_ALPHA64:		_processorArchitecture = Architecture.Alpha64; break;
				case PROCESSOR_ARCHITECTURE_MSIL:			_processorArchitecture = Architecture.MsIl; break;
				case PROCESSOR_ARCHITECTURE_AMD64:			_processorArchitecture = Architecture.Amd64; break;
				case PROCESSOR_ARCHITECTURE_IA32_ON_WIN64:	_processorArchitecture = Architecture.Ia32OnWin64; break;
				case PROCESSOR_ARCHITECTURE_UNKNOWN:		_processorArchitecture = Architecture.Unknown; break;
				default:									_processorArchitecture = Architecture.Unknown; break;
			}

			switch(info.dwProcessorType)
			{
				case PROCESSOR_INTEL_386:		_processorType = CpuType.cpu386; break;
				case PROCESSOR_INTEL_486:		_processorType = CpuType.cpu486; break;
				case PROCESSOR_INTEL_PENTIUM:	_processorType = CpuType.cpuPentium; break;
				default:						_processorType = CpuType.cpuPentium; break;
			}

			_reserved = info.wReserved;
			_pageSize = info.dwPageSize;
			_minimumApplicationAddress = (long)info.lpMinimumApplicationAddress;
			_maximumApplicationAddress = (long)info.lpMaximumApplicationAddress;
			_activeProcessorMask = (long)info.dwActiveProcessorMask;
			_numberOfProcessors = info.dwNumberOfProcessors;
			_allocationGranularity = info.dwAllocationGranularity;
			_processorLevel = info.wProcessorLevel;
			_processorRevision = info.wProcessorRevision;
		}

		#region members

		private Architecture _processorArchitecture = Architecture.Unknown;
		private int _reserved = 0;
		private long _pageSize = 0;
		private long _minimumApplicationAddress = 0;
		private long _maximumApplicationAddress = 0;
		private long _activeProcessorMask = 0;
		private long _numberOfProcessors = 0;
		private CpuType _processorType = CpuType.cpuPentium;
		private long _allocationGranularity  = 0;
		private int _processorLevel = 0;
		private int _processorRevision = 0;

		#endregion

		#region win32 interface

		[DllImport("kernel32")]
		internal extern static void GetSystemInfo(SYSTEM_INFO info);

		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		public class SYSTEM_INFO
		{
			public UInt16 wProcessorArchitecture;
			public UInt16 wReserved;
			public UInt32 dwPageSize;
			public IntPtr lpMinimumApplicationAddress;
			public IntPtr lpMaximumApplicationAddress;
			public IntPtr dwActiveProcessorMask;
			public UInt32 dwNumberOfProcessors;
			public UInt32 dwProcessorType;
			public UInt32 dwAllocationGranularity;
			public UInt16 wProcessorLevel;
			public UInt16 wProcessorRevision;
		}

		internal const UInt16 PROCESSOR_ARCHITECTURE_INTEL = 0;
		internal const UInt16 PROCESSOR_ARCHITECTURE_MIPS = 1;
		internal const UInt16 PROCESSOR_ARCHITECTURE_ALPHA = 2;
		internal const UInt16 PROCESSOR_ARCHITECTURE_PPC = 3;
		internal const UInt16 PROCESSOR_ARCHITECTURE_SHX = 4;
		internal const UInt16 PROCESSOR_ARCHITECTURE_ARM = 5;
		internal const UInt16 PROCESSOR_ARCHITECTURE_IA64 = 6;
		internal const UInt16 PROCESSOR_ARCHITECTURE_ALPHA64 = 7;
		internal const UInt16 PROCESSOR_ARCHITECTURE_MSIL = 8;
		internal const UInt16 PROCESSOR_ARCHITECTURE_AMD64 = 9;
		internal const UInt16 PROCESSOR_ARCHITECTURE_IA32_ON_WIN64 = 10;
		internal const UInt16 PROCESSOR_ARCHITECTURE_UNKNOWN = 0xFFFF;

		internal const UInt16 PROCESSOR_INTEL_386 = 386;
		internal const UInt16 PROCESSOR_INTEL_486 = 486;
		internal const UInt16 PROCESSOR_INTEL_PENTIUM = 586;

		#endregion
	}
}
