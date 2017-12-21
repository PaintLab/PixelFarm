#define MY_DLL_EXPORT __declspec(dllexport)  
 
typedef void (*del02)(int oIndex,int methodName);

MY_DLL_EXPORT int LibGetVersion();
MY_DLL_EXPORT int RegisterManagedCallBack(del02 callback,int callBackKind);
MY_DLL_EXPORT int TestCallBack();  
MY_DLL_EXPORT int CallServices(int serviceNumber); 

