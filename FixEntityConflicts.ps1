# Improved PowerShell script to fix namespace conflicts
# Save this as FixEntityConflicts.ps1 and run from your project root

# Define the replacements with fully qualified names for entities that have namespace conflicts
$replacements = @{
    'new Customer\b' = 'new BusinessObject.Customer'
    'new Product\b' = 'new BusinessObject.Product'
    'new Order\b' = 'new BusinessObject.Order'
    'new Employee\b' = 'new BusinessObject.Employee'
    'new Category\b' = 'new BusinessObject.Category'
    'new OrderDetail\b' = 'new BusinessObject.OrderDetail'
    '\bCustomer\s+' = 'BusinessObject.Customer '
    '\bProduct\s+' = 'BusinessObject.Product '
    '\bOrder\s+' = 'BusinessObject.Order '
    '\bEmployee\s+' = 'BusinessObject.Employee '
    '\bCategory\s+' = 'BusinessObject.Category '
    '\bOrderDetail\s+' = 'BusinessObject.OrderDetail '
}

# Get all C# files in the WPF project
$files = Get-ChildItem -Path "NguyenQuocHuy_SE193304_ASM01" -Include "*.cs" -Recurse | 
    Where-Object { $_.FullName -notlike "*\obj\*" -and $_.FullName -notlike "*\bin\*" -and $_.Name -notlike "*g.*.cs" }

Write-Host "Found $($files.Count) C# files to process..."

foreach ($file in $files) {
    Write-Host "Processing: $($file.Name)"
    
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Apply each replacement
    foreach ($pattern in $replacements.Keys) {
        $replacement = $replacements[$pattern]
        $content = $content -replace $pattern, $replacement
    }
    
    # Only write if content changed
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  ? Updated $($file.Name)" -ForegroundColor Green
    } else {
        Write-Host "  - No changes needed in $($file.Name)" -ForegroundColor Gray
    }
}

Write-Host "`nNamespace conflict fixes completed!" -ForegroundColor Cyan