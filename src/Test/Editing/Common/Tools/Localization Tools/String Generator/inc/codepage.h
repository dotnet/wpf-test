#ifndef __STRGEN_CODEPAGE_
#define __STRGEN_CODEPAGE_

extern UINT gEnum_Size;
extern UINT gEnum_Counter;
extern UINT* gEnum_CodPageList;

UINT  StringToCodePage(TCHAR* lpstrCodePage);
static BOOL CALLBACK EnumCount(LPTSTR lpCodePageString);
static BOOL CALLBACK EnumStore(LPTSTR lpCodePageString);

class _Generic_CodePageInfo
{
private:


public:
	//Data
    const static DWORD c_NOTHING_IS_DONE = 0xFFFFFFFF;
	INT m_Size;

	_Generic_CodePageInfo();
	virtual ~_Generic_CodePageInfo();

	virtual DWORD Init();
	virtual BOOL IsExist(UINT uCodePage);
	INT Size(){return m_Size;};

	
};

class CodePageInfo:public _Generic_CodePageInfo
{
private:
	
	DWORD m_Type;
	UINT* m_CodePageList;	
public:
	const static DWORD c_INVALID_TYPE = 0xFFFFFFFF;
	CodePageInfo();
	virtual ~CodePageInfo();

	DWORD Init(DWORD dwCPType);
	BOOL IsExist(UINT uCodePage);
	DWORD Type(){return m_Type;};
	UINT*  CodePageList(){return m_CodePageList;};
};

class CodePageCollect:public _Generic_CodePageInfo
{
private:
	CodePageInfo *m_Entry;

public:
	const static INT c_CollectionSize = 2;
	const static INT c_INSTALLED = 0;
	const static INT c_SUPPORTED = 1;

	CodePageCollect();
	virtual ~CodePageCollect();

	DWORD Init();
	CodePageInfo* Collection(){return m_Entry;};
	BOOL IsExist(UINT uCodePage);
	BOOL IsExistInCPType(INT iType,UINT uCodePage);
};







#endif //__STRGEN_CODEPAGE_
