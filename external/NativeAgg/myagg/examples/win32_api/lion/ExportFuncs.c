#include "ExportFuncs.h"  


del02 managedListner; 
int LibGetVersion()
{	
	return 2;
};
//==================================
int RegisterManagedCallBack(del02 funcPtr,int callbackKind)
{	
	switch(callbackKind)
	{
	case 0:
		{
		 	 
			managedListner= funcPtr; 
			return 0;
		}break;
	case 1:
		{
			 
		}break;
	} 
	return 1;
}
int TestCallBack()
{
	managedListner(0,1);
	return 20;
} 
 
int CallServices(int serviceNumber)
{
	switch(serviceNumber)
	{
		case 1:
		{		 

		}break;
		case 2:
	    {   
			 
		}break;				
		case 3:
	    {
			 
		}break; 
		case 4:
		{
		 
		}break;
		case 5:
	    { 

		}break; 
		default:
		{
			
		}
	}
	return 0;
}
 

//----------------------------------------------------
