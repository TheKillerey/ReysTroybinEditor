$file = "D:\Mods_Github\OldSummonersRiftV2\Old Summoners Rift V2\OldMinions_NightVersion\DATA\data\particles\sru_order_mm_ba_tar.troybin"
$bytes = [System.IO.File]::ReadAllBytes($file)
$out = New-Object System.Text.StringBuilder
$null = $out.AppendLine("File size: $($bytes.Length)")
$null = $out.AppendLine("")

for ($row = 0; $row * 16 -lt $bytes.Length; $row++) {
    $offset = $row * 16
    $len = [Math]::Min(16, $bytes.Length - $offset)
    $chunk = @($bytes[$offset..($offset + $len - 1)])
    $h = ($chunk | ForEach-Object { $_.ToString("X2") }) -join " "
    $a = ($chunk | ForEach-Object { if ($_ -ge 32 -and $_ -le 126) { [char]$_ } else { "." } }) -join ""
    $null = $out.AppendLine(("{0:X4}  {1,-47}  {2}" -f $offset, $h, $a))
}

$out.ToString() | Out-File "C:\Users\theki\RiderProjects\TroybinEditor\hexdump.txt" -Encoding utf8
Write-Host "Done. See hexdump.txt"


# Analyze the string table
# offset 0x94: F1 00 00 => count=1? then 16-bit offsets into string table
# Let me parse the 16-bit offsets at 0x97
Write-Host ""
Write-Host "=== Looking for string count and offset table ==="
# At 0x94: F1 00 00 09 00 0F 00 11 00 17 00 1B 00 32 00 40 00 47 00 4D 00 4F 00 56 00
# Byte F1 could be ignored, then: 00 00 = string count 0? No. Let's look differently.
# The value at 0x97 = 09 00 = offset 9, 0x99 = 0F 00 = offset 15...
# This looks like a uint16 string offset table

# Check byte at 0x94-0x96: 
Write-Host "Byte 0x94 = 0x$($bytes[0x94].ToString('X2'))"
Write-Host "Bytes 0x95-0x96 (uint16 little-endian) = $([BitConverter]::ToUInt16($bytes, 0x95))"

# Find the start of the string offset table
# Before the strings, there should be a table of uint16 offsets
# Strings start at 0xA3 ('C' of 'Cardbits')
# Offsets before that: 09 00  0F 00  11 00  17 00  1B 00  32 00  40 00  47 00  4D 00  4F 00  56 00
# => 9, 15, 17, 23, 27, 50, 64, 71, 77, 79, 86
$strTableBase = 0xA3  # 'C' of Cardbits
Write-Host ""
Write-Host "=== String table starting at 0x$($strTableBase.ToString('X3')) ==="
# Read null-terminated strings
$pos = $strTableBase
$idx = 0
while ($pos -lt $bytes.Length) {
    $start = $pos
    while ($pos -lt $bytes.Length -and $bytes[$pos] -ne 0) { $pos++ }
    if ($pos -gt $start) {
        $s = [System.Text.Encoding]::ASCII.GetString($bytes, $start, $pos - $start)
        Write-Host ("  [{0}] offset=0x{1:X4} (rel={2,3}) : '{3}'" -f $idx, $start, ($start - $strTableBase), $s)
    }
    $idx++
    $pos++
    if ($pos -ge $bytes.Length) { break }
}

