#pragma once

#pragma pack(8)

struct Function
{
	BSTR name = SysAllocString(L"");
	BSTR file = SysAllocString(L"");
	int first_line = 0;
	DWORD adress_section = 0;
	DWORD adress_offset = 0;
	ULONGLONG length = 0;
};

struct PdbSourceFile
{
	BSTR file_name = SysAllocString(L"");
};

class Pdb
{
public:
	std::vector<Function> GetFunctions();
	std::vector<PdbSourceFile> GetSourceFiles();
	std::wstring TestBstr();
	std::vector<std::wstring> TestBstrArray();
	Pdb(const wchar_t* fileName);
	~Pdb();

	IDiaDataSource* p_source_;
	IDiaSession* p_session_;
	IDiaSymbol* p_globalSymbol_;
	DWORD machineType_ = CV_CFL_80386;

private:
	std::optional<Function> GetFunction(IDiaSymbol* symbol);

};