#include "stdafx.h"
#include <stdio.h>
#include <unicode/brkiter.h>
#include <unicode/urename.h>
#include <unicode/udata.h>
#include <stdlib.h>

#include "ExportFuncs.h"  

void MyFt_IcuSetDataDir(const char* datadir){
	u_setDataDirectory(datadir);
}
void MyFt_IcuSetData(const void* dataBuffer, UErrorCode* errCode){
	udata_setCommonData(dataBuffer,errCode);
}
//
UBreakIterator* MtFt_UbrkOpen(UBreakIteratorType iterType,const char* locale, const wchar_t* startChar, int len, UErrorCode* errCode){	 
	
	return ubrk_open((UBreakIteratorType)iterType,locale,startChar,len, errCode); 	 
}
void MtFt_UbrkClose(UBreakIterator* brkIter){
	ubrk_close(brkIter);
}
int MtFt_UbrkFirst(UBreakIterator* brkIter){
	return ubrk_first(brkIter);
}
int MtFt_UbrkNext(UBreakIterator* brkIter){
	return ubrk_next(brkIter);
}
int MtFt_UbrkGetRuleStatus(UBreakIterator* brkIter){
	return ubrk_getRuleStatus(brkIter);
}
 