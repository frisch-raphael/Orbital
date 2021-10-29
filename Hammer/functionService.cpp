#include "pch.h"
#include "functionService.h"

FunctionService::FunctionService(IDiaSymbol* p_function_symbol, IDiaSession* p_session)
{
	this->p_function_symbol = p_function_symbol;
	this->locate(p_session);
}

void FunctionService::locate(IDiaSession* p_session)
{
	DWORD adress_offset;
	this->p_function_symbol->get_addressSection(&this->adress_section_);
	this->p_function_symbol->get_addressOffset(&adress_offset);
	this->p_function_symbol->get_length(&this->length_);

	if (this->adress_section_ == 0 || this->length_ == 0) throw std::runtime_error("function symbol point to empty data");

	IDiaEnumLineNumbers* p_lines;
	if (FAILED(p_session->findLinesByAddr(
		this->adress_section_,
		adress_offset,
		static_cast<DWORD>(this->length_),
		&p_lines)
	))
	{
		throw std::runtime_error("findLinesByAddr failed while trying to fetch line number from symbol");
	}
	IDiaLineNumber* p_first_line;

	if (FAILED(p_lines->Item(0, &p_first_line)))
	{
		// ?
		return;
	}

	IDiaSourceFile* p_source_file;

	p_first_line->get_sourceFile(&p_source_file);
	p_first_line->get_lineNumber(&this->line_number_);
	p_source_file->get_fileName(&this->source_file_);
	p_first_line->get_addressSection(&this->adress_section_);
	p_first_line->get_relativeVirtualAddress(&this->virtual_adress_);
}