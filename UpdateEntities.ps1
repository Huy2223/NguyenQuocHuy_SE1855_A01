# PowerShell script to update entity names in WPF files
# Save this as UpdateEntities.ps1 and run from your project root

# Define the replacements
$replacements = @{
    'Customers' = 'Customer'
    'Products' = 'Product'
    'Orders' = 'Order'
    'Employees' = 'Employee'
    'Categories' = 'Category'
    'OrderDetails' = 'OrderDetail'
    'CustomerID' = 'CustomerId'
    'ProductID' = 'ProductId'
    'OrderID' = 'OrderId'
    'EmployeeID' = 'EmployeeId'
    'CategoryID' = 'CategoryId'
}

# Get all C# files in the WPF project
$files = Get-ChildItem -Path "NguyenQuocHuy_SE193304_ASM01" -Include "*.cs" -Recurse

Write-Host "Found $($files.Count) C# files to process..."

foreach ($file in $files) {
    Write-Host "Processing: $($file.Name)"
    
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Apply each replacement
    foreach ($old in $replacements.Keys) {
        $new = $replacements[$old]
        
        # Replace whole words only (using word boundaries)
        $content = $content -replace "\b$old\b", $new
    }
    
    # Only write if content changed
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  ? Updated $($file.Name)" -ForegroundColor Green
    } else {
        Write-Host "  - No changes needed in $($file.Name)" -ForegroundColor Gray
    }
}

Write-Host "`nEntity name updates completed!" -ForegroundColor Cyan
Write-Host "Please review the changes and test your application." -ForegroundColor Yellow