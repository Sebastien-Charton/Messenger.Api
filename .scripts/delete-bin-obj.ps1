# Function to delete bin and obj folders recursively
function Delete-BinObjFolders {
    param (
        [string]$folder
    )

    Get-ChildItem -Path $folder -Recurse -Directory | ForEach-Object {
        if ($_.Name -eq "bin" -or $_.Name -eq "obj") {
            Write-Host "Deleting: $($_.FullName)"
            Remove-Item -Path $_.FullName -Recurse -Force
        }
    }
}

# Main script
if ($args.Count -eq 0) {
    Write-Host "Usage: Delete-BinObj.ps1 <directory>"
    exit 1
}

$directory = $args[0]

if (-not (Test-Path -Path $directory -PathType Container)) {
    Write-Host "Directory '$directory' not found."
    exit 1
}

Delete-BinObjFolders $directory
Write-Host "Finished deleting bin and obj folders."
