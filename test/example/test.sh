#!/bin/bash

# Run VHDLTest and capture the output (should report 2 passes and 2 fails)
vhdltest --config test.yaml --simulator ghdl > out.log
cat out.log

# Check for passes
if ! grep -q "Passed 2 of 4 tests" out.log; then
    echo Error - did not find expected passes
    exit 1
fi

# Check for fails
if ! grep -q "Failed 2 of 4 tests" out.log; then
    echo Error - did not find expected fails
    exit 1
fi

# Report success
echo Success
exit 0
