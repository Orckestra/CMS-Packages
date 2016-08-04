param([string] $filetotouch)
if(test-path $filetotouch)
    {
    Set-ItemProperty -Path $filetotouch -Name LastWriteTime -Value (get-date)
    “File $filetotouch timestamp succesfully updated”
    }
else
    {
    Set-Content -Path ($filetotouch) -Value ($null);
    “File $filetotouch succesfully created”
    }