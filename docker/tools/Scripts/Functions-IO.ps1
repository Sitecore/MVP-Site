using namespace System.Management.Automation.Host

Set-StrictMode -Version Latest

$esc = [char]27

function Write-PrePrompt {
    Write-Host "`n> " -NoNewline -ForegroundColor Green
}

function Read-ValueFromHost {    
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Question,
        [ValidateNotNullOrEmpty()]
        [string] 
        $DefaultValue,
        [ValidateNotNullOrEmpty()]
        [string] 
        $ValidationRegEx,
        [switch]$Required
    )
    Write-Host ""
    $Prompt = "$Question`n"
    if ($DefaultValue -ne "") {
        $Prompt = "$Prompt$esc[38;5;249mpress enter for: $DefaultValue$esc[0m"
    }
    do {
        Write-PrePrompt
        $value = Read-Host $Prompt
        if ($value -eq "" -band $DefaultValue -ne "") { $value = $DefaultValue }
        $invalid = ($Required -and $value -eq "") -or ($ValidationRegEx -ne "" -and $value -notmatch $ValidationRegEx)
    } while ($invalid -bor $value -eq "q")
    $value
}

function Confirm {    
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] 
        $Question,
        [switch] 
        $DefaultYes
    )
    $options = [ChoiceDescription[]](
        [ChoiceDescription]::new("&Yes"), 
        [ChoiceDescription]::new("&No")
    )
    $defaultOption = 1;
    if ($DefaultYes) { $defaultOption = 0 }
    Write-Host ""
    Write-PrePrompt
    $result = $host.ui.PromptForChoice("", $Question, $options, $defaultOption)
    switch ($result) {
        0 { return $true }
        1 { return $false }
    }
}