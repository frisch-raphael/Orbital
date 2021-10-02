#include "pch.h"
#include "pdbService.h"
#include "hammer.h"
#include <stdexcept>

// data structure manipulation functions for easier marshaling

const wchar_t* CharToWChar(const char* string) {

	// newsize describes the length of the
	// wchar_t string in terms of the number
	// of wide characters, not the number of bytes.
	size_t new_size = strlen(string) + 1;

	// The following creates a buffer large enough to contain
	// the exact number of characters in the original string
	// in the new format. If you want to add more characters
	// to the end of the string, increase the value of newsize
	// to increase the size of the buffer.
	wchar_t* wide_string = new wchar_t[new_size];

	// Convert char* string to a wchar_t* string.
	size_t converted_chars = 0;
	mbstowcs_s(&converted_chars, wide_string, new_size, string, _TRUNCATE);
	return wide_string;
}

// Convert from STL wstring to the ATL BSTR wrapper
inline CComBSTR ToBstr(const std::wstring stlString)
{
	// Special case of empty string
	if (stlString.empty())
	{
		return CComBSTR();
	}
	return CComBSTR(static_cast<int>(stlString.size()), stlString.data());
}

CComSafeArray<BSTR> StlToAtlStrings(std::vector<std::wstring> stlStrings)
{
	CComSafeArray<BSTR> safe_array(stlStrings.size());

	for (LONG i = 0; i < stlStrings.size(); i++)
	{
		// Copy the i-th wstring to a BSTR string
		// safely wrapped in ATL::CComBSTR
		CComBSTR bstr = ToBstr(stlStrings[i]);
		HRESULT hr;
		hr = safe_array.SetAt(i, bstr);

		if (FAILED(hr))
		{
			AtlThrow(hr);
		}
	}

	return safe_array;
}

// test functions
const wchar_t* pdb_test_path = L"C:\\Users\\rafou\\source\\repos\\NajiN\\Rodin\\Services\\Hammer\\Hammer.dll";
//const wchar_t* pdb_test_path = L"C:\\Users\\rafou\\source\\repos\\NajiN\\Rodin\\Services\\Hammer\\cmd.exe";
//const wchar_t* pdb_test_path = L"C:\\Users\\rafou\\source\\repos\\NajiN\\Rodin\\Uploads\\f2yd3we5.dll";

struct BstrResponse TestBstr()
{
	BstrResponse response;

	try
	{
		Pdb pdb_service(pdb_test_path);
		response.data = SysAllocString(pdb_service.TestBstr().c_str());
	}
	catch (const std::exception& ex)
	{
		const char* error_message = ex.what();
		response.is_error = true;
		response.error = SysAllocString(CharToWChar(error_message));
	}

	return response;

}

struct BstrArrayResponse TestBstrArray()
{
	BstrArrayResponse response;

	try
	{
		Pdb pdb_service(pdb_test_path);
		response.data = StlToAtlStrings(pdb_service.TestBstrArray());
	}
	catch (const std::exception& ex)
	{
		const char* error_message = ex.what();
		response.is_error = true;
		response.error = SysAllocString(CharToWChar(error_message));
	}

	return response;

}

// ReturnIntegerArray() creates in memory an array of
// 10 integers and returns to the caller a pointer to
// this array.
void TestInts
(
	/*[out]*/ int** ppIntegerArrayReceiver,
	/*[out]*/ int* iSizeReceiver
)
{
	*iSizeReceiver = 10;
	*ppIntegerArrayReceiver = (int*)::CoTaskMemAlloc(sizeof(int) * 10);

	for (int i = 0; i < 10; i++)
	{
		(*ppIntegerArrayReceiver)[i] = i;
	}

}

void TestFunctions
(
	/*[out]*/ Function** pp_structs_receiver,
	/*[out]*/ int* p_size_receiver
)
{
	*p_size_receiver = 3;
	int totalSize = sizeof(Function) * 3;
	*pp_structs_receiver = (Function*)::CoTaskMemAlloc(totalSize);

	(*pp_structs_receiver)[0] = { SysAllocString(L"Function A"), SysAllocString(L"File A"), 1 };
	(*pp_structs_receiver)[1] = { SysAllocString(L"Function B"), SysAllocString(L"File B"), 2 };
	(*pp_structs_receiver)[2] = { SysAllocString(L"Function C"), SysAllocString(L"File C"), 3 };

}

FunctionsResponse TestFunctionsResponse
(
	/*[out]*/ int* p_size_receiver
)
{
	*p_size_receiver = 3;
	int totalSize = sizeof(Function) * 3;

	Function* p_structs = (Function*)::CoTaskMemAlloc(totalSize);

	p_structs[0] = { SysAllocString(L"Function A"), SysAllocString(L"File A"), 1 };
	p_structs[1] = { SysAllocString(L"Function B"), SysAllocString(L"File B"), 2 };
	p_structs[2] = { SysAllocString(L"Function C"), SysAllocString(L"File C"), 3 };

	FunctionsResponse response;

	try
	{
		Pdb pdb_service(pdb_test_path);
		response.data = p_structs;

	}
	catch (const std::exception& ex)
	{
		const char* error_message = ex.what();
		response.is_error = true;
		response.error = SysAllocString(CharToWChar(error_message));
	}

	return response;
}


// exposed functions


FunctionsResponse GetFunctions
(
	/*[out]*/ int* p_number_of_elements,
	/*[in]*/ BSTR pe_path
)
{
	FunctionsResponse response;

	try
	{
		Pdb pdb_service(pe_path);

		std::vector<Function> pdb_functions_vector = pdb_service.GetFunctions();
		*p_number_of_elements = pdb_functions_vector.size();

		int pdb_function_size = sizeof(Function);
		int totalSize = pdb_function_size * *p_number_of_elements;
		response.data = (Function*)::CoTaskMemAlloc(totalSize);

		for (size_t i = 0; i < *p_number_of_elements; i++)
		{
			response.data[i] = pdb_functions_vector[i];
		}

	}
	catch (const std::exception& ex)
	{
		const char* error_message = ex.what();
		response.is_error = true;
		response.error = SysAllocString(CharToWChar(error_message));
	}

	return response;
}

PdbSourceFilesResponse GetSourceFiles
(
	/*[out]*/ int* p_number_of_elements
)
{
	PdbSourceFilesResponse response;

	try
	{
		Pdb pdb_service(pdb_test_path);

		std::vector<PdbSourceFile> pdb_functions_vector = pdb_service.GetSourceFiles();
		*p_number_of_elements = pdb_functions_vector.size();

		int struct_size = sizeof(PdbSourceFile);
		int totalSize = struct_size * *p_number_of_elements;
		response.data = (PdbSourceFile*)::CoTaskMemAlloc(totalSize);

		for (size_t i = 0; i < *p_number_of_elements; i++)
		{
			response.data[i] = pdb_functions_vector[i];
		}

	}
	catch (const std::exception& ex)
	{
		const char* error_message = ex.what();
		response.is_error = true;
		response.error = SysAllocString(CharToWChar(error_message));
	}

	return response;
}


