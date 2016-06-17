#region License
// Copyright (c) 2013 Antonie Blom
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

#if USE_GLFW2
using System;

namespace Pencil.Gaming {
	public enum Keystate {
		Release = 0,
		Press = 1,
	}

	public enum Key {
		Unknown = -1,
		Space = 32,
		Special = 256,
		Escape,
		F1,
		F2,
		F3,
		F4,
		F5,
		F6,
		F7,
		F8,
		F9,
		F10,
		F11,
		F12,
		F13,
		F14,
		F15,
		F16,
		F17,
		F18,
		F19,
		F20,
		F21,
		F22,
		F23,
		F24,
		F25,
		Up,
		Down,
		Left,
		Right,
		LeftShift,
		RightShift,
		LeftControl,
		RightControl,
		LeftAlt,
		RightAlt,
		Tab,
		Enter,
		Backspace,
		Insert,
		Delete,
		PageUp,
		PageDown,
		Home,
		End,
		KP0,
		KP1,
		KP2,
		KP3,
		KP4,
		KP5,
		KP7,
		KP8,
		KP9,
		Divide,
		Multiply,
		Subtract,
		Add,
		Decimal,
		Equal,
		KPEnter,
		NumLock,
		CapsLock,
		ScrollLock,
		Pause,
		LeftSuper,
		RightSuper,
		Menu,
	}

	public enum MouseButton {
		Button1 = 0,
		Button2,
		Button3,
		Button4,
		Button5,
		Button6,
		Button7,
		Button8,
		LeftButton = 0,
		RightButton,
		MiddleButton,
	}

	public enum Joystick {
		Joystick1 = 0,
		Joystick2,
		Joystick3,
		Joystick4,
		Joystick5,
		Joystick6,
		Joystick7,
		Joystick8,
		Joystick9,
		Joystick10,
		Joystick11,
		Joystick12,
		Joystick13,
		Joystick14,
		Joystick15,
		Joystick16,
	}

	public enum WindowMode {
		Window = 0x00010001,
		FullScreen,
	}

	public enum WindowParam {
		Opened = 0x00020001,
		Active,
		Iconified,
		Accelerated,
		RedBits,
		GreenBits,
		BlueBits,
		AlphaBits,
		DepthBits,
		StencilBits,
	}

	public enum OpenWindowHint {
		RefreshRate = 0x0002000B,
		AccumRedBits,
		AccumGreenBits,
		AccumBlueBits,
		AccumAlphaBits,
		AuxBuffers,
		Stereo,
		NoResize,
		FSAASamples,
		OpenGLMajorVersion,
		OpenGLMinorVersion,
		OpenGLForwardCompatibility,
		OpenGLDebugContext,
		OpenGLProfile,
	}

	public enum OpenGLProfile {
		CoreProfile = 0x00050001,
		CompatibilityProfile,
	}

	public enum GlfwEnableCap {
		MouseCursor = 0x00030001,
		StickyKeys,
		StickyMouseButtons,
		SystemKeys,
		KeyRepeat,
		AutoPollEvents,
	}

	public enum ThreadWait {
		Wait = 0x00040001,
		NoWait,
	}

	public enum JoystickParam {
		Present = 0x00050001,
		Axes,
		Buttons,
	}

	[Flags]
	public enum ReadImageModes {
		NoRescaleBit = 0x00000001,
		OriginULBit = 0x00000002,
		BuildMipmaps = 0x00000004,
		AlphaMapBit = 0x00000008
	}
}

#endif