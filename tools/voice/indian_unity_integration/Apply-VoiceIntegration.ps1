param([string]$VoiceStudio)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
if (!$VoiceStudio) {
    $VoiceStudio = Join-Path (Split-Path $repo -Parent) 'NPC-Voice-Generator'
}
$production = Join-Path $VoiceStudio 'production'
$batch = Join-Path $production 'batches\indian_level1_v1'
$report = Join-Path $batch 'FINAL_REPORT.md'
$log = Join-Path $batch 'final_verification.log'
if (!(Test-Path -LiteralPath $report -PathType Leaf) -or !(Test-Path -LiteralPath $log -PathType Leaf)) {
    throw 'Wait for the existing synthesis and final validator to finish before applying this patch.'
}
$verifications = Get-Content -LiteralPath $log -Raw
$actors = [ordered]@{
    lakshmi_amma = 'Lakshmamma'
    chinnamma_naik = 'ChinnammaNaik'
    abdul_rahman = 'AbdulRahman'
    saraswati_chetti = 'SaraswatiChetti'
}
foreach ($actor in $actors.GetEnumerator()) {
    $directory = Join-Path $production ('packages\' + $actor.Value + '_Level1_v1')
    $source = Join-Path $directory 'production_validation.json'
    $check = Get-Content -LiteralPath $source -Raw -Encoding UTF8 | ConvertFrom-Json
    if ($check.character -ne $actor.Key -or !$check.production_complete -or
        $verifications -notmatch ('(?m)^' + [regex]::Escape($actor.Key) + ' validated .* complete: True\r?$')) {
        throw "Final validation has not succeeded for $($actor.Key). See $report."
    }
}
$inUse = Get-CimInstance Win32_Process -Filter "Name = 'python.exe'" | Where-Object {
    $_.CommandLine -match 'batch_runner\.py.*--indian-level1|validate_indian_level1\.py'
}
if ($inUse) { throw 'An Indian production or validation process is still active. Try again once it exits.' }

$patch = Join-Path $PSScriptRoot 'UnityVoiceIntegration.patch'
Push-Location $repo
try {
    git apply --check $patch
    if ($LASTEXITCODE -ne 0) { throw 'Unity sources or destination files changed. The patch was not applied.' }
    git apply $patch
    if ($LASTEXITCODE -ne 0) { throw 'git apply failed.' }
    Write-Host 'Unity voice integration staged in Assets. Return to Unity, let compilation finish, then use Tools > Anubhava > Import Completed Indian NPC Voice Package.'
} finally { Pop-Location }
