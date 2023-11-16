--! Use IEEE library
LIBRARY ieee;

    --! Use IEEE standard logic
    USE ieee.std_logic_1164.ALL;

--! Half adder test-bench entity
ENTITY half_adder_fail_tb IS
END ENTITY half_adder_fail_tb;

--! Half adder test-bench architecture
ARCHITECTURE tb OF half_adder_fail_tb IS

    --! Stimulus record type
    TYPE t_stimulus IS RECORD
        name  : string(1 TO 20);
        a_in  : std_logic;
        b_in  : std_logic;
        s_out : std_logic;
        c_out : std_logic;
    END RECORD t_stimulus;

    --! Stimulus array type
    TYPE t_stimulus_array IS ARRAY(natural RANGE <>) OF t_stimulus;

    --! Test bench clock period
    CONSTANT c_clk_period : time := 10 ns;

    --! Test stimulus
    CONSTANT c_stimulus : t_stimulus_array :=
    (
        (
            name  => "a(0) + b(0)         ",
            a_in  => '0',
            b_in  => '0',
            s_out => '0',
            c_out => '0'
        ),
        (
            name  => "a(1) + b(0)         ",
            a_in  => '1',
            b_in  => '0',
            s_out => '1',
            c_out => '0'
        ),
        (
            name  => "a(0) + b(1)         ",
            a_in  => '0',
            b_in  => '1',
            s_out => '0', -- Whups, bad test
            c_out => '0'
        ),
        (
            name  => "a(1) + b(1)         ",
            a_in  => '1',
            b_in  => '1',
            s_out => '0',
            c_out => '1'
        )
    );

    -- Signals to unit under test
    SIGNAL a_in  : std_logic; --! A input to half-adder
    SIGNAL b_in  : std_logic; --! B input to half-adder
    SIGNAL s_out : std_logic; --! Sum output from half-adder
    SIGNAL c_out : std_logic; --! Carry output from half-adder

BEGIN

    --! Instantiate half-adder as unit under test
    i_uut : ENTITY work.half_adder(rtl)
        PORT MAP (
            a_in  => a_in,
            b_in  => b_in,
            s_out => s_out,
            c_out => c_out
        );

    --! Stimulus process
    pr_stimulus : PROCESS IS
    BEGIN

        -- Loop over stimulus
        FOR s IN c_stimulus'range LOOP

            -- Log start of stimulus
            REPORT "Starting: " & c_stimulus(s).name
                SEVERITY note;

            -- Drive inputs
            a_in <= c_stimulus(s).a_in;
            b_in <= c_stimulus(s).b_in;
            WAIT FOR c_clk_period;

            -- Assert s_out
            ASSERT s_out = c_stimulus(s).s_out
                REPORT "Expected s_out = " & std_logic'image(c_stimulus(s).s_out)
                & " but got " & std_logic'image(s_out)
                SEVERITY error;

            -- Assert c_out
            ASSERT c_out = c_stimulus(s).c_out
                REPORT "Expected c_out = " & std_logic'image(c_stimulus(s).c_out)
                & " but got " & std_logic'image(c_out)
                SEVERITY error;

        END LOOP;

        -- Log end of test
        REPORT "Finished"
            SEVERITY note;

        -- Finish the simulation
        std.env.finish;

    END PROCESS pr_stimulus;

END ARCHITECTURE tb;
