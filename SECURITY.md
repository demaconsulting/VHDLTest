# Security Policy

## Supported Versions

We release patches for security vulnerabilities. Currently, only the latest version is supported with security updates.

| Version | Supported          |
| ------- | ------------------ |
| Latest  | :white_check_mark: |
| < Latest| :x:                |

We recommend always using the latest version to ensure you have all security patches and improvements.

## Reporting a Vulnerability

We take the security of VHDLTest seriously. If you discover a security vulnerability, please report it responsibly.

### How to Report

**Please do NOT report security vulnerabilities through public GitHub issues.**

Instead, please report security vulnerabilities using GitHub's Security Advisory feature:

1. Navigate to the [Security tab](https://github.com/demaconsulting/VHDLTest/security) of this repository
2. Click on "Report a vulnerability"
3. Fill out the vulnerability report form with as much detail as possible

### What to Include

When reporting a vulnerability, please include:

* A description of the vulnerability
* Steps to reproduce the issue
* Potential impact of the vulnerability
* Any suggested fixes (if available)
* Your contact information for follow-up questions

## What to Expect

After reporting a vulnerability, the following process will be followed:

* You will receive an acknowledgment of your report
* We will confirm receipt and begin our assessment
* Regular updates will be provided on the status of your report during the investigation
* You will be informed when the vulnerability is confirmed or dismissed
* Security fixes will be prioritized based on severity
* Critical and high-severity vulnerabilities will be addressed as soon as possible
* Lower severity vulnerabilities will be addressed in a planned release
* If you discover a security vulnerability, you will be credited in:
  * The security advisory (unless you prefer to remain anonymous)
  * Our Security Hall of Fame (see below)
  * The release notes for the fix

## Security Update Process

### 1. Triage

* Security reports are reviewed and assessed for severity
* We use the Common Vulnerability Scoring System (CVSS) to evaluate severity
* Critical and high-severity issues are prioritized immediately

### 2. Development

* Fixes are developed in a private branch
* Security patches are reviewed by multiple team members
* Comprehensive testing is performed to ensure the fix doesn't introduce regressions

### 3. Release

* Security updates are released as soon as possible after validation
* For critical vulnerabilities, a patch release is issued immediately
* For less severe issues, fixes may be included in the next regular release

### 4. Disclosure

* A security advisory is published after the fix is released
* The advisory includes:
  * Description of the vulnerability
  * Affected versions
  * Fixed version
  * Workarounds (if available)
  * Credit to the reporter

### 5. Communication

* Users are notified through:
  * GitHub Security Advisories
  * Release notes
  * NuGet package updates

## Security Best Practices

### For Users

When using VHDLTest, we recommend:

* **Always use the latest version** to benefit from security patches
* **Validate input files** before processing them with VHDLTest
* **Run in isolated environments** when processing untrusted VHDL files
* **Review configuration files** (YAML) for unexpected content before use
* **Use environment variables carefully** when configuring simulator paths
* **Monitor for security advisories** in this repository

### For Contributors

When contributing to VHDLTest:

* **Never commit secrets** or sensitive information to the repository
* **Validate all inputs** from external sources (files, command-line arguments)
* **Use parameterized commands** when invoking external tools to prevent injection
* **Follow secure coding practices** as outlined in our contributing guidelines
* **Run security analysis tools** before submitting pull requests
* **Report security concerns** privately through the proper channels

## Input Validation

VHDLTest implements input validation for:

### Configuration Files (YAML)

* YAML configuration files are parsed using YamlDotNet
* Schema validation ensures required fields are present
* File paths are validated to prevent directory traversal attacks
* Malformed YAML files are rejected with clear error messages

### Command-Line Arguments

* All command-line arguments are validated before use
* File paths are sanitized to prevent path traversal
* Simulator names are validated against a whitelist
* Invalid arguments result in clear error messages and safe exit

### VHDL Files

* File existence and readability are verified before processing
* File paths are validated to prevent directory traversal
* Large files are handled appropriately to prevent resource exhaustion

### External Tool Invocation

* Simulator paths are validated before invocation
* Command arguments are properly escaped to prevent injection attacks
* Process execution is monitored and constrained

## Security Tools Used

VHDLTest employs multiple security tools and practices:

### Static Analysis

* **Microsoft.CodeAnalysis.NetAnalyzers**: Provides security analysis during compilation
* **SonarCloud**: Continuous security analysis and vulnerability detection
* **CodeQL**: Automated security scanning in CI/CD pipeline
* **Dependabot**: Automated dependency vulnerability scanning and updates

### Code Quality

* **EditorConfig**: Enforces consistent code formatting to reduce errors
* **Warnings as Errors**: All compiler warnings are treated as errors
* **Nullable Reference Types**: Enabled to prevent null reference exceptions

### CI/CD Security

* **SBOM Generation**: Software Bill of Materials for supply chain security
* **Dependency Scanning**: Weekly automated checks for vulnerable dependencies
* **Build Isolation**: Builds run in isolated, ephemeral environments
* **Code Signing**: Releases are signed for authenticity verification

### Testing

* **Comprehensive Unit Tests**: Validate security-critical code paths
* **Code Coverage**: Monitored to ensure test completeness
* **Integration Tests**: Test interactions with external simulators safely

## Responsible Disclosure

We follow responsible disclosure practices:

* **Coordinated Disclosure**: We work with security researchers to coordinate public disclosure
* **Reasonable Timeline**: We aim to fix and disclose vulnerabilities as quickly as possible
* **Private Discussion**: Vulnerabilities are discussed privately until patched
* **Public Advisory**: After fixes are released, we publish detailed security advisories
* **CVE Assignment**: We request CVE identifiers for significant vulnerabilities when appropriate

## Security Hall of Fame

We recognize and thank security researchers who have responsibly disclosed vulnerabilities to us:

*Currently, no vulnerabilities have been reported. Be the first to help us improve VHDLTest's security!*

<!-- Future entries will follow this format:
* **[Researcher Name]** - [Vulnerability Description] - [Date]
-->

## Contact

For security-related questions that are not vulnerability reports:

* Open a discussion in the [GitHub Discussions](https://github.com/demaconsulting/VHDLTest/discussions) area
* For general questions, refer to our [Contributing Guidelines](https://github.com/demaconsulting/VHDLTest/blob/main/CONTRIBUTING.md)

For security vulnerabilities, always use the
[GitHub Security Advisory](https://github.com/demaconsulting/VHDLTest/security) feature.

## Additional Resources

### External Security Resources

* [OWASP Top Ten](https://owasp.org/www-project-top-ten/) - Web application security risks
* [CWE - Common Weakness Enumeration](https://cwe.mitre.org/) - Software weaknesses catalog
* [CVSS - Common Vulnerability Scoring System](https://www.first.org/cvss/) - Vulnerability severity scoring
* [NVD - National Vulnerability Database](https://nvd.nist.gov/) - Vulnerability database

### .NET Security Resources

* [.NET Security Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/security/)
* [Secure Coding Guidelines for .NET](https://learn.microsoft.com/en-us/dotnet/standard/security/secure-coding-guidelines)
* [.NET Security Announcements](https://github.com/dotnet/announcements/labels/security)

### Project Resources

* [Contributing Guidelines](https://github.com/demaconsulting/VHDLTest/blob/main/CONTRIBUTING.md)
* [Code of Conduct](https://github.com/demaconsulting/VHDLTest/blob/main/CODE_OF_CONDUCT.md)
* [License](https://github.com/demaconsulting/VHDLTest/blob/main/LICENSE)

---

Thank you for helping keep VHDLTest and its users safe!
