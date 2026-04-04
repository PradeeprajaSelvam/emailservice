# EmailService

A Windows Service application that periodically sends automated emails using SMTP configuration, built with .NET Framework 4.7.2 and log4net logging.

## Overview

EmailService is a Windows Service that runs in the background and automatically sends emails on a scheduled interval (every 5 minutes by default). It uses SMTP to send emails and includes comprehensive logging capabilities.

## Features

- **Windows Service**: Runs as a Windows Service managed by the Service Control Manager (SCM)
- **Scheduled Email Sending**: Sends emails at configurable intervals (default: every 5 minutes)
- **SMTP Configuration**: Flexible SMTP configuration via `App.config`
- **Logging**: Comprehensive logging using log4net with both console and file appenders
- **Gmail Integration**: Pre-configured for Gmail SMTP with App Password support
- **Error Handling**: Robust error handling and logging for troubleshooting

## Prerequisites

- .NET Framework 4.7.2 or later
- Visual Studio or equivalent .NET build tools
- Administrator privileges to install/manage the Windows Service
- SMTP server credentials (e.g., Gmail account with App Password)

## Dependencies

- **log4net** (v2.0.14): Logging framework
- **System.Configuration.ConfigurationManager** (v9.0.0): Configuration management

## Configuration

All settings are configured in `App.config`:

### SMTP Settings

```xml
<appSettings>
    <!-- SMTP Configuration -->
    <add key="SmtpHost" value="smtp.gmail.com" />
    <add key="SmtpPort" value="587" />
    <add key="SmtpUsername" value="your-email@gmail.com" />
    <add key="SmtpPassword" value="your-app-password" />
    <add key="SmtpEnableSsl" value="true" />
    <add key="FromEmail" value="your-email@gmail.com" />
    <add key="ToEmail" value="recipient@example.com" />
</appSettings>
```

### Gmail Setup Instructions

1. Enable 2-Step Verification on your Gmail account
2. Generate an App Password at: https://myaccount.google.com/apppasswords
3. Add the 16-character App Password to `App.config` in the `SmtpPassword` setting
4. Ensure `SmtpEnableSsl` is set to `true`

### Logging Configuration

Logs are written to:
- **Console**: Real-time log output during debugging
- **File**: `C:\Logs\EmailService.log` (rolling file, max 10MB per file, keeps 5 backups)

Log level is set to DEBUG for detailed troubleshooting.

## Building and Installation

### Build

```powershell
# Restore NuGet packages
nuget restore EmailService.sln

# Build the solution
msbuild EmailService.sln /p:Configuration=Release
```

### Install as Windows Service

```powershell
# Run Command Prompt or PowerShell as Administrator

# Install the service
sc create EmailService binPath= "C:\path\to\EmailService.exe"

# Start the service
net start EmailService

# Stop the service
net stop EmailService

# Remove the service
sc delete EmailService
```

Alternatively, use InstallUtil.exe (if included with your .NET Framework installation):

```powershell
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe EmailService.exe
```

## Project Structure

- **Program.cs**: Application entry point; handles Windows Service registration and initialization
- **Service1.cs (EmailService)**: Main Windows Service class; manages the timer and service lifecycle
- **SendEmail.cs**: Core email sending logic using SMTP
- **App.config**: Configuration file for SMTP settings and logging
- **packages.config**: NuGet package dependencies

## Email Sending Interval

The default email sending interval is **5 minutes** (300,000 milliseconds). To change this:

Edit `Service1.cs` and modify the `Timer.Interval` value:

```csharp
Timer timer = new Timer
{
    Interval = 300000  // in milliseconds (5 minutes)
};
```

## Troubleshooting

### Service Won't Start

1. Check the log file at `C:\Logs\EmailService.log` for error details
2. Verify SMTP configuration in `App.config`
3. Ensure the Windows account running the service has appropriate permissions
4. Check that the `C:\Logs` directory exists and is writable

### Emails Not Sending

1. Verify SMTP credentials in `App.config`
2. For Gmail: Ensure you're using an App Password, not your regular Gmail password
3. Check that 2-Step Verification is enabled on your Gmail account
4. Verify email addresses are valid in `FromEmail` and `ToEmail` settings
5. Check the log file for SMTP connection errors

### Log File Not Created

1. Ensure `C:\Logs` directory exists
2. Verify the application has write permissions to the directory
3. Check the log4net configuration in `App.config`

## Development Notes

- The service is configured to send an email immediately upon startup and then every 5 minutes thereafter
- Email sending is fire-and-forget; consider refactoring if strict sequencing or guaranteed completion is required
- The timer runs on a ThreadPool thread; ensure `SendEmail` logic is thread-safe
- For debugging, consider adding an alternative entry point that doesn't run as a service

## License

[Add your license information here]

## Support

For issues or questions, please refer to the logs in `C:\Logs\EmailService.log` and verify your SMTP configuration.
