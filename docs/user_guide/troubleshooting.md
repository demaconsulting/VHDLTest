# Troubleshooting

## Common Issues

### Simulator Not Found

**Problem**: VHDLTest cannot find the VHDL simulator.

**Solution**:

- Ensure the simulator is installed
- Set the appropriate environment variable (e.g., `VHDLTEST_GHDL_PATH`)
- Add the simulator to your system PATH

### Compilation Errors

**Problem**: VHDL files fail to compile.

**Solution**:

- Verify VHDL syntax is correct
- Check that files are listed in dependency order
- Ensure all required libraries are included

### Tests Not Executing

**Problem**: Test benches are not running.

**Solution**:

- Verify test bench entity names match the `tests` section
- Ensure test bench files are included in the `files` section
- Check that test benches have correct structure

### Permission Errors

**Problem**: Cannot write test results file.

**Solution**:

- Ensure the output directory exists
- Verify write permissions on the output directory
- Check disk space availability

## Debug Mode

Enable verbose output for troubleshooting:

```bash
dotnet vhdltest --config test_suite.yaml --verbose
```

This provides detailed information about:

- File processing
- Compilation steps
- Test execution
- Simulator output

## Best Practices

### Test Organization

- **Separate Test Files**: Keep test benches in separate files from design units
- **Naming Convention**: Use `_tb` suffix for test bench files (e.g., `adder_tb.vhd`)
- **Dependency Order**: List files in dependency order in configuration

### Configuration Management

- **Version Control**: Keep configuration files in version control
- **Multiple Configs**: Use different configuration files for different test suites
- **Documentation**: Comment configuration files to explain purpose

### CI/CD Integration

- **Automated Testing**: Run VHDLTest on every commit
- **Test Results**: Always generate and publish test results
- **Multiple Simulators**: Test with multiple simulators when possible
- **Validation**: Include self-validation in release pipelines

### Test Design

- **Assertions**: Use VHDL assertions to verify behavior
- **Coverage**: Aim for comprehensive test coverage
- **Independence**: Ensure tests are independent and can run in any order
- **Clear Output**: Provide clear pass/fail indicators
