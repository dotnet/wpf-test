#ifndef __STRGENLIB_H__
#define __STRGENLIB_H__

//#ifdef _LIB
  
//#define STRGENDLL_API

//#else
  #ifdef STRGENDLL_EXPORTS
  #define STRGENDLL_API __declspec(dllexport)
  #else
  #define STRGENDLL_API //__declspec(dllimport)
  #endif
//#endif

 
#include "codepoint.h"
#include "strgen_prototype.h"

#ifdef __cplusplus
extern "C" {
#endif

STRGENDLL_API BOOL WINAPI StrGen_IsDBCSLocale(LANGID LangID);
STRGENDLL_API BOOL WINAPI StrGen_IsSurrogate(DWORD dwInput);
STRGENDLL_API BOOL WINAPI StrGen_IsError(DWORD dwInput);
STRGENDLL_API BOOL WINAPI StrGen_IsInSegment(CPT_SEGMENT_NAME segmentID, DWORD dwInput, LANGID LangID);
STRGENDLL_API BOOL WINAPI StrGen_IsValidInCP(UINT uiCP, DWORD dwInput);
STRGENDLL_API UINT WINAPI StrGen_GetCPFromLocale(LCID Locale, LCTYPE LCType);

STRGENDLL_API DWORD WINAPI StrGen_RandomOneCharFromSegmentW(CPT_SEGMENT_NAME segmentID, LANGID LangID = -1);
STRGENDLL_API DWORD WINAPI StrGen_RandomOneCharFromSegmentsEvenW(CPT_SEGMENT_NAME *Segments, int numSegment, LANGID LangID = -1);
STRGENDLL_API DWORD WINAPI StrGen_RandomOneCharFromSegmentsW(CPT_SEGMENT_NAME *Segments, int numSegment, LANGID LangID = -1, UINT uiMode = STRGEN_MODE_EVEN_DISTRIBUTION_IN_SEGEMENT);
STRGENDLL_API DWORD WINAPI StrGen_RandomStrFromSegmentsW(PRANDOM_DATA lpData, LPWSTR lpwstrBuffer, INT iNum);



//Risk Character
STRGENDLL_API DWORD WINAPI StrGen_RandomOneRiskChar(LANGID LangID, CPT_SEGMENT_NAME Segment);

#ifdef __cplusplus
}
#endif

#endif