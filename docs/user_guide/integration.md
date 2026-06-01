# CI/CD Integration

## GitHub Actions

Example GitHub Actions workflow:

```yaml
name: VHDL Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.x'

      - name: Setup GHDL
        uses: ghdl/setup-ghdl@v1
        with:
          version: nightly

      - name: Install VHDLTest
        run: dotnet tool install --global DEMAConsulting.VHDLTest

      - name: Run Tests
        run: vhdltest --config test_suite.yaml --results results.trx

      - name: Publish Test Results
        uses: dorny/test-reporter@v1
        if: always()
        with:
          name: VHDL Tests
          path: results.trx
          reporter: dotnet-trx
```

## Azure DevOps

Example Azure DevOps pipeline:

```yaml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: UseDotNet@2
    inputs:
      version: '8.x'

  - script: |
      dotnet tool install --global DEMAConsulting.VHDLTest
    displayName: 'Install VHDLTest'

  - script: |
      vhdltest --config test_suite.yaml --results $(Build.ArtifactStagingDirectory)/results.trx
    displayName: 'Run VHDL Tests'

  - task: PublishTestResults@2
    inputs:
      testResultsFormat: 'VSTest'
      testResultsFiles: '**/results.trx'
```

## Jenkins

<!-- cspell:ignore mstest -->

Example Jenkinsfile:

```groovy
pipeline {
    agent any

    stages {
        stage('Setup') {
            steps {
                sh 'dotnet tool install --global DEMAConsulting.VHDLTest'
            }
        }

        stage('Test') {
            steps {
                sh 'vhdltest --config test_suite.yaml --results results.trx'
            }
        }

        stage('Publish Results') {
            steps {
                mstest testResultsFile: 'results.trx'
            }
        }
    }
}
```
