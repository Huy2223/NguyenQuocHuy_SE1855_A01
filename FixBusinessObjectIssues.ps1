# PowerShell script to fix BusinessObject namespace issues
$files = Get-ChildItem -Path "NguyenQuocHuy_SE193304_ASM01" -Filter "*.cs" -Recurse

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Fix malformed foreach loops like "foreach (var BusinessObject.Product in Product)"
    $content = $content -replace 'foreach \(var BusinessObject\.(\w+) in (\w+)\)', 'foreach (var $2 in $1s)'
    
    # Fix malformed variable declarations like "var BusinessObject.Customer ="
    $content = $content -replace 'var BusinessObject\.(\w+) =', 'var $1 ='
    
    # Fix malformed declarations like "BusinessObject.Customer = _customerService"
    $content = $content -replace '^\s*BusinessObject\.(\w+) =', 'var $1 ='
    
    # Fix malformed comments and text like "// Process each BusinessObject.Order foreach (var BusinessObject.Order in Order)"
    $content = $content -replace '// Process each BusinessObject\.(\w+) foreach \(var BusinessObject\.\1 in (\w+)\)', '// Process each $1 foreach (var $1 in $2)'
    
    # Fix incorrect variable names in foreach loops
    $content = $content -replace 'foreach \(var (\w+) in (\w+)\)\s*\{\s*// Get BusinessObject\.(\w+) details', 'foreach (var $1 in $2) { // Get $1 details'
    
    # Fix malformed method signatures
    $content = $content -replace 'public List<(\w+)> Get(\w+)\(\) (\w+)', 'public List<$1> Get$2() { return _$1s; }'
    
    # Fix BusinessObject.BusinessObject.Entity references
    $content = $content -replace 'BusinessObject\.BusinessObject\.(\w+)', '$1'
    
    # Fix double BusinessObject prefixes in comments
    $content = $content -replace 'BusinessObject\.(\w+) BusinessObject\.(\w+)', '$1 $2'
    
    # Fix sentences that accidentally split BusinessObject.Entity references
    $content = $content -replace 'Get BusinessObject\.(\w+) information', 'Get $1 information'
    $content = $content -replace 'Track BusinessObject\.(\w+) sales', 'Track $1 sales'
    $content = $content -replace 'Add BusinessObject\.(\w+) to', 'Add $1 to'
    $content = $content -replace 'Update BusinessObject\.(\w+) summary', 'Update $1 summary'
    
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -Encoding UTF8
        Write-Host "Fixed: $($file.FullName)"
    }
}

Write-Host "BusinessObject namespace fix completed!"