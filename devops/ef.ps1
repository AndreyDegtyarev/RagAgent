<#
.SYNOPSIS
  A helper script to run Entity Framework Core migrations for the RagAgent project.

.DESCRIPTION
  This script simplifies EF Core commands by automatically specifying the correct
  project (--project RagAgent.Infrastructure), startup project (--startup-project RagAgent.Api), and output directory (--output-dir Persistence/Migrations).

.PARAMETER Action
  The migration action to perform: add, remove, update, or drop.
  If not specified, an interactive menu will be displayed.

.PARAMETER Name
  The name of the migration (required when adding a migration).

.EXAMPLE
  .\ef.ps1 add InitialCreate
  .\ef.ps1 remove
  .\ef.ps1 update
  .\ef.ps1 drop
#>

param (
    [Parameter(Position=0)]
    [ValidateSet("add", "remove", "update", "drop")]
    [string]$Action,

    [Parameter(Position=1)]
    [string]$Name
)

$Project = "RagAgent.Infrastructure"
$StartupProject = "RagAgent.Api"

# Helper function to run the command and report status
function Run-EfCommand {
    param (
        [string]$Command,
        [string]$Arguments
    )
    Write-Host "Running: dotnet ef $Command $Arguments" -ForegroundColor Cyan
    dotnet ef $Command $Arguments --project $Project --startup-project $StartupProject
}

# If no Action was specified, display an interactive menu
if ([string]::IsNullOrEmpty($Action)) {
    Write-Host "=== Entity Framework Core Helper ===" -ForegroundColor Yellow
    Write-Host "Select an action to perform:"
    Write-Host "1) Add Migration (dotnet ef migrations add)"
    Write-Host "2) Remove Last Migration (dotnet ef migrations remove)"
    Write-Host "3) Update Database (dotnet ef database update)"
    Write-Host "4) Drop Database (dotnet ef database drop)"
    Write-Host "5) Exit"
    Write-Host ""
    
    $choice = Read-Host "Enter your choice (1-5)"
    switch ($choice) {
        "1" {
            $Action = "add"
            $Name = Read-Host "Enter migration name"
            if ([string]::IsNullOrWhiteSpace($Name)) {
                Write-Host "Migration name cannot be empty. Exiting." -ForegroundColor Red
                exit 1
            }
        }
        "2" { $Action = "remove" }
        "3" { $Action = "update" }
        "4" { $Action = "drop" }
        default {
            Write-Host "Exiting."
            exit 0
        }
    }
}

# Perform the action
switch ($Action) {
    "add" {
        if ([string]::IsNullOrEmpty($Name)) {
            Write-Error "Migration name is required for 'add' action. Usage: .\ef.ps1 add <MigrationName>"
            exit 1
        }
        Run-EfCommand "migrations add" "$Name --output-dir Persistence/Migrations"
    }
    "remove" {
        Run-EfCommand "migrations remove" ""
    }
    "update" {
        Run-EfCommand "database update" ""
    }
    "drop" {
        # Ask for confirmation before dropping database, to prevent accidental loss
        $confirm = Read-Host "Are you sure you want to drop the database? (y/N)"
        if ($confirm -eq 'y' -or $confirm -eq 'Y') {
            Run-EfCommand "database drop" "--force"
        } else {
            Write-Host "Database drop canceled." -ForegroundColor Yellow
        }
    }
}
