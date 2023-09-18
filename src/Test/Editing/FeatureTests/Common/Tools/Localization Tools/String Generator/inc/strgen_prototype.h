#ifndef __strgen_prototype_
#define __strgen_prototype_

#include <stdlib.h>
#include "windows.h"
#include "codepoint.h"

#define STRGEN_ERROR MAKELONG(-1,0)
#define STRGEN_MODE_EVEN_DISTRIBUTION_IN_SEGEMENT 0 
#define STRGEN_MODE_EVEN_DISTRIBUTION_IN_RANGE	  1

#define STRGEN_STYLE_NETBIOS_FRIENDLY	0x1
#define STRGEN_STYLE_CODEPAGE_CHECK	0x2

typedef struct _GENERIC_LIST
{
 UINT* List;
 INT numList;
} CODE_PAGELIST, *PCODE_PAGELIST,  LOCALE_LIST, *PLOCALE_LIST;

typedef struct _SEGMET_LIST
{
 CPT_SEGMENT_NAME * List;
 INT numList;
}SEGMENT_LIST,  *PSEGMENT_LIST;



typedef struct _RANDOM_DATA
{
 CODE_PAGELIST CPInfo;
 SEGMENT_LIST Segments;
 LOCALE_LIST Locales;

 LCID LocaleID;
 UINT uiMode; 
 UINT uiStyle;


}RANDOM_DATA, *PRANDOM_DATA;



// Character Validation
bool __IsDBCSLocale(LANGID LangID);
bool __IsSurrogate(DWORD dwInput);
bool __IsError(DWORD dwInput);
bool __IsInSegment(CPT_SEGMENT_NAME segmentID, DWORD dwInput, LANGID LangID = -1);
bool __IsValidInCP(UINT uiCP, DWORD dwInput);
UINT __GetCPFromLocale(LCID Locale, LCTYPE LCType);
BOOL __IsValidNetBios(LCID lcid, DWORD dwInput);


// Random Character Generation
DWORD __RandomOneCharFromSegmentW(CPT_SEGMENT_NAME segmentID, LANGID LangID = -1);
DWORD __RandomOneCharFromSegmentsEvenW(CPT_SEGMENT_NAME *Segments, int numSegment, LANGID LangID = -1);
DWORD __RandomOneCharFromSegmentsRangeW(CPT_SEGMENT_NAME *Segments, int numSegment, LANGID LangID = -1);
DWORD __RandomOneCharFromSegmentsW(CPT_SEGMENT_NAME *Segments, int numSegment, LANGID LangID, UINT uiMode);
DWORD __RandomStrFromSegmentsW(PRANDOM_DATA lpData, LPWSTR lpwstrBuffer, INT iNum);




//Risk Character Generation
DWORD __RandomOneRiskChar(LANGID LangID, CPT_SEGMENT_NAME Segment);


class SBCS_Risk {
public:
	SBCS_Risk(LANGID LangID);
	virtual ~SBCS_Risk();

	DWORD RandomOneRiskSBCSChar(CPT_SEGMENT_NAME SegmentID);
	DWORD RandomOneRiskSBCSChar();


	int IsSBCSRisk(WCHAR wcInput);
	int IsSBCSRisk2(WCHAR wcInput);

protected:
	UINT m_uiAnsiCP;
	UINT m_uiOEMCP;
	LCID m_Locale;
	BOOL m_bValid;

};

class DBCS_Risk {
public:
	DBCS_Risk(LANGID LangID);
	virtual ~DBCS_Risk();
	DWORD RandomOneRiskDBCSChar();
	int IsDBCSRisk(WCHAR wcInput);
	
protected:
	UINT m_uiAnsiCP;
	CPINFO m_CPInfo;
	LCID m_Locale;
	BOOL m_bValid;
};



// Syntanx
//WORD RandomString(wchar *syntax, wchar *buffer);
//WORD RandomString(uint flag, wchar *buffer);
//#define VTH_VARTRACE(a,b)			\
//	{								\
//	WCHAR __szBuf[255];				\
//	wsprintfW( __szBuf, a, b);		\
//	OutputDebugStringW( __szBuf );	\
//	}



#endif