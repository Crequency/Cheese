$path = "./.idea/.idea.Cheese/.idea/"

$exists = Test-Path -Path $path -PathType Container

if ($exists -eq $false) {
    mkdir $path
}

Copy-Item ./icon.svg $path
