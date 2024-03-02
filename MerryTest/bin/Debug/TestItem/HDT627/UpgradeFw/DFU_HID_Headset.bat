@echo off
for %%f in (*headset*signed.bin) do (
    call ConsoleDFU.exe run_wired_DFU 0x046D 0x0AFF %%f
    pause
	exit
)