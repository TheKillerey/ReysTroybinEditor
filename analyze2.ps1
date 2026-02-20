# Analyze multiple troybin files to understand the format
$folder = "D:\Mods_Github\OldSummonersRiftV2\Old Summoners Rift V2\OldMinions_NightVersion\DATA\data\particles"
$files = @(
    "sru_order_mm_ba_tar.troybin",   # 261 bytes - smallest
    "chaos_inhibit_base_glow.troybin",  # 266 bytes
    "chaos_inhibit_crystal_glow.troybin",  # 542 bytes
    "order_inhibit_base_glow.troybin",  # 278 bytes
    "dragon_smoke.troybin"  # 501 bytes
)

foreach ($fname in $files) {
    $path = Join-Path $folder $fname
    $bytes = [System.IO.File]::ReadAllBytes($path)
    Write-Host "======== $fname ($($bytes.Length) bytes) ========"
    
    # Print hex dump
    for ($row = 0; $row * 16 -lt $bytes.Length; $row++) {
        $offset = $row * 16
        $len = [Math]::Min(16, $bytes.Length - $offset)
        $chunk = @($bytes[$offset..($offset + $len - 1)])
        $h = ($chunk | ForEach-Object { $_.ToString("X2") }) -join " "
        $a = ($chunk | ForEach-Object { if ($_ -ge 32 -and $_ -le 126) { [char]$_ } else { "." } }) -join ""
        Write-Host ("{0:X4}  {1,-47}  {2}" -f $offset, $h, $a)
    }
    Write-Host ""
}

