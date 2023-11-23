# chemcom-api

Source code for the Chemcom web API

## Making commits

Use conventional commits as described in https://github.com/equinor/sdscoredev-handbook#git-commits

## Running locally

1. Add Azure client id and secret to appsettings.Development.json file.
2. To connect to dev DB for local development, copy the connection string from the key vault in to appsettings.Development.json file.
3. Copy the Azure blob key and account name from the Dev Azure storage account.

## App in Radix

https://console.radix.equinor.com/applications/chemcom-api

## Pre-commit TruffleHog

Equinor [AppSec](https://appsec.equinor.com/guidelines/secret-scanning/) recommends to scan git pre-commits, using TruffleHog, for detecting secrets/keys when commits and push are made. Follow these instructions to install and set up pre-commit and Trufflehog for this repository on your local machine.

### Installation
1. Install Trufflehog by following the instructions [here](https://github.com/trufflesecurity/trufflehog#floppy_disk-installation).
2. Install pre-commit by running the following command:

```
pip install pre-commit
```
Verify installation:

```
pre-commit --version
```

If 'pre-commit' is not recognized as an internal/external command you need to include pre-commit executable file in your system PATH environment variable

3. From the root of the repository (where .pre-commit-config.yml is located) run the following command:

```
pre-commit install
```
If succesfully installed you should get following response:

```
pre-commit installed at .git/hooks/pre-commit
```
### Extra
Pre-commit will only run on the changed files whenever a commit or push is made to your git repository. Run the following command to scan all files in the repository:

```
pre-commit run --all-files
```

### Reference
[AppSec - Secret Scanning](https://appsec.equinor.com/guidelines/secret-scanning/)

[TruffleHog](https://github.com/trufflesecurity/trufflehog)

[Pre-commit](https://pre-commit.com/)