#include "stdafx.h"
#include "ExportFuncs.h"  
#include "msdfgen_main.h"

//int MyFtMSDFGEN(int argc, const char * const *argv) {
int MyFtMSDFGEN(int argc, char **argv){
	//char* test= "msdfgen msdf -font C:\\Windows\\Fonts\\tahoma.ttf 'M' - o msdf.png - size 32 32 - pxrange 4 - autoframe - testrender render.png 1024 1024";
	//char* arr = strtok(test,  " ");

	//char *a[18];
	//a[0] = "msdfgen";
	//a[1] = "msdf";
	//a[2] = "-font";
	//a[3] = "C:\\Windows\\Fonts\\tahoma.ttf";
	//a[4] = "'M'";
	//a[5] = "-o";
	//a[6] = "msdf.png";
	//a[7] = "-size";
	//a[8] = "32";
	//a[9] = "32";
	//a[10] = "-pxrange";
	//a[11] = "4";
	//a[12] = "-autoframe";
	//a[13] = "-testrenderer";
	//a[14] = "renderer.png";
	//a[15] = "1024";
	//a[16] = "1024";
	//a[17] = 0; 
	//return msdfgen_main(17, a);
	return msdfgen_main(argc, argv);
};