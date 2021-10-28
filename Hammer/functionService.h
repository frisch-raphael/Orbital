#pragma once


class FunctionService
{
	void locate(IDiaSession* p_session);
	IDiaSymbol* p_function_symbol;

public:
	FunctionService(IDiaSymbol* p_function_symbol, IDiaSession* p_session);
	DWORD line_number_ = 0;
	DWORD adress_section_ = 0;
	ULONGLONG virtual_adress_ = 0;
	ULONGLONG length_ = 0;
	BSTR source_file_ = SysAllocString(L"");

};

