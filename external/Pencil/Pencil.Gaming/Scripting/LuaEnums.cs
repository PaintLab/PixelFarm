using System;

namespace Pencil.Gaming.Scripting {
	public enum ThreadStatus {
		Ok = 0,
		Yield,
		ErrRun,
		ErrSyntax,
		ErrMem,
		ErrGCMM,
		ErrErr,
		ErrFile,
	}

	public enum BasicType {
		None = -1,
		Nil,
		Boolean,
		LightUserData,
		Number,
		String,
		Table,
		Function,
		UserData,
		Thread,
	}

	public enum ArithmeticOp {
		Add = 0,
		Sub,
		Mul,
		Div,
		Mod,
		Pow,
		UNM,
	}

	public enum CompareOp {
		Eq = 0,
		Lt,
		LE,
	}

	public enum GCOption {
		Stop = 0,
		Restart,
		Collect,
		Count,
		Countb,
		Step,
		SetPause,
		SetStepMul,
		SetMajorInc,
		IsRunning,
		Gen,
		Inc,
	}

	public enum EventCode {
		HookCall = 0,
		HookRet,
		HookLine,
		HookCount,
		HookTailCall,
	}
}

