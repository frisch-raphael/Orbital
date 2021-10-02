#include "pch.h"
#include <exception>
#include <stdexcept>
#include <system_error>
#include <filesystem>
#include "pdbService.h"
#include <iostream>
#include "functionService.h"

Pdb::Pdb(const wchar_t* pdb_source_path)
{
	wchar_t extension[MAX_PATH];
	const wchar_t* search_path = L"SRV**\\\\symbols\\symbols";  // Alternate path to search for debug data

	if (!std::filesystem::exists(pdb_source_path))
	{
		throw std::runtime_error("File does not exist");
	}

	HRESULT hr = CoInitialize(NULL);

	// Obtain access to the provider
	hr = CoCreateInstance(
		__uuidof(DiaSource),
		NULL,
		CLSCTX_INPROC_SERVER,
		__uuidof(IDiaDataSource),
		(void**)&this->p_source_
	);

	if (FAILED(hr))
	{
		throw std::runtime_error("CoCreateInstance failed : " + std::system_category().message(hr));
	}

	_wsplitpath_s(pdb_source_path, NULL, 0, NULL, 0, NULL, 0, extension, MAX_PATH);

	if (!_wcsicmp(extension, L".pdb"))
	{
		// Open and prepare a program database (.pdb) file as a debug data source
		hr = this->p_source_->loadDataFromPdb(pdb_source_path);

		if (FAILED(hr))
		{
			throw std::runtime_error("loadDataFromPdb failed - HRESULT = " + std::system_category().message(hr));
		}
	}

	else if (!_wcsicmp(extension, L".dll") || !_wcsicmp(extension, L".exe"))
	{
		CCallback callback;  // Receives callbacks from the DIA symbol locating
							 // procedure, thus enabling a user interface to report
							 // on the progress of the location attempt. The client
							 // application may optionally provide a reference to
							 // its own implementation of this virtual base class to
							 // the IDiaDataSource::loadDataForExe method.
		callback.AddRef();

		// Open and prepare the debug data associated with the executable
		hr = this->p_source_->loadDataForExe(pdb_source_path, search_path, &callback);

		if (FAILED(hr))
		{
			throw std::runtime_error("No PDB data found in exe - HRESULT = " + std::system_category().message(hr));
		}
	}
	else
	{
		throw std::runtime_error("Extension of provided pdb source is not supported");
	}

	// Open a session for querying symbols
	hr = this->p_source_->openSession(&this->p_session_);

	if (FAILED(hr))
	{
		throw std::runtime_error("openSession failed - HRESULT = " + hr);
	}

	// Retrieve a reference to the global scope
	hr = this->p_session_->get_globalScope(&this->p_globalSymbol_);

	if (hr != S_OK)
	{
		throw std::runtime_error("get_globalScope failed");
	}
}


std::vector<PdbSourceFile> Pdb::GetSourceFiles()
{
	std::vector<PdbSourceFile> source_files;

	IDiaEnumTables* p_tables;
	if (FAILED(this->p_session_->getEnumTables(&p_tables)))
	{
		throw std::runtime_error("getEnumTables failed");
	}
	IDiaTable* p_table;

	ULONG celt = 0;
	while (SUCCEEDED(p_tables->Next(1, &p_table, &celt))
		&& celt == 1)
	{
		IDiaEnumSourceFiles* p_source_files_enum;


		if (SUCCEEDED(p_table->QueryInterface(
			_uuidof(IDiaEnumSourceFiles),
			(void**)&p_source_files_enum)
		)
			)
		{
			IDiaSourceFile* p_source_file;

			while (SUCCEEDED(p_source_files_enum->Next(1, &p_source_file, &celt)) &&
				celt == 1)
			{
				BSTR fileName;
				if (p_source_file->get_fileName(&fileName) == S_OK)
				{
					PdbSourceFile source_file = {
						fileName
					};
					source_files.push_back(source_file);
				}
				p_source_file = NULL;
			}
		}
	}

	return source_files;
}

std::vector<Function> Pdb::GetFunctions()
{
	IDiaEnumSymbols* enumSymbols;

	if (FAILED(this->p_globalSymbol_->findChildren(
		SymTagFunction,
		NULL,
		nsNone,
		&enumSymbols)))
	{
		throw std::runtime_error("Could not find children for global symbol");
	}

	IDiaSymbol* symbol;
	ULONG celt = 0;
	std::vector<Function> pdb_functions;

	while (SUCCEEDED(enumSymbols->Next(1, &symbol, &celt)) && (celt == 1))
	{
		std::optional<Function> pdb_function = GetFunction(symbol);
		if (pdb_function) pdb_functions.push_back(pdb_function.value());
		symbol->Release();
	}
	enumSymbols->Release();
	return pdb_functions;
}

std::optional<Function> Pdb::GetFunction(IDiaSymbol* p_symbol)
{

	BSTR source_filename;
	BSTR undecorated_name;
	p_symbol->get_undecoratedName(&undecorated_name);

	FunctionService function(p_symbol, this->p_session_);

	Function pdb_function = {
		undecorated_name,
		function.source_file_,
		function.line_number_,
		function.adress_section_,
		function.adress_offset_,
		function.length_,
	};

	return pdb_function;

}

std::wstring Pdb::TestBstr() { return L"OK FROM TESTBSTR"; }

std::vector<std::wstring> Pdb::TestBstrArray()
{
	std::vector<std::wstring> bstrArray;
	bstrArray.push_back(L"A");
	bstrArray.push_back(L"B");
	return bstrArray;
}


Pdb::~Pdb()
{
	this->p_globalSymbol_->Release();
	this->p_globalSymbol_ = NULL;

	this->p_session_->Release();
	this->p_session_ = NULL;

	CoUninitialize();
}