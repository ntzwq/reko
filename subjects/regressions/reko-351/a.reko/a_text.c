// a_text.c
// Generated by decompiling a.out
// using Reko decompiler version 0.9.1.0.

#include "a_text.h"

// 80000080: Register word32 deregister_tm_clones()
// Called from:
//      __do_global_dtors_aux
word32 deregister_tm_clones()
{
	word32 * a6_n = fp - 0x04;
	if (false && 0x00 != 0x00)
	{
		word32 a0_n;
		null();
	}
	return *a6_n;
}

// 800000AE: void register_tm_clones()
// Called from:
//      frame_dummy
void register_tm_clones()
{
	int32 d0_n = 0;
	if (true)
		d0_n = 0x01;
	int32 d0_n = d0_n >> 0x01;
	if (d0_n != 0x00 && 0x00 != 0x00)
	{
		word32 a0_n;
		null();
	}
}

// 800000E4: void __do_global_dtors_aux()
void __do_global_dtors_aux()
{
	ptr32 a6_n = fp - 0x04;
	if (g_b80002724 == 0x00)
	{
		uint32 d0_n = g_dw80002726;
		<anonymous> * a2_n[] = g_a80002714;
		up32 d2_n = 0x00;
		if (d0_n < 0x00)
		{
			do
			{
				uint32 d0_n = d0_n + 0x01;
				g_dw80002726 = d0_n;
				word32 a0_n;
				word32 d1_n;
				a2_n[d0_n]();
				d0_n = g_dw80002726;
			} while (d2_n - d0_n > 0x00);
		}
		word32 a6_n = deregister_tm_clones();
		struct Eq_n * a7_n = (struct Eq_n *) <invalid>;
		if (0x00 != 0x00)
		{
			a7_n->ptrFFFFFFFC = 0x8000065C;
			word32 d0_n;
			word32 a0_n;
			word32 d1_n;
			null();
		}
		g_b80002724 = 0x01;
	}
}

// 80000158: void call___do_global_dtors_aux()
void call___do_global_dtors_aux()
{
}

// 80000160: void frame_dummy()
void frame_dummy()
{
	ptr32 a6_n = fp - 0x04;
	struct Eq_n * a7_n = fp - 0x04;
	if (0x00 != 0x00)
	{
		word32 a7_n;
		word32 a1_n;
		null();
		a7_n = a7_n + 8;
	}
	if (g_dw8000271C != 0x00 && 0x00 != 0x00)
	{
		a7_n->ptrFFFFFFFC = 0x8000271C;
		word32 a1_n;
		null();
		register_tm_clones();
	}
	else
		register_tm_clones();
}

// 800001A4: void call_frame_dummy()
void call_frame_dummy()
{
}

// 800001AC: void sine_taylor(Stack real64 rArg04)
// Called from:
//      main
void sine_taylor(real64 rArg04)
{
}

// 8000033C: void factorial(Stack int32 dwArg04)
// Called from:
//      sine_taylor
void factorial(int32 dwArg04)
{
	int32 dwLoc08_n = 2;
	while (dwLoc08_n - dwArg04 <= 0x00)
		++dwLoc08_n;
}

// 80000372: void pow_int(Stack real64 rArg04, Stack int32 dwArg0C)
// Called from:
//      sine_taylor
void pow_int(real64 rArg04, int32 dwArg0C)
{
	int32 dwLoc08_n = 0x00;
	while (dwLoc08_n - dwArg0C < 0x00)
		++dwLoc08_n;
}

// 800003BC: void sine_taylor(Stack real64 rArg04, Stack int32 dwArg0C)
void sine_taylor(real64 rArg04, int32 dwArg0C)
{
	int32 dwLoc08_n = 3;
	while (dwLoc08_n - dwArg0C <= 0x00)
	{
		pow_int(rArg04, dwLoc08_n);
		factorial(dwLoc08_n);
		dwLoc08_n += 0x04;
	}
	int32 dwLoc08_n = 5;
	while (dwLoc08_n - dwArg0C <= 0x00)
	{
		pow_int(rArg04, dwLoc08_n);
		factorial(dwLoc08_n);
		dwLoc08_n += 0x04;
	}
}

// 8000049A: void main()
void main()
{
	sine_taylor(3.14);
	_sin(3.14, 0.003, fp - 0x08);
}

// 800004DE: void _sin(Stack real64 rArg04, Stack real64 rArg0C, Stack Eq_n tArg14)
// Called from:
//      main
void _sin(real64 rArg04, real64 rArg0C, Eq_n tArg14)
{
	real64 rLoc0C_n = rArg04;
	real64 v9_n = (real64) ((real96) rArg04 * rArg04);
	word32 dwLoc20_n = 0x02;
	real64 rLoc14_n = 1.0;
	while ((real64) ((real96) rLoc0C_n / rLoc14_n) >= rArg0C)
	{
		*tArg14 = (word32) *tArg14 + 1;
		word32 v18_n = dwLoc20_n + 0x00;
		rLoc0C_n = (real64) ((real96) (real64) ((real96) rLoc0C_n * v9_n) * v9_n);
		dwLoc20_n = v18_n + 0x03;
		rLoc14_n = (real64) ((real96) (real64) ((real96) (real64) ((real96) (real64) ((real96) rLoc14_n * (real80) ((real96) v18_n)) * (real80) ((real96) (v18_n + 0x01))) * (real80) ((real96) (v18_n + 0x02))) * (real80) ((real96) (v18_n + 0x03)));
	}
}

// 80000624: void __do_global_ctors_aux()
void __do_global_ctors_aux()
{
	<anonymous> * a0_n = g_ptr8000270C;
	ptr32 a6_n = fp - 0x04;
	ptr32 a2_n = 0x8000270C;
	while (a0_n != (<anonymous> *) -1)
	{
		struct Eq_n * a2_n;
		a0_n();
		a0_n = a2_n->ptrFFFFFFFC;
		a2_n = a2_n - 4;
	}
}

// 8000064E: void call___do_global_ctors_aux()
void call___do_global_ctors_aux()
{
}

