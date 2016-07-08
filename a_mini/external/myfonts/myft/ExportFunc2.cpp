#include "stdafx.h"
#include "ExportFuncs.h"  
#include "msdfgen\msdfgen_main.h"
//-----------------------------
#include "msdfgen\Contour.h"
#include "msdfgen\Shape.h"
#include "msdfgen\edge-segments.h"
//-------------------------------
#include "msdfgen\msdfgen.h"
#include "msdfgen\Bitmap.h"
//-----------------------------
//int MyFtMSDFGEN(int argc, const char * const *argv) {
int MyFtMSDFGEN(int argc, char **argv) {
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

msdfgen::Shape* CreateShape() {
	msdfgen::Shape* shape = new msdfgen::Shape();
	return shape;
};
msdfgen::Contour* ShapeAddBlankContour(msdfgen::Shape* shape) {
	msdfgen::Contour* cnt = &(shape->addContour());
	return cnt;
};
void ContourAddLinearSegment(msdfgen::Contour* cnt,
	double x0, double y0,
	double x1, double y1) {

	cnt->addEdge(new msdfgen::LinearSegment(
		msdfgen::Point2(x0, y0),
		msdfgen::Point2(x1, y1)
	));
};
void ContourAddQuadraticSegment(msdfgen::Contour* cnt,
	double x0, double y0,
	double ctrl0X, double ctrl0Y,
	double x1, double y1) {

	cnt->addEdge(new msdfgen::QuadraticSegment(
		msdfgen::Point2(x0, y0),
		msdfgen::Point2(ctrl0X, ctrl0Y),
		msdfgen::Point2(x1, y1)
	));
};
void ContourAddCubicSegment(msdfgen::Contour* cnt,
	double x0, double y0,
	double ctrl0X, double ctrl0Y,
	double ctrl1X, double ctrl1Y,
	double x1, double y1) {

	cnt->addEdge(new msdfgen::CubicSegment(
		msdfgen::Point2(x0, y0),
		msdfgen::Point2(ctrl0X, ctrl0Y),
		msdfgen::Point2(ctrl1X, ctrl1Y),
		msdfgen::Point2(x1, y1)
	));
};
void MyFtGenerateMsdf(msdfgen::Shape* shape, int width, int height, double range,
	double scale, double tx, double ty, double edgeThreshold,
	double angleThreshold, int* outputBitmap) {

	msdfgen::Bitmap<msdfgen::FloatRGB> bmp(width, height);
	if (edgeThreshold < 0) {
		edgeThreshold = 1.00000001;//use default
	}

	edgeColoringSimple(*shape, angleThreshold);
	generateMSDF(bmp, *shape, range, scale, msdfgen::Vector2(tx, ty), edgeThreshold);
	//convert to int bmp

	int* outputH = outputBitmap;
	for (int y = height - 1; y >= 0; --y) {
		for (int x = 0; x < width; ++x) {
			//----------------------------------
			auto pixel = bmp(x, y);
			//a b g r
			*outputH = (255 << 24) |
				(msdfgen::clamp(int(pixel.b * 0x100), 0xff) << 16) |
				(msdfgen::clamp(int(pixel.g * 0x100), 0xff) <<8) |
				msdfgen::clamp(int(pixel.r * 0x100), 0xff);

			outputH += 1;
			//----------------------------------
			/**it++ = clamp(int(bitmap(x, y).r*0x100), 0xff);
			*it++ = clamp(int(bitmap(x, y).g*0x100), 0xff);
			*it++ = clamp(int(bitmap(x, y).b*0x100), 0xff);*/
		}
	}
};

MY_DLL_EXPORT bool ShapeValidate(msdfgen::Shape* shape) {
	return shape->validate();
};
MY_DLL_EXPORT void ShapeNormalize(msdfgen::Shape* shape) {
	shape->normalize();
};
MY_DLL_EXPORT void SetInverseYAxis(msdfgen::Shape* shape, bool inverseYAxis) {
	shape->inverseYAxis = inverseYAxis;
};