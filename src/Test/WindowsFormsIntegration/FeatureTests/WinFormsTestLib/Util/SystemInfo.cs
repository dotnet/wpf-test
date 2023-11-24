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
		public Architecture ProcessorArchitecture { get { return m_processorArchitecture; } }

		/// <summary>
		/// Reserved for future use. 
		/// </summary>
		public int Reserved { get { return m_reserved; } }

		/// <summary>
		/// Page size and the granularity of page protection and commitment. 
		/// This is the page size used by the VirtualAlloc function. 
		/// </summary>
		public long PageSize { get { return m_pageSize; } }

		/// <summary>
		/// Pointer to the lowest memory address accessible to applications and dynamic-link libraries (DLLs). 
		/// </summary>
		public long MinimumApplicationAddress { get { return m_minimumApplicationAddress; } }

		/// <summary>
		/// Pointer to the highest memory address accessible to applications and DLLs. 
		/// </summary>
		public long MaximumApplicationAddress { get { return m_maximumApplicationAddress; } }

		/// <summary>
		/// Mask representing the set of processors configured into the system. 
		/// Bit 0 is processor 0; bit 31 is processor 31. 
		/// </summary>
		public long ActiveProcessorMask { get { return m_activeProcessorMask; } }
		
		/// <summary>
		/// Number of processors in the system. 
		/// </summary>
		public long NumberOfProcessors { get { return m_numberOfProcessors; } }

		/// <summary>
		/// An obsolete member that is retained for compatibility with Windows NT 3.5 and earlier. 
		/// Use the ProcessorArchitecture, ProcessorLevel, and ProcessorRevision members to determine 
		/// the type of processor.
		/// </summary>
		public CpuType ProcessorType { get { return m_processorType; } }

		/// <summary>
		/// Granularity with which virtual memory is allocated. For example, a VirtualAlloc request to 
		/// allocate 1 byte will reserve an address space of dwAllocationGranularity bytes. This value 
		/// was hard coded as 64K in the past, but other hardware architectures may require different values.
		/// </summary>
		public long AllocationGranularity { get { return m_allocationGranularity; } }

		/// <summary>
		/// System's architecture-dependent processor level. It should be used only for display purposes. 
		/// To determine the feature set of a processor, use the IsProcessorFeaturePresent function.
		/// </summary>
		public int ProcessorLevel { get { return m_processorLevel; } }

		/// <summary>
		/// Architecture-dependent processor revision.
		/// </summary>
		public int ProcessorRevision { get { return m_processorRevision; } }

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
				case PROCESSOR_ARCHITECTURE_INTEL:			m_processorArchitecture = Architecture.Intel; break;
				case PROCESSOR_ARCHITECTURE_MIPS:			m_processorArchitecture = Architecture.Mips; break;
				case PROCESSOR_ARCHITECTURE_ALPHA:			m_processorArchitecture = Architecture.Alpha; break;
				case PROCESSOR_ARCHITECTURE_PPC:			m_processorArchitecture = Architecture.Ppc; break;
				case PROCESSOR_ARCHITECTURE_SHX:			m_processorArchitecture = Architecture.Shx; break;
				case PROCESSOR_ARCHITECTURE_ARM:			m_processorArchitecture = Architecture.Arm; break;
				case PROCESSOR_ARCHITECTURE_IA64:			m_processorArchitecture = Architecture.IA64; break;
				case PROCESSOR_ARCHITECTURE_ALPHA64:		m_processorArchitecture = Architecture.Alpha64; break;
				case PROCESSOR_ARCHITECTURE_MSIL:			m_processorArchitecture = Architecture.MsIl; break;
				case PROCESSOR_ARCHITECTURE_AMD64:			m_processorArchitecture = Architecture.Amd64; break;
				case PROCESSOR_ARCHITECTURE_IA32_ON_WIN64:	m_processorArchitecture = Architecture.Ia32OnWin64; break;
				case PROCESSOR_ARCHITECTURE_UNKNOWN:		m_processorArchitecture = Architecture.Unknown; break;
				default:									m_processorArchitecture = Architecture.Unknown; break;
			}

			switch(info.dwProcessorType)
			{
				case PROCESSOR_INTEL_386:		m_processorType = CpuType.cpu386; break;
				case PROCESSOR_INTEL_486:		m_processorType = CpuType.cpu486; break;
				case PROCESSOR_INTEL_PENTIUM:	m_processorType = CpuType.cpuPentium; break;
				default:						m_processorType = CpuType.cpuPentium; break;
			}

			m_reserved = info.wReserved;
			m_pageSize = info.dwPageSize;
			m_minimumApplicationAddress = (long)info.lpMinimumApplicationAddress;
			m_maximumApplicationAddress = (long)info.lpMaximumApplicationAddress;
			m_activeProcessorMask = (long)info.dwActiveProcessorMask;
			m_numberOfProcessors = info.dwNumberOfProcessors;
			m_allocationGranularity = info.dwAllocationGranularity;
			m_processorLevel = info.wProcessorLevel;
			m_processorRevision = info.wProcessorRevision;
		}

		#region members

		private Architecture m_processorArchitecture = Architecture.Unknown;
		private int m_reserved = 0;
		private long m_pageSize = 0;
		private long m_minimumApplicationAddress = 0;
		private long m_maximumApplicationAddress = 0;
		private long m_activeProcessorMask = 0;
		private long m_numberOfProcessors = 0;
		private CpuType m_processorType = CpuType.cpuPentium;
		private long m_allocationGranularity  = 0;
		private int m_processorLevel = 0;
		private int m_processorRevision = 0;

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
