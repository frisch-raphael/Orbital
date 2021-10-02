#pragma once
#pragma pack(8)

#ifdef HAMMER_EXPORTS
#define HAMMER_API __declspec(dllexport)
#else
#define HAMMER_API __declspec(dllimport)
#endif

struct BstrResponse
{
	BSTR data;
	BSTR error;
	bool is_error = false;
};

struct BstrArrayResponse
{
	CComSafeArray<BSTR> data = CComSafeArray<BSTR>((ULONG)0);
	BSTR error;
	bool is_error = false;
};

struct FunctionsResponse
{
	Function* data;
	BSTR error = SysAllocString(L"");
	bool is_error = false;
};

struct PdbSourceFilesResponse
{
	PdbSourceFile* data;
	BSTR error = SysAllocString(L"");
	bool is_error = false;
};



const wchar_t* CharToWChar(const char* string);

// test functions

extern "C" HAMMER_API BstrResponse TestBstr();
extern "C" HAMMER_API BstrArrayResponse TestBstrArray();
extern "C" HAMMER_API void TestInts(
	/*[out]*/ int** ppIntegerArrayReceiver,
	/*[out]*/ int* iSizeReceiver
);
extern "C" HAMMER_API void TestFunctions(
	/*[out]*/ Function * *pp_structs_receiver,
	/*[out]*/ int* p_size_receiver
);
extern "C" HAMMER_API FunctionsResponse TestFunctionsResponse(
	/*[out]*/ int* p_size_receiver
);

// exposed functions

extern "C" HAMMER_API FunctionsResponse GetFunctions(
	/*[out]*/ int* p_size_receiver,
	/*[in]*/ BSTR pe_path

);
extern "C" HAMMER_API PdbSourceFilesResponse GetSourceFiles(
	/*[out]*/ int* p_number_of_elements

);



