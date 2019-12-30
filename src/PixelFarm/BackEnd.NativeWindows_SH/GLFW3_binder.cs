//MIT, 2016-present, WinterDev
//autogen from glfw3.3
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;


/*x4*/
namespace Glfw
{
    //AUTOGEN
    /*x5*/
    [System.Security.SuppressUnmanagedCodeSecurity]
    /*x6*/
    public static class Glfw3
    {
        /*x7*/
        public const string LIB = "glfw3";
        /*x8*/
        public const int GLFW_VERSION_MAJOR = 3;
        /*x9*/
        public const int GLFW_VERSION_MINOR = 3;
        /*x10*/
        public const int GLFW_VERSION_REVISION = 0;
        /*x11*/
        public const int GLFW_TRUE = 1;
        /*x12*/
        public const int GLFW_FALSE = 0;
        /*x13*/
        public const int GLFW_RELEASE = 0;
        /*x14*/
        public const int GLFW_PRESS = 1;
        /*x15*/
        public const int GLFW_REPEAT = 2;
        /*x16*/
        public const int GLFW_HAT_CENTERED = 0;
        /*x17*/
        public const int GLFW_HAT_UP = 1;
        /*x18*/
        public const int GLFW_HAT_RIGHT = 2;
        /*x19*/
        public const int GLFW_HAT_DOWN = 4;
        /*x20*/
        public const int GLFW_HAT_LEFT = 8;
        /*x21*/
        public const int GLFW_HAT_RIGHT_UP = (GLFW_HAT_RIGHT | GLFW_HAT_UP);
        /*x22*/
        public const int GLFW_HAT_RIGHT_DOWN = (GLFW_HAT_RIGHT | GLFW_HAT_DOWN);
        /*x23*/
        public const int GLFW_HAT_LEFT_UP = (GLFW_HAT_LEFT | GLFW_HAT_UP);
        /*x24*/
        public const int GLFW_HAT_LEFT_DOWN = (GLFW_HAT_LEFT | GLFW_HAT_DOWN);
        /*x25*/
        public const int GLFW_KEY_UNKNOWN = -1;
        /*x26*/
        public const int GLFW_KEY_SPACE = 32;
        /*x27*/
        public const int GLFW_KEY_APOSTROPHE = 39  /* ' */;
        /*x28*/
        public const int GLFW_KEY_COMMA = 44  /* , */;
        /*x29*/
        public const int GLFW_KEY_MINUS = 45  /* - */;
        /*x30*/
        public const int GLFW_KEY_PERIOD = 46  /* . */;
        /*x31*/
        public const int GLFW_KEY_SLASH = 47  /* / */;
        /*x32*/
        public const int GLFW_KEY_0 = 48;
        /*x33*/
        public const int GLFW_KEY_1 = 49;
        /*x34*/
        public const int GLFW_KEY_2 = 50;
        /*x35*/
        public const int GLFW_KEY_3 = 51;
        /*x36*/
        public const int GLFW_KEY_4 = 52;
        /*x37*/
        public const int GLFW_KEY_5 = 53;
        /*x38*/
        public const int GLFW_KEY_6 = 54;
        /*x39*/
        public const int GLFW_KEY_7 = 55;
        /*x40*/
        public const int GLFW_KEY_8 = 56;
        /*x41*/
        public const int GLFW_KEY_9 = 57;
        /*x42*/
        public const int GLFW_KEY_SEMICOLON = 59  /* ; */;
        /*x43*/
        public const int GLFW_KEY_EQUAL = 61  /* = */;
        /*x44*/
        public const int GLFW_KEY_A = 65;
        /*x45*/
        public const int GLFW_KEY_B = 66;
        /*x46*/
        public const int GLFW_KEY_C = 67;
        /*x47*/
        public const int GLFW_KEY_D = 68;
        /*x48*/
        public const int GLFW_KEY_E = 69;
        /*x49*/
        public const int GLFW_KEY_F = 70;
        /*x50*/
        public const int GLFW_KEY_G = 71;
        /*x51*/
        public const int GLFW_KEY_H = 72;
        /*x52*/
        public const int GLFW_KEY_I = 73;
        /*x53*/
        public const int GLFW_KEY_J = 74;
        /*x54*/
        public const int GLFW_KEY_K = 75;
        /*x55*/
        public const int GLFW_KEY_L = 76;
        /*x56*/
        public const int GLFW_KEY_M = 77;
        /*x57*/
        public const int GLFW_KEY_N = 78;
        /*x58*/
        public const int GLFW_KEY_O = 79;
        /*x59*/
        public const int GLFW_KEY_P = 80;
        /*x60*/
        public const int GLFW_KEY_Q = 81;
        /*x61*/
        public const int GLFW_KEY_R = 82;
        /*x62*/
        public const int GLFW_KEY_S = 83;
        /*x63*/
        public const int GLFW_KEY_T = 84;
        /*x64*/
        public const int GLFW_KEY_U = 85;
        /*x65*/
        public const int GLFW_KEY_V = 86;
        /*x66*/
        public const int GLFW_KEY_W = 87;
        /*x67*/
        public const int GLFW_KEY_X = 88;
        /*x68*/
        public const int GLFW_KEY_Y = 89;
        /*x69*/
        public const int GLFW_KEY_Z = 90;
        /*x70*/
        public const int GLFW_KEY_LEFT_BRACKET = 91  /* [ */;
        /*x71*/
        public const int GLFW_KEY_BACKSLASH = 92  /* \ */;
        /*x72*/
        public const int GLFW_KEY_RIGHT_BRACKET = 93  /* ] */;
        /*x73*/
        public const int GLFW_KEY_GRAVE_ACCENT = 96  /* ` */;
        /*x74*/
        public const int GLFW_KEY_WORLD_1 = 161 /* non-US #1 */;
        /*x75*/
        public const int GLFW_KEY_WORLD_2 = 162 /* non-US #2 */;
        /*x76*/
        public const int GLFW_KEY_ESCAPE = 256;
        /*x77*/
        public const int GLFW_KEY_ENTER = 257;
        /*x78*/
        public const int GLFW_KEY_TAB = 258;
        /*x79*/
        public const int GLFW_KEY_BACKSPACE = 259;
        /*x80*/
        public const int GLFW_KEY_INSERT = 260;
        /*x81*/
        public const int GLFW_KEY_DELETE = 261;
        /*x82*/
        public const int GLFW_KEY_RIGHT = 262;
        /*x83*/
        public const int GLFW_KEY_LEFT = 263;
        /*x84*/
        public const int GLFW_KEY_DOWN = 264;
        /*x85*/
        public const int GLFW_KEY_UP = 265;
        /*x86*/
        public const int GLFW_KEY_PAGE_UP = 266;
        /*x87*/
        public const int GLFW_KEY_PAGE_DOWN = 267;
        /*x88*/
        public const int GLFW_KEY_HOME = 268;
        /*x89*/
        public const int GLFW_KEY_END = 269;
        /*x90*/
        public const int GLFW_KEY_CAPS_LOCK = 280;
        /*x91*/
        public const int GLFW_KEY_SCROLL_LOCK = 281;
        /*x92*/
        public const int GLFW_KEY_NUM_LOCK = 282;
        /*x93*/
        public const int GLFW_KEY_PRINT_SCREEN = 283;
        /*x94*/
        public const int GLFW_KEY_PAUSE = 284;
        /*x95*/
        public const int GLFW_KEY_F1 = 290;
        /*x96*/
        public const int GLFW_KEY_F2 = 291;
        /*x97*/
        public const int GLFW_KEY_F3 = 292;
        /*x98*/
        public const int GLFW_KEY_F4 = 293;
        /*x99*/
        public const int GLFW_KEY_F5 = 294;
        /*x100*/
        public const int GLFW_KEY_F6 = 295;
        /*x101*/
        public const int GLFW_KEY_F7 = 296;
        /*x102*/
        public const int GLFW_KEY_F8 = 297;
        /*x103*/
        public const int GLFW_KEY_F9 = 298;
        /*x104*/
        public const int GLFW_KEY_F10 = 299;
        /*x105*/
        public const int GLFW_KEY_F11 = 300;
        /*x106*/
        public const int GLFW_KEY_F12 = 301;
        /*x107*/
        public const int GLFW_KEY_F13 = 302;
        /*x108*/
        public const int GLFW_KEY_F14 = 303;
        /*x109*/
        public const int GLFW_KEY_F15 = 304;
        /*x110*/
        public const int GLFW_KEY_F16 = 305;
        /*x111*/
        public const int GLFW_KEY_F17 = 306;
        /*x112*/
        public const int GLFW_KEY_F18 = 307;
        /*x113*/
        public const int GLFW_KEY_F19 = 308;
        /*x114*/
        public const int GLFW_KEY_F20 = 309;
        /*x115*/
        public const int GLFW_KEY_F21 = 310;
        /*x116*/
        public const int GLFW_KEY_F22 = 311;
        /*x117*/
        public const int GLFW_KEY_F23 = 312;
        /*x118*/
        public const int GLFW_KEY_F24 = 313;
        /*x119*/
        public const int GLFW_KEY_F25 = 314;
        /*x120*/
        public const int GLFW_KEY_KP_0 = 320;
        /*x121*/
        public const int GLFW_KEY_KP_1 = 321;
        /*x122*/
        public const int GLFW_KEY_KP_2 = 322;
        /*x123*/
        public const int GLFW_KEY_KP_3 = 323;
        /*x124*/
        public const int GLFW_KEY_KP_4 = 324;
        /*x125*/
        public const int GLFW_KEY_KP_5 = 325;
        /*x126*/
        public const int GLFW_KEY_KP_6 = 326;
        /*x127*/
        public const int GLFW_KEY_KP_7 = 327;
        /*x128*/
        public const int GLFW_KEY_KP_8 = 328;
        /*x129*/
        public const int GLFW_KEY_KP_9 = 329;
        /*x130*/
        public const int GLFW_KEY_KP_DECIMAL = 330;
        /*x131*/
        public const int GLFW_KEY_KP_DIVIDE = 331;
        /*x132*/
        public const int GLFW_KEY_KP_MULTIPLY = 332;
        /*x133*/
        public const int GLFW_KEY_KP_SUBTRACT = 333;
        /*x134*/
        public const int GLFW_KEY_KP_ADD = 334;
        /*x135*/
        public const int GLFW_KEY_KP_ENTER = 335;
        /*x136*/
        public const int GLFW_KEY_KP_EQUAL = 336;
        /*x137*/
        public const int GLFW_KEY_LEFT_SHIFT = 340;
        /*x138*/
        public const int GLFW_KEY_LEFT_CONTROL = 341;
        /*x139*/
        public const int GLFW_KEY_LEFT_ALT = 342;
        /*x140*/
        public const int GLFW_KEY_LEFT_SUPER = 343;
        /*x141*/
        public const int GLFW_KEY_RIGHT_SHIFT = 344;
        /*x142*/
        public const int GLFW_KEY_RIGHT_CONTROL = 345;
        /*x143*/
        public const int GLFW_KEY_RIGHT_ALT = 346;
        /*x144*/
        public const int GLFW_KEY_RIGHT_SUPER = 347;
        /*x145*/
        public const int GLFW_KEY_MENU = 348;
        /*x146*/
        public const int GLFW_KEY_LAST = GLFW_KEY_MENU;
        /*x147*/
        public const int GLFW_MOD_SHIFT = 0x0001;
        /*x148*/
        public const int GLFW_MOD_CONTROL = 0x0002;
        /*x149*/
        public const int GLFW_MOD_ALT = 0x0004;
        /*x150*/
        public const int GLFW_MOD_SUPER = 0x0008;
        /*x151*/
        public const int GLFW_MOD_CAPS_LOCK = 0x0010;
        /*x152*/
        public const int GLFW_MOD_NUM_LOCK = 0x0020;
        /*x153*/
        public const int GLFW_MOUSE_BUTTON_1 = 0;
        /*x154*/
        public const int GLFW_MOUSE_BUTTON_2 = 1;
        /*x155*/
        public const int GLFW_MOUSE_BUTTON_3 = 2;
        /*x156*/
        public const int GLFW_MOUSE_BUTTON_4 = 3;
        /*x157*/
        public const int GLFW_MOUSE_BUTTON_5 = 4;
        /*x158*/
        public const int GLFW_MOUSE_BUTTON_6 = 5;
        /*x159*/
        public const int GLFW_MOUSE_BUTTON_7 = 6;
        /*x160*/
        public const int GLFW_MOUSE_BUTTON_8 = 7;
        /*x161*/
        public const int GLFW_MOUSE_BUTTON_LAST = GLFW_MOUSE_BUTTON_8;
        /*x162*/
        public const int GLFW_MOUSE_BUTTON_LEFT = GLFW_MOUSE_BUTTON_1;
        /*x163*/
        public const int GLFW_MOUSE_BUTTON_RIGHT = GLFW_MOUSE_BUTTON_2;
        /*x164*/
        public const int GLFW_MOUSE_BUTTON_MIDDLE = GLFW_MOUSE_BUTTON_3;
        /*x165*/
        public const int GLFW_JOYSTICK_1 = 0;
        /*x166*/
        public const int GLFW_JOYSTICK_2 = 1;
        /*x167*/
        public const int GLFW_JOYSTICK_3 = 2;
        /*x168*/
        public const int GLFW_JOYSTICK_4 = 3;
        /*x169*/
        public const int GLFW_JOYSTICK_5 = 4;
        /*x170*/
        public const int GLFW_JOYSTICK_6 = 5;
        /*x171*/
        public const int GLFW_JOYSTICK_7 = 6;
        /*x172*/
        public const int GLFW_JOYSTICK_8 = 7;
        /*x173*/
        public const int GLFW_JOYSTICK_9 = 8;
        /*x174*/
        public const int GLFW_JOYSTICK_10 = 9;
        /*x175*/
        public const int GLFW_JOYSTICK_11 = 10;
        /*x176*/
        public const int GLFW_JOYSTICK_12 = 11;
        /*x177*/
        public const int GLFW_JOYSTICK_13 = 12;
        /*x178*/
        public const int GLFW_JOYSTICK_14 = 13;
        /*x179*/
        public const int GLFW_JOYSTICK_15 = 14;
        /*x180*/
        public const int GLFW_JOYSTICK_16 = 15;
        /*x181*/
        public const int GLFW_JOYSTICK_LAST = GLFW_JOYSTICK_16;
        /*x182*/
        public const int GLFW_GAMEPAD_BUTTON_A = 0;
        /*x183*/
        public const int GLFW_GAMEPAD_BUTTON_B = 1;
        /*x184*/
        public const int GLFW_GAMEPAD_BUTTON_X = 2;
        /*x185*/
        public const int GLFW_GAMEPAD_BUTTON_Y = 3;
        /*x186*/
        public const int GLFW_GAMEPAD_BUTTON_LEFT_BUMPER = 4;
        /*x187*/
        public const int GLFW_GAMEPAD_BUTTON_RIGHT_BUMPER = 5;
        /*x188*/
        public const int GLFW_GAMEPAD_BUTTON_BACK = 6;
        /*x189*/
        public const int GLFW_GAMEPAD_BUTTON_START = 7;
        /*x190*/
        public const int GLFW_GAMEPAD_BUTTON_GUIDE = 8;
        /*x191*/
        public const int GLFW_GAMEPAD_BUTTON_LEFT_THUMB = 9;
        /*x192*/
        public const int GLFW_GAMEPAD_BUTTON_RIGHT_THUMB = 10;
        /*x193*/
        public const int GLFW_GAMEPAD_BUTTON_DPAD_UP = 11;
        /*x194*/
        public const int GLFW_GAMEPAD_BUTTON_DPAD_RIGHT = 12;
        /*x195*/
        public const int GLFW_GAMEPAD_BUTTON_DPAD_DOWN = 13;
        /*x196*/
        public const int GLFW_GAMEPAD_BUTTON_DPAD_LEFT = 14;
        /*x197*/
        public const int GLFW_GAMEPAD_BUTTON_LAST = GLFW_GAMEPAD_BUTTON_DPAD_LEFT;
        /*x198*/
        public const int GLFW_GAMEPAD_BUTTON_CROSS = GLFW_GAMEPAD_BUTTON_A;
        /*x199*/
        public const int GLFW_GAMEPAD_BUTTON_CIRCLE = GLFW_GAMEPAD_BUTTON_B;
        /*x200*/
        public const int GLFW_GAMEPAD_BUTTON_SQUARE = GLFW_GAMEPAD_BUTTON_X;
        /*x201*/
        public const int GLFW_GAMEPAD_BUTTON_TRIANGLE = GLFW_GAMEPAD_BUTTON_Y;
        /*x202*/
        public const int GLFW_GAMEPAD_AXIS_LEFT_X = 0;
        /*x203*/
        public const int GLFW_GAMEPAD_AXIS_LEFT_Y = 1;
        /*x204*/
        public const int GLFW_GAMEPAD_AXIS_RIGHT_X = 2;
        /*x205*/
        public const int GLFW_GAMEPAD_AXIS_RIGHT_Y = 3;
        /*x206*/
        public const int GLFW_GAMEPAD_AXIS_LEFT_TRIGGER = 4;
        /*x207*/
        public const int GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER = 5;
        /*x208*/
        public const int GLFW_GAMEPAD_AXIS_LAST = GLFW_GAMEPAD_AXIS_RIGHT_TRIGGER;
        /*x209*/
        public const int GLFW_NO_ERROR = 0;
        /*x210*/
        public const int GLFW_NOT_INITIALIZED = 0x00010001;
        /*x211*/
        public const int GLFW_NO_CURRENT_CONTEXT = 0x00010002;
        /*x212*/
        public const int GLFW_INVALID_ENUM = 0x00010003;
        /*x213*/
        public const int GLFW_INVALID_VALUE = 0x00010004;
        /*x214*/
        public const int GLFW_OUT_OF_MEMORY = 0x00010005;
        /*x215*/
        public const int GLFW_API_UNAVAILABLE = 0x00010006;
        /*x216*/
        public const int GLFW_VERSION_UNAVAILABLE = 0x00010007;
        /*x217*/
        public const int GLFW_PLATFORM_ERROR = 0x00010008;
        /*x218*/
        public const int GLFW_FORMAT_UNAVAILABLE = 0x00010009;
        /*x219*/
        public const int GLFW_NO_WINDOW_CONTEXT = 0x0001000A;
        /*x220*/
        public const int GLFW_FOCUSED = 0x00020001;
        /*x221*/
        public const int GLFW_ICONIFIED = 0x00020002;
        /*x222*/
        public const int GLFW_RESIZABLE = 0x00020003;
        /*x223*/
        public const int GLFW_VISIBLE = 0x00020004;
        /*x224*/
        public const int GLFW_DECORATED = 0x00020005;
        /*x225*/
        public const int GLFW_AUTO_ICONIFY = 0x00020006;
        /*x226*/
        public const int GLFW_FLOATING = 0x00020007;
        /*x227*/
        public const int GLFW_MAXIMIZED = 0x00020008;
        /*x228*/
        public const int GLFW_CENTER_CURSOR = 0x00020009;
        /*x229*/
        public const int GLFW_TRANSPARENT_FRAMEBUFFER = 0x0002000A;
        /*x230*/
        public const int GLFW_HOVERED = 0x0002000B;
        /*x231*/
        public const int GLFW_FOCUS_ON_SHOW = 0x0002000C;
        /*x232*/
        public const int GLFW_RED_BITS = 0x00021001;
        /*x233*/
        public const int GLFW_GREEN_BITS = 0x00021002;
        /*x234*/
        public const int GLFW_BLUE_BITS = 0x00021003;
        /*x235*/
        public const int GLFW_ALPHA_BITS = 0x00021004;
        /*x236*/
        public const int GLFW_DEPTH_BITS = 0x00021005;
        /*x237*/
        public const int GLFW_STENCIL_BITS = 0x00021006;
        /*x238*/
        public const int GLFW_ACCUM_RED_BITS = 0x00021007;
        /*x239*/
        public const int GLFW_ACCUM_GREEN_BITS = 0x00021008;
        /*x240*/
        public const int GLFW_ACCUM_BLUE_BITS = 0x00021009;
        /*x241*/
        public const int GLFW_ACCUM_ALPHA_BITS = 0x0002100A;
        /*x242*/
        public const int GLFW_AUX_BUFFERS = 0x0002100B;
        /*x243*/
        public const int GLFW_STEREO = 0x0002100C;
        /*x244*/
        public const int GLFW_SAMPLES = 0x0002100D;
        /*x245*/
        public const int GLFW_SRGB_CAPABLE = 0x0002100E;
        /*x246*/
        public const int GLFW_REFRESH_RATE = 0x0002100F;
        /*x247*/
        public const int GLFW_DOUBLEBUFFER = 0x00021010;
        /*x248*/
        public const int GLFW_CLIENT_API = 0x00022001;
        /*x249*/
        public const int GLFW_CONTEXT_VERSION_MAJOR = 0x00022002;
        /*x250*/
        public const int GLFW_CONTEXT_VERSION_MINOR = 0x00022003;
        /*x251*/
        public const int GLFW_CONTEXT_REVISION = 0x00022004;
        /*x252*/
        public const int GLFW_CONTEXT_ROBUSTNESS = 0x00022005;
        /*x253*/
        public const int GLFW_OPENGL_FORWARD_COMPAT = 0x00022006;
        /*x254*/
        public const int GLFW_OPENGL_DEBUG_CONTEXT = 0x00022007;
        /*x255*/
        public const int GLFW_OPENGL_PROFILE = 0x00022008;
        /*x256*/
        public const int GLFW_CONTEXT_RELEASE_BEHAVIOR = 0x00022009;
        /*x257*/
        public const int GLFW_CONTEXT_NO_ERROR = 0x0002200A;
        /*x258*/
        public const int GLFW_CONTEXT_CREATION_API = 0x0002200B;
        /*x259*/
        public const int GLFW_SCALE_TO_MONITOR = 0x0002200C;
        /*x260*/
        public const int GLFW_COCOA_RETINA_FRAMEBUFFER = 0x00023001;
        /*x261*/
        public const int GLFW_COCOA_FRAME_NAME = 0x00023002;
        /*x262*/
        public const int GLFW_COCOA_GRAPHICS_SWITCHING = 0x00023003;
        /*x263*/
        public const int GLFW_X11_CLASS_NAME = 0x00024001;
        /*x264*/
        public const int GLFW_X11_INSTANCE_NAME = 0x00024002;
        /*x265*/
        public const int GLFW_NO_API = 0;
        /*x266*/
        public const int GLFW_OPENGL_API = 0x00030001;
        /*x267*/
        public const int GLFW_OPENGL_ES_API = 0x00030002;
        /*x268*/
        public const int GLFW_NO_ROBUSTNESS = 0;
        /*x269*/
        public const int GLFW_NO_RESET_NOTIFICATION = 0x00031001;
        /*x270*/
        public const int GLFW_LOSE_CONTEXT_ON_RESET = 0x00031002;
        /*x271*/
        public const int GLFW_OPENGL_ANY_PROFILE = 0;
        /*x272*/
        public const int GLFW_OPENGL_CORE_PROFILE = 0x00032001;
        /*x273*/
        public const int GLFW_OPENGL_COMPAT_PROFILE = 0x00032002;
        /*x274*/
        public const int GLFW_CURSOR = 0x00033001;
        /*x275*/
        public const int GLFW_STICKY_KEYS = 0x00033002;
        /*x276*/
        public const int GLFW_STICKY_MOUSE_BUTTONS = 0x00033003;
        /*x277*/
        public const int GLFW_LOCK_KEY_MODS = 0x00033004;
        /*x278*/
        public const int GLFW_RAW_MOUSE_MOTION = 0x00033005;
        /*x279*/
        public const int GLFW_CURSOR_NORMAL = 0x00034001;
        /*x280*/
        public const int GLFW_CURSOR_HIDDEN = 0x00034002;
        /*x281*/
        public const int GLFW_CURSOR_DISABLED = 0x00034003;
        /*x282*/
        public const int GLFW_ANY_RELEASE_BEHAVIOR = 0;
        /*x283*/
        public const int GLFW_RELEASE_BEHAVIOR_FLUSH = 0x00035001;
        /*x284*/
        public const int GLFW_RELEASE_BEHAVIOR_NONE = 0x00035002;
        /*x285*/
        public const int GLFW_NATIVE_CONTEXT_API = 0x00036001;
        /*x286*/
        public const int GLFW_EGL_CONTEXT_API = 0x00036002;
        /*x287*/
        public const int GLFW_OSMESA_CONTEXT_API = 0x00036003;
        /*x288*/
        public const int GLFW_ARROW_CURSOR = 0x00036001;
        /*x289*/
        public const int GLFW_IBEAM_CURSOR = 0x00036002;
        /*x290*/
        public const int GLFW_CROSSHAIR_CURSOR = 0x00036003;
        /*x291*/
        public const int GLFW_HAND_CURSOR = 0x00036004;
        /*x292*/
        public const int GLFW_HRESIZE_CURSOR = 0x00036005;
        /*x293*/
        public const int GLFW_VRESIZE_CURSOR = 0x00036006;
        /*x294*/
        public const int GLFW_CONNECTED = 0x00040001;
        /*x295*/
        public const int GLFW_DISCONNECTED = 0x00040002;
        /*x296*/
        public const int GLFW_JOYSTICK_HAT_BUTTONS = 0x00050001;
        /*x297*/
        public const int GLFW_COCOA_CHDIR_RESOURCES = 0x00051001;
        /*x298*/
        public const int GLFW_COCOA_MENUBAR = 0x00051002;
        /*x299*/
        public const int GLFW_DONT_CARE = -1;
        /*x300*//*/*
		/*************************************************************************
		* GLFW API types**************************************************************************/
                /*x301*//*/*
                /*! @brief Client API function pointer type.
                *
                *  Generic function pointer used for returning client API function pointers
                *  without forcing a cast from a regular pointer.
                *
                *  @sa @ref context_glext
                *  @sa @ref glfwGetProcAddress
                *
                *  @since Added in version 3.0.
                *
                *  @ingroup context**/
                        /*x302*/
                        /// <summary>
                        /// Client API function pointer type.
                        /// </summary>
        /*x306*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWglproc(/*x307*/);
        /*x308*//*/*
		/*! @brief Vulkan API function pointer type.
		*
		*  Generic function pointer used for returning Vulkan API function pointers
		*  without forcing a cast from a regular pointer.
		*
		*  @sa @ref vulkan_proc
		*  @sa @ref glfwGetInstanceProcAddress
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup vulkan**/
                /*x309*/
                /// <summary>
                /// Vulkan API function pointer type.
                /// </summary>
        /*x313*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWvkproc(/*x314*/);
        /*x315*/
        /*x316*//*/*
		/*! @brief Opaque monitor object.
		*
		*  Opaque monitor object.
		*
		*  @see @ref monitor_object
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x317*/
                /// <summary>
                /// Opaque monitor object.
                /// </summary>
        /*x321*/
        /*x322*//*/*
		/*! @brief Opaque window object.
		*
		*  Opaque window object.
		*
		*  @see @ref window_object
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                /*x323*/
                /// <summary>
                /// Opaque window object.
                /// </summary>
        /*x327*/
        /*x328*//*/*
		/*! @brief Opaque cursor object.
		*
		*  Opaque cursor object.
		*
		*  @see @ref cursor_object
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup input**/
                /*x329*/
                /// <summary>
                /// Opaque cursor object.
                /// </summary>
        /*x333*//*/*
		/*! @brief The function signature for error callbacks.
		*
		*  This is the function signature for error callback functions.
		*
		*  @param[in] error An [error code](@ref errors).
		*  @param[in] description A UTF-8 encoded string describing the error.
		*
		*  @sa @ref error_handling
		*  @sa @ref glfwSetErrorCallback
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup init**/
                /*x334*/
                /// <summary>
                /// The function signature for error callbacks.
                /// </summary>
                /// <param name="error">An [error code](@ref errors).</param>
                /// <param name="description">A UTF-8 encoded string describing the error.</param>
        /*x340*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWerrorfun(int error, [MarshalAs(UnmanagedType.LPStr)] string description/*x341*/);
        /*x342*//*/*
		/*! @brief The function signature for window position callbacks.
		*
		*  This is the function signature for window position callback functions.
		*
		*  @param[in] window The window that was moved.
		*  @param[in] xpos The new x-coordinate, in screen coordinates, of the
		*  upper-left corner of the content area of the window.
		*  @param[in] ypos The new y-coordinate, in screen coordinates, of the
		*  upper-left corner of the content area of the window.
		*
		*  @sa @ref window_pos
		*  @sa @ref glfwSetWindowPosCallback
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                /*x343*/
                /// <summary>
                /// The function signature for window position callbacks.
                /// </summary>
                /// <param name="window">The window that was moved.</param>
                /// <param name="xpos">The new x-coordinate, in screen coordinates, of the</param>
                /// <param name="ypos">The new y-coordinate, in screen coordinates, of the</param>
        /*x350*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWwindowposfun(IntPtr/*GLFWwindow*/ window, int xpos, int ypos/*x351*/);
        /*x352*//*/*
		/*! @brief The function signature for window resize callbacks.
		*
		*  This is the function signature for window size callback functions.
		*
		*  @param[in] window The window that was resized.
		*  @param[in] width The new width, in screen coordinates, of the window.
		*  @param[in] height The new height, in screen coordinates, of the window.
		*
		*  @sa @ref window_size
		*  @sa @ref glfwSetWindowSizeCallback
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup window**/
                /*x353*/
                /// <summary>
                /// The function signature for window resize callbacks.
                /// </summary>
                /// <param name="window">The window that was resized.</param>
                /// <param name="width">The new width, in screen coordinates, of the window.</param>
                /// <param name="height">The new height, in screen coordinates, of the window.</param>
        /*x360*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWwindowsizefun(IntPtr/*GLFWwindow*/ window, int width, int height/*x361*/);
        /*x362*//*/*
		/*! @brief The function signature for window close callbacks.
		*
		*  This is the function signature for window close callback functions.
		*
		*  @param[in] window The window that the user attempted to close.
		*
		*  @sa @ref window_close
		*  @sa @ref glfwSetWindowCloseCallback
		*
		*  @since Added in version 2.5.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup window**/
                /*x363*/
                /// <summary>
                /// The function signature for window close callbacks.
                /// </summary>
                /// <param name="window">The window that the user attempted to close.</param>
        /*x368*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWwindowclosefun(IntPtr/*GLFWwindow*/ window/*x369*/);
        /*x370*//*/*
		/*! @brief The function signature for window content refresh callbacks.
		*
		*  This is the function signature for window refresh callback functions.
		*
		*  @param[in] window The window whose content needs to be refreshed.
		*
		*  @sa @ref window_refresh
		*  @sa @ref glfwSetWindowRefreshCallback
		*
		*  @since Added in version 2.5.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup window**/
                /*x371*/
                /// <summary>
                /// The function signature for window content refresh callbacks.
                /// </summary>
                /// <param name="window">The window whose content needs to be refreshed.</param>
        /*x376*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWwindowrefreshfun(IntPtr/*GLFWwindow*/ window/*x377*/);
        /*x378*//*/*
		/*! @brief The function signature for window focus/defocus callbacks.
		*
		*  This is the function signature for window focus callback functions.
		*
		*  @param[in] window The window that gained or lost input focus.
		*  @param[in] focused `GLFW_TRUE` if the window was given input focus, or
		*  `GLFW_FALSE` if it lost it.
		*
		*  @sa @ref window_focus
		*  @sa @ref glfwSetWindowFocusCallback
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                /*x379*/
                /// <summary>
                /// The function signature for window focus/defocus callbacks.
                /// </summary>
                /// <param name="window">The window that gained or lost input focus.</param>
                /// <param name="focused">`GLFW_TRUE` if the window was given input focus, or</param>
        /*x385*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWwindowfocusfun(IntPtr/*GLFWwindow*/ window, int focused/*x386*/);
        /*x387*//*/*
		/*! @brief The function signature for window iconify/restore callbacks.
		*
		*  This is the function signature for window iconify/restore callback
		*  functions.
		*
		*  @param[in] window The window that was iconified or restored.
		*  @param[in] iconified `GLFW_TRUE` if the window was iconified, or
		*  `GLFW_FALSE` if it was restored.
		*
		*  @sa @ref window_iconify
		*  @sa @ref glfwSetWindowIconifyCallback
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                /*x388*/
                /// <summary>
                /// The function signature for window iconify/restore callbacks.
                /// </summary>
                /// <param name="window">The window that was iconified or restored.</param>
                /// <param name="iconified">`GLFW_TRUE` if the window was iconified, or</param>
        /*x394*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWwindowiconifyfun(IntPtr/*GLFWwindow*/ window, int iconified/*x395*/);
        /*x396*//*/*
		/*! @brief The function signature for window maximize/restore callbacks.
		*
		*  This is the function signature for window maximize/restore callback
		*  functions.
		*
		*  @param[in] window The window that was maximized or restored.
		*  @param[in] iconified `GLFW_TRUE` if the window was maximized, or
		*  `GLFW_FALSE` if it was restored.
		*
		*  @sa @ref window_maximize
		*  @sa glfwSetWindowMaximizeCallback
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup window**/
                /*x397*/
                /// <summary>
                /// The function signature for window maximize/restore callbacks.
                /// </summary>
                /// <param name="window">The window that was maximized or restored.</param>
                /// <param name="iconified">`GLFW_TRUE` if the window was maximized, or</param>
        /*x403*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWwindowmaximizefun(IntPtr/*GLFWwindow*/ window, int iconified/*x404*/);
        /*x405*//*/*
		/*! @brief The function signature for framebuffer resize callbacks.
		*
		*  This is the function signature for framebuffer resize callback
		*  functions.
		*
		*  @param[in] window The window whose framebuffer was resized.
		*  @param[in] width The new width, in pixels, of the framebuffer.
		*  @param[in] height The new height, in pixels, of the framebuffer.
		*
		*  @sa @ref window_fbsize
		*  @sa @ref glfwSetFramebufferSizeCallback
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                /*x406*/
                /// <summary>
                /// The function signature for framebuffer resize callbacks.
                /// </summary>
                /// <param name="window">The window whose framebuffer was resized.</param>
                /// <param name="width">The new width, in pixels, of the framebuffer.</param>
                /// <param name="height">The new height, in pixels, of the framebuffer.</param>
        /*x413*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWframebuffersizefun(IntPtr/*GLFWwindow*/ window, int width, int height/*x414*/);
        /*x415*//*/*
		/*! @brief The function signature for window content scale callbacks.
		*
		*  This is the function signature for window content scale callback
		*  functions.
		*
		*  @param[in] window The window whose content scale changed.
		*  @param[in] xscale The new x-axis content scale of the window.
		*  @param[in] yscale The new y-axis content scale of the window.
		*
		*  @sa @ref window_scale
		*  @sa @ref glfwSetWindowContentScaleCallback
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup window**/
                /*x416*/
                /// <summary>
                /// The function signature for window content scale callbacks.
                /// </summary>
                /// <param name="window">The window whose content scale changed.</param>
                /// <param name="xscale">The new x-axis content scale of the window.</param>
                /// <param name="yscale">The new y-axis content scale of the window.</param>
        /*x423*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWwindowcontentscalefun(IntPtr/*GLFWwindow*/ window, float xscale, float yscale/*x424*/);
        /*x425*//*/*
		/*! @brief The function signature for mouse button callbacks.
		*
		*  This is the function signature for mouse button callback functions.
		*
		*  @param[in] window The window that received the event.
		*  @param[in] button The [mouse button](@ref buttons) that was pressed or
		*  released.
		*  @param[in] action One of `GLFW_PRESS` or `GLFW_RELEASE`.
		*  @param[in] mods Bit field describing which [modifier keys](@ref mods) were
		*  held down.
		*
		*  @sa @ref input_mouse_button
		*  @sa @ref glfwSetMouseButtonCallback
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle and modifier mask parameters.
		*
		*  @ingroup input**/
                /*x426*/
                /// <summary>
                /// The function signature for mouse button callbacks.
                /// </summary>
                /// <param name="window">The window that received the event.</param>
                /// <param name="button">The [mouse button](@ref buttons) that was pressed or</param>
                /// <param name="action">One of `GLFW_PRESS` or `GLFW_RELEASE`.</param>
                /// <param name="mods">Bit field describing which [modifier keys](@ref mods) were</param>
        /*x434*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWmousebuttonfun(IntPtr/*GLFWwindow*/ window, int button, int action, int mods/*x435*/);
        /*x436*//*/*
		/*! @brief The function signature for cursor position callbacks.
		*
		*  This is the function signature for cursor position callback functions.
		*
		*  @param[in] window The window that received the event.
		*  @param[in] xpos The new cursor x-coordinate, relative to the left edge of
		*  the content area.
		*  @param[in] ypos The new cursor y-coordinate, relative to the top edge of the
		*  content area.
		*
		*  @sa @ref cursor_pos
		*  @sa @ref glfwSetCursorPosCallback
		*
		*  @since Added in version 3.0.  Replaces `GLFWmouseposfun`.
		*
		*  @ingroup input**/
                /*x437*/
                /// <summary>
                /// The function signature for cursor position callbacks.
                /// </summary>
                /// <param name="window">The window that received the event.</param>
                /// <param name="xpos">The new cursor x-coordinate, relative to the left edge of</param>
                /// <param name="ypos">The new cursor y-coordinate, relative to the top edge of the</param>
        /*x444*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWcursorposfun(IntPtr/*GLFWwindow*/ window, double xpos, double ypos/*x445*/);
        /*x446*//*/*
		/*! @brief The function signature for cursor enter/leave callbacks.
		*
		*  This is the function signature for cursor enter/leave callback functions.
		*
		*  @param[in] window The window that received the event.
		*  @param[in] entered `GLFW_TRUE` if the cursor entered the window's content
		*  area, or `GLFW_FALSE` if it left it.
		*
		*  @sa @ref cursor_enter
		*  @sa @ref glfwSetCursorEnterCallback
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup input**/
                /*x447*/
                /// <summary>
                /// The function signature for cursor enter/leave callbacks.
                /// </summary>
                /// <param name="window">The window that received the event.</param>
                /// <param name="entered">`GLFW_TRUE` if the cursor entered the window's content</param>
        /*x453*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWcursorenterfun(IntPtr/*GLFWwindow*/ window, int entered/*x454*/);
        /*x455*//*/*
		/*! @brief The function signature for scroll callbacks.
		*
		*  This is the function signature for scroll callback functions.
		*
		*  @param[in] window The window that received the event.
		*  @param[in] xoffset The scroll offset along the x-axis.
		*  @param[in] yoffset The scroll offset along the y-axis.
		*
		*  @sa @ref scrolling
		*  @sa @ref glfwSetScrollCallback
		*
		*  @since Added in version 3.0.  Replaces `GLFWmousewheelfun`.
		*
		*  @ingroup input**/
                /*x456*/
                /// <summary>
                /// The function signature for scroll callbacks.
                /// </summary>
                /// <param name="window">The window that received the event.</param>
                /// <param name="xoffset">The scroll offset along the x-axis.</param>
                /// <param name="yoffset">The scroll offset along the y-axis.</param>
        /*x463*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWscrollfun(IntPtr/*GLFWwindow*/ window, double xoffset, double yoffset/*x464*/);
        /*x465*//*/*
		/*! @brief The function signature for keyboard key callbacks.
		*
		*  This is the function signature for keyboard key callback functions.
		*
		*  @param[in] window The window that received the event.
		*  @param[in] key The [keyboard key](@ref keys) that was pressed or released.
		*  @param[in] scancode The system-specific scancode of the key.
		*  @param[in] action `GLFW_PRESS`, `GLFW_RELEASE` or `GLFW_REPEAT`.
		*  @param[in] mods Bit field describing which [modifier keys](@ref mods) were
		*  held down.
		*
		*  @sa @ref input_key
		*  @sa @ref glfwSetKeyCallback
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle, scancode and modifier mask parameters.
		*
		*  @ingroup input**/
                /*x466*/
                /// <summary>
                /// The function signature for keyboard key callbacks.
                /// </summary>
                /// <param name="window">The window that received the event.</param>
                /// <param name="key">The [keyboard key](@ref keys) that was pressed or released.</param>
                /// <param name="scancode">The system-specific scancode of the key.</param>
                /// <param name="action">`GLFW_PRESS`, `GLFW_RELEASE` or `GLFW_REPEAT`.</param>
                /// <param name="mods">Bit field describing which [modifier keys](@ref mods) were</param>
        /*x475*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWkeyfun(IntPtr/*GLFWwindow*/ window, int key, int scancode, int action, int mods/*x476*/);
        /*x477*//*/*
		/*! @brief The function signature for Unicode character callbacks.
		*
		*  This is the function signature for Unicode character callback functions.
		*
		*  @param[in] window The window that received the event.
		*  @param[in] codepoint The Unicode code point of the character.
		*
		*  @sa @ref input_char
		*  @sa @ref glfwSetCharCallback
		*
		*  @since Added in version 2.4.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup input**/
                /*x478*/
                /// <summary>
                /// The function signature for Unicode character callbacks.
                /// </summary>
                /// <param name="window">The window that received the event.</param>
                /// <param name="codepoint">The Unicode code point of the character.</param>
        /*x484*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWcharfun(IntPtr/*GLFWwindow*/ window, uint codepoint/*x485*/);
        /*x486*//*/*
		/*! @brief The function signature for Unicode character with modifiers
		*  callbacks.
		*
		*  This is the function signature for Unicode character with modifiers callback
		*  functions.  It is called for each input character, regardless of what
		*  modifier keys are held down.
		*
		*  @param[in] window The window that received the event.
		*  @param[in] codepoint The Unicode code point of the character.
		*  @param[in] mods Bit field describing which [modifier keys](@ref mods) were
		*  held down.
		*
		*  @sa @ref input_char
		*  @sa @ref glfwSetCharModsCallback
		*
		*  @deprecated Scheduled for removal in version 4.0.
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup input**/
                /*x487*/
                /// <summary>
                /// The function signature for Unicode character with modifiers
                /// </summary>
                /// <param name="window">The window that received the event.</param>
                /// <param name="codepoint">The Unicode code point of the character.</param>
                /// <param name="mods">Bit field describing which [modifier keys](@ref mods) were</param>
        /*x494*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWcharmodsfun(IntPtr/*GLFWwindow*/ window, uint codepoint, int mods/*x495*/);
        /*x496*//*/*
		/*! @brief The function signature for file drop callbacks.
		*
		*  This is the function signature for file drop callbacks.
		*
		*  @param[in] window The window that received the event.
		*  @param[in] count The number of dropped files.
		*  @param[in] paths The UTF-8 encoded file and/or directory path names.
		*
		*  @sa @ref path_drop
		*  @sa @ref glfwSetDropCallback
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup input**/
                /*x497*/
                /// <summary>
                /// The function signature for file drop callbacks.
                /// </summary>
                /// <param name="window">The window that received the event.</param>
                /// <param name="count">The number of dropped files.</param>
                /// <param name="paths">The UTF-8 encoded file and/or directory path names.</param>
        /*x504*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWdropfun(IntPtr/*GLFWwindow*/ window, int count, out IntPtr paths/*x505*/);
        /*x506*//*/*
		/*! @brief The function signature for monitor configuration callbacks.
		*
		*  This is the function signature for monitor configuration callback functions.
		*
		*  @param[in] monitor The monitor that was connected or disconnected.
		*  @param[in] event One of `GLFW_CONNECTED` or `GLFW_DISCONNECTED`.  Remaining
		*  values reserved for future use.
		*
		*  @sa @ref monitor_event
		*  @sa @ref glfwSetMonitorCallback
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x507*/
                /// <summary>
                /// The function signature for monitor configuration callbacks.
                /// </summary>
                /// <param name="monitor">The monitor that was connected or disconnected.</param>
                /// <param name="event">One of `GLFW_CONNECTED` or `GLFW_DISCONNECTED`.  Remaining</param>
        /*x513*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWmonitorfun(IntPtr/*GLFWmonitor*/ monitor, int event0/*x514*/);
        /*x515*//*/*
		/*! @brief The function signature for joystick configuration callbacks.
		*
		*  This is the function signature for joystick configuration callback
		*  functions.
		*
		*  @param[in] jid The joystick that was connected or disconnected.
		*  @param[in] event One of `GLFW_CONNECTED` or `GLFW_DISCONNECTED`.  Remaining
		*  values reserved for future use.
		*
		*  @sa @ref joystick_event
		*  @sa @ref glfwSetJoystickCallback
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup input**/
                /*x516*/
                /// <summary>
                /// The function signature for joystick configuration callbacks.
                /// </summary>
                /// <param name="jid">The joystick that was connected or disconnected.</param>
                /// <param name="event">One of `GLFW_CONNECTED` or `GLFW_DISCONNECTED`.  Remaining</param>
        /*x522*/
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void GLFWjoystickfun(int jid, int event0/*x523*/);
        /*x524*/
        /*x525*//*/*
		/*! @brief Video mode type.
		*
		*  This describes a single video mode.
		*
		*  @sa @ref monitor_modes
		*  @sa @ref glfwGetVideoMode
		*  @sa @ref glfwGetVideoModes
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added refresh rate member.
		*
		*  @ingroup monitor**/
                /*x526*/
                /// <summary>
                /// Video mode type.
                /// </summary>
        /*x530*/
        /*x531*//*/*
		/*! @brief Gamma ramp.
		*
		*  This describes the gamma ramp for a monitor.
		*
		*  @sa @ref monitor_gamma
		*  @sa @ref glfwGetGammaRamp
		*  @sa @ref glfwSetGammaRamp
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x532*/
                /// <summary>
                /// Gamma ramp.
                /// </summary>
        /*x536*/
        /*x537*//*/*
		/*! @brief Image data.
		*
		*  This describes a single 2D image.  See the documentation for each related
		*  function what the expected pixel format is.
		*
		*  @sa @ref cursor_custom
		*  @sa @ref window_icon
		*
		*  @since Added in version 2.1.
		*  @glfw3 Removed format and bytes-per-pixel members.
		*
		*  @ingroup window**/
                /*x538*/
                /// <summary>
                /// Image data.
                /// </summary>
        /*x542*/
        /*x543*//*/*
		/*! @brief Gamepad input state
		*
		*  This describes the input state of a gamepad.
		*
		*  @sa @ref gamepad
		*  @sa @ref glfwGetGamepadState
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                /*x544*/
                /// <summary>
                /// Gamepad input state
                /// </summary>
        /*x548*//*/*
		/*************************************************************************
		* GLFW API functions**************************************************************************/
                /*x549*//*/*
                /*! @brief Initializes the GLFW library.
                *
                *  This function initializes the GLFW library.  Before most GLFW functions can
                *  be used, GLFW must be initialized, and before an application terminates GLFW
                *  should be terminated in order to free any resources allocated during or
                *  after initialization.
                *
                *  If this function fails, it calls @ref glfwTerminate before returning.  If it
                *  succeeds, you should call @ref glfwTerminate before the application exits.
                *
                *  Additional calls to this function after successful initialization but before
                *  termination will return `GLFW_TRUE` immediately.
                *
                *  @return `GLFW_TRUE` if successful, or `GLFW_FALSE` if an
                *  [error](@ref error_handling) occurred.
                *
                *  @errors Possible errors include @ref GLFW_PLATFORM_ERROR.
                *
                *  @remark @macos This function will change the current directory of the
                *  application to the `Contents/Resources` subdirectory of the application's
                *  bundle, if present.  This can be disabled with the @ref
                *  GLFW_COCOA_CHDIR_RESOURCES init hint.
                *
                *  @thread_safety This function must only be called from the main thread.
                *
                *  @sa @ref intro_init
                *  @sa @ref glfwTerminate
                *
                *  @since Added in version 1.0.
                *
                *  @ingroup init**/
                        /*x550*/
                        /// <summary>
                        /// Initializes the GLFW library.
                        /// </summary>
                        /// <returns>`GLFW_TRUE` if successful, or `GLFW_FALSE` if an</returns>
        /*x555*/
        [DllImport(LIB)]
        public static extern int glfwInit(/*x556*/);
        /*x557*/
        /*x558*/
        /*x559*//*/*
		/*! @brief Terminates the GLFW library.
		*
		*  This function destroys all remaining windows and cursors, restores any
		*  modified gamma ramps and frees any other allocated resources.  Once this
		*  function is called, you must again call @ref glfwInit successfully before
		*  you will be able to use most GLFW functions.
		*
		*  If GLFW has been successfully initialized, this function should be called
		*  before the application exits.  If initialization fails, there is no need to
		*  call this function, as it is called by @ref glfwInit before it returns
		*  failure.
		*
		*  @errors Possible errors include @ref GLFW_PLATFORM_ERROR.
		*
		*  @remark This function may be called before @ref glfwInit.
		*
		*  @warning The contexts of any remaining windows must not be current on any
		*  other thread when this function is called.
		*
		*  @reentrancy This function must not be called from a callback.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref intro_init
		*  @sa @ref glfwInit
		*
		*  @since Added in version 1.0.
		*
		*  @ingroup init**/
                /*x560*/
                /// <summary>
                /// Terminates the GLFW library.
                /// </summary>
        /*x564*/
        [DllImport(LIB)]
        public static extern void glfwTerminate(/*x565*/);
        /*x566*/
        /*x567*/
        /*x568*//*/*
		/*! @brief Sets the specified init hint to the desired value.
		*
		*  This function sets hints for the next initialization of GLFW.
		*
		*  The values you set hints to are never reset by GLFW, but they only take
		*  effect during initialization.  Once GLFW has been initialized, any values
		*  you set will be ignored until the library is terminated and initialized
		*  again.
		*
		*  Some hints are platform specific.  These may be set on any platform but they
		*  will only affect their specific platform.  Other platforms will ignore them.
		*  Setting these hints requires no platform specific headers or functions.
		*
		*  @param[in] hint The [init hint](@ref init_hints) to set.
		*  @param[in] value The new value of the init hint.
		*
		*  @errors Possible errors include @ref GLFW_INVALID_ENUM and @ref
		*  GLFW_INVALID_VALUE.
		*
		*  @remarks This function may be called before @ref glfwInit.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa init_hints
		*  @sa glfwInit
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup init**/
                /*x569*/
                /// <summary>
                /// Sets the specified init hint to the desired value.
                /// </summary>
                /// <param name="hint">The [init hint](@ref init_hints) to set.</param>
                /// <param name="value">The new value of the init hint.</param>
        /*x575*/
        [DllImport(LIB)]
        public static extern void glfwInitHint(int hint, int value/*x576*/);
        /*x577*/
        /*x578*/
        /*x579*//*/*
		/*! @brief Retrieves the version of the GLFW library.
		*
		*  This function retrieves the major, minor and revision numbers of the GLFW
		*  library.  It is intended for when you are using GLFW as a shared library and
		*  want to ensure that you are using the minimum required version.
		*
		*  Any or all of the version arguments may be `NULL`.
		*
		*  @param[out] major Where to store the major version number, or `NULL`.
		*  @param[out] minor Where to store the minor version number, or `NULL`.
		*  @param[out] rev Where to store the revision number, or `NULL`.
		*
		*  @errors None.
		*
		*  @remark This function may be called before @ref glfwInit.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref intro_version
		*  @sa @ref glfwGetVersionString
		*
		*  @since Added in version 1.0.
		*
		*  @ingroup init**/
                /*x580*/
                /// <summary>
                /// Retrieves the version of the GLFW library.
                /// </summary>
                /// <param name="major">Where to store the major version number, or `NULL`.</param>
                /// <param name="minor">Where to store the minor version number, or `NULL`.</param>
                /// <param name="rev">Where to store the revision number, or `NULL`.</param>
        /*x587*/
        [DllImport(LIB)]
        public static extern void glfwGetVersion(out int major, out int minor, out int rev/*x588*/);
        /*x589*/
        /*x590*/
        /*x591*//*/*
		/*! @brief Returns a string describing the compile-time configuration.
		*
		*  This function returns the compile-time generated
		*  [version string](@ref intro_version_string) of the GLFW library binary.  It
		*  describes the version, platform, compiler and any platform-specific
		*  compile-time options.  It should not be confused with the OpenGL or OpenGL
		*  ES version string, queried with `glGetString`.
		*
		*  __Do not use the version string__ to parse the GLFW library version.  The
		*  @ref glfwGetVersion function provides the version of the running library
		*  binary in numerical format.
		*
		*  @return The ASCII encoded GLFW version string.
		*
		*  @errors None.
		*
		*  @remark This function may be called before @ref glfwInit.
		*
		*  @pointer_lifetime The returned string is static and compile-time generated.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref intro_version
		*  @sa @ref glfwGetVersion
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup init**/
                /*x592*/
                /// <summary>
                /// Returns a string describing the compile-time configuration.
                /// </summary>
                /// <returns>The ASCII encoded GLFW version string.</returns>
        /*x597*/
        [DllImport(LIB)]
        public static extern IntPtr/*char*/ glfwGetVersionString(/*x598*/);
        /*x599*/
        /*x600*/
        /*x601*//*/*
		/*! @brief Returns and clears the last error for the calling thread.
		*
		*  This function returns and clears the [error code](@ref errors) of the last
		*  error that occurred on the calling thread, and optionally a UTF-8 encoded
		*  human-readable description of it.  If no error has occurred since the last
		*  call, it returns @ref GLFW_NO_ERROR (zero) and the description pointer is
		*  set to `NULL`.
		*
		*  @param[in] description Where to store the error description pointer, or `NULL`.
		*  @return The last error code for the calling thread, or @ref GLFW_NO_ERROR
		*  (zero).
		*
		*  @errors None.
		*
		*  @pointer_lifetime The returned string is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is guaranteed to be valid only until the
		*  next error occurs or the library is terminated.
		*
		*  @remark This function may be called before @ref glfwInit.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref error_handling
		*  @sa @ref glfwSetErrorCallback
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup init**/
                /*x602*/
                /// <summary>
                /// Returns and clears the last error for the calling thread.
                /// </summary>
                /// <param name="description">Where to store the error description pointer, or `NULL`.</param>
                /// <returns>The last error code for the calling thread, or @ref GLFW_NO_ERROR</returns>
        /*x608*/
        [DllImport(LIB)]
        public static extern int glfwGetError(out IntPtr description/*x609*/);
        /*x610*/
        /*x611*/
        /*x612*//*/*
		/*! @brief Sets the error callback.
		*
		*  This function sets the error callback, which is called with an error code
		*  and a human-readable description each time a GLFW error occurs.
		*
		*  The error code is set before the callback is called.  Calling @ref
		*  glfwGetError from the error callback will return the same value as the error
		*  code argument.
		*
		*  The error callback is called on the thread where the error occurred.  If you
		*  are using GLFW from multiple threads, your error callback needs to be
		*  written accordingly.
		*
		*  Because the description string may have been generated specifically for that
		*  error, it is not guaranteed to be valid after the callback has returned.  If
		*  you wish to use it after the callback returns, you need to make a copy.
		*
		*  Once set, the error callback remains set even after the library has been
		*  terminated.
		*
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set.
		*
		*  @errors None.
		*
		*  @remark This function may be called before @ref glfwInit.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref error_handling
		*  @sa @ref glfwGetError
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup init**/
                /*x613*/
                /// <summary>
                /// Sets the error callback.
                /// </summary>
                /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                /// <returns>The previously set callback, or `NULL` if no callback was set.</returns>
        /*x619*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWerrorfun*/ glfwSetErrorCallback(IntPtr /*GLFWerrorfun*/ cbfun/*x620*/);
        /*x621*/
        /*x622*/
        /*x623*//*/*
		/*! @brief Returns the currently connected monitors.
		*
		*  This function returns an array of handles for all currently connected
		*  monitors.  The primary monitor is always first in the returned array.  If no
		*  monitors were found, this function returns `NULL`.
		*
		*  @param[out] count Where to store the number of monitors in the returned
		*  array.  This is set to zero if an error occurred.
		*  @return An array of monitor handles, or `NULL` if no monitors were found or
		*  if an [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @pointer_lifetime The returned array is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is guaranteed to be valid only until the
		*  monitor configuration changes or the library is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_monitors
		*  @sa @ref monitor_event
		*  @sa @ref glfwGetPrimaryMonitor
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x624*/
                /// <summary>
                /// Returns the currently connected monitors.
                /// </summary>
                /// <param name="count">Where to store the number of monitors in the returned</param>
                /// <returns>An array of monitor handles, or `NULL` if no monitors were found or</returns>
        /*x630*/
        [DllImport(LIB)]
        public static extern IntPtr glfwGetMonitors(out int count/*x631*/);
        /*x632*/
        /*x633*/
        /*x634*//*/*
		/*! @brief Returns the primary monitor.
		*
		*  This function returns the primary monitor.  This is usually the monitor
		*  where elements like the task bar or global menu bar are located.
		*
		*  @return The primary monitor, or `NULL` if no monitors were found or if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @remark The primary monitor is always first in the array returned by @ref
		*  glfwGetMonitors.
		*
		*  @sa @ref monitor_monitors
		*  @sa @ref glfwGetMonitors
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x635*/
                /// <summary>
                /// Returns the primary monitor.
                /// </summary>
                /// <returns>The primary monitor, or `NULL` if no monitors were found or if an</returns>
        /*x640*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWmonitor*/ glfwGetPrimaryMonitor(/*x641*/);
        /*x642*/
        /*x643*/
        /*x644*//*/*
		/*! @brief Returns the position of the monitor's viewport on the virtual screen.
		*
		*  This function returns the position, in screen coordinates, of the upper-left
		*  corner of the specified monitor.
		*
		*  Any or all of the position arguments may be `NULL`.  If an error occurs, all
		*  non-`NULL` position arguments will be set to zero.
		*
		*  @param[in] monitor The monitor to query.
		*  @param[out] xpos Where to store the monitor x-coordinate, or `NULL`.
		*  @param[out] ypos Where to store the monitor y-coordinate, or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_properties
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x645*/
                /// <summary>
                /// Returns the position of the monitor's viewport on the virtual screen.
                /// </summary>
                /// <param name="monitor">The monitor to query.</param>
                /// <param name="xpos">Where to store the monitor x-coordinate, or `NULL`.</param>
                /// <param name="ypos">Where to store the monitor y-coordinate, or `NULL`.</param>
        /*x652*/
        [DllImport(LIB)]
        public static extern void glfwGetMonitorPos(IntPtr/*GLFWmonitor*/ monitor, out int xpos, out int ypos/*x653*/);
        /*x654*/
        /*x655*/
        /*x656*//*/*
		/*! @brief Retrives the work area of the monitor.
		*
		*  This function returns the position, in screen coordinates, of the upper-left
		*  corner of the work area of the specified monitor along with the work area
		*  size in screen coordinates. The work area is defined as the area of the
		*  monitor not occluded by the operating system task bar where present. If no
		*  task bar exists then the work area is the monitor resolution in screen
		*  coordinates.
		*
		*  Any or all of the position and size arguments may be `NULL`.  If an error
		*  occurs, all non-`NULL` position and size arguments will be set to zero.
		*
		*  @param[in] monitor The monitor to query.
		*  @param[out] xpos Where to store the monitor x-coordinate, or `NULL`.
		*  @param[out] ypos Where to store the monitor y-coordinate, or `NULL`.
		*  @param[out] width Where to store the monitor width, or `NULL`.
		*  @param[out] height Where to store the monitor height, or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_workarea
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup monitor**/
                /*x657*/
                /// <summary>
                /// Retrives the work area of the monitor.
                /// </summary>
                /// <param name="monitor">The monitor to query.</param>
                /// <param name="xpos">Where to store the monitor x-coordinate, or `NULL`.</param>
                /// <param name="ypos">Where to store the monitor y-coordinate, or `NULL`.</param>
                /// <param name="width">Where to store the monitor width, or `NULL`.</param>
                /// <param name="height">Where to store the monitor height, or `NULL`.</param>
        /*x666*/
        [DllImport(LIB)]
        public static extern void glfwGetMonitorWorkarea(IntPtr/*GLFWmonitor*/ monitor, out int xpos, out int ypos, out int width, out int height/*x667*/);
        /*x668*/
        /*x669*/
        /*x670*//*/*
		/*! @brief Returns the physical size of the monitor.
		*
		*  This function returns the size, in millimetres, of the display area of the
		*  specified monitor.
		*
		*  Some systems do not provide accurate monitor size information, either
		*  because the monitor
		*  [EDID](https://en.wikipedia.org/wiki/Extended_display_identification_data)
		*  data is incorrect or because the driver does not report it accurately.
		*
		*  Any or all of the size arguments may be `NULL`.  If an error occurs, all
		*  non-`NULL` size arguments will be set to zero.
		*
		*  @param[in] monitor The monitor to query.
		*  @param[out] widthMM Where to store the width, in millimetres, of the
		*  monitor's display area, or `NULL`.
		*  @param[out] heightMM Where to store the height, in millimetres, of the
		*  monitor's display area, or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @remark @win32 calculates the returned physical size from the
		*  current resolution and system DPI instead of querying the monitor EDID data.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_properties
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x671*/
                /// <summary>
                /// Returns the physical size of the monitor.
                /// </summary>
                /// <param name="monitor">The monitor to query.</param>
                /// <param name="widthMM">Where to store the width, in millimetres, of the</param>
                /// <param name="heightMM">Where to store the height, in millimetres, of the</param>
        /*x678*/
        [DllImport(LIB)]
        public static extern void glfwGetMonitorPhysicalSize(IntPtr/*GLFWmonitor*/ monitor, out int widthMM, out int heightMM/*x679*/);
        /*x680*/
        /*x681*/
        /*x682*//*/*
		/*! @brief Retrieves the content scale for the specified monitor.
		*
		*  This function retrieves the content scale for the specified monitor.  The
		*  content scale is the ratio between the current DPI and the platform's
		*  default DPI.  This is especially important for text and any UI elements.  If
		*  the pixel dimensions of your UI scaled by this look appropriate on your
		*  machine then it should appear at a reasonable size on other machines
		*  regardless of their DPI and scaling settings.  This relies on the system DPI
		*  and scaling settings being somewhat correct.
		*
		*  The content scale may depend on both the monitor resolution and pixel
		*  density and on user settings.  It may be very different from the raw DPI
		*  calculated from the physical size and current resolution.
		*
		*  @param[in] monitor The monitor to query.
		*  @param[out] xscale Where to store the x-axis content scale, or `NULL`.
		*  @param[out] yscale Where to store the y-axis content scale, or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_scale
		*  @sa @ref glfwGetWindowContentScale
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup monitor**/
                /*x683*/
                /// <summary>
                /// Retrieves the content scale for the specified monitor.
                /// </summary>
                /// <param name="monitor">The monitor to query.</param>
                /// <param name="xscale">Where to store the x-axis content scale, or `NULL`.</param>
                /// <param name="yscale">Where to store the y-axis content scale, or `NULL`.</param>
        /*x690*/
        [DllImport(LIB)]
        public static extern void glfwGetMonitorContentScale(IntPtr/*GLFWmonitor*/ monitor, out float xscale, out float yscale/*x691*/);
        /*x692*/
        /*x693*/
        /*x694*//*/*
		/*! @brief Returns the name of the specified monitor.
		*
		*  This function returns a human-readable name, encoded as UTF-8, of the
		*  specified monitor.  The name typically reflects the make and model of the
		*  monitor and is not guaranteed to be unique among the connected monitors.
		*
		*  @param[in] monitor The monitor to query.
		*  @return The UTF-8 encoded name of the monitor, or `NULL` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @pointer_lifetime The returned string is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the specified monitor is
		*  disconnected or the library is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_properties
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x695*/
                /// <summary>
                /// Returns the name of the specified monitor.
                /// </summary>
                /// <param name="monitor">The monitor to query.</param>
                /// <returns>The UTF-8 encoded name of the monitor, or `NULL` if an</returns>
        /*x701*/
        [DllImport(LIB)]
        public static extern IntPtr/*char*/ glfwGetMonitorName(IntPtr/*GLFWmonitor*/ monitor/*x702*/);
        /*x703*/
        /*x704*/
        /*x705*//*/*
		/*! @brief Sets the user pointer of the specified monitor.
		*
		*  This function sets the user-defined pointer of the specified monitor.  The
		*  current value is retained until the monitor is disconnected.  The initial
		*  value is `NULL`.
		*
		*  This function may be called from the monitor callback, even for a monitor
		*  that is being disconnected.
		*
		*  @param[in] monitor The monitor whose pointer to set.
		*  @param[in] pointer The new value.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.  Access is not
		*  synchronized.
		*
		*  @sa @ref monitor_userptr
		*  @sa @ref glfwGetMonitorUserPointer
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup monitor**/
                /*x706*/
                /// <summary>
                /// Sets the user pointer of the specified monitor.
                /// </summary>
                /// <param name="monitor">The monitor whose pointer to set.</param>
                /// <param name="pointer">The new value.</param>
        /*x712*/
        [DllImport(LIB)]
        public static extern void glfwSetMonitorUserPointer(IntPtr/*GLFWmonitor*/ monitor, IntPtr/*void*/ pointer/*x713*/);
        /*x714*/
        /*x715*/
        /*x716*//*/*
		/*! @brief Returns the user pointer of the specified monitor.
		*
		*  This function returns the current value of the user-defined pointer of the
		*  specified monitor.  The initial value is `NULL`.
		*
		*  This function may be called from the monitor callback, even for a monitor
		*  that is being disconnected.
		*
		*  @param[in] monitor The monitor whose pointer to return.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.  Access is not
		*  synchronized.
		*
		*  @sa @ref monitor_userptr
		*  @sa @ref glfwSetMonitorUserPointer
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup monitor**/
                /*x717*/
                /// <summary>
                /// Returns the user pointer of the specified monitor.
                /// </summary>
                /// <param name="monitor">The monitor whose pointer to return.</param>
        /*x722*/
        [DllImport(LIB)]
        public static extern IntPtr/*void*/ glfwGetMonitorUserPointer(IntPtr/*GLFWmonitor*/ monitor/*x723*/);
        /*x724*/
        /*x725*/
        /*x726*//*/*
		/*! @brief Sets the monitor configuration callback.
		*
		*  This function sets the monitor configuration callback, or removes the
		*  currently set callback.  This is called when a monitor is connected to or
		*  disconnected from the system.
		*
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_event
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x727*/
                /// <summary>
                /// Sets the monitor configuration callback.
                /// </summary>
                /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x733*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWmonitorfun*/ glfwSetMonitorCallback(IntPtr /*GLFWmonitorfun*/ cbfun/*x734*/);
        /*x735*/
        /*x736*/
        /*x737*//*/*
		/*! @brief Returns the available video modes for the specified monitor.
		*
		*  This function returns an array of all video modes supported by the specified
		*  monitor.  The returned array is sorted in ascending order, first by color
		*  bit depth (the sum of all channel depths) and then by resolution area (the
		*  product of width and height).
		*
		*  @param[in] monitor The monitor to query.
		*  @param[out] count Where to store the number of video modes in the returned
		*  array.  This is set to zero if an error occurred.
		*  @return An array of video modes, or `NULL` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The returned array is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the specified monitor is
		*  disconnected, this function is called again for that monitor or the library
		*  is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_modes
		*  @sa @ref glfwGetVideoMode
		*
		*  @since Added in version 1.0.
		*  @glfw3 Changed to return an array of modes for a specific monitor.
		*
		*  @ingroup monitor**/
                /*x738*/
                /// <summary>
                /// Returns the available video modes for the specified monitor.
                /// </summary>
                /// <param name="monitor">The monitor to query.</param>
                /// <param name="count">Where to store the number of video modes in the returned</param>
                /// <returns>An array of video modes, or `NULL` if an</returns>
        /*x745*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWvidmode*/ glfwGetVideoModes(IntPtr/*GLFWmonitor*/ monitor, out int count/*x746*/);
        /*x747*/
        /*x748*/
        /*x749*//*/*
		/*! @brief Returns the current mode of the specified monitor.
		*
		*  This function returns the current video mode of the specified monitor.  If
		*  you have created a full screen window for that monitor, the return value
		*  will depend on whether that window is iconified.
		*
		*  @param[in] monitor The monitor to query.
		*  @return The current mode of the monitor, or `NULL` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The returned array is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the specified monitor is
		*  disconnected or the library is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_modes
		*  @sa @ref glfwGetVideoModes
		*
		*  @since Added in version 3.0.  Replaces `glfwGetDesktopMode`.
		*
		*  @ingroup monitor**/
                /*x750*/
                /// <summary>
                /// Returns the current mode of the specified monitor.
                /// </summary>
                /// <param name="monitor">The monitor to query.</param>
                /// <returns>The current mode of the monitor, or `NULL` if an</returns>
        /*x756*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWvidmode*/ glfwGetVideoMode(IntPtr/*GLFWmonitor*/ monitor/*x757*/);
        /*x758*/
        /*x759*/
        /*x760*//*/*
		/*! @brief Generates a gamma ramp and sets it for the specified monitor.
		*
		*  This function generates an appropriately sized gamma ramp from the specified
		*  exponent and then calls @ref glfwSetGammaRamp with it.  The value must be
		*  a finite number greater than zero.
		*
		*  The software controlled gamma ramp is applied _in addition_ to the hardware
		*  gamma correction, which today is usually an approximation of sRGB gamma.
		*  This means that setting a perfectly linear ramp, or gamma 1.0, will produce
		*  the default (usually sRGB-like) behavior.
		*
		*  For gamma correct rendering with OpenGL or OpenGL ES, see the @ref
		*  GLFW_SRGB_CAPABLE hint.
		*
		*  @param[in] monitor The monitor whose gamma ramp to set.
		*  @param[in] gamma The desired exponent.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_VALUE and @ref GLFW_PLATFORM_ERROR.
		*
		*  @remark @wayland Gamma handling is a priviledged protocol, this function
		*  will thus never be implemented and emits @ref GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_gamma
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x761*/
                /// <summary>
                /// Generates a gamma ramp and sets it for the specified monitor.
                /// </summary>
                /// <param name="monitor">The monitor whose gamma ramp to set.</param>
                /// <param name="gamma">The desired exponent.</param>
        /*x767*/
        [DllImport(LIB)]
        public static extern void glfwSetGamma(IntPtr/*GLFWmonitor*/ monitor, float gamma/*x768*/);
        /*x769*/
        /*x770*/
        /*x771*//*/*
		/*! @brief Returns the current gamma ramp for the specified monitor.
		*
		*  This function returns the current gamma ramp of the specified monitor.
		*
		*  @param[in] monitor The monitor to query.
		*  @return The current gamma ramp, or `NULL` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark @wayland Gamma handling is a priviledged protocol, this function
		*  will thus never be implemented and emits @ref GLFW_PLATFORM_ERROR while
		*  returning `NULL`.
		*
		*  @pointer_lifetime The returned structure and its arrays are allocated and
		*  freed by GLFW.  You should not free them yourself.  They are valid until the
		*  specified monitor is disconnected, this function is called again for that
		*  monitor or the library is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_gamma
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x772*/
                /// <summary>
                /// Returns the current gamma ramp for the specified monitor.
                /// </summary>
                /// <param name="monitor">The monitor to query.</param>
                /// <returns>The current gamma ramp, or `NULL` if an</returns>
        /*x778*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWgammaramp*/ glfwGetGammaRamp(IntPtr/*GLFWmonitor*/ monitor/*x779*/);
        /*x780*/
        /*x781*/
        /*x782*//*/*
		/*! @brief Sets the current gamma ramp for the specified monitor.
		*
		*  This function sets the current gamma ramp for the specified monitor.  The
		*  original gamma ramp for that monitor is saved by GLFW the first time this
		*  function is called and is restored by @ref glfwTerminate.
		*
		*  The software controlled gamma ramp is applied _in addition_ to the hardware
		*  gamma correction, which today is usually an approximation of sRGB gamma.
		*  This means that setting a perfectly linear ramp, or gamma 1.0, will produce
		*  the default (usually sRGB-like) behavior.
		*
		*  For gamma correct rendering with OpenGL or OpenGL ES, see the @ref
		*  GLFW_SRGB_CAPABLE hint.
		*
		*  @param[in] monitor The monitor whose gamma ramp to set.
		*  @param[in] ramp The gamma ramp to use.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark The size of the specified gamma ramp should match the size of the
		*  current ramp for that monitor.
		*
		*  @remark @win32 The gamma ramp size must be 256.
		*
		*  @remark @wayland Gamma handling is a priviledged protocol, this function
		*  will thus never be implemented and emits @ref GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The specified gamma ramp is copied before this function
		*  returns.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref monitor_gamma
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup monitor**/
                /*x783*/
                /// <summary>
                /// Sets the current gamma ramp for the specified monitor.
                /// </summary>
                /// <param name="monitor">The monitor whose gamma ramp to set.</param>
                /// <param name="ramp">The gamma ramp to use.</param>
        /*x789*/
        [DllImport(LIB)]
        public static extern void glfwSetGammaRamp(IntPtr/*GLFWmonitor*/ monitor, IntPtr/*GLFWgammaramp*/ ramp/*x790*/);
        /*x791*/
        /*x792*/
        /*x793*//*/*
		/*! @brief Resets all window hints to their default values.
		*
		*  This function resets all window hints to their
		*  [default values](@ref window_hints_values).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_hints
		*  @sa @ref glfwWindowHint
		*  @sa @ref glfwWindowHintString
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                /*x794*/
                /// <summary>
                /// Resets all window hints to their default values.
                /// </summary>
        /*x798*/
        [DllImport(LIB)]
        public static extern void glfwDefaultWindowHints(/*x799*/);
        /*x800*/
        /*x801*/
        /*x802*//*/*
		/*! @brief Sets the specified window hint to the desired value.
		*
		*  This function sets hints for the next call to @ref glfwCreateWindow.  The
		*  hints, once set, retain their values until changed by a call to this
		*  function or @ref glfwDefaultWindowHints, or until the library is terminated.
		*
		*  Only integer value hints can be set with this function.  String value hints
		*  are set with @ref glfwWindowHintString.
		*
		*  This function does not check whether the specified hint values are valid.
		*  If you set hints to invalid values this will instead be reported by the next
		*  call to @ref glfwCreateWindow.
		*
		*  Some hints are platform specific.  These may be set on any platform but they
		*  will only affect their specific platform.  Other platforms will ignore them.
		*  Setting these hints requires no platform specific headers or functions.
		*
		*  @param[in] hint The [window hint](@ref window_hints) to set.
		*  @param[in] value The new value of the window hint.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_INVALID_ENUM.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_hints
		*  @sa @ref glfwWindowHintString
		*  @sa @ref glfwDefaultWindowHints
		*
		*  @since Added in version 3.0.  Replaces `glfwOpenWindowHint`.
		*
		*  @ingroup window**/
                /*x803*/
                /// <summary>
                /// Sets the specified window hint to the desired value.
                /// </summary>
                /// <param name="hint">The [window hint](@ref window_hints) to set.</param>
                /// <param name="value">The new value of the window hint.</param>
        /*x809*/
        [DllImport(LIB)]
        public static extern void glfwWindowHint(int hint, int value/*x810*/);
        /*x811*/
        /*x812*/
        /*x813*//*/*
		/*! @brief Sets the specified window hint to the desired value.
		*
		*  This function sets hints for the next call to @ref glfwCreateWindow.  The
		*  hints, once set, retain their values until changed by a call to this
		*  function or @ref glfwDefaultWindowHints, or until the library is terminated.
		*
		*  Only string type hints can be set with this function.  Integer value hints
		*  are set with @ref glfwWindowHint.
		*
		*  This function does not check whether the specified hint values are valid.
		*  If you set hints to invalid values this will instead be reported by the next
		*  call to @ref glfwCreateWindow.
		*
		*  Some hints are platform specific.  These may be set on any platform but they
		*  will only affect their specific platform.  Other platforms will ignore them.
		*  Setting these hints requires no platform specific headers or functions.
		*
		*  @param[in] hint The [window hint](@ref window_hints) to set.
		*  @param[in] value The new value of the window hint.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_INVALID_ENUM.
		*
		*  @pointer_lifetime The specified string is copied before this function
		*  returns.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_hints
		*  @sa @ref glfwWindowHint
		*  @sa @ref glfwDefaultWindowHints
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup window**/
                /*x814*/
                /// <summary>
                /// Sets the specified window hint to the desired value.
                /// </summary>
                /// <param name="hint">The [window hint](@ref window_hints) to set.</param>
                /// <param name="value">The new value of the window hint.</param>
        /*x820*/
        [DllImport(LIB)]
        public static extern unsafe void glfwWindowHintString(int hint, byte* value/*x821*/);
        /*x822*/
        /*x823*/
        /*x824*/
        [DllImport(LIB)]
        public static extern void glfwWindowHintString(int hint, [MarshalAs(UnmanagedType.LPStr)] string value/*x825*/);
        /*x826*/
        /*x827*/
        /*x828*//*/*
		/*! @brief Creates a window and its associated context.
		*
		*  This function creates a window and its associated OpenGL or OpenGL ES
		*  context.  Most of the options controlling how the window and its context
		*  should be created are specified with [window hints](@ref window_hints).
		*
		*  Successful creation does not change which context is current.  Before you
		*  can use the newly created context, you need to
		*  [make it current](@ref context_current).  For information about the `share`
		*  parameter, see @ref context_sharing.
		*
		*  The created window, framebuffer and context may differ from what you
		*  requested, as not all parameters and hints are
		*  [hard constraints](@ref window_hints_hard).  This includes the size of the
		*  window, especially for full screen windows.  To query the actual attributes
		*  of the created window, framebuffer and context, see @ref
		*  glfwGetWindowAttrib, @ref glfwGetWindowSize and @ref glfwGetFramebufferSize.
		*
		*  To create a full screen window, you need to specify the monitor the window
		*  will cover.  If no monitor is specified, the window will be windowed mode.
		*  Unless you have a way for the user to choose a specific monitor, it is
		*  recommended that you pick the primary monitor.  For more information on how
		*  to query connected monitors, see @ref monitor_monitors.
		*
		*  For full screen windows, the specified size becomes the resolution of the
		*  window's _desired video mode_.  As long as a full screen window is not
		*  iconified, the supported video mode most closely matching the desired video
		*  mode is set for the specified monitor.  For more information about full
		*  screen windows, including the creation of so called _windowed full screen_
		*  or _borderless full screen_ windows, see @ref window_windowed_full_screen.
		*
		*  Once you have created the window, you can switch it between windowed and
		*  full screen mode with @ref glfwSetWindowMonitor.  This will not affect its
		*  OpenGL or OpenGL ES context.
		*
		*  By default, newly created windows use the placement recommended by the
		*  window system.  To create the window at a specific position, make it
		*  initially invisible using the [GLFW_VISIBLE](@ref GLFW_VISIBLE_hint) window
		*  hint, set its [position](@ref window_pos) and then [show](@ref window_hide)
		*  it.
		*
		*  As long as at least one full screen window is not iconified, the screensaver
		*  is prohibited from starting.
		*
		*  Window systems put limits on window sizes.  Very large or very small window
		*  dimensions may be overridden by the window system on creation.  Check the
		*  actual [size](@ref window_size) after creation.
		*
		*  The [swap interval](@ref buffer_swap) is not set during window creation and
		*  the initial value may vary depending on driver settings and defaults.
		*
		*  @param[in] width The desired width, in screen coordinates, of the window.
		*  This must be greater than zero.
		*  @param[in] height The desired height, in screen coordinates, of the window.
		*  This must be greater than zero.
		*  @param[in] title The initial, UTF-8 encoded window title.
		*  @param[in] monitor The monitor to use for full screen mode, or `NULL` for
		*  windowed mode.
		*  @param[in] share The window whose context to share resources with, or `NULL`
		*  to not share resources.
		*  @return The handle of the created window, or `NULL` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM, @ref GLFW_INVALID_VALUE, @ref GLFW_API_UNAVAILABLE, @ref
		*  GLFW_VERSION_UNAVAILABLE, @ref GLFW_FORMAT_UNAVAILABLE and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark @win32 Window creation will fail if the Microsoft GDI software
		*  OpenGL implementation is the only one available.
		*
		*  @remark @win32 If the executable has an icon resource named `GLFW_ICON,` it
		*  will be set as the initial icon for the window.  If no such icon is present,
		*  the `IDI_APPLICATION` icon will be used instead.  To set a different icon,
		*  see @ref glfwSetWindowIcon.
		*
		*  @remark @win32 The context to share resources with must not be current on
		*  any other thread.
		*
		*  @remark @macos The OS only supports forward-compatible core profile contexts
		*  for OpenGL versions 3.2 and later.  Before creating an OpenGL context of
		*  version 3.2 or later you must set the
		*  [GLFW_OPENGL_FORWARD_COMPAT](@ref GLFW_OPENGL_FORWARD_COMPAT_hint) and
		*  [GLFW_OPENGL_PROFILE](@ref GLFW_OPENGL_PROFILE_hint) hints accordingly.
		*  OpenGL 3.0 and 3.1 contexts are not supported at all on macOS.
		*
		*  @remark @macos The GLFW window has no icon, as it is not a document
		*  window, but the dock icon will be the same as the application bundle's icon.
		*  For more information on bundles, see the
		*  [Bundle Programming Guide](https://developer.apple.com/library/mac/documentation/CoreFoundation/Conceptual/CFBundles/)
		*  in the Mac Developer Library.
		*
		*  @remark @macos The first time a window is created the menu bar is created.
		*  If GLFW finds a `MainMenu.nib` it is loaded and assumed to contain a menu
		*  bar.  Otherwise a minimal menu bar is created manually with common commands
		*  like Hide, Quit and About.  The About entry opens a minimal about dialog
		*  with information from the application's bundle.  Menu bar creation can be
		*  disabled entirely with the @ref GLFW_COCOA_MENUBAR init hint.
		*
		*  @remark @macos On OS X 10.10 and later the window frame will not be rendered
		*  at full resolution on Retina displays unless the
		*  [GLFW_COCOA_RETINA_FRAMEBUFFER](@ref GLFW_COCOA_RETINA_FRAMEBUFFER_hint)
		*  hint is `GLFW_TRUE` and the `NSHighResolutionCapable` key is enabled in the
		*  application bundle's `Info.plist`.  For more information, see
		*  [High Resolution Guidelines for OS X](https://developer.apple.com/library/mac/documentation/GraphicsAnimation/Conceptual/HighResolutionOSX/Explained/Explained.html)
		*  in the Mac Developer Library.  The GLFW test and example programs use
		*  a custom `Info.plist` template for this, which can be found as
		*  `CMake/MacOSXBundleInfo.plist.in` in the source tree.
		*
		*  @remark @macos When activating frame autosaving with
		*  [GLFW_COCOA_FRAME_NAME](@ref GLFW_COCOA_FRAME_NAME_hint), the specified
		*  window size and position may be overriden by previously saved values.
		*
		*  @remark @x11 Some window managers will not respect the placement of
		*  initially hidden windows.
		*
		*  @remark @x11 Due to the asynchronous nature of X11, it may take a moment for
		*  a window to reach its requested state.  This means you may not be able to
		*  query the final size, position or other attributes directly after window
		*  creation.
		*
		*  @remark @x11 The class part of the `WM_CLASS` window property will by
		*  default be set to the window title passed to this function.  The instance
		*  part will use the contents of the `RESOURCE_NAME` environment variable, if
		*  present and not empty, or fall back to the window title.  Set the
		*  [GLFW_X11_CLASS_NAME](@ref GLFW_X11_CLASS_NAME_hint) and
		*  [GLFW_X11_INSTANCE_NAME](@ref GLFW_X11_INSTANCE_NAME_hint) window hints to
		*  override this.
		*
		*  @remark @wayland Compositors should implement the xdg-decoration protocol
		*  for GLFW to decorate the window properly.  If this protocol isn't
		*  supported, or if the compositor prefers client-side decorations, a very
		*  simple fallback frame will be drawn using the wp_viewporter protocol.  A
		*  compositor can still emit close, maximize or fullscreen events, using for
		*  instance a keybind mechanism.  If neither of these protocols is supported,
		*  the window won't be decorated.
		*
		*  @remark @wayland A full screen window will not attempt to change the mode,
		*  no matter what the requested size or refresh rate.
		*
		*  @remark @wayland Screensaver inhibition requires the idle-inhibit protocol
		*  to be implemented in the user's compositor.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_creation
		*  @sa @ref glfwDestroyWindow
		*
		*  @since Added in version 3.0.  Replaces `glfwOpenWindow`.
		*
		*  @ingroup window**/
                /*x829*/
                /// <summary>
                /// Creates a window and its associated context.
                /// </summary>
                /// <param name="width">The desired width, in screen coordinates, of the window.</param>
                /// <param name="height">The desired height, in screen coordinates, of the window.</param>
                /// <param name="title">The initial, UTF-8 encoded window title.</param>
                /// <param name="monitor">The monitor to use for full screen mode, or `NULL` for</param>
                /// <param name="share">The window whose context to share resources with, or `NULL`</param>
                /// <returns>The handle of the created window, or `NULL` if an</returns>
        /*x839*/
        [DllImport(LIB)]
        public static extern unsafe IntPtr/*GLFWwindow*/ glfwCreateWindow(int width, int height, byte* title, IntPtr/*GLFWmonitor*/ monitor, IntPtr/*GLFWwindow*/ share/*x840*/);
        /*x841*/
        /*x842*/
        /*x843*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWwindow*/ glfwCreateWindow(int width, int height, [MarshalAs(UnmanagedType.LPStr)] string title, IntPtr/*GLFWmonitor*/ monitor, IntPtr/*GLFWwindow*/ share/*x844*/);
        /*x845*/
        /*x846*/
        /*x847*//*/*
		/*! @brief Destroys the specified window and its context.
		*
		*  This function destroys the specified window and its context.  On calling
		*  this function, no further callbacks will be called for that window.
		*
		*  If the context of the specified window is current on the main thread, it is
		*  detached before being destroyed.
		*
		*  @param[in] window The window to destroy.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @note The context of the specified window must not be current on any other
		*  thread when this function is called.
		*
		*  @reentrancy This function must not be called from a callback.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_creation
		*  @sa @ref glfwCreateWindow
		*
		*  @since Added in version 3.0.  Replaces `glfwCloseWindow`.
		*
		*  @ingroup window**/
                /*x848*/
                /// <summary>
                /// Destroys the specified window and its context.
                /// </summary>
                /// <param name="window">The window to destroy.</param>
        /*x853*/
        [DllImport(LIB)]
        public static extern void glfwDestroyWindow(IntPtr/*GLFWwindow*/ window/*x854*/);
        /*x855*/
        /*x856*/
        /*x857*//*/*
		/*! @brief Checks the close flag of the specified window.
		*
		*  This function returns the value of the close flag of the specified window.
		*
		*  @param[in] window The window to query.
		*  @return The value of the close flag.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.  Access is not
		*  synchronized.
		*
		*  @sa @ref window_close
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                /*x858*/
                /// <summary>
                /// Checks the close flag of the specified window.
                /// </summary>
                /// <param name="window">The window to query.</param>
                /// <returns>The value of the close flag.</returns>
        /*x864*/
        [DllImport(LIB)]
        public static extern int glfwWindowShouldClose(IntPtr/*GLFWwindow*/ window/*x865*/);
        /*x866*/
        /*x867*/
        /*x868*//*/*
		/*! @brief Sets the close flag of the specified window.
		*
		*  This function sets the value of the close flag of the specified window.
		*  This can be used to override the user's attempt to close the window, or
		*  to signal that it should be closed.
		*
		*  @param[in] window The window whose flag to change.
		*  @param[in] value The new value.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.  Access is not
		*  synchronized.
		*
		*  @sa @ref window_close
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                /*x869*/
                /// <summary>
                /// Sets the close flag of the specified window.
                /// </summary>
                /// <param name="window">The window whose flag to change.</param>
                /// <param name="value">The new value.</param>
        /*x875*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowShouldClose(IntPtr/*GLFWwindow*/ window, int value/*x876*/);
        /*x877*/
        /*x878*/
        /*x879*//*/*
		/*! @brief Sets the title of the specified window.
		*
		*  This function sets the window title, encoded as UTF-8, of the specified
		*  window.
		*
		*  @param[in] window The window whose title to change.
		*  @param[in] title The UTF-8 encoded window title.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark @macos The window title will not be updated until the next time you
		*  process events.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_title
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup window**/
                /*x880*/
                /// <summary>
                /// Sets the title of the specified window.
                /// </summary>
                /// <param name="window">The window whose title to change.</param>
                /// <param name="title">The UTF-8 encoded window title.</param>
        /*x886*/
        [DllImport(LIB)]
        public static extern unsafe void glfwSetWindowTitle(IntPtr/*GLFWwindow*/ window, byte* title/*x887*/);
        /*x888*/
        /*x889*/
        /*x890*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowTitle(IntPtr/*GLFWwindow*/ window, [MarshalAs(UnmanagedType.LPStr)] string title/*x891*/);
        /*x892*/
        /*x893*/
        /*x894*//*/*
		/*! @brief Sets the icon for the specified window.
		*
		*  This function sets the icon of the specified window.  If passed an array of
		*  candidate images, those of or closest to the sizes desired by the system are
		*  selected.  If no images are specified, the window reverts to its default
		*  icon.
		*
		*  The pixels are 32-bit, little-endian, non-premultiplied RGBA, i.e. eight
		*  bits per channel with the red channel first.  They are arranged canonically
		*  as packed sequential rows, starting from the top-left corner.
		*
		*  The desired image sizes varies depending on platform and system settings.
		*  The selected images will be rescaled as needed.  Good sizes include 16x16,
		*  32x32 and 48x48.
		*
		*  @param[in] window The window whose icon to set.
		*  @param[in] count The number of images in the specified array, or zero to
		*  revert to the default window icon.
		*  @param[in] images The images to create the icon from.  This is ignored if
		*  count is zero.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The specified image data is copied before this function
		*  returns.
		*
		*  @remark @macos The GLFW window has no icon, as it is not a document
		*  window, so this function does nothing.  The dock icon will be the same as
		*  the application bundle's icon.  For more information on bundles, see the
		*  [Bundle Programming Guide](https://developer.apple.com/library/mac/documentation/CoreFoundation/Conceptual/CFBundles/)
		*  in the Mac Developer Library.
		*
		*  @remark @wayland There is no existing protocol to change an icon, the
		*  window will thus inherit the one defined in the application's desktop file.
		*  This function always emits @ref GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_icon
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup window**/
                /*x895*/
                /// <summary>
                /// Sets the icon for the specified window.
                /// </summary>
                /// <param name="window">The window whose icon to set.</param>
                /// <param name="count">The number of images in the specified array, or zero to</param>
                /// <param name="images">The images to create the icon from.  This is ignored if</param>
        /*x902*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowIcon(IntPtr/*GLFWwindow*/ window, int count, IntPtr/*GLFWimage*/ images/*x903*/);
        /*x904*/
        /*x905*/
        /*x906*//*/*
		/*! @brief Retrieves the position of the content area of the specified window.
		*
		*  This function retrieves the position, in screen coordinates, of the
		*  upper-left corner of the content area of the specified window.
		*
		*  Any or all of the position arguments may be `NULL`.  If an error occurs, all
		*  non-`NULL` position arguments will be set to zero.
		*
		*  @param[in] window The window to query.
		*  @param[out] xpos Where to store the x-coordinate of the upper-left corner of
		*  the content area, or `NULL`.
		*  @param[out] ypos Where to store the y-coordinate of the upper-left corner of
		*  the content area, or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark @wayland There is no way for an application to retrieve the global
		*  position of its windows, this function will always emit @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_pos
		*  @sa @ref glfwSetWindowPos
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                /*x907*/
                /// <summary>
                /// Retrieves the position of the content area of the specified window.
                /// </summary>
                /// <param name="window">The window to query.</param>
                /// <param name="xpos">Where to store the x-coordinate of the upper-left corner of</param>
                /// <param name="ypos">Where to store the y-coordinate of the upper-left corner of</param>
        /*x914*/
        [DllImport(LIB)]
        public static extern void glfwGetWindowPos(IntPtr/*GLFWwindow*/ window, out int xpos, out int ypos/*x915*/);
        /*x916*/
        /*x917*/
        /*x918*//*/*
		/*! @brief Sets the position of the content area of the specified window.
		*
		*  This function sets the position, in screen coordinates, of the upper-left
		*  corner of the content area of the specified windowed mode window.  If the
		*  window is a full screen window, this function does nothing.
		*
		*  __Do not use this function__ to move an already visible window unless you
		*  have very good reasons for doing so, as it will confuse and annoy the user.
		*
		*  The window manager may put limits on what positions are allowed.  GLFW
		*  cannot and should not override these limits.
		*
		*  @param[in] window The window to query.
		*  @param[in] xpos The x-coordinate of the upper-left corner of the content area.
		*  @param[in] ypos The y-coordinate of the upper-left corner of the content area.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark @wayland There is no way for an application to set the global
		*  position of its windows, this function will always emit @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_pos
		*  @sa @ref glfwGetWindowPos
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup window**/
                /*x919*/
                /// <summary>
                /// Sets the position of the content area of the specified window.
                /// </summary>
                /// <param name="window">The window to query.</param>
                /// <param name="xpos">The x-coordinate of the upper-left corner of the content area.</param>
                /// <param name="ypos">The y-coordinate of the upper-left corner of the content area.</param>
        /*x926*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowPos(IntPtr/*GLFWwindow*/ window, int xpos, int ypos/*x927*/);
        /*x928*/
        /*x929*/
        /*x930*//*/*
		/*! @brief Retrieves the size of the content area of the specified window.
		*
		*  This function retrieves the size, in screen coordinates, of the content area
		*  of the specified window.  If you wish to retrieve the size of the
		*  framebuffer of the window in pixels, see @ref glfwGetFramebufferSize.
		*
		*  Any or all of the size arguments may be `NULL`.  If an error occurs, all
		*  non-`NULL` size arguments will be set to zero.
		*
		*  @param[in] window The window whose size to retrieve.
		*  @param[out] width Where to store the width, in screen coordinates, of the
		*  content area, or `NULL`.
		*  @param[out] height Where to store the height, in screen coordinates, of the
		*  content area, or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_size
		*  @sa @ref glfwSetWindowSize
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup window**/
                /*x931*/
                /// <summary>
                /// Retrieves the size of the content area of the specified window.
                /// </summary>
                /// <param name="window">The window whose size to retrieve.</param>
                /// <param name="width">Where to store the width, in screen coordinates, of the</param>
                /// <param name="height">Where to store the height, in screen coordinates, of the</param>
        /*x938*/
        [DllImport(LIB)]
        public static extern void glfwGetWindowSize(IntPtr/*GLFWwindow*/ window, out int width, out int height/*x939*/);
        /*x940*/
        /*x941*/
        /*x942*//*/*
		/*! @brief Sets the size limits of the specified window.
		*
		*  This function sets the size limits of the content area of the specified
		*  window.  If the window is full screen, the size limits only take effect
		*  once it is made windowed.  If the window is not resizable, this function
		*  does nothing.
		*
		*  The size limits are applied immediately to a windowed mode window and may
		*  cause it to be resized.
		*
		*  The maximum dimensions must be greater than or equal to the minimum
		*  dimensions and all must be greater than or equal to zero.
		*
		*  @param[in] window The window to set limits for.
		*  @param[in] minwidth The minimum width, in screen coordinates, of the content
		*  area, or `GLFW_DONT_CARE`.
		*  @param[in] minheight The minimum height, in screen coordinates, of the
		*  content area, or `GLFW_DONT_CARE`.
		*  @param[in] maxwidth The maximum width, in screen coordinates, of the content
		*  area, or `GLFW_DONT_CARE`.
		*  @param[in] maxheight The maximum height, in screen coordinates, of the
		*  content area, or `GLFW_DONT_CARE`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_VALUE and @ref GLFW_PLATFORM_ERROR.
		*
		*  @remark If you set size limits and an aspect ratio that conflict, the
		*  results are undefined.
		*
		*  @remark @wayland The size limits will not be applied until the window is
		*  actually resized, either by the user or by the compositor.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_sizelimits
		*  @sa @ref glfwSetWindowAspectRatio
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup window**/
                /*x943*/
                /// <summary>
                /// Sets the size limits of the specified window.
                /// </summary>
                /// <param name="window">The window to set limits for.</param>
                /// <param name="minwidth">The minimum width, in screen coordinates, of the content</param>
                /// <param name="minheight">The minimum height, in screen coordinates, of the</param>
                /// <param name="maxwidth">The maximum width, in screen coordinates, of the content</param>
                /// <param name="maxheight">The maximum height, in screen coordinates, of the</param>
        /*x952*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowSizeLimits(IntPtr/*GLFWwindow*/ window, int minwidth, int minheight, int maxwidth, int maxheight/*x953*/);
        /*x954*/
        /*x955*/
        /*x956*//*/*
		/*! @brief Sets the aspect ratio of the specified window.
		*
		*  This function sets the required aspect ratio of the content area of the
		*  specified window.  If the window is full screen, the aspect ratio only takes
		*  effect once it is made windowed.  If the window is not resizable, this
		*  function does nothing.
		*
		*  The aspect ratio is specified as a numerator and a denominator and both
		*  values must be greater than zero.  For example, the common 16:9 aspect ratio
		*  is specified as 16 and 9, respectively.
		*
		*  If the numerator and denominator is set to `GLFW_DONT_CARE` then the aspect
		*  ratio limit is disabled.
		*
		*  The aspect ratio is applied immediately to a windowed mode window and may
		*  cause it to be resized.
		*
		*  @param[in] window The window to set limits for.
		*  @param[in] numer The numerator of the desired aspect ratio, or
		*  `GLFW_DONT_CARE`.
		*  @param[in] denom The denominator of the desired aspect ratio, or
		*  `GLFW_DONT_CARE`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_VALUE and @ref GLFW_PLATFORM_ERROR.
		*
		*  @remark If you set size limits and an aspect ratio that conflict, the
		*  results are undefined.
		*
		*  @remark @wayland The aspect ratio will not be applied until the window is
		*  actually resized, either by the user or by the compositor.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_sizelimits
		*  @sa @ref glfwSetWindowSizeLimits
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup window**/
                /*x957*/
                /// <summary>
                /// Sets the aspect ratio of the specified window.
                /// </summary>
                /// <param name="window">The window to set limits for.</param>
                /// <param name="numer">The numerator of the desired aspect ratio, or</param>
                /// <param name="denom">The denominator of the desired aspect ratio, or</param>
        /*x964*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowAspectRatio(IntPtr/*GLFWwindow*/ window, int numer, int denom/*x965*/);
        /*x966*/
        /*x967*/
        /*x968*//*/*
		/*! @brief Sets the size of the content area of the specified window.
		*
		*  This function sets the size, in screen coordinates, of the content area of
		*  the specified window.
		*
		*  For full screen windows, this function updates the resolution of its desired
		*  video mode and switches to the video mode closest to it, without affecting
		*  the window's context.  As the context is unaffected, the bit depths of the
		*  framebuffer remain unchanged.
		*
		*  If you wish to update the refresh rate of the desired video mode in addition
		*  to its resolution, see @ref glfwSetWindowMonitor.
		*
		*  The window manager may put limits on what sizes are allowed.  GLFW cannot
		*  and should not override these limits.
		*
		*  @param[in] window The window to resize.
		*  @param[in] width The desired width, in screen coordinates, of the window
		*  content area.
		*  @param[in] height The desired height, in screen coordinates, of the window
		*  content area.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark @wayland A full screen window will not attempt to change the mode,
		*  no matter what the requested size.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_size
		*  @sa @ref glfwGetWindowSize
		*  @sa @ref glfwSetWindowMonitor
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup window**/
                /*x969*/
                /// <summary>
                /// Sets the size of the content area of the specified window.
                /// </summary>
                /// <param name="window">The window to resize.</param>
                /// <param name="width">The desired width, in screen coordinates, of the window</param>
                /// <param name="height">The desired height, in screen coordinates, of the window</param>
        /*x976*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowSize(IntPtr/*GLFWwindow*/ window, int width, int height/*x977*/);
        /*x978*/
        /*x979*/
        /*x980*//*/*
		/*! @brief Retrieves the size of the framebuffer of the specified window.
		*
		*  This function retrieves the size, in pixels, of the framebuffer of the
		*  specified window.  If you wish to retrieve the size of the window in screen
		*  coordinates, see @ref glfwGetWindowSize.
		*
		*  Any or all of the size arguments may be `NULL`.  If an error occurs, all
		*  non-`NULL` size arguments will be set to zero.
		*
		*  @param[in] window The window whose framebuffer to query.
		*  @param[out] width Where to store the width, in pixels, of the framebuffer,
		*  or `NULL`.
		*  @param[out] height Where to store the height, in pixels, of the framebuffer,
		*  or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_fbsize
		*  @sa @ref glfwSetFramebufferSizeCallback
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                /*x981*/
                /// <summary>
                /// Retrieves the size of the framebuffer of the specified window.
                /// </summary>
                /// <param name="window">The window whose framebuffer to query.</param>
                /// <param name="width">Where to store the width, in pixels, of the framebuffer,</param>
                /// <param name="height">Where to store the height, in pixels, of the framebuffer,</param>
        /*x988*/
        [DllImport(LIB)]
        public static extern void glfwGetFramebufferSize(IntPtr/*GLFWwindow*/ window, out int width, out int height/*x989*/);
        /*x990*/
        /*x991*/
        /*x992*//*/*
		/*! @brief Retrieves the size of the frame of the window.
		*
		*  This function retrieves the size, in screen coordinates, of each edge of the
		*  frame of the specified window.  This size includes the title bar, if the
		*  window has one.  The size of the frame may vary depending on the
		*  [window-related hints](@ref window_hints_wnd) used to create it.
		*
		*  Because this function retrieves the size of each window frame edge and not
		*  the offset along a particular coordinate axis, the retrieved values will
		*  always be zero or positive.
		*
		*  Any or all of the size arguments may be `NULL`.  If an error occurs, all
		*  non-`NULL` size arguments will be set to zero.
		*
		*  @param[in] window The window whose frame size to query.
		*  @param[out] left Where to store the size, in screen coordinates, of the left
		*  edge of the window frame, or `NULL`.
		*  @param[out] top Where to store the size, in screen coordinates, of the top
		*  edge of the window frame, or `NULL`.
		*  @param[out] right Where to store the size, in screen coordinates, of the
		*  right edge of the window frame, or `NULL`.
		*  @param[out] bottom Where to store the size, in screen coordinates, of the
		*  bottom edge of the window frame, or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_size
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup window**/
                /*x993*/
                /// <summary>
                /// Retrieves the size of the frame of the window.
                /// </summary>
                /// <param name="window">The window whose frame size to query.</param>
                /// <param name="left">Where to store the size, in screen coordinates, of the left</param>
                /// <param name="top">Where to store the size, in screen coordinates, of the top</param>
                /// <param name="right">Where to store the size, in screen coordinates, of the</param>
                /// <param name="bottom">Where to store the size, in screen coordinates, of the</param>
        /*x1002*/
        [DllImport(LIB)]
        public static extern void glfwGetWindowFrameSize(IntPtr/*GLFWwindow*/ window, out int left, out int top, out int right, out int bottom/*x1003*/);
        /*x1004*/
        /*x1005*/
        /*x1006*//*/*
		/*! @brief Retrieves the content scale for the specified window.
		*
		*  This function retrieves the content scale for the specified window.  The
		*  content scale is the ratio between the current DPI and the platform's
		*  default DPI.  This is especially important for text and any UI elements.  If
		*  the pixel dimensions of your UI scaled by this look appropriate on your
		*  machine then it should appear at a reasonable size on other machines
		*  regardless of their DPI and scaling settings.  This relies on the system DPI
		*  and scaling settings being somewhat correct.
		*
		*  On systems where each monitors can have its own content scale, the window
		*  content scale will depend on which monitor the system considers the window
		*  to be on.
		*
		*  @param[in] window The window to query.
		*  @param[out] xscale Where to store the x-axis content scale, or `NULL`.
		*  @param[out] yscale Where to store the y-axis content scale, or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_scale
		*  @sa @ref glfwSetWindowContentScaleCallback
		*  @sa @ref glfwGetMonitorContentScale
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup window**/
                 /*x1007*/
                 /// <summary>
                 /// Retrieves the content scale for the specified window.
                 /// </summary>
                 /// <param name="window">The window to query.</param>
                 /// <param name="xscale">Where to store the x-axis content scale, or `NULL`.</param>
                 /// <param name="yscale">Where to store the y-axis content scale, or `NULL`.</param>
        /*x1014*/
        [DllImport(LIB)]
        public static extern void glfwGetWindowContentScale(IntPtr/*GLFWwindow*/ window, out float xscale, out float yscale/*x1015*/);
        /*x1016*/
        /*x1017*/
        /*x1018*//*/*
		/*! @brief Returns the opacity of the whole window.
		*
		*  This function returns the opacity of the window, including any decorations.
		*
		*  The opacity (or alpha) value is a positive finite number between zero and
		*  one, where zero is fully transparent and one is fully opaque.  If the system
		*  does not support whole window transparency, this function always returns one.
		*
		*  The initial opacity value for newly created windows is one.
		*
		*  @param[in] window The window to query.
		*  @return The opacity value of the specified window.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_transparency
		*  @sa @ref glfwSetWindowOpacity
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup window**/
                 /*x1019*/
                 /// <summary>
                 /// Returns the opacity of the whole window.
                 /// </summary>
                 /// <param name="window">The window to query.</param>
                 /// <returns>The opacity value of the specified window.</returns>
        /*x1025*/
        [DllImport(LIB)]
        public static extern IntPtr/*float*/ glfwGetWindowOpacity(IntPtr/*GLFWwindow*/ window/*x1026*/);
        /*x1027*/
        /*x1028*/
        /*x1029*//*/*
		/*! @brief Sets the opacity of the whole window.
		*
		*  This function sets the opacity of the window, including any decorations.
		*
		*  The opacity (or alpha) value is a positive finite number between zero and
		*  one, where zero is fully transparent and one is fully opaque.
		*
		*  The initial opacity value for newly created windows is one.
		*
		*  A window created with framebuffer transparency may not use whole window
		*  transparency.  The results of doing this are undefined.
		*
		*  @param[in] window The window to set the opacity for.
		*  @param[in] opacity The desired opacity of the specified window.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_transparency
		*  @sa @ref glfwGetWindowOpacity
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup window**/
                 /*x1030*/
                 /// <summary>
                 /// Sets the opacity of the whole window.
                 /// </summary>
                 /// <param name="window">The window to set the opacity for.</param>
                 /// <param name="opacity">The desired opacity of the specified window.</param>
        /*x1036*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowOpacity(IntPtr/*GLFWwindow*/ window, float opacity/*x1037*/);
        /*x1038*/
        /*x1039*/
        /*x1040*//*/*
		/*! @brief Iconifies the specified window.
		*
		*  This function iconifies (minimizes) the specified window if it was
		*  previously restored.  If the window is already iconified, this function does
		*  nothing.
		*
		*  If the specified window is a full screen window, the original monitor
		*  resolution is restored until the window is restored.
		*
		*  @param[in] window The window to iconify.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark @wayland There is no concept of iconification in wl_shell, this
		*  function will emit @ref GLFW_PLATFORM_ERROR when using this deprecated
		*  protocol.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_iconify
		*  @sa @ref glfwRestoreWindow
		*  @sa @ref glfwMaximizeWindow
		*
		*  @since Added in version 2.1.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup window**/
                 /*x1041*/
                 /// <summary>
                 /// Iconifies the specified window.
                 /// </summary>
                 /// <param name="window">The window to iconify.</param>
        /*x1046*/
        [DllImport(LIB)]
        public static extern void glfwIconifyWindow(IntPtr/*GLFWwindow*/ window/*x1047*/);
        /*x1048*/
        /*x1049*/
        /*x1050*//*/*
		/*! @brief Restores the specified window.
		*
		*  This function restores the specified window if it was previously iconified
		*  (minimized) or maximized.  If the window is already restored, this function
		*  does nothing.
		*
		*  If the specified window is a full screen window, the resolution chosen for
		*  the window is restored on the selected monitor.
		*
		*  @param[in] window The window to restore.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_iconify
		*  @sa @ref glfwIconifyWindow
		*  @sa @ref glfwMaximizeWindow
		*
		*  @since Added in version 2.1.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup window**/
                 /*x1051*/
                 /// <summary>
                 /// Restores the specified window.
                 /// </summary>
                 /// <param name="window">The window to restore.</param>
        /*x1056*/
        [DllImport(LIB)]
        public static extern void glfwRestoreWindow(IntPtr/*GLFWwindow*/ window/*x1057*/);
        /*x1058*/
        /*x1059*/
        /*x1060*//*/*
		/*! @brief Maximizes the specified window.
		*
		*  This function maximizes the specified window if it was previously not
		*  maximized.  If the window is already maximized, this function does nothing.
		*
		*  If the specified window is a full screen window, this function does nothing.
		*
		*  @param[in] window The window to maximize.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @par Thread Safety
		*  This function may only be called from the main thread.
		*
		*  @sa @ref window_iconify
		*  @sa @ref glfwIconifyWindow
		*  @sa @ref glfwRestoreWindow
		*
		*  @since Added in GLFW 3.2.
		*
		*  @ingroup window**/
                 /*x1061*/
                 /// <summary>
                 /// Maximizes the specified window.
                 /// </summary>
                 /// <param name="window">The window to maximize.</param>
        /*x1066*/
        [DllImport(LIB)]
        public static extern void glfwMaximizeWindow(IntPtr/*GLFWwindow*/ window/*x1067*/);
        /*x1068*/
        /*x1069*/
        /*x1070*//*/*
		/*! @brief Makes the specified window visible.
		*
		*  This function makes the specified window visible if it was previously
		*  hidden.  If the window is already visible or is in full screen mode, this
		*  function does nothing.
		*
		*  By default, windowed mode windows are focused when shown
		*  Set the [GLFW_FOCUS_ON_SHOW](@ref GLFW_FOCUS_ON_SHOW_hint) window hint
		*  to change this behavior for all newly created windows, or change the
		*  behavior for an existing window with @ref glfwSetWindowAttrib.
		*
		*  @param[in] window The window to make visible.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_hide
		*  @sa @ref glfwHideWindow
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                 /*x1071*/
                 /// <summary>
                 /// Makes the specified window visible.
                 /// </summary>
                 /// <param name="window">The window to make visible.</param>
        /*x1076*/
        [DllImport(LIB)]
        public static extern void glfwShowWindow(IntPtr/*GLFWwindow*/ window/*x1077*/);
        /*x1078*/
        /*x1079*/
        /*x1080*//*/*
		/*! @brief Hides the specified window.
		*
		*  This function hides the specified window if it was previously visible.  If
		*  the window is already hidden or is in full screen mode, this function does
		*  nothing.
		*
		*  @param[in] window The window to hide.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_hide
		*  @sa @ref glfwShowWindow
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                 /*x1081*/
                 /// <summary>
                 /// Hides the specified window.
                 /// </summary>
                 /// <param name="window">The window to hide.</param>
        /*x1086*/
        [DllImport(LIB)]
        public static extern void glfwHideWindow(IntPtr/*GLFWwindow*/ window/*x1087*/);
        /*x1088*/
        /*x1089*/
        /*x1090*//*/*
		/*! @brief Brings the specified window to front and sets input focus.
		*
		*  This function brings the specified window to front and sets input focus.
		*  The window should already be visible and not iconified.
		*
		*  By default, both windowed and full screen mode windows are focused when
		*  initially created.  Set the [GLFW_FOCUSED](@ref GLFW_FOCUSED_hint) to
		*  disable this behavior.
		*
		*  Also by default, windowed mode windows are focused when shown
		*  with @ref glfwShowWindow. Set the
		*  [GLFW_FOCUS_ON_SHOW](@ref GLFW_FOCUS_ON_SHOW_hint) to disable this behavior.
		*
		*  __Do not use this function__ to steal focus from other applications unless
		*  you are certain that is what the user wants.  Focus stealing can be
		*  extremely disruptive.
		*
		*  For a less disruptive way of getting the user's attention, see
		*  [attention requests](@ref window_attention).
		*
		*  @param[in] window The window to give input focus.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark @wayland It is not possible for an application to bring its windows
		*  to front, this function will always emit @ref GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_focus
		*  @sa @ref window_attention
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup window**/
                 /*x1091*/
                 /// <summary>
                 /// Brings the specified window to front and sets input focus.
                 /// </summary>
                 /// <param name="window">The window to give input focus.</param>
        /*x1096*/
        [DllImport(LIB)]
        public static extern void glfwFocusWindow(IntPtr/*GLFWwindow*/ window/*x1097*/);
        /*x1098*/
        /*x1099*/
        /*x1100*//*/*
		/*! @brief Requests user attention to the specified window.
		*
		*  This function requests user attention to the specified window.  On
		*  platforms where this is not supported, attention is requested to the
		*  application as a whole.
		*
		*  Once the user has given attention, usually by focusing the window or
		*  application, the system will end the request automatically.
		*
		*  @param[in] window The window to request attention to.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark @macos Attention is requested to the application as a whole, not the
		*  specific window.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_attention
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup window**/
                 /*x1101*/
                 /// <summary>
                 /// Requests user attention to the specified window.
                 /// </summary>
                 /// <param name="window">The window to request attention to.</param>
        /*x1106*/
        [DllImport(LIB)]
        public static extern void glfwRequestWindowAttention(IntPtr/*GLFWwindow*/ window/*x1107*/);
        /*x1108*/
        /*x1109*/
        /*x1110*//*/*
		/*! @brief Returns the monitor that the window uses for full screen mode.
		*
		*  This function returns the handle of the monitor that the specified window is
		*  in full screen on.
		*
		*  @param[in] window The window to query.
		*  @return The monitor, or `NULL` if the window is in windowed mode or an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_monitor
		*  @sa @ref glfwSetWindowMonitor
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                 /*x1111*/
                 /// <summary>
                 /// Returns the monitor that the window uses for full screen mode.
                 /// </summary>
                 /// <param name="window">The window to query.</param>
                 /// <returns>The monitor, or `NULL` if the window is in windowed mode or an</returns>
        /*x1117*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWmonitor*/ glfwGetWindowMonitor(IntPtr/*GLFWwindow*/ window/*x1118*/);
        /*x1119*/
        /*x1120*/
        /*x1121*//*/*
		/*! @brief Sets the mode, monitor, video mode and placement of a window.
		*
		*  This function sets the monitor that the window uses for full screen mode or,
		*  if the monitor is `NULL`, makes it windowed mode.
		*
		*  When setting a monitor, this function updates the width, height and refresh
		*  rate of the desired video mode and switches to the video mode closest to it.
		*  The window position is ignored when setting a monitor.
		*
		*  When the monitor is `NULL`, the position, width and height are used to
		*  place the window content area.  The refresh rate is ignored when no monitor
		*  is specified.
		*
		*  If you only wish to update the resolution of a full screen window or the
		*  size of a windowed mode window, see @ref glfwSetWindowSize.
		*
		*  When a window transitions from full screen to windowed mode, this function
		*  restores any previous window settings such as whether it is decorated,
		*  floating, resizable, has size or aspect ratio limits, etc.
		*
		*  @param[in] window The window whose monitor, size or video mode to set.
		*  @param[in] monitor The desired monitor, or `NULL` to set windowed mode.
		*  @param[in] xpos The desired x-coordinate of the upper-left corner of the
		*  content area.
		*  @param[in] ypos The desired y-coordinate of the upper-left corner of the
		*  content area.
		*  @param[in] width The desired with, in screen coordinates, of the content
		*  area or video mode.
		*  @param[in] height The desired height, in screen coordinates, of the content
		*  area or video mode.
		*  @param[in] refreshRate The desired refresh rate, in Hz, of the video mode,
		*  or `GLFW_DONT_CARE`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark The OpenGL or OpenGL ES context will not be destroyed or otherwise
		*  affected by any resizing or mode switching, although you may need to update
		*  your viewport if the framebuffer size has changed.
		*
		*  @remark @wayland The desired window position is ignored, as there is no way
		*  for an application to set this property.
		*
		*  @remark @wayland Setting the window to full screen will not attempt to
		*  change the mode, no matter what the requested size or refresh rate.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_monitor
		*  @sa @ref window_full_screen
		*  @sa @ref glfwGetWindowMonitor
		*  @sa @ref glfwSetWindowSize
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup window**/
                 /*x1122*/
                 /// <summary>
                 /// Sets the mode, monitor, video mode and placement of a window.
                 /// </summary>
                 /// <param name="window">The window whose monitor, size or video mode to set.</param>
                 /// <param name="monitor">The desired monitor, or `NULL` to set windowed mode.</param>
                 /// <param name="xpos">The desired x-coordinate of the upper-left corner of the</param>
                 /// <param name="ypos">The desired y-coordinate of the upper-left corner of the</param>
                 /// <param name="width">The desired with, in screen coordinates, of the content</param>
                 /// <param name="height">The desired height, in screen coordinates, of the content</param>
                 /// <param name="refreshRate">The desired refresh rate, in Hz, of the video mode,</param>
        /*x1133*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowMonitor(IntPtr/*GLFWwindow*/ window, IntPtr/*GLFWmonitor*/ monitor, int xpos, int ypos, int width, int height, int refreshRate/*x1134*/);
        /*x1135*/
        /*x1136*/
        /*x1137*//*/*
		/*! @brief Returns an attribute of the specified window.
		*
		*  This function returns the value of an attribute of the specified window or
		*  its OpenGL or OpenGL ES context.
		*
		*  @param[in] window The window to query.
		*  @param[in] attrib The [window attribute](@ref window_attribs) whose value to
		*  return.
		*  @return The value of the attribute, or zero if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM and @ref GLFW_PLATFORM_ERROR.
		*
		*  @remark Framebuffer related hints are not window attributes.  See @ref
		*  window_attribs_fb for more information.
		*
		*  @remark Zero is a valid value for many window and context related
		*  attributes so you cannot use a return value of zero as an indication of
		*  errors.  However, this function should not fail as long as it is passed
		*  valid arguments and the library has been [initialized](@ref intro_init).
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_attribs
		*  @sa @ref glfwSetWindowAttrib
		*
		*  @since Added in version 3.0.  Replaces `glfwGetWindowParam` and
		*  `glfwGetGLVersion`.
		*
		*  @ingroup window**/
                 /*x1138*/
                 /// <summary>
                 /// Returns an attribute of the specified window.
                 /// </summary>
                 /// <param name="window">The window to query.</param>
                 /// <param name="attrib">The [window attribute](@ref window_attribs) whose value to</param>
                 /// <returns>The value of the attribute, or zero if an</returns>
        /*x1145*/
        [DllImport(LIB)]
        public static extern int glfwGetWindowAttrib(IntPtr/*GLFWwindow*/ window, int attrib/*x1146*/);
        /*x1147*/
        /*x1148*/
        /*x1149*//*/*
		/*! @brief Sets an attribute of the specified window.
		*
		*  This function sets the value of an attribute of the specified window.
		*
		*  The supported attributes are [GLFW_DECORATED](@ref GLFW_DECORATED_attrib),
		*  [GLFW_RESIZABLE](@ref GLFW_RESIZABLE_attrib),
		*  [GLFW_FLOATING](@ref GLFW_FLOATING_attrib),
		*  [GLFW_AUTO_ICONIFY](@ref GLFW_AUTO_ICONIFY_attrib) and
		*  [GLFW_FOCUS_ON_SHOW](@ref GLFW_FOCUS_ON_SHOW_attrib).
		*
		*  Some of these attributes are ignored for full screen windows.  The new
		*  value will take effect if the window is later made windowed.
		*
		*  Some of these attributes are ignored for windowed mode windows.  The new
		*  value will take effect if the window is later made full screen.
		*
		*  @param[in] window The window to set the attribute for.
		*  @param[in] attrib A supported window attribute.
		*  @param[in] value `GLFW_TRUE` or `GLFW_FALSE`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM, @ref GLFW_INVALID_VALUE and @ref GLFW_PLATFORM_ERROR.
		*
		*  @remark Calling @ref glfwGetWindowAttrib will always return the latest
		*  value, even if that value is ignored by the current mode of the window.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_attribs
		*  @sa @ref glfwGetWindowAttrib
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup window**/
                 /*x1150*/
                 /// <summary>
                 /// Sets an attribute of the specified window.
                 /// </summary>
                 /// <param name="window">The window to set the attribute for.</param>
                 /// <param name="attrib">A supported window attribute.</param>
                 /// <param name="value">`GLFW_TRUE` or `GLFW_FALSE`.</param>
        /*x1157*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowAttrib(IntPtr/*GLFWwindow*/ window, int attrib, int value/*x1158*/);
        /*x1159*/
        /*x1160*/
        /*x1161*//*/*
		/*! @brief Sets the user pointer of the specified window.
		*
		*  This function sets the user-defined pointer of the specified window.  The
		*  current value is retained until the window is destroyed.  The initial value
		*  is `NULL`.
		*
		*  @param[in] window The window whose pointer to set.
		*  @param[in] pointer The new value.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.  Access is not
		*  synchronized.
		*
		*  @sa @ref window_userptr
		*  @sa @ref glfwGetWindowUserPointer
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                 /*x1162*/
                 /// <summary>
                 /// Sets the user pointer of the specified window.
                 /// </summary>
                 /// <param name="window">The window whose pointer to set.</param>
                 /// <param name="pointer">The new value.</param>
        /*x1168*/
        [DllImport(LIB)]
        public static extern void glfwSetWindowUserPointer(IntPtr/*GLFWwindow*/ window, IntPtr/*void*/ pointer/*x1169*/);
        /*x1170*/
        /*x1171*/
        /*x1172*//*/*
		/*! @brief Returns the user pointer of the specified window.
		*
		*  This function returns the current value of the user-defined pointer of the
		*  specified window.  The initial value is `NULL`.
		*
		*  @param[in] window The window whose pointer to return.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.  Access is not
		*  synchronized.
		*
		*  @sa @ref window_userptr
		*  @sa @ref glfwSetWindowUserPointer
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                 /*x1173*/
                 /// <summary>
                 /// Returns the user pointer of the specified window.
                 /// </summary>
                 /// <param name="window">The window whose pointer to return.</param>
        /*x1178*/
        [DllImport(LIB)]
        public static extern IntPtr/*void*/ glfwGetWindowUserPointer(IntPtr/*GLFWwindow*/ window/*x1179*/);
        /*x1180*/
        /*x1181*/
        /*x1182*//*/*
		/*! @brief Sets the position callback for the specified window.
		*
		*  This function sets the position callback of the specified window, which is
		*  called when the window is moved.  The callback is provided with the
		*  position, in screen coordinates, of the upper-left corner of the content
		*  area of the window.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @remark @wayland This callback will never be called, as there is no way for
		*  an application to know its global position.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_pos
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                 /*x1183*/
                 /// <summary>
                 /// Sets the position callback for the specified window.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1190*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWwindowposfun*/ glfwSetWindowPosCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWwindowposfun*/ cbfun/*x1191*/);
        /*x1192*/
        /*x1193*/
        /*x1194*//*/*
		/*! @brief Sets the size callback for the specified window.
		*
		*  This function sets the size callback of the specified window, which is
		*  called when the window is resized.  The callback is provided with the size,
		*  in screen coordinates, of the content area of the window.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_size
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter and return value.
		*
		*  @ingroup window**/
                 /*x1195*/
                 /// <summary>
                 /// Sets the size callback for the specified window.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1202*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWwindowsizefun*/ glfwSetWindowSizeCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWwindowsizefun*/ cbfun/*x1203*/);
        /*x1204*/
        /*x1205*/
        /*x1206*//*/*
		/*! @brief Sets the close callback for the specified window.
		*
		*  This function sets the close callback of the specified window, which is
		*  called when the user attempts to close the window, for example by clicking
		*  the close widget in the title bar.
		*
		*  The close flag is set before this callback is called, but you can modify it
		*  at any time with @ref glfwSetWindowShouldClose.
		*
		*  The close callback is not triggered by @ref glfwDestroyWindow.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @remark @macos Selecting Quit from the application menu will trigger the
		*  close callback for all windows.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_close
		*
		*  @since Added in version 2.5.
		*  @glfw3 Added window handle parameter and return value.
		*
		*  @ingroup window**/
                 /*x1207*/
                 /// <summary>
                 /// Sets the close callback for the specified window.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1214*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWwindowclosefun*/ glfwSetWindowCloseCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWwindowclosefun*/ cbfun/*x1215*/);
        /*x1216*/
        /*x1217*/
        /*x1218*//*/*
		/*! @brief Sets the refresh callback for the specified window.
		*
		*  This function sets the refresh callback of the specified window, which is
		*  called when the content area of the window needs to be redrawn, for example
		*  if the window has been exposed after having been covered by another window.
		*
		*  On compositing window systems such as Aero, Compiz, Aqua or Wayland, where
		*  the window contents are saved off-screen, this callback may be called only
		*  very infrequently or never at all.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_refresh
		*
		*  @since Added in version 2.5.
		*  @glfw3 Added window handle parameter and return value.
		*
		*  @ingroup window**/
                 /*x1219*/
                 /// <summary>
                 /// Sets the refresh callback for the specified window.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1226*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWwindowrefreshfun*/ glfwSetWindowRefreshCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWwindowrefreshfun*/ cbfun/*x1227*/);
        /*x1228*/
        /*x1229*/
        /*x1230*//*/*
		/*! @brief Sets the focus callback for the specified window.
		*
		*  This function sets the focus callback of the specified window, which is
		*  called when the window gains or loses input focus.
		*
		*  After the focus callback is called for a window that lost input focus,
		*  synthetic key and mouse button release events will be generated for all such
		*  that had been pressed.  For more information, see @ref glfwSetKeyCallback
		*  and @ref glfwSetMouseButtonCallback.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_focus
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                 /*x1231*/
                 /// <summary>
                 /// Sets the focus callback for the specified window.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1238*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWwindowfocusfun*/ glfwSetWindowFocusCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWwindowfocusfun*/ cbfun/*x1239*/);
        /*x1240*/
        /*x1241*/
        /*x1242*//*/*
		/*! @brief Sets the iconify callback for the specified window.
		*
		*  This function sets the iconification callback of the specified window, which
		*  is called when the window is iconified or restored.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @remark @wayland The wl_shell protocol has no concept of iconification,
		*  this callback will never be called when using this deprecated protocol.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_iconify
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                 /*x1243*/
                 /// <summary>
                 /// Sets the iconify callback for the specified window.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1250*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWwindowiconifyfun*/ glfwSetWindowIconifyCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWwindowiconifyfun*/ cbfun/*x1251*/);
        /*x1252*/
        /*x1253*/
        /*x1254*//*/*
		/*! @brief Sets the maximize callback for the specified window.
		*
		*  This function sets the maximization callback of the specified window, which
		*  is called when the window is maximized or restored.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_maximize
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup window**/
                 /*x1255*/
                 /// <summary>
                 /// Sets the maximize callback for the specified window.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1262*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWwindowmaximizefun*/ glfwSetWindowMaximizeCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWwindowmaximizefun*/ cbfun/*x1263*/);
        /*x1264*/
        /*x1265*/
        /*x1266*//*/*
		/*! @brief Sets the framebuffer resize callback for the specified window.
		*
		*  This function sets the framebuffer resize callback of the specified window,
		*  which is called when the framebuffer of the specified window is resized.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_fbsize
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup window**/
                 /*x1267*/
                 /// <summary>
                 /// Sets the framebuffer resize callback for the specified window.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1274*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWframebuffersizefun*/ glfwSetFramebufferSizeCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWframebuffersizefun*/ cbfun/*x1275*/);
        /*x1276*/
        /*x1277*/
        /*x1278*//*/*
		/*! @brief Sets the window content scale callback for the specified window.
		*
		*  This function sets the window content scale callback of the specified window,
		*  which is called when the content scale of the specified window changes.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref window_scale
		*  @sa @ref glfwGetWindowContentScale
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup window**/
                 /*x1279*/
                 /// <summary>
                 /// Sets the window content scale callback for the specified window.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1286*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWwindowcontentscalefun*/ glfwSetWindowContentScaleCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWwindowcontentscalefun*/ cbfun/*x1287*/);
        /*x1288*/
        /*x1289*/
        /*x1290*//*/*
		/*! @brief Processes all pending events.
		*
		*  This function processes only those events that are already in the event
		*  queue and then returns immediately.  Processing events will cause the window
		*  and input callbacks associated with those events to be called.
		*
		*  On some platforms, a window move, resize or menu operation will cause event
		*  processing to block.  This is due to how event processing is designed on
		*  those platforms.  You can use the
		*  [window refresh callback](@ref window_refresh) to redraw the contents of
		*  your window when necessary during such operations.
		*
		*  Do not assume that callbacks you set will _only_ be called in response to
		*  event processing functions like this one.  While it is necessary to poll for
		*  events, window systems that require GLFW to register callbacks of its own
		*  can pass events to GLFW in response to many window system function calls.
		*  GLFW will pass those events on to the application callbacks before
		*  returning.
		*
		*  Event processing is not required for joystick input to work.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @reentrancy This function must not be called from a callback.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref events
		*  @sa @ref glfwWaitEvents
		*  @sa @ref glfwWaitEventsTimeout
		*
		*  @since Added in version 1.0.
		*
		*  @ingroup window**/
                 /*x1291*/
                 /// <summary>
                 /// Processes all pending events.
                 /// </summary>
        /*x1295*/
        [DllImport(LIB)]
        public static extern void glfwPollEvents(/*x1296*/);
        /*x1297*/
        /*x1298*/
        /*x1299*//*/*
		/*! @brief Waits until events are queued and processes them.
		*
		*  This function puts the calling thread to sleep until at least one event is
		*  available in the event queue.  Once one or more events are available,
		*  it behaves exactly like @ref glfwPollEvents, i.e. the events in the queue
		*  are processed and the function then returns immediately.  Processing events
		*  will cause the window and input callbacks associated with those events to be
		*  called.
		*
		*  Since not all events are associated with callbacks, this function may return
		*  without a callback having been called even if you are monitoring all
		*  callbacks.
		*
		*  On some platforms, a window move, resize or menu operation will cause event
		*  processing to block.  This is due to how event processing is designed on
		*  those platforms.  You can use the
		*  [window refresh callback](@ref window_refresh) to redraw the contents of
		*  your window when necessary during such operations.
		*
		*  Do not assume that callbacks you set will _only_ be called in response to
		*  event processing functions like this one.  While it is necessary to poll for
		*  events, window systems that require GLFW to register callbacks of its own
		*  can pass events to GLFW in response to many window system function calls.
		*  GLFW will pass those events on to the application callbacks before
		*  returning.
		*
		*  Event processing is not required for joystick input to work.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @reentrancy This function must not be called from a callback.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref events
		*  @sa @ref glfwPollEvents
		*  @sa @ref glfwWaitEventsTimeout
		*
		*  @since Added in version 2.5.
		*
		*  @ingroup window**/
                 /*x1300*/
                 /// <summary>
                 /// Waits until events are queued and processes them.
                 /// </summary>
        /*x1304*/
        [DllImport(LIB)]
        public static extern void glfwWaitEvents(/*x1305*/);
        /*x1306*/
        /*x1307*/
        /*x1308*//*/*
		/*! @brief Waits with timeout until events are queued and processes them.
		*
		*  This function puts the calling thread to sleep until at least one event is
		*  available in the event queue, or until the specified timeout is reached.  If
		*  one or more events are available, it behaves exactly like @ref
		*  glfwPollEvents, i.e. the events in the queue are processed and the function
		*  then returns immediately.  Processing events will cause the window and input
		*  callbacks associated with those events to be called.
		*
		*  The timeout value must be a positive finite number.
		*
		*  Since not all events are associated with callbacks, this function may return
		*  without a callback having been called even if you are monitoring all
		*  callbacks.
		*
		*  On some platforms, a window move, resize or menu operation will cause event
		*  processing to block.  This is due to how event processing is designed on
		*  those platforms.  You can use the
		*  [window refresh callback](@ref window_refresh) to redraw the contents of
		*  your window when necessary during such operations.
		*
		*  Do not assume that callbacks you set will _only_ be called in response to
		*  event processing functions like this one.  While it is necessary to poll for
		*  events, window systems that require GLFW to register callbacks of its own
		*  can pass events to GLFW in response to many window system function calls.
		*  GLFW will pass those events on to the application callbacks before
		*  returning.
		*
		*  Event processing is not required for joystick input to work.
		*
		*  @param[in] timeout The maximum amount of time, in seconds, to wait.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_VALUE and @ref GLFW_PLATFORM_ERROR.
		*
		*  @reentrancy This function must not be called from a callback.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref events
		*  @sa @ref glfwPollEvents
		*  @sa @ref glfwWaitEvents
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup window**/
                 /*x1309*/
                 /// <summary>
                 /// Waits with timeout until events are queued and processes them.
                 /// </summary>
                 /// <param name="timeout">The maximum amount of time, in seconds, to wait.</param>
        /*x1314*/
        [DllImport(LIB)]
        public static extern void glfwWaitEventsTimeout(double timeout/*x1315*/);
        /*x1316*/
        /*x1317*/
        /*x1318*//*/*
		/*! @brief Posts an empty event to the event queue.
		*
		*  This function posts an empty event from the current thread to the event
		*  queue, causing @ref glfwWaitEvents or @ref glfwWaitEventsTimeout to return.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref events
		*  @sa @ref glfwWaitEvents
		*  @sa @ref glfwWaitEventsTimeout
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup window**/
                 /*x1319*/
                 /// <summary>
                 /// Posts an empty event to the event queue.
                 /// </summary>
        /*x1323*/
        [DllImport(LIB)]
        public static extern void glfwPostEmptyEvent(/*x1324*/);
        /*x1325*/
        /*x1326*/
        /*x1327*//*/*
		/*! @brief Returns the value of an input option for the specified window.
		*
		*  This function returns the value of an input option for the specified window.
		*  The mode must be one of @ref GLFW_CURSOR, @ref GLFW_STICKY_KEYS,
		*  @ref GLFW_STICKY_MOUSE_BUTTONS, @ref GLFW_LOCK_KEY_MODS or
		*  @ref GLFW_RAW_MOUSE_MOTION.
		*
		*  @param[in] window The window to query.
		*  @param[in] mode One of `GLFW_CURSOR`, `GLFW_STICKY_KEYS`,
		*  `GLFW_STICKY_MOUSE_BUTTONS`, `GLFW_LOCK_KEY_MODS` or
		*  `GLFW_RAW_MOUSE_MOTION`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_INVALID_ENUM.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref glfwSetInputMode
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup input**/
                 /*x1328*/
                 /// <summary>
                 /// Returns the value of an input option for the specified window.
                 /// </summary>
                 /// <param name="window">The window to query.</param>
                 /// <param name="mode">One of `GLFW_CURSOR`, `GLFW_STICKY_KEYS`,</param>
        /*x1334*/
        [DllImport(LIB)]
        public static extern int glfwGetInputMode(IntPtr/*GLFWwindow*/ window, int mode/*x1335*/);
        /*x1336*/
        /*x1337*/
        /*x1338*//*/*
		/*! @brief Sets an input option for the specified window.
		*
		*  This function sets an input mode option for the specified window.  The mode
		*  must be one of @ref GLFW_CURSOR, @ref GLFW_STICKY_KEYS,
		*  @ref GLFW_STICKY_MOUSE_BUTTONS, @ref GLFW_LOCK_KEY_MODS or
		*  @ref GLFW_RAW_MOUSE_MOTION.
		*
		*  If the mode is `GLFW_CURSOR`, the value must be one of the following cursor
		*  modes:
		*  - `GLFW_CURSOR_NORMAL` makes the cursor visible and behaving normally.
		*  - `GLFW_CURSOR_HIDDEN` makes the cursor invisible when it is over the
		*    content area of the window but does not restrict the cursor from leaving.
		*  - `GLFW_CURSOR_DISABLED` hides and grabs the cursor, providing virtual
		*    and unlimited cursor movement.  This is useful for implementing for
		*    example 3D camera controls.
		*
		*  If the mode is `GLFW_STICKY_KEYS`, the value must be either `GLFW_TRUE` to
		*  enable sticky keys, or `GLFW_FALSE` to disable it.  If sticky keys are
		*  enabled, a key press will ensure that @ref glfwGetKey returns `GLFW_PRESS`
		*  the next time it is called even if the key had been released before the
		*  call.  This is useful when you are only interested in whether keys have been
		*  pressed but not when or in which order.
		*
		*  If the mode is `GLFW_STICKY_MOUSE_BUTTONS`, the value must be either
		*  `GLFW_TRUE` to enable sticky mouse buttons, or `GLFW_FALSE` to disable it.
		*  If sticky mouse buttons are enabled, a mouse button press will ensure that
		*  @ref glfwGetMouseButton returns `GLFW_PRESS` the next time it is called even
		*  if the mouse button had been released before the call.  This is useful when
		*  you are only interested in whether mouse buttons have been pressed but not
		*  when or in which order.
		*
		*  If the mode is `GLFW_LOCK_KEY_MODS`, the value must be either `GLFW_TRUE` to
		*  enable lock key modifier bits, or `GLFW_FALSE` to disable them.  If enabled,
		*  callbacks that receive modifier bits will also have the @ref
		*  GLFW_MOD_CAPS_LOCK bit set when the event was generated with Caps Lock on,
		*  and the @ref GLFW_MOD_NUM_LOCK bit when Num Lock was on.
		*
		*  If the mode is `GLFW_RAW_MOUSE_MOTION`, the value must be either `GLFW_TRUE`
		*  to enable raw (unscaled and unaccelerated) mouse motion when the cursor is
		*  disabled, or `GLFW_FALSE` to disable it.  If raw motion is not supported,
		*  attempting to set this will emit @ref GLFW_PLATFORM_ERROR.  Call @ref
		*  glfwRawMouseMotionSupported to check for support.
		*
		*  @param[in] window The window whose input mode to set.
		*  @param[in] mode One of `GLFW_CURSOR`, `GLFW_STICKY_KEYS`,
		*  `GLFW_STICKY_MOUSE_BUTTONS`, `GLFW_LOCK_KEY_MODS` or
		*  `GLFW_RAW_MOUSE_MOTION`.
		*  @param[in] value The new value of the specified input mode.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM and @ref GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref glfwGetInputMode
		*
		*  @since Added in version 3.0.  Replaces `glfwEnable` and `glfwDisable`.
		*
		*  @ingroup input**/
                 /*x1339*/
                 /// <summary>
                 /// Sets an input option for the specified window.
                 /// </summary>
                 /// <param name="window">The window whose input mode to set.</param>
                 /// <param name="mode">One of `GLFW_CURSOR`, `GLFW_STICKY_KEYS`,</param>
                 /// <param name="value">The new value of the specified input mode.</param>
        /*x1346*/
        [DllImport(LIB)]
        public static extern void glfwSetInputMode(IntPtr/*GLFWwindow*/ window, int mode, int value/*x1347*/);
        /*x1348*/
        /*x1349*/
        /*x1350*//*/*
		/*! @brief Returns whether raw mouse motion is supported.
		*
		*  This function returns whether raw mouse motion is supported on the current
		*  system.  This status does not change after GLFW has been initialized so you
		*  only need to check this once.  If you attempt to enable raw motion on
		*  a system that does not support it, @ref GLFW_PLATFORM_ERROR will be emitted.
		*
		*  Raw mouse motion is closer to the actual motion of the mouse across
		*  a surface.  It is not affected by the scaling and acceleration applied to
		*  the motion of the desktop cursor.  That processing is suitable for a cursor
		*  while raw motion is better for controlling for example a 3D camera.  Because
		*  of this, raw mouse motion is only provided when the cursor is disabled.
		*
		*  @return `GLFW_TRUE` if raw mouse motion is supported on the current machine,
		*  or `GLFW_FALSE` otherwise.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref raw_mouse_motion
		*  @sa @ref glfwSetInputMode
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                 /*x1351*/
                 /// <summary>
                 /// Returns whether raw mouse motion is supported.
                 /// </summary>
                 /// <returns>`GLFW_TRUE` if raw mouse motion is supported on the current machine,</returns>
        /*x1356*/
        [DllImport(LIB)]
        public static extern int glfwRawMouseMotionSupported(/*x1357*/);
        /*x1358*/
        /*x1359*/
        /*x1360*//*/*
		/*! @brief Returns the layout-specific name of the specified printable key.
		*
		*  This function returns the name of the specified printable key, encoded as
		*  UTF-8.  This is typically the character that key would produce without any
		*  modifier keys, intended for displaying key bindings to the user.  For dead
		*  keys, it is typically the diacritic it would add to a character.
		*
		*  __Do not use this function__ for [text input](@ref input_char).  You will
		*  break text input for many languages even if it happens to work for yours.
		*
		*  If the key is `GLFW_KEY_UNKNOWN`, the scancode is used to identify the key,
		*  otherwise the scancode is ignored.  If you specify a non-printable key, or
		*  `GLFW_KEY_UNKNOWN` and a scancode that maps to a non-printable key, this
		*  function returns `NULL` but does not emit an error.
		*
		*  This behavior allows you to always pass in the arguments in the
		*  [key callback](@ref input_key) without modification.
		*
		*  The printable keys are:
		*  - `GLFW_KEY_APOSTROPHE`
		*  - `GLFW_KEY_COMMA`
		*  - `GLFW_KEY_MINUS`
		*  - `GLFW_KEY_PERIOD`
		*  - `GLFW_KEY_SLASH`
		*  - `GLFW_KEY_SEMICOLON`
		*  - `GLFW_KEY_EQUAL`
		*  - `GLFW_KEY_LEFT_BRACKET`
		*  - `GLFW_KEY_RIGHT_BRACKET`
		*  - `GLFW_KEY_BACKSLASH`
		*  - `GLFW_KEY_WORLD_1`
		*  - `GLFW_KEY_WORLD_2`
		*  - `GLFW_KEY_0` to `GLFW_KEY_9`
		*  - `GLFW_KEY_A` to `GLFW_KEY_Z`
		*  - `GLFW_KEY_KP_0` to `GLFW_KEY_KP_9`
		*  - `GLFW_KEY_KP_DECIMAL`
		*  - `GLFW_KEY_KP_DIVIDE`
		*  - `GLFW_KEY_KP_MULTIPLY`
		*  - `GLFW_KEY_KP_SUBTRACT`
		*  - `GLFW_KEY_KP_ADD`
		*  - `GLFW_KEY_KP_EQUAL`
		*
		*  Names for printable keys depend on keyboard layout, while names for
		*  non-printable keys are the same across layouts but depend on the application
		*  language and should be localized along with other user interface text.
		*
		*  @param[in] key The key to query, or `GLFW_KEY_UNKNOWN`.
		*  @param[in] scancode The scancode of the key to query.
		*  @return The UTF-8 encoded, layout-specific name of the key, or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The returned string is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the next call to @ref
		*  glfwGetKeyName, or until the library is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref input_key_name
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup input**/
                 /*x1361*/
                 /// <summary>
                 /// Returns the layout-specific name of the specified printable key.
                 /// </summary>
                 /// <param name="key">The key to query, or `GLFW_KEY_UNKNOWN`.</param>
                 /// <param name="scancode">The scancode of the key to query.</param>
                 /// <returns>The UTF-8 encoded, layout-specific name of the key, or `NULL`.</returns>
        /*x1368*/
        [DllImport(LIB)]
        public static extern IntPtr/*char*/ glfwGetKeyName(int key, int scancode/*x1369*/);
        /*x1370*/
        /*x1371*/
        /*x1372*//*/*
		/*! @brief Returns the platform-specific scancode of the specified key.
		*
		*  This function returns the platform-specific scancode of the specified key.
		*
		*  If the key is `GLFW_KEY_UNKNOWN` or does not exist on the keyboard this
		*  method will return `-1`.
		*
		*  @param[in] key Any [named key](@ref keys).
		*  @return The platform-specific scancode for the key, or `-1` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM and @ref GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref input_key
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                 /*x1373*/
                 /// <summary>
                 /// Returns the platform-specific scancode of the specified key.
                 /// </summary>
                 /// <param name="key">Any [named key](@ref keys).</param>
                 /// <returns>The platform-specific scancode for the key, or `-1` if an</returns>
        /*x1379*/
        [DllImport(LIB)]
        public static extern int glfwGetKeyScancode(int key/*x1380*/);
        /*x1381*/
        /*x1382*/
        /*x1383*//*/*
		/*! @brief Returns the last reported state of a keyboard key for the specified
		*  window.
		*
		*  This function returns the last state reported for the specified key to the
		*  specified window.  The returned state is one of `GLFW_PRESS` or
		*  `GLFW_RELEASE`.  The higher-level action `GLFW_REPEAT` is only reported to
		*  the key callback.
		*
		*  If the @ref GLFW_STICKY_KEYS input mode is enabled, this function returns
		*  `GLFW_PRESS` the first time you call it for a key that was pressed, even if
		*  that key has already been released.
		*
		*  The key functions deal with physical keys, with [key tokens](@ref keys)
		*  named after their use on the standard US keyboard layout.  If you want to
		*  input text, use the Unicode character callback instead.
		*
		*  The [modifier key bit masks](@ref mods) are not key tokens and cannot be
		*  used with this function.
		*
		*  __Do not use this function__ to implement [text input](@ref input_char).
		*
		*  @param[in] window The desired window.
		*  @param[in] key The desired [keyboard key](@ref keys).  `GLFW_KEY_UNKNOWN` is
		*  not a valid key for this function.
		*  @return One of `GLFW_PRESS` or `GLFW_RELEASE`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_INVALID_ENUM.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref input_key
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup input**/
                 /*x1384*/
                 /// <summary>
                 /// Returns the last reported state of a keyboard key for the specified
                 /// </summary>
                 /// <param name="window">The desired window.</param>
                 /// <param name="key">The desired [keyboard key](@ref keys).  `GLFW_KEY_UNKNOWN` is</param>
                 /// <returns>One of `GLFW_PRESS` or `GLFW_RELEASE`.</returns>
        /*x1391*/
        [DllImport(LIB)]
        public static extern int glfwGetKey(IntPtr/*GLFWwindow*/ window, int key/*x1392*/);
        /*x1393*/
        /*x1394*/
        /*x1395*//*/*
		/*! @brief Returns the last reported state of a mouse button for the specified
		*  window.
		*
		*  This function returns the last state reported for the specified mouse button
		*  to the specified window.  The returned state is one of `GLFW_PRESS` or
		*  `GLFW_RELEASE`.
		*
		*  If the @ref GLFW_STICKY_MOUSE_BUTTONS input mode is enabled, this function
		*  returns `GLFW_PRESS` the first time you call it for a mouse button that was
		*  pressed, even if that mouse button has already been released.
		*
		*  @param[in] window The desired window.
		*  @param[in] button The desired [mouse button](@ref buttons).
		*  @return One of `GLFW_PRESS` or `GLFW_RELEASE`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_INVALID_ENUM.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref input_mouse_button
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup input**/
                 /*x1396*/
                 /// <summary>
                 /// Returns the last reported state of a mouse button for the specified
                 /// </summary>
                 /// <param name="window">The desired window.</param>
                 /// <param name="button">The desired [mouse button](@ref buttons).</param>
                 /// <returns>One of `GLFW_PRESS` or `GLFW_RELEASE`.</returns>
        /*x1403*/
        [DllImport(LIB)]
        public static extern int glfwGetMouseButton(IntPtr/*GLFWwindow*/ window, int button/*x1404*/);
        /*x1405*/
        /*x1406*/
        /*x1407*//*/*
		/*! @brief Retrieves the position of the cursor relative to the content area of
		*  the window.
		*
		*  This function returns the position of the cursor, in screen coordinates,
		*  relative to the upper-left corner of the content area of the specified
		*  window.
		*
		*  If the cursor is disabled (with `GLFW_CURSOR_DISABLED`) then the cursor
		*  position is unbounded and limited only by the minimum and maximum values of
		*  a `double`.
		*
		*  The coordinate can be converted to their integer equivalents with the
		*  `floor` function.  Casting directly to an integer type works for positive
		*  coordinates, but fails for negative ones.
		*
		*  Any or all of the position arguments may be `NULL`.  If an error occurs, all
		*  non-`NULL` position arguments will be set to zero.
		*
		*  @param[in] window The desired window.
		*  @param[out] xpos Where to store the cursor x-coordinate, relative to the
		*  left edge of the content area, or `NULL`.
		*  @param[out] ypos Where to store the cursor y-coordinate, relative to the to
		*  top edge of the content area, or `NULL`.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref cursor_pos
		*  @sa @ref glfwSetCursorPos
		*
		*  @since Added in version 3.0.  Replaces `glfwGetMousePos`.
		*
		*  @ingroup input**/
                 /*x1408*/
                 /// <summary>
                 /// Retrieves the position of the cursor relative to the content area of
                 /// </summary>
                 /// <param name="window">The desired window.</param>
                 /// <param name="xpos">Where to store the cursor x-coordinate, relative to the</param>
                 /// <param name="ypos">Where to store the cursor y-coordinate, relative to the to</param>
        /*x1415*/
        [DllImport(LIB)]
        public static extern void glfwGetCursorPos(IntPtr/*GLFWwindow*/ window, out double xpos, out double ypos/*x1416*/);
        /*x1417*/
        /*x1418*/
        /*x1419*//*/*
		/*! @brief Sets the position of the cursor, relative to the content area of the
		*  window.
		*
		*  This function sets the position, in screen coordinates, of the cursor
		*  relative to the upper-left corner of the content area of the specified
		*  window.  The window must have input focus.  If the window does not have
		*  input focus when this function is called, it fails silently.
		*
		*  __Do not use this function__ to implement things like camera controls.  GLFW
		*  already provides the `GLFW_CURSOR_DISABLED` cursor mode that hides the
		*  cursor, transparently re-centers it and provides unconstrained cursor
		*  motion.  See @ref glfwSetInputMode for more information.
		*
		*  If the cursor mode is `GLFW_CURSOR_DISABLED` then the cursor position is
		*  unconstrained and limited only by the minimum and maximum values of
		*  a `double`.
		*
		*  @param[in] window The desired window.
		*  @param[in] xpos The desired x-coordinate, relative to the left edge of the
		*  content area.
		*  @param[in] ypos The desired y-coordinate, relative to the top edge of the
		*  content area.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @remark @wayland This function will only work when the cursor mode is
		*  `GLFW_CURSOR_DISABLED`, otherwise it will do nothing.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref cursor_pos
		*  @sa @ref glfwGetCursorPos
		*
		*  @since Added in version 3.0.  Replaces `glfwSetMousePos`.
		*
		*  @ingroup input**/
                 /*x1420*/
                 /// <summary>
                 /// Sets the position of the cursor, relative to the content area of the
                 /// </summary>
                 /// <param name="window">The desired window.</param>
                 /// <param name="xpos">The desired x-coordinate, relative to the left edge of the</param>
                 /// <param name="ypos">The desired y-coordinate, relative to the top edge of the</param>
        /*x1427*/
        [DllImport(LIB)]
        public static extern void glfwSetCursorPos(IntPtr/*GLFWwindow*/ window, double xpos, double ypos/*x1428*/);
        /*x1429*/
        /*x1430*/
        /*x1431*//*/*
		/*! @brief Creates a custom cursor.
		*
		*  Creates a new custom cursor image that can be set for a window with @ref
		*  glfwSetCursor.  The cursor can be destroyed with @ref glfwDestroyCursor.
		*  Any remaining cursors are destroyed by @ref glfwTerminate.
		*
		*  The pixels are 32-bit, little-endian, non-premultiplied RGBA, i.e. eight
		*  bits per channel with the red channel first.  They are arranged canonically
		*  as packed sequential rows, starting from the top-left corner.
		*
		*  The cursor hotspot is specified in pixels, relative to the upper-left corner
		*  of the cursor image.  Like all other coordinate systems in GLFW, the X-axis
		*  points to the right and the Y-axis points down.
		*
		*  @param[in] image The desired cursor image.
		*  @param[in] xhot The desired x-coordinate, in pixels, of the cursor hotspot.
		*  @param[in] yhot The desired y-coordinate, in pixels, of the cursor hotspot.
		*  @return The handle of the created cursor, or `NULL` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The specified image data is copied before this function
		*  returns.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref cursor_object
		*  @sa @ref glfwDestroyCursor
		*  @sa @ref glfwCreateStandardCursor
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup input**/
                 /*x1432*/
                 /// <summary>
                 /// Creates a custom cursor.
                 /// </summary>
                 /// <param name="image">The desired cursor image.</param>
                 /// <param name="xhot">The desired x-coordinate, in pixels, of the cursor hotspot.</param>
                 /// <param name="yhot">The desired y-coordinate, in pixels, of the cursor hotspot.</param>
                 /// <returns>The handle of the created cursor, or `NULL` if an</returns>
        /*x1440*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWcursor*/ glfwCreateCursor(IntPtr/*GLFWimage*/ image, int xhot, int yhot/*x1441*/);
        /*x1442*/
        /*x1443*/
        /*x1444*//*/*
		/*! @brief Creates a cursor with a standard shape.
		*
		*  Returns a cursor with a [standard shape](@ref shapes), that can be set for
		*  a window with @ref glfwSetCursor.
		*
		*  @param[in] shape One of the [standard shapes](@ref shapes).
		*  @return A new cursor ready to use or `NULL` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM and @ref GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref cursor_object
		*  @sa @ref glfwCreateCursor
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup input**/
                 /*x1445*/
                 /// <summary>
                 /// Creates a cursor with a standard shape.
                 /// </summary>
                 /// <param name="shape">One of the [standard shapes](@ref shapes).</param>
                 /// <returns>A new cursor ready to use or `NULL` if an</returns>
        /*x1451*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWcursor*/ glfwCreateStandardCursor(int shape/*x1452*/);
        /*x1453*/
        /*x1454*/
        /*x1455*//*/*
		/*! @brief Destroys a cursor.
		*
		*  This function destroys a cursor previously created with @ref
		*  glfwCreateCursor.  Any remaining cursors will be destroyed by @ref
		*  glfwTerminate.
		*
		*  If the specified cursor is current for any window, that window will be
		*  reverted to the default cursor.  This does not affect the cursor mode.
		*
		*  @param[in] cursor The cursor object to destroy.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @reentrancy This function must not be called from a callback.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref cursor_object
		*  @sa @ref glfwCreateCursor
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup input**/
                 /*x1456*/
                 /// <summary>
                 /// Destroys a cursor.
                 /// </summary>
                 /// <param name="cursor">The cursor object to destroy.</param>
        /*x1461*/
        [DllImport(LIB)]
        public static extern void glfwDestroyCursor(IntPtr/*GLFWcursor*/ cursor/*x1462*/);
        /*x1463*/
        /*x1464*/
        /*x1465*//*/*
		/*! @brief Sets the cursor for the window.
		*
		*  This function sets the cursor image to be used when the cursor is over the
		*  content area of the specified window.  The set cursor will only be visible
		*  when the [cursor mode](@ref cursor_mode) of the window is
		*  `GLFW_CURSOR_NORMAL`.
		*
		*  On some platforms, the set cursor may not be visible unless the window also
		*  has input focus.
		*
		*  @param[in] window The window to set the cursor for.
		*  @param[in] cursor The cursor to set, or `NULL` to switch back to the default
		*  arrow cursor.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref cursor_object
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup input**/
                 /*x1466*/
                 /// <summary>
                 /// Sets the cursor for the window.
                 /// </summary>
                 /// <param name="window">The window to set the cursor for.</param>
                 /// <param name="cursor">The cursor to set, or `NULL` to switch back to the default</param>
        /*x1472*/
        [DllImport(LIB)]
        public static extern void glfwSetCursor(IntPtr/*GLFWwindow*/ window, IntPtr/*GLFWcursor*/ cursor/*x1473*/);
        /*x1474*/
        /*x1475*/
        /*x1476*//*/*
		/*! @brief Sets the key callback.
		*
		*  This function sets the key callback of the specified window, which is called
		*  when a key is pressed, repeated or released.
		*
		*  The key functions deal with physical keys, with layout independent
		*  [key tokens](@ref keys) named after their values in the standard US keyboard
		*  layout.  If you want to input text, use the
		*  [character callback](@ref glfwSetCharCallback) instead.
		*
		*  When a window loses input focus, it will generate synthetic key release
		*  events for all pressed keys.  You can tell these events from user-generated
		*  events by the fact that the synthetic ones are generated after the focus
		*  loss event has been processed, i.e. after the
		*  [window focus callback](@ref glfwSetWindowFocusCallback) has been called.
		*
		*  The scancode of a key is specific to that platform or sometimes even to that
		*  machine.  Scancodes are intended to allow users to bind keys that don't have
		*  a GLFW key token.  Such keys have `key` set to `GLFW_KEY_UNKNOWN`, their
		*  state is not saved and so it cannot be queried with @ref glfwGetKey.
		*
		*  Sometimes GLFW needs to generate synthetic key events, in which case the
		*  scancode may be zero.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new key callback, or `NULL` to remove the currently
		*  set callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref input_key
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter and return value.
		*
		*  @ingroup input**/
                 /*x1477*/
                 /// <summary>
                 /// Sets the key callback.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new key callback, or `NULL` to remove the currently</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1484*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWkeyfun*/ glfwSetKeyCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWkeyfun*/ cbfun/*x1485*/);
        /*x1486*/
        /*x1487*/
        /*x1488*//*/*
		/*! @brief Sets the Unicode character callback.
		*
		*  This function sets the character callback of the specified window, which is
		*  called when a Unicode character is input.
		*
		*  The character callback is intended for Unicode text input.  As it deals with
		*  characters, it is keyboard layout dependent, whereas the
		*  [key callback](@ref glfwSetKeyCallback) is not.  Characters do not map 1:1
		*  to physical keys, as a key may produce zero, one or more characters.  If you
		*  want to know whether a specific physical key was pressed or released, see
		*  the key callback instead.
		*
		*  The character callback behaves as system text input normally does and will
		*  not be called if modifier keys are held down that would prevent normal text
		*  input on that platform, for example a Super (Command) key on macOS or Alt key
		*  on Windows.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref input_char
		*
		*  @since Added in version 2.4.
		*  @glfw3 Added window handle parameter and return value.
		*
		*  @ingroup input**/
                 /*x1489*/
                 /// <summary>
                 /// Sets the Unicode character callback.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1496*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWcharfun*/ glfwSetCharCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWcharfun*/ cbfun/*x1497*/);
        /*x1498*/
        /*x1499*/
        /*x1500*//*/*
		/*! @brief Sets the Unicode character with modifiers callback.
		*
		*  This function sets the character with modifiers callback of the specified
		*  window, which is called when a Unicode character is input regardless of what
		*  modifier keys are used.
		*
		*  The character with modifiers callback is intended for implementing custom
		*  Unicode character input.  For regular Unicode text input, see the
		*  [character callback](@ref glfwSetCharCallback).  Like the character
		*  callback, the character with modifiers callback deals with characters and is
		*  keyboard layout dependent.  Characters do not map 1:1 to physical keys, as
		*  a key may produce zero, one or more characters.  If you want to know whether
		*  a specific physical key was pressed or released, see the
		*  [key callback](@ref glfwSetKeyCallback) instead.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or an
		*  [error](@ref error_handling) occurred.
		*
		*  @deprecated Scheduled for removal in version 4.0.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref input_char
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup input**/
                 /*x1501*/
                 /// <summary>
                 /// Sets the Unicode character with modifiers callback.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or an</returns>
        /*x1508*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWcharmodsfun*/ glfwSetCharModsCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWcharmodsfun*/ cbfun/*x1509*/);
        /*x1510*/
        /*x1511*/
        /*x1512*//*/*
		/*! @brief Sets the mouse button callback.
		*
		*  This function sets the mouse button callback of the specified window, which
		*  is called when a mouse button is pressed or released.
		*
		*  When a window loses input focus, it will generate synthetic mouse button
		*  release events for all pressed mouse buttons.  You can tell these events
		*  from user-generated events by the fact that the synthetic ones are generated
		*  after the focus loss event has been processed, i.e. after the
		*  [window focus callback](@ref glfwSetWindowFocusCallback) has been called.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref input_mouse_button
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter and return value.
		*
		*  @ingroup input**/
                 /*x1513*/
                 /// <summary>
                 /// Sets the mouse button callback.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1520*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWmousebuttonfun*/ glfwSetMouseButtonCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWmousebuttonfun*/ cbfun/*x1521*/);
        /*x1522*/
        /*x1523*/
        /*x1524*//*/*
		/*! @brief Sets the cursor position callback.
		*
		*  This function sets the cursor position callback of the specified window,
		*  which is called when the cursor is moved.  The callback is provided with the
		*  position, in screen coordinates, relative to the upper-left corner of the
		*  content area of the window.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref cursor_pos
		*
		*  @since Added in version 3.0.  Replaces `glfwSetMousePosCallback`.
		*
		*  @ingroup input**/
                 /*x1525*/
                 /// <summary>
                 /// Sets the cursor position callback.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1532*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWcursorposfun*/ glfwSetCursorPosCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWcursorposfun*/ cbfun/*x1533*/);
        /*x1534*/
        /*x1535*/
        /*x1536*//*/*
		/*! @brief Sets the cursor enter/exit callback.
		*
		*  This function sets the cursor boundary crossing callback of the specified
		*  window, which is called when the cursor enters or leaves the content area of
		*  the window.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref cursor_enter
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup input**/
                 /*x1537*/
                 /// <summary>
                 /// Sets the cursor enter/exit callback.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1544*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWcursorenterfun*/ glfwSetCursorEnterCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWcursorenterfun*/ cbfun/*x1545*/);
        /*x1546*/
        /*x1547*/
        /*x1548*//*/*
		/*! @brief Sets the scroll callback.
		*
		*  This function sets the scroll callback of the specified window, which is
		*  called when a scrolling device is used, such as a mouse wheel or scrolling
		*  area of a touchpad.
		*
		*  The scroll callback receives all scrolling input, like that from a mouse
		*  wheel or a touchpad scrolling area.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new scroll callback, or `NULL` to remove the currently
		*  set callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref scrolling
		*
		*  @since Added in version 3.0.  Replaces `glfwSetMouseWheelCallback`.
		*
		*  @ingroup input**/
                 /*x1549*/
                 /// <summary>
                 /// Sets the scroll callback.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new scroll callback, or `NULL` to remove the currently</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1556*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWscrollfun*/ glfwSetScrollCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWscrollfun*/ cbfun/*x1557*/);
        /*x1558*/
        /*x1559*/
        /*x1560*//*/*
		/*! @brief Sets the file drop callback.
		*
		*  This function sets the file drop callback of the specified window, which is
		*  called when one or more dragged files are dropped on the window.
		*
		*  Because the path array and its strings may have been generated specifically
		*  for that event, they are not guaranteed to be valid after the callback has
		*  returned.  If you wish to use them after the callback returns, you need to
		*  make a deep copy.
		*
		*  @param[in] window The window whose callback to set.
		*  @param[in] cbfun The new file drop callback, or `NULL` to remove the
		*  currently set callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @remark @wayland File drop is currently unimplemented.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref path_drop
		*
		*  @since Added in version 3.1.
		*
		*  @ingroup input**/
                 /*x1561*/
                 /// <summary>
                 /// Sets the file drop callback.
                 /// </summary>
                 /// <param name="window">The window whose callback to set.</param>
                 /// <param name="cbfun">The new file drop callback, or `NULL` to remove the</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1568*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWdropfun*/ glfwSetDropCallback(IntPtr/*GLFWwindow*/ window, IntPtr /*GLFWdropfun*/ cbfun/*x1569*/);
        /*x1570*/
        /*x1571*/
        /*x1572*//*/*
		/*! @brief Returns whether the specified joystick is present.
		*
		*  This function returns whether the specified joystick is present.
		*
		*  There is no need to call this function before other functions that accept
		*  a joystick ID, as they all check for presence before performing any other
		*  work.
		*
		*  @param[in] jid The [joystick](@ref joysticks) to query.
		*  @return `GLFW_TRUE` if the joystick is present, or `GLFW_FALSE` otherwise.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM and @ref GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref joystick
		*
		*  @since Added in version 3.0.  Replaces `glfwGetJoystickParam`.
		*
		*  @ingroup input**/
                 /*x1573*/
                 /// <summary>
                 /// Returns whether the specified joystick is present.
                 /// </summary>
                 /// <param name="jid">The [joystick](@ref joysticks) to query.</param>
                 /// <returns>`GLFW_TRUE` if the joystick is present, or `GLFW_FALSE` otherwise.</returns>
        /*x1579*/
        [DllImport(LIB)]
        public static extern int glfwJoystickPresent(int jid/*x1580*/);
        /*x1581*/
        /*x1582*/
        /*x1583*//*/*
		/*! @brief Returns the values of all axes of the specified joystick.
		*
		*  This function returns the values of all axes of the specified joystick.
		*  Each element in the array is a value between -1.0 and 1.0.
		*
		*  If the specified joystick is not present this function will return `NULL`
		*  but will not generate an error.  This can be used instead of first calling
		*  @ref glfwJoystickPresent.
		*
		*  @param[in] jid The [joystick](@ref joysticks) to query.
		*  @param[out] count Where to store the number of axis values in the returned
		*  array.  This is set to zero if the joystick is not present or an error
		*  occurred.
		*  @return An array of axis values, or `NULL` if the joystick is not present or
		*  an [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM and @ref GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The returned array is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the specified joystick is
		*  disconnected or the library is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref joystick_axis
		*
		*  @since Added in version 3.0.  Replaces `glfwGetJoystickPos`.
		*
		*  @ingroup input**/
                 /*x1584*/
                 /// <summary>
                 /// Returns the values of all axes of the specified joystick.
                 /// </summary>
                 /// <param name="jid">The [joystick](@ref joysticks) to query.</param>
                 /// <param name="count">Where to store the number of axis values in the returned</param>
                 /// <returns>An array of axis values, or `NULL` if the joystick is not present or</returns>
        /*x1591*/
        [DllImport(LIB)]
        public static extern IntPtr/*float*/ glfwGetJoystickAxes(int jid, out int count/*x1592*/);
        /*x1593*/
        /*x1594*/
        /*x1595*//*/*
		/*! @brief Returns the state of all buttons of the specified joystick.
		*
		*  This function returns the state of all buttons of the specified joystick.
		*  Each element in the array is either `GLFW_PRESS` or `GLFW_RELEASE`.
		*
		*  For backward compatibility with earlier versions that did not have @ref
		*  glfwGetJoystickHats, the button array also includes all hats, each
		*  represented as four buttons.  The hats are in the same order as returned by
		*  __glfwGetJoystickHats__ and are in the order _up_, _right_, _down_ and
		*  _left_.  To disable these extra buttons, set the @ref
		*  GLFW_JOYSTICK_HAT_BUTTONS init hint before initialization.
		*
		*  If the specified joystick is not present this function will return `NULL`
		*  but will not generate an error.  This can be used instead of first calling
		*  @ref glfwJoystickPresent.
		*
		*  @param[in] jid The [joystick](@ref joysticks) to query.
		*  @param[out] count Where to store the number of button states in the returned
		*  array.  This is set to zero if the joystick is not present or an error
		*  occurred.
		*  @return An array of button states, or `NULL` if the joystick is not present
		*  or an [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM and @ref GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The returned array is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the specified joystick is
		*  disconnected or the library is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref joystick_button
		*
		*  @since Added in version 2.2.
		*  @glfw3 Changed to return a dynamic array.
		*
		*  @ingroup input**/
                 /*x1596*/
                 /// <summary>
                 /// Returns the state of all buttons of the specified joystick.
                 /// </summary>
                 /// <param name="jid">The [joystick](@ref joysticks) to query.</param>
                 /// <param name="count">Where to store the number of button states in the returned</param>
                 /// <returns>An array of button states, or `NULL` if the joystick is not present</returns>
        /*x1603*/
        [DllImport(LIB)]
        public static extern IntPtr/*unsigned char*/ glfwGetJoystickButtons(int jid, out int count/*x1604*/);
        /*x1605*/
        /*x1606*/
        /*x1607*//*/*
		/*! @brief Returns the state of all hats of the specified joystick.
		*
		*  This function returns the state of all hats of the specified joystick.
		*  Each element in the array is one of the following values:
		*
		*  Name                  | Value
		*  ----                  | -----
		*  `GLFW_HAT_CENTERED`   | 0
		*  `GLFW_HAT_UP`         | 1
		*  `GLFW_HAT_RIGHT`      | 2
		*  `GLFW_HAT_DOWN`       | 4
		*  `GLFW_HAT_LEFT`       | 8
		*  `GLFW_HAT_RIGHT_UP`   | `GLFW_HAT_RIGHT` \| `GLFW_HAT_UP`
		*  `GLFW_HAT_RIGHT_DOWN` | `GLFW_HAT_RIGHT` \| `GLFW_HAT_DOWN`
		*  `GLFW_HAT_LEFT_UP`    | `GLFW_HAT_LEFT` \| `GLFW_HAT_UP`
		*  `GLFW_HAT_LEFT_DOWN`  | `GLFW_HAT_LEFT` \| `GLFW_HAT_DOWN`
		*
		*  The diagonal directions are bitwise combinations of the primary (up, right,
		*  down and left) directions and you can test for these individually by ANDing
		*  it with the corresponding direction.
		*
		*  @code
		*  if (hats[2] & GLFW_HAT_RIGHT)
		*  {
		*      // State of hat 2 could be right-up, right or right-down
		*  }
		*  @endcode
		*
		*  If the specified joystick is not present this function will return `NULL`
		*  but will not generate an error.  This can be used instead of first calling
		*  @ref glfwJoystickPresent.
		*
		*  @param[in] jid The [joystick](@ref joysticks) to query.
		*  @param[out] count Where to store the number of hat states in the returned
		*  array.  This is set to zero if the joystick is not present or an error
		*  occurred.
		*  @return An array of hat states, or `NULL` if the joystick is not present
		*  or an [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM and @ref GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The returned array is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the specified joystick is
		*  disconnected, this function is called again for that joystick or the library
		*  is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref joystick_hat
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                 /*x1608*/
                 /// <summary>
                 /// Returns the state of all hats of the specified joystick.
                 /// </summary>
                 /// <param name="jid">The [joystick](@ref joysticks) to query.</param>
                 /// <param name="count">Where to store the number of hat states in the returned</param>
                 /// <returns>An array of hat states, or `NULL` if the joystick is not present</returns>
        /*x1615*/
        [DllImport(LIB)]
        public static extern IntPtr/*unsigned char*/ glfwGetJoystickHats(int jid, out int count/*x1616*/);
        /*x1617*/
        /*x1618*/
        /*x1619*//*/*
		/*! @brief Returns the name of the specified joystick.
		*
		*  This function returns the name, encoded as UTF-8, of the specified joystick.
		*  The returned string is allocated and freed by GLFW.  You should not free it
		*  yourself.
		*
		*  If the specified joystick is not present this function will return `NULL`
		*  but will not generate an error.  This can be used instead of first calling
		*  @ref glfwJoystickPresent.
		*
		*  @param[in] jid The [joystick](@ref joysticks) to query.
		*  @return The UTF-8 encoded name of the joystick, or `NULL` if the joystick
		*  is not present or an [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM and @ref GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The returned string is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the specified joystick is
		*  disconnected or the library is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref joystick_name
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup input**/
                 /*x1620*/
                 /// <summary>
                 /// Returns the name of the specified joystick.
                 /// </summary>
                 /// <param name="jid">The [joystick](@ref joysticks) to query.</param>
                 /// <returns>The UTF-8 encoded name of the joystick, or `NULL` if the joystick</returns>
        /*x1626*/
        [DllImport(LIB)]
        public static extern IntPtr/*char*/ glfwGetJoystickName(int jid/*x1627*/);
        /*x1628*/
        /*x1629*/
        /*x1630*//*/*
		/*! @brief Returns the SDL comaptible GUID of the specified joystick.
		*
		*  This function returns the SDL compatible GUID, as a UTF-8 encoded
		*  hexadecimal string, of the specified joystick.  The returned string is
		*  allocated and freed by GLFW.  You should not free it yourself.
		*
		*  The GUID is what connects a joystick to a gamepad mapping.  A connected
		*  joystick will always have a GUID even if there is no gamepad mapping
		*  assigned to it.
		*
		*  If the specified joystick is not present this function will return `NULL`
		*  but will not generate an error.  This can be used instead of first calling
		*  @ref glfwJoystickPresent.
		*
		*  The GUID uses the format introduced in SDL 2.0.5.  This GUID tries to
		*  uniquely identify the make and model of a joystick but does not identify
		*  a specific unit, e.g. all wired Xbox 360 controllers will have the same
		*  GUID on that platform.  The GUID for a unit may vary between platforms
		*  depending on what hardware information the platform specific APIs provide.
		*
		*  @param[in] jid The [joystick](@ref joysticks) to query.
		*  @return The UTF-8 encoded GUID of the joystick, or `NULL` if the joystick
		*  is not present or an [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_INVALID_ENUM and @ref GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The returned string is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the specified joystick is
		*  disconnected or the library is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref gamepad
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                 /*x1631*/
                 /// <summary>
                 /// Returns the SDL comaptible GUID of the specified joystick.
                 /// </summary>
                 /// <param name="jid">The [joystick](@ref joysticks) to query.</param>
                 /// <returns>The UTF-8 encoded GUID of the joystick, or `NULL` if the joystick</returns>
        /*x1637*/
        [DllImport(LIB)]
        public static extern IntPtr/*char*/ glfwGetJoystickGUID(int jid/*x1638*/);
        /*x1639*/
        /*x1640*/
        /*x1641*//*/*
		/*! @brief Sets the user pointer of the specified joystick.
		*
		*  This function sets the user-defined pointer of the specified joystick.  The
		*  current value is retained until the joystick is disconnected.  The initial
		*  value is `NULL`.
		*
		*  This function may be called from the joystick callback, even for a joystick
		*  that is being disconnected.
		*
		*  @param[in] jid The joystick whose pointer to set.
		*  @param[in] pointer The new value.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.  Access is not
		*  synchronized.
		*
		*  @sa @ref joystick_userptr
		*  @sa @ref glfwGetJoystickUserPointer
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                 /*x1642*/
                 /// <summary>
                 /// Sets the user pointer of the specified joystick.
                 /// </summary>
                 /// <param name="jid">The joystick whose pointer to set.</param>
                 /// <param name="pointer">The new value.</param>
        /*x1648*/
        [DllImport(LIB)]
        public static extern void glfwSetJoystickUserPointer(int jid, IntPtr/*void*/ pointer/*x1649*/);
        /*x1650*/
        /*x1651*/
        /*x1652*//*/*
		/*! @brief Returns the user pointer of the specified joystick.
		*
		*  This function returns the current value of the user-defined pointer of the
		*  specified joystick.  The initial value is `NULL`.
		*
		*  This function may be called from the joystick callback, even for a joystick
		*  that is being disconnected.
		*
		*  @param[in] jid The joystick whose pointer to return.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.  Access is not
		*  synchronized.
		*
		*  @sa @ref joystick_userptr
		*  @sa @ref glfwSetJoystickUserPointer
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                 /*x1653*/
                 /// <summary>
                 /// Returns the user pointer of the specified joystick.
                 /// </summary>
                 /// <param name="jid">The joystick whose pointer to return.</param>
        /*x1658*/
        [DllImport(LIB)]
        public static extern IntPtr/*void*/ glfwGetJoystickUserPointer(int jid/*x1659*/);
        /*x1660*/
        /*x1661*/
        /*x1662*//*/*
		/*! @brief Returns whether the specified joystick has a gamepad mapping.
		*
		*  This function returns whether the specified joystick is both present and has
		*  a gamepad mapping.
		*
		*  If the specified joystick is present but does not have a gamepad mapping
		*  this function will return `GLFW_FALSE` but will not generate an error.  Call
		*  @ref glfwJoystickPresent to check if a joystick is present regardless of
		*  whether it has a mapping.
		*
		*  @param[in] jid The [joystick](@ref joysticks) to query.
		*  @return `GLFW_TRUE` if a joystick is both present and has a gamepad mapping,
		*  or `GLFW_FALSE` otherwise.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_INVALID_ENUM.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref gamepad
		*  @sa @ref glfwGetGamepadState
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                 /*x1663*/
                 /// <summary>
                 /// Returns whether the specified joystick has a gamepad mapping.
                 /// </summary>
                 /// <param name="jid">The [joystick](@ref joysticks) to query.</param>
                 /// <returns>`GLFW_TRUE` if a joystick is both present and has a gamepad mapping,</returns>
        /*x1669*/
        [DllImport(LIB)]
        public static extern int glfwJoystickIsGamepad(int jid/*x1670*/);
        /*x1671*/
        /*x1672*/
        /*x1673*//*/*
		/*! @brief Sets the joystick configuration callback.
		*
		*  This function sets the joystick configuration callback, or removes the
		*  currently set callback.  This is called when a joystick is connected to or
		*  disconnected from the system.
		*
		*  For joystick connection and disconnection events to be delivered on all
		*  platforms, you need to call one of the [event processing](@ref events)
		*  functions.  Joystick disconnection may also be detected and the callback
		*  called by joystick functions.  The function will then return whatever it
		*  returns if the joystick is not present.
		*
		*  @param[in] cbfun The new callback, or `NULL` to remove the currently set
		*  callback.
		*  @return The previously set callback, or `NULL` if no callback was set or the
		*  library had not been [initialized](@ref intro_init).
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref joystick_event
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup input**/
                 /*x1674*/
                 /// <summary>
                 /// Sets the joystick configuration callback.
                 /// </summary>
                 /// <param name="cbfun">The new callback, or `NULL` to remove the currently set</param>
                 /// <returns>The previously set callback, or `NULL` if no callback was set or the</returns>
        /*x1680*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWjoystickfun*/ glfwSetJoystickCallback(IntPtr /*GLFWjoystickfun*/ cbfun/*x1681*/);
        /*x1682*/
        /*x1683*/
        /*x1684*//*/*
		/*! @brief Adds the specified SDL_GameControllerDB gamepad mappings.
		*
		*  This function parses the specified ASCII encoded string and updates the
		*  internal list with any gamepad mappings it finds.  This string may
		*  contain either a single gamepad mapping or many mappings separated by
		*  newlines.  The parser supports the full format of the `gamecontrollerdb.txt`
		*  source file including empty lines and comments.
		*
		*  See @ref gamepad_mapping for a description of the format.
		*
		*  If there is already a gamepad mapping for a given GUID in the internal list,
		*  it will be replaced by the one passed to this function.  If the library is
		*  terminated and re-initialized the internal list will revert to the built-in
		*  default.
		*
		*  @param[in] string The string containing the gamepad mappings.
		*  @return `GLFW_TRUE` if successful, or `GLFW_FALSE` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_INVALID_VALUE.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref gamepad
		*  @sa @ref glfwJoystickIsGamepad
		*  @sa @ref glfwGetGamepadName
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                 /*x1685*/
                 /// <summary>
                 /// Adds the specified SDL_GameControllerDB gamepad mappings.
                 /// </summary>
                 /// <param name="string">The string containing the gamepad mappings.</param>
                 /// <returns>`GLFW_TRUE` if successful, or `GLFW_FALSE` if an</returns>
        /*x1691*/
        [DllImport(LIB)]
        public static extern unsafe int glfwUpdateGamepadMappings(byte* string0/*x1692*/);
        /*x1693*/
        /*x1694*/
        /*x1695*/
        [DllImport(LIB)]
        public static extern int glfwUpdateGamepadMappings([MarshalAs(UnmanagedType.LPStr)] string string0/*x1696*/);
        /*x1697*/
        /*x1698*/
        /*x1699*//*/*
		/*! @brief Returns the human-readable gamepad name for the specified joystick.
		*
		*  This function returns the human-readable name of the gamepad from the
		*  gamepad mapping assigned to the specified joystick.
		*
		*  If the specified joystick is not present or does not have a gamepad mapping
		*  this function will return `NULL` but will not generate an error.  Call
		*  @ref glfwJoystickPresent to check whether it is present regardless of
		*  whether it has a mapping.
		*
		*  @param[in] jid The [joystick](@ref joysticks) to query.
		*  @return The UTF-8 encoded name of the gamepad, or `NULL` if the
		*  joystick is not present, does not have a mapping or an
		*  [error](@ref error_handling) occurred.
		*
		*  @pointer_lifetime The returned string is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the specified joystick is
		*  disconnected, the gamepad mappings are updated or the library is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref gamepad
		*  @sa @ref glfwJoystickIsGamepad
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                 /*x1700*/
                 /// <summary>
                 /// Returns the human-readable gamepad name for the specified joystick.
                 /// </summary>
                 /// <param name="jid">The [joystick](@ref joysticks) to query.</param>
                 /// <returns>The UTF-8 encoded name of the gamepad, or `NULL` if the</returns>
        /*x1706*/
        [DllImport(LIB)]
        public static extern IntPtr/*char*/ glfwGetGamepadName(int jid/*x1707*/);
        /*x1708*/
        /*x1709*/
        /*x1710*//*/*
		/*! @brief Retrieves the state of the specified joystick remapped as a gamepad.
		*
		*  This function retrives the state of the specified joystick remapped to
		*  an Xbox-like gamepad.
		*
		*  If the specified joystick is not present or does not have a gamepad mapping
		*  this function will return `GLFW_FALSE` but will not generate an error.  Call
		*  @ref glfwJoystickPresent to check whether it is present regardless of
		*  whether it has a mapping.
		*
		*  The Guide button may not be available for input as it is often hooked by the
		*  system or the Steam client.
		*
		*  Not all devices have all the buttons or axes provided by @ref
		*  GLFWgamepadstate.  Unavailable buttons and axes will always report
		*  `GLFW_RELEASE` and 0.0 respectively.
		*
		*  @param[in] jid The [joystick](@ref joysticks) to query.
		*  @param[out] state The gamepad input state of the joystick.
		*  @return `GLFW_TRUE` if successful, or `GLFW_FALSE` if no joystick is
		*  connected, it has no gamepad mapping or an [error](@ref error_handling)
		*  occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_INVALID_ENUM.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref gamepad
		*  @sa @ref glfwUpdateGamepadMappings
		*  @sa @ref glfwJoystickIsGamepad
		*
		*  @since Added in version 3.3.
		*
		*  @ingroup input**/
                 /*x1711*/
                 /// <summary>
                 /// Retrieves the state of the specified joystick remapped as a gamepad.
                 /// </summary>
                 /// <param name="jid">The [joystick](@ref joysticks) to query.</param>
                 /// <param name="state">The gamepad input state of the joystick.</param>
                 /// <returns>`GLFW_TRUE` if successful, or `GLFW_FALSE` if no joystick is</returns>
        /*x1718*/
        [DllImport(LIB)]
        public static extern int glfwGetGamepadState(int jid, IntPtr/*GLFWgamepadstate*/ state/*x1719*/);
        /*x1720*/
        /*x1721*/
        /*x1722*//*/*
		/*! @brief Sets the clipboard to the specified string.
		*
		*  This function sets the system clipboard to the specified, UTF-8 encoded
		*  string.
		*
		*  @param[in] window Deprecated.  Any valid window or `NULL`.
		*  @param[in] string A UTF-8 encoded string.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The specified string is copied before this function
		*  returns.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref clipboard
		*  @sa @ref glfwGetClipboardString
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup input**/
                 /*x1723*/
                 /// <summary>
                 /// Sets the clipboard to the specified string.
                 /// </summary>
                 /// <param name="window">Deprecated.  Any valid window or `NULL`.</param>
                 /// <param name="string">A UTF-8 encoded string.</param>
        /*x1729*/
        [DllImport(LIB)]
        public static extern unsafe void glfwSetClipboardString(IntPtr/*GLFWwindow*/ window, byte* string0/*x1730*/);
        /*x1731*/
        /*x1732*/
        /*x1733*/
        [DllImport(LIB)]
        public static extern void glfwSetClipboardString(IntPtr/*GLFWwindow*/ window, [MarshalAs(UnmanagedType.LPStr)] string string0/*x1734*/);
        /*x1735*/
        /*x1736*/
        /*x1737*//*/*
		/*! @brief Returns the contents of the clipboard as a string.
		*
		*  This function returns the contents of the system clipboard, if it contains
		*  or is convertible to a UTF-8 encoded string.  If the clipboard is empty or
		*  if its contents cannot be converted, `NULL` is returned and a @ref
		*  GLFW_FORMAT_UNAVAILABLE error is generated.
		*
		*  @param[in] window Deprecated.  Any valid window or `NULL`.
		*  @return The contents of the clipboard as a UTF-8 encoded string, or `NULL`
		*  if an [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @pointer_lifetime The returned string is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is valid until the next call to @ref
		*  glfwGetClipboardString or @ref glfwSetClipboardString, or until the library
		*  is terminated.
		*
		*  @thread_safety This function must only be called from the main thread.
		*
		*  @sa @ref clipboard
		*  @sa @ref glfwSetClipboardString
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup input**/
                 /*x1738*/
                 /// <summary>
                 /// Returns the contents of the clipboard as a string.
                 /// </summary>
                 /// <param name="window">Deprecated.  Any valid window or `NULL`.</param>
                 /// <returns>The contents of the clipboard as a UTF-8 encoded string, or `NULL`</returns>
        /*x1744*/
        [DllImport(LIB)]
        public static extern IntPtr/*char*/ glfwGetClipboardString(IntPtr/*GLFWwindow*/ window/*x1745*/);
        /*x1746*/
        /*x1747*/
        /*x1748*//*/*
		/*! @brief Returns the value of the GLFW timer.
		*
		*  This function returns the value of the GLFW timer.  Unless the timer has
		*  been set using @ref glfwSetTime, the timer measures time elapsed since GLFW
		*  was initialized.
		*
		*  The resolution of the timer is system dependent, but is usually on the order
		*  of a few micro- or nanoseconds.  It uses the highest-resolution monotonic
		*  time source on each supported platform.
		*
		*  @return The current value, in seconds, or zero if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.  Reading and
		*  writing of the internal timer offset is not atomic, so it needs to be
		*  externally synchronized with calls to @ref glfwSetTime.
		*
		*  @sa @ref time
		*
		*  @since Added in version 1.0.
		*
		*  @ingroup input**/
                 /*x1749*/
                 /// <summary>
                 /// Returns the value of the GLFW timer.
                 /// </summary>
                 /// <returns>The current value, in seconds, or zero if an</returns>
        /*x1754*/
        [DllImport(LIB)]
        public static extern IntPtr/*double*/ glfwGetTime(/*x1755*/);
        /*x1756*/
        /*x1757*/
        /*x1758*//*/*
		/*! @brief Sets the GLFW timer.
		*
		*  This function sets the value of the GLFW timer.  It then continues to count
		*  up from that value.  The value must be a positive finite number less than
		*  or equal to 18446744073.0, which is approximately 584.5 years.
		*
		*  @param[in] time The new value, in seconds.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_INVALID_VALUE.
		*
		*  @remark The upper limit of the timer is calculated as
		*  floor((2<sup>64</sup> - 1) / 10<sup>9</sup>) and is due to implementations
		*  storing nanoseconds in 64 bits.  The limit may be increased in the future.
		*
		*  @thread_safety This function may be called from any thread.  Reading and
		*  writing of the internal timer offset is not atomic, so it needs to be
		*  externally synchronized with calls to @ref glfwGetTime.
		*
		*  @sa @ref time
		*
		*  @since Added in version 2.2.
		*
		*  @ingroup input**/
                 /*x1759*/
                 /// <summary>
                 /// Sets the GLFW timer.
                 /// </summary>
                 /// <param name="time">The new value, in seconds.</param>
        /*x1764*/
        [DllImport(LIB)]
        public static extern void glfwSetTime(double time/*x1765*/);
        /*x1766*/
        /*x1767*/
        /*x1768*//*/*
		/*! @brief Returns the current value of the raw timer.
		*
		*  This function returns the current value of the raw timer, measured in
		*  1&nbsp;/&nbsp;frequency seconds.  To get the frequency, call @ref
		*  glfwGetTimerFrequency.
		*
		*  @return The value of the timer, or zero if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref time
		*  @sa @ref glfwGetTimerFrequency
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup input**/
                 /*x1769*/
                 /// <summary>
                 /// Returns the current value of the raw timer.
                 /// </summary>
                 /// <returns>The value of the timer, or zero if an</returns>
        /*x1774*/
        [DllImport(LIB)]
        public static extern IntPtr/*uint64_t*/ glfwGetTimerValue(/*x1775*/);
        /*x1776*/
        /*x1777*/
        /*x1778*//*/*
		/*! @brief Returns the frequency, in Hz, of the raw timer.
		*
		*  This function returns the frequency, in Hz, of the raw timer.
		*
		*  @return The frequency of the timer, in Hz, or zero if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref time
		*  @sa @ref glfwGetTimerValue
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup input**/
                 /*x1779*/
                 /// <summary>
                 /// Returns the frequency, in Hz, of the raw timer.
                 /// </summary>
                 /// <returns>The frequency of the timer, in Hz, or zero if an</returns>
        /*x1784*/
        [DllImport(LIB)]
        public static extern IntPtr/*uint64_t*/ glfwGetTimerFrequency(/*x1785*/);
        /*x1786*/
        /*x1787*/
        /*x1788*//*/*
		/*! @brief Makes the context of the specified window current for the calling
		*  thread.
		*
		*  This function makes the OpenGL or OpenGL ES context of the specified window
		*  current on the calling thread.  A context must only be made current on
		*  a single thread at a time and each thread can have only a single current
		*  context at a time.
		*
		*  When moving a context between threads, you must make it non-current on the
		*  old thread before making it current on the new one.
		*
		*  By default, making a context non-current implicitly forces a pipeline flush.
		*  On machines that support `GL_KHR_context_flush_control`, you can control
		*  whether a context performs this flush by setting the
		*  [GLFW_CONTEXT_RELEASE_BEHAVIOR](@ref GLFW_CONTEXT_RELEASE_BEHAVIOR_hint)
		*  hint.
		*
		*  The specified window must have an OpenGL or OpenGL ES context.  Specifying
		*  a window without a context will generate a @ref GLFW_NO_WINDOW_CONTEXT
		*  error.
		*
		*  @param[in] window The window whose context to make current, or `NULL` to
		*  detach the current context.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_NO_WINDOW_CONTEXT and @ref GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref context_current
		*  @sa @ref glfwGetCurrentContext
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup context**/
                 /*x1789*/
                 /// <summary>
                 /// Makes the context of the specified window current for the calling
                 /// </summary>
                 /// <param name="window">The window whose context to make current, or `NULL` to</param>
        /*x1794*/
        [DllImport(LIB)]
        public static extern void glfwMakeContextCurrent(IntPtr/*GLFWwindow*/ window/*x1795*/);
        /*x1796*/
        /*x1797*/
        /*x1798*//*/*
		/*! @brief Returns the window whose context is current on the calling thread.
		*
		*  This function returns the window whose OpenGL or OpenGL ES context is
		*  current on the calling thread.
		*
		*  @return The window whose context is current, or `NULL` if no window's
		*  context is current.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref context_current
		*  @sa @ref glfwMakeContextCurrent
		*
		*  @since Added in version 3.0.
		*
		*  @ingroup context**/
                 /*x1799*/
                 /// <summary>
                 /// Returns the window whose context is current on the calling thread.
                 /// </summary>
                 /// <returns>The window whose context is current, or `NULL` if no window's</returns>
        /*x1804*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWwindow*/ glfwGetCurrentContext(/*x1805*/);
        /*x1806*/
        /*x1807*/
        /*x1808*//*/*
		/*! @brief Swaps the front and back buffers of the specified window.
		*
		*  This function swaps the front and back buffers of the specified window when
		*  rendering with OpenGL or OpenGL ES.  If the swap interval is greater than
		*  zero, the GPU driver waits the specified number of screen updates before
		*  swapping the buffers.
		*
		*  The specified window must have an OpenGL or OpenGL ES context.  Specifying
		*  a window without a context will generate a @ref GLFW_NO_WINDOW_CONTEXT
		*  error.
		*
		*  This function does not apply to Vulkan.  If you are rendering with Vulkan,
		*  see `vkQueuePresentKHR` instead.
		*
		*  @param[in] window The window whose buffers to swap.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_NO_WINDOW_CONTEXT and @ref GLFW_PLATFORM_ERROR.
		*
		*  @remark __EGL:__ The context of the specified window must be current on the
		*  calling thread.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref buffer_swap
		*  @sa @ref glfwSwapInterval
		*
		*  @since Added in version 1.0.
		*  @glfw3 Added window handle parameter.
		*
		*  @ingroup window**/
                 /*x1809*/
                 /// <summary>
                 /// Swaps the front and back buffers of the specified window.
                 /// </summary>
                 /// <param name="window">The window whose buffers to swap.</param>
        /*x1814*/
        [DllImport(LIB)]
        public static extern void glfwSwapBuffers(IntPtr/*GLFWwindow*/ window/*x1815*/);
        /*x1816*/
        /*x1817*/
        /*x1818*//*/*
		/*! @brief Sets the swap interval for the current context.
		*
		*  This function sets the swap interval for the current OpenGL or OpenGL ES
		*  context, i.e. the number of screen updates to wait from the time @ref
		*  glfwSwapBuffers was called before swapping the buffers and returning.  This
		*  is sometimes called _vertical synchronization_, _vertical retrace
		*  synchronization_ or just _vsync_.
		*
		*  A context that supports either of the `WGL_EXT_swap_control_tear` and
		*  `GLX_EXT_swap_control_tear` extensions also accepts _negative_ swap
		*  intervals, which allows the driver to swap immediately even if a frame
		*  arrives a little bit late.  You can check for these extensions with @ref
		*  glfwExtensionSupported.
		*
		*  A context must be current on the calling thread.  Calling this function
		*  without a current context will cause a @ref GLFW_NO_CURRENT_CONTEXT error.
		*
		*  This function does not apply to Vulkan.  If you are rendering with Vulkan,
		*  see the present mode of your swapchain instead.
		*
		*  @param[in] interval The minimum number of screen updates to wait for
		*  until the buffers are swapped by @ref glfwSwapBuffers.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_NO_CURRENT_CONTEXT and @ref GLFW_PLATFORM_ERROR.
		*
		*  @remark This function is not called during context creation, leaving the
		*  swap interval set to whatever is the default on that platform.  This is done
		*  because some swap interval extensions used by GLFW do not allow the swap
		*  interval to be reset to zero once it has been set to a non-zero value.
		*
		*  @remark Some GPU drivers do not honor the requested swap interval, either
		*  because of a user setting that overrides the application's request or due to
		*  bugs in the driver.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref buffer_swap
		*  @sa @ref glfwSwapBuffers
		*
		*  @since Added in version 1.0.
		*
		*  @ingroup context**/
                 /*x1819*/
                 /// <summary>
                 /// Sets the swap interval for the current context.
                 /// </summary>
                 /// <param name="interval">The minimum number of screen updates to wait for</param>
        /*x1824*/
        [DllImport(LIB)]
        public static extern void glfwSwapInterval(int interval/*x1825*/);
        /*x1826*/
        /*x1827*/
        /*x1828*//*/*
		/*! @brief Returns whether the specified extension is available.
		*
		*  This function returns whether the specified
		*  [API extension](@ref context_glext) is supported by the current OpenGL or
		*  OpenGL ES context.  It searches both for client API extension and context
		*  creation API extensions.
		*
		*  A context must be current on the calling thread.  Calling this function
		*  without a current context will cause a @ref GLFW_NO_CURRENT_CONTEXT error.
		*
		*  As this functions retrieves and searches one or more extension strings each
		*  call, it is recommended that you cache its results if it is going to be used
		*  frequently.  The extension strings will not change during the lifetime of
		*  a context, so there is no danger in doing this.
		*
		*  This function does not apply to Vulkan.  If you are using Vulkan, see @ref
		*  glfwGetRequiredInstanceExtensions, `vkEnumerateInstanceExtensionProperties`
		*  and `vkEnumerateDeviceExtensionProperties` instead.
		*
		*  @param[in] extension The ASCII encoded name of the extension.
		*  @return `GLFW_TRUE` if the extension is available, or `GLFW_FALSE`
		*  otherwise.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_NO_CURRENT_CONTEXT, @ref GLFW_INVALID_VALUE and @ref
		*  GLFW_PLATFORM_ERROR.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref context_glext
		*  @sa @ref glfwGetProcAddress
		*
		*  @since Added in version 1.0.
		*
		*  @ingroup context**/
                 /*x1829*/
                 /// <summary>
                 /// Returns whether the specified extension is available.
                 /// </summary>
                 /// <param name="extension">The ASCII encoded name of the extension.</param>
                 /// <returns>`GLFW_TRUE` if the extension is available, or `GLFW_FALSE`</returns>
        /*x1835*/
        [DllImport(LIB)]
        public static extern unsafe int glfwExtensionSupported(byte* extension/*x1836*/);
        /*x1837*/
        /*x1838*/
        /*x1839*/
        [DllImport(LIB)]
        public static extern int glfwExtensionSupported([MarshalAs(UnmanagedType.LPStr)] string extension/*x1840*/);
        /*x1841*/
        /*x1842*/
        /*x1843*//*/*
		/*! @brief Returns the address of the specified function for the current
		*  context.
		*
		*  This function returns the address of the specified OpenGL or OpenGL ES
		*  [core or extension function](@ref context_glext), if it is supported
		*  by the current context.
		*
		*  A context must be current on the calling thread.  Calling this function
		*  without a current context will cause a @ref GLFW_NO_CURRENT_CONTEXT error.
		*
		*  This function does not apply to Vulkan.  If you are rendering with Vulkan,
		*  see @ref glfwGetInstanceProcAddress, `vkGetInstanceProcAddr` and
		*  `vkGetDeviceProcAddr` instead.
		*
		*  @param[in] procname The ASCII encoded name of the function.
		*  @return The address of the function, or `NULL` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_NO_CURRENT_CONTEXT and @ref GLFW_PLATFORM_ERROR.
		*
		*  @remark The address of a given function is not guaranteed to be the same
		*  between contexts.
		*
		*  @remark This function may return a non-`NULL` address despite the
		*  associated version or extension not being available.  Always check the
		*  context version or extension string first.
		*
		*  @pointer_lifetime The returned function pointer is valid until the context
		*  is destroyed or the library is terminated.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref context_glext
		*  @sa @ref glfwExtensionSupported
		*
		*  @since Added in version 1.0.
		*
		*  @ingroup context**/
                 /*x1844*/
                 /// <summary>
                 /// Returns the address of the specified function for the current
                 /// </summary>
                 /// <param name="procname">The ASCII encoded name of the function.</param>
                 /// <returns>The address of the function, or `NULL` if an</returns>
        /*x1850*/
        [DllImport(LIB)]
        public static extern unsafe IntPtr/*GLFWglproc*/ glfwGetProcAddress(byte* procname/*x1851*/);
        /*x1852*/
        /*x1853*/
        /*x1854*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWglproc*/ glfwGetProcAddress([MarshalAs(UnmanagedType.LPStr)] string procname/*x1855*/);
        /*x1856*/
        /*x1857*/
        /*x1858*//*/*
		/*! @brief Returns whether the Vulkan loader and an ICD have been found.
		*
		*  This function returns whether the Vulkan loader and any minimally functional
		*  ICD have been found.
		*
		*  The availability of a Vulkan loader and even an ICD does not by itself
		*  guarantee that surface creation or even instance creation is possible.
		*  For example, on Fermi systems Nvidia will install an ICD that provides no
		*  actual Vulkan support.  Call @ref glfwGetRequiredInstanceExtensions to check
		*  whether the extensions necessary for Vulkan surface creation are available
		*  and @ref glfwGetPhysicalDevicePresentationSupport to check whether a queue
		*  family of a physical device supports image presentation.
		*
		*  @return `GLFW_TRUE` if Vulkan is minimally available, or `GLFW_FALSE`
		*  otherwise.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref vulkan_support
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup vulkan**/
                 /*x1859*/
                 /// <summary>
                 /// Returns whether the Vulkan loader and an ICD have been found.
                 /// </summary>
                 /// <returns>`GLFW_TRUE` if Vulkan is minimally available, or `GLFW_FALSE`</returns>
        /*x1864*/
        [DllImport(LIB)]
        public static extern int glfwVulkanSupported(/*x1865*/);
        /*x1866*/
        /*x1867*/
        /*x1868*//*/*
		/*! @brief Returns the Vulkan instance extensions required by GLFW.
		*
		*  This function returns an array of names of Vulkan instance extensions required
		*  by GLFW for creating Vulkan surfaces for GLFW windows.  If successful, the
		*  list will always contains `VK_KHR_surface`, so if you don't require any
		*  additional extensions you can pass this list directly to the
		*  `VkInstanceCreateInfo` struct.
		*
		*  If Vulkan is not available on the machine, this function returns `NULL` and
		*  generates a @ref GLFW_API_UNAVAILABLE error.  Call @ref glfwVulkanSupported
		*  to check whether Vulkan is at least minimally available.
		*
		*  If Vulkan is available but no set of extensions allowing window surface
		*  creation was found, this function returns `NULL`.  You may still use Vulkan
		*  for off-screen rendering and compute work.
		*
		*  @param[out] count Where to store the number of extensions in the returned
		*  array.  This is set to zero if an error occurred.
		*  @return An array of ASCII encoded extension names, or `NULL` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_API_UNAVAILABLE.
		*
		*  @remark Additional extensions may be required by future versions of GLFW.
		*  You should check if any extensions you wish to enable are already in the
		*  returned array, as it is an error to specify an extension more than once in
		*  the `VkInstanceCreateInfo` struct.
		*
		*  @remark @macos This function currently only supports the
		*  `VK_MVK_macos_surface` extension from MoltenVK.
		*
		*  @pointer_lifetime The returned array is allocated and freed by GLFW.  You
		*  should not free it yourself.  It is guaranteed to be valid only until the
		*  library is terminated.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref vulkan_ext
		*  @sa @ref glfwCreateWindowSurface
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup vulkan**/
                 /*x1869*/
                 /// <summary>
                 /// Returns the Vulkan instance extensions required by GLFW.
                 /// </summary>
                 /// <param name="count">Where to store the number of extensions in the returned</param>
                 /// <returns>An array of ASCII encoded extension names, or `NULL` if an</returns>
        /*x1875*/
        [DllImport(LIB)]
        public static extern char[] glfwGetRequiredInstanceExtensions(IntPtr/*uint32_t*/ count/*x1876*/);
        /*x1877*/
        /*x1878*/
        /*x1879*//*/*
		/*! @brief Returns the address of the specified Vulkan instance function.
		*
		*  This function returns the address of the specified Vulkan core or extension
		*  function for the specified instance.  If instance is set to `NULL` it can
		*  return any function exported from the Vulkan loader, including at least the
		*  following functions:
		*
		*  - `vkEnumerateInstanceExtensionProperties`
		*  - `vkEnumerateInstanceLayerProperties`
		*  - `vkCreateInstance`
		*  - `vkGetInstanceProcAddr`
		*
		*  If Vulkan is not available on the machine, this function returns `NULL` and
		*  generates a @ref GLFW_API_UNAVAILABLE error.  Call @ref glfwVulkanSupported
		*  to check whether Vulkan is at least minimally available.
		*
		*  This function is equivalent to calling `vkGetInstanceProcAddr` with
		*  a platform-specific query of the Vulkan loader as a fallback.
		*
		*  @param[in] instance The Vulkan instance to query, or `NULL` to retrieve
		*  functions related to instance creation.
		*  @param[in] procname The ASCII encoded name of the function.
		*  @return The address of the function, or `NULL` if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED and @ref
		*  GLFW_API_UNAVAILABLE.
		*
		*  @pointer_lifetime The returned function pointer is valid until the library
		*  is terminated.
		*
		*  @thread_safety This function may be called from any thread.
		*
		*  @sa @ref vulkan_proc
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup vulkan**/
                 /*x1880*/
                 /// <summary>
                 /// Returns the address of the specified Vulkan instance function.
                 /// </summary>
                 /// <param name="instance">The Vulkan instance to query, or `NULL` to retrieve</param>
                 /// <param name="procname">The ASCII encoded name of the function.</param>
                 /// <returns>The address of the function, or `NULL` if an</returns>
        /*x1887*/
        [DllImport(LIB)]
        public static extern unsafe IntPtr/*GLFWvkproc*/ glfwGetInstanceProcAddress(IntPtr /*VkInstance*/ instance, byte* procname/*x1888*/);
        /*x1889*/
        /*x1890*/
        /*x1891*/
        [DllImport(LIB)]
        public static extern IntPtr/*GLFWvkproc*/ glfwGetInstanceProcAddress(IntPtr /*VkInstance*/ instance, [MarshalAs(UnmanagedType.LPStr)] string procname/*x1892*/);
        /*x1893*/
        /*x1894*/
        /*x1895*//*/*
		/*! @brief Returns whether the specified queue family can present images.
		*
		*  This function returns whether the specified queue family of the specified
		*  physical device supports presentation to the platform GLFW was built for.
		*
		*  If Vulkan or the required window surface creation instance extensions are
		*  not available on the machine, or if the specified instance was not created
		*  with the required extensions, this function returns `GLFW_FALSE` and
		*  generates a @ref GLFW_API_UNAVAILABLE error.  Call @ref glfwVulkanSupported
		*  to check whether Vulkan is at least minimally available and @ref
		*  glfwGetRequiredInstanceExtensions to check what instance extensions are
		*  required.
		*
		*  @param[in] instance The instance that the physical device belongs to.
		*  @param[in] device The physical device that the queue family belongs to.
		*  @param[in] queuefamily The index of the queue family to query.
		*  @return `GLFW_TRUE` if the queue family supports presentation, or
		*  `GLFW_FALSE` otherwise.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_API_UNAVAILABLE and @ref GLFW_PLATFORM_ERROR.
		*
		*  @remark @macos This function currently always returns `GLFW_TRUE`, as the
		*  `VK_MVK_macos_surface` extension does not provide
		*  a `vkGetPhysicalDevice*PresentationSupport` type function.
		*
		*  @thread_safety This function may be called from any thread.  For
		*  synchronization details of Vulkan objects, see the Vulkan specification.
		*
		*  @sa @ref vulkan_present
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup vulkan**/
                 /*x1896*/
                 /// <summary>
                 /// Returns whether the specified queue family can present images.
                 /// </summary>
                 /// <param name="instance">The instance that the physical device belongs to.</param>
                 /// <param name="device">The physical device that the queue family belongs to.</param>
                 /// <param name="queuefamily">The index of the queue family to query.</param>
                 /// <returns>`GLFW_TRUE` if the queue family supports presentation, or</returns>
        /*x1904*/
        [DllImport(LIB)]
        public static extern int glfwGetPhysicalDevicePresentationSupport(IntPtr /*VkInstance*/ instance, IntPtr /*VkPhysicalDevice*/ device, IntPtr /*uint32_t*/ queuefamily/*x1905*/);
        /*x1906*/
        /*x1907*/
        /*x1908*//*/*
		/*! @brief Creates a Vulkan surface for the specified window.
		*
		*  This function creates a Vulkan surface for the specified window.
		*
		*  If the Vulkan loader or at least one minimally functional ICD were not found,
		*  this function returns `VK_ERROR_INITIALIZATION_FAILED` and generates a @ref
		*  GLFW_API_UNAVAILABLE error.  Call @ref glfwVulkanSupported to check whether
		*  Vulkan is at least minimally available.
		*
		*  If the required window surface creation instance extensions are not
		*  available or if the specified instance was not created with these extensions
		*  enabled, this function returns `VK_ERROR_EXTENSION_NOT_PRESENT` and
		*  generates a @ref GLFW_API_UNAVAILABLE error.  Call @ref
		*  glfwGetRequiredInstanceExtensions to check what instance extensions are
		*  required.
		*
		*  The window surface cannot be shared with another API so the window must
		*  have been created with the [client api hint](@ref GLFW_CLIENT_API_attrib)
		*  set to `GLFW_NO_API` otherwise it generates a @ref GLFW_INVALID_VALUE error
		*  and returns `VK_ERROR_NATIVE_WINDOW_IN_USE_KHR`.
		*
		*  The window surface must be destroyed before the specified Vulkan instance.
		*  It is the responsibility of the caller to destroy the window surface.  GLFW
		*  does not destroy it for you.  Call `vkDestroySurfaceKHR` to destroy the
		*  surface.
		*
		*  @param[in] instance The Vulkan instance to create the surface in.
		*  @param[in] window The window to create the surface for.
		*  @param[in] allocator The allocator to use, or `NULL` to use the default
		*  allocator.
		*  @param[out] surface Where to store the handle of the surface.  This is set
		*  to `VK_NULL_HANDLE` if an error occurred.
		*  @return `VK_SUCCESS` if successful, or a Vulkan error code if an
		*  [error](@ref error_handling) occurred.
		*
		*  @errors Possible errors include @ref GLFW_NOT_INITIALIZED, @ref
		*  GLFW_API_UNAVAILABLE, @ref GLFW_PLATFORM_ERROR and @ref GLFW_INVALID_VALUE
		*
		*  @remark If an error occurs before the creation call is made, GLFW returns
		*  the Vulkan error code most appropriate for the error.  Appropriate use of
		*  @ref glfwVulkanSupported and @ref glfwGetRequiredInstanceExtensions should
		*  eliminate almost all occurrences of these errors.
		*
		*  @remark @macos This function currently only supports the
		*  `VK_MVK_macos_surface` extension from MoltenVK.
		*
		*  @remark @macos This function creates and sets a `CAMetalLayer` instance for
		*  the window content view, which is required for MoltenVK to function.
		*
		*  @thread_safety This function may be called from any thread.  For
		*  synchronization details of Vulkan objects, see the Vulkan specification.
		*
		*  @sa @ref vulkan_surface
		*  @sa @ref glfwGetRequiredInstanceExtensions
		*
		*  @since Added in version 3.2.
		*
		*  @ingroup vulkan**/
                 /*x1909*/
                 /// <summary>
                 /// Creates a Vulkan surface for the specified window.
                 /// </summary>
                 /// <param name="instance">The Vulkan instance to create the surface in.</param>
                 /// <param name="window">The window to create the surface for.</param>
                 /// <param name="allocator">The allocator to use, or `NULL` to use the default</param>
                 /// <param name="surface">Where to store the handle of the surface.  This is set</param>
                 /// <returns>`VK_SUCCESS` if successful, or a Vulkan error code if an</returns>
        /*x1918*/
        [DllImport(LIB)]
        public static extern IntPtr/*VkResult*/ glfwCreateWindowSurface(IntPtr /*VkInstance*/ instance, IntPtr/*GLFWwindow*/ window, IntPtr/*VkAllocationCallbacks*/ allocator, IntPtr/*VkSurfaceKHR*/ surface/*x1919*/);
        /*x1920*/
        /*x1921*/
        /*x1922*/
    }
    /*x1923*/
}
