;;; Segment .init (00000000000005A0)

;; _init: 00000000000005A0
;;   Called from:
;;     0000000000000A9C (in __libc_csu_init)
_init proc
	sub	rsp,08
	mov	rax,[0000000000200FE8]                                 ; [rip+00200A3D]
	test	rax,rax
	jz	00000000000005B2

l00000000000005B0:
	call	rax

l00000000000005B2:
	add	rsp,08
	ret
