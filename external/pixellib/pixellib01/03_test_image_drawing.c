#include "../npixel.h"

 
IBITMAP *picture, *zaka;

void drawing3(IVideo* winVideo)
{
	static double theta = 0.0, x = 320;
	static int count = 0, mode = 0;
	IUINT32 r, g, b, a, cc;
	int w, h, i;

	ipaint_fill(winVideo->paint, NULL, 0xffffffff);
	imisc_bitmap_demo(winVideo->cscreen, 0);
	
	w = (int)picture->w;
	h = (int)picture->h;

	if (mode == 0) r = g = b = 0xff, a = count;
	else if (mode == 1) r = count, g = b = 0, a = 0xff;
	else if (mode == 2) g = count, r = b = 0, a = 0xff;
	else if (mode == 3) b = count, r = g = 0, a = 0xff;
	else if (mode == 4) r = b = count, g = 0, a = 0xff;

	cc = IRGBA_TO_PIXEL(A8R8G8B8, r, g, b, a);

	ipaint_raster_draw(winVideo->paint, 480, 320, picture, NULL,
		w / 2, h / 2, 1.0, 1.0, 15, cc);

	ipaint_raster_draw(winVideo->paint, x, 250 + zaka->h, zaka, NULL, 0, 0, 1.0, 1.0, 0, 0xffffffff);
	ipaint_draw(winVideo->paint, x, 250, zaka, NULL, 0xffffffff, 0);

	ipaint_text_background(winVideo->paint, 0);
	ipaint_text_color(winVideo->paint, 0xaf00cccc);
	ipaint_sprintf(winVideo->paint, 1, 260, "TRADITION INTEGER POSITION DRAW");
	ipaint_sprintf(winVideo->paint, 1, 320, "MORDEN FLOAT POINT POSITION DRAW");
	ipaint_sprintf(winVideo->paint, 400, 460, "COLOR SPACE TRANSFORM");

	ipaint_raster_draw(winVideo->paint, 150, 120, picture, NULL,
		w / 2, h / 2, 1.0, 1.0, theta, 0xffffffff);

	ipaint_raster_draw_3d(winVideo->paint, 400, 120, 0, picture, NULL,
		w / 2, h / 2, 1.5, 1.5, theta * 10, 30, 0, 0xffffffff);
	theta += 0.5;
	x -= 0.1;

	ipaint_sprintf(winVideo->paint, 1, 0, "2D ROTATION");
	ipaint_sprintf(winVideo->paint, 321, 0, "3D ROTATION");

	if (x < -((int)picture->w)) x = 320;
	if (theta < -360) theta = 0;
	count++;
	if (++count > 255) {
		count = 0;
		if (++mode > 4) mode = 0;
	}
}

//! lib: .
//! link: pixel
//! win: 
int main3(IVideo* winVideo)
{
	int retval; 
 
	if ((retval = iscreen_init(1024, 600, 32,winVideo->hWnd,winVideo)) != 0) {
		printf("error init\n");
		return -1;
	}

	//assign drawing canvas
	winVideo->paint = ipaint_create(winVideo->cscreen);

	picture = ipic_load_file("res_fish_small.bmp", 0, NULL);
	zaka = ipic_load_file("res_zaka.bmp", 0, NULL);
	assert(picture);

	picture = ipic_convert(picture, IPIX_FMT_A8R8G8B8, 0);
	zaka = ipic_convert(zaka, IPIX_FMT_A8R8G8B8, 0);
	ibitmap_imode(picture, overflow) = IBOM_REPEAT;
	ibitmap_imode(zaka, overflow) = IBOM_REPEAT;

	/*while (iscreen_dispatch() == 0) {
		iscreen_tick(30);
		if (iscreen_keyon(IKEY_ESCAPE)) break;

		drawing3();

		iscreen_convert(NULL, 1);
		iscreen_update(NULL, 1);
	}*/

	return 0;
}

