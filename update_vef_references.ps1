# PowerShell script to update VEF references in XML files for RimWorld 1.6 compatibility
param(
    [string]$RootPath = ".",
    [switch]$DryRun = $false
)

Write-Host "Updating VEF references for RimWorld 1.6 compatibility..." -ForegroundColor Green

# Mapping of old VEF references to new ones
$vefMappings = @{
    # Class references
    'VEF.ExpandableProjectileDef' = 'VanillaExpandedFramework.ExpandableProjectileDef'
    'VEF.HediffComp_Phasing' = 'VanillaExpandedFramework.HediffComp_Phasing'
    'VEF.Abilities.AbilityDef' = 'VanillaExpandedFramework.Abilities.AbilityDef'
    'VEF.CompProperties_Shield' = 'VanillaExpandedFramework.CompProperties_Shield'
    'VEF.CompShield' = 'VanillaExpandedFramework.CompShield'
    
    # XML Class declarations
    '<VEF.' = '<VanillaExpandedFramework.'
    '</VEF.' = '</VanillaExpandedFramework.'
    
    # Package IDs
    'VanillaExpanded.VEF' = 'OskarPotocki.VanillaExpandedFramework'
    'VanillaExpanded.VFEF' = 'OskarPotocki.VanillaExpandedFramework'
}

# Additional mappings for specific components
$componentMappings = @{
    # Replace deprecated components with RimWorld 1.6 equivalents
    'CompProperties_AffectedByFacilities' = 'CompProperties_AffectedByFacilities'
    'CompProperties_Facility' = 'CompProperties_Facility'
}

function Update-XmlContent {
    param([string]$content, [string]$filePath)
    
    $updatedContent = $content
    $changesMade = $false
    
    # Apply VEF mappings
    foreach ($old in $vefMappings.Keys) {
        $new = $vefMappings[$old]
        if ($updatedContent -match [regex]::Escape($old)) {
            $updatedContent = $updatedContent -replace [regex]::Escape($old), $new
            $changesMade = $true
            Write-Host "  - Updated: $old -> $new" -ForegroundColor Yellow
        }
    }
    
    # Apply component mappings
    foreach ($old in $componentMappings.Keys) {
        $new = $componentMappings[$old]
        if ($updatedContent -match [regex]::Escape($old)) {
            $updatedContent = $updatedContent -replace [regex]::Escape($old), $new
            $changesMade = $true
            Write-Host "  - Updated component: $old -> $new" -ForegroundColor Yellow
        }
    }
    
    # Fix common XML issues
    # Fix broken faction references
    if ($updatedContent -match 'category<Debug>') {
        $updatedContent = $updatedContent -replace 'category<Debug>', '<category>Debug</category>'
        $changesMade = $true
        Write-Host "  - Fixed broken faction category tag" -ForegroundColor Yellow
    }
    
    # Fix incomplete comp properties
    if ($updatedContent -match '<comps>\s*</comps>') {
        $updatedContent = $updatedContent -replace '<comps>\s*</comps>', ''
        $changesMade = $true
        Write-Host "  - Removed empty comps section" -ForegroundColor Yellow
    }
    
    # Add missing XML declaration if needed
    if (-not ($updatedContent -match '^\s*<\?xml')) {
        $updatedContent = "<?xml version=`"1.0`" encoding=`"utf-8`"?>`n" + $updatedContent
        $changesMade = $true
        Write-Host "  - Added XML declaration" -ForegroundColor Yellow
    }
    
    return @{
        Content = $updatedContent
        Changed = $changesMade
    }
}

# Get all XML files in the project
$xmlFiles = Get-ChildItem -Path $RootPath -Filter "*.xml" -Recurse | Where-Object {
    $_.FullName -notmatch "\\obj\\" -and
    $_.FullName -notmatch "\\bin\\"
}

Write-Host "Found $($xmlFiles.Count) XML files to process..." -ForegroundColor Yellow

$processedCount = 0
$changedCount = 0

foreach ($file in $xmlFiles) {
    try {
        Write-Host "Processing: $($file.FullName)" -ForegroundColor Cyan
        
        $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        $result = Update-XmlContent -content $content -filePath $file.FullName
        
        if ($result.Changed) {
            $changedCount++
            
            if (-not $DryRun) {
                # Create backup
                $backupPath = $file.FullName + ".backup"
                Copy-Item -Path $file.FullName -Destination $backupPath -Force
                
                # Write updated content
                [System.IO.File]::WriteAllText($file.FullName, $result.Content, [System.Text.Encoding]::UTF8)
                Write-Host "  - File updated and backup created" -ForegroundColor Green
            } else {
                Write-Host "  - Would update file (dry run)" -ForegroundColor Magenta
            }
        } else {
            Write-Host "  - No changes needed" -ForegroundColor Gray
        }
        
        $processedCount++
    }
    catch {
        Write-Host "  - ERROR: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`nUpdate completed!" -ForegroundColor Green
Write-Host "Files processed: $processedCount" -ForegroundColor Green
Write-Host "Files changed: $changedCount" -ForegroundColor Green

if ($DryRun) {
    Write-Host "`nThis was a dry run. Use -DryRun:`$false to apply changes." -ForegroundColor Yellow
} elseif ($changedCount -gt 0) {
    Write-Host "`nBackup files created with .backup extension" -ForegroundColor Yellow
    Write-Host "You can delete backup files if everything works correctly" -ForegroundColor Yellow
}

# Create a summary report
$reportPath = Join-Path $RootPath "vef_update_report.txt"
$report = @"
VEF Update Report
Generated: $(Get-Date)

Files Processed: $processedCount
Files Changed: $changedCount

VEF Mappings Applied:
$(($vefMappings.GetEnumerator() | ForEach-Object { "$($_.Key) -> $($_.Value)" }) -join "`n")

Next Steps:
1. Test the mod in RimWorld 1.6
2. Check for any remaining compilation errors
3. Update C# code references to VEF classes
4. Test all mod features for compatibility

"@

$report | Out-File -FilePath $reportPath -Encoding UTF8
Write-Host "`nReport saved to: $reportPath" -ForegroundColor Cyan
