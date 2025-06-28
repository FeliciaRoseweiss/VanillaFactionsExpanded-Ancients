# PowerShell script to fix broken XML files created from TXT files
param(
    [string]$RootPath = "."
)

Write-Host "Starting XML file repair process..." -ForegroundColor Green

# Function to clean XML content
function Clean-XmlContent {
    param([string]$content)
    
    # Split content into lines
    $lines = $content -split "`n"
    $cleanLines = @()
    
    $insideDiff = $false
    $foundValidXml = $false
    
    foreach ($line in $lines) {
        $trimmedLine = $line.Trim()
        
        # Skip git diff headers and markers
        if ($trimmedLine -match "^diff --git" -or 
            $trimmedLine -match "^index " -or 
            $trimmedLine -match "^---" -or 
            $trimmedLine -match "^\+\+\+" -or 
            $trimmedLine -match "^@@.*@@") {
            $insideDiff = $true
            continue
        }
        
        # Skip lines that start with + (git diff addition markers)
        if ($trimmedLine -match "^\+(.*)") {
            $cleanLine = $matches[1]
            # If it's a valid XML line, add it without the +
            if ($cleanLine -match "^\s*<" -or $cleanLine -match "^\s*\w+" -or $cleanLine -eq "") {
                $cleanLines += $cleanLine
                if ($cleanLine -match "^\s*<\?xml" -or $cleanLine -match "^\s*<\w+") {
                    $foundValidXml = $true
                }
            }
            continue
        }
        
        # For regular lines, check if they're valid XML
        if ($trimmedLine -match "^\s*<" -or ($foundValidXml -and ($trimmedLine -match "^\s*\w+" -or $trimmedLine -eq ""))) {
            $cleanLines += $line
            if ($trimmedLine -match "^\s*<\?xml" -or $trimmedLine -match "^\s*<\w+") {
                $foundValidXml = $true
            }
        }
    }
    
    # Join lines and ensure proper XML structure
    $cleanContent = $cleanLines -join "`n"
    
    # If no XML declaration found, add one
    if ($cleanContent -notmatch "^\s*<\?xml") {
        $cleanContent = "<?xml version=`"1.0`" encoding=`"utf-8`"?>`n" + $cleanContent
    }
    
    return $cleanContent.Trim()
}

# Get all XML files recursively
$xmlFiles = Get-ChildItem -Path $RootPath -Filter "*.xml" -Recurse

Write-Host "Found $($xmlFiles.Count) XML files to process..." -ForegroundColor Yellow

$processedCount = 0
$errorCount = 0

foreach ($file in $xmlFiles) {
    try {
        Write-Host "Processing: $($file.FullName)" -ForegroundColor Cyan
        
        # Read file content
        $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        
        # Check if file contains diff markers
        if ($content -match "@@.*@@" -or $content -match "^\+" -or $content -match "^diff --git") {
            Write-Host "  - File contains diff markers, cleaning..." -ForegroundColor Yellow
            
            # Clean the content
            $cleanContent = Clean-XmlContent -content $content
            
            # Create backup
            $backupPath = $file.FullName + ".backup"
            Copy-Item -Path $file.FullName -Destination $backupPath -Force
            
            # Write cleaned content with UTF-8 encoding
            [System.IO.File]::WriteAllText($file.FullName, $cleanContent, [System.Text.Encoding]::UTF8)
            
            Write-Host "  - File cleaned and saved with UTF-8 encoding" -ForegroundColor Green
            $processedCount++
        } else {
            # File seems okay, but ensure it has UTF-8 encoding
            $bytes = [System.IO.File]::ReadAllBytes($file.FullName)
            
            # Check if file has BOM or wrong encoding
            if ($bytes.Length -ge 3 -and $bytes[0] -eq 0xEF -and $bytes[1] -eq 0xBB -and $bytes[2] -eq 0xBF) {
                Write-Host "  - File has UTF-8 BOM, removing..." -ForegroundColor Yellow
                # Remove BOM and resave
                $contentWithoutBom = [System.Text.Encoding]::UTF8.GetString($bytes, 3, $bytes.Length - 3)
                [System.IO.File]::WriteAllText($file.FullName, $contentWithoutBom, [System.Text.Encoding]::UTF8)
                $processedCount++
            } else {
                # Ensure UTF-8 encoding
                [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
                Write-Host "  - File encoding verified/fixed" -ForegroundColor Gray
            }
        }
    }
    catch {
        Write-Host "  - ERROR processing file: $($_.Exception.Message)" -ForegroundColor Red
        $errorCount++
    }
}

Write-Host "`nProcess completed!" -ForegroundColor Green
Write-Host "Files processed: $processedCount" -ForegroundColor Green
Write-Host "Errors: $errorCount" -ForegroundColor $(if ($errorCount -gt 0) { "Red" } else { "Green" })

if ($processedCount -gt 0) {
    Write-Host "`nBackup files created with .backup extension" -ForegroundColor Yellow
    Write-Host "You can delete backup files if everything works correctly" -ForegroundColor Yellow
}
