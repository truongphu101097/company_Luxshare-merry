@echo off
for %%f in (*dongle*signed.bin) do (
    call ConsoleDFU.exe run_wired_DFU 0x046D 0x0AFE %%f
    pause
	exit
)